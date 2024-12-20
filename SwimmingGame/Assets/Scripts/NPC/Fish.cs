using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : NPCOverworld
{

    [Header("Fish Misc.")]
    private float closeToPlayerTimer=0f;
    [Tooltip("After this time start following player again")]
    public float closeToPlayerTime=2f;

    void Start(){

    }

    void Update(){
        float distanceFromPlayer=Vector3.Distance(transform.position,player.transform.position);
        closeToPlayerTimer=Mathf.Clamp(closeToPlayerTimer,0f,closeToPlayerTime);

        switch(movementBehavior){
            case MovementBehavior.FollowPlayer:
                if(distanceFromPlayer<=minDistanceFromPlayer){
                    closeToPlayerTimer+=Time.deltaTime;
                    if(closeToPlayerTimer>=closeToPlayerTime){
                        movementBehavior=MovementBehavior.Wander;
                        closeToPlayerTimer=0f;
                    }
                }else{
                    closeToPlayerTimer-=Time.deltaTime;
                }
                break;
            case MovementBehavior.Wander:
                if(distanceFromPlayer<=maxDistanceFromPlayer){
                    closeToPlayerTimer+=Time.deltaTime;
                    if(closeToPlayerTimer>=closeToPlayerTime){
                        movementBehavior=MovementBehavior.FollowPlayer;
                        closeToPlayerTimer=0f;
                    }
                }else{
                    closeToPlayerTimer-=Time.deltaTime;
                }
                break;

        }
    }


}
