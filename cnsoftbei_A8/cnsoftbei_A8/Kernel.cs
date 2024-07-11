using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace cnsoftbei_A8
{
    internal class Kernel
    {
        private static Kernel instance = new Kernel();

        public static Kernel getKernel()
        {
            return instance;
        }
        private Err err;
        private List<RemoteApp> remoteAppList;
        private string hostName;
        public bool isUpdate;
        private string rdpRegistryKeyPath = @"SYSTEM\CurrentControlSet\Control\Terminal Server";
        private string remoteAppRegistryKeyPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Terminal Server\TSAppAllowList\Applications";

        private Kernel() 
        {
            err = Err.getErr();
            remoteAppList = new List<RemoteApp>();
            hostName = "";
            isUpdate = true;
        }

        public void init()
        {
            if (!checkRDP())
            {

            }
            hostName = Environment.MachineName;
            readRemoteAppList();
            err.handle();
        }
        public bool checkRDP()
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(rdpRegistryKeyPath, false);
                if (key != null)
                {
                    int value = (int)key.GetValue("fDenyTSConnections");
                    key.Close();
                    if (value != 0)
                    {
                        // 告诉用户 RDP 未开启，需要开启 RDP
                    }
                    return value == 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking remote desktop status: " + ex.Message);
            }
            return false;
        }
        public void readRemoteAppList()
        {
            remoteAppList.Clear();
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(remoteAppRegistryKeyPath, false);
            if( key != null)
            {
                RegistryKey remoteAppKey;
                foreach (String fullName in key.GetSubKeyNames())
                {
                    remoteAppKey = key.OpenSubKey(fullName, false);
                    remoteAppList.Add(new RemoteApp(remoteAppKey.GetValue("Name") as string, fullName, remoteAppKey.GetValue("Path") as string, remoteAppKey.GetValue("IconPath") as string));
                }
                key.Close();
            }
            else
            {
                err.setErrType(ErrType.GET_RAPP_ERR);
            }
        }
        public void addRemoteApp(string fullName, string path)
        {
            addRemoteAppToRegistry(fullName, path, path);
        }
        public void addRemoteApp(string fullName, string path, string iconPath)
        {
            addRemoteAppToRegistry(fullName, path, iconPath);
        }
        private void addRemoteAppToRegistry(string fullName, string path, string iconPath)
        {
            for(int i = 0; i < remoteAppList.Count; i++)
            {

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
                    remoteAppList.Add(new RemoteApp(name, fullName, path, iconPath));
                    err.setErrType(ErrType.SUCCESS);
                }
                else
                {
                    remoteAppKey.Close();
                    err.setErrType(ErrType.RAPP_EXIST);
                }
                key.Close();
            }
        }
        public void removeApp(string fullName)
        {
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(remoteAppRegistryKeyPath, true);
            if(key == null)
            {
               err.setErrType(ErrType.GET_RAPP_ERR);
            }
            else
            {
                if(key.OpenSubKey(fullName) != null) key.DeleteSubKeyTree(fullName);
                key.Close();
            }
            foreach (RemoteApp remoteApp in remoteAppList)
            {
                if(String.Compare(remoteApp.getFullName(), fullName) == 0)
                {
                    remoteAppList.Remove(remoteApp);
                    break;
                }
            }
        }
        public void openAutoStart()
        {

        }
        public void closeAutoStart()
        {

        }
        public void lockCurrentUser()
        {
            
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
        public string getHostName()
        {
            return hostName;
        }
        public List<RemoteApp> getRemoteAppList()
        {
            return remoteAppList;
        }
    }
}
