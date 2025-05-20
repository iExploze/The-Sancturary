using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SecurityCameraManager : MonoBehaviour
{
    public static SecurityCameraManager Instance;

    public List<Camera> securityCameras = new List<Camera>();
    public RenderTexture cameraFeedTexture;
    private int currentCameraIndex = 0;

    public Button closeDoorButton;    // assign your “Close Door” UI Button here
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // hook up the button click
        if (closeDoorButton != null)
            closeDoorButton.onClick.AddListener(OnCloseDoorClicked);
    }

    private void OnCloseDoorClicked()
    {
        // grab the active camera
        Camera cam = securityCameras[currentCameraIndex];
        if (cam == null) return;

        // find the door script in its children
        var door = cam.transform.GetComponentInChildren<DoorController>();
        if (door != null)
        {
            door.ToggleDoor();    // or whatever your door’s “close” method is called
        }
        else
        {
            Debug.LogWarning($"[SecurityCameraManager] No DoorController found under camera '{cam.name}'");
        }
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
