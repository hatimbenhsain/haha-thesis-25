using System;
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

    public float maximumDisplacement=1f;

    [Tooltip("Past this treshold of closeness to maximum position start selecting a choice.")]
    public float selectingTreshold=.5f;

    private CuddleDialogue dialogue;


    public List<int> optionsIndex; //Index of choices assigned to each movement
                        // 0 is right, 1 is up, 2 is left, 3 is down
    private int lastPickedChoiceIndex;

    void Start()
    {
        touchController = FindObjectOfType<TouchController>();
        initialNeckRotation = neck.localEulerAngles;
        initialBodyTransform = bodyPos.position;
        dialogue=FindObjectOfType<CuddleDialogue>();

        optionsIndex=new List<int>();
        optionsIndex.Add(0);
        optionsIndex.Add(1);
        optionsIndex.Add(2);
        lastPickedChoiceIndex=3;
    }

    void FixedUpdate()
    {
        Vector2 moveXY = touchController.moveXZ;
        moveXY.x=-moveXY.x;

        Vector3 targetPosition = new Vector3(
            Mathf.Clamp(bodyPos.position.x + moveXY.x, minX, maxX),
            Mathf.Clamp(bodyPos.position.y + moveXY.y, minY, maxY),
            bodyPos.position.z
        );

        Vector2 tempTargetPos=
            new Vector2(targetPosition.x,targetPosition.y)-new Vector2(minX+(maxX-minX)/2f,minY+(maxY-minY)/2f);
        tempTargetPos=Vector2.ClampMagnitude(tempTargetPos,maximumDisplacement);
        
        targetPosition.x=tempTargetPos.x+minX+(maxX-minX)/2f;
        targetPosition.y=tempTargetPos.y+minY+(maxY-minY)/2f;

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

    void Update()
    {
        float v=Mathf.Abs(bodyPos.position.x-minX)/Mathf.Abs(minX-maxX);
        if(v<=selectingTreshold){
            dialogue.HoveringChoice(optionsIndex.IndexOf(0));
            dialogue.SelectingChoice(optionsIndex.IndexOf(0),(selectingTreshold-v)/selectingTreshold);
        }
        v=Mathf.Abs(bodyPos.position.y-minY)/Mathf.Abs(minY-maxY);
        if(v<=selectingTreshold){
            dialogue.HoveringChoice(optionsIndex.IndexOf(1));
            dialogue.SelectingChoice(optionsIndex.IndexOf(1),(selectingTreshold-v)/selectingTreshold);
        }
        v=Mathf.Abs(bodyPos.position.x-maxX)/Mathf.Abs(minX-maxX);
        if(v<=selectingTreshold){
            dialogue.HoveringChoice(optionsIndex.IndexOf(2));
            dialogue.SelectingChoice(optionsIndex.IndexOf(2),(selectingTreshold-v)/selectingTreshold);
        }
        v=Mathf.Abs(bodyPos.position.y-maxY)/Mathf.Abs(minY-maxY);
        if(v<=selectingTreshold){
            dialogue.HoveringChoice(optionsIndex.IndexOf(3));
            dialogue.SelectingChoice(optionsIndex.IndexOf(3),(selectingTreshold-v)/selectingTreshold);
        }
    }

    public void PickedChoice(int index){
        optionsIndex.Insert(index,lastPickedChoiceIndex);
        lastPickedChoiceIndex=optionsIndex[index+1];
        optionsIndex.Remove(lastPickedChoiceIndex);
    }

}
