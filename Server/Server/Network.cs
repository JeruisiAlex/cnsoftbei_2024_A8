using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server
{
    public class Network
    {
        private static Network network = new Network();
        public static Network getNetwork() { return network; }
        private int serverPort = 6789;
        private int remoteAppPort = 5678;
        private int port;
        private string clientName;
        private string username;

        private Thread mainThread;
        private Thread serverThread;
        private TcpListener mainListener;
        private TcpListener serverListener;
        private Mutex mainMutex;
        private bool isRun;
        private bool isLimit;
        private Mutex isLimitMutex;

        public bool isConnect;
        public Mutex isConnectMutex;

        private Kernel kernel;

        private Network() {
            mainListener = new TcpListener(serverPort);
            mainMutex = new Mutex();
            isRun = true;
            isLimit = false;
            isLimitMutex = new Mutex();

            isConnect = false;
            isConnectMutex = new Mutex();

            kernel = Kernel.getKernel();
        }

        public void init()
        {
            if (!checkPort(serverPort))
            {
                return;
            }
            start();
        }

        private bool checkPort(int port)
        {
            bool isAvailable = true;

            TcpListener listener = new TcpListener(IPAddress.Any, port);

            try
            {
                listener.Start();
            }
            catch (SocketException)
            {
                isAvailable = false;
            }
            finally
            {
                // 停止TcpListener
                listener.Stop();
            }

            return isAvailable;
        }
        public void start()
        {
            mainThread = new Thread(new ThreadStart(mainStart));
            isRun = true;
            mainThread.Start();
        }
        public void stop()
        {
            mainMutex.WaitOne();
            isRun = false;
            mainMutex.ReleaseMutex();
            mainListener.Stop();
            Debug.WriteLine("完成关闭");
        }

        private void mainStart()
        {
            mainMutex.WaitOne();
            while (isRun)
            {
                mainMutex.ReleaseMutex();
                try
                {
                    mainListener.Start();
                    TcpClient client = null;
                    Debug.WriteLine("开始等待数据");
                    client = mainListener.AcceptTcpClient();
                    Debug.WriteLine("已被连接");
                    if (client != null)
                    {
                        isConnectMutex.WaitOne();
                        isConnect = true;
                        isConnectMutex.ReleaseMutex();
                        NetworkStream stream = client.GetStream();
                        Debug.WriteLine("连接成功");
                        if (checkUserInfo(stream))
                        {
                            Debug.WriteLine("完成用户信息检查");
                            kernel.lockCurrentUser();
                            choosePort(stream);
                            Debug.WriteLine("完成端口检查");
                            serverListener = new TcpListener(port);
                            serverListener.Start();
                            serverThread = new Thread(new ThreadStart(serverStart));
                            serverThread.Start();

                            sendHostName(stream);
                            Debug.WriteLine("完成主机信息互换");
                            kernel.histories.Add(new History((client.Client.RemoteEndPoint as IPEndPoint).ToString(), clientName, username));
                        }
                        if(stream != null) stream.Close();
                        client.Close();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("连接被停止");
                }
                mainMutex.WaitOne();
            }
            mainMutex.ReleaseMutex();
            Debug.WriteLine("主线程结束");
        }
        private void serverStart()
        {
            try
            {
                TcpClient client = serverListener.AcceptTcpClient();
                if (client != null)
                {
                    NetworkStream stream = client.GetStream();
                    string name = kernel.getRppName(); ;
                    sendString(stream, name);
                    receiveInt(stream);
                    Debug.WriteLine("error1");
                    stream.Close();
                    client.Close();
                    serverListener.Stop();
                    serverListener.Dispose();

                    isConnectMutex.WaitOne();
                    isConnect = false;
                    isConnectMutex.ReleaseMutex();

                    isLimitMutex.WaitOne();
                    isLimit = false;
                    isLimitMutex.ReleaseMutex();
                    Debug.WriteLine("服务线程结束1");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("error2");
                serverListener.Stop();
                serverListener.Dispose();

                isConnectMutex.WaitOne();
                isConnect = false;
                isConnectMutex.ReleaseMutex();

                isLimitMutex.WaitOne();
                isLimit = false;
                isLimitMutex.ReleaseMutex();
                Debug.WriteLine("服务线程结束2");
            }
        }
        private bool checkUserInfo(NetworkStream stream)
        {
            int length = receiveInt(stream);
            username = receiveString(stream, length);
            length = receiveInt(stream);
            string password = receiveString(stream, length);
            if (kernel.checkUserInfo(username, password) && isLimit == false)
            {
                isLimitMutex.WaitOne();
                isLimit = true;
                isLimitMutex.ReleaseMutex();
                sendInt(stream, 0);
            }
            else if (isLimit == true)
            {
                sendInt(stream, 3);
                isConnectMutex.WaitOne();
                isConnect = false;
                isConnectMutex.ReleaseMutex();
                return false;
            }
            else
            {
                sendInt(stream, 2);
                return false;
            }
            return true;
        }
        private void choosePort(NetworkStream stream)
        {
            while (true)
            {
                port = receiveInt(stream);
                if (checkPort(port))
                {
                    sendInt(stream, 0);
                    break;
                }
                else
                {
                    sendInt(stream, 1);
                }
            }
        } 
        private void sendHostName(NetworkStream stream)
        {
            int length = receiveInt(stream);
            clientName = receiveString(stream, length);
            string hostname = kernel.getHostName();
            sendString(stream, hostname);
        }
        private void sendString(NetworkStream stream, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            int length = data.Length;
            sendInt(stream, length);
            stream.Write(data);
        }
        private void sendInt(NetworkStream stream, int number)
        {
            byte[] data = BitConverter.GetBytes(number);
            stream.Write(data);
        }
        private string receiveString(NetworkStream stream, int length)
        {
            byte[] data = new byte[length];
            length = stream.Read(data, 0, length);
            string message = Encoding.UTF8.GetString(data);
            return message;
        }
        private int receiveInt(NetworkStream stream)
        {
            byte[] data = new byte[4];
            stream.Read(data, 0 ,4);
            int number = BitConverter.ToInt32(data);
            return number;
        }
    }
}
