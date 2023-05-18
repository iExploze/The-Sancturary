using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleDoorController : MonoBehaviour
{
    public Animator doorAnimator;
    public string toggleDoorParameter = "ToggleDoor";
    private GameObject player;
    private bool playerIsNearby = false;
    private AudioSource audioSource;
    private BoxCollider2D doorCollider;

    void Start()
    {
        doorCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = false;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && playerIsNearby)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(mousePos.x, mousePos.y), Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                ToggleDoor();
            }
        }
    }

    private void ToggleDoor()
    {
        doorAnimator.SetTrigger(toggleDoorParameter);
//        audioSource.Play();
        if(doorCollider.enabled)
        {
            player.GetComponent<PlayerMovement>().isInCustodianRoom = true;
            doorCollider.enabled = false;
        }
        else if(!doorCollider.enabled)
        {
            player.GetComponent<PlayerMovement>().isInCustodianRoom = false;
            doorCollider.enabled = true;
        }
    }
}
