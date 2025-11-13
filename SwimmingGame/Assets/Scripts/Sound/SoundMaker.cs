using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class SoundMaker : Sound
{
    public static SoundMaker Instance { get; private set; }

    public Dictionary<string, EventInstance> eventInstances;

    void Start()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
        eventInstances=new Dictionary<string, EventInstance>();
    }

    void OnDestroy(){
        StopAndReleaseAll();
    }

    public void StopAndReleaseAll(){
        foreach(KeyValuePair<string,EventInstance> kvp  in eventInstances){
            StopInstance(kvp.Value,true);
            eventInstances.Remove(kvp.Key);
        }
    }
}
