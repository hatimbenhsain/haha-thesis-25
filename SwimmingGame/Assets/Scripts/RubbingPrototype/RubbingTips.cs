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
    private Vector2 movementVector;

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
    }

    void Update()
    {
        ConvertMovementInput(playerInput.movingForward, playerInput.movingBackward, playerInput.movingLeft, playerInput.movingRight);
        ApplyMovement(object1, playerInput.look.x, playerInput.look.y);
        ApplyMovement(object2, movementVector.x, movementVector.y);
    }

    void ConvertMovementInput(bool movingForward, bool movingBackward, bool movingLeft, bool movingRight)
    {
        if (movingForward) { movementVector.x += 1; }
        if (movingBackward) { movementVector.x += -1; }
        if (movingLeft) { movementVector.y += 1; }
        if (movingRight) { movementVector.y += -1; }
        movementVector = movementVector.normalized;
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