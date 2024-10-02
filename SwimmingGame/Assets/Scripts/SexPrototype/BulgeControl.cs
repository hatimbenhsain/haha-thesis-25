using Obi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulgeControl : MonoBehaviour
{
    public SpringController springController; // reference to the SpringController for inhale/exhale states
    public float pumpSpeed = 5f;
    public float baseThickness = 0.04f;
    public float bulgeThickness = 0.06f;
    public float bulgeMoveSpeed = 10f; // speed at which the bulge moves along the rope
    public float endThinness = 0.02f;  // thickness of the rope at the end (snake shape)

    private ObiRope rope;
    private bool isExhaling = false;
    private bool isInhaling = false;
    private bool bulgePulsed = false; // track if the bulge has pulsed after inhale
    private float distance = 0;
    private bool pulseTriggered = false; // track if the pulse has been triggered during exhale

    void Start()
    {
        rope = GetComponent<ObiRope>();
        springController = springController != null ? springController : FindObjectOfType<SpringController>();
    }

    void Update()
    {
        // Sync with SpringController inhale/exhale process
        if (springController.isInhaling && !isInhaling)
        {
            isInhaling = true;
            isExhaling = false;
            distance = rope.restLength; // set the distance to the end of the rope
            bulgePulsed = false; // reset bulge pulse state
            pulseTriggered = false; // reset pulse trigger state
            Debug.Log("Inhale started");
        }
        else if (!springController.isInhaling && !isExhaling && !bulgePulsed)
        {
            isExhaling = true;
            isInhaling = false;
            distance = rope.restLength; // set the bulge to start at the end of the rope
            Debug.Log("Exhale started");
        }
    }

    void FixedUpdate()
    {
        if (isInhaling)
        {
            SimulateInhaleBulge();
        }
        else if (isExhaling)
        {
            SimulateExhaleBulge();
        }

        // Continuously taper the rope to give it a snake-like shape
        TaperRope();
    }

    void SimulateInhaleBulge()
    {
        // Set a constant bulge at the start of the rope during inhale
        int startIndex = rope.elements[rope.elements.Count - 1].particle2;;
        rope.solver.principalRadii[startIndex] = Vector3.one * bulgeThickness;

        Debug.Log($"Inhaling: Bulge={bulgeThickness}");
    }

    void SimulateExhaleBulge()
    {
        // Move the bulge from the end of the rope to the start during exhale
        if (!pulseTriggered)
        {
            pulseTriggered = true; // mark that the pulse has been triggered
            Debug.Log("Pulse triggered");
        }

        // Move the bulge toward the start
        distance -= Time.deltaTime * bulgeMoveSpeed;

        // Iterate over the particles in reverse order, creating the bulge
        for (int i = rope.solverIndices.count - 1; i >= 0; --i)
        {
            int solverIndex = rope.solverIndices[i];
            float bulgePos = distance - i * 0.1f; // Move bulge along the rope toward the start
            float bulge = Mathf.Lerp(baseThickness, bulgeThickness, Mathf.Clamp01(1 - (bulgePos / distance))); // control bulge based on position

            rope.solver.principalRadii[solverIndex] = Vector3.one * bulge;

            // If bulge reaches the start of the rope, stop pulsing
            if (i == 0 && bulge <= baseThickness)
            {
                isExhaling = false;
                bulgePulsed = true;
                Debug.Log("Exhale complete");
            }
        }

        Debug.Log($"Exhaling: Distance={distance}");
    }

    void TaperRope()
    {
        // Gradually decrease the thickness toward the end of the rope to give a snake-like appearance
        for (int i = 0; i < rope.solverIndices.count; ++i)
        {
            int solverIndex = rope.solverIndices[i];
            float taperFactor = Mathf.Lerp(endThinness, baseThickness, (float)i / rope.solverIndices.count); // Taper thickness
            rope.solver.principalRadii[solverIndex] = new Vector3(taperFactor, taperFactor, taperFactor);
        }
    }
}