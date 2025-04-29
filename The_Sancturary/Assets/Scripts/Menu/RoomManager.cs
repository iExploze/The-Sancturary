using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("start the join lobby");
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnJoinLobbyButton() 
    {
        if (PhotonNetwork.IsConnected)
        {
            //Debug.Log("Trying to join or create a room...");
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            //Debug.LogWarning("Not connected yet. Please wait...");
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //Debug.Log("No room found, creating a new one...");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }

    public override void OnJoinedRoom()
    {

        //Debug.Log("Joined a room!");
        SceneManager.LoadScene("Lobby Scene");
    }

    public override void OnCreatedRoom()
    {
        //Debug.Log("Created a new room!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
