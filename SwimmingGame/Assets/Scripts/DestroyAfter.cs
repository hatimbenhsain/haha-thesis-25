using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float time=1f;
    private float timer;

    public bool fadeOut=false;
    private SpriteRenderer spriteRenderer;
    private float originalOpacity;
    // Start is called before the first frame update
    void Start()
    {
        timer=0f;
        if(fadeOut){
            spriteRenderer=GetComponentInChildren<SpriteRenderer>();
            originalOpacity=spriteRenderer.color.a;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer+=Time.deltaTime;
        if(fadeOut){
            Color c=spriteRenderer.color;
            c.a=originalOpacity*(1f-timer/time);
            spriteRenderer.color=c;
        }
        if(timer>=time){
            Destroy(gameObject);
        }
    }
}
