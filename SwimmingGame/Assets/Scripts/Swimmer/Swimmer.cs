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
    [Tooltip("Max speed gained from kicking wall")]
    public float wallBoost=1.5f;
    [Tooltip("Number to multiply the capsule radius when checking for walls")]
    public float wallCheckCapsuleRadiusMultiplier=1.5f;

    private CharacterController controller;
    private Rigidbody body;
    private PlayerInput playerInput;
    private SwimmerCamera swimmerCamera;

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

    [Header("Misc.")]
    public GameObject dustCloudPrefab;
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
        swimmerCamera=GetComponent<SwimmerCamera>();

        Cursor.lockState = CursorLockMode.Locked;  // Locks the cursor to the center of the screen


    }

    void Update(){
        if(playerInput.movedForwardTrigger){
            boostTimer=0f;
            animator.SetTrigger("boostForward");
            playerInput.movedForwardTrigger=false;
            forcesToAdd+=CheckForWallAndKick(Vector3.forward);
        }else if(playerInput.movingBackward && !playerInput.prevMovingBackward){
            forcesToAdd+=CheckForWallAndKick(-Vector3.forward);
        }
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

            //Dividing force by number of contacts
            force=force/allHits.Count;

            collisionImpulses.Add(force);

            //playerVelocity+=force;
            float rotationAmount=minCollisionRotationAmount+(maxCollisionRotationAmount-minCollisionRotationAmount)*
            Mathf.Clamp((force.magnitude-minCollisionForceToRotate)/(maxCollisionForceToRotate-minCollisionForceToRotate),0f,1f);
            newRotationQ=Quaternion.RotateTowards(newRotationQ,Quaternion.LookRotation(force),rotationAmount);

            //Attempt at changing rotation speed instead of immediately swerving MAYBE TRY THIS AGAIN
            // Vector3 rv=Quaternion.FromToRotation(body.rotation.eulerAngles,force).eulerAngles;
            // rv=rv.normalized*rv.magnitude*rotationAmount;
            // rotationVelocity+=rv;
        }

        foreach(Vector3 force in collisionImpulses){
            playerVelocity+=force;
        }

        //Friction
        ArrayList frictionImpulses=new ArrayList();
        for(int i=0;i<allHits.Count;i++){
            ContactPoint hit=(ContactPoint)allHits[i];
            PhysicsObject physicsObject;
            if(hit.otherCollider.gameObject.TryGetComponent<PhysicsObject>(out physicsObject)){
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
                Vector3 force=relativeVelocity-Vector3.Project(relativeVelocity,normal)*collisionSpeedFactor;

                force=Vector3.ClampMagnitude(force,((Vector3)collisionImpulses[i]).magnitude*physicsObject.friction);
                Debug.Log("friction");
                Debug.Log(force);

                frictionImpulses.Add(force);
                Debug.DrawRay(body.position,force*3f, Color.red, 1f);
            }
        }

        foreach(Vector3 force in frictionImpulses){
            playerVelocity+=force;
        }


        //Rotating player
        body.MoveRotation(newRotationQ);


        boostTimer+=Time.fixedDeltaTime;

        //Boosting player velocity at the end of the swimstroke
        if(boostTimer>boostTime && boostTimer-Time.fixedDeltaTime<=boostTime){
            playerVelocity+=transform.forward*boostSpeed;
            BoostAnimation();
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

    // Boost can be from external effect for e.g. ring
    public void Boost(Vector3 force){
        forcesToAdd+=force;
    }

    //Things to animate right after boost happens
    void BoostAnimation(){
        swimmerCamera.BoostAnimation();
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

        LayerMask mask = LayerMask.GetMask("Default","Wall");

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

    private Vector3 CheckForWallAndKick(Vector3 direction){
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
        float radius=capsule.radius*wallCheckCapsuleRadiusMultiplier;
        Vector3 point1=body.position+capsule.center+dir*(capsule.height/2f-radius);
        Vector3 point2=body.position+capsule.center-dir*(capsule.height/2f-radius);
        RaycastHit[] hits;

        LayerMask mask = LayerMask.GetMask("Wall");

        Vector3 forward=transform.rotation*direction;

        hits=Physics.CapsuleCastAll(point1,point2,radius+skinWidth,-forward,0f,mask,QueryTriggerInteraction.Ignore);

        Vector3 totalForce=Vector3.zero;

        GameObject dustCloud=null;

        foreach(RaycastHit hit in hits){
            if(hit.collider.gameObject.tag!="Player"){
                RaycastHit hit2=CreateRaycastHitFromCollider(body.position+capsule.center,hit.collider);
                Vector3 normal=hit2.normal;
                if(Vector3.Angle(normal,forward)<90f){
                    Vector3 force=Vector3.Project(forward,normal)*wallBoost;
                    force=forward*force.magnitude;
                    Debug.DrawRay(body.position,normal*3f, Color.magenta, 3f);
                    Debug.DrawRay(body.position,force*3f, Color.green, 3f);
                    totalForce+=force;
                    Debug.Log("found wall");
                    Debug.Log(normal);
                    Debug.Log(hit.collider.gameObject);
                    if(dustCloud==null){
                        dustCloud=Instantiate(dustCloudPrefab,transform.parent);
                        dustCloud.transform.position=hit2.point+normal*0.2f;
                    }
                }
            }
        }

        totalForce=Vector3.ClampMagnitude(totalForce,wallBoost);

        Debug.Log(totalForce);
        Debug.Log(totalForce.magnitude);

        return totalForce;
    }

    // Function borrowed from https://discussions.unity.com/t/how-do-i-obtain-the-surface-normal-for-a-point-on-a-collider-cant-use-raycasthit-normal/16223/4
    public static RaycastHit CreateRaycastHitFromCollider(Vector3 _rayOrigin, Collider _collider)
    {
        var colliderTr = _collider.transform;

        // Returns a point on the given collider that is closest to the specified location.
        // Note that in case the specified location is inside the collider, or exactly on the boundary of it, the input location is returned instead.
        // The collider can only be BoxCollider, SphereCollider, CapsuleCollider or a convex MeshCollider.
        var closestPoint = Physics.ClosestPoint(_rayOrigin, _collider, colliderTr.position, colliderTr.rotation);
  
        if (_collider is MeshCollider {convex: false} meshCollider)
        {
            Debug.LogWarning($"do not use convex mesh-colliders as it does not deal well with physics at all. " +
                             $"There are solutions provided in the asset store to automatically transform non-convex meshes to convex meshes. The problematic mesh: {_collider.transform} meshName:{meshCollider.sharedMesh.name}");
            // This is not great. If we have complex meshColliders we will encounter issues.
            closestPoint = _collider.ClosestPointOnBounds(_rayOrigin);
        }
        
        var dir = (closestPoint - _rayOrigin).normalized;
        var ray = new Ray(_rayOrigin, dir);
        var hasHit = _collider.Raycast(ray, out var hitInfo, float.MaxValue);
        
        if (hasHit == false)
        {
            Debug.LogError($"This case will never happen!");
        }

        return hitInfo;
    }

    public Vector3 GetVelocity(){
        return body.velocity;
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