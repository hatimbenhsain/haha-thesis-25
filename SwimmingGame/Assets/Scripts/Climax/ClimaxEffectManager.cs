using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class ClimaxEffectManager : MonoBehaviour
{
    [Header("Inputs")]
    public Volume globalVolume;
    public Light directionalLight;
    public Camera mainCamera;
    public RubbingGameManager rubbingGameManager;
    public SexMaterialManager sexMaterialManager; // Reference to the material manager
    public List<BulgeEffect> bulgeEffects;

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
    public Color[] targetStartColors;

    [Header("Parameters")]
    [Tooltip("Universal lerp speed for every changing thing.")]
    public float lerpSpeed;
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

        originalExcitementLevels = new List<float>();

        // Save original excitement levels from the material manager
        if (sexMaterialManager != null && sexMaterialManager.excitementLevels.Count > 0)
        {
            foreach (var level in sexMaterialManager.excitementLevels)
            {
                originalExcitementLevels.Add(level);
            }
        }
    }

    void Update()
    {

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
        Color targetStartColor = targetStartColors[Random.Range(0, targetStartColors.Length)];
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
            bloom.intensity.value = Mathf.Lerp(bloom.intensity.value, Mathf.Lerp(originalBloomIntensity, maxBloomIntensity, normalizedBloomIntensity), lerpSpeed * Time.deltaTime);
        }
        else
        {
            // Increase bloom intensity slowly below threshold
            float slowIncrease = Mathf.InverseLerp(0f, speedMeterThreshold, speedMeter);
            bloom.intensity.value = Mathf.Lerp(bloom.intensity.value, Mathf.Lerp(originalBloomIntensity, maxBloomIntensity, slowIncrease), lerpSpeed * Time.deltaTime);
        }
    }

    private void HandleDirectionalLight()
    {
        float normalizedDistance = Mathf.InverseLerp(0f, 100f, distanceMeter);
        directionalLight.intensity = Mathf.Lerp(directionalLight.intensity, Mathf.Lerp(directionalLightMinIntensity, directionalLightMaxIntensity, normalizedDistance), lerpSpeed * Time.deltaTime);
    }

    private void HandleFogColor()
    {
        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, fogColorList[currentIntensity], lerpSpeed * Time.deltaTime);
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
