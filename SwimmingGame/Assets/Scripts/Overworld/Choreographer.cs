using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Choreographer : MonoBehaviour
{
    private Animator animator;
    private MusicBeat musicBeat;

    [Tooltip("If at 1, do a full animation every beat.")]
    public float animationSpeedFactor=.5f;

    void Start()
    {
        animator=GetComponentInChildren<Animator>();
        musicBeat=FindObjectOfType<MusicBeat>();
    }

    void Update()
    {
        //animator.speed=animationSpeedFactor*musicBeat.timelineInfo.currentTempo/120f;
        float period=60f/(musicBeat.timelineInfo.currentTempo*animationSpeedFactor);
        animator.SetFloat("time",((musicBeat.timelineInfo.currentTime*0.001f)%(period))/period);
    }
}
