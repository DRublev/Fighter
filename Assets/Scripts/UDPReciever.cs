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
    private string allReceived;
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
        allReceived = string.Empty;
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
        allReceived += System.Text.Encoding.UTF8.GetString(recvd);
        //checking for comlete msg
        string[] msgs;
        if (JSONParser.TryParseRegex(out msgs, allReceived, @"\[\[.*?\]\]"))
        {
            foreach (string element in msgs)
            {
                received.Enqueue(element);
                allReceived = allReceived.Replace(element, string.Empty);
            }
        }
        //we don't need infinite strings, save only last 256 chars
        if (allReceived.Length >= 10240)
        {
            allReceived = allReceived.Substring(allReceived.Length - 256);
        }
    }
    public bool GetMsg(ref List<Vector2JSON[]> result)
    {
        if (received.Count == 0)
            return false;
        result = JSONParser.GetBonesList(received.Dequeue());
        return true;
    }    
}
