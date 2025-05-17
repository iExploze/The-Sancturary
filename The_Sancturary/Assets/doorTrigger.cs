using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    private DoorController doorController;
    private float doorHitCooldown = 1.0f;  // 1 second cooldown
    private float lastHitTime;
    void Start()
    {
        this.doorController = GetComponent<DoorController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            MonsterBase monster = other.GetComponent<MonsterBase>();
            if (!doorController.IsOpen() && Time.time >= lastHitTime + doorHitCooldown)
            {
                Debug.Log("Monster hit the door!");
                monster.OnDoorTouch();
                lastHitTime = Time.time;  // Update the last hit time
            }
        }
    }
}
