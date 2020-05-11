using System;
using System.Net.Sockets;

public class NetworkService
{
    private UdpClient client;

    public NetworkService(String ip, int port)
    {
        if (ip == String.Empty) return;

        this.client = new UdpClient(ip, port);
    }

    public void Send(byte[] data)
    {
        try
        {
            this.client.Send(data, data.Length);
        }
        catch (System.Exception ex)
        {
            throw ex;
        }
    }
}