using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCOverworld : MonoBehaviour
{
    public Transform pathParent;
    public PathNode[] path;
    public int pathIndex=0;    //Index of current path node
    public float pauseTimer=0f; //Timer for when NPC pauses at a "Pause" node

    private Rigidbody body;
    private Swimmer player;
    
    [Header("Behavior")]
        public NPCStates currentState;
        public MovementBehavior movementBehavior;
        public bool waitForPlayer=false;
        [Tooltip("Max distance between self and player before pausing.")]
        public float maxDistanceFromPlayer=10f;
        [Tooltip("If npc has stopped, Player has to get this close to start again.")]
        public float minDistanceFromPlayer=8f;
        [Tooltip("How often NPC makes a swimming stroke in seconds.")]
        public float strokeFrequency=1f;
        public bool loopingPath=true;
        


    [Header("Movement")]
        public float acceleration=4f;
        public float maxVelocity=10f;
        [Tooltip("Deceleration always takes effect")]
        public float deceleration=1.5f;
        [Tooltip("Instant speed gain at the end of a stroke")]
        public float boostSpeed=1.5f;
        [Tooltip("Swimmer maintains coasting speed when not boosting")]
        public float coastingSpeed=2f;  //maintains coasting speed when not boosting

        [Tooltip("Timer for before boost takes effect")]
        public float boostTimer=0f;
        [Tooltip("Total time it takes before boost takes effect")]
        public float boostTime=0.3f;     //time before boost takes effect
        [Tooltip("Max speed gained from kicking wall")]
        public float rotationAcceleration=120f;
        public float rotationMaxVelocity=90f;
        private Vector3 rotationVelocity=Vector3.zero;
        public float rotationDeceleration=60;
        [Tooltip("Distance between self and path node before moving on to the next node")]
        public float maxNodeDistance=0.1f;
        [Tooltip("Factor to multiply anti-collision vector to mess with bounce. Usually 1.")]
        public float collisionSpeedFactor=1f;
        [Tooltip("Min reaction speed when reacting with anything even if the relative velocity is 0.")]
        public float minCollisionSpeed=0.01f;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private bool pausing=false;
    private bool currentlyWaitingForPlayer=false;

    ArrayList allHits=new ArrayList();


    void Start()
    {
        body=GetComponent<Rigidbody>();
        player=FindObjectOfType<Swimmer>();
        FindPath();
        //Changing path's parent so it doesn't move with self
        pathParent.parent=transform.parent;
    }

    void FixedUpdate()
    {
        switch(currentState){
            case NPCStates.Swimming:
                Swim();
                break;
            case NPCStates.SwimmingAndSinging:
                Swim();
                break;
        }
    }

    void Swim(){
        pausing=false;
        GetTarget();
        if(waitForPlayer && Vector3.Distance(transform.position,player.transform.position)>=maxDistanceFromPlayer){
            currentlyWaitingForPlayer=true;
        }
        if(currentlyWaitingForPlayer){
            pausing=true;
            boostTimer=0f;
            if(Vector3.Distance(transform.position,player.transform.position)<minDistanceFromPlayer){
                currentlyWaitingForPlayer=false;
            }
        }
        bool movingForward=!pausing;
        bool boosting=false;

        //Rotation Things

        // //Deceleration of rotation speed
        // Vector3 antiRotationVector=rotationVelocity;
        // Vector3 prevRotationVelocity=rotationVelocity;
        // antiRotationVector=antiRotationVector.normalized*rotationDeceleration*Time.fixedDeltaTime;

        // rotationVelocity=rotationVelocity-=antiRotationVector;

        // Vector3 rotationDifference=targetRotation.eulerAngles-body.rotation.eulerAngles;    //QUESTIONABLE part, look at this again later
        // rotationDifference=Vector3.ClampMagnitude(rotationDifference,rotationAcceleration);

        // //Cancel rotation deceleration if it goes past the direction
        // if(rotationDifference.magnitude<=1f){
        //     if(rotationVelocity.x/Mathf.Abs(rotationVelocity.x)!=prevRotationVelocity.x/Mathf.Abs(prevRotationVelocity.x)){
        //         rotationVelocity.x=0;
        //     }
        //     if(rotationVelocity.y/Mathf.Abs(rotationVelocity.y)!=prevRotationVelocity.y/Mathf.Abs(prevRotationVelocity.y)){
        //         rotationVelocity.y=0;
        //     }
        //     if(rotationVelocity.z/Mathf.Abs(rotationVelocity.z)!=prevRotationVelocity.z/Mathf.Abs(prevRotationVelocity.z)){
        //         rotationVelocity.z=0;
        //     }
        // }

        // rotationVelocity+=new Vector3(rotationDifference.x,rotationDifference.y,rotationDifference.z)*Time.fixedDeltaTime;
        // rotationVelocity=Vector3.ClampMagnitude(rotationVelocity,rotationMaxVelocity);

        // //Finding rotation to do
        // Vector3 newRotation=body.rotation.eulerAngles;
        // newRotation+=rotationVelocity*Time.fixedDeltaTime;

        //Quaternion newRotationQ=Quaternion.Euler(newRotation);
        Quaternion newRotationQ=Quaternion.Lerp(transform.rotation,targetRotation,rotationMaxVelocity*Time.fixedDeltaTime);
        //Rotating self
        body.MoveRotation(newRotationQ);

        //Translation things

        Vector3 currentVelocity=new Vector3(body.velocity.x,body.velocity.y,body.velocity.z);
        Vector3 velocity=currentVelocity;

        //Deceleration
        Vector3 decelerationVector=currentVelocity;
        decelerationVector=decelerationVector.normalized*deceleration*Time.fixedDeltaTime;

        velocity=velocity-decelerationVector;        

        //Cancel deceleration if it goes past the direction
        if(!movingForward){
            if(velocity.x/Mathf.Abs(velocity.x)!=currentVelocity.x/Mathf.Abs(currentVelocity.x)){
                velocity.x=0;
            }
            if(velocity.y/Mathf.Abs(velocity.y)!=currentVelocity.y/Mathf.Abs(currentVelocity.y)){
                velocity.y=0;
            }
            if(velocity.z/Mathf.Abs(velocity.z)!=currentVelocity.z/Mathf.Abs(currentVelocity.z)){
                velocity.z=0;
            }
        }

        Vector3 positionDifference=targetPosition-body.position;    //For now I'm not using this for anything

        //THIS IS WHERE COLLISION CODE WOULD HAPPEN; FOR NOW WE'RE IGNORING IT

                //Deal with collisions
        //The way this works: find normal of every collision, project the player's velocity on it, move the player by that much
        ArrayList collisionImpulses=new ArrayList();
        foreach (ContactPoint hit in allHits)
        {
            Vector3 collisionPoint=hit.point;
            Vector3 normal=hit.normal;
            Vector3 relativeVelocity;
            if(hit.otherCollider.TryGetComponent<Rigidbody>(out Rigidbody otherBody)){
                relativeVelocity=otherBody.velocity;
            }
            else{
                relativeVelocity=Vector3.zero;
            }
            relativeVelocity=relativeVelocity-velocity;
            Vector3 force=normal*relativeVelocity.magnitude*collisionSpeedFactor;
            //Projection of relative velocity on normal
            force=Vector3.Project(relativeVelocity,normal)*collisionSpeedFactor;
            if(Vector3.Angle(force,normal)>=90){
                force=Vector3.zero;
            }
            if(force==Vector3.zero){
                force+=normal*minCollisionSpeed;
            }

            //Dividing force by number of contacts
            force=force/allHits.Count;

            collisionImpulses.Add(force);

        }

        allHits.Clear();

        foreach(Vector3 force in collisionImpulses){
            velocity+=force;
        }

        boostTimer+=Time.fixedDeltaTime;

        //Boosting player velocity at the end of the swimstroke
        if(boostTimer>boostTime && boostTimer-Time.fixedDeltaTime<=boostTime){
            if(Quaternion.Angle(transform.rotation,targetRotation)<=45f){
                velocity+=transform.forward*boostSpeed;
            }
            else{
                boostTimer-=Time.fixedDeltaTime;
            }
            //BoostAnimation();
        }

        if(boostTimer>=strokeFrequency){
            boostTimer=0f;
        }

        //Adding velocity from swimming
        if(movingForward){
            if(velocity.magnitude<coastingSpeed || Vector3.Angle(velocity,transform.forward)>=90f){
                //Limiting speed if difference between current direction & target is too big
                float modifier=1-(Mathf.Clamp(Quaternion.Angle(transform.rotation,targetRotation)-25f,0f,45f)/45f);
                velocity+=transform.forward*acceleration*Time.fixedDeltaTime*modifier;
            }
            //animator.SetBool("swimmingForward",true);
        }else{
            //animator.SetBool("swimmingForward",false);
            boostTimer=boostTime+1f;
        }

        velocity=Vector3.ClampMagnitude(velocity,maxVelocity);
        // Using MovePosition because it interpolates movement smoothly and keeps velocity in next frame
        body.MovePosition(body.position+velocity*Time.fixedDeltaTime);
        //animator.SetFloat("speed",velocity.magnitude);

    }

    //Get target position and rotation depending on behavior
    void GetTarget(){
        switch(movementBehavior){
            case MovementBehavior.FollowPath:
                if(Vector3.Distance(targetPosition,transform.position)<maxNodeDistance && path[pathIndex].type==PathNodeType.Continue){
                    pathIndex+=1;
                    pauseTimer=0f;
                }else if(Vector3.Distance(targetPosition,transform.position)<maxNodeDistance && path[pathIndex].type==PathNodeType.Pause){
                    pausing=true;
                    pauseTimer+=Time.fixedDeltaTime;
                    if(pauseTimer>=path[pathIndex].pauseLength){
                        pauseTimer+=1;
                        pathIndex+=1;
                    }
                }
                if(loopingPath){    //Wrap around path if NPC is looping
                    pathIndex=pathIndex%path.Length;
                }else if(pathIndex>=path.Length){   //If not looping and reached destination NPC becomes idle
                    currentState=NPCStates.Idle;
                    return;
                }
                targetPosition=path[pathIndex].transform.position;
                targetRotation=Quaternion.LookRotation(targetPosition-transform.position,Vector3.up);

                Debug.DrawRay(transform.position,targetRotation*Vector3.forward,Color.green,3f);
                break;
        }
    }

    // Get path objects from parent
    void FindPath(){
        path=new PathNode[pathParent.childCount];
        for(int i=0;i<pathParent.childCount;i++){
            Transform t=pathParent.GetChild(i);
            PathNode pn;
            if(!t.gameObject.TryGetComponent<PathNode>(out pn)){
                pn=t.gameObject.AddComponent<PathNode>();
                pn.type=PathNodeType.Continue;
            }
            path[i]=pn;
        }
        if(path.Length>0){
            targetPosition=path[pathIndex].transform.position;
            targetRotation=Quaternion.LookRotation(targetPosition-transform.position);
        }
    }

    private void OnCollisionStay(Collision other) {
    if(other.collider.gameObject.tag!="Player"){
        ContactPoint[] myContacts = new ContactPoint[other.contactCount];
        for(int i = 0; i < myContacts.Length; i++)
        {
            myContacts[i] = other.GetContact(i);
            allHits.Add(myContacts[i]);
        }
    }
    }
}

public enum NPCStates{
    Swimming,
    Singing,
    SwimmingAndSinging,
    Idle
}

public enum MovementBehavior{
    FollowPath, //NPC follow a predetermined path
    //Possible other behaviors: go around player, random, etc.
}