using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class NetworkService
{
    private UdpClient client;
    private IPEndPoint endpoint = null;
    private IAsyncResult recieveResult = null;
    private MemoryStream recieveStream;

    public NetworkService(String ip, int port)
    {
        this.client = new UdpClient(ip, port);
        this.endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
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

    public void Sent(ref MemoryStream stream, AsyncCallback callback)
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

    public ref MemoryStream StartRecieve()
    {
        StartListening();
        return ref this.recieveStream;
    }

    private void Recieve(IAsyncResult result)
    {
        byte[] recievedBytes = this.client.EndReceive(result, ref this.endpoint);
        this.recieveStream.Write(recievedBytes, 0, recievedBytes.Length);
        StartListening();
    }

    private void StartListening()
    {
        this.recieveResult = this.client.BeginReceive(Recieve, this.client);
    }
}
