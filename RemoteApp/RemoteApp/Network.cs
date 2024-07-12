﻿using System;
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
        private string clentName;
        
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
            Debug.WriteLine("主线程回归");
        }
        private void connect()
        {
            try
            {
                Debug.WriteLine("通信线程开始");
                server = new TcpListener(port);
                server.Start(); 
                client = server.AcceptTcpClient();
                if (isRun)
                {
                    stateMutex.WaitOne();
                    networkState = NetworkState.Connected;
                    stateMutex.ReleaseMutex();

                    stream = client.GetStream();
                    readClientName();
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
        private void readClientName()
        {
            byte[] buffer = new byte[1024];
            int length = stream.Read(buffer, 0, buffer.Length);
            clentName = Encoding.UTF8.GetString(buffer, 0, length);
        }
        private void run()
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
            Debug.WriteLine("网络资源释放完毕");
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
        public string getClientName() { return clentName; }
    }
}