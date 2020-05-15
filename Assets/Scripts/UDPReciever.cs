using System;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using UnityEngine;
public class UDPReceiver
{
    //public string Ip
    //{ get; set; }
    public int Port
    { get; set; }
    private UdpClient udpReceiver;
    private Queue<string> received;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="port"></param>
    public UDPReceiver(int port)
    {
        //Ip = ip;
        Port = port;
        udpReceiver = new UdpClient(Port);
        received = new Queue<string>();
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
        received.Enqueue(System.Text.Encoding.UTF8.GetString(recvd));
    }
    public bool GetMsg(ref List<Vector2[]> result)
    {
        if (received.Count == 0)
            return false;
        return true
    }
    
}
