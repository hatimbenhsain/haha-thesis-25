using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is for starting dialogue when entering the trigger
public class DialogueTriggerZone : MonoBehaviour
{
    public string targetTag="Player";
    public bool destroyAfterUse=true;

    [Tooltip("Name of dialogue knot to start with.")]
    public string knotName="";
    [Tooltip("Text asset to pull dialogue from. Default takes it from room.")]
    public TextAsset inkJSONAsset=null;
    public bool forceDialogue=false;

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag==targetTag){
            Dialogue d=FindObjectOfType<Dialogue>();
            if(forceDialogue){
                d.StartDialogue(inkJSONAsset,knotName);
            }else{
                d.TryStartDialogue(inkJSONAsset,knotName);
            }
            if(destroyAfterUse) Destroy(gameObject);
        }
    }
}