using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    public Rigidbody2D Rb;
    public Animator animator;

    public AudioSource walkingSound;

    public bool isHiding = false;

    Vector2 movement;

    public Light2D lightToControl;

    private void Start()
    {
        
    }

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

        if (isHiding)
        {
            lightToControl.pointLightInnerRadius = 0.1f;
            lightToControl.pointLightOuterRadius = 1f;
        }
        else
        {
            lightToControl.pointLightInnerRadius = 1f;
            lightToControl.pointLightOuterRadius = 3.25f;
        }
    }

    void FixedUpdate()
    {
        if (!isHiding) // Only allow movement if not hiding
        {
            Rb.MovePosition(Rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
