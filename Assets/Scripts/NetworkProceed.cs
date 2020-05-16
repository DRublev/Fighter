using System;
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
        private static readonly int bytesOnDataId = 36;
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
            try
            {
                string dataId = GenerateId();
                string dataLength = toSend.Length.ToString();
                int lastChunkSize;
                int lastChunkIndex = -1;
                int chunksCount = Math.DivRem(toSend.Length, bytesOnChunkData, out lastChunkSize);
                if (lastChunkSize > 0)
                {
                    chunksCount++;
                    lastChunkIndex = -(lastChunkSize - toSend.Length - 1);

                }
                string chunksLength = chunksCount.ToString();
                string chunkInfo = dataId + separator + dataLength + separator + chunksLength + separator;

                if (chunksCount >= 1)
                {
                    List<byte> toSendList = toSend.ToList();
                    for (int i = 0; i < (lastChunkSize > 0 ? chunksCount - 1 : chunksCount); i++)
                    {
                        byte[] chunkData = new byte[bytesOnChunkData];
                        chunkData = toSendList.Take(bytesOnChunkData).ToArray();
                        Debug.Log("Chunk info: " + chunkInfo + i);

                        sendingStream.Add(CreateChunk(chunkInfo + i + separator, chunkData));
                    }
                    if (lastChunkIndex != -1)
                    {
                        byte[] chunkData = GetNotFullChunk(toSend, lastChunkIndex);
                        Debug.Log("Last chunk info: " + chunkInfo + (chunksCount - 1));

                        sendingStream.Add(CreateChunk(chunkInfo + (chunksCount - 1) + separator, chunkData));
                    }
                }
                else
                {
                    byte[] chunk = GetNotFullChunk(toSend, 0);
                    Debug.Log("Last chunk info: " + chunkInfo);

                    sendingStream.Add(CreateChunk(chunkInfo + '0' + separator, chunk));
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

        }

        private byte[] GetNotFullChunk(byte[] data, int fromIndex)
        {
            byte[] chunk = new byte[bytesOnChunkData];
            int length = data.Length - fromIndex;
            Array.ConstrainedCopy(data, fromIndex, chunk, 0, length);
            return chunk;
        }

        private byte[] CreateChunk(string chunkInfo, byte[] chunkData)
        {
            byte[] chunkInfoByted = Encoding.UTF8.GetBytes(chunkInfo);
            byte[] chunk = new byte[chunkSize + 1];

            Array.ConstrainedCopy(chunkInfoByted, 0, chunk, 0, chunkInfoByted.Length);
            Array.ConstrainedCopy(chunkData, 0, chunk, chunkInfoByted.Length - 1, chunkData.Length);

            return chunk;
        }

        private string GenerateId() => Guid.NewGuid().ToString().Replace(@"-", "");

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
