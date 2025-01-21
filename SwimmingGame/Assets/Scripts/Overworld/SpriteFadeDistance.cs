using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFadeDistance : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float originalOpacity;

    public float minDistance=1f;
    public float maxDistance=5f;

    private Transform swimmer;

    void Start()
    {
        spriteRenderer=GetComponent<SpriteRenderer>();
        originalOpacity=spriteRenderer.color.a;
        swimmer=FindObjectOfType<Swimmer>().transform;
    }

    void Update()
    {
        float opacity=Mathf.Clamp(Vector3.Distance(transform.position,swimmer.position),minDistance,maxDistance);
        opacity=originalOpacity*(1f-(opacity-minDistance)/(maxDistance-minDistance));
        Color c=spriteRenderer.color;
        c.a=opacity;
        spriteRenderer.color=c;
    }
}
