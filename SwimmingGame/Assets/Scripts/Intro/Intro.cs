using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    public Image darkScreen;
    private Tutorial tutorial;
    private int prevTutorialIndex;
    public float fadeInSpeed=20f;

    public float darkScreenOpacityTarget=1f;
    public float darkScreenPulseIntensity=0.1f;
    public float darkScreenPulsePeriod=10f;

    private float timer=0f;
    

    public NPCSequencer exSequencer;

    public RectTransform exRect;
    public float exMoveAcceleration=1f;
    private float exMoveSpeed=0f;

    private Dialogue dialogue;

    public bool swimmerCamOn=false;
    public RawImage swimmerCamImage;
    public float swimmerCamLerpSpeed=1f;

    private bool prevSwimmerCamOn;

    public RectTransform singingWheel;

    private bool loadedCutscene=false;

    public StudioEventEmitter ambiance;

    void Start()
    {
        tutorial=FindObjectOfType<Tutorial>();
        dialogue=FindObjectOfType<Dialogue>();
        Color c=darkScreen.color;
        c.a=1f;
        darkScreen.color=c;
    }

    void Update()
    {
        timer+=Time.deltaTime;
        if(tutorial.index>0){
            Color c=darkScreen.color;
            darkScreenOpacityTarget=darkScreenOpacityTarget-Time.deltaTime/fadeInSpeed;
            darkScreenOpacityTarget=Mathf.Clamp(darkScreenOpacityTarget,darkScreenPulseIntensity,1f);
            c.a=darkScreenOpacityTarget+Mathf.Sin(timer*Mathf.PI*2f/darkScreenPulsePeriod)*darkScreenPulseIntensity;
            darkScreen.color=c;
            if(exSequencer.brainIndex==0){
                exSequencer.nextBrainTrigger=true;
            }
        }
        if(exSequencer.brainIndex>=4){
            exMoveSpeed+=exMoveAcceleration*Time.deltaTime;
            Vector2 pos=exRect.anchoredPosition;
            pos+=exMoveSpeed*new Vector2(.5f,1f)*Time.deltaTime;
            exRect.anchoredPosition=pos;
        }

        if(exSequencer.brainIndex>=4 && (bool)dialogue.story.variablesState["swimmerCamOn"]==true){
            swimmerCamOn=true;
        }

        if(dialogue.inDialogue) ambiance.EventInstance.setParameterByName("intensity",(int)dialogue.story.variablesState["intensity"]);

        if(swimmerCamOn){
            
            FindObjectOfType<Swimmer>().canRotate=false;
            Color c=swimmerCamImage.color;
            c.a=Mathf.Lerp(c.a,1f,swimmerCamLerpSpeed*Time.deltaTime);
            swimmerCamImage.color=c;
            swimmerCamImage.GetComponent<RectTransform>().localScale=Vector3.Lerp(swimmerCamImage.GetComponent<RectTransform>().localScale,Vector3.one,swimmerCamLerpSpeed*Time.deltaTime);
        
            if(!prevSwimmerCamOn){
                FindObjectOfType<Swimmer>().canMove=true;
                
                tutorial.GoToTutorialPart(3);
                Vector2 pos=singingWheel.anchoredPosition;
                pos.x=0;
                singingWheel.anchoredPosition=pos;
            }
        }

        if(!loadedCutscene && (bool)dialogue.story.variablesState["loadCutscene"]==true){
            loadedCutscene=true;
            StartCoroutine(StartCutscene());
        }


        prevTutorialIndex=tutorial.index;
        prevSwimmerCamOn=swimmerCamOn;
    }

    IEnumerator StartCutscene(){
        yield return new WaitForSeconds(4f);    // U CAN ADJUST THIS TIME
        dialogue.EndDialogue();
        // START CUTSCENE
    }
    
}
