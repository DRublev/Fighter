using UnityEngine;

public class LobbyTest : MonoBehaviour {
	public virtual void OnClientEnterLobby() 
	{
		Debug.Log("Client connected!");
	}

	public virtual void OnClientExitLobby()
	{
		Debug.Log("CLient disconnected! So sad T_T");
	}
}
