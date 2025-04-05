using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFloat : MonoBehaviour
{
    public float floatPeriod=5f;
    private float floatTimer=0f;
    public float floatIntensity=.02f;
    private Vector3 spriteOriginalPosition;
    private Transform spriteTransform;

    public bool matchMusic=false;

    void Start(){
        SpriteRenderer spriteRenderer=GetComponentInChildren<SpriteRenderer>();
        if(spriteRenderer!=null){
            spriteTransform=spriteRenderer.transform;
        }else{
            spriteTransform=transform;
        }
        spriteOriginalPosition=spriteTransform.localPosition;
        floatTimer=Random.Range(0f,floatPeriod);
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
