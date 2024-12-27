using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifferentLightingArea : MonoBehaviour
{
    public Light directionalLight;
    private float directionalLightBaseIntensity;
    public float directionalLightInsideIntensity;
    public float directionalLightLerpSpeed;

    private bool inside=false;

    void Start()
    {
        directionalLightBaseIntensity=directionalLight.intensity;
    }

    void Update()
    {
        float directionalLightTargetIntensity;
        if(inside) directionalLightTargetIntensity=directionalLightInsideIntensity;
        else directionalLightTargetIntensity=directionalLightBaseIntensity;
        directionalLight.intensity=Mathf.Lerp(directionalLight.intensity,directionalLightTargetIntensity,
            directionalLightLerpSpeed*Time.deltaTime);
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag=="Player"){
            inside=true;
        }
    }

    void OnTriggerExit(Collider other){
        if(other.gameObject.tag=="Player"){
            inside=false;
        }
    }
}
