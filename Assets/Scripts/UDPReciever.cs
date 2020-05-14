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
    /// <param name="ip">Not in use</param>
    /// <param name="port"></param>
    public UDPReceiver(string ip, int port)
    {
        //Ip = ip;
        Port = port;
        udpReceiver = new UdpClient(Port);
        received = new Queue<string>();
    }
    public UDPReceiver() : this("127.0.0.1", 8081)
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
        string[] blocks = this.GetBlocks(received.Dequeue());
        List<string[]> list = new List<string[]>();
        foreach (string element in blocks)
        {
            string[] rawVectors = element.Trim(new char[] { '{', '}' }).Split(new string[] { "},{" }, StringSplitOptions.None);
            if (rawVectors.Length != 2)
                Debug.LogError("Data received via UDP is fucked");
            Vector2[] vectors = new Vector2[2];
            for (int i = 0; i < 2; i++)
            {
                vectors[i] = this.ToVector(rawVectors[i]);
            }
            result.Add(vectors);
        }
        return true;
    }
    private string[] GetBlocks(string arg)
    {
        arg = arg.Trim(new char[] { '[', ']' });
        return arg.Split(new string[] { "],[" }, StringSplitOptions.None);
    }
    private Vector2 ToVector(string str)
    {
        Vector2 vector = new Vector2();
        string[] divStr = str.Split(new char[] { ',' });
        string digitStr = null;
        foreach (char ch in divStr[0])
        {
            if (char.IsDigit(ch))
                digitStr += ch;
        }
        vector.x = int.Parse(digitStr);
        foreach (char ch in divStr[1])
        {
            if (char.IsDigit(ch))
                digitStr += ch;
        }
        vector.y = int.Parse(digitStr);
        return vector;
    }
}
