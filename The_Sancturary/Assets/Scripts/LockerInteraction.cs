using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockerInteraction : MonoBehaviour
{
    public float interactionDistance = 1f;
    public GameObject insideLockerView;
    public Animator lockerAnimator;
    public string toggleLockerParameter = "ToggleLocker";
    public GameObject player;

    public GameObject LockerCamera;

    private bool playerIsHiding = false;
    private Vector3 playerPositionWhenHiding;

    private AudioSource audioSource;

    public static Vector2 currentLocation;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !playerIsHiding)
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Vector2.Distance(transform.position, player.transform.position) <= interactionDistance &&
                Vector2.Distance(transform.position, mouseWorldPos) <= interactionDistance)
            {
                lockerAnimator.SetTrigger(toggleLockerParameter);
                StartCoroutine(EnterLocker());
            }
        }
        else if (Input.GetMouseButtonDown(0) && playerIsHiding)
        {
            StartCoroutine(ExitLocker());
        }
    }

    IEnumerator EnterLocker()
    {
        currentLocation = transform.localPosition;
        Debug.Log(currentLocation);
        audioSource.Play();
        yield return new WaitForSeconds(0.28f); // Adjust this value based on the duration of the opening animation
        playerPositionWhenHiding = player.transform.position;
        player.SetActive(false);
        LockerCamera.SetActive(true);
        lockerAnimator.SetTrigger(toggleLockerParameter);
        insideLockerView.SetActive(true);
        playerIsHiding = true;
    }

    IEnumerator ExitLocker()
    {
        Debug.Log(currentLocation);
        insideLockerView.SetActive(false);
        lockerAnimator.SetTrigger(toggleLockerParameter);
        audioSource.Play();
        yield return new WaitForSeconds(0.28f); // Adjust this value based on the duration of the opening animation
        LockerCamera.SetActive(false);
        player.SetActive(true);
        player.transform.position = playerPositionWhenHiding;
        lockerAnimator.SetTrigger(toggleLockerParameter);
        playerIsHiding = false;
    }
}