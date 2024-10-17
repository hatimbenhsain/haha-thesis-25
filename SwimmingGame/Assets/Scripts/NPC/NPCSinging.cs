using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class NPCSinging : Singing
{
   
    public SpriteRenderer singingTraceSpriteRenderer;
    public Sprite[] singingTraceSprites;


    [Tooltip("How long to pause between notes (seconds).")]
    public float pauseLength=2f;
    [Tooltip("How long to sing for (seconds).")]
    public float singingLength=2f;

    [Tooltip("Can set starting time here for offset")]
    public float timer=0f;

    private float targetOpacity=0f;
    public float imageOpacityLerpSpeed=1f;
    public float maxOpacity=0.4f;

    public bool singing=false;


    void Start()
    {
        SingingStart();
    }

    void Update()
    {
        timer+=Time.deltaTime;
        timer=timer%(pauseLength+singingLength);

        bool prevSinging=singing;

        if(timer<=singingLength){
            singing=true;
            singingVolume=1f;
        }else{
            singing=false;
            singingVolume=0f;
        }

        if(singing && !prevSinging){
            StopAllNotes();
            PlayNote(singingNote);
            targetOpacity=1f;
        }else if(!singing){
            StopAllNotes();
            targetOpacity=0f;
        }

        singingTraceSpriteRenderer.sprite=singingTraceSprites[possibleNotes.IndexOf(singingNote)];

        Color c=singingTraceSpriteRenderer.color;
        float a=Mathf.Lerp(c.a,targetOpacity,imageOpacityLerpSpeed*Time.deltaTime);
        singingTraceSpriteRenderer.color=new Color(c.r,c.g,c.b,a);

        SingingUpdate();
    }
}
