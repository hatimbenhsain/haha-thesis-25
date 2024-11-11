using UnityEngine;
using Obi;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class RopeCollisionDetect : MonoBehaviour
{
    public ObiActor ropeA; // Reference to RopeA (the one with this script attached)
    public ObiActor ropeB; // Reference to RopeB (the other rope we want to detect collisions with)
    public TMPro.TMP_Text countA;
    public TMPro.TMP_Text countB;
    public ObiSolver solver;
    public float touchThreshold;
    private float contactNum;


    void Awake()
    {
    }

    void OnEnable()
    {
        solver.OnCollision += Solver_OnCollision;
        Debug.Log("Collision detection enabled.");
    }

    void OnDisable()
    {
        solver.OnCollision -= Solver_OnCollision;
        Debug.Log("Collision detection disabled.");
    }

    private void Solver_OnCollision(object sender, Obi.ObiNativeContactList contacts)
    {
        var world = ObiColliderWorld.GetInstance();

        List<int> ropeAParticles = new List<int>();
        List<int> ropeBParticles = new List<int>();

        foreach (Oni.Contact contact in contacts)
        {
            //Debug.Log("Processing contact - BodyA: " + contact.bodyA + ", BodyB: " + contact.bodyB + ", Distance: " + contact.distance);

            if (contact.distance < touchThreshold) // Adjusted threshold
            {
                // Retrieve the offset and size of the simplex in the solver.simplices array
                int simplexStartA = solver.simplexCounts.GetSimplexStartAndSize(contact.bodyA, out int simplexSizeA);
                int simplexStartB = solver.simplexCounts.GetSimplexStartAndSize(contact.bodyB, out int simplexSizeB);
                int particleIndexA = solver.simplices[contact.bodyA];
                int particleIndexB = solver.simplices[contact.bodyB];
                ObiSolver.ParticleInActor paA = solver.particleToActor[particleIndexA];
                ObiSolver.ParticleInActor paB = solver.particleToActor[particleIndexB];
                if (paA.actor.gameObject != paB.actor.gameObject)
                {
                    Debug.Log("collision success");
                    ropeBParticles.Add(particleIndexB);
                    contactNum++;
                }
                /*
                // Use this simplex information to retrieve the particles involved
                for (int i = 0; i < simplexSizeA; ++i)
                {
                    int particleIndexA = solver.simplices[simplexStartA + i];

                    // Retrieve the particle-in-actor data
                    ObiSolver.ParticleInActor paA = solver.particleToActor[particleIndexA];
                    Debug.Log($"A Particle Index: {particleIndexA}, Actor: {paA.actor.name}");
                    for (int j = 0; j < simplexSizeB; ++j)
                    {
                        int particleIndexB = solver.simplices[simplexStartB + j];

                        // Retrieve the particle-in-actor data
                        ObiSolver.ParticleInActor paB = solver.particleToActor[particleIndexB];
                        Debug.Log($"B Particle Index: {particleIndexB}, Actor: {paB.actor.name}");

                        if (paA.actor.gameObject!=paB.actor.gameObject)
                        {
                            Debug.Log("collision success");
                            ropeBParticles.Add(particleIndexB);
                            contactNum++;
                        }

                    }
                    
                }*/

                // Log detected particles
                //Debug.Log($"RopeA Particles Count: {ropeAParticles.Count}, RopeB Particles Count: {ropeBParticles.Count}");
            }
        }

        // Update TextMeshPro text components with the sizes of the lists
        countA.text = " " + contactNum;
        countB.text = " " + ropeBParticles.Count;

        // Print out the size of each list after processing all contacts
        //Debug.Log("Total RopeA Particles Added This Frame: " + ropeAParticles.Count);
        //Debug.Log("Total RopeB Particles Added This Frame: " + ropeBParticles.Count);
    }



}
