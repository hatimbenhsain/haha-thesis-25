using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECS_Rustler : MonoBehaviour
{
    static SwimmerSinging swimmerSinging;

    void Start(){
        swimmerSinging=FindObjectOfType<SwimmerSinging>();
    }

    void Update(){
        if(swimmerSinging.singing){
            foreach(ECS_CulledObject culledObject in ECS_CullSphere.GetObjectsInPlay()){
                ECS_RustlingThing rt=culledObject.GetComponentInChildren<ECS_RustlingThing>();
                if(rt!=null && Vector3.Distance(rt.transform.position,swimmerSinging.transform.position)<rt.minimumSingingDistance*Mathf.Pow(swimmerSinging.singingVolume,2f)){
                    Rustle(rt,false);
                }
            }
        }
    }

    static void Rustle(ECS_RustlingThing rustlingThing, bool playSound=true){
        if(playSound) Sound.Play3DOneShotVolume(rustlingThing.rustleSound,rustlingThing.volume,rustlingThing.transform,"",0,rustlingThing.pitch);
        rustlingThing.GetComponent<Animator>().SetTrigger("Rustle");
    }

    private void OnTriggerEnter(Collider other){
        ECS_RustlingThing rt=other.gameObject.GetComponentInChildren<ECS_RustlingThing>();
        if(rt!=null){
            Rustle(rt);
        }
    }
}
