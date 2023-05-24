using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightSwitchController : MonoBehaviour
{
    public static int barrelCount = 0; // Count of collected barrels
    public GameObject exit; // Reference to the exit object

    public Light2D areaLight;
    public Light2D playerLight;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            barrelCount++;

            if (barrelCount == 1)
            {
                // Turn on the area light and turn off the player light when the first barrel/light switch is collected
                if (areaLight != null) 
                {
                    areaLight.intensity = 1;
                }
                if (playerLight != null)
                {
                    playerLight.intensity = 0;
                }
            }
            else if (barrelCount == 2)
            {
                // Activate the exit when the second barrel/light switch is collected
                if (exit != null)
                {
                    exit.SetActive(true); 
                }
            }
        }
    }
}
