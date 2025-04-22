using UnityEngine;
using Photon.Pun;


public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public BoxCollider2D roomBounds;
    private Camera cam;

    public float yoffset = 0.6f;
    private PhotonView view;

    void Start()
    {
        Debug.Log("🎥 CameraFollow Init | PhotonView found: " + (view != null) + " | IsMine: " + view?.IsMine);

        cam = GetComponent<Camera>();

        view = GetComponentInParent<PhotonView>(); // ← THIS is more reliable than TryGetComponent on root

        if (view != null && !view.IsMine)
        {
            cam.enabled = false;

            if (TryGetComponent<AudioListener>(out var listener))
                listener.enabled = false;

            enabled = false;
            return;
        }

        if (target == null)
            target = view.transform; // ← use PhotonView object as default
    }

    void LateUpdate()
    {
        if (target != null && roomBounds != null)
        {
            // Follow the target's position
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y+yoffset, transform.position.z);
            transform.position = targetPosition;

            // Restrict the camera within the room's bounds
            float camHalfHeight = cam.orthographicSize;
            float camHalfWidth = cam.aspect * camHalfHeight;
            float minX = roomBounds.bounds.min.x + camHalfWidth;
            float maxX = roomBounds.bounds.max.x - camHalfWidth;
            float minY = roomBounds.bounds.min.y + camHalfHeight;
            float maxY = roomBounds.bounds.max.y - camHalfHeight;

            Vector3 clampedPosition = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), Mathf.Clamp(transform.position.y, minY, maxY), transform.position.z);

            // Center the camera in the room if the room's size is less than the camera's bounds
            if (roomBounds.size.x < camHalfWidth * 2f)
            {
                clampedPosition.x = roomBounds.bounds.center.x;
            }
            if (roomBounds.size.y < camHalfHeight * 2f)
            {
                clampedPosition.y = roomBounds.bounds.center.y;
            }

            transform.position = clampedPosition;
        }
    }
}
