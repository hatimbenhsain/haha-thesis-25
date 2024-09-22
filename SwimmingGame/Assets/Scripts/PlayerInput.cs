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
    public bool boosting;

    public bool yAxisInverted=false;

    public void OnMoveForward(InputAction.CallbackContext value){
        MoveForwardInput(value.performed);
    }

    public void OnMoveBackward(InputAction.CallbackContext value){
        MoveBackwardInput(value.performed);
    }

    public void OnMoveLeft(InputAction.CallbackContext value){
        MoveLeftInput(value.performed);
    }

    public void OnMoveRight(InputAction.CallbackContext value){
        MoveRightInput(value.performed);
    }

    public void OnBoost(InputAction.CallbackContext value){
        BoostInput(value.performed);
    }

    public void OnLook(InputAction.CallbackContext value)
    {
        LookInput(value.ReadValue<Vector2>());
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

    void BoostInput(bool b){
        boosting=b;
    }

    public void LookInput(Vector2 newLookDirection){
        look = newLookDirection;
        if(yAxisInverted){
            look.y=-look.y;
        }
    }

}
