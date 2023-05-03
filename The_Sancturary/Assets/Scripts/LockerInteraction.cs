using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockerInteraction : MonoBehaviour
{
    public float interactionDistance = 1f;
    public GameObject insideLockerView;
    public Animator lockerAnimator;
    public string toggleLockerParameter = "ToggleLocker";

    public GameObject LockerCamera;
    public Canvas lockerCanvas;

    private GameObject player;
    private Vector3 playerPositionWhenHiding;

    private AudioSource audioSource;

    public static Vector2 currentLocation;

    private bool playerIsNearby = false;

    private SpriteRenderer playerSpriteRenderer;
    private Rigidbody2D playerRigidbody2D;
    private Camera playerCamera;

    private bool playerInThisLocker = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        playerRigidbody2D = player.GetComponent<Rigidbody2D>();
        playerCamera = player.GetComponentInChildren<Camera>();
        lockerCanvas.enabled = true;
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
        if (Input.GetMouseButtonDown(0) && !player.GetComponent<PlayerMovement>().isHiding && playerIsNearby)
        {
            // Cast a ray from the mouse position to check if it hits the locker
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                playerInThisLocker = true;
                StartCoroutine(EnterLocker());
            }
        }
        else if (Input.GetMouseButtonDown(0) && player.GetComponent<PlayerMovement>().isHiding && playerInThisLocker)
        {
            playerInThisLocker = false;
            StartCoroutine(ExitLocker());
        }
    }

    IEnumerator EnterLocker()
    {
        lockerAnimator.SetTrigger(toggleLockerParameter);
        //currentLocation = transform.localPosition;
        audioSource.Play();
        yield return new WaitForSeconds(0.28f);
        playerPositionWhenHiding = player.transform.position;

        // Disable Sprite Renderer, Rigidbody2D, and Camera
        playerSpriteRenderer.enabled = false;
        playerRigidbody2D.simulated = false;
        playerCamera.enabled = false;
        player.GetComponent<PlayerMovement>().isHiding = true;
        player.transform.position = transform.position;

        LockerCamera.SetActive(true);
        lockerAnimator.SetTrigger(toggleLockerParameter);
        insideLockerView.SetActive(true);
    }

    IEnumerator ExitLocker()
    {
        insideLockerView.SetActive(false);
        lockerAnimator.SetTrigger(toggleLockerParameter);
        audioSource.Play();
        yield return new WaitForSeconds(0.28f);

        // Enable Sprite Renderer, Rigidbody2D, and Camera
        playerSpriteRenderer.enabled = true;
        playerRigidbody2D.simulated = true;
        player.GetComponent<PlayerMovement>().isHiding = false;
        playerCamera.enabled = true;

        LockerCamera.SetActive(false);
        player.transform.position = playerPositionWhenHiding;
        lockerAnimator.SetTrigger(toggleLockerParameter);
    }
}
