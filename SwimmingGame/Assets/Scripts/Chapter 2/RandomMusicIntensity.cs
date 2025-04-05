using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class RandomMusicIntensity : MonoBehaviour
{
    public StudioEventEmitter emitter;
    private EventInstance instance;

    public float timer;
    public float timeBeforeIntensityChange=10f;

    public float averageTimeBeforeIntensityChange=10f;
    public float timeVariance=5f;

    public float currentIntensity=1f;

    public float minSpeed=-21f;
    public float maxSpeed=-8f;
    void Start()
    {  
        instance=emitter.EventInstance;
    }

    // Update is called once per frame
    void Update()
    {
        timer+=Time.deltaTime;
        if (timer>timeBeforeIntensityChange){
            timer=0f;
            timeBeforeIntensityChange=averageTimeBeforeIntensityChange+Random.Range(-timeVariance,timeVariance);
            int k=Random.Range(1,6);
            while(k==currentIntensity){
                k=Random.Range(1,6);
            }
            currentIntensity=k;
            instance.setParameterByName("Target Loop Index",k);
            instance.setParameterByName("Speed",Random.Range(minSpeed,maxSpeed));
        }
    }
}
