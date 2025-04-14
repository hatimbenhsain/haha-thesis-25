using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;

public class KnotGoCrazyEffect : MonoBehaviour
{
    public GameObject collisionCylinder1;
    public GameObject collisionCylinder2;
    public ObiRope rope1;
    public ObiRope rope2;
    public IntroHeads introHeads1;
    public IntroHeads introHeads2;
    public bool testGoCrazy;

    // Start is called before the first frame update
    void Update()
    {
        if (testGoCrazy)
        {
            GoCrazy();
            testGoCrazy = false; // Reset the test variable to prevent multiple calls
        }
    }
    void GoCrazy()
    {
        // Deactivate collision cylinders
        if (collisionCylinder1 != null) collisionCylinder1.SetActive(false);
        if (collisionCylinder2 != null) collisionCylinder2.SetActive(false);

        // Start lerping the stretching scale of both ropes
        StartCoroutine(LerpRopeStretch(rope1, rope2, 1f, 1.2f, 2f)); // 2 seconds duration

        // Set the goCrazy variable of both IntroHeads to true
        if (introHeads1 != null) introHeads1.goCrazy = true;
        if (introHeads2 != null) introHeads2.goCrazy = true;
    }

    private IEnumerator LerpRopeStretch(ObiRope rope1, ObiRope rope2, float startScale, float endScale, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            // Lerp the stretching scale of both ropes
            float currentScale = Mathf.Lerp(startScale, endScale, t);
            if (rope1 != null) rope1.stretchingScale = currentScale;
            if (rope2 != null) rope2.stretchingScale = currentScale;

            yield return null;
        }

        // Ensure the final stretching scale is set
        if (rope1 != null) rope1.stretchingScale = endScale;
        if (rope2 != null) rope2.stretchingScale = endScale;
    }
}
