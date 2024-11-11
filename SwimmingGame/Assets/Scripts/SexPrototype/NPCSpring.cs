using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class NPCSpring : SexSpring
{
    [Header("NPC Movement Parameters")]
    public MovementBehavior movementBehavior;
    [Tooltip("Intensity determines movement speed and frequency etc.")]
    public int currentIntensity=0;
    public float timeBetweenBreaths=1f;
    [Tooltip("Min distance before stopping to move")]
    public float minDistanceFromPlayer=1f;
    [Tooltip("Max distance before stopping to move")]
    public float maxDistanceFromPlayer=5f;
    [Tooltip("Time to inhale for.")]
    public float inhaleLength=1f;

    public SpringMovementValues[] springMovementValues;


    private Transform player;
    private Vector3 targetLocation;

    private MovementBehavior prevMovementBehavior;

    void Start()
    {
        player=GameObject.FindWithTag("Player").transform;
        SpringStart();
    }

    void Update()
    {
        SpringUpdate();
    }

    void FixedUpdate()
    {
        //HandleTurning(playerInput.look);
        if(prevMovementBehavior!=movementBehavior){
            GetMovementValues();
        }
        Swim();

        SpringFixedUpdate();

        prevMovementBehavior=movementBehavior;
    }

    //Get movement parameters depending on current behavior type and intensity
    void GetMovementValues(){
        bool foundValues=false;
        foreach(SpringMovementValues values in springMovementValues){
            if(values.movementBehavior==movementBehavior && currentIntensity==values.intensity){
                CopyValues(values);
                foundValues=true;
                break;
            }
        }
        if(!foundValues){
            int intensityFound=-1;
            SpringMovementValues valuesFound=new SpringMovementValues();
            bool valuesWereFound=false;
            foreach(SpringMovementValues values in springMovementValues){
                if(values.movementBehavior==movementBehavior){
                    if(intensityFound==-1){
                        valuesFound=values;
                        intensityFound=values.intensity;
                    }else{
                        if(Mathf.Abs(currentIntensity-values.intensity)<Mathf.Abs(currentIntensity-intensityFound)){
                            valuesFound=values;
                            intensityFound=values.intensity;
                        }
                    }    
                    valuesWereFound=true;
                }
            }
            if(valuesWereFound){
                CopyValues(valuesFound);
            }
        }
    }

    void CopyValues(SpringMovementValues values){
        timeBetweenBreaths=values.timeBetweenBreaths;
        minDistanceFromPlayer=values.minDistanceFromPlayer;
        maxDistanceFromPlayer=values.maxDistanceFromPlayer;
        inhaleLength=values.inhaleLength;
        turnSpeed=values.turnSpeed;
    }

    //Handle movement depending on current movement behavior
    void Swim(){
        float inhaleTime=Time.time-inhaleStartTime;
        float distanceFromPlayer=Vector3.Distance(player.transform.position,characterRb.position);

        switch(movementBehavior){
            case MovementBehavior.FollowPlayer:
                targetLocation=player.position;
                if(!isExhaling){
                    TurnTowards(targetLocation,turnSpeed);
                }
                if(!isInhaling && !isExhaling){
                    if(timeSinceExhale>=timeBetweenBreaths && distanceFromPlayer>=minDistanceFromPlayer){
                        timeSinceExhale=0f;
                        StartInhaling();
                    }
                }else if(isInhaling){
                    if(inhaleTime>=inhaleLength){
                        StartExhaling();
                    }
                }
                break;
            case MovementBehavior.RunFromPlayer:
                targetLocation=characterRb.position+(characterRb.position-player.position);
                if(!isExhaling){
                    TurnTowards(targetLocation,turnSpeed);
                }
                if(!isInhaling && !isExhaling){
                    if(timeSinceExhale>=timeBetweenBreaths && distanceFromPlayer<=maxDistanceFromPlayer){
                        timeSinceExhale=0f;
                        StartInhaling();
                    }
                }else if(isInhaling){
                    if(inhaleTime>=inhaleLength){
                        StartExhaling();
                    }
                }
                break;
            case MovementBehavior.LookAtPlayer:
                targetLocation=player.position;
                TurnTowards(targetLocation,turnSpeed);
                break;
        }
    }

}

[System.Serializable]
public struct SpringMovementValues{
    public MovementBehavior movementBehavior;
    public int intensity;
    public float timeBetweenBreaths;
    [Tooltip("Min distance before stopping to move")]
    public float minDistanceFromPlayer;
    [Tooltip("Max distance before stopping to move")]
    public float maxDistanceFromPlayer;
    [Tooltip("Time to inhale for.")]
    public float inhaleLength;
    public float turnSpeed;

}

/*TO DO:
+ more complex behavior: run then turn, run then follow, etc.
+ distance buffer where if close or far ish it breathes for shorter amount of time
+ raycast target location
*/ 