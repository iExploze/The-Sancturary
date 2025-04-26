using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviourPun
{
    public GameObject playerPrefab;

    void Start()
    {
        if (PhotonNetwork.IsConnected && playerPrefab != null)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, this.transform.position, Quaternion.identity);
        }
    }
}
