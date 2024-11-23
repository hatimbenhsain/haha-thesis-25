using UnityEngine;

public class FingerTipsController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float maxMoveRadius = 1.5f;
    public float dampingFactor = 0.9f;
    public float lerpSpeed = 1.0f;
    public float yLerpSpeed = 1.0f;
    public float yOffset = 1.0f;
    public float inputResetDelay = 0.5f; // Time in seconds before resetting position

    [Header("References")]
    public GameObject reference;
    public LayerMask sexPartnerMask;

    private PlayerInput playerInput;
    private Vector3 startLocalPosition;
    private Vector3 velocity;
    private CuddleGameManager gameManager;
    public string lastDialogueOption = "Null option";
    private bool isUsingGamepad;

    private float inputTimer = 0f; // Tracks time since last input

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        startLocalPosition = transform.localPosition; // Set the starting position in local space
        gameManager = FindObjectOfType<CuddleGameManager>();
    }

    void FixedUpdate()
    {
        HandleInput();
        Moving();

        if (inputTimer >= inputResetDelay)
        {
            ResetPosition();
        }
        AdjustYPosition();
    }

    // Handle input and determine control scheme
    private void HandleInput()
    {
        if (playerInput.currentControlScheme == "Gamepad")
        {
            isUsingGamepad = true;
        }
        else
        {
            isUsingGamepad = false;
        }
    }

    private void Moving()
    {
        float moveX = isUsingGamepad ? -playerInput.rotation.y : -playerInput.look.y;
        float moveY = isUsingGamepad ? playerInput.rotation.x : playerInput.look.x;

        if (Mathf.Abs(moveX) > 0.01f || Mathf.Abs(moveY) > 0.01f)
        {
            DetectDialogueOption();
            Vector3 inputDirection = new Vector3(moveX, moveY, 0).normalized;
            velocity = inputDirection * moveSpeed * Time.fixedDeltaTime;
            inputTimer = 0f; 
        }
        else
        {
            velocity *= dampingFactor; // Apply damping
            inputTimer += Time.fixedDeltaTime; // increment timer when no input
        }

        Vector3 localPosition = transform.localPosition + velocity;
        Vector3 localOffsetFromCenter = localPosition - startLocalPosition;

        if (localOffsetFromCenter.magnitude > maxMoveRadius)
        {
            localOffsetFromCenter = localOffsetFromCenter.normalized * maxMoveRadius;
            localPosition = startLocalPosition + localOffsetFromCenter;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, localPosition, lerpSpeed * Time.fixedDeltaTime);
    }

    // Return to the reference object's position
    private void ResetPosition()
    {
        Vector3 referenceWorldPosition = reference.transform.position;
        transform.position = Vector3.Lerp(transform.position, referenceWorldPosition, lerpSpeed * Time.fixedDeltaTime);
        startLocalPosition = transform.localPosition; // Update start position
    }

    // Detect nearby dialogue options
    private void DetectDialogueOption()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.1f, LayerMask.GetMask("DialogueOption"));
        foreach (var hitCollider in hitColliders)
        {
            string currentOption = hitCollider.gameObject.name;
            if (currentOption != lastDialogueOption)
            {
                lastDialogueOption = currentOption;
                gameManager.UpdateDialogueText(currentOption); 
            }
        }
    }

    // Adjust Y position based on the highest object below
    private void AdjustYPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x, 100f, transform.position.z), Vector3.down, out hit, Mathf.Infinity, sexPartnerMask))
        {
            Vector3 targetPosition = new Vector3(transform.position.x, hit.point.y + yOffset, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, yLerpSpeed * Time.fixedDeltaTime);
        }
    }
}
