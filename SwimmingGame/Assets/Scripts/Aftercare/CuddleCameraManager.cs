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
    public BoxCollider[] boundingBoxes;
    public GameObject[] planes;
    public bool[] sexPartnerAnimationSwitchPose;
    public Quaternion[] handRotationOffset;
    public Transform sexPartnerBody;
    public Transform hand;
    public Transform handControlPoint;
    public TouchController handController;
    public Animator sexPartnerAnimator;

    [Header("Head Bob Settings")]
    public float bobSpeed = 2f;      
    public float bobAmount = 0.05f;  
    private float defaultCameraY;    
    private float bobTimer;

    public int shotIndex;
    private int prevShotIndex;

    void Start()
    {
        SetActiveCamera(shotIndex);
        defaultCameraY = cameras[shotIndex].transform.localPosition.y; // Store initial Y position
        UpdatePosition(sexPartnerBody, bodyPos);
        UpdatePosition(hand, handPos);
        UpdatePosition(handControlPoint, handControlPos);
        SetActivePlane(shotIndex);
        SetActiveBoundingBox(shotIndex);
        SetActiveSexPartnerColliders(shotIndex);
        handController.boundingBox = boundingBoxes[shotIndex];
        handController.rotationOffset = handRotationOffset[shotIndex];
        sexPartnerAnimator.SetBool("SwitchPose", sexPartnerAnimationSwitchPose[shotIndex]);
    }

    void FixedUpdate()
    {
        if (shotIndex != prevShotIndex)
        {
            handController.lockRotation = true;
            SetActiveCamera(shotIndex);
            defaultCameraY = cameras[shotIndex].transform.localPosition.y; // Reset Y position for new active camera
            UpdatePosition(sexPartnerBody, bodyPos);
            UpdatePosition(hand, handPos);
            UpdatePosition(handControlPoint, handControlPos);
            SetActivePlane(shotIndex);
            SetActiveBoundingBox(shotIndex);
            SetActiveSexPartnerColliders(shotIndex);
            handController.boundingBox = boundingBoxes[shotIndex];
            handController.rotationOffset = handRotationOffset[shotIndex];
            sexPartnerAnimator.SetBool("SwitchPose", sexPartnerAnimationSwitchPose[shotIndex]);
            foreach (GameObject cv in colliderViews){
                cv.SetActive(false);
            }
            colliderViews[shotIndex].SetActive(true);
            handController.lockRotation = false;
        }
        ApplyHeadBob(); 
        prevShotIndex = shotIndex;
    }

    public void SetActiveCamera(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(i == index); // Enable the active camera, disable others
        }
        // Reset bob timer when switching cameras
        bobTimer = 0f;
    }

    public void SetActivePlane(int index)
    {
        for (int i = 0; i < planes.Length; i++)
        {
            planes[i].gameObject.SetActive(i == index); // Enable the active plane, disable others
        }
    }
    public void SetActiveBoundingBox(int index)
    {
        for (int i = 0; i < boundingBoxes.Length; i++)
        {
            boundingBoxes[i].gameObject.SetActive(i == index); // Enable the active bounding box, disable others
        }
    }

    public void SetActiveSexPartnerColliders(int index) 
    {
        for (int i = 0; i < sexPartnerColliders.Length; i++)
        {
            sexPartnerColliders[i].gameObject.SetActive(i == index); // Enable the active sex partner collider, disable others
        }
    }

    void ApplyHeadBob()
    {
        if (cameras[shotIndex].gameObject.activeSelf)
        {
            bobTimer += Time.fixedDeltaTime * bobSpeed;
            float newY = defaultCameraY + Mathf.Sin(bobTimer) * bobAmount; // Calculate new Y position based on sine wave
            Vector3 newPosition = cameras[shotIndex].transform.localPosition;
            newPosition.y = newY;
            cameras[shotIndex].transform.localPosition = newPosition;
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
