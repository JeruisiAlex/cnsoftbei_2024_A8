using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class History
    {
        public string ip { get; set; }
        public string userName { get; set; }
        public string password { get; set; }

        public History(string ip, string userName, string password)
        {
            this.ip = ip;
            this.userName = userName;
            this.password = password;
        }
    }
}
