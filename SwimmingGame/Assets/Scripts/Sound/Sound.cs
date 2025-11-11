using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public float masterVolume=1f;

    public static Dictionary<string, EventInstance> eventInstances;

    void Awake()
    {
        if(eventInstances==null) eventInstances=new Dictionary<string, EventInstance>();
    }



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
        if(eventInstances.ContainsKey(name)){
            instance=eventInstances[name];
        }else{
            GameObject go=GameObject.Find(name);
            if(go!=null){
                if(go.TryGetComponent<EventInstance>(out instance)){
                    eventInstances[name]=instance;
                }                
            }
        }
        return instance;
    }

    public static void PlayInstance(string path, string name, float volume=1f){
        var instance=GetInstance(name);
        if(instance.isValid()){
            instance = RuntimeManager.CreateInstance(path);
            eventInstances[name]=instance;
        }
        instance.setVolume(volume);
        instance.start();
    }


    public static void SetInstanceParameter(string name, string parameterName,float parameterValue){
        var instance=GetInstance(name);
        if(instance.isValid()){
            instance=eventInstances[name];
            instance.setParameterByName(parameterName, parameterValue);
        }else{
            Debug.LogWarning("Tried to set instance parameters but couldn't find instance.");
        }
    }

    public static void StopAndReleaseAll(){
        foreach(KeyValuePair<string,EventInstance> kvp  in eventInstances){
            kvp.Value.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            kvp.Value.release();
        }
    }
}
