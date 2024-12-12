using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerTipsRefOffset : MonoBehaviour
{
    public Transform hand;
    private Vector3 offset;
    public float yOffset = 1.0f;
    public float yLerpSpeed = 1.0f;
    public LayerMask sexPartnerMask;

    void Start()
    {
        offset = transform.position - hand.position;
    }

    void Update()
    {
        // Update the position of fingetips while keeping the same offset
        transform.position = new Vector3(hand.position.x + offset.x, transform.position.y, hand.position.z + offset.z);
        AdjustYPosition();
    }

    void AdjustYPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x, 100f, transform.position.z), Vector3.down, out hit, Mathf.Infinity, sexPartnerMask))
        {
            Vector3 targetPosition = new Vector3(transform.position.x, hit.point.y + yOffset, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, yLerpSpeed * Time.deltaTime);
        }
    }
}
