using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public abstract class MonsterBase : MonoBehaviourPun
{
    public enum MonsterState { Chill, Hunt, Kill, Return }
    [SerializeField] protected MonsterState currentState = MonsterState.Chill;

    [Header("Monster Parameters")]
    public float detectionRange = 10f;
    public float killRange = 1.5f;
    public float waitAtPoint = 2f;
    public Transform[] patrolPoints;

    protected int patrolIndex = 0;
    protected float waitTimer = 0f;
    protected NavMeshAgent agent;
    protected Vector3 startPosition;

    // Player targeting (multiplayer)
    protected Transform targetPlayer; // Who we're chasing (can be extended to list for co-op)
    protected GameObject[] allPlayers;

    protected int MonsterID;

    public virtual int returnID() 
    {
        return MonsterID;
    }

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        startPosition = transform.position;
    }

    protected virtual void Start()
    {
        // Multiplayer: Find all players on the scene (customize for Photon)
        allPlayers = GameObject.FindGameObjectsWithTag("Player");
        // Optionally, assign targetPlayer now, or later when one is detected
    }

    protected virtual void Update()
    {
        // ============ PHOTON MULTIPLAYER HOOK ============
        if (!PhotonNetwork.IsMasterClient) return; // Only master runs AI

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

    // ========== STATE LOGIC ==========

    protected virtual void ChillUpdate()
    {
        // Patrol logic (or idle at startPosition)
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            if (Vector3.Distance(transform.position, patrolPoints[patrolIndex].position) < 0.5f)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer >= waitAtPoint)
                {
                    patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                    waitTimer = 0f;
                }
            }
            else
            {
                agent.SetDestination(patrolPoints[patrolIndex].position);
            }
        }
        else
        {
            agent.SetDestination(startPosition);
        }

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

        float distance = Vector3.Distance(transform.position, targetPlayer.position);
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
        agent.SetDestination(startPosition);
        if (Vector3.Distance(transform.position, startPosition) < 1f)
            SwitchState(MonsterState.Chill);
    }

    // ========== UTILITIES ==========

    protected virtual bool PlayerInRange(Transform player, float range)
    {
        return Vector3.Distance(transform.position, player.position) <= range;
    }

    protected virtual Transform FindNearestPlayer()
    {
        Transform nearest = null;
        float minDist = float.MaxValue;
        foreach (var go in allPlayers)
        {
            if (go == null) continue;
            float dist = Vector3.Distance(transform.position, go.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = go.transform;
            }
        }
        return nearest;
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

    protected virtual void OnTouchDoor() 
    {
        
    }
}
