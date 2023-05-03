using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class EthanController : MonoBehaviour
{
    public float timeToTrigger = 5f;
    public AudioSource ethanAudioSource;
    public AudioClip ethanSound;
    private Animator animator;
    public Canvas jumpscareCanvas;
    public VideoPlayer jumpscareVideo;
    public string deathSceneName;
    public string survivalPlayerPrefKey = "Survived";
    
    private bool playerInTrigger = false;
    private float timePlayerInTrigger = 0f;
    private int audioRepeatCount = 0;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        animator = transform.GetComponent<Animator>();
        jumpscareCanvas.GetComponent<CanvasGroup>().alpha = 0;
        spriteRenderer.enabled = false;
    }

    void Update()
    {
        if (playerInTrigger)
        {
            timePlayerInTrigger += Time.deltaTime;
            if (timePlayerInTrigger >= timeToTrigger)
            {
                animator.SetBool("TooLong", true);
                spriteRenderer.enabled = true;
                StartCoroutine(PlayJumpscare());
            }
        }
    }

    IEnumerator PlayEthanSound()
    {
        while (audioRepeatCount < Mathf.Floor(timeToTrigger / ethanSound.length))
        {
            ethanAudioSource.volume = Mathf.Lerp(0.25f, 1f, audioRepeatCount / Mathf.Floor(timeToTrigger / ethanSound.length));
            ethanAudioSource.PlayOneShot(ethanSound);
            audioRepeatCount++;
            yield return new WaitForSeconds(ethanSound.length);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = true;
            StartCoroutine(PlayEthanSound());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = false;
            timePlayerInTrigger = 0f;
            audioRepeatCount = 0;
            StopCoroutine(PlayEthanSound());
        }
    }

    IEnumerator PlayJumpscare()
    {
        yield return new WaitForSeconds(1f);
        CanvasGroup canvasGroup = jumpscareCanvas.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        jumpscareVideo.Play();

        animator.SetFloat("Speed", 0f);
        yield return new WaitForSeconds((float)jumpscareVideo.length - 0.3f);

        PlayerPrefs.SetInt(survivalPlayerPrefKey, 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene(deathSceneName);
    }
}
