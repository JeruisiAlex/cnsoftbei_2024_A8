using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace cnsoftbei_A8
{
    public class Network
    {
        private static Network network = new Network();
        public static Network getNetwork()
        {
            return network;
        }
        private int port;
        private string clientName;
        private int code;

        private Network()
        {
            port = 6789;
            maxConnect = 1;
            connectCount = 0;
            clientName = "还没有被客户端连接";
        }
        public void init()
        {

        }
        public void startServer()
        {
            
        }
        public void stopServer()
        {

        }
        public void connect()
        {
            
        }

        public int getPort() { return port; }
        public string getClientName() { return clientName; }
    }
}
