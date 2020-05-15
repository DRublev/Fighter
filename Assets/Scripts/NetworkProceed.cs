﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

using UnityEngine;

public class NetworkProceed : MonoBehaviour
{
    public string ServerIP = "62.122.242.63";
    public int Port = 8080;

    private NetworkService networkService = null;
    private UDPReceiver udpReceiver;

    void Awake()
    {
        this.networkService = new NetworkService(ServerIP, Port);
        this.udpReceiver = new UDPReceiver(8081);
    }
    private void Start()
    {
        udpReceiver.ReceiveStart();
    }
    private void FixedUpdate()
    {
        
    }

    public void Send(byte[] toSendBytes)
    {
        try
        {
            if (this.networkService != null)
            {
                Debug.Log("Data size is " + toSendBytes.Length);
                this.networkService.Send(toSendBytes, new AsyncCallback(SendCallBack));
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Sending data error: " + ex.Message);
        }
    }

    private static void SendCallBack(IAsyncResult result)
    {
        UdpClient udpClient = (UdpClient) result.AsyncState;
        udpClient.EndSend(result);
        Debug.Log("Sending data " + udpClient.Available);
    }

    public List<Vector2[]> RecieveList()
    {
        List<Vector2JSON[]> result = new List<Vector2JSON[]>();
        if (!udpReceiver.GetMsg(ref result))
            Debug.Log("No usefull info riceived via UDP");
        List<Vector2[]> uVector2 = new List<Vector2[]>();
        //dumb but fast to make
        foreach(Vector2JSON[] element in result)
        {
            Vector2[] vectors = new Vector2[element.Length];
            for(int i = 0; i < element.Length; i++)
            {
                vectors[i] = element[i].ToVector2();
            }
            uVector2.Add(vectors);
        }
        return uVector2;
    }
}
