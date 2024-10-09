using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SpringController : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject character;
    public Camera playerCamera;
    public CinemachineVirtualCamera virtualCamera;

    [Header("Movement Parameters")]
    public float maxInhaleTime = 3f;
    public float maxExhaleTime = 1f;
    public float minExhaleTime = 0f;
    public float acceleration = 10f;
    public float drag = 1f;
    public float turnSpeed = 5f; // speed at which the character turns
    public float lerpSpeed = 5f; // speed for the character to align with the camera direction
    public float tiltAngle = 15f;
    public float movementVelocityThreshold;
    public float movementMultiplier = 1f;  // speed for movement
    public float movementLerpSpeed = 1f;
    public bool isExhaling = false;
    public bool isInhaling = false;

    [Header("Camera Parameters")]
    public float zoomedInDistance;

    private PlayerInput playerInput;
    private Vector3 originalScale;
    private Rigidbody characterRb;
    private float inhaleStartTime = 0f;
    private float inhaleDuration = 0f;
    private float exhaleForce = 0f;
    private bool isAligningWithCamera = false;
    private Quaternion targetRotation;
    private Vector3 currentVelocity;
    private float originalCameraDistance;
    private Cinemachine3rdPersonFollow thirdPersonFollow;


    private float exhaleTimeLeft = 0f;

    void Start()
    {
        originalScale = character.transform.localScale;
        characterRb = character.GetComponent<Rigidbody>();
        characterRb.drag = drag;
        characterRb.useGravity = false;
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

        if (playerInput.aiming)
        {
            AlignWithCamera();
        }
        else
        {
            isAligningWithCamera = false;
            RestoreOriginalCameraDistance();
        }
    }


    void FixedUpdate()
    {

        if (isInhaling)
        {
            Inhale();
        }

        //HandleTurning();

        // handle exhaling process
        if (exhaleTimeLeft > 0f)
        {
            Exhale();
            isExhaling = true;
        }
        else
        {
            isExhaling = false;
        }
    }

    // starts the inhaling process
    void StartInhaling()
    {
        isInhaling = true;
        inhaleStartTime = Time.time;
    }

    // inhale lerping scale
    void Inhale()
    {
        float inhaleTime = Time.time - inhaleStartTime;
        inhaleDuration = Mathf.Clamp(inhaleTime, 0, maxInhaleTime);

        float t = inhaleDuration / maxInhaleTime;
        character.transform.localScale = Vector3.Lerp(originalScale, new Vector3(1.2f, 1.2f, 1f), t); // for cylinder scale
    }

    // starts the exhaling process and calculates the force
    void StartExhaling()
    {
        isInhaling = false;

        float heldInhaleTime = Time.time - inhaleStartTime;
        exhaleTimeLeft = Mathf.Clamp(heldInhaleTime / maxInhaleTime, minExhaleTime, maxExhaleTime);

        exhaleForce = Mathf.Clamp(heldInhaleTime / maxInhaleTime, 0, 1) * acceleration; // calculate the exhale force based on inhale time
    }

    // exhale lerping scale and applying forward acceleration
    void Exhale()
    {
        float t = 1f - (exhaleTimeLeft / maxExhaleTime); // progress of the exhale time
        character.transform.localScale = Vector3.Lerp(character.transform.localScale, originalScale, t);

        // apply forward force proportional to inhale
        characterRb.AddForce(character.transform.forward * exhaleForce, ForceMode.Acceleration);

        exhaleTimeLeft -= Time.fixedDeltaTime;

        // when done exhaling, reset the scale and force
        if (exhaleTimeLeft <= 0f)
        {
            character.transform.localScale = originalScale;
        }
    }

    // TODO: Think about whether or not to keep this
    void HandleTurning()
    {
        Vector3 moveDirection = Vector3.zero;
        bool isMoving = false;

        // Forward tilt (up direction)
        if (playerInput.look.y < 0f)
        {
            Quaternion targetTilt = Quaternion.Euler(-90, characterRb.rotation.eulerAngles.y, 0);
            characterRb.MoveRotation(Quaternion.Slerp(characterRb.rotation, targetTilt, turnSpeed * Time.fixedDeltaTime));
            moveDirection += character.transform.up;
            isMoving = true;
        }
        // Backward tilt (down direction)
        else if (playerInput.look.y > 0f)
        {
            Quaternion targetTilt = Quaternion.Euler(90, characterRb.rotation.eulerAngles.y, 0);
            characterRb.MoveRotation(Quaternion.Slerp(characterRb.rotation, targetTilt, turnSpeed * Time.fixedDeltaTime));
            moveDirection -= character.transform.up;
            isMoving = true;
        }
        // Left tilt (around y-axis)
        else if (playerInput.look.x < 0f)
        {
            Quaternion targetTilt = Quaternion.Euler(characterRb.rotation.eulerAngles.x, characterRb.rotation.eulerAngles.y - 90, 0);
            characterRb.MoveRotation(Quaternion.Slerp(characterRb.rotation, targetTilt, turnSpeed * Time.fixedDeltaTime));
            moveDirection -= character.transform.right;  // Keep moving in the character's right direction
            isMoving = true;
        }
        // Right tilt (turning right)
        else if (playerInput.look.x > 0f)
        {
            Quaternion targetTilt = Quaternion.Euler(characterRb.rotation.eulerAngles.x, characterRb.rotation.eulerAngles.y + 90, 0);
            characterRb.MoveRotation(Quaternion.Slerp(characterRb.rotation, targetTilt, turnSpeed * Time.fixedDeltaTime));
            moveDirection += character.transform.right;  // Keep moving in the character's right direction
            isMoving = true;
        }

        // Handle movement
        if (isMoving && characterRb.velocity.magnitude > movementVelocityThreshold)
        {
            Vector3 targetVelocity = moveDirection.normalized * movementMultiplier;
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.fixedDeltaTime * movementLerpSpeed);
            characterRb.AddForce(currentVelocity, ForceMode.Acceleration);
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