using System;
using UnityEngine;

public class NetworkProceed : MonoBehaviour
{
    public string ServerIP = "62.122.242.63";
    public int Port = 8080;

    private NetworkService networkService;

    void Awake()
    {
        this.networkService = new NetworkService(ServerIP, Port);
    }

    public void Send(byte[] toSendBytes)
    {
        try
        {
            Debug.Log("Sending data to " + ServerIP + ":" + Port);
            this.networkService.Send(toSendBytes);
        }
        catch (Exception ex)
        {
            Debug.LogError("Sending data error: " + ex.Message);
        }
    }

    public byte[] Recieve()
    {
        try
        {
            return this.networkService.Recieve();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

        return new byte[0];
    }
}