using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private GameObject doorTop;
    [SerializeField] private GameObject doorBottom;
    [SerializeField] private float moveSpeed = 2f;

    private bool opened = true;
    private Vector3 topClosedPosition;
    private Vector3 bottomClosedPosition;
    private Vector3 topOpenPosition;
    private Vector3 bottomOpenPosition;
    private bool isMoving = false;
    private float moveProgress = 0f;

    void Start()
    {
        // Store the original open positions
        topOpenPosition = doorTop.transform.localPosition;
        bottomOpenPosition = doorBottom.transform.localPosition;

        // Calculate the closed positions
        topClosedPosition = topOpenPosition - new Vector3(0, 2.5f, 0);
        bottomClosedPosition = bottomOpenPosition + new Vector3(0, 2.5f, 0);
    }

    void Update()
    {   
        if (isMoving)
        {
            moveProgress += moveSpeed * Time.deltaTime;

            if (opened)
            {
                doorTop.transform.localPosition = Vector3.Lerp(topClosedPosition, topOpenPosition, moveProgress);
                doorBottom.transform.localPosition = Vector3.Lerp(bottomClosedPosition, bottomOpenPosition, moveProgress);
            }
            else
            {
                doorTop.transform.localPosition = Vector3.Lerp(topOpenPosition, topClosedPosition, moveProgress);
                doorBottom.transform.localPosition = Vector3.Lerp(bottomOpenPosition, bottomClosedPosition, moveProgress);
            }

            if (moveProgress >= 1f)
            {
                moveProgress = 0f;
                isMoving = false;
            }
        }
    }

    public void ToggleDoor()
    {
        if (!isMoving)
        {
            opened = !opened;
            isMoving = true;
            moveProgress = 0f;
        }
    }

    public bool IsOpen()
    {
        return opened;
    }
}
