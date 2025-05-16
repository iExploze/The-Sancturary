using UnityEngine;
using Photon.Pun;

public class GeoAI : MonsterBase
{
    private int geoMonsterID = 1;

    protected override void Start()
    {
        MonsterID = geoMonsterID;
        base.Start();
        currentState = MonsterState.Chill;
        // no extra setup
    }

    // we don’t need Kill/Return logic yet, so leave everything else on the base
}
