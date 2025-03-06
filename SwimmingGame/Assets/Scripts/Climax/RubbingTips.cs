using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubbingTips : MonoBehaviour
{
    public Rigidbody[] object1;  
    public Rigidbody[] object2;  
    public float moveForce = 10f;  // Force applied for movement
    public float dragFactor = 0.95f;  // Drag factor to slow down objects

    private PlayerInput playerInput;
    private Vector2 movementVector;
    private bool isUsingGamepad;

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
    }

    void FixedUpdate()
    {
        // handle input change
        if (playerInput.currentControlScheme == "Gamepad")
        {
            isUsingGamepad = true;
            for (int i = 0; i < object1.Length; i++)
            {
                ApplyMovement(object1[i], playerInput.look.x, playerInput.look.y);
            }
            for (int i = 0; i < object2.Length; i++)
            {
                ApplyMovement(object2[i], playerInput.rotation.x, -playerInput.rotation.y);
            }

        }
        else
        {
            isUsingGamepad = false;
            
            ConvertMovementInput(playerInput.movingForward, playerInput.movingBackward, playerInput.movingLeft, playerInput.movingRight);
            for (int i = 0; i < object1.Length; i++)
            {
                ApplyMovement(object1[i], playerInput.look.x, playerInput.look.y);
            }
            for (int i = 0; i < object2.Length; i++)
            {
                ApplyMovement(object2[i], -movementVector.y, movementVector.x);
            }
        }

    }

    void ConvertMovementInput(bool movingForward, bool movingBackward, bool movingLeft, bool movingRight)
    {
        // Reset movementVector before accumulating input
        movementVector.x = 0f;
        movementVector.y = 0f;

        // Determine movement direction based on input
        if (movingForward) { movementVector.x += 1; }
        if (movingBackward) { movementVector.x += -1; }
        if (movingLeft) { movementVector.y += 1; }
        if (movingRight) { movementVector.y += -1; }

        // Normalize the vector only if it has a non-zero length
        if (movementVector.magnitude > 1f)
        {
            movementVector.Normalize();
        }

        //Debug.Log(movementVector);
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
