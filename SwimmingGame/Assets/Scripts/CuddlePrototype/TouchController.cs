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

    private PlayerInput playerInput;
    private Vector3 targetPosition; // the target position for Lerp movement
    private Vector3 rubOffset; // the offset for the circular rubbing motion

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        targetPosition = transform.position;
    }

    void Update()
    {
        Moving();
        AdjustYPosition();
    }

    // handle movement of the character around
    void Moving()
    {
        float moveX = playerInput.look.x;
        float moveZ = playerInput.look.y;


        // calculate the target position based on input
        Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;
        targetPosition = transform.position + move;
        // lerp the character to the target position within the allowed circular radius
        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
    }

    // handle smaller circular rubbing movement


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
