using Photon.Pun;
using UnityEngine;

public class LevelManager : MonoBehaviourPunCallbacks
{
    public string playerPrefabName = "Player"; // prefab name in Resources folder
    public GameObject spawnPoint;              // assign in Inspector

    void Start()
    {
        Debug.Log($"🟡 LevelManager.Start() - Connected={PhotonNetwork.IsConnectedAndReady}, InRoom={PhotonNetwork.InRoom}");
    }

    public override void OnJoinedRoom()
{
}


}
