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
    public SexMaterialManager sexMaterialManager; // Reference to the material manager
    public List<BulgeEffect> bulgeEffects;
    public NPCSexAI npcSexAI;
    public NPCSpring npcSpring;

    [Header("Global Volumes")]
    public float maxBloomIntensity;
    public float contrast;
    public float saturation;
    public float bloomThreshold;

    [Header("Particles")]
    public ParticleSystem particleSystem;
    public float defaultEmissionRate;
    public float maxEmissionRate;
    public Color defaultStartColor;
    public Color targetStartColor;

    [Header("Parameters")]
    public float entanglementMeterThreshold = 50f;
    public float speedMeterThreshold = 50f;
    public float distanceMeterThreshold = 50f;
    public float directionalLightMinIntensity = 1f;
    public float directionalLightMaxIntensity = 5f;
    public float maxSecondPerPulse;
    public float minSecondPerPulse;
    public List<Color> fogColorList;

    [Header("Debug Values")]
    public float distanceMeter;
    public float entanglementMeter;
    public float speedMeter;
    public int currentIntensity;
    public MovementBehavior movementBehavior;



    private ColorAdjustments colorAdjustments;
    private Bloom bloom;
    private ParticleSystem.EmissionModule particleEmission;
    private ParticleSystem.MainModule particleMain;
    private float previousDistanceMeter;
    private List<float> originalExcitementLevels;
    private float originalBloomIntensity;
    private float pulseTimer = 0f;

    void Start()
    {
        // Extract components from the global volume
        if (globalVolume.profile.TryGet(out colorAdjustments) &&
            globalVolume.profile.TryGet(out bloom))
        {
            Debug.Log("Post-processing components found!");
            originalBloomIntensity = bloom.intensity.value;  // Store original bloom intensity
        }

        // Initialize particle system modules
        if (particleSystem != null)
        {
            particleEmission = particleSystem.emission;
            particleMain = particleSystem.main;
        }

        // Save original excitement levels from the material manager
        if (sexMaterialManager != null && sexMaterialManager.excitementLevels != null)
        {
            foreach (var level in sexMaterialManager.excitementLevels)
            {
                originalExcitementLevels.Add(level);
            }
        }
    }

    void Update()
    {
        // Get the input variables
        if (npcSexAI != null)
        {
            speedMeter = npcSexAI.speedMeter;
            distanceMeter = npcSexAI.distanceMeter;
            entanglementMeter = npcSexAI.entanglementMeter;
        }
        if (npcSpring != null)
        {
            currentIntensity = npcSpring.currentIntensity;
            movementBehavior = npcSpring.movementBehavior;
        }

        // Handle effects
        if (previousDistanceMeter == 0f && distanceMeter > 0f)
        {
            TriggerBulgeEffects();
        }
        previousDistanceMeter = distanceMeter;

        HandleExcitementLevels();
        HandleParticleSystem();
        HandleBloomIntensity();
        HandleDirectionalLight();
        HandleFogColor();
        HandleBulgePulse();
    }

    public void HandleParticleSystem()
    {
        float normalizedSpeed = speedMeter / 100f;
        particleEmission.rateOverTime = Mathf.Lerp(defaultEmissionRate, maxEmissionRate, normalizedSpeed);
        particleMain.startColor = Color.Lerp(defaultStartColor, targetStartColor, normalizedSpeed);
    }

    public void TriggerBulgeEffects()
    {
        foreach (var bulgeEffect in bulgeEffects)
        {
            bulgeEffect.triggerPulse = true;
        }
    }

    private void HandleExcitementLevels()
    {
        for (int i = 0; i < sexMaterialManager.excitementLevels.Count; i++)
        {
            if (entanglementMeter > entanglementMeterThreshold)
            {
                // Normalize excitement level change from threshold to 100
                float normalizedExcitement = Mathf.InverseLerp(entanglementMeterThreshold, 100f, entanglementMeter);
                sexMaterialManager.excitementLevels[i] = Mathf.Lerp(originalExcitementLevels[i], 1f, normalizedExcitement);
            }
            else
            {
                // Increase excitement level slowly below threshold
                float slowIncrease = Mathf.InverseLerp(0f, entanglementMeterThreshold, entanglementMeter);
                sexMaterialManager.excitementLevels[i] = Mathf.Lerp(originalExcitementLevels[i], 1f, slowIncrease);
            }
        }

    }

    private void HandleBloomIntensity()
    {
        if (speedMeter > speedMeterThreshold)
        {
            float normalizedBloomIntensity = Mathf.InverseLerp(speedMeterThreshold, 100f, speedMeter);
            bloom.intensity.value = Mathf.Lerp(originalBloomIntensity, maxBloomIntensity, normalizedBloomIntensity);
        }
        else
        {
            // Increase bloom intensity slowly below threshold
            float slowIncrease = Mathf.InverseLerp(0f, speedMeterThreshold, speedMeter);
            bloom.intensity.value = Mathf.Lerp(originalBloomIntensity, maxBloomIntensity, slowIncrease);
        }
    }

    private void HandleDirectionalLight()
    {
        float normalizedDistance = Mathf.InverseLerp(0f, 100f, distanceMeter);
        directionalLight.intensity = Mathf.Lerp(1, 2, normalizedDistance);
    }

    private void HandleFogColor()
    {
        RenderSettings.fogColor = fogColorList[currentIntensity];
    }

    private void HandleBulgePulse()
    {      
        float normalizedDistance = Mathf.InverseLerp(distanceMeterThreshold, 100f, distanceMeter);
        float pulseInterval = Mathf.Lerp(maxSecondPerPulse, minSecondPerPulse, normalizedDistance); 
        pulseTimer += Time.deltaTime;   
        if (pulseTimer >= pulseInterval)      
        {               
            TriggerBulgeEffects(); 
            pulseTimer = 0f; 
        }
    }
}
