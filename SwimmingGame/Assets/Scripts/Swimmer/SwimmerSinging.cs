using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using System.Runtime.InteropServices;

public class SwimmerSinging : MonoBehaviour
{
    public bool canSing=false;
    private EventInstance  a4Event;
    private EventInstance b4Event;

    public Dictionary<string,EventInstance> events;

    [Tooltip("Keys that player can play.")]
    public string[] keys;

    [Tooltip("The dot that moves on the canvas to signify singing")]
    public GameObject singingDot;
    private RectTransform singingDotRect;
    [Tooltip("Radius of singing circle on canvas.")]
    public float circleRadius=255f;
    private Vector2 rectTargetPosition;
    private Vector2 rectCenter;
    public float noteLerpValue=10f;
    public float rectDeadzone=0.1f;

    private PlayerInput playerInput;

    private Vector2 singingNote;

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
    }

    void Update()
    {
        if(canSing){
            if(Input.GetKeyDown(KeyCode.B)){
                ToggleNote("B4");
            }
            Vector2 inputNote=playerInput.singingNote;
            if(playerInput.currentControlScheme=="Gamepad"){
                inputNote=Vector2.ClampMagnitude(inputNote/(1f-rectDeadzone),1f);
            }
            
            singingNote=Vector2.Lerp(singingNote,inputNote,noteLerpValue*Time.deltaTime);
            singingDotRect.anchoredPosition=rectCenter+singingNote*circleRadius;

            if(IsPlaying("B4")){
                SetVolume("B4",singingNote.magnitude);
            }
        }

    }

    void PlayNote(string note){
        Debug.Log("play note");
        EventInstance instance=events[note];
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.start();
        SetVolume(note,1f);
    }

    void SetVolume(string note,float v){
        EventInstance instance=events[note];
        instance.setParameterByName("Volume",v);
    }

    void StopNote(string note){
        EventInstance instance=events[note];
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        Debug.Log("stop note");
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
