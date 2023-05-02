using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public BoxCollider2D roomBounds;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target != null && roomBounds != null)
        {
            // Follow the target's position
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
            transform.position = targetPosition;

            // Restrict the camera within the room's bounds
            float camHalfHeight = cam.orthographicSize;
            float camHalfWidth = cam.aspect * camHalfHeight;
            float minX = roomBounds.bounds.min.x + camHalfWidth;
            float maxX = roomBounds.bounds.max.x - camHalfWidth;
            float minY = roomBounds.bounds.min.y + camHalfHeight;
            float maxY = roomBounds.bounds.max.y - camHalfHeight;

            Vector3 clampedPosition = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), Mathf.Clamp(transform.position.y, minY, maxY), transform.position.z);
            transform.position = clampedPosition;
        }
    }
}
