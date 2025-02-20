using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDistanceResizer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float minDistance=10f;
    [Tooltip("Past this distance keep it fixed")]
    public float maxDistance=-1f;
    private Vector3 baseScale;

    [Tooltip("If 1f, basically stay the same size on screen. If between 0 and 1, shrink at a slower pace than normal.")]
    public float resizeFactor=1f;
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
            if(maxDistance!=-1f){
                distance=Mathf.Min(distance,maxDistance);
            }
            Vector3 scale=baseScale+baseScale*(distance/minDistance-1)*resizeFactor;
            scale=scale.normalized*Mathf.Max(baseScale.magnitude,scale.magnitude);
            transform.localScale=scale;
        }
    }

    void OnDisable()
    {
        transform.localScale=baseScale;
    }
}
