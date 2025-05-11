using System.Collections;
using System.Collections.Generic;
using RootMotion.Demos;
using UnityEngine;

public class SpriteFloat : MonoBehaviour
{
    public float floatPeriod=5f;
    private float floatTimer=0f;
    public float floatIntensity=.02f;
    private Vector3 spriteOriginalPosition;
    private Transform spriteTransform;

    public bool matchMusic=false;
    public float offset=-1f;

    void Start(){
        SpriteRenderer spriteRenderer=GetComponentInChildren<SpriteRenderer>();
        if(spriteRenderer!=null){
            spriteTransform=spriteRenderer.transform;
        }else{
            spriteTransform=transform;
        }
        spriteOriginalPosition=spriteTransform.localPosition;
        if(offset==-1f) floatTimer=Random.Range(0f,floatPeriod);
        else floatTimer=offset;
    }

    void Update(){
        float period=floatPeriod;
        if(matchMusic){
            period=60f*period/MusicBeat.GetBPM();
        }
        floatTimer+=Time.deltaTime;
        Vector3 spritePos=spriteOriginalPosition;
        spritePos.y=spriteOriginalPosition.y+Mathf.Sin(floatTimer*Mathf.PI*2f/period)*floatIntensity;
        spriteTransform.localPosition=spritePos;
    }
}
