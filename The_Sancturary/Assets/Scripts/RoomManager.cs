using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public CameraFollow cameraScript;
    public BoxCollider2D[] roomColliders;
    public int startingRoomIndex = 0;

    void Start()
    {
        // Set the camera bounds to the starting room's bounds
        cameraScript.roomBounds = roomColliders[startingRoomIndex];
    }

    // Implement methods to change rooms, detect when the player enters a new room, etc.

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < roomColliders.Length; i++)
            {
                if (other.bounds.Intersects(roomColliders[i].bounds))
                {
                    cameraScript.roomBounds = roomColliders[i];
                    break;
                }
            }
        }
    }

}
