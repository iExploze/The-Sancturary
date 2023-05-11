using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public BoxCollider2D roomCollider;
    private Collider2D playerCollider;
    private CameraFollow cameraScript;
    private bool playerInThisRoom = false;

    void Start()
    {
        cameraScript = Camera.main.GetComponent<CameraFollow>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInThisRoom = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInThisRoom = false;
        }
    }

    void Update()
    {
        if (playerInThisRoom && !roomCollider.bounds.Intersects(playerCollider.bounds))
        {
            cameraScript.roomBounds = roomCollider;
        }
    }
}
