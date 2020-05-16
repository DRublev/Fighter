using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

using Assets.Scripts;

using UnityEngine;
using UnityEngine.UI;

public class CameraStream : MonoBehaviour
{
	[SerializeField]
	private RawImage rawImage;
	private WebCamTexture webCamTexture;

	[SerializeField]
	private RawImage toRenderRecievedImage;

	[SerializeField]
	private NetworkProceed network;

	public float sendingPeriod = 0.4f;

	private static int chunkSize = 1024 + 32 + 6 + 6 + 6;
	private readonly byte[] emptyChunk = new byte[chunkSize];

	private List<byte> webcamStream = new List<byte>();

	void Start()
	{
		WebCamDevice[] devices = WebCamTexture.devices;

		// for debugging purposes, prints available devices to the console
		for (int i = 0; i < devices.Length; i++)
		{
			Debug.Log("Webcam available: " + devices[i].name);
		}

		WebCamTexture webCamTexture = new WebCamTexture(devices[0].name);
		this.webCamTexture = webCamTexture;

		rawImage = GetComponent<RawImage>();
		rawImage.texture = webCamTexture;
		webCamTexture.Play();

		InvokeRepeating("TakeSnapshot", 0.2f, sendingPeriod / 1.5f);
		InvokeRepeating("SendFromCamera", 0.1f, sendingPeriod);
		var guid = Guid.NewGuid().ToString();
	}

	private void SendFromCamera()
	{
		if (webcamStream.Count >= chunkSize)
		{
			network.Send(webcamStream.Take(chunkSize).ToArray());
			webcamStream.RemoveRange(0, chunkSize);
		}
	}

	private void TakeSnapshot()
	{
		Texture2D texture = new Texture2D(rawImage.texture.width, rawImage.texture.height, TextureFormat.RGB24, false);

		texture.SetPixels(webCamTexture.GetPixels());
		texture.Apply();

		byte[] bytes = texture.EncodeToJPG();

		webcamStream.AddRange(bytes);
		webcamStream.AddRange(emptyChunk);
	}

	private static void SaveTextureOnDisk(byte[] image)
	{
		try
		{
			Debug.Log("Writing to disk " + image.Length);
			string filename = "fromUnity.jpg";
			FileStream fs = File.Create(@"E:\" + filename, image.Length);
			fs.Write(image, 0, image.Length);
			fs.Close();
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message);
		}
	}
}
