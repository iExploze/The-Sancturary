using UnityEngine;
using Photon.Pun;

public class GeoAI : MonsterBase
{
    private int geoMonsterID = 1;

    protected override void Awake()
    {
        base.Awake();
        MonsterID = geoMonsterID;
        // start in “hunt” so we never patrol
        currentState = MonsterState.Chill;
    }

    protected override void Start()
    {
        base.Start();
        // no extra setup
    }

    protected override void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        //if (!PhotonNetwork.IsMasterClient) return;  // only the host drives AI

        // find closest player and chase
        var nearest = FindNearestPlayer();
        if (nearest != null)
        {
            agent.SetDestination(nearest.position);
        }
    }

    // we don’t need Kill/Return logic yet, so leave everything else on the base
}
