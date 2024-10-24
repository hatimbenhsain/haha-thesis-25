using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubbingTips : MonoBehaviour
{
    public Rigidbody object1;  // look
    public Rigidbody object2;  // rotation
    public float moveForce = 10f;  // Force applied for movement
    public float dragFactor = 0.95f;  // Drag factor to slow down objects

    private PlayerInput playerInput;

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
    }

    void Update()
    {
        ApplyMovement(object1, playerInput.look.x, playerInput.look.y);
        ApplyMovement(object2, playerInput.rotation.x, playerInput.rotation.y);
    }

    // Apply force-based movement based on input
    void ApplyMovement(Rigidbody rb, float inputX, float inputY)
    {
        // Calculate the movement direction in XZ plane
        Vector3 movementDirection = new Vector3(inputX, 0, inputY).normalized;

        // Apply force to the rigidbody based on input
        rb.AddForce(movementDirection * moveForce);

        // Apply drag to gradually slow down the object
        rb.velocity *= dragFactor;
    }
}
