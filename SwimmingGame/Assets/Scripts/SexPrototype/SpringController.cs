using Cinemachine;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class SpringController : SexSpring
{
    [Header("Game Objects")]
    public Camera playerCamera;
    public CinemachineVirtualCamera virtualCamera;

    [Header("Camera Parameters")]
    public float defaultCameraLerpSpeed; // speed for the character to align with the camera direction
    public float quickCameraLerpSpeed;
    public float defaultCameraDistance;
    public float ZoomOutCameraDistance;
    public float ZoomInCameraDistance;
    public float shakeAmplitude = 0.2f; 
    public bool lockCamera;


    private PlayerInput playerInput;
    private bool isAligningWithCamera = false;
    private bool isShaking;

    private Cinemachine3rdPersonFollow thirdPersonFollow;
    private Vector2 movementVector;
    private Vector3 initialCameraPosition;
    private int cameraDistance; // 0 zoom in, 1 default, 2 zoom out
    private float targetCameraDistance;
    private float cameraLerpSpeed;

    private EventInstance chargingInstance;


    void Start()
    {
        SpringStart();

        playerInput = FindObjectOfType<PlayerInput>();
        thirdPersonFollow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

        chargingInstance=RuntimeManager.CreateInstance("event:/Sex/Charge");
    }

    private void Update()
    {
        if (playerInput.movingForward && !playerInput.prevMovingForward)
        {
            StartInhaling();
            // enable screenshake and quick camera lerp speed
            isShaking = true;
            cameraLerpSpeed = quickCameraLerpSpeed;
            chargingInstance.start();
        }

        if (!playerInput.movingForward && playerInput.prevMovingForward)
        {
            StartExhaling();
            // disable screenshake and return to default camera lerp speed
            isShaking = false;
            cameraLerpSpeed = defaultCameraLerpSpeed;

            chargingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            Sound.PlayOneShotVolume("event:/Sex/Thrust",1f,"force",1f);
        }
        
        SpringUpdate();

        if(isInhaling){
            Rumble.AddRumble("Inhaling",Mathf.Clamp((Time.time - inhaleStartTime) / maxInhaleTime,0f,1f));
        }

    }
    void FixedUpdate()
    {

        if (!playerInput.aiming&&!lockCamera)
        {
            AlignWithCamera();
        }
        else
        {
            isAligningWithCamera = false;
            cameraDistance = 2;
        }
         
        if (playerInput.currentControlScheme == "Gamepad")
        {
            HandleTurning(new Vector2(playerInput.look.x, -playerInput.look.y), lockCamera);
        }
        else
        {
            ConvertMovementInput(playerInput.movingForward, playerInput.movingBackward, playerInput.movingLeft, playerInput.movingRight);
            HandleTurning(movementVector, lockCamera);
        }
        if (isShaking)
        {
            InhaleCameraFeedback(shakeAmplitude);
            if (isAligningWithCamera) // if not zooming out
            {
                cameraDistance = 0;
            }
        }
        else
        {
            InhaleCameraFeedback(0f);
            if (isAligningWithCamera) // if not zooming out
            {
                cameraDistance = 1;
            }
        }

        LerpCameraDistance();
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
        characterRb.MoveRotation(Quaternion.Lerp(characterRb.rotation, targetRotation, cameraLerpSpeed * Time.fixedDeltaTime));
        characterRb.angularVelocity = Vector3.zero; // clear all angular acceleration
    }

    void ZoomOutCamera()
    {
        isAligningWithCamera = false;

        // Smoothly restore the original camera distance
        if (thirdPersonFollow != null)
        {
            thirdPersonFollow.CameraDistance = Mathf.Lerp(thirdPersonFollow.CameraDistance, ZoomOutCameraDistance, cameraLerpSpeed * Time.fixedDeltaTime);
        }
    }

    void InhaleCameraFeedback(float intensity)
    {
        // Shake and zoom in camera
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;

    }

    void LerpCameraDistance()
    {
        switch (cameraDistance)
        {
            case 0:
                targetCameraDistance = ZoomInCameraDistance; 
                break;
            case 1:
                targetCameraDistance = defaultCameraDistance; break;
                
            case 2:
                targetCameraDistance = ZoomOutCameraDistance; break;
        }
        thirdPersonFollow.CameraDistance = Mathf.Lerp(thirdPersonFollow.CameraDistance, targetCameraDistance, cameraLerpSpeed * Time.fixedDeltaTime);
    }

    void OnDestroy()
    {
        chargingInstance.release();
    }
}