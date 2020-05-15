using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;

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
        //udpReceiver.ReceiveStart();
        Vector2JSON[,] v2 = { { new Vector2JSON(1, 1), new Vector2JSON(2, 1) },
                              { new Vector2JSON(2, 1), new Vector2JSON(2, 2) },
                              { new Vector2JSON(3, 1), new Vector2JSON(3, 2) }
                            };
        string str = "(asd1),(ASD2),(asd3)";
        Regex regex = new Regex(@"\(.*?\)");
        MatchCollection matches = regex.Matches(str);
        for(int i=0; i < matches.Count; i++)
        {
            string st = matches[i].Value;
            Debug.Log(st);
        }
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
        List<Vector2[]> result = new List<Vector2[]>();
        if (!udpReceiver.GetMsg(ref result))
            Debug.Log("Server is not sending");
        return result;
    }
}
