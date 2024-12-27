using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDistanceResizer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float minDistance=10f;
    private Vector3 baseScale;
    void Start()
    {
        spriteRenderer=GetComponentInChildren<SpriteRenderer>();
        baseScale=transform.localScale;
    }

    void Update()
    {
        transform.localScale=baseScale;
        float distance=Vector3.Distance(transform.position,Camera.main.transform.position);
        if(distance>minDistance){
            Vector3 scale=baseScale*distance/minDistance;
            scale=scale.normalized*Mathf.Max(baseScale.magnitude,scale.magnitude);
            transform.localScale=scale;
        }
    }
}
