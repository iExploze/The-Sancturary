using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Photon.Pun;

public class JumpscarePlayer : MonoBehaviourPun
{
    [Header("Jumpscare Settings")]
    public VideoPlayer jumpscarePlayer;
    public Canvas jumpscareCanvas;

    [Header("Jumpscare Videos by Monster ID")]
    [SerializeField] private List<VideoClip> jumpscareVideos;

    private void Start()
    {
        // Hide the jumpscare canvas initially
        if (jumpscareCanvas != null)
            jumpscareCanvas.enabled = false;
    }

    //public void Update()
    //{
    //    if (Input.GetKeyDown("j")) 
    //    {
    //        TriggerJumpscare(1);
    //    }
    //}

    // Public method to trigger jumpscare by Monster ID
    public void TriggerJumpscare(int monsterID)
    {
        // Ensure only the local player sees the jumpscare
        if (!photonView.IsMine) return;

        if (monsterID < 0 || monsterID > jumpscareVideos.Count)
        {
            Debug.LogWarning("Invalid Monster ID: " + monsterID);
            return;
        }


        // Set the appropriate video clip based on Monster ID
        jumpscarePlayer.clip = jumpscareVideos[monsterID - 1];
        jumpscarePlayer.Play();

        // Display the canvas
        if (jumpscareCanvas != null)
            jumpscareCanvas.enabled = true;

        // Hide the jumpscare after the video length
        Invoke(nameof(HideJumpscare), (float)jumpscareVideos[monsterID-1].length);
    }

    // Hide the jumpscare canvas
    private void HideJumpscare()
    {
        if (jumpscareCanvas != null)
            jumpscareCanvas.enabled = false;
    }
}
