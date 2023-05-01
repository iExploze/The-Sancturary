using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightSwitchController : MonoBehaviour
{

    public Light2D lightToControl;
    public float newIntensity = 1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            lightToControl.intensity = newIntensity;
        }
    }
}
