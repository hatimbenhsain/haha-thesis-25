using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SpringController : SexSpring
{
    [Header("Game Objects")]
    public Camera playerCamera;
    public CinemachineVirtualCamera virtualCamera;

    [Header("Camera Parameters")]
    public float lerpSpeed = 2f; // speed for the character to align with the camera direction
    public float zoomedInDistance;

    private PlayerInput playerInput;
    private bool isAligningWithCamera = false;

    private float originalCameraDistance;
    private Cinemachine3rdPersonFollow thirdPersonFollow;
    private Vector2 movementVector;

    void Start()
    {
        SpringStart();

        playerInput = FindObjectOfType<PlayerInput>();
        thirdPersonFollow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        if (thirdPersonFollow != null)
        {
            originalCameraDistance = thirdPersonFollow.CameraDistance;
        }
    }

    private void Update()
    {
        if (playerInput.movingForward && !playerInput.prevMovingForward)
        {
            StartInhaling();
        }

        if (!playerInput.movingForward && playerInput.prevMovingForward)
        {
            StartExhaling();
        }
        
        SpringUpdate();

    }
    void FixedUpdate()
    {

        if (!playerInput.aiming)
        {
            AlignWithCamera();
        }
        else
        {
            isAligningWithCamera = false;
            RestoreOriginalCameraDistance();
        }
        if (playerInput.currentControlScheme == "Gamepad")
        {
            HandleTurning(playerInput.look);
        }
        else
        {
            ConvertMovementInput(playerInput.movingForward, playerInput.movingBackward, playerInput.movingLeft, playerInput.movingRight);
            HandleTurning(movementVector);
        }


        SpringFixedUpdate();
    }

    void ConvertMovementInput(bool movingForward, bool movingBackward, bool movingLeft, bool movingRight)
    {
        // Reset movementVector before accumulating input
        movementVector.x = 0f;
        movementVector.y = 0f;

        // Determine movement direction based on input
        if (movingForward) { movementVector.x += 1; }
        if (movingBackward) { movementVector.x -= 1; }
        if (movingLeft) { movementVector.y += 1; }
        if (movingRight) { movementVector.y -= 1; }

        // Normalize the vector only if it has a non-zero length
        if (movementVector.magnitude > 1f)
        {
            movementVector.Normalize();
        }
    }



    // align character with camera direction
    void AlignWithCamera()
    {
        Vector3 cameraForward = playerCamera.transform.forward;
        targetRotation = Quaternion.LookRotation(cameraForward); // set target rotation based on camera direction
        isAligningWithCamera = true; // start aligning
        characterRb.MoveRotation(Quaternion.Lerp(characterRb.rotation, targetRotation, lerpSpeed * Time.fixedDeltaTime));
        characterRb.angularVelocity = Vector3.zero; // clear all angular acceleration

        // Smoothly zoom in the camera
        if (thirdPersonFollow != null)
        {
            thirdPersonFollow.CameraDistance = Mathf.Lerp(thirdPersonFollow.CameraDistance, zoomedInDistance, lerpSpeed * Time.fixedDeltaTime);
        }
    }

    private void RestoreOriginalCameraDistance()
    {
        isAligningWithCamera = false;

        // Smoothly restore the original camera distance
        if (thirdPersonFollow != null)
        {
            thirdPersonFollow.CameraDistance = Mathf.Lerp(thirdPersonFollow.CameraDistance, originalCameraDistance, lerpSpeed * Time.fixedDeltaTime);
        }
    }
}