using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class GeorgyController : MonoBehaviour
{
    private GameObject player;
    public Canvas jumpscareCanvas;
    public VideoPlayer jumpscareVideo;
    public GameObject monsterSprite;
    public float jumpscareDistanceThreshold = 1f;
    public float wanderAreaRadius = 10f;
    public string deathSceneName;

    private bool isSlowed = false;
    public float slowDownDistance = 10f;

    public float slowedSpeed = 0.8f;

    public float normalSpeed = 1.8f;
    public string survivalPlayerPrefKey = "Survived";
    private UnityEngine.AI.NavMeshAgent agent;
    private Animator animator;

    public float chaseRadius = 20f;
    public float viewRadius = 10f;
    private bool hasSeenPlayer = false;

    private bool returningToLastSeenSpot = false;
    private Vector3 lastSeenSpot;

    private Vector2 startspot;

    void Start()
    {
        startspot = transform.position;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = monsterSprite.GetComponent<Animator>();
        jumpscareCanvas.GetComponent<CanvasGroup>().alpha = 0;
        player = GameObject.FindGameObjectWithTag("Player");

        // Adjust the NavMeshAgent2D parameters for smoother movement
        agent.acceleration = 20f;
        agent.angularSpeed = 600f;
        agent.autoBraking = false;

        // Make the monster wander right away at the start
        Wanderer();
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer < jumpscareDistanceThreshold && CanSeePlayer())
        {
            Jumpscare();
        }
        if (CanSeePlayer())
        {
            lastSeenSpot = player.transform.position;
            chasePlayer();
        }
        else if (distanceToPlayer > viewRadius && distanceToPlayer < chaseRadius)
        {
            if (player.GetComponent<PlayerMovement>().isHiding && hasSeenPlayer && !returningToLastSeenSpot)
            {
                agent.SetDestination(lastSeenSpot);
                returningToLastSeenSpot = true;
            }
            else if (!player.GetComponent<PlayerMovement>().isHiding && hasSeenPlayer)
            {
                chasePlayer();
            }
            else if (HasReachedDestination() || player.GetComponent<PlayerMovement>().isInCustodianRoom)
            {
                Wanderer();
            }
        }
        else
        {
            if (IsOutsideWanderArea())
            {
                ReturnToWanderArea();
            }
            else if (HasReachedDestination() || player.GetComponent<PlayerMovement>().isInCustodianRoom)
            {
                Wanderer();
                hasSeenPlayer = false;
            }
        }

        // Constrain rotation on the Y and Z axes for the parent GameObject
        transform.rotation = Quaternion.Euler(0, 0, 0);

        // Constrain angular movement for the MonsterSprite GameObject
        monsterSprite.transform.localRotation = Quaternion.identity;
    }
    bool HasReachedDestination()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }
    void chasePlayer()
    {
        agent.SetDestination(player.transform.position);

        Vector2 velocity = agent.velocity;
        animator.SetFloat("Horizontal", velocity.x);
        animator.SetFloat("Vertical", velocity.y);
        animator.SetFloat("Speed", velocity.sqrMagnitude);

        if (!isSlowed)
        {
            StartCoroutine(SlowDownMonster());
        }
    }

    void Jumpscare()
    {
        // Stop the monster movement
                agent.isStopped = true;

                // Trigger the jumpscare video
                CanvasGroup canvasGroup = jumpscareCanvas.GetComponent<CanvasGroup>();
                canvasGroup.alpha = 1;
                jumpscareVideo.Play();

                // Set the "Speed" parameter to zero to stop the walking animation
                animator.SetFloat("Speed", 0f);
                // Wait for the jump
                // Wait for the jumpscare video to finish, then transition to the death scene
                StartCoroutine(WaitForJumpscare());
    }

    void Wanderer()
    {
        // Get a random point within the specified area around the start spot
        Vector2 randomPoint = startspot + Random.insideUnitCircle * wanderAreaRadius;

        // Set the destination of the agent to the random point
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 2.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        // Set the animator "Speed" parameter to a non-zero value to play the walking animation
        Vector2 velocity = agent.velocity;
        animator.SetFloat("Horizontal", velocity.x);
        animator.SetFloat("Vertical", velocity.y);
        animator.SetFloat("Speed", velocity.sqrMagnitude);
    }

    IEnumerator SlowDownMonster()
    {
        isSlowed = true;

        // Wait for a random duration between 10 and 20 seconds
        float chaseDuration = Random.Range(10f, 20f);
        yield return new WaitForSeconds(chaseDuration);

        // Check if the monster is close enough to the player
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= slowDownDistance)
        {
            // Slow down the monster
            agent.speed = slowedSpeed;

            // Wait for a random duration between 10 and 20 seconds
            float slowDuration = Random.Range(10f, 20f);
            yield return new WaitForSeconds(slowDuration);

            // Restore the monster's speed
            agent.speed = normalSpeed;
        }

        isSlowed = false;
    }

    IEnumerator WaitForJumpscare()
    {
        // Wait for the length of the jumpscare video
        yield return new WaitForSeconds((float)jumpscareVideo.length-0.3f);

        // Set the PlayerPrefs value for the survival status to 0 (not survived)
        PlayerPrefs.SetInt(survivalPlayerPrefKey, 0);
        PlayerPrefs.Save();

        // Load the death scene
        SceneManager.LoadScene(deathSceneName);
    }

    bool IsOutsideWanderArea()
    {
        float distanceToStart = Vector2.Distance(transform.position, startspot);
        return distanceToStart > wanderAreaRadius;
    }

    void ReturnToWanderArea()
    {
        agent.SetDestination(startspot);

        Vector2 velocity = agent.velocity;
        animator.SetFloat("Horizontal", velocity.x);
        animator.SetFloat("Vertical", velocity.y);
        animator.SetFloat("Speed", velocity.sqrMagnitude);
    }


    bool CanSeePlayer()
    {
        if(player.GetComponent<PlayerMovement>().isHiding)
        {
            return false;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= viewRadius)
        {
            hasSeenPlayer = true; // Set this variable to true when the player is seen
            return true;
        }

        return false;
    }
}
