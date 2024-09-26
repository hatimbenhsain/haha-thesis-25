using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform cameraRoot; 
    public float mouseSensitivity = 100f; // Mouse sensitivity for looking around

    private float xRotation = 0f; // Current x-axis rotation

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Locks the cursor to the center of the screen
    }

    void Update()
    {
        HandleMouseLook();
    }

    void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate around the Y axis based on mouse X movement (horizontal look)
        cameraRoot.Rotate(Vector3.up * mouseX);

        // Adjust rotation for vertical look (mouse Y), clamping to avoid flipping
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f); // Limit vertical rotation

        // Apply rotation to the camera root
        cameraRoot.localRotation = Quaternion.Euler(xRotation, cameraRoot.localEulerAngles.y, 0f);
    }
}
