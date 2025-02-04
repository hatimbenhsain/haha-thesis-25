using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using Obi;
using Ink.Parsed;
using Unity.VisualScripting;

public class NPCSinging : Singing
{
    public SingingMode singingMode=SingingMode.Sequence;
   
    public SpriteRenderer singingTraceSpriteRenderer;
    public Sprite[] singingTraceSprites;
    [Header("Sequence Mode")]


    [Tooltip("Musical sequence")]
    public List<MusicalEvent> sequence;
    public int sequenceIndex=0;




    [Header("Responding Mode")]

    [Tooltip("How long before NPC picks up MC's harmony")]
    public float timeBeforePickingUpHarmony=1f; 
    [Tooltip("How long before NPC stops singing MC's harmony when switching")]
    public float timeBeforeDroppingHarmony=1f;

    [Tooltip("Time after which npc stops singing the same note")]
    public float gettingTiredTime=4f;
    private float gettingTiredTimer=0f;
    
    [Header("Misc")]
    [Tooltip("Can set starting time here for offset")]
    public float timer=0f;

    private float targetOpacity=0f;
    public float imageOpacityLerpSpeed=1f;
    public float maxOpacity=0.4f;

    public bool singing=false;

    private SwimmerSinging swimmerSinging;
    [Tooltip("Max distance to detect swimmer when singing to harmonize. Short if facing NPC.")]
    public float shortSwimmerDistance=2f;
    [Tooltip("Max distance to detect swimmer when singing to harmonize. Long if facing NPC.")]
    public float longSwimmerDistance=4f;
    

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

    private Animator animator;

    private string prevNote;


    void Start()
    {
        SingingStart();

        Rigidbody body;
        if(!TryGetComponent<Rigidbody>(out body)){
            body=GetComponentInParent<Rigidbody>();
        }

        if(singleVoiceMode) RuntimeManager.AttachInstanceToGameObject(voice,transform,body);
        else{
            foreach(var e in events.Values){
                RuntimeManager.AttachInstanceToGameObject(e,transform,body);
            }
        }        

        swimmerSinging=FindObjectOfType<SwimmerSinging>();

        PrepareSinging();

        npcBrain=GetComponent<NPCOverworld>();

        if(!TryGetComponent<Animator>(out animator)){
            transform.parent.TryGetComponent<Animator>(out animator);
        }
    }

    void Update()
    {
        if(singingMode==SingingMode.Sequence && sequence.Count<=0){
            canSing=false;
        }

        if(canSing){
            float distanceFromPlayer=Vector3.Distance(transform.position,swimmerSinging.transform.position);

            timer+=Time.deltaTime;

            //Picking note and telling what/whether to sing
            switch(singingMode){
                case SingingMode.Sequence:
                    if(timer>=currentEventLength){
                        sequenceIndex+=1;
                        sequenceIndex=sequenceIndex%sequence.Count;
                        currentEventLength=sequence[sequenceIndex].length+Random.Range(-timerMaxVariationLength,timerMaxVariationLength);
                        timer=0f;
                        if(harmonized){ //Only do harmonized effects at the end of a singing event, so there's no abrupt cutoff
                            StopAllNotes();
                            harmonyValue=0f;
                            targetOpacity=0f;
                            npcBrain.Harmonized();
                            harmonized=false;
                        }else{
                            if(sequence[sequenceIndex].musicNote=="" || sequence[sequenceIndex].musicNote=="0" || sequence[sequenceIndex].musicNote.ToLower()=="pause"){
                                singing=false;
                                singingVolume=0f;
                            }else{
                                singing=true;
                                singingVolume=1f;
                                singingNote=sequence[sequenceIndex].musicNote;
                            }
                        }
                            
                    }
                    break;
                case SingingMode.Responding:
                    if(singing){
                        singingVolume=1f;
                        gettingTiredTimer+=Time.deltaTime;
                    }else{
                        gettingTiredTimer-=Time.deltaTime;
                    }
                    if(InRange() && !singing){
                        if(!swimmerSinging.singing){
                            timer=0f;
                        }else if(timer>=timeBeforePickingUpHarmony+currentEventLength){
                            gettingTiredTimer=0f;
                            timer=0f;
                            singing=true;
                            singingVolume=1f;
                            singingNote=PickHarmony(swimmerSinging);
                            currentEventLength=Random.Range(-timerMaxVariationLength,timerMaxVariationLength);
                        }
                    }else if(!InRange() || !swimmerSinging.singing || !isHarmonizing(swimmerSinging)){
                        if(timer>=timeBeforeDroppingHarmony+currentEventLength){
                            singing=false;
                            singingVolume=0f;
                            StopAllNotes();
                            currentEventLength=Random.Range(-timerMaxVariationLength,timerMaxVariationLength);
                        }else if(timer>=(timeBeforeDroppingHarmony+currentEventLength)/2){
                            singingVolume=(timer-(timeBeforeDroppingHarmony+currentEventLength)/2)/(timeBeforeDroppingHarmony+currentEventLength);
                        }
                    }else{
                        timer-=Time.deltaTime*2f;
                        timer=Mathf.Max(timer,0.01f);
                        if(harmonized){
                            harmonyValue=0f;
                            targetOpacity=0f;
                            npcBrain.Harmonized();
                            harmonized=false;
                        }
                    }

                    if(singing && gettingTiredTimer>=gettingTiredTime+currentEventLength){
                        singing=false;
                        singingVolume=0f;
                        targetOpacity=0f;
                        StopAllNotes();
                        timer=-timeBeforePickingUpHarmony;
                    }

                    if(distanceFromPlayer>longSwimmerDistance){
                        harmonyValue=0f;
                    }

                    break;
            }

            if(canSing){
                bool prevSinging=singing;

                if(singing && !IsPlaying(singingNote)){
                    if(!singleVoiceMode) StopAllNotes();
                    PlayNote(singingNote);     
                    prevNote=singingNote;      
                    if(singleVoiceMode) RuntimeManager.AttachInstanceToGameObject(voice,transform); 
                    else RuntimeManager.AttachInstanceToGameObject(events[singingNote],transform); // 3D Singing
                }else if(!singing && timer==0f){
                    StopAllNotes();
                    targetOpacity=0f;
                }

                //singingTraceSpriteRenderer.sprite=singingTraceSprites[possibleNotes.IndexOf(singingNote)];
                singingTraceSpriteRenderer.GetComponent<Animator>().SetInteger("note",possibleNotes.IndexOf(singingNote));

                SingingUpdate();

                if(singing && !InRange()){
                    targetOpacity=0.1f*singingVolume;
                }else if(harmonyValue>=harmonyTargetValue && !singing){
                    targetOpacity=0.6f*singingVolume;
                }else if(singing){
                    targetOpacity=0.4f*singingVolume;
                }

                // Checking harmony and starting dialogue if harmony is achieved
                // Right now harmony only goes up/down if the npc is currently
                if(singing && swimmerSinging.singing && InRange()){
                    if(isHarmonizing(swimmerSinging) || harmonized){
                        harmonyValue+=Time.deltaTime;
                        targetOpacity=1f;
                        swimmerSinging.Harmonizing();
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
        }else if(prevCanSing){
            StopAllNotes();
        }

        if(animator!=null){
            if(canSing && singing){
                animator.SetBool("singing",true);
            }else{
                animator.SetBool("singing",false);
            }
        }

        
        Color c=singingTraceSpriteRenderer.color;
        float a=Mathf.Lerp(c.a,targetOpacity,imageOpacityLerpSpeed*Time.deltaTime);
        singingTraceSpriteRenderer.color=new Color(c.r,c.g,c.b,a);

        prevCanSing=canSing;
    }

    public bool InRange(){
        float distanceFromPlayer=Vector3.Distance(transform.position,swimmerSinging.transform.position);
        if(distanceFromPlayer<=shortSwimmerDistance){
            return true;
        }else if(distanceFromPlayer<=longSwimmerDistance){
            float angle=Vector3.Angle(swimmerSinging.transform.forward,transform.position-swimmerSinging.transform.position);
            if(angle<=40f){
                return true;
            }
        }
        return false;
    }

    string PickHarmony(Singing s){
        string harmonyNote="";
        int index=possibleNotes.IndexOf(s.singingNote);
        switch(harmonyType){
            case HarmonyTypes.Triangle:
                if(Random.Range(0f,1f)<0.5f){
                    harmonyNote=possibleNotes[(index+2)%possibleNotes.Count];
                }else{
                    harmonyNote=possibleNotes[(index+possibleNotes.Count-2)%possibleNotes.Count];
                }
                break;
            case HarmonyTypes.NextNote:
                if(Random.Range(0f,1f)<0.5f){
                    harmonyNote=possibleNotes[(index+1)%possibleNotes.Count];
                }else{
                    harmonyNote=possibleNotes[(index+possibleNotes.Count-1)%possibleNotes.Count];
                }
                break;
            case HarmonyTypes.SameNote:
                harmonyNote=s.singingNote;
                break;
            case HarmonyTypes.Anything:
                harmonyNote=possibleNotes[Random.Range(0,possibleNotes.Count)];
                break;
            case HarmonyTypes.None:
                harmonyNote=possibleNotes[Random.Range(0,possibleNotes.Count)];
                break;
        }
        return harmonyNote;
    }

    public bool isHarmonizing(){
        return isHarmonizing(swimmerSinging);
    }

    public bool isHarmonizing(Singing s){
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
        singing=false;
    }

    public void ContinueSinging(){
        canSing=true;
    }

    public void RestartSinging(){
        canSing=true;
        PrepareSinging();
    }

    void PrepareSinging(){
        sequenceIndex=0;
        timer=0f;
        switch(singingMode){
            case SingingMode.Sequence:
                StartSequence();
                break;
            case SingingMode.Responding:
                StartResponding();
                break;
        }
    }

    public void SwitchSingingMode(SingingMode mode){
        SingingMode prevMode=singingMode;
        singingMode=mode;
        if(prevMode!=mode){
            PrepareSinging();
        }
    }

    public void ToggleSingingMode(){
        switch(singingMode){
            case SingingMode.Sequence:
                SwitchSingingMode(SingingMode.Responding);
                break;
            case SingingMode.Responding:
                SwitchSingingMode(SingingMode.Sequence);
                break;
        }
    }

    void StartSequence(){
        sequenceIndex=0;
        currentEventLength=sequence[sequenceIndex].length;
    }

    void StartResponding(){
        currentEventLength=Random.Range(-timerMaxVariationLength,timerMaxVariationLength);
    }

}

public enum HarmonyTypes{
    Triangle,   //Harmony note is on either extreme edge of the pentacle (~3rd/5th)
    SameNote,   //Singing the same note
    Anything,   //Singing whatever
    NextNote,   //Singing the next note up or down on the pentacle
    None        //Never harmonizes
}

public enum SingingMode{
    Sequence,
    Responding
}