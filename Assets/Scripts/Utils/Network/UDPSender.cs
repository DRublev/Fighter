using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class UDPSender
{
    private UdpClient client;

    public UDPSender(String ip, int port)
    {
        this.client = new UdpClient(ip, port);
    }

    public void Send(byte[] data, AsyncCallback callback)
    {
        try
        {
            client.BeginSend(data, data.Length, callback, client);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void Close() => client.Close();
}
