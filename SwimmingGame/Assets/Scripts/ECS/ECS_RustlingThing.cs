using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECS_RustlingThing : MonoBehaviour 
{
    [Tooltip("FMOD Path sound for when player boops fish.")]
    public string rustleSound="event:/Overworld/Things/Rustle/";
    public float pitch=0f;
    public float volume=1f;

    [Tooltip("Minimum distance from singer to rustle")]
    public float minimumSingingDistance=10f;
}
