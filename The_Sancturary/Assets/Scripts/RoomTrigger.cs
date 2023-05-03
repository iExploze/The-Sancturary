using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public BoxCollider2D roomCollider;
    private CameraFollow cameraScript;

    void Start()
    {
        cameraScript = Camera.main.GetComponent<CameraFollow>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cameraScript.roomBounds = roomCollider;
        }
    }
}
