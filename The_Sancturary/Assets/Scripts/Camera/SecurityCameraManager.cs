using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraManager : MonoBehaviour
{
    public static SecurityCameraManager Instance;

    public List<Camera> securityCameras = new List<Camera>();
    public RenderTexture cameraFeedTexture;
    private int currentCameraIndex = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SwitchToCamera(0); // Start at first camera
    }

    public void SwitchToCamera(int index)
    {
        if (index < 0 || index >= securityCameras.Count)
            return;

        // Disable all cameras
        foreach (var cam in securityCameras)
        {
            cam.enabled = false;
            cam.targetTexture = null;
        }

        // Enable selected camera
        securityCameras[index].enabled = true;
        securityCameras[index].targetTexture = cameraFeedTexture;

        currentCameraIndex = index;
    }
}
