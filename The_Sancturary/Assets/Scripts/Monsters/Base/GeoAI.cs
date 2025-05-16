using UnityEngine;
using Photon.Pun;

public class GeoAI : MonsterBase
{

    protected override void Start()
    {
        MonsterID = 1;
        base.Start();
        currentState = MonsterState.Chill;
        // no extra setup
    }

    protected override void Update() 
    {
        base.Update();


    }

    // the override for killing the player
    protected override void PerformKill(GameObject obj)
    {
        base.PerformKill(obj);

        if (obj.tag == "Player") 
        {
            PlayerKiller playerKiller = obj.GetComponent<PlayerKiller>();

            playerKiller.killPlayer(returnID());   
        }
    }
}
