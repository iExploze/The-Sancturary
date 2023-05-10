using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustodianRoomController : MonoBehaviour
{
    private GameObject player;
    private bool playerIsNearby = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.GetComponent<PlayerMovement>().isInCustodianRoom = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.GetComponent<PlayerMovement>().isInCustodianRoom = false;
        }
    }
}
