using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PartyMusic : MonoBehaviour
{
    private Dialogue dialogue;
    private EventInstance instance;
    void Start()
    {
        instance=GetComponent<StudioEventEmitter>().EventInstance;
        dialogue=FindObjectOfType<Dialogue>();
    }

    void Update()
    {
        if(dialogue.inDialogue){
            instance.setParameterByName("Music Intensity",1f);
        }else{
            instance.setParameterByName("Music Intensity",0f);
        }
    } 

    public bool IsMuffled(){
        float m;
        instance.getParameterByName("Muffled",out m);
        if(m==0f){
            return false;
        }else{
            return true;
        }
    }
}
