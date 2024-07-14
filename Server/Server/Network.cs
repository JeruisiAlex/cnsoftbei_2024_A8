using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public bool isConnect;
        public Mutex isConnectMutex;

        private Kernel kernel;

        private Network() {
            mainThread = new Thread(new ThreadStart(mainStart));
            serverThread = new Thread(new ThreadStart(serverStart));
            mainListener = new TcpListener(serverPort);
            mainMutex = new Mutex();
            isRun = true;

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
            if (!checkPort(remoteAppPort))
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
            Debug.WriteLine("开启主线程");
            mainThread.Start();
        }
        public void stop()
        {
            Debug.WriteLine("关闭主线程");
            mainMutex.WaitOne();
            isRun = false;
            mainListener.Stop();
            mainMutex.ReleaseMutex();
            mainThread.Join();
        }

        private void mainStart()
        {
            try
            {
                mainMutex.WaitOne();
                while (isRun)
                {
                    mainMutex.ReleaseMutex();
                    mainListener.Start();
                    TcpClient client = null;
                    Debug.WriteLine("开启等待数据");
                    client = mainListener.AcceptTcpClient();
                    Debug.WriteLine("开始接受数据");
                    if (client != null)
                    {
                        NetworkStream stream = client.GetStream();
                        if (isConnect == false && checkUserInfo(stream))
                        {
                            choosePort(stream);

                            isConnectMutex.WaitOne();
                            isConnect = true;
                            kernel.lockCurrentUser();
                            isConnectMutex.ReleaseMutex();

                            serverListener = new TcpListener(port);
                            serverListener.Start();
                            serverThread.Start();

                            sendHostName(stream);
                            isConnectMutex.WaitOne();
                            kernel.histories.Add(new History((client.Client.RemoteEndPoint as IPEndPoint).ToString(), clientName, username));
                            isConnectMutex.ReleaseMutex();
                        }
                        else
                        {
                            sendInt(stream, 0);
                        }
                        if(stream != null) stream.Close();
                        client.Close();
                    }
                    mainMutex.WaitOne();
                }
                mainMutex.ReleaseMutex();
            }
            catch (Exception e)
            {
                
            }
        }
        private void serverStart()
        {
            TcpClient client = serverListener.AcceptTcpClient();
            if (client != null)
            {
                NetworkStream stream = client.GetStream();
                string name = "RemoteApp";
                sendInt(stream, name.Length);
                sendString(stream, name);
                receiveInt(stream);
            }
        }
        private bool checkUserInfo(NetworkStream stream)
        {
            int length = receiveInt(stream);
            username = receiveString(stream, length);
            Debug.WriteLine(username);
            length = receiveInt(stream);
            string password = receiveString(stream, length);
            Debug.WriteLine(password);
            if (kernel.checkUserInfo(username, password))
            {
                sendInt(stream, 0);
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
            Debug.WriteLine(hostname);
            sendInt(stream, hostname.Length);
            sendString(stream, hostname);
        }
        private void sendString(NetworkStream stream, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
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
