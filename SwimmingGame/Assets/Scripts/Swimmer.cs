using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swimmer : MonoBehaviour
{
    public float acceleration=1f;
    public float maxVelocity=10f;
    public float deceleration=5f;
    public float lateralAcceleration=1f;
    public float lateralMaxVelocity=5f;

    private CharacterController controller;
    private PlayerInput playerInput;

    public float rotationAcceleration=180f;
    public float rotationMaxVelocity=180f;
    private Vector3 rotationVelocity=Vector3.zero;
    public float rotationDeceleration=180f;
    public float maxRotationXAngle=60f;

    void Start()
    {
        controller=GetComponent<CharacterController>();
        playerInput=GetComponent<PlayerInput>();
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
        transform.rotation=Quaternion.Euler(newRotation);

        Vector3 currentVelocity=new Vector3(controller.velocity.x,controller.velocity.y,controller.velocity.z);
        Vector3 playerVelocity=currentVelocity;

        //Deceleration
        Vector3 decelerationVector=currentVelocity;
        decelerationVector=decelerationVector.normalized*deceleration*Time.deltaTime;

        playerVelocity=playerVelocity-=decelerationVector;

        //Cancel deceleration if it goes past the direction
        if(!playerInput.movingForward && !playerInput.movingBackward){
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

        //Adding velocity
        if(playerInput.movingForward && !playerInput.movingBackward){
            playerVelocity+=transform.forward*acceleration*Time.deltaTime;
        }else if(playerInput.movingBackward && !playerInput.movingForward){
            playerVelocity+=-transform.forward*acceleration*Time.deltaTime;
        }

        //Lateral movement
        if(playerInput.movingLeft && !playerInput.movingRight && Mathf.Abs((transform.rotation*playerVelocity).x)<lateralMaxVelocity){
            playerVelocity+=-transform.right*lateralAcceleration*Time.deltaTime;
        }else if(playerInput.movingRight && !playerInput.movingLeft && Mathf.Abs((transform.rotation*playerVelocity).x)<lateralMaxVelocity){
            playerVelocity+=transform.right*lateralAcceleration*Time.deltaTime;
        }

        playerVelocity=Vector3.ClampMagnitude(playerVelocity,maxVelocity);

        controller.Move(playerVelocity*Time.deltaTime); 
    }

    public void Swim(){

    }
}


/*
TO-DO:
    + MAKE LATERAL/FORWARD MOVEMENT ONLY ONE AT A TIME
    + ANIMATE
    + ADD BREAST STROKE/COASTING
    + ADD BOOST
*/