using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;   // for Hashtable
using UnityEngine.SceneManagement;

public class GameEndController : MonoBehaviourPunCallbacks
{
    [Header("Scene Names")]
    [SerializeField] string endScene = "End Scene";   // your single end scene

    const string PREF_SURVIVED = "Survived";         // 1 = good, 0 = bad
    bool _loadEndSceneWhenLeft = false;              // only for non-masters

    /// <summary>
    /// Master-only: call this once when the round finishes.
    /// </summary>
    public void TriggerGameEnd()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PlayerPrefs.SetInt(PREF_SURVIVED, 1);
            CloseLobbyForEveryone();
        }
        else
        {
            PlayerPrefs.SetInt(PREF_SURVIVED, 1);

            _loadEndSceneWhenLeft = true;   // remember to load End Scene later
            PhotonNetwork.LeaveRoom();      // async → OnLeftRoom callback
        }
    }

    public override void OnLeftRoom()
    {
        if (_loadEndSceneWhenLeft)
        {
            SceneManager.LoadScene(endScene);
        }
        // For the master, we already loaded Menu via LoadLevel,
        // so nothing extra is needed here.
    }

    /* ------------------------------------------------------------------ */
    /*  Master-only helper                                                */

    void CloseLobbyForEveryone()
    {
        PlayerPrefs.SetInt(PREF_SURVIVED, 1);
        // prevent late joiners during the fade-out
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        // AutomaticallySyncScene must be true somewhere in your bootstrap code.
        // This call teleports *all* still-connected clients to the Menu.
        PhotonNetwork.LoadLevel(endScene);
    }
}