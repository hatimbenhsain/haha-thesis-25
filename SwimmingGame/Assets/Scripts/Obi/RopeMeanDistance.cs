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



    void Update()
    {
        List<float> closeDistances = new List<float>();

        // Iterate over particles in both ropes
        foreach (int indexA in ropeA.solverIndices)
        {
            Vector3 posA = solver.positions[indexA];

            foreach (int indexB in ropeB.solverIndices)
            {
                Vector3 posB = solver.positions[indexB];
                float distance = Vector3.Distance(posA, posB);

                // Only consider distances within the threshold
                if (distance <= proximityThreshold)
                {
                    closeDistances.Add(distance);
                }
            }
        }

        // Calculate the mean of close distances if any are found
        meanDistance = closeDistances.Count > 0 ? CalculateMean(closeDistances) : 0;

        // Display the mean distance in the TextMeshPro text
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
