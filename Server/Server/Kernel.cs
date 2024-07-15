using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Win32;

namespace Server
{
    public class Kernel
    {
        private static Kernel instance = new Kernel();

        public static Kernel getKernel()
        {
            return instance;
        }

        private string hostName;
        public List<History> histories;
        private MouseActionFactory.MouseActionFactory mouseAction;
        private string historiesPath = "./history.json"; // 历史记录的路径
        private string rappFullName = "RemoteApp"; // remoteApp.exe 的全称
        private string rappPath = "./RemoteApp.exe"; // remoteApp.exe 的路径
        public string tutorialPath; // 启用远程桌面教程的路径
        private string rdpRegistryKeyPath = @"SYSTEM\CurrentControlSet\Control\Terminal Server";
        private string remoteAppRegistryKeyPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Terminal Server\TSAppAllowList\Applications";

        private Kernel()
        {
            hostName = "";
            histories = new List<History>();
        }

        public void init()
        {
            hostName = System.Net.Dns.GetHostName();
            if(hostName == null || hostName == "")
            {
                hostName = "没有找到主机名";
            }
            checkRemoteApp();
            readHistories();
            // 获取当前工作目录
            string currentDirectory = Directory.GetCurrentDirectory();
            // 将相对路径转换为绝对路径
            tutorialPath = Path.GetFullPath(Path.Combine(currentDirectory, "./启用远程桌面.pdf"));
            mouseAction = MouseActionFactory.MouseActionFactory.Instance;
            if (!checkRDP())
            {
                // 错误框,错误框关闭，则整个程序关闭
                mouseAction.errPDF("没有启用远程桌面！请手动启用。\n是否参考启用远程桌面教程？");
                return;
            }
            if (!File.Exists(rappPath))
            {
                // 错误框,错误框关闭，则整个程序关闭
                mouseAction.errMessage("没有找到必要程序，请重新安装");
                return ;
            }
;
        }

        // 检查 rdp 是否打开
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
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking remote desktop status: " + ex.Message);
            }
            return false;
        }

        // 从注册表中读 remoteApp.exe 注册表是否存在。如果不存在则创建
        public void checkRemoteApp()
        {
            string name = getName(rappFullName);
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(remoteAppRegistryKeyPath, true);
            if(key != null)
            {
                RegistryKey remoteAppKey = key.OpenSubKey(rappFullName, true);
                if (remoteAppKey == null)
                {
                    remoteAppKey = key.CreateSubKey(rappFullName, true);
                }
                // 用户可能迁移这个应用，所以每一次应该重新加一遍
                remoteAppKey.SetValue("Name", name, RegistryValueKind.String);
                remoteAppKey.SetValue("FullName", rappFullName, RegistryValueKind.String);
                remoteAppKey.SetValue("Path", Path.GetFullPath(rappPath), RegistryValueKind.String);
                remoteAppKey.SetValue("IconPath", Path.GetFullPath(rappPath), RegistryValueKind.String);
                remoteAppKey.SetValue("UninstallPath", "", RegistryValueKind.String);
                remoteAppKey.SetValue("Type", 1, RegistryValueKind.DWord);
                remoteAppKey.Close();
                key.Close();
            }
        }

        // 读取历史连接记录
        public void readHistories()
        {
            try
            {
                if (File.Exists(historiesPath))
                {
                    string jsonString = File.ReadAllText(historiesPath);
                    histories = JsonSerializer.Deserialize<List<History>>(jsonString);
                }
            }

            catch (JsonException ex)
            {
                // 处理JSON解析错误
                Console.WriteLine($"JSON 解析错误: {ex.Message}");
            }
            catch (Exception ex)
            {
                // 处理其他错误
                Console.WriteLine($"读取或反序列化期间发生错误: {ex.Message}");
            }
        }
        
        // 写入历史记录。在Form1关闭的时候调用。其他时候，pinpin只需要改动 histories 这个 List
        public void writeHistories()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(histories, options);
                File.WriteAllText(historiesPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving history: {ex.Message}");
            }
        }

        // 弹出pdf
        public void OpenTutorial()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = tutorialPath,
                    UseShellExecute = true,
                    Verb = "open"
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("无法打开文件: " + ex.Message);
            }
        }

        private static extern void WTSFreeMemory(IntPtr pointer);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool LockWorkStation();
        public bool lockCurrentUser()
        {
            return LockWorkStation();
        }
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private extern static bool CloseHandle(IntPtr handle);
        public bool checkUserInfo(string username, string password)
        {
            IntPtr tokenHandle = IntPtr.Zero;
            // 调用 LogonUser 函数来验证用户名和密码
            bool isSuccess = LogonUser(username, "", password, 2, 0, out tokenHandle);
            if (isSuccess)
            {
                CloseHandle(tokenHandle);
                return true;
            }
            else
            {
                return false;
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
        public string getHostName()
        {
            return System.Net.Dns.GetHostName();
        }
    }
}
