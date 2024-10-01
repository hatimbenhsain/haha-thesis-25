using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class StickySurfaceRopeInteraction : MonoBehaviour
{
    public float stickiness = 1.0f; // adjust the level of stickiness

    private ObiRope rope;
    private ObiSolver solver;
    private float[] stickTraction;
    private Vector3[] contactNormals;

    private void Start()
    {
        rope = GetComponent<ObiRope>();
        solver = rope.solver;

        // Subscribe to simulation and collision events
        rope.OnSimulationStart += ResetStickInfo;
        solver.OnCollision += AnalyzeContacts;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to avoid memory leaks
        rope.OnSimulationStart -= ResetStickInfo;
        solver.OnCollision -= AnalyzeContacts;
    }

    // Resets stickiness and contact normal info for each particle
    private void ResetStickInfo(ObiActor actor, float simulatedTime, float substepTime)
    {
        if (stickTraction == null)
        {
            stickTraction = new float[rope.activeParticleCount];
            contactNormals = new Vector3[rope.activeParticleCount];
        }

        // Reset traction and surface normals for each particle
        for (int i = 0; i < stickTraction.Length; ++i)
        {
            stickTraction[i] = 0;
            contactNormals[i] = Vector3.zero;
        }
    }

    // Analyzes contact points for stickiness
    private void AnalyzeContacts(object sender, ObiNativeContactList e)
    {
        for (int i = 0; i < e.count; ++i)
        {
            var contact = e[i];
            if (contact.distance < 0.005f) // Small distance implies contact
            {
                // Identify the simplex index of the particle involved in the collision
                int simplexIndex = solver.simplices[contact.bodyA];
                var particleInActor = solver.particleToActor[simplexIndex];

                // Check if the particle belongs to the rope and the other object is tagged "sticky"
                if (particleInActor != null && particleInActor.actor == rope)
                {
                    // Stick the particle to the surface (use stickiness value to control strength)
                    stickTraction[particleInActor.indexInActor] = stickiness;

                    // Accumulate the surface normal
                    contactNormals[particleInActor.indexInActor] += (Vector3)contact.normal;
                }
            }
        }
    }
}