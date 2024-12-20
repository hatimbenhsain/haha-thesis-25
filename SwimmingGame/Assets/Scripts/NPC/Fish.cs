using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Fish : NPCOverworld
{

    [Header("Fish Misc.")]
    private float closeToPlayerTimer=0f;
    private float awayFromPlayerTimer=0f;
    [Tooltip("After this time start following player again")]
    public float followToWanderTime=2f;
    public float wanderToFollowShortTime=0.5f;
    public float wanderToFollowLongTime=5f;
    public float awayFromPlayerTime=5f;
    public float wanderToRunTime=1f;

    private SwimmerSinging swimmerSinging;

    void Start(){
        swimmerSinging=FindObjectOfType<SwimmerSinging>();
    }

    void Update(){
        float distanceFromPlayer=Vector3.Distance(transform.position,player.transform.position);

        switch(movementBehavior){
            case MovementBehavior.FollowPlayer:
                if(distanceFromPlayer<=maxDistanceFromPlayer){
                    closeToPlayerTimer+=Time.deltaTime;
                    if(closeToPlayerTimer>=followToWanderTime){
                        ChangeMovementBehavior(MovementBehavior.Wander);
                        closeToPlayerTimer=0f;
                        awayFromPlayerTimer=0f;
                        break;
                    }
                }else{
                    closeToPlayerTimer-=Time.deltaTime;
                }
                if(distanceFromPlayer>=minDistanceFromPlayer){
                    awayFromPlayerTimer+=Time.deltaTime;
                    if(awayFromPlayerTimer>=awayFromPlayerTime){
                        ChangeMovementBehavior(MovementBehavior.Wander);
                        pastMovementBehavior=MovementBehavior.Wander;
                        awayFromPlayerTimer=0f;
                        closeToPlayerTimer=0f;
                    }
                }
                break;
            case MovementBehavior.Wander:
                if(distanceFromPlayer<=minDistanceFromPlayer){
                    closeToPlayerTimer+=Time.deltaTime;
                    if(swimmerSinging.singing){
                        closeToPlayerTimer+=Time.deltaTime;
                    }
                    if(pastMovementBehavior==MovementBehavior.FollowPlayer && closeToPlayerTimer>=wanderToFollowShortTime){
                        ChangeMovementBehavior(MovementBehavior.FollowPlayer);
                        closeToPlayerTimer=0f;
                        break;
                    }else if(closeToPlayerTimer>=wanderToFollowLongTime){
                        ChangeMovementBehavior(MovementBehavior.FollowPlayer);
                        closeToPlayerTimer=0f;
                        break;
                    }
                }else{
                    closeToPlayerTimer-=Time.deltaTime;
                }
                if(distanceFromPlayer<=maxDistanceFromPlayer){
                    closeToPlayerTimer+=Time.deltaTime;
                    if(closeToPlayerTimer>wanderToRunTime){
                        ChangeMovementBehavior(MovementBehavior.RunFromPlayer);
                        awayFromPlayerTimer=0f;
                        closeToPlayerTimer=0f;
                    }
                }
                break;
            case MovementBehavior.RunFromPlayer:
                if((pastMovementBehavior==MovementBehavior.FollowLeader || 
                pastMovementBehavior==MovementBehavior.Wander) && distanceFromPlayer<=maxDistanceFromPlayer){
                    awayFromPlayerTime+=Time.deltaTime;
                }
                if(distanceFromPlayer>=minDistanceFromPlayer){
                    awayFromPlayerTimer+=Time.deltaTime;
                    if(swimmerSinging.singing){
                        awayFromPlayerTimer-=Time.deltaTime;
                    }
                }
                if(awayFromPlayerTimer>=awayFromPlayerTime){
                    if(pastMovementBehavior==MovementBehavior.FollowLeader){
                        ChangeMovementBehavior(MovementBehavior.FollowLeader);
                    }else{
                        ChangeMovementBehavior(MovementBehavior.Wander);
                    }
                    awayFromPlayerTimer=0f;
                    closeToPlayerTimer=0f;
                }                
                break;
        }


        closeToPlayerTimer=Mathf.Max(closeToPlayerTimer,0f);
        awayFromPlayerTimer=Mathf.Max(awayFromPlayerTimer,0f);
    }

    void OnCollisionEnter(Collision other){
        if(other.gameObject.tag=="Player"){
            ChangeMovementBehavior(MovementBehavior.RunFromPlayer);
            awayFromPlayerTimer=0f;
            closeToPlayerTimer=0f;
        }
    }


}
