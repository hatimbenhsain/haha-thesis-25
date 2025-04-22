using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimmingScreen : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float minSpeed=3f;
    public float maxSpeed=9f;
    public float lerpSpeed=1f;
    public float maxOpacity=0.5f;

    private Swimmer swimmer;
    private float targetAlpha;
    void Start()
    {
        spriteRenderer=GetComponent<SpriteRenderer>();
        swimmer=FindObjectOfType<Swimmer>();
    }

    void Update()
    {
        targetAlpha=Mathf.Clamp((swimmer.GetVelocity().magnitude-minSpeed)/(maxSpeed-minSpeed),0f,1f)*maxOpacity;
        Color c=spriteRenderer.color;
        c.a=Mathf.Lerp(c.a,targetAlpha,lerpSpeed*Time.deltaTime);
        spriteRenderer.color=c;
    }
}
