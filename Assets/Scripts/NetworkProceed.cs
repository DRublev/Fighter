using System;
using UnityEngine;

public class NetworkProceed : MonoBehaviour {
	public String ServerIP = "127.0.0.1";
	public int Port = 8080;

	private NetworkService networkService;

	void Awake() {
		this.networkService = new NetworkService(ServerIP, Port);
	}

	public void Send(byte[] toSendBytes) {
		try {
			Debug.Log("Sending data to " + ServerIP + ":" + Port);
			this.networkService.Send(toSendBytes);
		} catch (Exception ex) {
			Debug.LogError("Sending data error: " + ex.Message);
		}
	}
}