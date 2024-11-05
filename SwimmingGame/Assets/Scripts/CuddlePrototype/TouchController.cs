using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public LayerMask sexPartnerMask;
    public float lerpSpeed = 1.0f; // the lerping speed for XZ movement
    public float yLerpSpeed = 1.0f; // the lerping speed for Y matching movement
    public float yOffset = 1.0f; // offset of the character from the top of touching objects
    public Quaternion rotationOffset; // offset the rotation after timing normal


    private PlayerInput playerInput;
    private Vector3 targetPosition;
    private Quaternion initialRotation;
    private Vector2 movementVector;
    private bool isUsingGamepad;

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        targetPosition = transform.position;

        // Store the initial rotation of the object
        initialRotation = transform.rotation;
    }

    void Update()
    {
        // handle input change
        if (playerInput.currentControlScheme == "Gamepad")
        {
            isUsingGamepad = true;
        }
        else
        {
            isUsingGamepad = false;
        }
        Moving();
        if (isUsingGamepad == false)
        {
            ConvertMovementInput(playerInput.movingForward, playerInput.movingBackward, playerInput.movingLeft, playerInput.movingRight);
        }       
        AdjustPositionAndRotation();
    }

    // handle movement of the character around
    void Moving()
    {
        float moveX = 0f;
        float moveZ = 0f;
        // using keyboard
        if (isUsingGamepad == false)
        {
            moveX = -movementVector.y;
            moveZ = movementVector.x;
        }
        // using gamepad
        else
        {
            moveX = playerInput.look.x;
            moveZ = playerInput.look.y;
        }


        // calculate the target position based on input
        Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;
        targetPosition = transform.position + move;

        // lerp the character to the target position within the allowed circular radius
        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
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

    void AdjustPositionAndRotation()
    {
        RaycastHit hit;

        // casting a ray downward to find the highest object
        if (Physics.Raycast(new Vector3(transform.position.x, 100f, transform.position.z), Vector3.down, out hit, Mathf.Infinity, sexPartnerMask))
        {
            // set Y position to the top of the hit object
            Vector3 positionTarget = new Vector3(transform.position.x, hit.point.y + yOffset, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, positionTarget, yLerpSpeed * Time.deltaTime);

            // Calculate the rotation offset based on the initial rotation
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.right, hit.normal) * initialRotation * rotationOffset; ;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, yLerpSpeed * Time.deltaTime);
        }
    }

}
