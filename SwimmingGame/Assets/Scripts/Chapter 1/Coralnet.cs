using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VLB;

public class Coralnet : Metamorphosis
{
    public NPCOverworld npcOverworld;
    public Light light;

    public EffectPulse effectPulse;
    // public MinMaxRangeFloat targetIntensityRange;
    private MinMaxRangeFloat originalIntensityRange;
    public float targetIntensity;
    public float lerpValue;
    private float originalIntensity;
    


    void Start()
    {
        originalIntensityRange = effectPulse.intensityAmplitude;
    }

    void Update()
    {
        if (!metamorphosing)
        {
            if (Dialogue.Instance.npcInterlocutor == npcOverworld)
            {
                MinMaxRangeFloat mmrf = new MinMaxRangeFloat(Mathf.Lerp(effectPulse.intensityAmplitude.minValue, targetIntensity, Time.deltaTime * lerpValue), Mathf.Lerp(effectPulse.intensityAmplitude.maxValue, targetIntensity, Time.deltaTime * lerpValue));
                effectPulse.intensityAmplitude = mmrf;
                // if (!metamorphosing)
                // {
                //     if (light.TryGetComponent<EffectPulse>(out effectPulse)) effectPulse.enabled = false;
                // }
                //light.intensity = Mathf.Lerp(light.intensity, targetIntensity, Time.deltaTime * lerpValue);
            }
            else
            {
                if (effectPulse.enabled)
                {
                    MinMaxRangeFloat mmrf = new MinMaxRangeFloat(Mathf.Lerp(effectPulse.intensityAmplitude.minValue, originalIntensityRange.minValue, Time.deltaTime * lerpValue), Mathf.Lerp(effectPulse.intensityAmplitude.maxValue, originalIntensityRange.maxValue, Time.deltaTime * lerpValue));
                    effectPulse.intensityAmplitude = mmrf;
                }
                // if (effectPulse != null)
                // {
                //     effectPulse.enabled = true;
                // }
            }
        }
    }

}
