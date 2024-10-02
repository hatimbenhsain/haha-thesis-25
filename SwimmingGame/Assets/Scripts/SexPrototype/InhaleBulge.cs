using Obi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InhaleBulge : MonoBehaviour
{
    public SpringController springController; // reference to the SpringController for inhale/exhale states
    public float bulgeThickness = 0.06f;
    public float baseThickness = 0.04f;

    private ObiRope rope;
    private bool isInhaling = false;

    void Start()
    {
        rope = GetComponent<ObiRope>();
        springController = springController != null ? springController : FindObjectOfType<SpringController>();
    }

    void Update()
    {
        // Sync with SpringController inhale state
        if (springController.isInhaling && !isInhaling)
        {
            isInhaling = true;
            CreateInhaleBulge();
            Debug.Log("Inhale started");
        }
        else if (!springController.isInhaling && isInhaling)
        {
            isInhaling = false;
            //ResetRope();
            Debug.Log("Inhale stopped");
        }
    }

    void CreateInhaleBulge()
    {
        // Set the bulge at the start of the rope
        if (rope.elements.Count > 0)
        {
            int startParticle = rope.elements[rope.elements.Count - 1].particle2;
            rope.solver.principalRadii[startParticle] = Vector3.one * bulgeThickness;
            Debug.Log($"Bulging start of the rope: Thickness={bulgeThickness}");
        }
    }

    void ResetRope()
    {
        // Reset the rope thickness back to the base
        foreach (var element in rope.elements)
        {
            int particle = element.particle1;
            rope.solver.principalRadii[particle] = Vector3.one * baseThickness;
        }
        Debug.Log($"Rope reset: Thickness={baseThickness}");
    }
}