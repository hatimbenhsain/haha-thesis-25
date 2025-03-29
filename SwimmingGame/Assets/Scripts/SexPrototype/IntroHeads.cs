using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroHeads : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float thrustInterval = 2f;
    public float thrustForce = 1f;
    public float moveBackDistance = 5f; // Distance to move back
    private Rigidbody rb;
    public bool thrusted = false; // Flag to check if thrust has occurred

    private float thrustTimer = 0f; // Timer to track thrust intervals
    private Vector3 moveBackStep; // Step to move back per frame

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveBackStep = transform.forward * (moveBackDistance / thrustInterval); // Calculate backward step
    }

    // Update is called once per frame
    void Update()
    {
        thrustTimer += Time.deltaTime;

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

            // Reset the timer
            thrustTimer = 0f;
            thrusted = true;
        }
    }
}
