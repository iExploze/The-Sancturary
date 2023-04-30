using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MonsterNavMeshMovement : MonoBehaviour
{
    public GameObject player;
    public Canvas jumpscareCanvas;
    public VideoPlayer jumpscareVideo;
    public GameObject monsterSprite;
    public float jumpscareDistanceThreshold = 1f;

    private UnityEngine.AI.NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = monsterSprite.GetComponent<Animator>();
        jumpscareCanvas.GetComponent<CanvasGroup>().alpha = 0;

        // Adjust the NavMeshAgent2D parameters for smoother movement
        agent.acceleration = 20f;
        agent.angularSpeed = 600f;
        agent.autoBraking = false;
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer > jumpscareDistanceThreshold)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            float horizontalDistance = Mathf.Abs(player.transform.position.x - transform.position.x);
            float verticalDistance = Mathf.Abs(player.transform.position.y - transform.position.y);

            Vector3 destination;

            if (horizontalDistance > verticalDistance)
            {
                destination = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
            }
            else
            {
                destination = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
            }

            agent.SetDestination(destination);

            Vector2 velocity = agent.velocity;
            animator.SetFloat("Horizontal", velocity.x);
            animator.SetFloat("Vertical", velocity.y);
            animator.SetFloat("Speed", velocity.sqrMagnitude);
        }
        else
        {
            // Stop the monster movement
            agent.isStopped = true;

            // Trigger the jumpscare video
            CanvasGroup canvasGroup = jumpscareCanvas.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1;
            jumpscareVideo.Play();

            // Set the "Speed" parameter to zero to stop the walking animation
            animator.SetFloat("Speed", 0f);
        }

        // Constrain rotation on the Y and Z axes for the parent GameObject
        transform.rotation = Quaternion.Euler(0, 0, 0);

        // Constrain angular movement for the MonsterSprite GameObject
        monsterSprite.transform.localRotation = Quaternion.identity;
    }
}
