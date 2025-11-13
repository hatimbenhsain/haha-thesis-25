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

    public static bool IsPlaying(EventInstance instance){
        PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        if(state==PLAYBACK_STATE.STOPPED){
            return false;
        }else{
            return true;
        }
    }

    public static bool IsPlaying(string name){
        PLAYBACK_STATE state;
        var instance=GetInstance(name);
        if(instance.isValid()) return false;
        instance.getPlaybackState(out state);
        if(state==PLAYBACK_STATE.STOPPED){
            return false;
        }else{
            return true;
        }
    }

    public static EventInstance GetInstance(string name){
        EventInstance instance=new EventInstance();
        if(SoundMaker.Instance.eventInstances.ContainsKey(name)){
            instance=SoundMaker.Instance.eventInstances[name];
            Debug.Log("found instance in dictionary");
        }else{
            GameObject go=GameObject.Find(name);
            if(go!=null){
                StudioEventEmitter em;
                Debug.Log("found gameobject");
                if(go.TryGetComponent<StudioEventEmitter>(out em)){
                    Debug.Log("found emitter");
                    instance=em.EventInstance;
                    SoundMaker.Instance.eventInstances[name]=instance;
                }                
            }
        }
        return instance;
    }

    public static void PlayInstance(string path, string name, float volume=1f){
        var instance=GetInstance(name);
        if(!instance.isValid()){
            instance = RuntimeManager.CreateInstance(path);
            SoundMaker.Instance.eventInstances[name]=instance;
        }
        instance.setVolume(volume);
        instance.start();
    }


    public static void SetInstanceParameter(string name, string parameterName,float parameterValue, bool ignoreSeekSpeed=false){
        var instance=GetInstance(name);
        if(instance.isValid()){
            instance=SoundMaker.Instance.eventInstances[name];
            instance.setParameterByName(parameterName, parameterValue, ignoreSeekSpeed);
        }else{
            Debug.LogWarning("Tried to set instance parameters but couldn't find instance.");
        }
    }

    public static void SetInstanceVolume(string name, float volume){
        var instance=GetInstance(name);
        if(instance.isValid()){
            instance=SoundMaker.Instance.eventInstances[name];
            instance.setVolume(volume);
        }else{
            Debug.LogWarning("Tried to set instance parameters but couldn't find instance.");
        }
    }

    public static void StopInstance(string name, bool release=false){
        var instance=GetInstance(name);
        if(instance.isValid()){
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            if(release){
                instance.release();
            }
        }else{
            Debug.LogWarning("Tried to stop instance but couldn't find instance.");
        }
    }

    public static void StopInstance(EventInstance instance, bool release=false){
        if(instance.isValid()){
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            if(release){
                instance.release();
            }
        }
    }


}
