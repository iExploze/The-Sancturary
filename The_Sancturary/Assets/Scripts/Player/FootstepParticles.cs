using UnityEngine;

public class FootstepParticles : MonoBehaviour
{
    public ParticleSystem footstepParticles; // Assign your particle system in the inspector

    private PlayerMovement playerMovement; // Reference to the player movement script

    private void Awake()
    {
        // Get reference to the PlayerMovement script
        playerMovement = GetComponent<PlayerMovement>();

        // If the ParticleSystem is attached to a child object, get it
        footstepParticles = GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
        // If the player is moving, emit particles
        if (playerMovement.isMoving)
        {
            Debug.Log("playing");
            if (!footstepParticles.isEmitting)
            {
                footstepParticles.Play();
            }
        }
        else
        {
            if (footstepParticles.isEmitting)
            {
                footstepParticles.Stop();
            }
        }
    }
}
