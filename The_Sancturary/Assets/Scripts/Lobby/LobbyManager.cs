using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI playerListText;

    public enum Difficulty { Easy, Medium, Hard }
    public static Difficulty selectedDifficulty = Difficulty.Medium; // default

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        UpdatePlayerList();
    }

    void UpdatePlayerList()
    {
        int counts = 0;
        playerListText.text = "#players: ";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            counts++;
        }
        playerListText.text = playerListText.text.ToString() + counts.ToString();
    }

    // update the amount of players that are in the game
    public override void OnJoinedRoom()
    {
        UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }
    public void OnBackButtonPressed()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Menu Scene");
    }

    public void SetEasyDifficulty()
    {
        if (PhotonNetwork.IsMasterClient) 
        {
            selectedDifficulty = Difficulty.Easy;
            StartCoroutine(LoadGameScene());
        }
            
    }

    public void SetMediumDifficulty()
    {
        if (PhotonNetwork.IsMasterClient) { 
            selectedDifficulty = Difficulty.Medium;
            StartCoroutine(LoadGameScene());
        }
        
    }

    public void SetHardDifficulty()
    {
        if (PhotonNetwork.IsMasterClient) 
        {
            selectedDifficulty = Difficulty.Hard;
            StartCoroutine(LoadGameScene());
        }
            
    }

    private IEnumerator LoadGameScene()
    {
        //Debug.Log("loading into the game scene");
        yield return new WaitForSeconds(2f); // Short fake load
        PhotonNetwork.LoadLevel("Map 1 Test"); // Sync load
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
