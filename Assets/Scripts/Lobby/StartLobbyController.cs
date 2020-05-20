using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLobbyController : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject startBtn;
    [SerializeField]
    private GameObject cancleBtn;
    [SerializeField]
    private int RoomSize;


    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        startBtn.SetActive(true);
    }

    public void StartGame()
    {
        startBtn.SetActive(false);
        cancleBtn.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Start the random game");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a room");
        CreateRoom();
    }

    void CreateRoom()
    {
        Debug.Log("Creating room now");
        int randomRoomNumber = Random.Range(0, 1000);
        RoomOptions roomOpt = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 2 };
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOpt);
        Debug.Log("Room is running");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room");
        CreateRoom();
    }

    public void CancleRoom()
    {
        cancleBtn.SetActive(false);
        startBtn.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }

}
