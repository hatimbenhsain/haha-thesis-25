using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceLightBeam : MonoBehaviour
{
    public bool inside=false;
    public float targetIntensity=100f;
    private float startingIntensity;
    private Light light;
    public float lightLerpSpeed=1f;
    public float minDistance=20f;
    public float maxDistance=100f;

    private Transform player;

    void Start()
    {
        light=GetComponent<Light>();
        startingIntensity=light.intensity;
        player=FindObjectOfType<Swimmer>().transform;
    }

    void Update()
    {   
        float currentTargetIntensity;
        float distance=Vector3.Distance(transform.position,player.position);
        if(inside || distance<maxDistance){
            currentTargetIntensity=startingIntensity+(maxDistance-
                Mathf.Clamp(distance,minDistance,maxDistance))*targetIntensity/(maxDistance-minDistance);
        }else{
            currentTargetIntensity=startingIntensity;
        }
        light.intensity=Mathf.Lerp(light.intensity,currentTargetIntensity,lightLerpSpeed*Time.deltaTime);
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
