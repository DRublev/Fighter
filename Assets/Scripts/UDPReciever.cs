using System;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public class UDPReceiver
{
    public int Port
    { get; set; }
    private UdpClient udpReceiver;
    protected byte[] readBuffer;
    protected string stringBuffer;
    private Queue<string> received;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="port"></param>
    public UDPReceiver(int port)
    {
        Port = port;
        udpReceiver = new UdpClient(Port);
        received = new Queue<string>();
        stringBuffer = string.Empty;
        readBuffer = new byte[10240];
    }
    public UDPReceiver() : this(8081)
    {
    }

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
        this.AddToByteBuffer(recvd);
        stringBuffer += System.Text.Encoding.UTF8.GetString(recvd);
        //checking for comlete msg
        //parsing to string, may remove **************************************************
        string[] msgs;
        if (JSONParser.TryParseRegex(out msgs, stringBuffer, @"\[\[.*?\]\]"))
        {
            foreach (string element in msgs)
            {
                received.Enqueue(element);
                stringBuffer = stringBuffer.Replace(element, string.Empty);
            }
        }
        //************************************************************************
        //we don't need infinite strings, save only last 256 chars
        if (stringBuffer.Length >= 10240)
        {
            stringBuffer = stringBuffer.Substring(stringBuffer.Length - 256);
        }
    }
    protected void AddToByteBuffer(byte[] bytes)
    {
        //todo
    }
    public bool GetMsg(ref List<Vector2JSON[]> result)
    {
        if (received.Count == 0)
            return false;
        result = JSONParser.GetBonesList(received.Dequeue());
        return true;
    }    
}

public class TCPReciever : UDPReceiver
{
    public string Host
    { get; set; }
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
        this.Connect();
        readingThread = new Thread(new ThreadStart(Receiver));
    }
    public new void ReceiveEnd()
    {

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
        catch(Exception e)
        {
            Debug.LogException(e);
        }
    }
    private void Receiver()
    {
        
    }
}
