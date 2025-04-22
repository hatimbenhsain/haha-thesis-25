using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBars : MonoBehaviour
{
    public bool reactToDialogue;

    private Dialogue dialogue;

    public float inactiveScale=1.7f;
    public float activeScale=1f;

    public float lerpSpeed=1f;

    private bool active=false;
    void Start()
    {
        dialogue=FindObjectOfType<Dialogue>();
        float sc=inactiveScale;
        if(active) sc=activeScale;
        transform.localScale=Vector3.one*sc;
    }

    void Update()
    {
        if(reactToDialogue){
            if(dialogue.inDialogue && !dialogue.isAmbient) active=true;
            else active=false;
        }

        float targetScale;

        if(active) targetScale=activeScale;
        else targetScale=inactiveScale;

        transform.localScale=Vector3.one*Mathf.Lerp(transform.localScale.x,targetScale,Time.deltaTime*lerpSpeed);
    }
}
