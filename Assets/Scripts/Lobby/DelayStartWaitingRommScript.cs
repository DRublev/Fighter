using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DelayStartWaitingRommScript : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update

    private PhotonView myPhotonView;

    [SerializeField]
    private int multiplayerSceneIndex;

    [SerializeField]
    private int menuSceneIndex;

    private int playerCount;
    private int roomSize;


    [SerializeField]
    private int minPlayersToStart;

    [SerializeField]
    private Text roomCountDisplay;
    [SerializeField]
    private Text timerToStartDisplay;

    private bool readyToCountDown;
    private bool readyToStart;
    private bool startingGame;

    private float timerToStartGame;
    private float notFullGameTimer;
    private float fullGameTimer;

    [SerializeField]
    private float maxWaitTime;
    [SerializeField]
    private float maxFullGameWaitTime;
    void Start()
    {
        myPhotonView = GetComponent<PhotonView>();
        fullGameTimer = maxFullGameWaitTime;
        notFullGameTimer = maxWaitTime;
        timerToStartGame = maxWaitTime;

        PlayerCountUpdate();
    }


    void PlayerCountUpdate()
    {
        Debug.Log("Players");
        playerCount = PhotonNetwork.PlayerList.Length;
        roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
        roomCountDisplay.text = playerCount + ":" + roomSize;
        Debug.Log(roomCountDisplay.text);

        if (playerCount == roomSize)
        {
            readyToStart = true;

        } else if (playerCount >= minPlayersToStart)
        {
            readyToCountDown = true;
        }
        else
        {
            readyToCountDown = false;
            readyToStart = false;
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("On Other player entered Room");
        PlayerCountUpdate();

        if(PhotonNetwork.IsMasterClient)
            myPhotonView.RPC("RPS_SendTimer", RpcTarget.Others, timerToStartGame);
    }

    [PunRPC]
    private void RPS_SendTimer(float timeIn)
    {
        Debug.Log("This is an RpC function");
        timerToStartGame = timeIn;
        notFullGameTimer = timeIn;
        if(timeIn < fullGameTimer)
        {
            fullGameTimer = timeIn;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayerCountUpdate();
    }
    // Update is called once per frame
    private void Update()
    {
        WaitingForMorePlayers();   
    }


    void WaitingForMorePlayers()
    {
        if(playerCount <= 1)
        {
            ResetTimer();
        }
        if(readyToStart)
        {
            fullGameTimer -= Time.deltaTime;
            timerToStartGame = fullGameTimer;
        } else if(readyToCountDown)
        {
            notFullGameTimer -= Time.deltaTime;
            timerToStartGame = notFullGameTimer;
        }
        string tempTimer = string.Format("{0:00}", timerToStartGame);
        timerToStartDisplay.text = tempTimer;

        if(timerToStartGame <= 0f)
        {
            if(startingGame)
                return;
               StartGame();
        }
    }

    void ResetTimer()
    {
        timerToStartGame = maxWaitTime;
        notFullGameTimer = maxWaitTime;
        fullGameTimer = maxFullGameWaitTime;
    }

    public void StartGame()
    {
        startingGame = true;
        if(!PhotonNetwork.IsMasterClient)
            return;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(multiplayerSceneIndex);
    }

    public void DelayCancle()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(menuSceneIndex);
    }
}
