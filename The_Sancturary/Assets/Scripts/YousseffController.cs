using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class YousseffController : MonoBehaviour
{
    private GameObject player;
    public Canvas jumpscareCanvas;
    public VideoPlayer jumpscareVideo;
    public GameObject monsterSprite;
    public float jumpscareDistanceThreshold = 0.7f;
    public float wanderAreaRadius = 10f;
    public string deathSceneName;
    public float chargeUpTime = 2.5f;
    public float dashCooldown = 10f;
    public float dashDuration = 0.3f;
    public float dechargeTime = 2.5f;
    public float NormalSpeed = 0.8f;
    public float DashSpeed = 10f;
    public string survivalPlayerPrefKey = "Survived";
    private UnityEngine.AI.NavMeshAgent agent;
    private Animator animator;
    private float cooldownCount = 7f;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = monsterSprite.GetComponent<Animator>();
        jumpscareCanvas.GetComponent<CanvasGroup>().alpha = 0;
        player = GameObject.FindGameObjectWithTag("Player");

        // Adjust the NavMeshAgent2D parameters for smoother movement
        agent.acceleration = 20f;
        agent.angularSpeed = 600f;
        agent.autoBraking = false;
    }

    void Update()
    {
        // Check if the player is hiding
        if (player.GetComponent<PlayerMovement>().isHiding)
        {
            //Wanderer();
        }
        else
        {
            // Check the distance to the player
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (distanceToPlayer > jumpscareDistanceThreshold)
            {
                //start the dash
                cooldownCount += Time.deltaTime;
                if (cooldownCount >= dashCooldown)
                {
                    StartCoroutine(ChargeUpDash());
                }
                else
                {
                    agent.speed = NormalSpeed;
                    agent.SetDestination(player.transform.position);
                    Vector2 velocity = agent.velocity;
                    animator.SetFloat("Horizontal", velocity.x);
                    animator.SetFloat("Vertical", velocity.y);
                    animator.SetFloat("Speed", velocity.sqrMagnitude);
                }
            }
            else
            {
                // Stop the monster movement
                    agent.isStopped = true;

                    // Trigger the jumpscare video
                    CanvasGroup canvasGroup = jumpscareCanvas.GetComponent<CanvasGroup>();
                    canvasGroup.alpha = 1;
                    jumpscareVideo.Play();

                    // Set the "Speed" parameter to zero to stop the walking animation
                    animator.SetFloat("Speed", 0f);
                    // Wait for the jumpscare video to finish, then transition to the death scene
                    StartCoroutine(WaitForJumpscare());
            }
        }

        // Constrain rotation on the Y and Z axes for the parent GameObject
        transform.rotation = Quaternion.Euler(0, 0, 0);

        // Constrain angular movement for the MonsterSprite GameObject
        monsterSprite.transform.localRotation = Quaternion.identity;
    }



    void Wanderer()
    {
        // Get a random point within the specified area
        Vector2 randomPoint = Random.insideUnitCircle * wanderAreaRadius;

        // Set the destination of the agent to the random point
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 2.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        // Set the animator "Speed" parameter to a non-zero value to play the walking animation
        Vector2 velocity = agent.velocity;
        animator.SetFloat("Horizontal", velocity.x);
        animator.SetFloat("Vertical", velocity.y);
        animator.SetFloat("Speed", velocity.sqrMagnitude);
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

    IEnumerator ChargeUpDash()
    {
        agent.speed = 0;
        animator.SetBool("isCharging", true);

        // Set the "Speed" parameter to 0 to stop the walking animation
        animator.SetFloat("Speed", 0f);

        yield return new WaitForSeconds(chargeUpTime);
        StartCoroutine(PerformDash());
    }


    IEnumerator PerformDash()
    {
        animator.SetBool("isCharging", true);
        agent.speed = DashSpeed;
        Debug.Log(agent.speed);
        agent.SetDestination(player.transform.position);

        Vector2 velocity = agent.velocity;
        animator.SetFloat("Horizontal", velocity.x);
        animator.SetFloat("Vertical", velocity.y);
        animator.SetFloat("Speed", velocity.sqrMagnitude);

        // Set the animation speed based on the ratio of dashDuration to the actual animation length
        StartCoroutine(DeChargeDash());
        yield return new WaitForSeconds(dashDuration);
        animator.SetBool("isCharging", false);
    }

    IEnumerator DeChargeDash()
    {
        agent.speed = 0;
        agent.velocity = Vector3.zero; // Stop the agent's movement
        animator.SetBool("isCharging", false);

        // Set the "Speed" parameter to 0 to stop the walking animation
        animator.SetFloat("Speed", 0f);

        yield return new WaitForSeconds(dechargeTime);

        // Reset the agent's speed to NormalSpeed
        agent.speed = NormalSpeed;
        cooldownCount = 0;
    }

}
