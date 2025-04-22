using UnityEngine;
using Photon.Pun;

public class PlayerCameraHandler : MonoBehaviourPun
{
    void Start()
    {
        if (!photonView.IsMine)
        {
            // Disable the local camera for other players
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null) cam.enabled = false;

            AudioListener audio = GetComponentInChildren<AudioListener>();
            if (audio != null) audio.enabled = false;
        }
    }
}
