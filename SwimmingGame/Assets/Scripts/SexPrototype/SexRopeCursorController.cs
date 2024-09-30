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
        // Calculate the distance from the character to the rope origin
        float distance = Vector3.Distance(swimCharacter.transform.position, ropeOrigin.transform.position);

        // Determine the speed of the character (assuming using Rigidbody)
        float characterSpeed = swimCharacter.GetComponent<Rigidbody>().velocity.magnitude;

        // Calculate the desired length change
        float lengthChange = distance * speedMultiplier * characterSpeed * Time.deltaTime;
        Debug.Log(rope.particleCount);
        // Check current particle count and change length accordingly
        if (rope.particleCount < (2*pooledParticles))
        {
            cursor.ChangeLength(lengthChange);
        }
    }
}