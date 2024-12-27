using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RustlingThing : MonoBehaviour
{
    [Tooltip("FMOD Path sound for when player boops fish.")]
    public string rustleSound="event:/Overworld/Things/Rustle/";    
    private Animator animator;
    public float pitch=0f;

    private void Start() {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.tag=="Player"){
            Sound.Play3DOneShotVolume(rustleSound,1f,transform,"",0,pitch);
            animator.SetTrigger("Rustle");
        }
    }
}
