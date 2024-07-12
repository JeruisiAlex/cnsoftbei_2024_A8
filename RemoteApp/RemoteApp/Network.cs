using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RemoteApp
{
    public class Network
    {
        private static Network instance = new Network();
        public static Network getNetwork() { return instance; }
        private int port = 5678;
        private bool isRun;
        public Mutex mutex;
        public Mutex stateMutex;
        private Thread thread;
        private TcpListener server;
        private TcpClient client;
        private NetworkStream stream;
        private NetworkState networkState;
        private Queue<string> messages;
        private int code;
        
        private Network() {
            isRun = true;
            mutex = new Mutex();
            stateMutex = new Mutex();
            thread = new Thread(new ThreadStart(connect));
            networkState = NetworkState.Waiting;
            messages = new Queue<string>();
        }
        public void init()
        {
            mutex.WaitOne();
            thread.Start();
        }
        private void connect()
        {
            try
            {
                server = new TcpListener(port);
                server.Start(); 
                client = server.AcceptTcpClient();
                if (isRun)
                {
                    stateMutex.WaitOne();
                    networkState = NetworkState.Connected;
                    stateMutex.ReleaseMutex();

                    stream = client.GetStream();
                    run();
                    stream.Close();
                    client.Close();
                }
                Debug.WriteLine("通信服务线程结束");
            }
            catch (Exception e)
            {
                
            }
        }
        public void run()
        {
            stateMutex.WaitOne();
            while (isRun)
            {
                stateMutex.ReleaseMutex();

                mutex.WaitOne();
                string name = messages.Dequeue();
                int length = name.Length;
                byte[] data = Encoding.UTF8.GetBytes(code+length+name);
                stream.Write(data);
                mutex.ReleaseMutex();

                stateMutex.WaitOne();
            }
            stateMutex.ReleaseMutex();
        }
        public void send(int code, string name)
        {
            this.code = code;
            messages.Enqueue(name);
            mutex.ReleaseMutex();
            mutex.WaitOne();
        }
        public void stop()
        {
            stateMutex.WaitOne();
            isRun = false;
            if (networkState == NetworkState.Waiting)
            {
                server.Stop();
            }
            else if(networkState == NetworkState.Connected)
            {
                send(0, "");
            }
            stateMutex.ReleaseMutex();
            mutex.ReleaseMutex();

            release();
        }
        private void release()
        {
            thread.Join();
            mutex.ReleaseMutex();
            stateMutex.ReleaseMutex();
            server.Dispose();
            messages.Clear();
        }

        public int getPort() { return port; }
        public Thread getThread() { return thread; }
    }
}
