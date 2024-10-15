using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using System.Runtime.InteropServices;
using System.Dynamic;
using UnityEditor.EditorTools;
using System.Reflection;
using UnityEngine.UI;

public class SwimmerSinging : MonoBehaviour
{
    public bool canSing=false;
    private EventInstance  a4Event;
    private EventInstance b4Event;

    public Dictionary<string,EventInstance> events;

    [Tooltip("Keys that player can play.")]
    public string[] keys;

    [Tooltip("Currently playable notes on the wheel.")]
    public string[] possibleNotes;

    [Tooltip("The dot that moves on the canvas to signify singing")]
    public GameObject singingDot;
    private RectTransform singingDotRect;
    [Tooltip("Radius of singing circle on canvas.")]
    public float circleRadius=255f;
    private Vector2 rectTargetPosition;
    private Vector2 rectCenter;
    [Tooltip("Images to affect opacity.")]
    public Image[] images;
    public float noteLerpValue=10f;

    private PlayerInput playerInput;

    [Tooltip("Current note being sung")]
    public string singingNote;
    private Vector2 singingNotePosition; // Relative position on the wheel

    private float startingAngle=1.5f;

    private Vector2 inputNote;

    private float targetOpacity=0f;
    public float imageOpacityLerpSpeed=1f;

    public float mouseSensitivity=2f;

    public bool singing;

    void Start()
    {
        events=new Dictionary<string,EventInstance>();
        foreach(string key in keys){
            EventInstance instance=FMODUnity.RuntimeManager.CreateInstance("event:/Singing/Swimmer/"+key.ToUpper());
            events.Add(key,instance);
        }

        playerInput=FindObjectOfType<PlayerInput>();
        singingDotRect=singingDot.GetComponent<RectTransform>();
        rectTargetPosition=singingDotRect.anchoredPosition;
        rectCenter=singingDotRect.anchoredPosition;

        foreach(Image image in images){
            Color c=image.color;
            image.color=new Color(c.r,c.g,c.b,0f);
        }
    }

    void Update()
    {
        if(canSing){
            if(Input.GetKeyDown(KeyCode.B)){
                ToggleNote("B4");
            }

            if(playerInput.currentControlScheme=="Gamepad"){
                inputNote=playerInput.singingNote;
                if(inputNote.magnitude>0f){
                    singing=true;
                }else{
                    singing=false;
                }
            }else if(playerInput.currentControlScheme=="Keyboard&Mouse" && playerInput.singing){
                inputNote=playerInput.singingNote*Time.deltaTime*mouseSensitivity+inputNote;
                Debug.Log("inputting mouse");
                singing=true;
            }else{
                inputNote=Vector2.zero;
                singing=false;
            }

            inputNote=Vector2.ClampMagnitude(inputNote,1f);
            
            singingNotePosition=Vector2.Lerp(singingNotePosition,inputNote,noteLerpValue*Time.deltaTime);
            singingDotRect.anchoredPosition=rectCenter+singingNotePosition*circleRadius;

            float angle=Mathf.Atan2(singingNotePosition.y,singingNotePosition.x)/Mathf.PI+1;

            string note="";

            for(var i=0;i<possibleNotes.Length;i++){
                float minAngle=(startingAngle-((i*2f+1f)/possibleNotes.Length)+2f)%2f;
                float maxAngle=(startingAngle-((i*2f-1f)/possibleNotes.Length)+2f)%2f;
                if((angle<maxAngle && angle>=minAngle) ||
                (angle<maxAngle && angle>=0 && maxAngle<minAngle) || (angle<=2 && angle>=minAngle && maxAngle<minAngle)){
                    note=possibleNotes[i];
                    break;
                }
            }

            if(singingNote!=note){
                if(singingNote!=""){
                    StopNote(singingNote);
                }
                if(note!=""){
                    PlayNote(note);
                }
                singingNote=note;
            }

            if(singingNote!=""){
                SetVolume(singingNote,singingNotePosition.magnitude);
            }

            if(singing){
                targetOpacity=1f;
            }else{
                targetOpacity=0f;
            }

            foreach(Image image in images){
                Color c=image.color;
                float a=Mathf.Lerp(c.a,targetOpacity,imageOpacityLerpSpeed*Time.deltaTime);
                image.color=new Color(c.r,c.g,c.b,a);
            }

        }

    }

    void StopAllNotes(){
        foreach(var note in possibleNotes){
            StopNote(note);
        }
    }

    void PlayNote(string note){
        EventInstance instance=events[note];
        instance.start();
    }

    void SetVolume(string note,float v){
        EventInstance instance=events[note];
        instance.setParameterByName("Volume",v);
    }

    void StopNote(string note){
        EventInstance instance=events[note];
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    void ToggleNote(string note){
        if(!IsPlaying(note)){
            PlayNote(note);
        }else{
            StopNote(note);
        }
    }

    bool IsPlaying(string note){
        EventInstance instance=events[note];
        PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        if(state==PLAYBACK_STATE.STOPPED){
            return false;
        }else{
            return true;
        }
    }

    private void OnDestroy() {
        foreach(var e in events.Values){
            e.release();
        }
    }
}
