using UnityEngine;

public class StairController : MonoBehaviour
{
   public Transform teleportPoint;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TeleportPlayer(other.gameObject);
        }
    }

    private void TeleportPlayer(GameObject player)
    {
        // Teleport the player to the target stair's teleport point
        player.transform.position = teleportPoint.transform.position;
    }
}
