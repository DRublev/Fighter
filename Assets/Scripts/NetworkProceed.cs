using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

using Assets.Scripts;

using UnityEngine;

namespace Assets.Scripts
{
    public class NetworkProceed : MonoBehaviour
    {
        public string ServerIP = "62.122.242.63";
        public int Port = 8080;
        public int RecievePort = 8081;

        private UDPSender sender = null;
        private UDPReceiver udpReceiver;
        private TCPReciever tcpReceiver;

        public float sendingPeriod = 0.4f;

        private static int chunkSize = 1024 + 32 + 6 + 6 + 6;
        private readonly byte[] emptyChunk = new byte[chunkSize];
        private int recievingFramerate = 30;

        void Awake()
        {
            this.sender = new UDPSender(ServerIP, Port);
            this.tcpReceiver = new TCPReciever(ServerIP, RecievePort, recievingFramerate);
            //add enqueuing of bone messages
            //tcpReceiver.toQueue += JSONParser.GetBoneMessage;
        }
        private void Start()
        {
            //tcpReceiver.ReceiveStart();
        }

        void OnDisable()
        {
            // Close all connections
            sender.Close();
        }

        public void Send(ref List<byte> toSend)
        {

        }

        public void Send(byte[] toSendBytes)
        {
            try
            {
                if (sender != null)
                {
                    Debug.Log("Data size is " + toSendBytes.Length);
                    sender.Send(toSendBytes, new AsyncCallback(SendCallBack));
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Sending data error: " + ex.Message);
            }
        }

        public byte[] GetEmptyChunk() => emptyChunk;

        private static void SendCallBack(IAsyncResult result)
        {
            UdpClient udpClient = (UdpClient) result.AsyncState;
            udpClient.EndSend(result);
        }

        public List<Vector2[]> RecieveList()
        {
            List<Vector2JSON[]> result = JSONParser.GetBonesList(tcpReceiver.GetMessage());
            List<Vector2[]> uVector2 = new List<Vector2[]>();

            //dumb but fast to make
            foreach (Vector2JSON[] element in result)
            {
                Vector2[] vectors = new Vector2[element.Length];
                for (int i = 0; i < element.Length; i++)
                {
                    vectors[i] = element[i].ToVector2();
                }
                uVector2.Add(vectors);
            }
            return uVector2;
        }
    }
}
