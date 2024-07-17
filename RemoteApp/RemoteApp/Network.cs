﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SKRO_rdp
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
        private string message;
        private int code;
        private string clientName;
        private bool isShare;
        
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
            isRun = true;
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
                    /*readClientName();*/

                    int share = receiveInt(stream);
                    if(share == 1)
                    {
                        isShare = true;
                    }
                    else
                    {
                        isShare = false;
                    }
                    run();
                    stateMutex.WaitOne();
                    networkState = NetworkState.Disconnected;
                    stateMutex.ReleaseMutex();
                    stream.Close();
                    client.Close();
                }
            }
            catch (Exception e)
            {
                
            }
        }
        private void readClientName()
        {
            byte[] buffer = new byte[1024];
            int length = stream.Read(buffer, 0, buffer.Length);
            clientName = Encoding.UTF8.GetString(buffer, 0, length);
        }
        private void run()
        {
            stateMutex.WaitOne();
            while (isRun)
            {
                stateMutex.ReleaseMutex();

                mutex.WaitOne();
                /*string name = messages.Dequeue();*/
                string name = message;
                sendInt(stream, code);
                sendString(stream, name);
                mutex.ReleaseMutex();

                stateMutex.WaitOne();
            }
            stateMutex.ReleaseMutex();
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
        private int receiveInt(NetworkStream stream)
        {
            byte[] data = new byte[4];
            stream.Read(data, 0, 4);
            int number = BitConverter.ToInt32(data);
            return number;
        }
        public void send(int code, string name)
        {
            this.code = code;
            message = name;
            /*messages.Enqueue(name);*/
            mutex.ReleaseMutex();
            mutex.WaitOne();
        }
        public void stop()
        {
            /*Debug.WriteLine("开始停止网络服务");*/
            stateMutex.WaitOne();
            isRun = false;
            if (networkState == NetworkState.Waiting)
            {
                server.Stop();
            }
            else if(networkState == NetworkState.Connected)
            {
                send(0, "结束");
            }
            stateMutex.ReleaseMutex();
            release();
/*            Debug.WriteLine("网络资源释放完毕");*/
        }
        private void release()
        {
            thread.Join();
        }

        public int getPort() { return port; }
        public string getClientName() { return clientName; }
    }
}
