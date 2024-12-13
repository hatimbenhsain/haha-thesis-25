using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class PlayerInput : MonoBehaviour
{
    public Vector2 look;
    public Vector2 rotation;
    public Vector2 singingNote;

    public bool movingForward;
    public float movingForwardValue;
    public bool movingBackward;
    public bool movingLeft;
    public bool movingRight;
    public bool movingUp;
    public bool movingDown;
    public bool boosting;
    public bool interacting;
    public bool aiming;
    public bool singing;

    public bool navigateLeft;
    public bool navigateRight;
    public bool navigateUp;
    public bool navigateDown;

    public bool pausing;
    public bool entering;


    [HideInInspector]
    public object[] values;

    public bool yAxisInverted=false;

    //[HideInInspector]
    public bool prevMovingForward, prevMovingBackward, prevMovingLeft, prevMovingRight, prevMovingUp, prevMovingDown, prevBoosting, prevInteracting, prevAiming, prevSinging, prevNavigateLeft, prevNavigateRight, prevNavigateUp, prevNavigateDown, prevPausing, prevEntering;

    public bool movedForwardTrigger;

    private UnityEngine.InputSystem.PlayerInput playerInput;

    public string currentControlScheme;

    [Tooltip("More sensitivity means more movement")]
    public float mouseSensitivity=50f; 

    void Start(){
        playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();

        values=new object[] {look.x,look.y,rotation.x,rotation.y,singingNote.x,singingNote.y,movingForward,movingBackward,movingLeft,movingRight,movingUp,movingDown,boosting,interacting,aiming,singing,pausing,entering};
    }

    void Update(){
        values=new object[] {look.x,look.y,rotation.x,rotation.y,singingNote.x,singingNote.y,movingForward,movingBackward,movingLeft,movingRight,movingUp,movingDown,boosting,interacting,aiming,singing,pausing,entering};
        if(Input.GetKeyDown(KeyCode.I)){
            yAxisInverted=!yAxisInverted;
        }
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
        prevAiming=aiming;
        prevSinging=singing;
        currentControlScheme=playerInput.currentControlScheme;
        prevNavigateDown=navigateDown;
        prevNavigateLeft=navigateLeft;
        prevNavigateRight=navigateRight;
        prevNavigateUp=navigateUp;
        prevPausing=pausing;
        prevEntering=entering;
    }

    public void OnMoveForward(InputAction.CallbackContext value){
        MoveForwardInput(value.ReadValue<float>());
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

    public void OnSing(InputAction.CallbackContext value){
        SingInput(value.performed || value.started);
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

    public void OnAim(InputAction.CallbackContext value)
    {
        AimInput(value.performed || value.started);
    }

    public void OnSingingNote(InputAction.CallbackContext value){
        SingingNoteInput(value.ReadValue<Vector2>());
    }

    public void OnNavigateLeft(InputAction.CallbackContext value){
        NavigateLeftInput(value.ReadValue<float>() < 0.8f ? false : true);
    }

    public void OnNavigateRight(InputAction.CallbackContext value){
        NavigateRightInput(value.ReadValue<float>() < 0.8f ? false : true);
    }

    public void OnNavigateUp(InputAction.CallbackContext value){
        NavigateUpInput(value.ReadValue<float>() < 0.8f ? false : true);
    }

    public void OnNavigateDown(InputAction.CallbackContext value){
        NavigateDownInput(value.ReadValue<float>() < 0.8f ? false : true);
    }

    public void OnPause(InputAction.CallbackContext value){
        PauseInput(value.performed || value.started);
    }

    public void OnEnter(InputAction.CallbackContext value){
        EnterInput(value.performed || value.started);
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

    void SingInput(bool b){
        singing=b;
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
        interacting=b;
    }

    void AimInput(bool b)
    {
        aiming = b;
    }

    void SingingNoteInput(Vector2 newSingNote){
        singingNote=newSingNote;
        if(currentControlScheme=="KeyboardMouse"){
            singingNote=singingNote*mouseSensitivity/5000f;
        }
    }

    void NavigateLeftInput(bool b){
        navigateLeft=b;
    }

    void NavigateRightInput(bool b){
        navigateRight=b;
    }

    void NavigateUpInput(bool b){
        navigateUp=b;
    }

    void NavigateDownInput(bool b){
        navigateDown=b;
    }

    public void SwitchMap(string actionMap){
        playerInput.SwitchCurrentActionMap(actionMap);
    }

    public void RestoreDefaultMap(){
        playerInput.SwitchCurrentActionMap(playerInput.defaultActionMap);
    }

    void PauseInput(bool b){
        pausing=b;
    }

    void EnterInput(bool b){
        entering=b;
    }
}
