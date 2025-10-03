using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECS_RustlingThing : MonoBehaviour 
{
    public RustlableData rustlableData;
    public SpriteRenderer spriteRenderer;

    public float timeToRustle;
    public int rustleState=0;
    public bool rustling=false;

    void Start(){
        spriteRenderer=GetComponentInChildren<SpriteRenderer>();
    }
}
