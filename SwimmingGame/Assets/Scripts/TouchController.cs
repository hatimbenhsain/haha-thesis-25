using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    public float moveSpeed = 5f;  
    public LayerMask sexPartnerMask;

    private PlayerInput playerInput;

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
    }

    void Update()
    {
        float moveX = playerInput.look.x;
        float moveZ = playerInput.look.y;
        Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;
        transform.Translate(move);

        // raycast from above the character to detect the highest object below
        AdjustYPosition();
    }

    void AdjustYPosition()
    {
        RaycastHit hit;

        // casting a ray downward from infinite
        if (Physics.Raycast(new Vector3(transform.position.x, 100f, transform.position.z), Vector3.down, out hit, Mathf.Infinity, sexPartnerMask))
        {
            // set Y position to the top of the hit object
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
    }
}
