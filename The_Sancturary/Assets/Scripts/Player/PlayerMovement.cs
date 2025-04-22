using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Photon.Pun;
public class PlayerMovement : MonoBehaviourPun
{
    public bool isInCustodianRoom;
    public bool isHiding;
    public bool isChased;
    public Vector3 custodianRoomLoc;


    [Header("Movement")]
    private Vector2 movement;
    public float speed;

    [Header("Visuals")]
    public Animator animator;

    [Header("Audio")]
    public AudioSource walkingSound;

    [Header("Gameplay States")]
    public bool isMoving = false;

    private void Start()
    {

    }

    void Update()
    {
        if (!photonView.IsMine) return;

        // Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normalize diagonal
        if (movement.x != 0 && movement.y != 0)
            movement.Normalize();

        transform.Translate(movement * speed * Time.deltaTime);

        // Animation
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        // Walking sound
        bool currentlyMoving = movement.sqrMagnitude > 0;

        if (currentlyMoving && !walkingSound.isPlaying)
        {
            walkingSound.Play();
        }
        else if (!currentlyMoving && walkingSound.isPlaying)
        {
            walkingSound.Stop();
        }
    }
}
