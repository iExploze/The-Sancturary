using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkBootstrap : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public GameObject SpawnLoc;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Connect to Photon master servers
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("TestRoom", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room!");
        Vector3 spawnPos = new Vector3(SpawnLoc.transform.position.x, SpawnLoc.transform.position.y, 0);
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);
    }
}
