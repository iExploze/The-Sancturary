using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public BoxCollider2D roomCollider;
    private CameraFollow cameraScript;
    private CameraMask cameraMaskScript;

    void Start()
    {
        cameraScript = Camera.main.GetComponent<CameraFollow>();
        cameraMaskScript = Camera.main.GetComponent<CameraMask>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cameraScript.roomBounds = roomCollider;
            cameraMaskScript.roomBounds = roomCollider;
        }
    }
}
