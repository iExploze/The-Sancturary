using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerKiller : MonoBehaviourPun
{
    // Start is called before the first frame update
    private PlayerMovement playerMovement;
    [SerializeField] private GameObject flashlight;
    [SerializeField] private GameObject playersprite;
    [SerializeField] private GameObject playerassets;
    [SerializeField] private GameObject ghost;
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

        // Check if the script is running on the local player
        if (PhotonNetwork.IsConnected && !photonView.IsMine)
        {
            enabled = false;
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void killPlayer(int monsterID)
    {
        // Ensure that the player is controlled by the local client
        if (PhotonNetwork.IsConnected && !photonView.IsMine) return;

        int playerID = playerMovement.GetPlayerID();
        Debug.Log($"Killing player with ID: {playerID}");

        // If already a ghost, do nothing
        if (playerMovement.isGhost()) return;

        // Set the player into ghost mode and update all necessary components
        playerMovement.setGhost(true);
        flashlight.SetActive(false);
        playersprite.SetActive(false);
        playerassets.SetActive(false);
        ghost.SetActive(true);

        // Notify all other players about the kill via RPC
        if (PhotonNetwork.IsConnected) 
        {
            photonView.RPC("SyncKillPlayer", RpcTarget.All, playerID);
        }

        if (monsterID > 0) 
        {
            // Trigger the jumpscare using the JumpscareManager
            JumpscarePlayer jumpscareManager = playerMovement.GetComponent<JumpscarePlayer>();
            if (jumpscareManager != null)
            {
                //Debug.Log("trigger monsterID: " + monsterID);
                jumpscareManager.TriggerJumpscare(monsterID);
            }
        }
    }

    // RPC to sync the ghost state across the network
    [PunRPC]
    void SyncKillPlayer(int playerID)
    {
        if (playerMovement.isGhost()) return;

        playerMovement.setGhost(true);
        flashlight.SetActive(false);
        playersprite.SetActive(false);
        playerMovement.gameObject.SetActive(false);
        ghost.SetActive(true);

        Debug.Log($"Player with ID {playerID} has been turned into a ghost on all clients.");
    }
}
