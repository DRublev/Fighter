﻿using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CameraStream : MonoBehaviour
{
    [SerializeField]
    private RawImage _rawImage;

    [SerializeField]
    private NetworkProceed _network;

    private WebCamTexture _webCamTexture;
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        // for debugging purposes, prints available devices to the console
        for (int i = 0; i < devices.Length; i++)
        {
            print("Webcam available: " + devices[i].name);
        }

        WebCamTexture webCamTexture = new WebCamTexture(devices[0].name);
        this._webCamTexture = webCamTexture;

        RawImage rawImage;
        rawImage = GetComponent<RawImage>();
        rawImage.texture = webCamTexture;
        webCamTexture.Play();
    }

    void Update()
    {
        byte[] snap = TakeSnapshot(_rawImage, this._webCamTexture);
        _network.Send(snap);
    }

    private static byte[] TakeSnapshot(RawImage rawImage, WebCamTexture webCamTexture)
    {
        //Create a Texture2D with the size of the rendered image on the screen
        Texture2D texture = new Texture2D(rawImage.texture.width, rawImage.texture.height, TextureFormat.ARGB32, false);

        //Save the image to the Texture2D
        texture.SetPixels(webCamTexture.GetPixels());
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        return bytes;
    }

    private static void SaveTextureOnDisk(byte[] image)
    {
        try
        {
            Debug.Log("Writing to disk " + image.Length);
            string filename = "fromUnity.png";
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