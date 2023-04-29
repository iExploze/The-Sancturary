using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Monster1Movement : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject
    public float chaseSpeed = 1.1f;
    public float accelerationRate = 0.1f;
    public float decelerationRate = 0.2f;
    public float maxSpeedMultiplier = 2f;

    public Canvas jumpscareCanvas;
    public VideoPlayer jumpscareVideo;
    public float jumpscareDistanceThreshold = 1f;

    private Rigidbody2D monsterRb;
    private Animator animator; // Reference to the Animator component
    private Vector2 lastDirection;
    private float speedMultiplier = 1f;

    void Start()
    {
        monsterRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Get the Animator component from the monster
        jumpscareCanvas.GetComponent<CanvasGroup>().alpha = 0; // Hide the jumpscare video at the start
        jumpscareVideo.loopPointReached += OnJumpscareVideoFinished;
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

            // Check if the movement direction has changed
            if (Vector2.Dot(lastDirection, direction) < 0.99f)
            {
                // Decrease the speed multiplier when the direction changes
                speedMultiplier = Mathf.Max(1f, speedMultiplier - decelerationRate * Time.deltaTime);
            }
            else
            {
                // Increase the speed multiplier while walking in the same direction
                speedMultiplier = Mathf.Min(maxSpeedMultiplier, speedMultiplier + accelerationRate * Time.deltaTime);
            }

            monsterRb.MovePosition(monsterRb.position + direction * chaseSpeed * speedMultiplier * Time.deltaTime);

            // Update the Animator parameters
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
            animator.SetFloat("Speed", direction.sqrMagnitude);

            lastDirection = direction;
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

    public void OnJumpscareVideoFinished(VideoPlayer vp)
    {
        Application.Quit();
    }
}
