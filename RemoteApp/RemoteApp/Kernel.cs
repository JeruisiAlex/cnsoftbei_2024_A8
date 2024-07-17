using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace SKRO_rdp
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
        private string rappName = "RemoteApp";
        private string serveName = "Server";
        private string remoteAppRegistryKeyPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Terminal Server\TSAppAllowList\Applications";
        private string[] unistallRegistryPaths = new string[]{
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
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(remoteAppRegistryKeyPath, true);
            if (key != null)
            {
                RegistryKey remoteAppKey;
                foreach (String fullName in key.GetSubKeyNames())
                {
                    if (!fullName.Equals(rappName) && !fullName.Equals(serveName))
                    {
                        remoteAppKey = key.OpenSubKey(fullName, true);

                        // 如果是一个应用，则添加到remoteAppList
                        if (Convert.ToInt32(remoteAppKey.GetValue("Type")) == 1)
                        {
                            if(File.Exists(remoteAppKey.GetValue("Path") as string))
                            {
                                App uninstall;
                                string uninstallPath = remoteAppKey.GetValue("UninstallPath") as string;
                                if ("".Equals(uninstallPath))
                                {
                                    uninstall = null;
                                }
                                else
                                {
                                    uninstall = new App(Path.GetFileNameWithoutExtension(uninstallPath), uninstallPath);
                                }

                                App remoteApp = new App(remoteAppKey.GetValue("Name") as string, fullName, remoteAppKey.GetValue("Path") as string, remoteAppKey.GetValue("IconPath") as string, uninstall);
                                remoteAppList.Add(remoteApp);
                            }
                            else
                            {
                                key.DeleteSubKeyTree(fullName);
                            }
                        }
                        // 如果是卸载程序或安装程序，则从注册表移除
                        else
                        {
                            removeAppFromRegistry(fullName);
                        }

                    }
                }
                key.Close();
                err.setErrType(ErrType.SUCCESS);
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
            foreach (string regPath in unistallRegistryPaths)
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
                            
                            return new App(getName(name), name, path);
                        }
                    }
                }
            }
            
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
            for (int i = 0; i < installList.Count; i++)
            {
                removeAppFromRegistry(installList[i].getFullName());
            }

            err.handle();
        }

        /* **********************   下面的函数供 pinpin 调用   ************************/
        
        /* 
         * 功能：添加应用到远程应用
         * flag说明：
         *  0:表示这是一个卸载程序，可以重新发布
         *  1;表示这是应用，不能够重复发布
         *  2:表示这是一个安装程序，可以重复发布
         */
        private void addRemoteAppToRegistry(string fullName, string path, string iconPath, string uninstallPath ,int flag)
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
                    newKey.SetValue("UninstallPath", uninstallPath, RegistryValueKind.String);
                    newKey.SetValue("Type",flag, RegistryValueKind.DWord);
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
            if (isAppExist(fullName) == null && !fullName.Equals(rappName) && !fullName.Equals(serveName))
            {
                // 找到卸载程序
                App uninstall = getUninstall(fullName);
                err.setErrType(ErrType.SUCCESS);

                App app;
                app = new App(getName(fullName), fullName, path, path, uninstall);

                if (uninstall != null)
                {
                    addRemoteAppToRegistry(fullName, path, path, uninstall.getPath(), 1);
                }
                else
                {
                    addRemoteAppToRegistry(fullName, path, path, "", 1);
                }

                remoteAppList.Add(app);
                
            }
            else if (fullName.Equals(rappName) || fullName.Equals(serveName))
            {
                err.setErrType(ErrType.CNOT_SEND_RAPP);
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
            if (isAppExist(fullName) == null && !fullName.Equals(rappName) && !fullName.Equals(serveName))
            {
                // 找到卸载程序
                App uninstall = getUninstall(fullName);
                err.setErrType(ErrType.SUCCESS);

                App app;
                app = new App(getName(fullName), fullName, path, iconPath, uninstall);

                if (uninstall != null)
                {
                    addRemoteAppToRegistry(fullName, path, iconPath, uninstall.getPath(), 1);
                }
                else
                {
                    addRemoteAppToRegistry(fullName, path, iconPath, "", 1);
                }

                remoteAppList.Add(app);

            }
            else if (fullName.Equals(rappName) || fullName.Equals(serveName))
            {
                err.setErrType(ErrType.CNOT_SEND_RAPP);
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
            if (app != null && app.getUninstall() != null)
            {
                addRemoteAppToRegistry(app.getUninstall().getFullName(), app.getUninstall().getPath(), app.getUninstall().getIconPath(), "",0);
                uninstallList.Add(app.getUninstall());
                network.send(1, app.getUninstall().getName());
                removeAppFromRegistry(fullname);
                removeAppFromList(fullname);
                MouseActionFactory.MouseActionFactory.Instance.flushAppPanel(remoteAppList, new Size(Form1.screenWidth, Form1.screenHeight)); 
                err.setErrType(ErrType.SUCCESS);
            }
            else
            {
                err.setErrType(ErrType.CAN_NOT_UNINSTALL);
            }
            err.handle();
        }
        // 移除已发布应用
        public void removeApp(string fullName)
        {
            App app = isAppExist(fullName);
            removeAppFromList(fullName);
            removeAppFromRegistry(fullName);
            err.handle();
        }

        // 安装
        public void installApp(string fullName, string path)
        {
            App app = new App(fullName, path);
            installList.Add(app);
            addRemoteAppToRegistry(fullName, path, path, "",2);

            if (err.getErrType() == ErrType.SUCCESS)
            {
                network.send(1, app.getName());
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
                        network.send(1,app.getName());
                    }
                    else
                    {
                        remoteAppList.Remove(app);
                        removeAppFromRegistry(app.getFullName());
                        MouseActionFactory.MouseActionFactory.Instance.flushAppPanel(remoteAppList,new Size(Form1.screenWidth,Form1.screenHeight));
                        MessageBox.Show("该应用已迁移，请重新发布应用");
                    }
                }
                else
                {
                    MessageBox.Show("获取应用列表或已发布应用列表失败");
                }
            }
        }


        public List<App> getRemoteAppList() { return remoteAppList; }
    }
}
