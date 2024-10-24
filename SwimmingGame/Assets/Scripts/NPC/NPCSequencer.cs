using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

//This script is for sequencing between different brains for the NPC; for example one where they sing A B A and swim slow, then one where the sing C# D# and swim very fast, etc.
//Theoretically we'll also be able to use this through dialogue
public class NPCSequencer : MonoBehaviour
{
    public GameObject[] brains;
    public Transform[] pathTransforms;
    public int brainIndex=0;
    private int prevIndex=0;

    public bool nextBrainTrigger=false;

    public bool looping=false;

    void Awake(){
        //Changing path's parent so it doesn't move with self
        foreach(Transform pathTransform in pathTransforms){
            if(pathTransform.parent==transform) pathTransform.parent=transform.parent;
        }
    }

    void Update()
    {
        if(nextBrainTrigger){
            brainIndex++;
            if(looping){
                brainIndex=brainIndex%brains.Length;
            }
            SetBrain(brainIndex);
            nextBrainTrigger=false;
        }
        prevIndex=brainIndex;
    }

    public void NextBrain(){
        nextBrainTrigger=true;
    }

    public void SetBrain(int i){
        if(i<=brains.Length){
            foreach(var brain in brains){
                brain.SetActive(false);
            }
            brains[i].SetActive(true);
        }else{
            Debug.LogWarning("Tried to set brain but "+i+" is bigger than brains length.");
        }
    }
}
