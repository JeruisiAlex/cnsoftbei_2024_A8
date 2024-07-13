using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class History
    {
        private string ip;
        private string userName;
        private string password;

        public History(string ip, string userName, string password)
        {
            this.ip = ip;
            this.userName = userName;
            this.password = password;
        }

        public string getIp()
        {
            return ip;
        }

        public string getUserName()
        {
            return userName;
        }

        public string getPassword()
        {
            return password;
        }
    }
}
