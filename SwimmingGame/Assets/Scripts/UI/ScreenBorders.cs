using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ScreenBorders : MonoBehaviour
{

    public ScreenBorder[] screenBorders;

    private Dialogue dialogue;

    public float lerpSpeed=1f;

    private bool active=false;
    protected virtual void Start()
    {
        dialogue=FindObjectOfType<Dialogue>();

        foreach(ScreenBorder sb in screenBorders){
            sb.anchoredPositions=new Vector2[sb.components.Length];
            sb.scales=new Vector2[sb.components.Length];
            sb.alphas=new float[sb.components.Length];
            sb.images=new Image[sb.components.Length];
            for(var i=0;i<sb.components.Length;i++){
                sb.images[i]=sb.components[i].GetComponentInChildren<Image>();
                sb.alphas[i]=sb.images[i].color.a;
                sb.anchoredPositions[i]=sb.components[i].anchoredPosition;
                sb.components[i].anchoredPosition=sb.targetAnchoredPositions[i];
                sb.components[i].gameObject.SetActive(true);
                sb.scales[i]=sb.components[i].localScale;
                sb.components[i].localScale=sb.targetScales[i];
            }
        }
    }

    protected virtual void Update()
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
                float targetAlpha=sb.alphas[i];
                if(!sb.active){ 
                    targetAP=sb.targetAnchoredPositions[i];
                    targetScale=sb.targetScales[i];
                    targetAlpha=sb.targetAlphas[i];
                }
                sb.components[i].anchoredPosition=Vector2.Lerp(sb.components[i].anchoredPosition,targetAP,lerpSpeed*Time.deltaTime);
                sb.components[i].localScale=Vector2.Lerp(sb.components[i].localScale,targetScale,lerpSpeed*Time.deltaTime);
                Color c=sb.images[i].color;
                c.a=Mathf.Lerp(c.a,targetAlpha,lerpSpeed*Time.deltaTime);
                sb.images[i].color=c;
            }
        }
    }

    virtual public void ActivateBorder(string name, bool b){
        ScreenBorder sb=GetBorder(name);
        if(sb!=null) sb.active=b;
    }

    virtual public bool IsActive(string name){
        ScreenBorder sb=GetBorder(name);
        if(sb!=null && sb.active) return true;
        return false; 
    }

    public ScreenBorder GetBorder(string name){
        foreach(ScreenBorder sb in screenBorders){
            if(sb.name.ToLower()==name.ToLower()){
                return sb;
            }
        }
        return null;
    }
}

[Serializable]
public class ScreenBorder{
    public string name;
    public RectTransform[] components;
    [HideInInspector]
    public Image[] images;
    [HideInInspector]
    public Vector2[] anchoredPositions;
    [Tooltip("Anchored positions when inactive.")]
    public Vector2[] targetAnchoredPositions;
    [HideInInspector]
    public Vector2[] scales;
    [Tooltip("Scales when inactive.")]
    public Vector2[] targetScales;
    [HideInInspector]
    public float[] alphas;
    [Tooltip("Color alpha when inactive.")]
    public float[] targetAlphas;
    public bool active;
}
