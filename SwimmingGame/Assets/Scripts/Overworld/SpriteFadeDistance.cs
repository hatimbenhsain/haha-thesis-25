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

    [Tooltip("If true, this disappears when getting close. If false, disappears when getting far.")]
    public bool fadeWhenNear=false;

    void Start()
    {
        spriteRenderer=GetComponent<SpriteRenderer>();
        originalOpacity=spriteRenderer.color.a;
        swimmer=FindObjectOfType<Swimmer>().transform;
    }

    void Update()
    {
        float opacity=Mathf.Clamp(Vector3.Distance(transform.position,swimmer.position),minDistance,maxDistance);
        if(!fadeWhenNear) opacity=originalOpacity*(1f-(opacity-minDistance)/(maxDistance-minDistance));
        else opacity=originalOpacity*(opacity-minDistance)/(maxDistance-minDistance);
        Color c=spriteRenderer.color;
        c.a=opacity;
        spriteRenderer.color=c;
    }
}
