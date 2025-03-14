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
            Debug.Log("intensity 2");
        }else{
            instance.setParameterByName("Music Intensity",0f);
            Debug.Log("intensity 1");
        }
    } 
}
