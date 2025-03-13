using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class FMODParameterTrigger : MonoBehaviour
{
    public string targetTag="Player";
    public StudioEventEmitter emitter;
    private EventInstance instance;

    public FMODParameter[] onEnterParameters;
    public FMODParameter[] onExitParameters;
    void Start()
    {
        instance=emitter.EventInstance;
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag==targetTag){
            foreach(FMODParameter parameter in onEnterParameters){
                if(parameter.isGlobal){
                    if(parameter.stringValue!=""){
                        RuntimeManager.StudioSystem.setParameterByNameWithLabel(parameter.name,parameter.stringValue);
                    }else{
                        RuntimeManager.StudioSystem.setParameterByName(parameter.name,parameter.floatValue);
                    }
                }else{
                    if(parameter.stringValue!=""){
                        instance.setParameterByNameWithLabel(parameter.name,parameter.stringValue);
                    }else{
                        instance.setParameterByName(parameter.name,parameter.floatValue);
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other){
        if(other.gameObject.tag==targetTag){
            foreach(FMODParameter parameter in onExitParameters){
                if(parameter.isGlobal){
                    if(parameter.stringValue!=""){
                        RuntimeManager.StudioSystem.setParameterByNameWithLabel(parameter.name,parameter.stringValue);
                    }else{
                        RuntimeManager.StudioSystem.setParameterByName(parameter.name,parameter.floatValue);
                    }
                }else{
                    if(parameter.stringValue!=""){
                        instance.setParameterByNameWithLabel(parameter.name,parameter.stringValue);
                    }else{
                        instance.setParameterByName(parameter.name,parameter.floatValue);
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class FMODParameter{
    public string name;
    public float floatValue;
    public string stringValue;
    public bool isGlobal;
}