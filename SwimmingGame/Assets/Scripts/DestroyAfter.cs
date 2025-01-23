using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float time=1f;
    private float timer;

    public bool fadeOut=false;
    public bool fadeIn=false;
    public float fadeInTime=0f;
    private SpriteRenderer spriteRenderer;
    private float originalOpacity;
    // Start is called before the first frame update
    void Start()
    {
        timer=0f;
        if(fadeOut || fadeIn){
            spriteRenderer=GetComponentInChildren<SpriteRenderer>();
            originalOpacity=spriteRenderer.color.a;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer+=Time.deltaTime;
        if(fadeIn){
            Color c=spriteRenderer.color;
            c.a=originalOpacity*timer/fadeInTime;
            spriteRenderer.color=c;
        }
        if(fadeOut && (!fadeIn || timer>fadeInTime)){
            Color c=spriteRenderer.color;
            c.a=originalOpacity*(1f-(timer-fadeInTime)/(time-fadeInTime));
            spriteRenderer.color=c;
        }
        if(timer>=time){
            Destroy(gameObject);
        }
    }
}
