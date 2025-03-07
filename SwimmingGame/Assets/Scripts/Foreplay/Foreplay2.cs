using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Foreplay2 : MonoBehaviour
{
    public int harmonyCounter=0;
    public int harmonyNumberToReach=10;

    public float cooldownTimer=0f;
    [Tooltip("Do not count harmonies during this time")]
    public float cooldownTime=2f;


    [Header("Effects")]
    public float lerpSpeed=2f;

    public Volume volume;
    private ColorAdjustments colorAdjustments;
    private Bloom bloom;
    private LensDistortion lensDistortion;

    private float lensDistortionInitialIntensity;
    public float lensDistortionTargetIntensity;
    private float lensDistortionInitialScale;
    public float lensDistortionTargetScale;

    public Color fogTargetColor;
    private Color fogInitalColor;



    void Start()
    {
        fogInitalColor=RenderSettings.fogColor;
        volume.profile.TryGet<LensDistortion>(out lensDistortion);
        lensDistortionInitialIntensity=lensDistortion.intensity.value;
        lensDistortionInitialScale=lensDistortion.scale.value;
    }

    void Update()
    {
        NPCSinging[] singers=FindObjectsOfType<NPCSinging>();
        cooldownTimer+=Time.deltaTime;
        if(cooldownTimer>cooldownTime){
            foreach(NPCSinging singer in singers){
                if(singer.gameObject.activeInHierarchy && singer.HasHarmonized()){ 
                    harmonyCounter++;
                    if(harmonyCounter==harmonyNumberToReach){
                        FindObjectOfType<LevelLoader>().LoadLevel();
                    }
                    cooldownTimer=0f;
                    break;
                }
            }
        }

        //Effects
        float progress=(float)harmonyCounter/(float)harmonyNumberToReach;
        Color currentFogTargetColor=Color.Lerp(fogInitalColor,fogTargetColor,Mathf.Pow(progress,.25f));
        RenderSettings.fogColor=Color.Lerp(RenderSettings.fogColor,currentFogTargetColor,lerpSpeed*Time.deltaTime);

        float currentLDTargetScale=Mathf.Lerp(lensDistortionInitialScale,lensDistortionTargetScale,Mathf.Pow(progress,4f));
        float currentLDTargetIntensity=Mathf.Lerp(lensDistortionInitialIntensity,lensDistortionTargetIntensity,Mathf.Pow(progress,4f));
        lensDistortion.scale.value=Mathf.Lerp(lensDistortion.scale.value,currentLDTargetScale,lerpSpeed*Time.deltaTime);
        lensDistortion.intensity.value=Mathf.Lerp(lensDistortion.intensity.value,currentLDTargetIntensity,lerpSpeed*Time.deltaTime);
    }
}
