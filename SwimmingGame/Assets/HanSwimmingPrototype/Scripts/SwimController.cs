using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimController : MonoBehaviour
{
    public GameObject character; // cylinder representing the character

    public float maxInhaleTime = 3f; // time to fully inhale
    public float maxExhaleTime = 1f; // maximum exhale time
    public float minExhaleTime = 0f; // minimum exhale time

    private Vector3 originalScale; // original scale of the cylinder
    private bool isInhaling = false;
    private float inhaleStartTime = 0f;
    private float inhaleDuration = 0f;

    void Start()
    {
        originalScale = character.transform.localScale; // store the original scale
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartInhaling();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            StartExhaling();
        }

        if (isInhaling)
        {
            Inhale();
        }
    }

    // starts the inhaling process
    void StartInhaling()
    {
        isInhaling = true;
        inhaleStartTime = Time.time;
    }

    // inhale lerping scale
    void Inhale()
    {
        float inhaleTime = Time.time - inhaleStartTime;
        inhaleDuration = Mathf.Clamp(inhaleTime, 0, maxInhaleTime);

        float t = inhaleDuration / maxInhaleTime;
        character.transform.localScale = Vector3.Lerp(originalScale, new Vector3(1.2f, 1.2f, 0.8f), t);
    }

    // starts the exhaling process
    void StartExhaling()
    {
        isInhaling = false;

        float heldInhaleTime = Time.time - inhaleStartTime;
        float exhaleTime = Mathf.Clamp(heldInhaleTime / maxInhaleTime, minExhaleTime, maxExhaleTime);

        StartCoroutine(Exhale(exhaleTime));
    }

    // exhale lerping scale
    System.Collections.IEnumerator Exhale(float exhaleTime)
    {
        Vector3 currentScale = character.transform.localScale;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / exhaleTime;
            character.transform.localScale = Vector3.Lerp(currentScale, originalScale, t);
            yield return null;
        }
    }
}