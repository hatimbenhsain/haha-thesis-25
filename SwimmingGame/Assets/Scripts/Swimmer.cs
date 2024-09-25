using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

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

    [Header("Misc")]

    [Tooltip("Sometimes when colliding with something the character goes flying off. We use this value to limit the speed resulting.")]
    public float maxCollisionSpeed=4f;

    private bool justCollided=false;
    private Vector3 collisionVelocity;
    private Vector3 prevVelocity;

    private int collisionNumber=0;





    void Start()
    {
        controller=GetComponent<CharacterController>();
        playerInput=GetComponent<PlayerInput>();
        animator=GetComponent<Animator>();
    }

    void Update()
    {
        Move();
    }

    void Move(){
        //Deceleration of rotation speed
        Vector3 antiRotationVector=rotationVelocity;
        Vector3 prevRotationVelocity=rotationVelocity;
        antiRotationVector=antiRotationVector.normalized*rotationDeceleration*Time.deltaTime;

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
            rotationVelocity+=new Vector3(playerInput.look.y,playerInput.look.x,0f)*rotationAcceleration*Time.deltaTime;
            rotationVelocity=Vector3.ClampMagnitude(rotationVelocity,rotationMaxVelocity);
        }else{//Deceleration of rotation speed

        }

        //Rotating player
        Vector3 newRotation=transform.rotation.eulerAngles;
        newRotation+=rotationVelocity*Time.deltaTime;
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
        newRotation.z=Mathf.Lerp(newRotation.z,targetRotationZ,Time.deltaTime*angleTiltSpeed);

        transform.rotation=Quaternion.Euler(newRotation);

        Vector3 currentVelocity=new Vector3(controller.velocity.x,controller.velocity.y,controller.velocity.z);
        Vector3 playerVelocity=currentVelocity;

        //Deceleration
        Vector3 decelerationVector=currentVelocity;
        decelerationVector=decelerationVector.normalized*deceleration*Time.deltaTime;

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

        boostTimer+=Time.deltaTime;

        //Checking if the player just collided and went flying off => dampen speed
        if(justCollided){
            
            Debug.Log("just collided");
            if(collisionNumber==1){
                Debug.Log(collisionVelocity);
                Debug.Log(prevVelocity);
            }
            Vector3 velocityChange=playerVelocity-collisionVelocity;
            if(velocityChange.magnitude>=maxCollisionSpeed && Vector3.Angle(collisionVelocity,playerVelocity)>=45){
                Debug.Log("big change");
            //     //playerVelocity=Vector3.zero;

                
                //playerVelocity=collisionVelocity+Vector3.ClampMagnitude(velocityChange,maxCollisionSpeed);
                playerVelocity=collisionVelocity+velocityChange.normalized*(maxCollisionSpeed+Mathf.Sqrt(velocityChange.magnitude-maxCollisionSpeed+1)-1);
            }
            Debug.Log(collisionNumber);
            
            Debug.Log(playerVelocity);
            justCollided=false;
        }else{
            collisionNumber=0;
        }

        //Boosting player velocity at the end of the brushstroke
        if(boostTimer>boostTime && boostTimer-Time.deltaTime<=boostTime){
            playerVelocity+=transform.forward*boostSpeed;
        }

        //Adding velocity
        if(playerInput.movingForward && !playerInput.movingBackward){
            if(playerVelocity.magnitude<coastingSpeed || Vector3.Angle(playerVelocity,transform.forward)>=90f){
                playerVelocity+=transform.forward*acceleration*Time.deltaTime;
            }
            animator.SetBool("swimmingForward",true);
            if(!playerInput.prevMovingForward){
                boostTimer=0f;
                animator.SetTrigger("boostForward");
            }
        }else if(playerInput.movingBackward && !playerInput.movingForward){
            if(playerVelocity.magnitude<coastingSpeed || Vector3.Angle(playerVelocity,-transform.forward)>=90f){
                playerVelocity+=-transform.forward*backwardAcceleration*Time.deltaTime;
            }
            animator.SetBool("swimmingBackward",true);
            boostTimer=boostTime+1f;
        }else{
            animator.SetBool("swimmingForward",false);
            animator.SetBool("swimmingBackward",false);
            boostTimer=boostTime+1f;
        }

        //Lateral movement
        if(playerInput.movingLeft && !playerInput.movingRight && Mathf.Abs((transform.rotation*playerVelocity).x)<lateralMaxVelocity){
            playerVelocity+=-transform.right*lateralAcceleration*Time.deltaTime;
        }else if(playerInput.movingRight && !playerInput.movingLeft && Mathf.Abs((transform.rotation*playerVelocity).x)<lateralMaxVelocity){
            playerVelocity+=transform.right*lateralAcceleration*Time.deltaTime;
        }

        //Vertical movement
        if(playerInput.movingUp && !playerInput.movingDown && Mathf.Abs((transform.rotation*playerVelocity).y)<lateralMaxVelocity){
            playerVelocity+=transform.up*lateralAcceleration*Time.deltaTime;
        }else if(playerInput.movingDown && !playerInput.movingUp && Mathf.Abs((transform.rotation*playerVelocity).y)<lateralMaxVelocity){
            playerVelocity+=-transform.up*lateralAcceleration*Time.deltaTime;
        }

        playerVelocity=Vector3.ClampMagnitude(playerVelocity,maxVelocity);

        controller.Move(playerVelocity*Time.deltaTime); 

        prevVelocity=controller.velocity;

    }

    public void Swim(){

    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        justCollided=true;
        collisionVelocity=controller.velocity;
        collisionNumber+=1;
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