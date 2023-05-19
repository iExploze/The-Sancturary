using UnityEngine;

public class CustodianRoomController : MonoBehaviour
{
    private GameObject player;
    private AudioSource audioSource;
    public BoxCollider2D roomCollider;
    private Collider2D playerCollider;
    private CameraFollow cameraScript;
    private bool playerInThisRoom = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        cameraScript = Camera.main.GetComponent<CameraFollow>();
        playerCollider = player.GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.GetComponent<PlayerMovement>().isInCustodianRoom = true;
            player.GetComponent<PlayerMovement>().custodianRoomLoc = transform.position;
            playerInThisRoom = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.GetComponent<PlayerMovement>().isInCustodianRoom = false;
            playerInThisRoom = false;
        }
    }

    private void Update()
    {
        if (playerInThisRoom && !roomCollider.bounds.Intersects(playerCollider.bounds))
        {
            cameraScript.roomBounds = roomCollider;
        }
    }
}
