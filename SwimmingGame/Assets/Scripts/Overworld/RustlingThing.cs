using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RustlingThing : MonoBehaviour
{
    [Tooltip("FMOD Path sound for when player boops fish.")]
    public string rustleSound="event:/Overworld/Things/Rustle/";    
    private Animator animator;
    public float pitch=0f;

    private SwimmerSinging swimmerSinging;
    [Tooltip("Minimum distance from singer to rustle")]
    public float minimumSingingDistance=10f;

    private void Start() {
        animator = GetComponent<Animator>();
        swimmerSinging=FindObjectOfType<SwimmerSinging>();
    }

    private void Update() {
        if(swimmerSinging.singing && Vector3.Distance(transform.position,swimmerSinging.transform.position)<minimumSingingDistance*Mathf.Pow(swimmerSinging.singingVolume,2f)){
            Rustle(false);
        }
    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.tag=="Player"){
            Rustle();
        }
    }

    public void Rustle(bool playSound=true){
        if(playSound) Sound.Play3DOneShotVolume(rustleSound,1f,transform,"",0,pitch);
        animator.SetTrigger("Rustle");
    }
}
