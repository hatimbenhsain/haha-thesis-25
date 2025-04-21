using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DifferentLightingArea : MonoBehaviour
{
    [Tooltip("If true, these settings continue applying when leaving, until reach a different area.")]
    public bool permanent=false;
    public Light directionalLight;
    private float directionalLightBaseIntensity;
    public float directionalLightInsideIntensity;
    public float lerpSpeed=1f;

    private float environmentalLightBaseIntensity;
    public float environmentalLightInsideIntensity=-1f;

    public float bloomTresholdInside=-1f;

    public Color cameraBGTargetColor;
    public float cameraTargetClippingPlane=-1f;
    public Color fogTargetColor;
    public float fogTargetDensity=-1f;

    public bool inside=false;

    public bool active=false;

    private VolumeProfile profile;
    private Bloom  bloom;
    private float initialBloomTreshold;

    void Start()
    {
        if(directionalLight!=null) directionalLightBaseIntensity=directionalLight.intensity;
        environmentalLightBaseIntensity=RenderSettings.ambientIntensity;
        profile=FindObjectOfType<Volume>().sharedProfile;
        profile.TryGet<Bloom>(out bloom);
        if(bloomTresholdInside!=-1){
            initialBloomTreshold=bloom.threshold.value;
        }
    }

    void Update()
    {
        if(active){
            float directionalLightTargetIntensity=directionalLightBaseIntensity;
            float environmentalLightTargetIntensity=environmentalLightBaseIntensity;
            float bloomTargetTreshold=bloomTresholdInside;
            if(inside){
                if(directionalLightInsideIntensity!=-1f) directionalLightTargetIntensity=directionalLightInsideIntensity;
                if(environmentalLightInsideIntensity!=-1f) environmentalLightTargetIntensity=environmentalLightInsideIntensity;
                if(bloomTresholdInside!=-1) bloomTargetTreshold=bloomTresholdInside; 
            }
            if(directionalLight!=null) directionalLight.intensity=Mathf.Lerp(directionalLight.intensity,directionalLightTargetIntensity,
                lerpSpeed*Time.deltaTime);
            RenderSettings.ambientIntensity=Mathf.Lerp(RenderSettings.ambientIntensity,environmentalLightTargetIntensity,
                lerpSpeed*Time.deltaTime);
            if(bloom!=null){
                bloom.threshold.value=Mathf.Lerp(bloom.threshold.value,bloomTargetTreshold,
                lerpSpeed*Time.deltaTime);
            }
            if(cameraBGTargetColor.a>0f){
                Camera.main.backgroundColor=Color.Lerp(Camera.main.backgroundColor,cameraBGTargetColor,lerpSpeed*Time.deltaTime);
            }
            if(fogTargetColor.a>0f){
                RenderSettings.fogColor=Color.Lerp(RenderSettings.fogColor,fogTargetColor,lerpSpeed*Time.deltaTime);
            }
            if(fogTargetDensity!=-1f){
                RenderSettings.fogDensity=Mathf.Lerp(RenderSettings.fogDensity,fogTargetDensity,lerpSpeed*Time.deltaTime);
            }
            if(cameraTargetClippingPlane!=-1f){
                Camera.main.farClipPlane=Mathf.Lerp(Camera.main.farClipPlane,cameraTargetClippingPlane,lerpSpeed*Time.deltaTime);
            }
        }
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag=="Player"){
            inside=true;
            DeactivateOtherAreas();
        }
        
    }

    void OnTriggerExit(Collider other){
        if(other.gameObject.tag=="Player"){
            inside=false;
            DeactivateOtherAreas(true);
        }
        if(permanent){
            active=true;
        }
    }

    void DeactivateOtherAreas(bool exiting=false){
        foreach(DifferentLightingArea differentLightingArea in FindObjectsOfType<DifferentLightingArea>()){
            if(exiting && differentLightingArea!=this && differentLightingArea.inside && differentLightingArea.active){
                return;
            }
            differentLightingArea.active=false;
        }
        active=true;
    }

    void OnDestroy(){
        if(bloom!=null){
            bloom.threshold.value=initialBloomTreshold;
        }
    }
}
