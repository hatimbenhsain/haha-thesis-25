using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringController : MonoBehaviour
{
    public GameObject character; // cylinder representing the character
    public Camera playerCamera; // reference to the camera

    public float maxInhaleTime = 3f; // time to fully inhale
    public float maxExhaleTime = 1f; // maximum exhale time
    public float minExhaleTime = 0f; // minimum exhale time
    public float acceleration = 10f; // adjustable acceleration
    public float drag = 1f; // adjustable drag
    public float turnSpeed = 5f; // speed at which the character turns
    public float lerpSpeed = 5f; // speed for the character to align with the camera direction

    private Vector3 originalScale; // original scale of the cylinder
    private Rigidbody characterRb;
    private bool isInhaling = false;
    private float inhaleStartTime = 0f;
    private float inhaleDuration = 0f;
    private float exhaleForce = 0f; // forward force based on inhale time
    private bool isAligningWithCamera = false; // flag to check if aligning with camera direction
    private Quaternion targetRotation; // target rotation based on camera direction

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

        if (isInhaling)
        {
            Inhale();
        }

        // handle turning based on WASD input
        HandleTurning();

        // align character with camera when RMB is held
        if (Input.GetMouseButton(1)) // RMB
        {
            AlignWithCamera();
        }
        else
        {
            isAligningWithCamera = false; // stop aligning when RMB is released
        }

        // smoothly rotate to the target direction if aligning
        if (isAligningWithCamera)
        {
            character.transform.rotation = Quaternion.Lerp(character.transform.rotation, targetRotation, lerpSpeed * Time.deltaTime);
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

    // starts the exhaling process and applies forward force
    void StartExhaling()
    {
        isInhaling = false;

        float heldInhaleTime = Time.time - inhaleStartTime;
        float exhaleTime = Mathf.Clamp(heldInhaleTime / maxInhaleTime, minExhaleTime, maxExhaleTime);

        exhaleForce = Mathf.Clamp(heldInhaleTime / maxInhaleTime, 0, 1) * acceleration; // calculate the exhale force based on inhale time

        StartCoroutine(Exhale(exhaleTime));
    }

    // exhale lerping scale and applying forward acceleration
    System.Collections.IEnumerator Exhale(float exhaleTime)
    {
        Vector3 currentScale = character.transform.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / exhaleTime;
            character.transform.localScale = Vector3.Lerp(currentScale, originalScale, t);

            // apply forward force proportional to inhale
            characterRb.AddForce(character.transform.forward * exhaleForce, ForceMode.Acceleration);

            yield return null;
        }
    }

    // handle turning based on WASD keys
    void HandleTurning()
    {
        if (Input.GetKey(KeyCode.W))
        {
            character.transform.rotation = Quaternion.Slerp(character.transform.rotation, Quaternion.Euler(90, character.transform.eulerAngles.y, 0), turnSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            character.transform.rotation = Quaternion.Slerp(character.transform.rotation, Quaternion.Euler(-90, character.transform.eulerAngles.y, 0), turnSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            // rotate left relative to the current yaw
            character.transform.Rotate(0, -turnSpeed * Time.deltaTime * 100, 0);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // rotate right relative to the current yaw
            character.transform.Rotate(0, turnSpeed * Time.deltaTime * 100, 0);
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