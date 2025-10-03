using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECS_Rustler : MonoBehaviour
{
    static SwimmerSinging swimmerSinging;

    static ECS_Rustler rustler;

    private float timeSinceStart=0f;

    public float timeToRustle=.5f;

    void Start(){
        swimmerSinging=FindObjectOfType<SwimmerSinging>();
        rustler=this;
    }

    void Update(){
        if(swimmerSinging.singing){
            foreach(ECS_CulledObject culledObject in ECS_CullSphere.GetObjectsInPlay()){
                ECS_RustlingThing rt=culledObject.GetComponentInChildren<ECS_RustlingThing>();
                if(rt!=null && Vector3.Distance(rt.transform.position,swimmerSinging.transform.position)<rt.rustlableData.minimumSingingDistance*Mathf.Pow(swimmerSinging.singingVolume,2f)){
                    Rustle(rt,false);
                }
            }
        }
    }

    static void Rustle(ECS_RustlingThing rustlingThing, bool playSound=true){
        if(playSound) Sound.Play3DOneShotVolume(rustlingThing.rustlableData.rustleSound,rustlingThing.rustlableData.volume,rustlingThing.transform,"",0,rustlingThing.rustlableData.pitch);
        //rustlingThing.GetComponent<Animator>().SetTrigger("Rustle");
        rustlingThing.timeToRustle=rustler.timeToRustle;
        if(!rustlingThing.rustling){
            rustler.StartCoroutine(AnimateSprite(rustlingThing,0f));
            rustlingThing.rustling=true;
        }
    }

    private void OnTriggerEnter(Collider other){
        ECS_RustlingThing rt=other.gameObject.GetComponentInChildren<ECS_RustlingThing>();
        if(rt!=null){
            Rustle(rt);
        }
    }

    static IEnumerator AnimateSprite(ECS_RustlingThing rustlingThing, float waitTime){
        yield return new WaitForSeconds(waitTime);
        rustlingThing.timeToRustle-=waitTime;
        rustlingThing.rustleState++;
        rustlingThing.rustleState=rustlingThing.rustleState%rustlingThing.rustlableData.sprites.Length;
        rustlingThing.spriteRenderer.sprite=rustlingThing.rustlableData.sprites[rustlingThing.rustleState];
        if(rustlingThing.timeToRustle>0f) rustler.StartCoroutine(AnimateSprite(rustlingThing,rustler.timeToRustle*.5f/rustlingThing.rustlableData.sprites.Length));
        else{
            rustlingThing.rustling=false;
            rustlingThing.spriteRenderer.sprite=rustlingThing.rustlableData.sprites[0];
            rustlingThing.rustleState=rustlingThing.rustlableData.sprites.Length;
        }
    }
}
