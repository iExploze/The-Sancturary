using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PlayerExitController : MonoBehaviourPunCallbacks
{
    [Header("Scene Names")]
    [SerializeField] string endScene = "End Scene"; // bad-ending banner

    const string PREF_SURVIVED = "Survived";         // 1 = good, 0 = bad
    bool _loadEndSceneWhenLeft = false;              // only for non-masters

    /* ------------------------------------------------------------------ */
    /*  Hook this to the UI Button’s OnClick()                            */

    public void OnExitClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            CloseLobbyForEveryone();
        }
        else
        {
            // always bad ending for rage-quitters
            PlayerPrefs.SetInt(PREF_SURVIVED, 0);

            _loadEndSceneWhenLeft = true;   // remember to load End Scene later
            PhotonNetwork.LeaveRoom();      // async → OnLeftRoom callback
        }
    }

    /* ------------------------------------------------------------------ */
    /*  Local callback after *this* client has left the room              */

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
        // prevent late joiners during the fade-out
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        // AutomaticallySyncScene must be true somewhere in your bootstrap code.
        // This call teleports *all* still-connected clients to the Menu.
        PhotonNetwork.LoadLevel(endScene);
    }
}
