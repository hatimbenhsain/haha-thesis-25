using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroHeads : MonoBehaviour
{
    public float moveSpeedMin = 3f;
    public float moveSpeedMax = 7f;
    public float moveSpeed = 5f;
    public float thrustIntervalMin = 1f;
    public float thrustIntervalMax = 3f;
    public float thrustInterval = 2f;

    public float thrustForceMin = 0.5f;
    public float thrustForceMax = 2f;
    public float thrustForce = 1f;

    public float moveBackDistanceMin = 3f;
    public float moveBackDistanceMax = 7f;
    public float moveBackDistance = 5f; // Distance to move back

    private Rigidbody rb;
    public bool thrusted = false; // Flag to check if thrust has occurred
    public bool lockRigidBodyRotation = true;

    private float thrustTimer = 0f; // Timer to track thrust intervals
    private Vector3 moveBackStep; // Step to move back per frame
    public Intro intro;
    public bool goCrazy = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveBackStep = transform.forward * (moveBackDistance / thrustInterval); // Calculate backward step
    }

    // Update is called once per frame
    void Update()
    {
        if (goCrazy)
        {
            CrazyMovement();
            return; 
        }

        int intensity = intro.GetIntensity();
        if (intensity >= 4)
        {
            lockRigidBodyRotation = false;
        }

        thrustTimer += Time.deltaTime;

        // Lock or unlock Rigidbody rotation based on lockRigidBodyRotation
        if (lockRigidBodyRotation)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
        }

        // While the thrust timer is less than the thrust interval, move backward
        if (thrustTimer < thrustInterval)
        {
            if (thrusted)
            {
                transform.position -= moveBackStep * Time.deltaTime;
            }
        }

        if (thrustTimer >= thrustInterval)
        {
            // Apply a force in the forward direction
            rb.AddForce(transform.forward * thrustForce, ForceMode.Impulse);

            // Randomize the variables
            RandomizeVariables();

            // Reset the timer
            thrustTimer = 0f;
            thrusted = true;
        }
    }

    private void RandomizeVariables()
    {
        moveSpeed = Random.Range(moveSpeedMin, moveSpeedMax);
        thrustInterval = Random.Range(thrustIntervalMin, thrustIntervalMax);
        thrustForce = Random.Range(thrustForceMin, thrustForceMax);
        moveBackDistance = Random.Range(moveBackDistanceMin, moveBackDistanceMax);

        // Recalculate moveBackStep based on the new values
        moveBackStep = transform.forward * (moveBackDistance / thrustInterval);
    }

    private void CrazyMovement()
    {
        // Increase shake intensity
        float shakeAmount = 0.5f; 
        Vector3 randomShake = new Vector3(
            Random.Range(-shakeAmount, shakeAmount),
            Random.Range(-shakeAmount, shakeAmount),
            Random.Range(-shakeAmount, shakeAmount)
        );
        transform.position += randomShake;

        // Increase rotation speed for spinning effect
        float rotationSpeed = 500f; 
        Vector3 randomRotation = new Vector3(
            Random.Range(-rotationSpeed, rotationSpeed),
            Random.Range(-rotationSpeed, rotationSpeed),
            Random.Range(-rotationSpeed, rotationSpeed)
        );
        transform.Rotate(randomRotation * Time.deltaTime);
    }
}
