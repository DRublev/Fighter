using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TextureUtils 
{
    private static Texture2D ToTexture2D(Texture texture) {
		Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
		RenderTexture currentRT = RenderTexture.active;
		RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
		Graphics.Blit(texture, renderTexture);

		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
		texture2D.Apply();

		RenderTexture.active = currentRT;
		RenderTexture.ReleaseTemporary(renderTexture);
		return texture2D;
	}
}