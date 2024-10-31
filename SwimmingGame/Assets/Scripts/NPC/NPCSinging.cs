using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using UnityEditor.EditorTools;
using Obi;

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
    public float harmonyTargetValue=1f;

    public bool canSing=true;

    [Tooltip("To humanize the singing every singing event's duration is randomized by a maximum of this value.")]
    public float timerMaxVariationLength=0.2f;
    private float currentEventLength;

    private NPCOverworld npcBrain;

    public HarmonyTypes harmonyType=HarmonyTypes.Triangle;

    private bool prevCanSing;

    private bool harmonized=false;


    void Start()
    {
        SingingStart();

        foreach(var e in events.Values){
            Rigidbody body;
            if(!TryGetComponent<Rigidbody>(out body)){
                body=GetComponentInParent<Rigidbody>();
            }
            RuntimeManager.AttachInstanceToGameObject(e,transform,body);
        }

        swimmerSinging=FindObjectOfType<SwimmerSinging>();

        currentEventLength=sequence[sequenceIndex].length;

        npcBrain=GetComponent<NPCOverworld>();
    }

    void Update()
    {
        if(canSing && sequence.Count>0){
            float distanceFromPlayer=Vector3.Distance(transform.position,swimmerSinging.transform.position);

            timer+=Time.deltaTime;
            if(timer>=currentEventLength){
                sequenceIndex+=1;
                sequenceIndex=sequenceIndex%sequence.Count;
                currentEventLength=sequence[sequenceIndex].length+Random.Range(-timerMaxVariationLength,timerMaxVariationLength);
                timer=0f;

                if(harmonized){ //Only do harmonized effects at the end of a singing event, so there's no abrupt cutoff
                    StopAllNotes();
                    //canSing=false;
                    harmonyValue=0f;
                    targetOpacity=0f;
                    npcBrain.Harmonized();
                    harmonized=false;
                }
            }

            if(canSing){
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

                    RuntimeManager.AttachInstanceToGameObject(events[singingNote],transform);
                }else if(!singing && timer==0f){
                    StopAllNotes();
                    targetOpacity=0f;
                }

                //singingTraceSpriteRenderer.sprite=singingTraceSprites[possibleNotes.IndexOf(singingNote)];
                singingTraceSpriteRenderer.GetComponent<Animator>().SetInteger("note",possibleNotes.IndexOf(singingNote));

                SingingUpdate();

                if(singing && distanceFromPlayer>maxSwimmerDistance){
                    targetOpacity=0.1f;
                }else if(singing){
                    targetOpacity=0.25f;
                }

                // Checking harmony and starting dialogue if harmony is achieved
                // Right now harmony only goes up/down if the npc is currently
                if(singing && swimmerSinging.singing && distanceFromPlayer<=maxSwimmerDistance){
                    if(isHarmonizing(swimmerSinging) || harmonized){
                        harmonyValue+=Time.deltaTime;
                        targetOpacity=1f;
                    }
                    if(harmonyValue>=harmonyTargetValue){
                        Harmonized();
                    }
                }else if(singing){
                    harmonyValue-=Time.deltaTime/2f;
                }

                if(singing && swimmerSinging.singing && isHarmonizing(swimmerSinging)){
                    singingTraceSpriteRenderer.GetComponent<Animator>().SetBool("growing",true);
                }else{
                    singingTraceSpriteRenderer.GetComponent<Animator>().SetBool("growing",false);
                }


                harmonyValue=Mathf.Clamp(harmonyValue,0f,harmonyTargetValue);
            }
        }else if(sequence.Count>0 && prevCanSing){
            StopAllNotes();
        }

        
        Color c=singingTraceSpriteRenderer.color;
        float a=Mathf.Lerp(c.a,targetOpacity,imageOpacityLerpSpeed*Time.deltaTime);
        singingTraceSpriteRenderer.color=new Color(c.r,c.g,c.b,a);

        prevCanSing=canSing;
    }

    bool isHarmonizing(Singing s){
        string otherNote=s.singingNote;
        int index=possibleNotes.IndexOf(singingNote);
        bool harmonizing=false;
        switch(harmonyType){
            case HarmonyTypes.Triangle:
                if(otherNote==possibleNotes[(index+2)%possibleNotes.Count] || otherNote==possibleNotes[(index+possibleNotes.Count-2)%possibleNotes.Count]){
                    harmonizing=true;
                }
                break;
            case HarmonyTypes.NextNote:
                if(otherNote==possibleNotes[(index+1)%possibleNotes.Count] || otherNote==possibleNotes[(index+possibleNotes.Count-1)%possibleNotes.Count]){
                    harmonizing=true;
                }
                break;
            case HarmonyTypes.SameNote:
                if(otherNote==possibleNotes[index]){
                    harmonizing=true;
                }
                break;
            case HarmonyTypes.Anything:
                harmonizing=true;
                break;
            case HarmonyTypes.None:
                break;
        }
        return harmonizing;
    }

    void Harmonized(){
        harmonized=true;
    }

    public void StopSinging(){
        canSing=false;
        targetOpacity=0f;
    }

    public void ContinueSinging(){
        canSing=true;
    }

    public void RestartSinging(){
        canSing=true;
        sequenceIndex=0;
        timer=0f;
    }

}

public enum HarmonyTypes{
    Triangle,   //Harmony note is on either extreme edge of the pentacle (~3rd/5th)
    SameNote,   //Singing the same note
    Anything,   //Singing whatever
    NextNote,   //Singing the next note up or down on the pentacle
    None        //Never harmonizes
}