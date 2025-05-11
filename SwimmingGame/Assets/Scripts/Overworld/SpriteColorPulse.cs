using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorPulse : MonoBehaviour
{
    public float pulsePeriod=5f;
    private float pulseTimer=0f;
    [Tooltip("If true, flicker on/off. If false, lerp value.")]
    public bool flicker=false;

    public Color[] colors;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer=GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        pulseTimer+=Time.deltaTime;
        float value=pulseTimer*(colors.Length-1)/pulsePeriod;
        if(flicker){
            value=Mathf.Round(value);
        }
        Color c=Color.Lerp(colors[(int)Mathf.Floor(value)%colors.Length],colors[((int)Mathf.Floor(value)+1)%colors.Length],value%1);
        spriteRenderer.color=c;
    }
}
