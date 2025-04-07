using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPulse : MonoBehaviour
{
    private MusicBeat musicBeat;

    [Tooltip("If at 1, do a full animation every beat.")]
    public float animationSpeedFactor=.5f;
    [Tooltip("If true, flicker on/off. If false, lerp value.")]
    public bool flicker=false;

    public Color[] colors;
    private Light light;
    public float offset;
    public float ratio=1f;
    void Start()
    {
        light=GetComponent<Light>();
        musicBeat=FindObjectOfType<MusicBeat>();
    }

    void Update()
    {
        float period=60f/(musicBeat.timelineInfo.currentTempo*animationSpeedFactor);
        float value;
        if(Mathf.Floor(musicBeat.timelineInfo.currentTime*0.001f/period+offset)%(1/ratio)==0f){
            value=Mathf.Abs(Mathf.Sin(Mathf.PI*(((musicBeat.timelineInfo.currentTime*0.001f)%(period))/period+offset)));
        }else{
            value=0f;
        }
        value=value*(colors.Length-1);
        if(flicker){
            value=Mathf.Round(value);
        }
        Color c=Color.Lerp(colors[(int)Mathf.Floor(value)],colors[Mathf.Min((int)Mathf.Floor(value)+1,colors.Length-1)],value%1);
        light.color=c;
    }
}
