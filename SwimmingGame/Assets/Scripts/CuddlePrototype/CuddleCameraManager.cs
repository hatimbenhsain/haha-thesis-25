using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuddleCameraManager : MonoBehaviour
{
    public Camera[] cameras; 
    public GameObject[] handPositions;
    public Transform hand;

    [Header("Head Bob Settings")]
    public float bobSpeed = 2f;      
    public float bobAmount = 0.05f;  
    private float defaultCameraY;    
    private float bobTimer;

    public int currentCameraIndex;

    void Start()
    {
        SetActiveCamera(currentCameraIndex);
        defaultCameraY = cameras[currentCameraIndex].transform.localPosition.y; // Store initial Y position
        UpdateHandPosition();
    }

    void FixedUpdate()
    {
        SetActiveCamera(currentCameraIndex);
        defaultCameraY = cameras[currentCameraIndex].transform.localPosition.y; // Reset Y position for new active camera
        UpdateHandPosition(); 
        ApplyHeadBob(); 
    }

    void SetActiveCamera(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(i == index); // Enable the active camera, disable others
        }
        // Reset bob timer when switching cameras
        bobTimer = 0f;
    }

    void ApplyHeadBob()
    {
        if (cameras[currentCameraIndex].gameObject.activeSelf)
        {
            bobTimer += Time.fixedDeltaTime * bobSpeed;
            float newY = defaultCameraY + Mathf.Sin(bobTimer) * bobAmount; // Calculate new Y position based on sine wave
            Vector3 newPosition = cameras[currentCameraIndex].transform.localPosition;
            newPosition.y = newY;
            cameras[currentCameraIndex].transform.localPosition = newPosition;
        }
    }

    void UpdateHandPosition()
    {
        if (hand != null && handPositions[currentCameraIndex] != null)
        {
            hand.position = handPositions[currentCameraIndex].transform.position;
            hand.rotation = handPositions[currentCameraIndex].transform.rotation;
        }
    }
}
