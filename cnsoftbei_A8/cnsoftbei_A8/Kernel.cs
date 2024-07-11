using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace cnsoftbei_A8
{
    public partial class Kernel
    {
        private static Kernel instance = new Kernel();

        public static Kernel getKernel()
        {
            return instance;
        }
        private Err err;
        private List<RemoteApp> remoteAppList;
        private List<RemoteApp> updateList;
        private List<RemoteApp> removeList;
        private string hostName;
        private string rdpRegistryKeyPath = @"SYSTEM\CurrentControlSet\Control\Terminal Server";
        private string remoteAppRegistryKeyPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Terminal Server\TSAppAllowList\Applications";

        private Kernel() 
        {
            err = Err.getErr();
            remoteAppList = new List<RemoteApp>();
            updateList = new List<RemoteApp>();
            hostName = "";
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
                    updateList.Add(new RemoteApp(remoteAppKey.GetValue("Name") as string, fullName, remoteAppKey.GetValue("Path") as string, remoteAppKey.GetValue("IconPath") as string));
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
        public RemoteApp isAppExist(string fullName)
        {
            for (int i = 0; i < remoteAppList.Count; i++)
            {
                if (String.Compare(remoteAppList[i].getFullName(), fullName) == 0) return remoteAppList[i];
            }
            return null;
        }
        private void addRemoteAppToRegistry(string fullName, string path, string iconPath)
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
                    RemoteApp remoteApp = new RemoteApp(name, fullName, path, iconPath);
                    updateList.Add(remoteApp);
                    remoteAppList.Add(remoteApp);
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
            RemoteApp remoteApp = isAppExist(fullName);
            if (remoteApp != null)
            {
                remoteAppList.Remove(remoteApp);
                removeList.Add(remoteApp);
            }
        }
        private static extern void WTSFreeMemory(IntPtr pointer);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool LockWorkStation();
        public void lockCurrentUser()
        {
            bool result = LockWorkStation();
            if (!result)
            {
                err.setErrType(ErrType.LOCK_USER_FAILED);
            }
        }
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private extern static bool CloseHandle(IntPtr handle);
        public void checkUserInfo(string username, string password)
        {
            IntPtr tokenHandle = IntPtr.Zero;
            // 调用 LogonUser 函数来验证用户名和密码
            bool isSuccess = LogonUser(username, "", password, 2, 0, out tokenHandle);
            if (isSuccess)
            {
                err.setErrType(ErrType.SUCCESS);
                CloseHandle(tokenHandle);
            }
            else
            {
                err.setErrType(ErrType.USER_INFO_ERR);
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
        public string getHostName() { return hostName; }
        public List<RemoteApp> getRemoteAppList() { return remoteAppList; }
        public List<RemoteApp> getUpdateList() { return updateList; }
        public List<RemoteApp> getRemoveList() { return removeList; }
    }
}
