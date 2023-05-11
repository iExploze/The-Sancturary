using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class YousseffController : MonoBehaviour
{
    public GameObject player;
    public Canvas jumpscareCanvas;
    public VideoPlayer jumpscareVideo;
    public GameObject monsterSprite;
    public float jumpscareDistanceThreshold = 1f;
    public float wanderAreaRadius = 10f;
    public string deathSceneName;
    public string survivalPlayerPrefKey = "Survived";

    public float normalSpeed = 0.5f;
    public float dashSpeed = 10f;
    public float dashCooldown = 5f;
    public float chargeUpTime = 1f;
    public float dashDuration = 0.3f;
    public float dechargeTime = 1f;

    private NavMeshAgent agent;
    private Animator animator;
    private float cooldownCount = 0f;

    private Transform monsterSpriteTransform;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        monsterSpriteTransform = monsterSprite.transform;

        agent.acceleration = 20f;
        agent.angularSpeed = 600f;
        agent.autoBraking = false;
        agent.speed = normalSpeed;

        jumpscareCanvas.GetComponent<CanvasGroup>().alpha = 0;
    }

    void Update()
    {
        if (player.GetComponent<PlayerMovement>().isHiding || player.GetComponent<PlayerMovement>().isInCustodianRoom)
        {
            Wanderer();
        }
        else
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (distanceToPlayer > jumpscareDistanceThreshold)
            {
                Move();
            }
            else
            {
                agent.isStopped = true;
                animator.SetFloat("Speed", 0f);
                CanvasGroup canvasGroup = jumpscareCanvas.GetComponent<CanvasGroup>();
                canvasGroup.alpha = 1;
                jumpscareVideo.Play();
                StartCoroutine(WaitForJumpscare());
            }
        }

        // Constrain rotation on the Y and Z axes for the parent GameObject
        transform.rotation = Quaternion.Euler(0, 0, 0);

        // Constrain angular movement for the MonsterSprite GameObject
        monsterSpriteTransform.localRotation = Quaternion.identity;
    }

    IEnumerator DashSequence()
    {
        // Charge up
        agent.speed = 0;
        animator.SetBool("isCharging", true);
        yield return new WaitForSeconds(chargeUpTime);

        // Pre-dash animation
        animator.SetTrigger("preDash");
        yield return new WaitForSeconds(0.1f); // Adjust the duration according to your animation length

        // Dash
        agent.speed = dashSpeed;
        animator.SetBool("isDashing", true);
        yield return new WaitForSeconds(dashDuration);

        // Post-dash animation
        agent.speed = 0;
        animator.SetTrigger("postDash");
        yield return new WaitForSeconds(0.1f); // Adjust the duration according to your animation length

        // Decharge
        agent.speed = 0;
        animator.SetBool("isDashing", false);
        animator.SetBool("isCharging", false);
        yield return new WaitForSeconds(dechargeTime);

        // Reset cooldown
        agent.speed = normalSpeed;
    }

    void Move()
    {
        if (cooldownCount >= dashCooldown)
                {
                    StartCoroutine(DashSequence());
                    cooldownCount = 0;
                }
                else
                {
                    cooldownCount += Time.deltaTime;
                    agent.SetDestination(player.transform.position);

                    Vector2 velocity = agent.velocity;
                    animator.SetFloat("Horizontal", velocity.x);
                    animator.SetFloat("Vertical", velocity.y);
                    animator.SetFloat("Speed", velocity.sqrMagnitude);
                }
    }    


    void Wanderer()
    {
        Vector2 randomPoint = Random.insideUnitCircle * wanderAreaRadius;

            // Set the destination of the agent to the random point
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 2.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        if (cooldownCount >= dashCooldown)
        {
            StartCoroutine(DashSequence());
            cooldownCount = 0;
        }
        else
        {
            cooldownCount += Time.deltaTime;
            // Get a random point within the specified area

            // Set the animator "Speed" parameter to a non-zero value to play the walking animation
            Vector2 velocity = agent.velocity;
            animator.SetFloat("Horizontal", velocity.x);
            animator.SetFloat("Vertical", velocity.y);
            animator.SetFloat("Speed", velocity.sqrMagnitude);
        }
    }

    IEnumerator WaitForJumpscare()
    {
        // Wait for the length of the jumpscare video
        yield return new WaitForSeconds((float)jumpscareVideo.length - 0.3f);

        // Set the PlayerPrefs value for the survival status to 0 (not survived)
        PlayerPrefs.SetInt(survivalPlayerPrefKey, 0);
        PlayerPrefs.Save();

        // Load the death scene
        SceneManager.LoadScene(deathSceneName);
    }
}
