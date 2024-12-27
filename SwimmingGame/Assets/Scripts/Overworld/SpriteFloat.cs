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

    void Start(){
        spriteTransform=GetComponentInChildren<SpriteRenderer>().transform;
        spriteOriginalPosition=spriteTransform.localPosition;
        floatTimer=Random.Range(0f,floatPeriod);
    }

    void Update(){
        floatTimer+=Time.deltaTime;
        Vector3 spritePos=spriteOriginalPosition;
        spritePos.y=spriteOriginalPosition.y+Mathf.Sin(floatTimer*Mathf.PI*2f/floatPeriod)*floatIntensity;
        spriteTransform.localPosition=spritePos;
    }
}
