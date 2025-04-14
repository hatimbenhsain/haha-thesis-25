using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;
using UnityEngine.Rendering;

public class KnotGoCrazyEffect : MonoBehaviour
{
    public GameObject collisionCylinder1;
    public GameObject collisionCylinder2;
    public ObiRope rope1;
    public ObiRope rope2;
    public IntroHeads introHeads1;
    public IntroHeads introHeads2;
    public bool testGoCrazy;
    private Volume globalVolume;
    public VolumeProfile newProfile; 
    public float postExposureStart = 0f;
    public float postExposureEnd = 2f; 
    public float postExposureLerpDuration = 2f;
    public Material noiseVertexShader1;
    public Material noiseVertexShader2;

    // Variables to store initial shader values
    public float initialAngleOffsetSpeed1;
    public float initialAmplitude1;
    public float initialAngleOffsetSpeed2;
    public float initialAmplitude2;

    void Start()
    {
        globalVolume = FindObjectOfType<Volume>();
        if (noiseVertexShader1 != null)
        {
            noiseVertexShader1.SetFloat("_AngleOffsetSpeed", initialAngleOffsetSpeed1);
            noiseVertexShader1.SetFloat("_Amplitude", initialAmplitude1);
        }

        // Reset shader values for noiseVertexShader2
        if (noiseVertexShader2 != null)
        {
            noiseVertexShader2.SetFloat("_AngleOffsetSpeed", initialAngleOffsetSpeed2);
            noiseVertexShader2.SetFloat("_Amplitude", initialAmplitude2);
        }

    }

    void OnDestroy()
    {
        // Reset shader values for noiseVertexShader1
        if (noiseVertexShader1 != null)
        {
            noiseVertexShader1.SetFloat("_AngleOffsetSpeed", initialAngleOffsetSpeed1);
            noiseVertexShader1.SetFloat("_Amplitude", initialAmplitude1);
        }

        // Reset shader values for noiseVertexShader2
        if (noiseVertexShader2 != null)
        {
            noiseVertexShader2.SetFloat("_AngleOffsetSpeed", initialAngleOffsetSpeed2);
            noiseVertexShader2.SetFloat("_Amplitude", initialAmplitude2);
        }
    }

    // Start is called before the first frame update
    void Update()
    {
        if (testGoCrazy)
        {
            GoCrazy();
            testGoCrazy = false; // Reset the test variable to prevent multiple calls
        }
    }
    public void GoCrazy()
    {
        // Deactivate collision cylinders
        if (collisionCylinder1 != null) collisionCylinder1.SetActive(false);
        if (collisionCylinder2 != null) collisionCylinder2.SetActive(false);

        // Switch the global volume profile
        if (globalVolume != null && newProfile != null)
        {
            globalVolume.profile = newProfile;
        }

        // Start lerping the post-exposure value
        StartCoroutine(LerpPostExposure(postExposureStart, postExposureEnd, postExposureLerpDuration));

        // Start lerping the stretching scale of both ropes
        StartCoroutine(LerpRopeStretch(rope1, rope2, 1f, 1.2f, 2f)); // 2 seconds duration

        // Set the goCrazy variable of both IntroHeads to true
        if (introHeads1 != null) introHeads1.goCrazy = true;
        if (introHeads2 != null) introHeads2.goCrazy = true;

        // Modify shader properties
        if (noiseVertexShader1 != null)
        {
            noiseVertexShader1.SetFloat("_AngleOffsetSpeed", 5f);
            noiseVertexShader1.SetFloat("_Amplitude", 2f);
        }

        if (noiseVertexShader2 != null)
        {
            noiseVertexShader2.SetFloat("_AngleOffsetSpeed", 8f);
            noiseVertexShader2.SetFloat("_Amplitude", 0.07f);
        }
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

    private IEnumerator LerpPostExposure(float startValue, float endValue, float duration)
    {

        if (!globalVolume.profile.TryGet(out UnityEngine.Rendering.Universal.ColorAdjustments colorAdjustments))
        {
            Debug.LogError("ColorAdjustments component not found in the volume profile!");
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            // Lerp the post-exposure value
            colorAdjustments.postExposure.value = Mathf.Lerp(startValue, endValue, t);

            yield return null;
        }

        // Ensure the final post-exposure value is set
        colorAdjustments.postExposure.value = endValue;
    }
}
