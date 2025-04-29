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
    public LightBeamFollow lightBeamFollow;

    void Start()
    {
        if (cameras != null && cameras.Length > shotIndex)
            defaultCameraY = cameras[shotIndex].transform.localPosition.y; // Store initial Y position

        if (bodyPos != null)
            UpdatePosition(sexPartnerBody, bodyPos);

        if (handPos != null)
            UpdatePosition(hand, handPos);

        if (handControlPos != null)
            UpdatePosition(handControlPoint, handControlPos);

        if (boundingBoxes != null && boundingBoxes.Length > shotIndex)
            handController.boundingBox = boundingBoxes[shotIndex];

        if (handRotationOffset != null && handRotationOffset.Length > shotIndex)
            handController.rotationOffset = handRotationOffset[shotIndex];

        blink = GetComponentInChildren<Animator>();
        blinkDuration = 0f;
    }

    void FixedUpdate()
    {
        if (shotIndex != prevShotIndex)
        {
            if (blink != null)
                blink.SetBool("Blink", true); // Trigger blink animation

            blinkDuration += Time.deltaTime;
            Debug.Log("Blink Duration: " + blinkDuration);

            if (blinkDuration >= 0.1f)
            {
                handController.lockRotation = true;

                SetActiveElements(shotIndex);

                if (cameras != null && cameras.Length > shotIndex)
                    defaultCameraY = cameras[shotIndex].transform.localPosition.y; // Reset Y position for new active camera

                if (bodyPos != null)
                    UpdatePosition(sexPartnerBody, bodyPos);

                if (handPos != null)
                    UpdatePosition(hand, handPos);

                if (handControlPos != null)
                    UpdatePosition(handControlPoint, handControlPos);

                if (boundingBoxes != null && boundingBoxes.Length > shotIndex)
                    handController.boundingBox = boundingBoxes[shotIndex];

                if (handRotationOffset != null && handRotationOffset.Length > shotIndex)
                    handController.rotationOffset = handRotationOffset[shotIndex];

                if (sexPartnerAnimator != null && sexPartnerAnimationSwitchPose != null && sexPartnerAnimationSwitchPose.Length > shotIndex)
                    sexPartnerAnimator.SetBool("SwitchPose", sexPartnerAnimationSwitchPose[shotIndex]);

                if (colliderViews != null)
                {
                    foreach (GameObject cv in colliderViews)
                    {
                        if (cv != null)
                            cv.SetActive(false);
                    }

                    if (colliderViews.Length > shotIndex && colliderViews[shotIndex] != null)
                        colliderViews[shotIndex].SetActive(true);
                }

                handController.lockRotation = false;

                blinkDuration = 0f;

                if (blink != null)
                    blink.SetBool("Blink", false);

                prevShotIndex = shotIndex;
            }
        }

        if (isApplyingHeadBob)
        {
            ApplyHeadBob();
        }
    }

    public void SetActiveElements(int index)
    {
        if (cameras != null)
        {
            for (int i = 0; i < cameras.Length; i++)
            {
                if (cameras[i] != null)
                    cameras[i].gameObject.SetActive(i == index); // Enable the active camera, disable others
            }
        }

        if (planes != null)
        {
            for (int i = 0; i < planes.Length; i++)
            {
                if (planes[i] != null)
                    planes[i].gameObject.SetActive(i == index); // Enable the active plane, disable others
            }
        }

        if (boundingBoxes != null)
        {
            for (int i = 0; i < boundingBoxes.Length; i++)
            {
                if (boundingBoxes[i] != null)
                    boundingBoxes[i].gameObject.SetActive(i == index); // Enable the active bounding box, disable others
            }
        }

        if (sexPartnerColliders != null)
        {
            for (int i = 0; i < sexPartnerColliders.Length; i++)
            {
                if (sexPartnerColliders[i] != null)
                    sexPartnerColliders[i].gameObject.SetActive(i == index); // Enable
            }
        }

        if (backPlane != null)
        {
            for (int i = 0; i < backPlane.Length; i++)
            {
                if (backPlane[i] != null)
                    backPlane[i].gameObject.SetActive(i == index); // Enable the active background plane, disable others
            }
        }

        // Reset bob timer when switching cameras
        bobTimer = 0f;

        if (cameras != null && cameras.Length > index && cameras[index] != null)
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
        if (transformObject != null && transformObject.Length > shotIndex && transformObject[shotIndex] != null)
        {
            transform.position = transformObject[shotIndex].transform.position;
            transform.rotation = transformObject[shotIndex].transform.rotation;

            // Update initial rotation in hand controller
            handController.initialRotation = transform.rotation;
        }
    }
}
