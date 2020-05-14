using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Newtonsoft.Json;

using UnityEngine;

public class NetworkProceed : MonoBehaviour
{
    public string ServerIP = "62.122.242.63";
    public int Port = 8080;

    private NetworkService networkService = null;

    void Awake()
    {
        this.networkService = new NetworkService(ServerIP, Port);
    }
    private void FixedUpdate()
    {
        Vector2[] v2 = { new Vector2(2, 3), new Vector2(12, 13), new Vector2(112, 113) };
        Debug.Log(JsonConvert.SerializeObject(v2));
    }

    public void Send(byte[] toSendBytes)
    {
        try
        {
            if (this.networkService != null)
            {
                Debug.Log("Data size is " + toSendBytes.Length);
                this.networkService.Send(toSendBytes, new AsyncCallback(sendCallBack));
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Sending data error: " + ex.Message);
        }
    }

    private static void sendCallBack(IAsyncResult result)
    {
        UdpClient udpClient = (UdpClient) result.AsyncState;
        udpClient.EndSend(result);
        Debug.Log("Sending data " + udpClient.Available);
    }

    public byte[] Recieve()
    {
        try
        {
            MemoryStream recievedStream = this.networkService.StartRecieve();
            byte[] recievedData = new byte[recievedStream.Length];

            recievedStream.Read(recievedData, 0, (int) recievedStream.Length);
            return recievedData;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

        return new byte[0];
    }
}
