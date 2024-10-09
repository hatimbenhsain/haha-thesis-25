using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform cameraRoot;
    public float sensitivity = 100f;  // sensitivity for looking around
    public float rotationSmoothTime = 0.1f;
    public bool cameraLocked;

    private float xRotation = 0f;  // Current x-axis rotation
    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private Vector3 rotationVelocity;
    private PlayerInput playerInput;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // Locks the cursor to the center of the screen
        playerInput = FindObjectOfType<PlayerInput>(); 
    }

    void FixedUpdate()
    {
        if (!cameraLocked)
        {
            HandleMouseLook();
        }

    }

    void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = playerInput.rotation.x * sensitivity * Time.deltaTime;
        float mouseY = playerInput.rotation.y * sensitivity * Time.deltaTime;

        // Calculate target rotations based on mouse input
        targetRotation.y += mouseX;  // Horizontal rotation
        xRotation -= mouseY;  // Vertical rotation
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);  // Clamp vertical rotation to prevent flipping

        // Set the target rotation for the cameraRoot
        targetRotation.x = xRotation;

        // Lerp the camera's rotation for a, heavier feel
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationVelocity, rotationSmoothTime);

        // Apply the smoothed rotation to the cameraRoot
        cameraRoot.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);
    }
}
