using System;
using System.Net;
using System.Net.Sockets;

public class NetworkService
{
    private UdpClient client;
    private IPEndPoint endpoint = null;

    public NetworkService(String ip, int port)
    {
        this.client = new UdpClient(ip, port);
        this.client.Connect(ip, port);
        // this.client.Client.ReceiveBufferSize = 1024 * 1024;
    }

    public void Send(byte[] data)
    {
        try
        {
            this.client.BeginSend(data, data.Length, new AsyncCallback(sendCallBack), this.client);
        }
        catch (System.Exception ex)
        {
            throw ex;
        }
    }

    private static void sendCallBack(IAsyncResult result)
    {
        UdpClient udpClient = (UdpClient)result.AsyncState;
        udpClient.EndSend(result);
    }

}