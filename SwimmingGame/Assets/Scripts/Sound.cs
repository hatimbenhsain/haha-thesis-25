using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public float masterVolume=1f;

    public static void PlayOneShotVolume(string path, float volume, string parameterName="",float parameterValue=0f, float pitch=-100f)
    {
        var instance = RuntimeManager.CreateInstance(path);
        instance.setVolume(volume);
        if(pitch!=-100f){
            instance.setPitch(pitch);
        }
        if(parameterName!=""){
            instance.setParameterByName(parameterName, parameterValue);
        }
        instance.start();
        instance.release();
    }

    public static void Play3DOneShotVolume(string path, float volume, Transform t, string parameterName="",float parameterValue=0f, float pitch=-100f)
    {
        var instance = RuntimeManager.CreateInstance(path);
        RuntimeManager.AttachInstanceToGameObject(instance,t);
        instance.setVolume(volume);
        if(pitch!=-100f){
            instance.setPitch(pitch);
        }
        if(parameterName!=""){
            instance.setParameterByName(parameterName, parameterValue);
        }
        instance.start();
        instance.release();
    }

    

    public bool IsPlaying(EventInstance instance){
        PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        if(state==PLAYBACK_STATE.STOPPED){
            return false;
        }else{
            return true;
        }
    }
}
