using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster1Movement : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject
    public float chaseSpeed = 3f;
    public float accelerationRate = 0.1f;
    public float decelerationRate = 0.2f;
    public float maxSpeedMultiplier = 2f;

    private Rigidbody2D monsterRb;
    private Animator animator; // Reference to the Animator component
    private Vector2 lastDirection;
    private float speedMultiplier = 1f;

    void Start()
    {
        monsterRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // Get the Animator component from the monster
    }

    void Update()
    {
        ChasePlayer(); // Call the ChasePlayer function in Update
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
}