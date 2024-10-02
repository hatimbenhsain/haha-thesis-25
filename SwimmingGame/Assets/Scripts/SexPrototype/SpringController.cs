using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringController : MonoBehaviour
{
    public GameObject character; 
    public Camera playerCamera; // reference to the camera

    public float maxInhaleTime = 3f; // time to fully inhale
    public float maxExhaleTime = 1f; // maximum exhale time
    public float minExhaleTime = 0f; // minimum exhale time
    public float acceleration = 10f; // adjustable acceleration
    public float drag = 1f; // adjustable drag
    public float turnSpeed = 5f; // speed at which the character turns
    public float lerpSpeed = 5f; // speed for the character to align with the camera direction
    public float tiltAngle = 15f; // max tilt angle when pressing WASD

    private Vector3 originalScale; // original scale of the cylinder
    private Rigidbody characterRb;
    public bool isInhaling = false;
    private float inhaleStartTime = 0f;
    private float inhaleDuration = 0f;
    private float exhaleForce = 0f; // forward force based on inhale time
    private bool isAligningWithCamera = false; // flag to check if aligning with camera direction
    private Quaternion targetRotation; // target rotation based on camera direction

    private float exhaleTimeLeft = 0f; // time remaining for the exhale process

    void Start()
    {
        originalScale = character.transform.localScale; // store the original scale
        characterRb = character.GetComponent<Rigidbody>(); // get the rigidbody for physics-based movement
        characterRb.drag = drag; // set the drag for the character's movement
        characterRb.useGravity = false; // disable gravity for the character
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // LMB
        {
            StartInhaling();
        }

        if (Input.GetMouseButtonUp(0)) // LMB
        {
            StartExhaling();
        }

        if (Input.GetMouseButton(1)) // RMB
        {
            AlignWithCamera();
        }
        else
        {
            isAligningWithCamera = false; // stop aligning when RMB is released
        }
    }

    void FixedUpdate()
    {
        if (isInhaling)
        {
            Inhale();
        }

        // handle turning and movement based on WASD input
        HandleTurning();

        // handle exhaling process
        if (exhaleTimeLeft > 0f)
        {
            Exhale();
        }

        // smoothly rotate to the target direction if aligning
        if (isAligningWithCamera)
        {
            characterRb.MoveRotation(Quaternion.Lerp(characterRb.rotation, targetRotation, lerpSpeed * Time.fixedDeltaTime));
            characterRb.angularVelocity = Vector3.zero; // clear all angular acceleration
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
        character.transform.localScale = Vector3.Lerp(originalScale, new Vector3(1.5f, 1.5f, 0.5f), t); // for cylinder scale
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

        // handle tilt during exhale based on WASD input
        HandleExhaleTilt();

        exhaleTimeLeft -= Time.fixedDeltaTime;

        // when done exhaling, reset the scale and force
        if (exhaleTimeLeft <= 0f)
        {
            character.transform.localScale = originalScale;
        }
    }

    // handle turning based on WASD keys
    void HandleTurning()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Quaternion targetTilt = Quaternion.Euler(90, characterRb.rotation.eulerAngles.y, 0);
            characterRb.MoveRotation(Quaternion.Slerp(characterRb.rotation, targetTilt, turnSpeed * Time.fixedDeltaTime));
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Quaternion targetTilt = Quaternion.Euler(-90, characterRb.rotation.eulerAngles.y, 0);
            characterRb.MoveRotation(Quaternion.Slerp(characterRb.rotation, targetTilt, turnSpeed * Time.fixedDeltaTime));
        }
        else if (Input.GetKey(KeyCode.A))
        {
            characterRb.MoveRotation(characterRb.rotation * Quaternion.Euler(0, -turnSpeed * Time.fixedDeltaTime * 100, 0));
        }
        else if (Input.GetKey(KeyCode.D))
        {
            characterRb.MoveRotation(characterRb.rotation * Quaternion.Euler(0, turnSpeed * Time.fixedDeltaTime * 100, 0));
        }
    }

    // handle tilt during exhale based on WASD input
    void HandleExhaleTilt()
    {
        // Apply slight tilts based on movement inputs
        if (Input.GetKey(KeyCode.W))
        {
            characterRb.MoveRotation(characterRb.rotation * Quaternion.Euler(-tiltAngle * Time.fixedDeltaTime, 0, 0)); // slight tilt upward
        }
        else if (Input.GetKey(KeyCode.S))
        {
            characterRb.MoveRotation(characterRb.rotation * Quaternion.Euler(tiltAngle * Time.fixedDeltaTime, 0, 0)); // slight tilt downward
        }

        if (Input.GetKey(KeyCode.A))
        {
            characterRb.MoveRotation(characterRb.rotation * Quaternion.Euler(0, 0, tiltAngle * Time.fixedDeltaTime)); // slight tilt left
        }
        else if (Input.GetKey(KeyCode.D))
        {
            characterRb.MoveRotation(characterRb.rotation * Quaternion.Euler(0, 0, -tiltAngle * Time.fixedDeltaTime)); // slight tilt right
        }
    }

    // align character with camera direction
    void AlignWithCamera()
    {
        Vector3 cameraForward = playerCamera.transform.forward;
        targetRotation = Quaternion.LookRotation(cameraForward); // set target rotation based on camera direction
        isAligningWithCamera = true; // start aligning
    }
}