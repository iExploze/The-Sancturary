using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    public Rigidbody2D Rb;
    public Animator animator;

    public AudioSource walkingSound;

    Vector2 movement;

    void Update()
    {
        if (Input.GetAxisRaw("Horizontal") != 0f)
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = 0f;
    }
    else if (Input.GetAxisRaw("Vertical") != 0f)
    {
        movement.y = Input.GetAxisRaw("Vertical");
        movement.x = 0f;
    }
    else
    {
        movement.x = 0f;
        movement.y = 0f;
    }

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        if (movement.x != 0 || movement.y != 0)
        {
            if (!walkingSound.isPlaying)
            {
                walkingSound.Play();
            }
        }
        else
        {
            walkingSound.Stop();
        }
    }

    void FixedUpdate()
    {
        Rb.MovePosition(Rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
