using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using UnityEngine;

namespace Assets.Scripts
{
    public class UDPReceiver
    {
        public delegate string[] ToQueue(ref string buffer);
        public ToQueue toQueue;
        public int Port
        {
            get;
            set;
        }
        private UdpClient udpReceiver;
        protected Encoding defaultEncoding = Encoding.UTF8;
        protected byte[] readBuffer; // not in use
        protected string stringBuffer;
        private Queue<string> completeMessages;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        public UDPReceiver(int port)
        {
            Port = port;
            udpReceiver = new UdpClient(Port);
            completeMessages = new Queue<string>();
            stringBuffer = string.Empty;
            readBuffer = new byte[10240];
        }
        public UDPReceiver() : this(8081)
        { }

        public void ReceiveStart()
        {
            try
            {
                udpReceiver.BeginReceive(new AsyncCallback(Receive), null);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        public void ReceiveEnd()
        {
            udpReceiver.Close();
        }
        //beginReceive callback
        private void Receive(IAsyncResult result)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, Port);
            byte[] recvd = udpReceiver.EndReceive(result, ref endPoint);
            udpReceiver.BeginReceive(new AsyncCallback(Receive), null);
            if (recvd.Length > 1024)
                Debug.LogWarning("UDP Receiver: ALARM, incoming data is longer then 1kb");
            AddToStringBuffer(recvd);
        }
        /// <summary>
        /// not in use
        /// </summary>
        /// <param name="bytes"></param>
        protected void AddToByteBuffer(byte[] bytes)
        {
            //todo
        }
        protected void AddToStringBuffer(byte[] buffer, Encoding encoding)
        {
            stringBuffer = encoding.GetString(buffer);
            if (stringBuffer.Length >= 10240)
            {
                stringBuffer = stringBuffer.Substring(stringBuffer.Length - 256);
            }
            EnqueueMessages();
        }
        protected void AddToStringBuffer(byte[] buffer)
        {
            AddToStringBuffer(buffer, defaultEncoding);
        }
        private void EnqueueMessages()
        {
            foreach (string element in toQueue.Invoke(ref stringBuffer))
            {
                completeMessages.Enqueue(element);
            }
        }
        public ref byte[] GetReceivedByte()
        {
            return ref readBuffer;
        }
        public ref string GetReceivedString()
        {
            return ref stringBuffer;
        }
        public string GetMessage()
        {
            //temporary
            return completeMessages.Dequeue();
        }
    }

    public class TCPReciever : UDPReceiver
    {
        public string Host
        {
            get;
            set;
        }
        private TcpClient tcpClient;
        private NetworkStream netStream;
        private int readTimeout;
        private Thread readingThread;
        public TCPReciever(string host, int port, int ticksPerSecond) : base(port)
        {
            tcpClient = new TcpClient();
            Host = host;
            readTimeout = 1000 / ticksPerSecond;
        }
        public new void ReceiveStart()
        {
            Connect();
            readingThread = new Thread(new ThreadStart(Receiver));
        }
        public new void ReceiveEnd()
        {
            readingThread.Abort();
            tcpClient.Close();
        }
        private async void Connect()
        {
            try
            {
                await tcpClient.ConnectAsync(Host, Port);
                netStream = tcpClient.GetStream();
                netStream.ReadTimeout = readTimeout;
                Debug.Log("Connected to: " + Host + " : " + Port);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        private void Receiver()
        {
            if (netStream.CanRead)
            {
                byte[] buffer = new byte[1024];
                netStream.Read(buffer, 0, buffer.Length);
                AddToStringBuffer(buffer);
            }
            else
            {
                Debug.LogWarning("TCP receiver: network stream cant be read");
            }
            Receiver();
        }
    }

}
