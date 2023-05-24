using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class HenryController : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private Canvas jumpscareCanvas;
    [SerializeField] private VideoPlayer jumpscareVideo;
    [SerializeField] private GameObject monsterSprite;
    public float attackDistanceThreshold = 0.8f;
    public float wanderAreaRadius = 10f;
    public string deathSceneName = "End Scene";

    public float normalSpeed = 2.2f;

    public string survivalPlayerPrefKey = "Survived";
    private UnityEngine.AI.NavMeshAgent agent;
    private Animator animator;

    public float chaseRadius = 20f;
    public float viewRadius = 4;

    private Vector2 Destination;

    private float custodianRoomRadius = 9f;
    
    [SerializeField] private AudioSource transitionAudioSource;

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
        agent.acceleration = 20f;
        agent.angularSpeed = 600f;

        currentState = State.Roam;
        agent.speed = normalSpeed;
        Destination = RandomLocation();
    }

    // Update is called once per frame
    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                currentState = State.Roam;
                break;
            case State.Roam:
                if (playerInViewRange() && !isPlayerHiding() && !isPlayerInCustodian() && isPlayerMoving())
                {
                    StartCoroutine(TransitionToAngry());
                    currentState = State.Chase;
                    break;
                }
                else if (HasReachedDestination())
                {
                    Destination = RandomLocation();
                }
                break;
            case State.Chase:
                setPlayerChased(true);
                Destination = playerLocation();
                if(isPlayerHiding())
                {
                    StartCoroutine(TransitionToNormal());
                    Destination = RandomLocation();
                    setPlayerChased(false);
                    currentState = State.Roam;
                    break;
                }
                else if (isPlayerInCustodian())
                {
                    Destination = getPlayerCustodianLocation();
                    if (inCustodianRaidus())
                    {
                        StartCoroutine(TransitionToNormal());
                        Destination = RandomLocation();
                        setPlayerChased(false);
                        currentState = State.Roam;
                        break;
                    }
                }
                else
                {
                    if (playerOutOfChaseRange())
                    {
                        StartCoroutine(TransitionToNormal());
                        Destination = RandomLocation();
                        setPlayerChased(false);
                        currentState = State.Roam;
                        break;
                    }
                    else if (playerInAttackRange())
                    {
                        currentState = State.Attack;
                        break;
                    }
                }
                break;
            case State.Attack:
                Destination = playerLocation();
                if(!isPlayerHiding() && !isPlayerInCustodian())
                {
                    kill();
                }
                StartCoroutine(TransitionToNormal());
                setPlayerChased(false);
                currentState = State.Roam;
                break;
        }

        agent.SetDestination(Destination);
        Vector2 velocity = agent.velocity;

        animator.SetFloat("Speed", velocity.sqrMagnitude); 

        // Constrain rotation on the Y and Z axes for the parent GameObject
        transform.rotation = Quaternion.Euler(0, 0, 0);

        // Constrain angular movement for the MonsterSprite GameObject
        monsterSprite.transform.localRotation = Quaternion.identity;
    }

    private bool isPlayerMoving()
    {
        return player.GetComponent<PlayerMovement>().isMoving;
    }


    private bool inCustodianRaidus()
    {
        float distanctToCustodian = Vector2.Distance(getPlayerCustodianLocation(), transform.position);
        if(distanctToCustodian <= custodianRoomRadius)
        {
            return true;
        }
        return false;
    }

    private bool isPlayerInCustodian()
    {
        return player.GetComponent<PlayerMovement>().isInCustodianRoom;
    }

    private Vector2 getPlayerCustodianLocation()
    {
        if(isPlayerInCustodian())
            return player.GetComponent<PlayerMovement>().custodianRoomLoc;
        else
            Debug.Log("Player Not In Custodian Room");
            return Vector2.one;
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
        // Trigger the jumpscare video
        CanvasGroup canvasGroup = jumpscareCanvas.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        jumpscareVideo.Play();

        // Set the "Speed" parameter to zero to stop the walking animation
        animator.SetFloat("Speed", 0f);
        // Wait for the jumpscare video to finish, then transition to the death scene
        StartCoroutine(WaitForJumpscare());
    }

    private Vector2 RandomLocation()
    {
        // Get a random point within the specified area around the monster's current position
        Vector2 randomPoint = (Vector2)transform.position + Random.insideUnitCircle * wanderAreaRadius;

        // Set the destination of the agent to the random point
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 2.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return RandomLocation();
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

    IEnumerator TransitionToAngry()
    {
        agent.speed = 0;
        animator.SetTrigger("TransitionToAngry");
        transitionAudioSource.Play();
        yield return new WaitForSeconds(0.5f);
        agent.speed = normalSpeed;
        animator.ResetTrigger("TransitionToAngry");
    }

    IEnumerator TransitionToNormal()
    {
        agent.speed = 0;
        animator.SetTrigger("TransitionToNormal");
        yield return new WaitForSeconds(0.5f);
        agent.speed = normalSpeed;
        animator.ResetTrigger("TransitionToNormal");
    }

    IEnumerator WaitForJumpscare()
    {
        // Wait for the length of the jumpscare video
        yield return new WaitForSeconds((float)jumpscareVideo.length);

        // Set the PlayerPrefs value for the survival status to 0 (not survived)
        PlayerPrefs.SetInt(survivalPlayerPrefKey, 0);
        PlayerPrefs.Save();

        // Load the death scene
        SceneManager.LoadScene(deathSceneName);
    }

    private void setPlayerChased(bool temp)
    {
        player.GetComponent<PlayerMovement>().isChased = temp;
    }
}
