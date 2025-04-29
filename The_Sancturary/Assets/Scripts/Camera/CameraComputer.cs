using UnityEngine;
using Photon.Pun; // Needed for multiplayer check

public class CameraComputer : MonoBehaviourPun
{
    public GameObject cameraUI;
    public float interactionDistance = 2f;
    private Transform player;
    private PlayerMovement playerMovement; // Reference to the movement script

    private void Start()
    {
        // Only assign local player (Photon View check)
        foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (p.GetComponent<PhotonView>() != null && p.GetComponent<PhotonView>().IsMine)
            {
            
                player = p.transform;
                playerMovement = player.GetComponent<PlayerMovement>();
                break;
            }
        }

        if (cameraUI != null)
            cameraUI.SetActive(false);
    }

    private void Update()
    {
        if (player == null)
        {
            foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (p.GetComponent<PhotonView>() != null && p.GetComponent<PhotonView>().IsMine)
                {
                    player = p.transform;
                    playerMovement = player.GetComponent<PlayerMovement>();
                    break;
                }
            }
        }
    }

    private void OnMouseDown()
    {
        if (player == null) return;
        Debug.Log("click");
        // Only let local player interact
        if (Vector2.Distance(transform.position, player.position) <= interactionDistance)
        {
            ToggleCameraUI();
        }
    }

    public void ToggleCameraUI()
    {
        Debug.Log("open cam");

        if (cameraUI == null) return;

        bool isActive = !cameraUI.activeSelf;
        cameraUI.SetActive(isActive);

        if (playerMovement != null)
        {
            if(playerMovement.ableMove)
                playerMovement.ableMove = false; // Disable movement while in camera
            else playerMovement.ableMove = true;
        }
    }
}
