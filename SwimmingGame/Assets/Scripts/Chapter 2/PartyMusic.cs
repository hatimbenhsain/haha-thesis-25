using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PartyMusic : MonoBehaviour
{
    private Dialogue dialogue;
    private EventInstance instance;

    private Transform[] speakers;
    private Transform player;
    [Tooltip("Further than this proximity do not boost music volume")]
    public float maxProximity=5f;
    [Tooltip("Closer than this proximity music volume is at max")]
    public float minProximity=1f;

    void Start()
    {
        instance=GetComponent<StudioEventEmitter>().EventInstance;
        dialogue=FindObjectOfType<Dialogue>();

        GameObject[] gs=GameObject.FindGameObjectsWithTag("Speaker");
        speakers=new Transform[gs.Length];
        for(var i=0;i<gs.Length;i++){
            speakers[i]=gs[i].transform;
        }

        player=GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if(dialogue.inDialogue){
            instance.setParameterByName("Music Intensity",1f);
        }else{
            instance.setParameterByName("Music Intensity",0f);
        }

        float proximityToSpeakers=100f;
        foreach(Transform t in speakers){
            float d=Vector3.Distance(t.position,player.position);
            proximityToSpeakers=Mathf.Min(d,proximityToSpeakers);
        }
        proximityToSpeakers=Mathf.Clamp(proximityToSpeakers,minProximity,maxProximity);
        instance.setParameterByName("proximityToSpeaker",(maxProximity-proximityToSpeakers)/(maxProximity-minProximity));
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
