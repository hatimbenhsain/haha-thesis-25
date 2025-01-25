using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritePulse : MonoBehaviour
{
    public float pulsePeriod=5f;
    private float pulseTimer=0f;
    public float pulseIntensity=.1f;
    private Vector3 spriteOriginalScale;
    private Transform spriteTransform;

    void Start()
    {
        spriteTransform=GetComponentInChildren<SpriteRenderer>().transform;
        spriteOriginalScale=spriteTransform.localScale;
        pulseTimer=Random.Range(0f,pulsePeriod);
    }

    void Update()
    {
        pulseTimer+=Time.deltaTime;
        Vector3 spriteScale=spriteOriginalScale;
        spriteScale=spriteOriginalScale*(1f+Mathf.Sin(pulseTimer*Mathf.PI*2f/pulsePeriod)*pulseIntensity);
        spriteTransform.localScale=spriteScale;
    }
}
