using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBorders : MonoBehaviour
{
    public bool reactToDialogue;

    public ScreenBorder[] screenBorders;

    private Dialogue dialogue;

    public float inactiveScale=2.2f;
    public float activeScale=1.2f;

    public float lerpSpeed=1f;

    private bool active=false;
    void Start()
    {
        dialogue=FindObjectOfType<Dialogue>();

        foreach(ScreenBorder sb in screenBorders){
            sb.anchoredPositions=new Vector2[sb.components.Length];
            sb.scales=new Vector2[sb.components.Length];
            for(var i=0;i<sb.components.Length;i++){
                sb.anchoredPositions[i]=sb.components[i].anchoredPosition;
                sb.components[i].anchoredPosition=sb.anchoredPositions[i]*2f;
                sb.components[i].gameObject.SetActive(true);
                sb.scales[i]=sb.components[i].localScale;
            }
        }
    }

    void Update()
    {
        //Update 

        // foreach(ScreenBorder sb in screenBorders){
        //     sb.active=false;
        // }

        // if(reactToDialogue){
        //     if(dialogue.inDialogue && !dialogue.isAmbient){

        //     }
        // }


        foreach(ScreenBorder sb in screenBorders){
            for(var i=0;i<sb.components.Length;i++){
                Vector2 targetAP=sb.anchoredPositions[i];
                Vector2 targetScale=sb.scales[i];
                if(!sb.active){ 
                    targetAP=sb.targetAnchoredPositions[i];
                    targetScale=sb.targetScales[i];
                }
                sb.components[i].anchoredPosition=Vector2.Lerp(sb.components[i].anchoredPosition,targetAP,lerpSpeed*Time.deltaTime);
                sb.components[i].localScale=Vector2.Lerp(sb.components[i].localScale,targetScale,lerpSpeed*Time.deltaTime);
            }
        }
    }

    public void ActivateBorder(string name, bool b){
        foreach(ScreenBorder sb in screenBorders){
            if(sb.name.ToLower()==name.ToLower()){
                sb.active=b;
            }
        }
    }
}

[Serializable]
public class ScreenBorder{
    public string name;
    public RectTransform[] components;
    [HideInInspector]
    public Vector2[] anchoredPositions;
    public Vector2[] targetAnchoredPositions;
    [HideInInspector]
    public Vector2[] scales;
    public Vector2[] targetScales;
    public bool active;
}
