using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.ServiceProcess;
using System.Diagnostics;

namespace cnsoftbei_A8
{
    internal class Kernel
    {
        private static Kernel instance = new Kernel();

        public static Kernel getKernel()
        {
            return instance;
        }
        private List<App> appList;
        private List<RemoteApp> remoteAppList;
        private string hostName;

        private Kernel() { 

        }

        public void init()
        {
            if (!checkRDP()) ;
        }
        public bool checkRDP()
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Terminal Server", false);
                if (key != null)
                {
                    int value = (int)key.GetValue("fDenyTSConnections");
                    key.Close();
                    return value == 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking remote desktop status: " + ex.Message);
            }
            return false;
        }
       
    }
}
