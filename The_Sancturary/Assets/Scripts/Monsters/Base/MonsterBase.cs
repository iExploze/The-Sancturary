using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public abstract class MonsterBase : MonoBehaviourPun
{
    public enum MonsterState { Chill, Hunt, Kill, Return }
    [SerializeField] protected MonsterState currentState = MonsterState.Chill;

    [Header("Monster base settings")]
    public float detectionRange = 10f;
    public float killRange = 1.5f;
    public Transform[] patrolPoints;

    protected int patrolIndex = 0;
    protected NavMeshAgent agent;
    protected Animator animator;

    // Player targeting (multiplayer)
    protected Transform targetPlayer; // Who we're chasing (can be extended to list for co-op)
    protected GameObject[] allPlayers;

    protected private int MonsterID;

    public virtual int returnID() 
    {
        return MonsterID;
    }

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        agent.updateUpAxis = false;     // ← keeps Y as vertical (no X-axis flipping)
        agent.updateRotation = false;   // ← stops the agent from rotating your object entirely
    }

    protected virtual void Start()
    {
        // Multiplayer: Find all players on the scene (customize for Photon)
        allPlayers = GameObject.FindGameObjectsWithTag("Player");

        // ============ PHOTON MULTIPLAYER HOOK ============
        // also good for rapid testing
        if (PhotonNetwork.IsConnected)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            // Only master runs AI
        }
        SwitchState(MonsterState.Chill);
        // Optionally, assign targetPlayer now, or later when one is detected
    }

    protected virtual void Update()
    {
        UpdateAnimation();
        switch (currentState)
        {
            case MonsterState.Chill:
                ChillUpdate();
                break;
            case MonsterState.Hunt:
                HuntUpdate();
                break;
            case MonsterState.Kill:
                KillUpdate();
                break;
            case MonsterState.Return:
                ReturnUpdate();
                break;
        }
    }

    // Animation updating method
    protected virtual void UpdateAnimation()
    {
        Vector3 velocity = agent.velocity;
        if (animator != null)
        {
            animator.SetFloat("Horizontal", velocity.x);
            animator.SetFloat("Vertical", velocity.y);
            animator.SetFloat("Speed", velocity.sqrMagnitude);
        }
    }

    // ========== STATE LOGIC ==========

    protected virtual void ChillUpdate()
    {
        // Check if the monster has reached the current patrol point
        if (Vector2.Distance(transform.position, patrolPoints[patrolIndex].position) < 1f)
        {
            // Move to the next patrol point
            patrolIndex++;

            // If reached the last point, go back to the first point
            if (patrolIndex >= patrolPoints.Length)
            {
                patrolIndex = 0;
            }
        }
        // Debug.Log(Vector2.Distance(transform.position, patrolPoints[patrolIndex].position));
        // Set the destination to the current patrol point
        agent.SetDestination(patrolPoints[patrolIndex].position);

        // Check for players in range
        var p = FindNearestPlayer();
        if (p != null && PlayerInRange(p, detectionRange))
        {
            targetPlayer = p;
            SwitchState(MonsterState.Hunt);
        }
    }

    protected virtual void HuntUpdate()
    {
        if (targetPlayer == null || !PlayerInRange(targetPlayer, detectionRange * 1.5f))
        {
            // Lost player: return
            targetPlayer = null;
            SwitchState(MonsterState.Return);
            return;
        }

        agent.SetDestination(targetPlayer.position);

        float distance = Vector2.Distance(transform.position, targetPlayer.position);
        if (distance <= killRange)
        {
            SwitchState(MonsterState.Kill);
        }
    }

    protected virtual void KillUpdate()
    {
        agent.ResetPath();
        if (targetPlayer != null)
        {
            // Networked kill/jumpscare logic here
            PerformKill(targetPlayer.gameObject);
        }
        SwitchState(MonsterState.Return);
    }

    protected virtual void ReturnUpdate()
    {
        agent.SetDestination(patrolPoints[0].position);
        if (Vector3.Distance(transform.position, patrolPoints[0].position) < 1f)
            SwitchState(MonsterState.Chill);
    }

    // ========== UTILITIES ==========

    protected virtual bool PlayerInRange(Transform player, float range)
    {
        return Vector2.Distance(transform.position, player.position) <= range;
    }

    protected virtual Transform FindNearestPlayer()
    {
        Transform closest = null;
        float closestDistance = Mathf.Infinity;
        //Debug.Log("playercount: " + players);
        foreach (GameObject p in allPlayers)
        {
            // Get the PlayerMovement component
            PlayerMovement playerMovement = p.GetComponent<PlayerMovement>();
            // Check if the player exists and is not a ghost
            if (playerMovement != null && !playerMovement.isGhost())
            {
                float dist = Vector2.Distance(transform.position, p.transform.position);
                if (dist < closestDistance)
                {
                    closest = p.transform;
                    closestDistance = dist;
                }
            }

        }

            return closest;
    }

    protected virtual void SwitchState(MonsterState newState)
    {
        currentState = newState;
        // Optionally trigger animation or sound here!
    }

    // This is where you will override for each monster type!
    protected virtual void PerformKill(GameObject obj)
    {

        Debug.Log($"{name} performed kill on {obj.name}");
    }

    public virtual void OnDoorTouch() 
    {
        patrolIndex++;
        // If reached the last point, go back to the first point
        if (patrolIndex >= patrolPoints.Length)
        {
            patrolIndex = 0;
        }
        currentState = MonsterState.Chill;
    }
}
