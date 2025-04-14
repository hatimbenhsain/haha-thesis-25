using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuddleCameraManager : MonoBehaviour
{
    public Camera[] cameras; 
    public GameObject[] bodyPos;
    public GameObject[] handPos;
    public GameObject[] handControlPos;
    public GameObject[] colliderViews;
    public GameObject[] sexPartnerColliders;
    public GameObject[] backPlane;
    public BoxCollider[] boundingBoxes;
    public GameObject[] planes;
    public bool[] sexPartnerAnimationSwitchPose;
    public Quaternion[] handRotationOffset;
    public Transform sexPartnerBody;
    public Transform hand;
    public Transform handControlPoint;
    public TouchController handController;
    public Animator sexPartnerAnimator;
    public bool isApplyingHeadBob;

    [Header("Head Bob Settings")]
    public float bobSpeed = 2f;      
    public float bobAmount = 0.05f;  
    public float defaultCameraY;    
    private float bobTimer;

    public int shotIndex;
    private int prevShotIndex;
    private Animator blink;
    private float blinkDuration;
    void Start()
    {
        SetActiveElements(shotIndex);
        defaultCameraY = cameras[shotIndex].transform.localPosition.y; // Store initial Y position
        UpdatePosition(sexPartnerBody, bodyPos);
        UpdatePosition(hand, handPos);
        UpdatePosition(handControlPoint, handControlPos);
        handController.boundingBox = boundingBoxes[shotIndex];
        handController.rotationOffset = handRotationOffset[shotIndex];
        blink = GetComponentInChildren<Animator>();
        blinkDuration = 0f;

    }

    void FixedUpdate()
    {
        // when change shots
        if (shotIndex != prevShotIndex)
        {
            
            blink.SetBool("Blink", true); // Trigger blink animation
            blinkDuration += Time.deltaTime;
            Debug.Log("Blink Duration: " + blinkDuration);
            if (blinkDuration >= 0.2f) 
            {
                                handController.lockRotation = true;
                SetActiveElements(shotIndex);
                defaultCameraY = cameras[shotIndex].transform.localPosition.y; // Reset Y position for new active camera
                UpdatePosition(sexPartnerBody, bodyPos);
                UpdatePosition(hand, handPos);
                UpdatePosition(handControlPoint, handControlPos);
                handController.boundingBox = boundingBoxes[shotIndex];
                handController.rotationOffset = handRotationOffset[shotIndex];
                if (sexPartnerAnimator != null){
                    sexPartnerAnimator.SetBool("SwitchPose", sexPartnerAnimationSwitchPose[shotIndex]);
                }
                foreach (GameObject cv in colliderViews){
                    cv.SetActive(false);
                }
                colliderViews[shotIndex].SetActive(true);
                handController.lockRotation = false;
                blinkDuration = 0f;
                blink.SetBool("Blink", false);
                prevShotIndex = shotIndex;
            }

        }
        if (isApplyingHeadBob){
            ApplyHeadBob();
        }

    }

    public void SetActiveElements(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(i == index); // Enable the active camera, disable others
            planes[i].gameObject.SetActive(i == index); // Enable the active plane, disable others
            boundingBoxes[i].gameObject.SetActive(i == index); // Enable the active bounding box, disable others
            sexPartnerColliders[i].gameObject.SetActive(i == index); // Enable
            backPlane[i].gameObject.SetActive(i == index); // Enable the active background plane, disable others
        }
        // Reset bob timer when switching cameras
        bobTimer = 0f;
        defaultCameraY = cameras[index].transform.position.y; // Reset Y position for new active camera
    }



void ApplyHeadBob()
{
    if (cameras[shotIndex].gameObject.activeSelf)
    {
        // Record the current local position of the active camera
        Vector3 currentLocalPosition = cameras[shotIndex].transform.localPosition;

        // Apply head bob effect to the local position
        bobTimer += Time.fixedDeltaTime * bobSpeed;
        float newY = defaultCameraY + Mathf.Sin(bobTimer) * bobAmount; // Calculate new Y position based on sine wave
        currentLocalPosition.y = newY;
        cameras[shotIndex].transform.localPosition = currentLocalPosition;
    }
}

    void UpdatePosition(Transform transform, GameObject[] transformObject)
    {
        transform.position = transformObject[shotIndex].transform.position;
        transform.rotation = transformObject[shotIndex].transform.rotation;
        //update initial rotation in hand controller
        handController.initialRotation = transform.rotation;
    }
}
