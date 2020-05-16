﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

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
        private TCPReciever tcpReceiver;

        public float sendingPeriod = 0.4f;

        private static readonly string separator = "||";
        private static readonly int bytesOnDataId = 32;
        private static readonly int bytesOnDataSize = 6;
        private static readonly int bytesOnChunkIndex = 6;
        private static readonly int bytesOnChunkLength = 6;
        private static readonly int bytesOnSeparators = 3 * separator.Length; // (fields count - 1) * separator bytelength
        private static readonly int bytesOnChunkData = 1024;
        private static readonly int chunkSize =
            bytesOnDataId + bytesOnDataSize + bytesOnChunkIndex + bytesOnChunkLength + bytesOnChunkData + bytesOnSeparators;
        private readonly byte[] emptyChunk = new byte[chunkSize];
        private readonly int recievingFramerate = 30;

        private List<byte[]> sendingStream = new List<byte[]>();

        private void Awake()
        {
            this.sender = new UDPSender(ServerIP, Port);
            this.tcpReceiver = new TCPReciever(ServerIP, RecievePort, recievingFramerate);
            //add enqueuing of bone messages
            tcpReceiver.toQueue += JSONParser.GetBoneMessage;
        }

        private void Start()
        {
            tcpReceiver.ReceiveStart();
            InvokeRepeating("SendChunk", 0f, sendingPeriod);
        }

        private void OnDisable()
        {
            // Close all connections
            sender.Close();
        }

        public void SendWithoutChunking(byte[] toSendBytes)
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

        public void Send(byte[] toSend)
        {
            // Create chunk info and concat with chunk data
            string dataId = GenerateId();
            string dataLength = toSend.Length.ToString();
            int lastChunkSize;
            int lastChunkIndex = -1;
            int chunksCount = Math.DivRem(toSend.Length, bytesOnChunkData, out lastChunkSize);
            if (lastChunkSize > 0)
            {
                chunksCount++;
                lastChunkIndex = chunksCount * bytesOnChunkData;

            }
            string chunksLength = chunksCount.ToString();
            string chunkInfo = dataId + separator + dataLength + separator + chunksLength + separator;
            Debug.Log("Data chunk info: " + chunkInfo);

            if (chunksCount >= 1)
            {
                List<byte> toSendList = toSend.ToList();
                for (int i = 0; i < (lastChunkSize > 0 ? chunksCount - 1 : chunksCount); i++)
                {
                    byte[] chunkData = toSendList.Take(bytesOnChunkData).ToArray();
                    chunkInfo += i + separator;
                    sendingStream.Add(CreateChunk(chunkInfo, chunkData));
                }
                if (lastChunkIndex != -1)
                {
                    byte[] chunkData = GetNotFullChunk(toSend, lastChunkIndex);
                    chunkInfo += chunksCount + separator;
                    sendingStream.Add(CreateChunk(chunkInfo, chunkData));
                }
            }
            else
            {
                byte[] chunk = GetNotFullChunk(toSend, 0);
                chunkInfo += 0 + separator;
                sendingStream.Add(CreateChunk(chunkInfo, chunk));
            }
        }

        private byte[] GetNotFullChunk(byte[] data, int fromIndex)
        {
            byte[] chunk = new byte[chunkSize];
            data.CopyTo(chunk, fromIndex);
            return chunk;
        }

        private byte[] CreateChunk(string chunkInfo, byte[] chunkData)
        {
            byte[] chunkInfoByted = Encoding.UTF8.GetBytes(chunkInfo);
            byte[] chunk = new byte[chunkSize];
            chunkInfoByted.CopyTo(chunk, 0);
            chunkData.CopyTo(chunk, chunkInfoByted.Length - 1);

            return chunk;
        }

        private string GenerateId() => Guid.NewGuid().ToString().Replace(@"\-*", "");
        private string NormalizeTolength(int value, int length) => value.ToString().PadLeft(length, '0');

        private void SendChunk()
        {
            try
            {
                if (sendingStream.Count > 0)
                {
                    sender.Send(sendingStream.First(), new AsyncCallback(SendCallBack));
                    sendingStream.RemoveAt(0);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Sending data error: " + ex.Message);
            }
        }

        private byte[] GetEmptyChunk() => emptyChunk;

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
