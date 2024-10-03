using Obi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InhaleBulge : MonoBehaviour
{
    public SpringController springController; 
    public float bulgeThickness = 0.06f;
    public float baseThickness = 0.04f;
    public float lerpSpeed = 2f; 

    private ObiRope rope;
    private bool isInhaling = false;
    private float currentThickness;

    void Start()
    {
        rope = GetComponent<ObiRope>();
        springController = springController != null ? springController : FindObjectOfType<SpringController>();
        currentThickness = baseThickness; // Initialize the current thickness to the base thickness
    }

    void Update()
    {
        // Sync with SpringController inhale state
        if (springController.isInhaling && !isInhaling)
        {
            isInhaling = true;
            Debug.Log("Inhale started");
        }
        else if (!springController.isInhaling && isInhaling)
        {
            isInhaling = false;
            Debug.Log("Exhale started");
        }

        // Lerp thickness based on inhale/exhale state
        if (isInhaling)
        {
            LerpToBulgeThickness();
        }
        else
        {
            LerpToBaseThickness();
        }
    }

    void LerpToBulgeThickness()
    {
        // increase the thickness towards bulgeThickness
        if (rope.elements.Count > 0)
        {
            // Access the second to last particle
            int startParticle = rope.elements[rope.elements.Count - 1].particle2; 
            currentThickness = Mathf.Lerp(currentThickness, bulgeThickness, Time.deltaTime * lerpSpeed);
            rope.solver.principalRadii[startParticle] = Vector3.one * currentThickness;
            //Debug.Log($"Inhaling: Thickness={currentThickness}");
        }
    }

    void LerpToBaseThickness()
    {
        // decrease the thickness towards baseThickness
        if (rope.elements.Count > 0)
        {
            int startParticle = rope.elements[rope.elements.Count - 1].particle2;
            currentThickness = Mathf.Lerp(currentThickness, baseThickness, Time.deltaTime * lerpSpeed);
            rope.solver.principalRadii[startParticle] = Vector3.one * currentThickness;
            //Debug.Log($"Exhaling: Thickness={currentThickness}");
        }
    }
}