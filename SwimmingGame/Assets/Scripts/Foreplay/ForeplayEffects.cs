using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ForeplayEffects : MonoBehaviour
{
    public float totalIntensity=0f;
    public float targetIntensity=0f;
    public Volume volume;
    private ColorAdjustments colorAdjustments;
    private Bloom bloom;
    public NPCSequencer npcSequencer;

    public float variance=0.5f;
    public float period=2f;

    public Light light1;
    private float light1TargetIntensity;
    public Light light2;
    private float light2TargetIntensity;

    public float saturationMinValue=-20f;
    public float saturationMaxValue=20f;
    public float bloomMinValue=0f;
    public float bloomMaxValue=10f;
    public float postExposureMinValue=0f;
    public float postExposureMaxValue=10f;

    public float lerpSpeed=2f;

    void Start()
    {
        light1TargetIntensity=light1.intensity;
        light2TargetIntensity=light2.intensity;

        volume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
        volume.profile.TryGet<Bloom>(out bloom);
    }

    void Update()
    {
        NPCSinging npcSinging=npcSequencer.brains[npcSequencer.brainIndex].GetComponent<NPCSinging>();
        targetIntensity=(npcSequencer.brainIndex+npcSinging.harmonyValue/npcSinging.harmonyTargetValue)/npcSequencer.brains.Length;
        totalIntensity=Mathf.Lerp(totalIntensity,targetIntensity,lerpSpeed*Time.deltaTime);

        float value=Mathf.Clamp(totalIntensity+Mathf.Sin(2*Mathf.PI*Time.time/period)*variance,0f,1f);

        light1.intensity=value*1f;
        light2.intensity=value*1f;

        colorAdjustments.saturation.value=saturationMinValue+(saturationMaxValue-saturationMinValue)*value;
        bloom.intensity.value=bloomMinValue+(bloomMaxValue-bloomMinValue)*value;

        float secondaryIntensity=0f;

        if(npcSequencer.brainIndex==npcSequencer.brains.Length-2){
            secondaryIntensity=npcSinging.harmonyValue/npcSinging.harmonyTargetValue;
        }else if(npcSequencer.brainIndex>=npcSequencer.brains.Length-1){
            secondaryIntensity=1f;
        }

        colorAdjustments.postExposure.value=postExposureMinValue+secondaryIntensity*(postExposureMaxValue-postExposureMinValue);
    }
}
