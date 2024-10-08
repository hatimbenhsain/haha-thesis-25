using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class PlayerInput : MonoBehaviour
{
    public Vector2 look;
    public Vector2 rotation;

    public bool movingForward;
    public float movingForwardValue;
    public bool movingBackward;
    public bool movingLeft;
    public bool movingRight;
    public bool movingUp;
    public bool movingDown;
    public bool boosting;
    public bool interacting;

    public bool yAxisInverted=false;

    //[HideInInspector]
    public bool prevMovingForward, prevMovingBackward, prevMovingLeft, prevMovingRight, prevMovingUp, prevMovingDown, prevBoosting, prevInteracting;

    public bool movedForwardTrigger;

    private UnityEngine.InputSystem.PlayerInput playerInput;

    public string currentControlScheme;

    [Tooltip("More sensitivity means more movement")]
    public float mouseSensitivity=50f; 

    void Start(){
        playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
    }

    void LateUpdate() {
        prevMovingForward=movingForward;
        prevMovingBackward=movingBackward;
        prevMovingUp=movingUp;
        prevMovingLeft=movingLeft;
        prevMovingRight=movingRight;
        prevMovingUp=movingUp;
        prevMovingDown=movingDown;
        prevBoosting=boosting;
        prevInteracting=interacting;
        currentControlScheme=playerInput.currentControlScheme;
    }

    public void OnMoveForward(InputAction.CallbackContext value){
        MoveForwardInput(value.ReadValue<float>());
        Debug.Log("on move forward");
        if(value.performed) Debug.Log("performed");
        if(value.started) Debug.Log("started");
        Debug.Log(value.ReadValue<float>());
    }

    public void OnMoveForwardTrigger(InputAction.CallbackContext value){
        MoveForwardTriggerInput(value.ReadValue<float>());
    }

    public void OnMoveBackward(InputAction.CallbackContext value){
        MoveBackwardInput(value.performed || value.started);
    }

    public void OnMoveLeft(InputAction.CallbackContext value){
        MoveLeftInput(value.performed || value.started);
    }

    public void OnMoveRight(InputAction.CallbackContext value){
        MoveRightInput(value.performed || value.started);
    }

    public void OnMoveUp(InputAction.CallbackContext value){
        MoveUpInput(value.performed || value.started);
    }

    public void OnMoveDown(InputAction.CallbackContext value){
        MoveDownInput(value.performed || value.started);
    }


    public void OnBoost(InputAction.CallbackContext value){
        BoostInput(value.performed || value.started);
    }

    public void OnLook(InputAction.CallbackContext value)
    {
        LookInput(value.ReadValue<Vector2>());
    }

    public void OnRotateCamera(InputAction.CallbackContext value){
        RotateCameraInput(value.ReadValue<Vector2>());
    }

    public void OnInteract(InputAction.CallbackContext value){
        InteractInput(value.performed || value.started);
    }



    void MoveForwardInput(float f){
        bool b;
        if(f>=0.2f){
            b=true;
        }else{
            b=false;
        }
        movingForwardValue=f;
        movingForward=b;
    }

    void MoveForwardTriggerInput(float f){
        if(f==1) movedForwardTrigger=true;
    }

    void MoveBackwardInput(bool b){
        movingBackward=b;
    }

    void MoveLeftInput(bool b){
        movingLeft=b;
    }

    void MoveRightInput(bool b){
        movingRight=b;
    }

    void MoveUpInput(bool b){
        movingUp=b;
    }

    void MoveDownInput(bool b){
        movingDown=b;
    }

    void BoostInput(bool b){
        boosting=b;
    }

    public void LookInput(Vector2 newLookDirection){
        look=newLookDirection;
        if(yAxisInverted){
            look.y=-look.y;
        }
    }

    public void RotateCameraInput(Vector2 newRotation){
        rotation=newRotation;
        if(yAxisInverted){
            rotation.y=-rotation.y;
        }
        if(currentControlScheme=="KeyboardMouse"){
            rotation=rotation*mouseSensitivity/5000f;
        }
    }

    void InteractInput(bool b){
        interacting=false;
    }

}
