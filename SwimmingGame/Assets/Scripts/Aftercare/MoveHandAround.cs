using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHandAround : MonoBehaviour
{
    public float moveSpeed;
    public float lerpSpeed;
    [Tooltip("Max distance x-wise before moving the whole arm")]
    public float maxXDistance=0.7f;
    [Tooltip("Max distance z-wise before moving the whole arm")]
    public float maxZDistance=0.7f;
    public float parentMoveSpeed;  // Speed at which the parent object moves when out of bounds
    public Transform handController;  // Reference to the handController
    private PlayerInput playerInput;
    private Vector3 targetPosition;
    private bool isUsingGamepad;
    private Vector2 movementVector;
    public bool isMoving;
    public float decelerationSpeed = 0.1f;
    private Vector3 parentVelocity;
    public float boundingRadius;
    public Vector2 xPositionRange;
    public Vector2 zPositionRange;
    private Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        initialPosition = transform.position;  // Store the initial position of the object
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput.currentControlScheme == "Gamepad")
        {
            isUsingGamepad = true;
        }
        else
        {
            isUsingGamepad = false;
            ConvertMovementInput(playerInput.movingForward, playerInput.movingBackward, playerInput.movingLeft, playerInput.movingRight);
        }

        if (isMoving)
        {
            Moving();
        }
    }

    void Moving()
    {
        float moveX = 0f;
        float moveZ = 0f;

        // using keyboard
        if (!isUsingGamepad)
        {
            moveX = -movementVector.y;
            moveZ = movementVector.x;
        }
        // using gamepad
        else
        {
            moveX = playerInput.look.x;
            moveZ = -playerInput.look.y;
        }

        // calculate the target position based on input
        Vector3 move = Vector3.ClampMagnitude(new Vector3(moveX, 0, moveZ), 1f) * moveSpeed * Time.deltaTime;
        targetPosition = handController.position + move;

        // Check if the handController is within the max distance
        if (Mathf.Abs((transform.position - targetPosition).x) > maxXDistance || Mathf.Abs((transform.position - targetPosition).z) > maxZDistance)
        {
            // Move the parent object if the handController is out of bounds
            parentVelocity = parentMoveSpeed * Vector3.ClampMagnitude(new Vector3(moveX, 0, moveZ), 1f) * Time.deltaTime;
            transform.position += parentVelocity;
        }
        else
        {
            // Move the handController if within bounds
            handController.position = Vector3.Lerp(handController.position, targetPosition, lerpSpeed * Time.deltaTime);

            // Smoothly reduce the parent's velocity to zero if it has a non-zero speed
            if (parentVelocity != Vector3.zero)
            {
                parentVelocity = Vector3.Lerp(parentVelocity, Vector3.zero, decelerationSpeed * Time.deltaTime);
                transform.position += parentVelocity;
            }
        }

        // Clamp the transform position within the specified x and z ranges
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, xPositionRange.x, xPositionRange.y),
            transform.position.y,
            Mathf.Clamp(transform.position.z, zPositionRange.x, zPositionRange.y)
        );
    }

    void ConvertMovementInput(bool movingForward, bool movingBackward, bool movingLeft, bool movingRight)
    {
        // Reset movementVector before accumulating input
        movementVector.x = 0f;
        movementVector.y = 0f;

        // Determine movement direction based on input
        if (movingForward) { movementVector.x += 1; }
        if (movingBackward) { movementVector.x -= 1; }
        if (movingLeft) { movementVector.y += 1; }
        if (movingRight) { movementVector.y -= 1; }

        // Normalize the vector only if it has a non-zero length
        if (movementVector.magnitude > 1f)
        {
            movementVector.Normalize();
        }
    }
}
