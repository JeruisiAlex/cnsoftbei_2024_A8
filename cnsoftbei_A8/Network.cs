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
        private Network(){ }
        public void startServer()
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }
        public void connect()
        {
            TcpListener server = null;
            try
            {
                // 设置服务器监听的IP地址和端口
                IPAddress localAddr = IPAddress.Parse("0.0.0.0"); // 监听所有IP地址
                int port = 6789;
                server = new TcpListener(localAddr, port);

                // 开始监听客户端连接
                server.Start();
                Console.WriteLine("Waiting for a connection...");

                // 接受客户端连接
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");

                // 获取网络流
                NetworkStream stream = client.GetStream();

                // 接收来自客户端的数据
                byte[] buffer = new byte[256];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received: {0}", message);

                // 发送数据到客户端
                string response = "Hello from C# server";
                byte[] data = Encoding.ASCII.GetBytes(response);
                stream.Write(data, 0, data.Length);
                Console.WriteLine("Sent: {0}", response);

                // 关闭流和客户端
                stream.Close();
                client.Close();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e.Message);
            }
            finally
            {
                // 停止服务器
                if (server != null)
                {
                    server.Stop();
                }
            }

            Console.Read();
        }
    }
}
