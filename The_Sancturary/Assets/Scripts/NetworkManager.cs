using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // connect to master server
    }

    public void OnClickPlay()
    {
        PhotonNetwork.JoinOrCreateRoom("MainRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Map 1 Test"); // everyone loads GameScene
    }
}
