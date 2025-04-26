using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public abstract class MonsterBaseAI : MonoBehaviourPun
{
    protected enum MonsterState { Idle, Roam, Chase, Attack }
    protected MonsterState currentState = MonsterState.Idle;

    protected Transform player;
    protected NavMeshAgent agent;
    protected Animator animator;

    [Header("Detection")]
    public float viewRadius = 4f;
    public float chaseRadius = 10f;
    public float attackRange = 0.8f;
    public LayerMask playerMask;

    [Header("Roaming")]
    public float roamRadius = 8f;
    protected Vector3 spawnPoint;
    protected Vector3 roamTarget;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        spawnPoint = transform.position;

        agent.updateUpAxis = false;     // ← keeps Y as vertical (no X-axis flipping)
        agent.updateRotation = false;   // ← stops the agent from rotating your object entirely

    }

    protected Transform FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closest = null;
        float closestDistance = Mathf.Infinity;
        Debug.Log("playercount: " + players);
        foreach (GameObject p in players)
        {
            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < closestDistance)
            {
                closest = p.transform;
                closestDistance = dist;
            }
        }

        return closest;
    }


    protected virtual void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        ChangeState(MonsterState.Roam);
    }

    protected virtual void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // Always find the closest player
        player = FindClosestPlayer();

        switch (currentState)
        {
            case MonsterState.Idle:
                OnIdle();
                break;
            case MonsterState.Roam:
                OnRoam();
                break;
            case MonsterState.Chase:
                OnChase();
                break;
            case MonsterState.Attack:
                OnAttack();
                break;
        }

        Debug.Log("Current state: " + currentState);
        UpdateAnimation();
    }


    protected void ChangeState(MonsterState newState)
    {
        currentState = newState;
    }

    protected virtual void OnIdle()
    {
        ChangeState(MonsterState.Roam);
    }

    protected virtual void OnRoam()
    {
        if (ReachedDestination())
            roamTarget = GetRandomRoamPosition();

        if (PlayerInSight())
            ChangeState(MonsterState.Chase);
        else
            agent.SetDestination(roamTarget);
    }

    protected virtual void OnChase()
    {
        if (player == null)
        {
            ChangeState(MonsterState.Roam);
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > chaseRadius)
        {
            ChangeState(MonsterState.Roam);
            return;
        }

        agent.SetDestination(player.position);

        if (dist <= attackRange)
            ChangeState(MonsterState.Attack);
    }

    protected virtual void OnAttack()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > attackRange)
        {
            ChangeState(MonsterState.Chase);
            return;
        }

        // Call to abstract function — each monster can define its own kill behavior
        PerformAttack();
    }

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

    protected bool PlayerInSight()
    {
        if (player == null) return false;
        float dist = Vector3.Distance(transform.position, player.position);
        return dist <= viewRadius;
    }

    protected bool ReachedDestination()
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }

    protected Vector3 GetRandomRoamPosition()
    {
        Vector2 randomOffset = Random.insideUnitCircle * roamRadius;
        Vector3 randomPoint = spawnPoint + new Vector3(randomOffset.x, 0, randomOffset.y);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 2f, NavMesh.AllAreas))
            return hit.position;

        return spawnPoint;
    }

    // Override this for custom attack behavior
    protected abstract void PerformAttack();


}
