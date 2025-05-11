using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritePulse : MonoBehaviour
{
    public float pulsePeriod=5f;
    private float pulseTimer=0f;
    public float pulseIntensity=.1f;
    public float pulseOpacityIntensity=0f;
    private Vector3 spriteOriginalScale;
    private Transform spriteTransform;
    private float initialOpacity;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer=GetComponentInChildren<SpriteRenderer>();
        spriteTransform=spriteRenderer.transform;
        spriteOriginalScale=spriteTransform.localScale;
        pulseTimer=Random.Range(0f,pulsePeriod);
        initialOpacity=spriteRenderer.color.a;
    }

    void Update()
    {
        pulseTimer+=Time.deltaTime;
        Vector3 spriteScale=spriteOriginalScale;
        spriteScale=spriteOriginalScale*(1f+Mathf.Sin(pulseTimer*Mathf.PI*2f/pulsePeriod)*pulseIntensity);
        spriteTransform.localScale=spriteScale;
        
        if(pulseOpacityIntensity!=0f){
            Color c=spriteRenderer.color;
            c.a=initialOpacity-Mathf.Sin(pulseTimer*Mathf.PI*2f/pulsePeriod)*pulseOpacityIntensity;
            spriteRenderer.color=c;
        }
    }
}
