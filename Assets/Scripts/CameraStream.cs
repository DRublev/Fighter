using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CameraStream : MonoBehaviour {
	[SerializeField]
	private RawImage rawImage;

	[SerializeField]
	private RawImage toRenderRecievedImage;

	[SerializeField]
	private NetworkProceed network;

	private WebCamTexture webCamTexture;

	private MemoryStream webcamStream;
	void Start() {
		WebCamDevice[] devices = WebCamTexture.devices;

		// for debugging purposes, prints available devices to the console
		for (int i = 0; i < devices.Length; i++) {
			Debug.Log("Webcam available: " + devices[i].name);
		}

		WebCamTexture webCamTexture = new WebCamTexture(devices[0].name);
		this.webCamTexture = webCamTexture;

		RawImage rawImage;
		rawImage = GetComponent<RawImage>();
		rawImage.texture = webCamTexture;
		webCamTexture.Play();
		this.webcamStream = new MemoryStream();
	}

	void FixedUpdate() {
		byte[] snap = TakeSnapshot(rawImage, this.webCamTexture);
		//this.network.Send(snap);

		if (Input.GetKey(KeyCode.Mouse0)) {
			//SaveTextureOnDisk(snap);
		}
	}

	private static byte[] TakeSnapshot(RawImage rawImage, WebCamTexture webCamTexture) {
		Texture2D texture = new Texture2D(rawImage.texture.width, rawImage.texture.height, TextureFormat.ARGB32, false);

		texture.SetPixels(webCamTexture.GetPixels());
		texture.Apply();

		byte[] bytes = texture.EncodeToJPG();
		return bytes;
	}

	private static void SaveTextureOnDisk(byte[] image) {
		try {
			Debug.Log("Writing to disk " + image.Length);
			string filename = "fromUnity.jpg";
			FileStream fs = File.Create(@"E:\" + filename, image.Length);
			fs.Write(image, 0, image.Length);
			fs.Close();
		} catch (Exception e) {
			Debug.LogError(e.Message);
		}
	}
}