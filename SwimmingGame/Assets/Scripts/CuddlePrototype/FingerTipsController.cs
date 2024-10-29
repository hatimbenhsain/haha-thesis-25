using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerTipsController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float maxMoveRadius = 1.5f; // max radius for movement in XZ plane
    public float dampingFactor = 0.9f;
    public LayerMask sexPartnerMask;
    public float lerpSpeed = 1.0f; // the lerping speed for XZ movement
    public float yLerpSpeed = 1.0f; // the lerping speed for Y matching movement
    public float yOffset = 1.0f; // offset of the character from the top of touching objects


    private PlayerInput playerInput;
    private Vector3 startLocalPosition; 
    private Vector3 velocity;
    private Vector2 movementVector;

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        startLocalPosition = transform.localPosition; // set the starting position in local space
    }

    void Update()
    {
        ConvertMovementInput(playerInput.movingForward, playerInput.movingBackward, playerInput.movingLeft, playerInput.movingRight);
        Moving();
        AdjustYPosition();
    }

    void ConvertMovementInput(bool movingForward, bool movingBackward, bool movingLeft, bool movingRight)
    {
        if (movingForward) { movementVector.x += 1; }
        if (movingBackward) { movementVector.x += -1; }
        if (movingLeft) { movementVector.y += 1; }
        if (movingRight) { movementVector.y += -1; }
        movementVector = movementVector.normalized;
        //Debug.Log(movementVector);
    }

    // handle movement of the character around, constrained within a small circle in XZ plane
    void Moving()
    {
        // get input direction from mouse or joystick
        float moveX = -movementVector.x;
        float moveY = -movementVector.y;

        // if there is input, update velocity based on input direction
        if (Mathf.Abs(moveX) > 0.01f || Mathf.Abs(moveY) > 0.01f)
        {
            Vector3 inputDirection = new Vector3(moveX, moveY, 0).normalized;
            velocity = inputDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            // apply damping to slow down velocity when there is no input
            velocity *= dampingFactor;
        }

        Vector3 localPosition = transform.localPosition + velocity;
        Vector3 localOffsetFromCenter = localPosition - startLocalPosition;
        if (localOffsetFromCenter.magnitude > maxMoveRadius)
        {
            // scale the offset back to the maximum radius
            localOffsetFromCenter = localOffsetFromCenter.normalized * maxMoveRadius;
            localPosition = startLocalPosition + localOffsetFromCenter;
        }

        // lerp the character to the target local position within the radius
        transform.localPosition = Vector3.Lerp(transform.localPosition, localPosition, lerpSpeed * Time.deltaTime);
    }

    // raycast to adjust Y position based on the highest object below
    void AdjustYPosition()
    {
        RaycastHit hit;

        // casting a ray downward to find the highest object
        if (Physics.Raycast(new Vector3(transform.position.x, 100f, transform.position.z), Vector3.down, out hit, Mathf.Infinity, sexPartnerMask))
        {
            // set Y position to the top of the hit object
            Vector3 targetPosition = new Vector3(transform.position.x, hit.point.y + yOffset, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, yLerpSpeed * Time.deltaTime);
        }
    }
}
