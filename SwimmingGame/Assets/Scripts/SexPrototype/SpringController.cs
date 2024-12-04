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

        HandleTurning(playerInput.look);

        SpringFixedUpdate();
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