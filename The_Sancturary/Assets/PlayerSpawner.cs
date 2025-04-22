using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    void Start()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points found! Spawning at Vector.zero.");
            PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
            return;
        }

        // Pick a random spawn point
        Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;

        PhotonNetwork.Instantiate("Player", spawn.position, spawn.rotation);
    }
}
