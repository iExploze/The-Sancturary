using UnityEngine;

public class LockerInteraction : MonoBehaviour
{
    public Animator lockerAnimator;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                ToggleLocker();
            }
        }
    }

    private void ToggleLocker()
    {
        lockerAnimator.SetTrigger("ToggleLocker");
    }
}
