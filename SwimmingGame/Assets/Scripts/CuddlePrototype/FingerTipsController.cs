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
    public float circleDuration = 1.0f; // duration for completing one full circle
    public GameObject reference;

    private PlayerInput playerInput;
    private Vector3 startLocalPosition;
    private Vector3 velocity;
    private Vector2 movementVector;
    private float yPos; // y position after raycast
    private bool isCircling = false; // flag to check if circular movement is in progress

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        startLocalPosition = transform.localPosition; // set the starting position in local space
    }

    void Update()
    {
        // Start circular movement if space is pressed and we're not already circling
        if (Input.GetKeyDown(KeyCode.Space) && !isCircling)
        {
            yPos = transform.position.y;
            StartCoroutine(CircularMovement());
        }

        // Only process movement if not in circular mode
        if (!isCircling)
        {
            Moving();
            Debug.Log(DetectDialogueOption());

        }

        AdjustYPosition();
    }

    // handle movement of the character around, constrained within a small circle in XZ plane
    void Moving()
    {
        float moveX = -playerInput.look.y;
        float moveY = playerInput.look.x;

        if (Mathf.Abs(moveX) > 0.01f || Mathf.Abs(moveY) > 0.01f)
        {
            Vector3 inputDirection = new Vector3(moveX, moveY, 0).normalized;
            velocity = inputDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            velocity *= dampingFactor;
        }

        Vector3 localPosition = transform.localPosition + velocity;
        Vector3 localOffsetFromCenter = localPosition - startLocalPosition;
        if (localOffsetFromCenter.magnitude > maxMoveRadius)
        {
            localOffsetFromCenter = localOffsetFromCenter.normalized * maxMoveRadius;
            localPosition = startLocalPosition + localOffsetFromCenter;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, localPosition, lerpSpeed * Time.deltaTime);
    }

    // Coroutine to perform a circular movement around the radius
    IEnumerator CircularMovement()
    {
        // Store the initial local position
        startLocalPosition = transform.localPosition;
        isCircling = true;
        float elapsedTime = 0f;

        // Store the original local z position
        float originalZ = transform.localPosition.z;

        while (elapsedTime < circleDuration)
        {
            // Calculate the angle for each point in the circle
            float angle = Mathf.Lerp(0f, 2f * Mathf.PI, elapsedTime / circleDuration);
            float x = Mathf.Cos(angle) * maxMoveRadius;
            float y = Mathf.Sin(angle) * maxMoveRadius;

            // Set the position based on the circle path, keeping the original z value
            Vector3 circularPosition = startLocalPosition + new Vector3(x, y, originalZ);
            transform.localPosition = Vector3.Lerp(transform.localPosition, circularPosition, lerpSpeed * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Lerp to the reference object's absolute position after the circle is complete
        Vector3 referenceWorldPosition = reference.transform.position;
        float returnElapsedTime = 0f;
        while (returnElapsedTime < (circleDuration * 0.3f))
        {
            // Lerp to the reference object's world position
            transform.position = Vector3.Lerp(transform.position, referenceWorldPosition, lerpSpeed * Time.deltaTime * 2);
            returnElapsedTime += Time.deltaTime;
            yield return null;
        }

        // Update the start local position to the current local position
        startLocalPosition = transform.localPosition;
        isCircling = false;
    }


    // raycast to adjust Y position based on the highest object below
    void AdjustYPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x, 100f, transform.position.z), Vector3.down, out hit, Mathf.Infinity, sexPartnerMask))
        {
            Vector3 targetPosition = new Vector3(transform.position.x, hit.point.y + yOffset, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, yLerpSpeed * Time.deltaTime);
        }
    }

    // Lerp the fingertip object to be offset from the reference object's position
    public void LerpToReference(Transform referenceObject, Vector3 offset)
    {
        // Calculate the target position based on the reference object's position and the offset
        Vector3 targetPosition = referenceObject.position + offset;

        // Lerp the fingertip's position to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);

        // Reset localPosition after lerping
        transform.localPosition = startLocalPosition;
    }

    public string DetectDialogueOption()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.1f, LayerMask.GetMask("DialogueOption"));
        foreach (var hitCollider in hitColliders)
        {
            // Return the name of the GameObject with the trigger box
            return hitCollider.gameObject.name;
        }
        return "Null option"; // No object detected
    }
}
