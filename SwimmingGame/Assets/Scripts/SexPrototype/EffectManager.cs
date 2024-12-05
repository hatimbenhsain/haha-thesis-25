using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic; 

public class EffectManager : MonoBehaviour
{
    [Header("Inputs")]
    public Volume globalVolume;
    public Light directionalLight;
    public Camera mainCamera;
    public List<Material> materials; 
    public List<BulgeEffect> bulgeEffects; 

    [Header("Lighting")]
    public bool testModeOn; 
    public Color fogColorTest; 
    public Color directionalLightColorTest; 

    [Header("Global Volumes")]
    public FloatParameter exposure;
    public float contrast; 
    public float saturation; 
    public float bloomThreshold; 
    public float bloomIntensity; 

    private Color originalFogColor;

    private ColorAdjustments colorAdjustments;
    private Bloom bloom;

    void Start()
    {
        // Save original fog color
        if (RenderSettings.fog)
            originalFogColor = RenderSettings.fogColor;

        // Extract components from the global volume
        if (globalVolume.profile.TryGet(out colorAdjustments) &&
            globalVolume.profile.TryGet(out bloom))
        {
            Debug.Log("Post-processing components found!");
        }
    }

    void Update()
    {
        if (testModeOn)
        {
            // Override fog and light colors
            RenderSettings.fogColor = fogColorTest;
            directionalLight.color = directionalLightColorTest;

            // Adjust post-processing values
            if (colorAdjustments != null)
            {
                colorAdjustments.postExposure = exposure;
                colorAdjustments.contrast.value = contrast;
                colorAdjustments.saturation.value = saturation;
            }
            if (bloom != null)
            {
                bloom.threshold.value = bloomThreshold;
                bloom.intensity.value = bloomIntensity;
            }
        }
    }

    public void TriggerAllBulgeEffects()
    {
        foreach (var bulgeEffect in bulgeEffects)
        {
            if (bulgeEffect != null)
            {
                bulgeEffect.triggerPulse = true;
            }
        }
    }
}
