using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBody : MonoBehaviour
{
    private TouchController touchController;
    public Transform bodyPos;
    public Transform neck;
    public float lerpSpeed; 
    public float neckLerpSpeed; 
    public float maxX;
    public float minX;
    public float maxY;
    public float minY;

    public float minNeckRotationX;
    public float maxNeckRotationX;
    public float minNeckRotationZ;
    public float maxNeckRotationZ;

    private Vector3 initialBodyTransform;
    private Vector3 initialNeckRotation;

    void Start()
    {
        touchController = FindObjectOfType<TouchController>();
        initialNeckRotation = neck.localEulerAngles;
        initialBodyTransform = bodyPos.position;
    }

    void FixedUpdate()
    {
        Vector2 moveXY = touchController.moveXZ;
        Vector3 targetPosition = new Vector3(
            Mathf.Clamp(bodyPos.position.x + moveXY.x, minX, maxX),
            Mathf.Clamp(bodyPos.position.y + moveXY.y, minY, maxY),
            bodyPos.position.z
        );

        bodyPos.position = Vector3.Lerp(bodyPos.position, targetPosition, lerpSpeed * Time.fixedDeltaTime);

        // Calculate the target rotation for the neck based on the position of bodyPos
        float targetRotationX = Mathf.Clamp((initialBodyTransform.y - transform.position.y) * 10f, minNeckRotationX, maxNeckRotationX);
        float targetRotationZ = Mathf.Clamp((initialBodyTransform.x - transform.position.x) * 10f, minNeckRotationZ, maxNeckRotationZ);

        // Apply the rotation to the neck using Quaternion
        Quaternion targetRotation = Quaternion.Euler(targetRotationX, neck.localEulerAngles.y, -targetRotationZ);
        neck.localRotation = Quaternion.Lerp(neck.localRotation, targetRotation, neckLerpSpeed * Time.fixedDeltaTime);

        /*
        // Debug statements
        Debug.Log("moveXY: " + moveXY);
        Debug.Log("targetRotationX: " + targetRotationX);
        Debug.Log("targetRotationZ: " + targetRotationZ);
        Debug.Log("targetRotation: " + targetRotation.eulerAngles);
        Debug.Log("neck.localRotation: " + neck.localRotation.eulerAngles);
        */
    }
}
