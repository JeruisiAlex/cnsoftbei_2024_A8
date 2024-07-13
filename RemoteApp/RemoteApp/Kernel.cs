﻿using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace RemoteApp
{
    public partial class Kernel
    {
        private static Kernel instance = new Kernel();

        public static Kernel getKernel()
        {
            return instance;
        }
        private Err err;
        private Network network;
        private List<App> remoteAppList;
        private List<App> installList;
        private List<App> uninstallList;
        private string rdpRegistryKeyPath = @"SYSTEM\CurrentControlSet\Control\Terminal Server";
        private string remoteAppRegistryKeyPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Terminal Server\TSAppAllowList\Applications";
        private string[] UnistallRegistryPaths = new string[]{
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"};

        private Kernel()
        {
            err = Err.getErr();
            network = Network.getNetwork();
            remoteAppList = new List<App>();
            installList = new List<App>();
            uninstallList = new List<App>();
        }

        public void init()
        {
            readRemoteAppList();
            err.handle();
        }
        
        // 读取远程应用列表。（同时会读取到各个应用列表的卸载程序）
        private void readRemoteAppList()
        {
            remoteAppList.Clear();
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(remoteAppRegistryKeyPath, false);
            if (key != null)
            {
                RegistryKey remoteAppKey;
                foreach (String fullName in key.GetSubKeyNames())
                {
                    remoteAppKey = key.OpenSubKey(fullName, false);

                    App uninstall = getUninstall(fullName);
                    if (uninstall != null) uninstallList.Add(uninstall);

                    App remoteApp = new App(remoteAppKey.GetValue("Name") as string, fullName, remoteAppKey.GetValue("Path") as string, remoteAppKey.GetValue("IconPath") as string, uninstall);
                    remoteAppList.Add(remoteApp);

                }
                key.Close();
            }
            else
            {
                err.setErrType(ErrType.GET_RAPP_ERR);
            }
        }

        // 检查注册表和App的路径等是否相同，若不相同，则修改App的路径
        private void checkAppInRegistry(App app)
        {
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(remoteAppRegistryKeyPath, false);
            if (key != null)
            {
                RegistryKey remoteAppKey = key.OpenSubKey(app.getFullName(), false);

                if(remoteAppKey != null)
                {
                    app.setName(remoteAppKey.GetValue("Name") as string);
                    app.setPath(remoteAppKey.GetValue("Path") as string);
                    app.setIconPath(remoteAppKey.GetValue("IconPath") as string);
                }
                else
                {
                    err.setErrType(ErrType.RAPP_NOT_EXIST);
                }

                key.Close();
            }
            else
            {
                err.setErrType(ErrType.GET_RAPP_ERR);
            }
        }

        // 从本地远程应用列表中根据fullName查找App
        private App isAppExist(string fullName)
        {
            for (int i = 0; i < remoteAppList.Count; i++)
            {
                if (String.Compare(remoteAppList[i].getFullName(), fullName) == 0) return remoteAppList[i];
            }
            return null;
        }

        /* 
         * 功能：查找应用的卸载程序。
         * 参数：需要查找的应用名称
         * 注意：使用后 err.handle()
         * 返回值：
         *  * null
         *  * 卸载程序的 App
        */
        private App getUninstall(string fullName)
        {
            //遍历注册表
            foreach (string regPath in UnistallRegistryPaths)
            {
                using RegistryKey? key = Registry.LocalMachine.OpenSubKey(regPath);
                if (key != null)
                {
                    foreach (string subKeyName in key.GetSubKeyNames())
                    {
                        using RegistryKey? subKey = key.OpenSubKey(subKeyName);
                        string? displayName = subKey?.GetValue("DisplayName") as string;
                        if (displayName != null && displayName.IndexOf(fullName, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            string path = subKey.GetValue("UninstallString") as string;
                            string name = Path.GetFileNameWithoutExtension(path);
                            err.setErrType(ErrType.SUCCESS);
                            return new App(getName(name), name, path);
                        }
                    }
                }
            }
            err.setErrType(ErrType.CAN_NOT_UNINSTALL);
            return null;
        }

        // 从注册表中移除该应用
        private void removeAppFromRegistry(string fullName)
        {
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(remoteAppRegistryKeyPath, true);
            if (key == null)
            {
                err.setErrType(ErrType.GET_RAPP_ERR);
            }
            else
            {
                if (key.OpenSubKey(fullName) != null) key.DeleteSubKeyTree(fullName);
                key.Close();
                err.setErrType(ErrType.SUCCESS);
            }
        }

        public void removeAppFromList(string fullName)
        {
            App remoteApp = isAppExist(fullName);
            if (remoteApp != null)
            {
                remoteAppList.Remove(remoteApp);
            }
        }

        private string getName(string fullName)
        {
            Regex rx = new Regex(@"[^\p{L}0-9\-_"" ]");
            string name = fullName;
            if (rx.IsMatch(fullName))
            {
                name = rx.Replace(fullName, string.Empty);
            }
            name = name.Trim();
            return name;
        }

        // 从注册表中移除所有的安装程序和卸载程序
        public void removeUninstallAndInstall()
        {
            for(int i = 0; i < uninstallList.Count; i++)
            {
                removeAppFromRegistry(uninstallList[i].getFullName());
            }
            for(int i = 0; i < installList.Count; i++)
            {
                removeAppFromRegistry(installList[i].getFullName());
            }
        }

        /* **********************   下面的函数供 pinpin 调用   ************************/
        
        /* 
         * 功能：添加应用到远程应用
         * flag说明：
         *  0:表示这是一个卸载程序，可以重新发布
         *  1;表示这是应用，不能够重复发布
         *  2:表示这是一个安装程序，可以重复发布
         */
        private void addRemoteAppToRegistry(string fullName, string path, string iconPath, int flag)
        {
            if (isAppExist(fullName) != null)
            {
                err.setErrType(ErrType.RAPP_EXIST);
                return;
            }
            string name = getName(fullName);
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(remoteAppRegistryKeyPath, true);
            if (key == null)
            {
                err.setErrType(ErrType.GET_RAPP_ERR);
            }
            else
            {
                RegistryKey remoteAppKey = key.OpenSubKey(fullName, false);
                if (remoteAppKey == null)
                {
                    RegistryKey newKey = key.CreateSubKey(fullName, true);
                    newKey.SetValue("Name", name, RegistryValueKind.String);
                    newKey.SetValue("FullName", fullName, RegistryValueKind.String);
                    newKey.SetValue("Path", path, RegistryValueKind.String);
                    newKey.SetValue("IconPath", iconPath, RegistryValueKind.String);
                    err.setErrType(ErrType.SUCCESS);
                }
                else if(flag == 1)
                {
                    remoteAppKey.Close();
                    err.setErrType(ErrType.RAPP_EXIST);
                }
                else
                {
                    err.setErrType(ErrType.SUCCESS);
                }
                key.Close();
            }
        }

        
        // 发布远程应用
        public void addRemoteApp(string fullName, string path)
        {
            if (isAppExist(fullName) == null)
            {
                // 找到卸载程序
                App uninstall = getUninstall(fullName);
                err.setErrType(ErrType.SUCCESS);
                uninstallList.Add(uninstall);
                App app;
                app = new App(getName(fullName), fullName, path, path, uninstall);

                if (uninstall != null)
                {
                    uninstallList.Add(uninstall);
                }

                remoteAppList.Add(app);
                addRemoteAppToRegistry(fullName, path, path, 1);
            }
            else
            {
                err.setErrType(ErrType.RAPP_EXIST);
            }
            err.handle();
        }
        // 发布远程应用
        public void addRemoteApp(string fullName, string path, string iconPath)
        {
            if (isAppExist(fullName) == null)
            {
                // 找到卸载程序
                App uninstall = getUninstall(fullName);
                err.setErrType(ErrType.SUCCESS);
                uninstallList.Add(uninstall);
                App app;
                app = new App(getName(fullName), fullName, path, iconPath, uninstall);

                if (uninstall != null)
                {
                    uninstallList.Add(uninstall);
                }

                remoteAppList.Add(app);
                addRemoteAppToRegistry(fullName, path, iconPath, 1);
            }
            else
            {
                err.setErrType(ErrType.RAPP_EXIST);
            }
            err.handle();
        }

        // 卸载已发布应用
        public void uninstallApp(string fullname)
        {
            App app = isAppExist(fullname);
            if (app != null && app.getUninstall != null)
            {
                addRemoteAppToRegistry(app.getUninstall().getFullName(), app.getUninstall().getPath(), app.getUninstall().getIconPath(), 0);

                network.send(0, app.getUninstall().getName());

                // 如果卸载成功，移除这卸载程序和发布应用
                if (!File.Exists(app.getPath()))
                {
                    removeAppFromRegistry(fullname);
                    removeAppFromRegistry(app.getUninstall().getFullName());
                    uninstallList.Remove(app.getUninstall());
                    removeAppFromList(fullname);
                }
                err.setErrType(ErrType.SUCCESS);
            }
            else
            {
                err.setErrType(ErrType.CAN_NOT_UNINSTALL);
            }
        }
        // 移除已发布应用
        public void removeApp(string fullName)
        {
            App app = isAppExist(fullName);
            if (app != null && app.getUninstall() != null)
            {
                uninstallList.Remove(app.getUninstall());
                removeAppFromRegistry(app.getUninstall().getFullName());
            }
            removeAppFromList(fullName);
            removeAppFromRegistry(fullName);
            err.handle();
        }

        // 安装
        public void installApp(string fullName, string path)
        {
            App app = new App(fullName, path);
            installList.Add(app);
            addRemoteAppToRegistry(fullName, path, path, 2);
            if (err.getErrType() == ErrType.SUCCESS)
            {
                network.send(0, app.getName());
            }
            else
            {
                err.handle();
            }
        }

        // 打开远程应用
        public void openApp(string fullName)
        {
            App app = isAppExist(fullName);
            if (app != null)
            {
                checkAppInRegistry(app);
                if(err.getErrType() == ErrType.SUCCESS)
                {
                    if (File.Exists(app.getPath()))
                    {
                        network.send(0,app.getName());
                    }
                    else
                    {
                        remoteAppList.Remove(app);
                        removeAppFromRegistry(app.getFullName());
                        err.setErrType(ErrType.RAPP_NOT_IN_PATH);
                    }
                }
                else
                {
                    err.handle();
                }
            }
        }


        public List<App> getRemoteAppList() { return remoteAppList; }
    }
}