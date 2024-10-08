using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Timeline;

public class Swimmer : MonoBehaviour
{
    [Header("Movement")]
    public float acceleration=1f;
    public float backwardAcceleration=1f;
    public float maxVelocity=10f;
    [Tooltip("Deceleration always takes effect")]
    public float deceleration=5f;
    public float lateralAcceleration=1f;
    public float lateralMaxVelocity=5f;

    [Tooltip("Instant speed gain at the end of a stroke")]
    public float boostSpeed=1f;
    [Tooltip("Swimmer maintains coasting speed when not boosting")]
    public float coastingSpeed=3f;  //maintains coasting speed when not boosting

    [Tooltip("Timer for before boost takes effect")]
    public float boostTimer=0f;
    [Tooltip("Total time it takes before boost takes effect")]
    public float boostTime=1f;     //time before boost takes effect

    private CharacterController controller;
    private Rigidbody body;
    private PlayerInput playerInput;

    [Header("Rotation")]
    public float rotationAcceleration=180f;
    public float rotationMaxVelocity=180f;
    private Vector3 rotationVelocity=Vector3.zero;
    public float rotationDeceleration=180f;
    public float maxRotationXAngle=60f;

    public float maxTiltAngle=10f; //max angle to tilt when moving laterally
    public float angleTiltSpeed=1f; //speed to tile when moving laterally

    private Animator animator;

    [Header("Collision")]
    [Tooltip("Factor to multiply anti-collision vector to mess with bounce. Usually 1.")]
    public float collisionSpeedFactor=1f;
    [Tooltip("Min reaction speed when reacting with anything even if the relative velocity is 0.")]
    public float minCollisionSpeed=0.01f;
    [Tooltip("Min distance to move player out of object it's clipping into.")]
    public float minMoveoutStep=0.001f;
    [Tooltip("How many maximum moveout steps can the player perform every fixedupdate.")]
    public int maxMoveoutEffort=100;
    [Tooltip("Distance to keep object away from any collider.")]
    public float skinWidth=0.05f;
    [Tooltip("Min angle to rotate player when colliding with something.")]
    public float minCollisionRotationAmount=0.1f;
    [Tooltip("Max angle to rotate player when colliding with something.")]
    public float maxCollisionRotationAmount=1f;
    [Tooltip("Minimum force of collision to make player rotate.")]
    public float minCollisionForceToRotate=0.01f;
    [Tooltip("Cap force of collision to make player rotate.")]
    public float maxCollisionForceToRotate=5f;

    [Header("Camera")]
    public float cameraRotationSmoothTime = 0.1f;  // Smoothing factor for the camera movement
    public float maxCameraRotationAngle=60f;
    public float cameraRotationSpeed=100f;
    [Tooltip("If there is no input from player for this long, camera moves back to player.")]
    public float cameraPauseLength=2f;
    private float cameraPauseTimer=0f;

    [Header("Misc.")]
    public Transform cameraTarget;
    
    
    //Camera things
    private Vector3 targetRotation;
    private Vector3 cameraRotationVelocity;


    private Vector3 prevVelocity;

    ArrayList allHits=new ArrayList();

    private CapsuleCollider capsule;


    private Vector3 forcesToAdd; //Forces to add at the beginning of the next frame; this is used for e.g. for ring boosts


    void Start()
    {
        controller=GetComponent<CharacterController>();
        playerInput=FindObjectOfType<PlayerInput>();
        animator=GetComponent<Animator>();
        body=GetComponent<Rigidbody>();
        capsule=GetComponentInChildren<CapsuleCollider>();

        Cursor.lockState = CursorLockMode.Locked;  // Locks the cursor to the center of the screen

    }

    void Update(){
        if(playerInput.movedForwardTrigger){
            boostTimer=0f;
            animator.SetTrigger("boostForward");
            playerInput.movedForwardTrigger=false;
        }
        Camera();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move(){
        Vector3 currentVelocity=new Vector3(body.velocity.x,body.velocity.y,body.velocity.z);
        Vector3 playerVelocity=currentVelocity;

        //Deceleration of rotation speed
        Vector3 antiRotationVector=rotationVelocity;
        Vector3 prevRotationVelocity=rotationVelocity;
        antiRotationVector=antiRotationVector.normalized*rotationDeceleration*Time.fixedDeltaTime;

        rotationVelocity=rotationVelocity-=antiRotationVector;

        //Cancel rotation deceleration if it goes past the direction
        if(playerInput.look==Vector2.zero){
            if(rotationVelocity.x/Mathf.Abs(rotationVelocity.x)!=prevRotationVelocity.x/Mathf.Abs(prevRotationVelocity.x)){
                rotationVelocity.x=0;
            }
            if(rotationVelocity.y/Mathf.Abs(rotationVelocity.y)!=prevRotationVelocity.y/Mathf.Abs(prevRotationVelocity.y)){
                rotationVelocity.y=0;
            }
            if(rotationVelocity.z/Mathf.Abs(rotationVelocity.z)!=prevRotationVelocity.z/Mathf.Abs(prevRotationVelocity.z)){
                rotationVelocity.z=0;
            }
        }

        if(playerInput.look!=Vector2.zero){//Setting rotation speed
            rotationVelocity+=new Vector3(playerInput.look.y,playerInput.look.x,0f)*rotationAcceleration*Time.fixedDeltaTime;
            rotationVelocity=Vector3.ClampMagnitude(rotationVelocity,rotationMaxVelocity);
        }

        //Finding rotation to do
        Vector3 newRotation=body.rotation.eulerAngles;
        newRotation+=rotationVelocity*Time.fixedDeltaTime;
        //Clamping x rotation
        if(newRotation.x<180f){
            newRotation.x=Mathf.Clamp(newRotation.x,-maxRotationXAngle,maxRotationXAngle);
        }else{
            newRotation.x=Mathf.Clamp(newRotation.x,360f-maxRotationXAngle,360f+maxRotationXAngle);
        }

        //Tilt player if moving laterally
        float targetRotationZ=0f;
        if(playerInput.movingLeft && !playerInput.movingRight){
            targetRotationZ=maxTiltAngle;
        }else if(playerInput.movingRight && !playerInput.movingLeft){
            targetRotationZ=-maxTiltAngle;
        }
        if(newRotation.z>=180f){
            targetRotationZ=360f+targetRotationZ;
        }
        newRotation.z=Mathf.Lerp(newRotation.z,targetRotationZ,Time.fixedDeltaTime*angleTiltSpeed);

        Quaternion newRotationQ=Quaternion.Euler(newRotation);

        //Deceleration
        Vector3 decelerationVector=currentVelocity;
        decelerationVector=decelerationVector.normalized*deceleration*Time.fixedDeltaTime;

        playerVelocity=playerVelocity-=decelerationVector;

        //Cancel deceleration if it goes past the direction
        if(!playerInput.movingForward && !playerInput.movingBackward && !playerInput.movingLeft && !playerInput.movingRight && !playerInput.movingUp && !playerInput.movingUp){
            if(playerVelocity.x/Mathf.Abs(playerVelocity.x)!=currentVelocity.x/Mathf.Abs(currentVelocity.x)){
                playerVelocity.x=0;
            }
            if(playerVelocity.y/Mathf.Abs(playerVelocity.y)!=currentVelocity.y/Mathf.Abs(currentVelocity.y)){
                playerVelocity.y=0;
            }
            if(playerVelocity.z/Mathf.Abs(playerVelocity.z)!=currentVelocity.z/Mathf.Abs(currentVelocity.z)){
                playerVelocity.z=0;
            }
        }

        //Deal with collisions
        //The way this works: find normal of every collision, project the player's velocity on it, move the player by that much
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
            relativeVelocity=relativeVelocity-playerVelocity;
            Vector3 force=normal*relativeVelocity.magnitude*collisionSpeedFactor;
            //Projection of relative velocity on normal
            force=Vector3.Project(relativeVelocity,normal)*collisionSpeedFactor;
            if(Vector3.Angle(force,normal)>=90){
                force=Vector3.zero;
            }
            if(force==Vector3.zero){
                force+=normal*minCollisionSpeed;
            }
            playerVelocity+=force;
            float rotationAmount=minCollisionRotationAmount+(maxCollisionRotationAmount-minCollisionRotationAmount)*
            Mathf.Clamp((force.magnitude-minCollisionForceToRotate)/(maxCollisionForceToRotate-minCollisionForceToRotate),0f,1f);
            newRotationQ=Quaternion.RotateTowards(newRotationQ,Quaternion.LookRotation(force),rotationAmount);

            //Attempt at changing rotation speed instead of immediately swerving MAYBE TRY THIS AGAIN
            // Vector3 rv=Quaternion.FromToRotation(body.rotation.eulerAngles,force).eulerAngles;
            // rv=rv.normalized*rv.magnitude*rotationAmount;
            // rotationVelocity+=rv;
        }

        //Rotating player
        body.MoveRotation(newRotationQ);


        boostTimer+=Time.fixedDeltaTime;

        //Boosting player velocity at the end of the swimstroke
        if(boostTimer>boostTime && boostTimer-Time.fixedDeltaTime<=boostTime){
            playerVelocity+=transform.forward*boostSpeed;
        }

        //Adding external forces, for e.g. from ring booster
        playerVelocity+=forcesToAdd;
        forcesToAdd=Vector3.zero;

        //Adding velocity from swimming
        if(playerInput.movingForward && !playerInput.movingBackward){
            if(playerVelocity.magnitude<coastingSpeed || Vector3.Angle(playerVelocity,transform.forward)>=90f){
                playerVelocity+=transform.forward*acceleration*playerInput.movingForwardValue*Time.fixedDeltaTime;
            }
            animator.SetBool("swimmingForward",true);
        }else if(playerInput.movingBackward && !playerInput.movingForward){
            if(playerVelocity.magnitude<coastingSpeed || Vector3.Angle(playerVelocity,-transform.forward)>=90f){
                playerVelocity+=-transform.forward*backwardAcceleration*Time.fixedDeltaTime;
            }
            animator.SetBool("swimmingBackward",true);
            boostTimer=boostTime+1f;
        }else{
            animator.SetBool("swimmingForward",false);
            animator.SetBool("swimmingBackward",false);
            boostTimer=boostTime+1f;
        }

        //Lateral movement
        if(playerInput.movingLeft && !playerInput.movingRight && Mathf.Abs((body.rotation*playerVelocity).x)<lateralMaxVelocity){
            playerVelocity+=-transform.right*lateralAcceleration*Time.fixedDeltaTime;
        }else if(playerInput.movingRight && !playerInput.movingLeft && Mathf.Abs((body.rotation*playerVelocity).x)<lateralMaxVelocity){
            playerVelocity+=transform.right*lateralAcceleration*Time.fixedDeltaTime;
        }

        //Vertical movement
        if(playerInput.movingUp && !playerInput.movingDown && Mathf.Abs((body.rotation*playerVelocity).y)<lateralMaxVelocity){
            playerVelocity+=transform.up*lateralAcceleration*Time.deltaTime;
        }else if(playerInput.movingDown && !playerInput.movingUp && Mathf.Abs((body.rotation*playerVelocity).y)<lateralMaxVelocity){
            playerVelocity+=-transform.up*lateralAcceleration*Time.fixedDeltaTime;
        }

        playerVelocity=Vector3.ClampMagnitude(playerVelocity,maxVelocity);

        // Deal with collisions part 2 (in case of clipping)
        Vector3 collisionMovement=Vector3.zero;
        foreach (ContactPoint hit in allHits)
        {
            collisionMovement+=MoveOutOf(hit.otherCollider,transform.position+playerVelocity*Time.fixedDeltaTime,hit.normal);
        }

        // Anti-clip movement outside of MovePosition so it doesn't affect the velocity 
        body.position+=collisionMovement;
        // Using MovePosition because it interpolates movement smoothly and keeps velocity in next frame
        body.MovePosition(body.position+playerVelocity*Time.fixedDeltaTime);
        animator.SetFloat("speed",playerVelocity.magnitude);

        prevVelocity=body.velocity;

        allHits.Clear();

    }

    public void Boost(Vector3 force){
        forcesToAdd+=force;
    }

    void Camera(){
        cameraPauseTimer+=Time.deltaTime;

        Vector3 input=new Vector3(playerInput.rotation.y,playerInput.rotation.x,0f);

        if(input.magnitude>=0.05f){
            cameraPauseTimer=0f;
        }

        targetRotation+=input*Time.deltaTime*cameraRotationSpeed;

        if(cameraPauseTimer>=cameraPauseLength){
            targetRotation=Vector3.zero;
        }

        targetRotation.x=Mathf.Clamp(targetRotation.x,-maxCameraRotationAngle,maxCameraRotationAngle);
        targetRotation.y=Mathf.Clamp(targetRotation.y,-maxCameraRotationAngle,maxCameraRotationAngle);

        //Inverting current rotation values if they go over 180
        Vector3 currentRotation=cameraTarget.localRotation.eulerAngles;
        if(currentRotation.x>180f){
            currentRotation.x-=360;
        }
        if(currentRotation.y>180f){
            currentRotation.y-=360;
        }

        // Lerp the camera's rotation for a, heavier feel
        Vector3 newRotation=Vector3.SmoothDamp(currentRotation, targetRotation, 
        ref cameraRotationVelocity, cameraRotationSmoothTime);

        // Apply the smoothed rotation to the cameraRoot
        cameraTarget.localRotation = Quaternion.Euler(newRotation);

    }


    private void OnCollisionStay(Collision other) {
        ContactPoint[] myContacts = new ContactPoint[other.contactCount];
        for(int i = 0; i < myContacts.Length; i++)
        {
            myContacts[i] = other.GetContact(i);
            allHits.Add(myContacts[i]);
        }
    }

    //This function is for movement of player out of a collider without moving too fast
    //It doesn't move the player but it returns the Vector3 to move it by
    public Vector3 MoveOutOf(Collider other, Vector3 pos, Vector3 normal){
        Vector3 prevVelocity=body.velocity;

        Vector3 dir=new Vector3();
        switch(capsule.direction){
            case 0:
                dir=transform.right;
                break;
            case 1:
                dir=transform.up;
                break;
            case 2:
                dir=transform.forward;
                break;
        }
        Vector3 point1=pos+capsule.center+dir*(capsule.height/2f-capsule.radius);
        Vector3 point2=pos+capsule.center-dir*(capsule.height/2f-capsule.radius);
        RaycastHit[] hits;

        bool colliding=true;
        int tries=0;
        Vector3 movement=Vector3.zero; //Amount to move the collider 

        LayerMask mask = LayerMask.GetMask("Default");

        Debug.DrawRay(point1,point2-point1, Color.red, 1f);

        while(colliding && tries<maxMoveoutEffort){
            hits=Physics.CapsuleCastAll(point1+movement,point2+movement,capsule.radius+skinWidth,normal,0f,mask,QueryTriggerInteraction.Ignore);
            colliding=false;
            foreach(RaycastHit hit in hits){
                if(hit.collider==other){
                    colliding=true;
                    movement+=normal*minMoveoutStep;
                    break;
                }
            }
            tries++;
        }

        return movement;

    }
}


/*
TO-DO:
    + MAKE LATERAL/FORWARD MOVEMENT ONLY ONE AT A TIME
    + ADD BOOST
    + EXPRIMENT WITH LERP INSTEAD OF LINEAR DECELERATION
    + LOOK INTO PHYSICS SNAPPING
    + MAKE CAMERA COLLISIONS work
*/

/* REMOVED BUT COULD STILL BE USEFUL CODE:

Dampening post-collision speed:
        //Checking if the player just collided and went flying off => dampen speed
        
        [Tooltip("Sometimes when colliding with something the character goes flying off. We use this value to limit the speed resulting.")]
        public float maxCollisionSpeed=4f;

        if(justCollided){
            
            if(collisionNumber==1){

            }
            Vector3 velocityChange=playerVelocity-collisionVelocity;
            if(velocityChange.magnitude>=maxCollisionSpeed && Vector3.Angle(collisionVelocity,playerVelocity)>=45){
            //     //playerVelocity=Vector3.zero;

                
                //playerVelocity=collisionVelocity+Vector3.ClampMagnitude(velocityChange,maxCollisionSpeed);
                playerVelocity=collisionVelocity+velocityChange.normalized*(maxCollisionSpeed+Mathf.Sqrt(velocityChange.magnitude-maxCollisionSpeed+1)-1);
            }
            justCollided=false;
        }else{
            collisionNumber=0;
        }

*/