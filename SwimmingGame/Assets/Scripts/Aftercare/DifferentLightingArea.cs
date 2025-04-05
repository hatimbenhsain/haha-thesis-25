using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class DifferentLightingArea : MonoBehaviour
{
    public Light directionalLight;
    private float directionalLightBaseIntensity;
    public float directionalLightInsideIntensity;
    public float lerpSpeed=1f;

    private float environmentalLightBaseIntensity;
    public float environmentalLightInsideIntensity=-1f;

    private bool inside=false;

    public bool active=false;

    void Start()
    {
        directionalLightBaseIntensity=directionalLight.intensity;
        environmentalLightBaseIntensity=RenderSettings.ambientIntensity;
    }

    void Update()
    {
        if(active){
            float directionalLightTargetIntensity=directionalLightBaseIntensity;
            float environmentalLightTargetIntensity=environmentalLightBaseIntensity;
            if(inside){
                if(directionalLightInsideIntensity!=-1f) directionalLightTargetIntensity=directionalLightInsideIntensity;
                if(environmentalLightInsideIntensity!=-1f) environmentalLightTargetIntensity=environmentalLightInsideIntensity;
            }
            directionalLight.intensity=Mathf.Lerp(directionalLight.intensity,directionalLightTargetIntensity,
                lerpSpeed*Time.deltaTime);
            RenderSettings.ambientIntensity=Mathf.Lerp(RenderSettings.ambientIntensity,environmentalLightTargetIntensity,
                lerpSpeed*Time.deltaTime);
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
            DeactivateOtherAreas();
        }
    }

    void DeactivateOtherAreas(){
        foreach(DifferentLightingArea differentLightingArea in FindObjectsOfType<DifferentLightingArea>()){
            differentLightingArea.active=false;
        }
        active=true;
    }
}
