using System;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
public class UDPReciver
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
    /// <param name="ip">Not in use</param>
    /// <param name="port"></param>
    public UDPReciver(string ip, int port)
    {
        //Ip = ip;
        Port = port;
        udpReceiver = new UdpClient(Port);
        received = new Queue<string>();
    }
    public UDPReciver() : this("127.0.0.1", 8081)
    {
    }

    public void ReceiveStart()
    {
        try
        {
            udpReceiver.BeginReceive(new AsyncCallback(Receive), null);
        }
        catch(Exception e)
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
    /*public bool GetMsg()
    {
        if(received.Count == 0)
            return false;
         = JsonConvert.
        
    }*/
    private string[] GetBlocks(string arg)
    {
        arg = arg.Trim(new char[] { '[', ']' });
        return arg.Split(new string[] { "],[" }, StringSplitOptions.None);
    }
}
