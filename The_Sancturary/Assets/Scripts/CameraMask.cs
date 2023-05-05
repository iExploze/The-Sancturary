using UnityEngine;

public class CameraMask : MonoBehaviour
{
    public Camera mainCamera;
    public BoxCollider2D roomBounds;

    private Vector2 screenSize;

    void Start()
    {
        screenSize = new Vector2(mainCamera.pixelWidth, mainCamera.pixelHeight);
    }

    void Update()
{
    if (roomBounds != null)
    {
        float minOrthographicSize = 3f; // Set this to the minimum size you want for the camera

        float desiredOrthographicSizeX = roomBounds.size.x / (2f * mainCamera.aspect);
        float desiredOrthographicSizeY = roomBounds.size.y / 2f;

        float desiredOrthographicSize = Mathf.Max(desiredOrthographicSizeX, desiredOrthographicSizeY, minOrthographicSize);

        mainCamera.orthographicSize = desiredOrthographicSize;
        mainCamera.transform.position = new Vector3(roomBounds.bounds.center.x, roomBounds.bounds.center.y, mainCamera.transform.position.z);
    }
}

}
