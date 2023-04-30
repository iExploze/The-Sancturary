using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Video;

public class MonsterNavMeshMovement : MonoBehaviour
{
    public GameObject player;
    public float chaseDistanceThreshold = 5f;
    public float jumpscareDistanceThreshold = 1f;

    public Canvas jumpscareCanvas;
    public VideoPlayer jumpscareVideo;

    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        jumpscareCanvas.GetComponent<CanvasGroup>().alpha = 0; // Hide the jumpscare video at the start
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= chaseDistanceThreshold)
            {
                agent.SetDestination(player.transform.position);
            }
            else
            {
                agent.ResetPath();
            }

            RestrictDiagonalMovement();

            UpdateAnimator(agent.velocity);

            // Check for jumpscare
            if (distanceToPlayer <= jumpscareDistanceThreshold)
            {
                TriggerJumpscare();
            }
        }
    }

    private void RestrictDiagonalMovement()
    {
        Vector2 direction = agent.desiredVelocity;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            direction.y = 0;
        }
        else
        {
            direction.x = 0;
        }

        agent.velocity = direction.normalized * agent.speed;
    }

    private void UpdateAnimator(Vector2 velocity)
    {
        animator.SetFloat("Horizontal", velocity.x);
        animator.SetFloat("Vertical", velocity.y);
        animator.SetFloat("Speed", velocity.sqrMagnitude);
    }

    void TriggerJumpscare()
    {
        CanvasGroup canvasGroup = jumpscareCanvas.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1; // Set the canvas alpha to 1 (fully visible)
        jumpscareVideo.Play();
    }
}
