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

public class SwimmerSinging : Singing
{
    public bool canSing=false;

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

    private Vector2 singingNotePosition; // Relative position on the wheel

    private float startingAngle=1.5f;

    private Vector2 inputNote;

    private float targetOpacity=0f;
    public float imageOpacityLerpSpeed=1f;

    public float mouseSensitivity=2f;

    public bool singing;

    void Start()
    {
        playerInput=FindObjectOfType<PlayerInput>();
        singingDotRect=singingDot.GetComponent<RectTransform>();
        rectTargetPosition=singingDotRect.anchoredPosition;
        rectCenter=singingDotRect.anchoredPosition;

        foreach(Image image in images){
            Color c=image.color;
            image.color=new Color(c.r,c.g,c.b,0f);
        }

        SingingStart();
    }

    void Update()
    {
        if(canSing){
            
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

            singingVolume=singingNotePosition.magnitude;

            float angle=Mathf.Atan2(singingNotePosition.y,singingNotePosition.x)/Mathf.PI+1;

            string note="";

            for(var i=0;i<possibleNotes.Count;i++){
                float minAngle=(startingAngle-((i*2f+1f)/possibleNotes.Count)+2f)%2f;
                float maxAngle=(startingAngle-((i*2f-1f)/possibleNotes.Count)+2f)%2f;
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
                    Debug.Log(note);
                    PlayNote(note);
                }
                singingNote=note;
            }

            SingingUpdate();

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

}