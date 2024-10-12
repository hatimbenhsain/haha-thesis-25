using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimmerCamera : MonoBehaviour
{
    public Camera camera;
    public Transform target;

    private PlayerInput playerInput;
    private Swimmer swimmer;

    public float rotationSmoothTime = 1;  // Smoothing factor for the camera movement
    public float maxRotationationAngle=45;
    public float rotationSpeed=60f;
    [Tooltip("If there is no input from player for this long, camera moves back to player.")]
    public float pauseLength=2f;
    private float pauseTimer=0f;
    
    public float fovChangeSpeed=2f;
    public float fovRestoreSpeed=1f;
    public float targetFov=80f;
    [Tooltip("FOV to change to when boosting")]
    public float boostFov=90f;
    private float baseFov;
    private Vector3 targetRotation;
    private Vector3 rotationVelocity;

    void Start()
    {
        playerInput=FindObjectOfType<PlayerInput>();
        swimmer=GetComponent<Swimmer>();

        baseFov=camera.fieldOfView;
        targetFov=baseFov;
    }

    void Update()
    {
        pauseTimer+=Time.deltaTime;

        Vector3 input=new Vector3(playerInput.rotation.y,playerInput.rotation.x,0f);

        if(input.magnitude>=0.05f){
            pauseTimer=0f;
        }

        targetRotation+=input*Time.deltaTime*rotationSpeed;

        if(pauseTimer>=pauseLength){
            targetRotation=Vector3.zero;
        }

        targetRotation.x=Mathf.Clamp(targetRotation.x,-maxRotationationAngle,maxRotationationAngle);
        targetRotation.y=Mathf.Clamp(targetRotation.y,-maxRotationationAngle,maxRotationationAngle);

        //Inverting current rotation values if they go over 180
        Vector3 currentRotation=target.localRotation.eulerAngles;
        if(currentRotation.x>180f){
            currentRotation.x-=360;
        }
        if(currentRotation.y>180f){
            currentRotation.y-=360;
        }

        // Lerp the camera's rotation for a, heavier feel
        Vector3 newRotation=Vector3.SmoothDamp(currentRotation, targetRotation, 
        ref rotationVelocity, rotationSmoothTime);

        // Apply the smoothed rotation to the cameraRoot
        target.localRotation = Quaternion.Euler(newRotation);

        camera.fieldOfView=Mathf.Lerp(camera.fieldOfView,targetFov,fovChangeSpeed*Time.deltaTime);
        targetFov=Mathf.Lerp(targetFov,baseFov,fovRestoreSpeed*Time.deltaTime);
    }

    public void BoostAnimation(){
        targetFov=boostFov;
        //Plan to add panino effect and/or chromatic aberration
    }
}
