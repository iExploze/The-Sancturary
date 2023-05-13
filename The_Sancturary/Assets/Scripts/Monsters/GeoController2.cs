using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class GeoController2 : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private Canvas jumpscareCanvas;
    [SerializeField] private VideoPlayer jumpscareVideo;
    [SerializeField] private GameObject monsterSprite;
    public float attackDistanceThreshold = 0.8f;
    public float wanderAreaRadius = 10f;
    public string deathSceneName = "End Scene";

    private bool isSlowed = false;
    public float slowedSpeed = 0.5f;
    public float normalSpeed = 2f;
    public string survivalPlayerPrefKey = "Survived";
    private UnityEngine.AI.NavMeshAgent agent;
    private Animator animator;

    public float chaseRadius = 20f;
    public float viewRadius = 4;
    private bool hasSeenPlayer = false;

    private Vector2 startSpot;
    private Vector2 Destination;
    private enum State
    {
        Idle,
        Roam,
        Chase,
        Attack
    }

    private State currentState;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        animator = monsterSprite.GetComponent<Animator>();
        jumpscareCanvas.GetComponent<CanvasGroup>().alpha = 0;
    }

    private void Start()
    {
        currentState = State.Roam;
        startSpot = transform.position;
        agent.speed = normalSpeed;
        Destination = RandomLocation();
    }

    // Update is called once per frame
    private void Update()
    {
        Debug.Log(currentState);
        Debug.Log(playerInViewRange());
        switch (currentState)
        {
            case State.Idle:
                currentState = State.Roam;
                break;
            case State.Roam:
                StopCoroutine(SlowDownMonster());
                if(playerInViewRange() && !isPlayerHiding())
                {
                    hasSeenPlayer = true;
                    currentState = State.Chase;
                    break;
                }
                else if(HasReachedDestination())
                {
                    Destination = RandomLocation();
                }
                break;
            case State.Chase:
                Destination = playerLocation();
                if(!isSlowed)
                {
                    StartCoroutine(SlowDownMonster());
                }
                if(!playerInViewRange())
                {
                    if(isPlayerHiding())
                    {
                        hasSeenPlayer = false;
                    }
                    if(playerOutOfChaseRange())
                    {
                        hasSeenPlayer = false;
                        currentState = State.Roam;
                        break;
                    }
                }
                else if(playerInAttackRange())
                {
                    currentState = State.Attack;
                    break;
                }
                break;
            case State.Attack:
                Destination = playerLocation();
                if(hasSeenPlayer)
                    kill();
                else
                    hasSeenPlayer = false;
                    currentState = State.Roam;
                break;
        }

        // Set the animator "Speed" parameter to a non-zero value to play the walking animation
        agent.SetDestination(Destination);

        Vector2 velocity = agent.velocity;
        animator.SetFloat("Horizontal", velocity.x);
        animator.SetFloat("Vertical", velocity.y);
        animator.SetFloat("Speed", velocity.sqrMagnitude);

        // Constrain rotation on the Y and Z axes for the parent GameObject
        transform.rotation = Quaternion.Euler(0, 0, 0);

        // Constrain angular movement for the MonsterSprite GameObject
        monsterSprite.transform.localRotation = Quaternion.identity;
    }

    private bool isPlayerHiding()
    {
        return player.GetComponent<PlayerMovement>().isHiding;
    }

    private bool HasReachedDestination()
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
    private Vector2 playerLocation()
    {
        return player.transform.position;
    }

    private bool playerInAttackRange()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if(distanceToPlayer <= attackDistanceThreshold)
        {
            return true;
        }
        return false;
    }

    private bool playerOutOfChaseRange()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if(distanceToPlayer > chaseRadius)
        {
            return true;
        }
        return false;
    }

    private void kill()
    {
        // Stop the monster movement
        agent.isStopped = true;
        if(!isPlayerHiding())
        {
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
    }

    private Vector2 RandomLocation()
    {
        // Get a random point within the specified area around the start spot
        Vector2 randomPoint = startSpot + Random.insideUnitCircle * wanderAreaRadius;

        // Set the destination of the agent to the random point
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 2.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return RandomLocation();
    }

    IEnumerator SlowDownMonster()
    {
        isSlowed = true;

        // Wait for a random duration between 10 and 20 seconds
        float chaseDuration = Random.Range(10f, 20f);
        yield return new WaitForSeconds(chaseDuration);
    
        // Slow down the monster
        agent.speed = slowedSpeed;

        // Wait for a random duration between 10 and 20 seconds
        float slowDuration = Random.Range(10f, 20f);
        yield return new WaitForSeconds(slowDuration);

        // Restore the monster's speed
        agent.speed = normalSpeed;

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

    private bool IsOutsideWanderArea()
    {
        float distanceToStart = Vector2.Distance(transform.position, startSpot);
        return distanceToStart > wanderAreaRadius;
    }

    private Vector2 startLocation()
    {
        return startSpot;
    }


    private bool playerInViewRange()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= viewRadius)
        {
            return true;
        }

        return false;
    }
}