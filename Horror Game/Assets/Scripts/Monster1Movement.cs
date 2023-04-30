using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Monster1Movement : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject
    public Canvas jumpscareCanvas;
    public VideoPlayer jumpscareVideo;
    public float jumpscareDistanceThreshold = 1f;
    public float MonsterSpeed = 2;

    private Rigidbody2D monsterRb;
    private Animator animator; // Reference to the Animator component

    void Start()
    {
        monsterRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Get the Animator component from the monster
        jumpscareCanvas.GetComponent<CanvasGroup>().alpha = 0; // Hide the jumpscare video at the start
    }

    void Update()
    {
        ChasePlayer(); // Call the ChasePlayer function in Update
        CheckForJumpscare(); // Check if the monster is close enough to trigger a jumpscare
    }

    // Function to make the monster chase the player
    void ChasePlayer()
    {
        if (player != null)
        {
            Vector2 direction = (Vector2)player.transform.position - (Vector2)transform.position;
            direction.Normalize();

            // Restrict diagonal movement by setting one of the components to zero
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                direction.y = 0;
            }
            else
            {
                direction.x = 0;
            }


            monsterRb.MovePosition(monsterRb.position + direction * MonsterSpeed * Time.fixedDeltaTime);

            // Update the Animator parameters
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
            animator.SetFloat("Speed", 1);
        }
    }

    void CheckForJumpscare()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= jumpscareDistanceThreshold)
        {
            // Trigger the jumpscare video
            CanvasGroup canvasGroup = jumpscareCanvas.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1; // Set the canvas alpha to 1 (fully visible)
            jumpscareVideo.Play();
        }
    }
}