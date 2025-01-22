using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SwimmerSound : Sound
{
    [Tooltip("Past this speed stop increasing ambient swimming sound volume.")]
    public float maxSwimmingSpeed=3f;

    private EventInstance ambientSwimmingInstance;

    void Start()
    {
        ambientSwimmingInstance=FMODUnity.RuntimeManager.CreateInstance("event:/Swimming/AmbientSwimming");
    }

    void Update()
    {
        
    }

    public void Stride(){
        PlayOneShotVolume("event:/Swimming/Stride",masterVolume);
    }

    public void Kick(){
        PlayOneShotVolume("event:/Swimming/Kick",masterVolume);
    }

    public void Dash(){
        PlayOneShotVolume("event:/Swimming/Dash",masterVolume);
    }


    public void StartSwimming(float speed){
        if(!IsPlaying(ambientSwimmingInstance)){
            ambientSwimmingInstance.start();
        }
        float volume=Mathf.Clamp(speed/maxSwimmingSpeed,0f,1f);
        ambientSwimmingInstance.setParameterByName("swimmingVolume",volume*masterVolume);
    }

    public void StopSwimming(){
        ambientSwimmingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

}
