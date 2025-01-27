using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SexSpring : MonoBehaviour{
    public GameObject character;

    [Header("Movement Parameters")]
    public float maxInhaleTime = 1.5f;
    public float maxExhaleTime = 0.3f;
    public float minExhaleTime = 0f;
    public float acceleration = 80f;
    public float drag = 0.7f;
    public float turnSpeed = 0.7f; // speed at which the character turns
    public float tiltAngle = 45f;
    public float movementVelocityThreshold=0f;
    public float movementMultiplier = 5f;  // speed for movement
    public float movementLerpSpeed = 5f;
    public bool isExhaling = false;
    public bool isInhaling = false;

    [System.NonSerialized]
    public Rigidbody characterRb;
    [System.NonSerialized]
    public float inhaleStartTime, inhaleDuration, exhaleForce, exhaleTimeLeft, timeSinceExhale;
    [System.NonSerialized]
    public Vector3 originalScale, currentVelocity;
    [System.NonSerialized]
    public Quaternion targetRotation;


    public void SpringStart() {
        inhaleStartTime = 0f;
        inhaleDuration = 0f;
        exhaleForce = 0f;
        exhaleTimeLeft = 0f;
        timeSinceExhale=0f;

        originalScale = character.transform.localScale;
        characterRb = character.GetComponent<Rigidbody>();
        characterRb.drag = drag;
        characterRb.useGravity = false;
    }

    public void SpringUpdate(){
        if (isInhaling)
        {
            Inhale();
        }
    }

    public void SpringFixedUpdate(){
        timeSinceExhale+=Time.deltaTime;
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
    public void StartInhaling()
    {
        isInhaling = true;
        inhaleStartTime = Time.time;
    }

    // starts the exhaling process and calculates the force
    public void StartExhaling()
    {
        timeSinceExhale=0f;

        isInhaling = false;

        float heldInhaleTime = Time.time - inhaleStartTime;
        exhaleTimeLeft = Mathf.Clamp(heldInhaleTime / maxInhaleTime, minExhaleTime, maxExhaleTime);

        exhaleForce = Mathf.Clamp(heldInhaleTime / maxInhaleTime, 0, 1) * acceleration; // calculate the exhale force based on inhale time
    }

    // inhale lerping scale
    public void Inhale()
    {
        float inhaleTime = Time.time - inhaleStartTime;
        inhaleDuration = Mathf.Clamp(inhaleTime, 0, maxInhaleTime);

        float t = inhaleDuration / maxInhaleTime;
        character.transform.localScale = Vector3.Lerp(originalScale, new Vector3(1.7f, 1.7f, 1f), t); // for cylinder scale
    }

    public void HandleTurning(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        bool isMoving = false;

        // Backward tilt (up direction)
        if (input.y > 0f)
        {
            Quaternion targetTilt = Quaternion.Euler(-90, characterRb.rotation.eulerAngles.y, 0);
            characterRb.MoveRotation(Quaternion.Slerp(characterRb.rotation, targetTilt, turnSpeed * Time.fixedDeltaTime));
            moveDirection += character.transform.up;
            isMoving = true;
        }
        // Forward tilt (down direction)
        else if (input.y < 0f)
        {
            Quaternion targetTilt = Quaternion.Euler(90, characterRb.rotation.eulerAngles.y, 0);
            characterRb.MoveRotation(Quaternion.Slerp(characterRb.rotation, targetTilt, turnSpeed * Time.fixedDeltaTime));
            moveDirection -= character.transform.up;
            isMoving = true;
        }
        // Left tilt (around y-axis)
        else if (input.x < 0f)
        {
            Quaternion targetTilt = Quaternion.Euler(characterRb.rotation.eulerAngles.x, characterRb.rotation.eulerAngles.y - 90, 0);
            characterRb.MoveRotation(Quaternion.Slerp(characterRb.rotation, targetTilt, turnSpeed * Time.fixedDeltaTime));
            moveDirection -= character.transform.right;  
            isMoving = true;
        }
        // Right tilt (around y-axis)
        else if (input.x > 0f)
        {
            Quaternion targetTilt = Quaternion.Euler(characterRb.rotation.eulerAngles.x, characterRb.rotation.eulerAngles.y + 90, 0);
            characterRb.MoveRotation(Quaternion.Slerp(characterRb.rotation, targetTilt, turnSpeed * Time.fixedDeltaTime));
            moveDirection += character.transform.right;  
            isMoving = true;
        }

        // Handle movement
        if (isMoving && characterRb.velocity.magnitude > movementVelocityThreshold)
        {
            Vector3 targetVelocity = moveDirection.normalized * movementMultiplier;
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.fixedDeltaTime * movementLerpSpeed);
            //characterRb.AddForce(currentVelocity, ForceMode.Acceleration);
        }
    }

    public void TurnTowards(Vector3 position,float rotationSpeed){
        targetRotation=Quaternion.LookRotation(position-characterRb.transform.position,Vector3.up);
        Quaternion newRotationQ=Quaternion.Lerp(characterRb.transform.rotation,targetRotation,rotationSpeed*Time.fixedDeltaTime);
        //Rotating self
        characterRb.MoveRotation(newRotationQ);
    }

    public void TurnTowards(Quaternion rotation,float rotationSpeed){
        targetRotation=rotation;
        Quaternion newRotationQ=Quaternion.Lerp(characterRb.transform.rotation,targetRotation,rotationSpeed*Time.fixedDeltaTime);
        //Rotating self
        characterRb.MoveRotation(newRotationQ);
    }

    // exhale lerping scale and applying forward acceleration
    public void Exhale()
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
}