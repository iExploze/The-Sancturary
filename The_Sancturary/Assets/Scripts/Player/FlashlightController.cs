using UnityEngine;
using Photon.Pun;
using UnityEngine.Rendering.Universal;  // make sure your URP package matches this namespace

public class FlashlightController : MonoBehaviourPun
{
    [SerializeField] private LayerMask obstacleMask;

    private Light2D light2D;
    private Transform flashlight;
    private float maxDistance;
    private float minDistance; 

    [SerializeField] private float addedDistance = 1;

    void Awake()
    {
        // grab your 2D light in the children
        light2D = GetComponentInChildren<Light2D>();
        if (light2D == null)
        {
            return;
        }

        flashlight = light2D.transform;
        maxDistance = light2D.pointLightOuterRadius;
    }

    void Update()
    {
        if (!photonView.IsMine || flashlight == null) return;

        RotateToMouse();
        ClampBeam();
    }

    void RotateToMouse()
    {
        Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = m - transform.position;
        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        flashlight.rotation = Quaternion.Euler(0, 0, ang - 90f);
    }

    void ClampBeam()
    {
        Vector2 origin = flashlight.position;
        Vector2 forward = flashlight.up;

        // debug‐draw the full‐length ray in red
        Debug.DrawRay(origin, forward * maxDistance, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(origin, forward, maxDistance, obstacleMask);
        if (hit.collider != null)
        {
            Debug.DrawRay(origin, forward * hit.distance, Color.green);  // show the hit portion
            light2D.pointLightOuterRadius = hit.distance + addedDistance;
            Debug.Log($"Hit '{hit.collider.name}' at {hit.distance}");
        }
        else
        {
            light2D.pointLightOuterRadius = maxDistance;
        }
    }
}
