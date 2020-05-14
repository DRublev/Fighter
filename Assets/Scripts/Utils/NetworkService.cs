using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class NetworkService
{
    private UdpClient client;

    public NetworkService(String ip, int port)
    {
        this.client = new UdpClient(ip, port);
    }

    public void Send(byte[] data, AsyncCallback callback)
    {
        try
        {
            this.client.BeginSend(data, data.Length, callback, this.client);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void Sent(ref MemoryStream stream)
    {
        try
        {
            while (stream.CanRead)
            {
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, 256);
                this.client.Send(data, data.Length);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
