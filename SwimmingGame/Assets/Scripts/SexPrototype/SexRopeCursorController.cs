using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class SexRopeCursorController : MonoBehaviour
{
    public GameObject ropeOrigin;
    public GameObject swimCharacter;
    public float speedMultiplier = 1f;
    public int pooledParticles = 100; // Maximum number of particles that can be generated

    private ObiRopeCursor cursor;
    private ObiRope rope;

    void Start()
    {
        cursor = GetComponentInChildren<ObiRopeCursor>();
        rope = cursor.GetComponent<ObiRope>();
    }

    void Update()
    {

        // Determine the speed of the character (assuming using Rigidbody)
        float characterSpeed = swimCharacter.GetComponent<Rigidbody>().velocity.magnitude;

        // Calculate the desired length change
        float lengthChange = speedMultiplier * characterSpeed * Time.deltaTime;

        // Only add rope length when space is pressed
        if (Input.GetKey(KeyCode.Space))
        {
            // Check if particle count is below the pooled particles limit
            if (rope.particleCount < (2 * pooledParticles))
            {
                cursor.ChangeLength(lengthChange);
            }
        }
    }
}