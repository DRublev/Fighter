using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkControl : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();   
    }

    // Update is called once per frame

    public override void OnConnectedToMaster()
    {
        Debug.Log("This app is running on " + PhotonNetwork.CloudRegion + " server");
    }
    void Update()
    {
        
    }
}
