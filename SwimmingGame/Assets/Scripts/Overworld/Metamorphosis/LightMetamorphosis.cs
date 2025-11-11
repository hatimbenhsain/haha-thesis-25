using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VLB;

public class LightPulseMetamorphosis : Metamorphosis
{
    private Light light;

    public float lerpValue;
    public float targetIntensity;


    void Start()
    {
        
    }

    void Update()
    {
        if (metamorphosing)
        {
            light.intensity = Mathf.Lerp(light.intensity, targetIntensity, Time.deltaTime * lerpValue);
            if (Mathf.Abs(light.intensity - targetIntensity) <= 0.1f)
            {
                light.intensity = targetIntensity;
                Destroy(this);
            }
        }
    }
    
    public override void TriggerMetamorphosis()
    {
        base.TriggerMetamorphosis();
        EffectPulse effectPulse;
        if(TryGetComponent<EffectPulse>(out effectPulse)) effectPulse.enabled=false;
        light = GetComponent<Light>();
    }
}
