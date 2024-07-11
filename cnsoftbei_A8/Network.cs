using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace cnsoftbei_A8
{
    internal class Network
    {
        private static Network instance = new Network();
        public static Network getNetwork()
        {
            return instance;
        }
        private int port;
        private int maxConnect;
        private int connectCount;
        private string clientName;

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
            try
            {

            }
            catch (Exception ex)
            {

            }
        }
        public void stopServer()
        {

        }
        public void connect()
        {
            
        }

        public int getPort()
        {
            return port;
        }
        public int getMaxConnect()
        {
            return maxConnect;
        }
        public int getConnectCount()
        {
            return connectCount;
        }
    }
}
