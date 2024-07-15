using System;
using System.Collections.Generic;
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

        public bool isConnect;
        public Mutex isConnectMutex;

        private Kernel kernel;

        private Network() {
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
            mainThread.Start();
        }
        public void stop()
        {
            mainMutex.WaitOne();
            isRun = false;
            mainListener.Stop();
            mainMutex.ReleaseMutex();
            mainThread.Join();
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
                    client = mainListener.AcceptTcpClient();
                    isConnectMutex.WaitOne();
                    isConnect = true;
                    isConnectMutex.ReleaseMutex();
                    if (client != null)
                    {
                        NetworkStream stream = client.GetStream();
                        if (checkUserInfo(stream))
                        {
                            kernel.lockCurrentUser();
                            choosePort(stream);
                            serverListener = new TcpListener(port);
                            serverListener.Start();
                            serverThread = new Thread(new ThreadStart(serverStart));
                            serverThread.Start();

                            sendHostName(stream);
                            isConnectMutex.WaitOne();
                            kernel.histories.Add(new History((client.Client.RemoteEndPoint as IPEndPoint).ToString(), clientName, username));
                            MouseActionFactory.MouseActionFactory.Instance.showConnection(clientName);
                            isConnectMutex.ReleaseMutex();
                        }
                        else
                        {
                            isConnectMutex.WaitOne();
                            isConnect = false;
                            isConnectMutex.ReleaseMutex();
                        }
                        if(stream != null) stream.Close();
                        client.Close();
                    }
                }
                catch (Exception e)
                {
                    
                }
                mainMutex.WaitOne();
            }
            mainMutex.ReleaseMutex();
        }
        private void serverStart()
        {
            try
            {
                TcpClient client = serverListener.AcceptTcpClient();
                if (client != null)
                {
                    NetworkStream stream = client.GetStream();
                    string name = "RemoteApp";
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
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("error1");
                serverListener.Stop();
                serverListener.Dispose();

                isConnectMutex.WaitOne();
                isConnect = false;
                isConnectMutex.ReleaseMutex();
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
            if (kernel.checkUserInfo(username, password) && isConnect == false)
            {
                sendInt(stream, 0);
            }
            else if (isConnect == true)
            {
                sendInt(stream, 3);
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
            Debug.WriteLine(port);
        } 
        private void sendHostName(NetworkStream stream)
        {
            int length = receiveInt(stream);
            clientName = receiveString(stream, length);
            string hostname = kernel.getHostName();
            Debug.WriteLine(hostname);
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
