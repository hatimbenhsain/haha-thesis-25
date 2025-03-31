using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        tutorial=FindObjectOfType<Tutorial>();
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
        prevTutorialIndex=tutorial.index;
    }
}
