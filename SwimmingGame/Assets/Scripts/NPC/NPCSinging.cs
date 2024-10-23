using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using UnityEditor.EditorTools;

public class NPCSinging : Singing
{
   
    public SpriteRenderer singingTraceSpriteRenderer;
    public Sprite[] singingTraceSprites;

    [Tooltip("Musical sequence")]
    public List<MusicalEvent> sequence;
    public int sequenceIndex=0;


    [Tooltip("Can set starting time here for offset")]
    public float timer=0f;

    private float targetOpacity=0f;
    public float imageOpacityLerpSpeed=1f;
    public float maxOpacity=0.4f;

    public bool singing=false;

    private SwimmerSinging swimmerSinging;
    [Tooltip("Max distance to detect swimmer when singing to harmonize.")]
    public float maxSwimmerDistance=2f;

    public float harmonyValue;
    [Tooltip("If player is harmonizing for this long (in seconds) the dialogue may start.")]
    public float harmonyMaxValue=1f;

    public bool canSing=true;


    void Start()
    {
        SingingStart();

        foreach(var e in events.Values){
            RuntimeManager.AttachInstanceToGameObject(e,transform,GetComponent<Rigidbody>());
        }

        swimmerSinging=FindObjectOfType<SwimmerSinging>();
    }

    void Update()
    {
        if(canSing && sequence.Count>=0){
            timer+=Time.deltaTime;
            if(timer>=sequence[sequenceIndex].length){
                sequenceIndex+=1;
                sequenceIndex=sequenceIndex%sequence.Count;
                timer=0f;
            }

            bool prevSinging=singing;

            if(sequence[sequenceIndex].musicNote=="" || sequence[sequenceIndex].musicNote=="0" || sequence[sequenceIndex].musicNote.ToLower()=="pause"){
                singing=false;
                singingVolume=0f;
            }else{
                singing=true;
                singingVolume=1f;
                singingNote=sequence[sequenceIndex].musicNote;
            }

            if(singing && timer==0f){
                StopAllNotes();
                PlayNote(singingNote);
                targetOpacity=1f;
            }else if(!singing){
                StopAllNotes();
                targetOpacity=0f;
            }

            singingTraceSpriteRenderer.sprite=singingTraceSprites[possibleNotes.IndexOf(singingNote)];

            SingingUpdate();

            // Checking harmony and starting dialogue if harmony is achieved
            // Right now harmony only goes up/down if the npc is currently
            if(singing && swimmerSinging.singing && Vector3.Distance(transform.position,swimmerSinging.transform.position)<=maxSwimmerDistance){
                if(isHarmonizing(swimmerSinging)){
                    harmonyValue+=Time.deltaTime;
                }
                if(harmonyValue>=harmonyMaxValue){
                    DialogueStart();
                }
            }else if(singing){
                harmonyValue-=Time.deltaTime/2f;
            }

            harmonyValue=Mathf.Clamp(harmonyValue,0f,harmonyMaxValue);
        }

        
        Color c=singingTraceSpriteRenderer.color;
        float a=Mathf.Lerp(c.a,targetOpacity,imageOpacityLerpSpeed*Time.deltaTime);
        singingTraceSpriteRenderer.color=new Color(c.r,c.g,c.b,a);
    }

    bool isHarmonizing(Singing s){
        string otherNote=s.singingNote;
        int index=possibleNotes.IndexOf(singingNote);
        if(otherNote==possibleNotes[(index+2)%possibleNotes.Count] || otherNote==possibleNotes[(index+possibleNotes.Count-2)%possibleNotes.Count]){
            return true;
        }
        return false;
    }

    void DialogueStart(){
        StopAllNotes();
        canSing=false;
        harmonyValue=0f;
        targetOpacity=0f;
        Debug.Log("Start dialogue!");
    }
}