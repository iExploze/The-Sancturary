using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using NavMeshPlus.Components;
using System;

public class ArjunLockerController : MonoBehaviour
{   
    public float navMeshUpdateRadius = 5f;
    public NavMeshSurface navMeshSurface;
    public NavMeshModifier navMeshModifier;
    public Canvas jumpscareCanvas;
    public VideoPlayer jumpscareVideo;
    public Animator lockerAnimator;
    public string toggleLockerParameter = "ToggleLocker";
    public string survivalPlayerPrefKey = "Survived";
    private GameObject player;
    private AudioSource audioSource;
    private bool playerIsNearby = false;
    private SpriteRenderer playerSpriteRenderer;
    private Rigidbody2D playerRigidbody2D;
    private Camera playerCamera;
    public float activationDistance = 5f;
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider;
    public CapsuleCollider2D capsuleCollider;
    private bool playerCame;
    public string deathSceneName;
    private bool openingAnimationPlaying = false;
    private float openingAnimationTimer = 0f;
    public float jumpscareDelay = 2f;
    void Start()
    {
        spriteRenderer.enabled = false;
        boxCollider.enabled = false;
        capsuleCollider.enabled = false;
        
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        jumpscareCanvas.GetComponent<CanvasGroup>().alpha = 0;
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        playerRigidbody2D = player.GetComponent<Rigidbody2D>();
        playerCamera = player.GetComponentInChildren<Camera>();
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
        float distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
        if (distanceToPlayer < activationDistance)
        {
            playerCame = true;
        }
        if (distanceToPlayer > activationDistance && playerCame)
        {
            spriteRenderer.enabled = true;
            boxCollider.enabled = true;
            capsuleCollider.enabled = true;
            //RebakeNavMesh();
        }

        if (Input.GetMouseButtonDown(0) && !player.GetComponent<PlayerMovement>().isHiding && playerIsNearby)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject && !openingAnimationPlaying)
            {
                StartCoroutine(EnterLocker());
            }
        }

        if (openingAnimationPlaying)
        {
            openingAnimationTimer += Time.deltaTime;
            if (openingAnimationTimer >= jumpscareDelay && playerIsNearby)
            {
                openingAnimationPlaying = false;
                openingAnimationTimer = 0f;
                Jumpscare();
            }
        }
    }

    private void RebakeNavMesh()
    {
        navMeshModifier.ignoreFromBuild = false;
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
    }

    IEnumerator EnterLocker()
    {
        lockerAnimator.SetTrigger(toggleLockerParameter);
        audioSource.Play();
        openingAnimationPlaying = true;

        yield break;
    }

    void Jumpscare()
    {

        // Trigger the jumpscare video
        CanvasGroup canvasGroup = jumpscareCanvas.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        jumpscareVideo.Play();
        StartCoroutine(WaitForJumpscare());
    }

    IEnumerator WaitForJumpscare()
    {
        // Wait for the length of the jumpscare video
        yield return new WaitForSeconds((float)jumpscareVideo.length-0.3f);

        // Set the PlayerPrefs value for the survival status to 0 (not survived)
        PlayerPrefs.SetInt(survivalPlayerPrefKey, 0);
        PlayerPrefs.Save();

        // Load the death scene
        SceneManager.LoadScene(deathSceneName);
    }
}
