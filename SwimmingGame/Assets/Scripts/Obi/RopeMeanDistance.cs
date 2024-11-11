using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using TMPro;

public class RopeMeanDistance : MonoBehaviour
{
    public ObiRope ropeA;
    public ObiRope ropeB;
    public TMP_Text distanceText;
    public float proximityThreshold = 0.5f;
    public ObiSolver solver;
    public float meanDistance;
    public int elementOffset;

    private int firstParticleA;
    private int firstParticleB;
    private int lastParticleA;
    private int lastParticleB;

    [Range(0, 100)] public float ropeAStartPercentage = 0f;
    [Range(0, 100)] public float ropeAEndPercentage = 100f;
    [Range(0, 100)] public float ropeBStartPercentage = 0f;
    [Range(0, 100)] public float ropeBEndPercentage = 100f;

    private void Start()
    {
        // Initialize first and last particles for ropeA
        firstParticleA = ropeA.elements[0].particle1;
        lastParticleA = elementOffset + ropeA.elements[ropeA.elements.Count - 1].particle2;

        // Initialize first and last particles for ropeB
        firstParticleB = ropeB.elements[0].particle1;
        lastParticleB = elementOffset + ropeB.elements[ropeB.elements.Count - 1].particle2;
    }

    void Update()
    {
        List<float> closeDistances = new List<float>();

        // Calculate the particle indices based on the percentage ranges for ropeA
        int startParticleA = firstParticleA + Mathf.RoundToInt((lastParticleA - firstParticleA) * (ropeAStartPercentage / 100f));
        int endParticleA = firstParticleA + Mathf.RoundToInt((lastParticleA - firstParticleA) * (ropeAEndPercentage / 100f));

        // Calculate the particle indices based on the percentage ranges for ropeB
        int startParticleB = firstParticleB + Mathf.RoundToInt((lastParticleB - firstParticleB) * (ropeBStartPercentage / 100f));
        int endParticleB = firstParticleB + Mathf.RoundToInt((lastParticleB - firstParticleB) * (ropeBEndPercentage / 100f));

        // Iterate over particles within the specified ranges for both ropes
        for (int i = startParticleA; i <= endParticleA; i++)
        {
            Vector3 posA = solver.positions[i];

            for (int j = startParticleB; j <= endParticleB; j++)
            {
                Vector3 posB = solver.positions[j];
                float distance = Vector3.Distance(posA, posB);

                // filterdistances within the threshold
                if (distance <= proximityThreshold)
                {
                    closeDistances.Add(distance);
                }
            }
        }

        // Calculate the mean of close distances
        meanDistance = closeDistances.Count > 0 ? CalculateMean(closeDistances) : 0;
        distanceText.text = $"Mean Distance: {meanDistance:F2}";
    }

    private float CalculateMean(List<float> distances)
    {
        float sum = 0;
        foreach (float distance in distances)
        {
            sum += distance;
        }
        return distances.Count > 0 ? sum / distances.Count : 0;
    }

}
