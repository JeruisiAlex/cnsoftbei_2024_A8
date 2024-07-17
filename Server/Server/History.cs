using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKRO_server
{
    public class History
    {
        public string ip { get; set; }
        public string userName { get; set; }

        public string hostName { get; set; }

        public string connectTime { get; set; }


        public History(string ip, string hostName,string userName)
        {
            this.ip = ip;
            this.userName = userName;
            this.hostName = hostName;
            connectTime = DateTime.Now.ToString();
        }
    }
}
