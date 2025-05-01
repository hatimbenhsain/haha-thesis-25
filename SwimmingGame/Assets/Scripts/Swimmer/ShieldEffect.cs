using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ShieldEffect : MonoBehaviour
{
    public Swimmer swimmer;
    public float alphaLerpSpeed=1f;
    private float initialAlpha;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer=GetComponent<SpriteRenderer>();

        Color c=spriteRenderer.color;
        initialAlpha=c.a;
        c.a=0f;
        spriteRenderer.color=c;
    }

    void Update()
    {
        Color c=spriteRenderer.color;
        float targetAlpha=0f;
        if(swimmer.IsCoasting()){
            targetAlpha=initialAlpha;
        }
        c.a=Mathf.Lerp(c.a,targetAlpha,alphaLerpSpeed*Time.deltaTime);

        spriteRenderer.color=c;
    }
}
