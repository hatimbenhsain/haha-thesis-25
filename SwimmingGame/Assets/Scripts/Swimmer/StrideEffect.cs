using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrideEffect : MonoBehaviour
{
    public Animator animator;
    public Swimmer swimmer;
    private bool orphaned=false;
    public float timeBeforeOrphaning=.5f;
    

    private Vector3 velocity;
    public float deceleration=1.5f;

    void Update()
    {
        if(!orphaned && animator.GetCurrentAnimatorStateInfo(0).normalizedTime*animator.GetCurrentAnimatorStateInfo(0).length>=timeBeforeOrphaning){
            orphaned=true;
            transform.parent=transform.parent.parent.parent;
            velocity=swimmer.GetVelocity();
        }

    }

    void FixedUpdate()
    {
        if(orphaned){
            Vector3 prevVelocity=velocity;

            Vector3 decelerationVector=velocity;
            decelerationVector=decelerationVector.normalized*deceleration*Time.fixedDeltaTime;
            velocity=velocity-=decelerationVector;

            if(velocity.x/Mathf.Abs(velocity.x)!=prevVelocity.x/Mathf.Abs(prevVelocity.x)){
                velocity.x=0;
            }
            if(velocity.y/Mathf.Abs(velocity.y)!=prevVelocity.y/Mathf.Abs(prevVelocity.y)){
                velocity.y=0;
            }
            if(velocity.z/Mathf.Abs(velocity.z)!=prevVelocity.z/Mathf.Abs(prevVelocity.z)){
                velocity.z=0;
            }

            transform.position+=velocity*Time.fixedDeltaTime;
        }
    }
}
