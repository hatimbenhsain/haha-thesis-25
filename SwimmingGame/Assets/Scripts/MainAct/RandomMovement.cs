using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public BoxCollider movementArea;  // BoxCollider defining the area for movement
    public float moveSpeed = 3f;      // Movement speed of the fish
    public float turnSpeed = 2f;      // Turning speed
    public float changeDirectionInterval = 2f;  // Time interval to change direction

    private Vector3 targetPosition;   // Next target position inside the box
    private float timer;              // Timer to track when to change direction

    void Start()
    {
        GetNewTargetPosition();  // Set initial target position
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Rotate smoothly towards the target direction
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);

        // If the fish reaches the target or it's time to change direction, get a new target
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f || timer >= changeDirectionInterval)
        {
            GetNewTargetPosition();
            timer = 0f;  // Reset the timer
        }
    }

    void GetNewTargetPosition()
    {
        // Get a random point within the BoxCollider's bounds
        Bounds bounds = movementArea.bounds;
        targetPosition = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (movementArea != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(movementArea.bounds.center, movementArea.bounds.size);
        }
    }
}
