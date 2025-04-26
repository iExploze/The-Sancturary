using UnityEngine;
using Photon.Pun;

public class FlashlightController : MonoBehaviourPun
{
    [SerializeField] private Transform flashlight; // ← manually assign

    void Update()
    {
        if (photonView.IsMine)
        {
            RotateFlashlightToMouse();
        }
    }

    void RotateFlashlightToMouse()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouseWorld - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        flashlight.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }
}
