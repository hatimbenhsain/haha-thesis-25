using Obi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulgeEffects: MonoBehaviour
{
    public SpringController springController;
    public float bulgeThickness = 0.06f;
    public float baseThickness = 0.04f;
    public float inhaleLerpSpeed;
    public float exhaleLerpSpeed;

    private ObiRope rope;
    public ObiPathSmoother smoother;
    private float currentThickness;

    void OnEnable()
    {
        rope = GetComponent<ObiRope>();
        smoother = GetComponent<ObiPathSmoother>();
        rope.OnSimulationStart += Rope_OnBeginStep;
    }
    void OnDisable()
    {
        rope.OnSimulationStart -= Rope_OnBeginStep;
    }


    void Start()
    {
        springController = springController != null ? springController : FindObjectOfType<SpringController>();
        currentThickness = baseThickness; // Initialize the current thickness to the base thickness
    }

    private void Rope_OnBeginStep(ObiActor actor, float stepTime, float substepTime)
    {
        int startParticle = rope.elements[rope.elements.Count - 2].particle2;

        // Lerp thickness based on inhale/exhale state
        if (springController.isInhaling)
        {
            currentThickness = Mathf.Lerp(currentThickness, bulgeThickness, Time.deltaTime * inhaleLerpSpeed);
            rope.solver.principalRadii[startParticle] = Vector3.one * currentThickness;
            Debug.Log($"Inhaling: Thickness={rope.solver.principalRadii[startParticle]}");
        }
        else
        {
            currentThickness = Mathf.Lerp(currentThickness, baseThickness, Time.deltaTime * exhaleLerpSpeed);
            rope.solver.principalRadii[startParticle] = Vector3.one * currentThickness;
            // Debug.Log($"Exhaling: Thickness={currentThickness}");
        }
    }
}