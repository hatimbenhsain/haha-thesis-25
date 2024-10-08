using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public Vector2 look;

    public bool movingForward;
    public bool movingBackward;
    public bool movingLeft;
    public bool movingRight;
    public bool movingUp;
    public bool movingDown;
    public bool boosting;
    public bool interacting;
    public bool advancing;

    public bool yAxisInverted=false;

    //[HideInInspector]
    public bool prevMovingForward, prevMovingBackward, prevMovingLeft, prevMovingRight, prevMovingUp, prevMovingDown, prevBoosting, prevInteracting, prevAdvancing;

    
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
        prevAdvancing=advancing;
    }

    public void OnMoveForward(InputAction.CallbackContext value){
        MoveForwardInput(value.performed || value.started);
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

    public void OnInteract(InputAction.CallbackContext value){
        InteractInput(value.performed || value.started);
    }

    public void OnAdvance(InputAction.CallbackContext value)
    {
        AdvanceInput(value.performed || value.started);
    }



    void MoveForwardInput(bool b){
        movingForward=b;
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
        look = newLookDirection;
        if(yAxisInverted){
            look.y=-look.y;
        }
    }

    void InteractInput(bool b){
        interacting=false;
    }

    void AdvanceInput(bool b)
    {
        advancing = b;
    }
}
