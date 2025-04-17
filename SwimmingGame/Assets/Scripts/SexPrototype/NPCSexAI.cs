using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script decides the NPC's state based on player input and regulates intensity
public class NPCSexAI : MonoBehaviour
{
    [HideInInspector]
    public NPCSpring npcSpring;
    [HideInInspector]
    public SexGameManager sexGameManager;

    [HideInInspector]
    public int stateCounter=0; //How many different states the NPC has gone through
    [Header("Parameters")]

    [Tooltip("Distance meter starts going up from this value, slowly.")]
    public float distanceMinTreshold=5f;
    [Tooltip("Distance meter starts going up from this value, fast.")]
    public float distanceMaxTreshold=2f;
    [Tooltip("Entanglement meter starts going up from this value, slowly.")]
    public float entanglementMinTreshold=3f;
    [Tooltip("Entanglement meter starts going up from this value, fast.")]
    public float entanglementMaxTreshold=6f;

    [Tooltip("Speed meter starts going up from this value, slowly.")]
    public float speedMinTreshold=1f;
    [Tooltip("Speed meter starts going up from this value, fast.")]
    public float speedMaxTreshold=2f;

    public float meterGrowthSpeed=10f;
    public float meterDecaySpeed=5f;

    [Tooltip("When reached this intensity, move on")]
    public int intensityToReach=4;
    [Tooltip("When gone through this many states, move on")]
    public int statesToCycleThrough=6;
    [Tooltip("In seconds, only change state or intensity after at least this time.")]
    public float minTimeBeforeStateChange=4f;
    [Tooltip("In seconds, after this time maybe change state or intensity.")]
    public float maxTimeBeforeStateChange=10f;

    [Header("Debug Values")]
    [Tooltip("Controlled by how close the two heads are to each other.")]
    public float distanceMeter=0f;
    [Tooltip("Controlled by how entangled the two organs are.")]
    public float entanglementMeter=0f;
    [Tooltip("Controlled by how fast the player is moving.")]
    public float speedMeter=0f;
    public float timeSinceStateChange;

    [Header("Misc.")]
    [Tooltip("Index in this array is intensity.")]
    public SexAIParameters[] sexIntensityParameters;

    void Start()
    {
        npcSpring=GetComponent<NPCSpring>();
        sexGameManager=FindObjectOfType<SexGameManager>();
    }

    void Update()
    {
        timeSinceStateChange+=Time.deltaTime;
        UpdateMeters();

        //AI PART BEGIN (flowchart)

        AIBehavior();

        //AI PART END

        //Rumble.AddRumble("Sex Intensity",npcSpring.currentIntensity/intensityToReach);
        
    }

    public virtual void AIBehavior(){

        // TELLS US WHEN TO GO TO OTHER BEHAVIORS/CHANGE INTENSITY

        switch(npcSpring.movementBehavior){
            case MovementBehavior.FollowPath:
                //HERE I WOULD TELL NPC TO CHANGE TO OTHER STATE 
                //ONCE IT'S NEAR OBJECTIVE ENOUGH OR WHATEVER
            case MovementBehavior.FollowTarget:
                //HERE I WOULD TELL NPC TO CHANGE TO OTHER STATE 
                //ONCE IT'S NEAR OBJECTIVE ENOUGH OR WHATEVER
            case MovementBehavior.Wander:
                if(distanceMeter>=50f){
                    ChangeState(MovementBehavior.FollowPlayer);
                }
                break;
            case MovementBehavior.FollowPlayer:
                if(entanglementMeter>=100f){
                    ChangeIntensity(1);
                }else if(speedMeter>=100f && npcSpring.currentIntensity<=4f){
                    ChangeIntensity(-1);
                    if(npcSpring.currentIntensity<=0){
                        ChangeState(MovementBehavior.RunFromPlayer);
                    }
                }
                break;
            case MovementBehavior.RunFromPlayer:
                if(entanglementMeter>=100f || timeSinceStateChange>=maxTimeBeforeStateChange || 
                    npcSpring.currentIntensity>=3f){
                    ChangeIntensity(1);
                    ChangeState(MovementBehavior.FollowPlayer);
                }else if(distanceMeter>=100f && timeSinceStateChange>=minTimeBeforeStateChange){
                    ChangeIntensity(1);
                    if(npcSpring.currentIntensity>=2f){
                        ChangeState(MovementBehavior.FollowThenRun);
                    }
                }
                break;
            case MovementBehavior.FollowThenRun:
                if(entanglementMeter>=100f || timeSinceStateChange>=maxTimeBeforeStateChange){
                    ChangeIntensity(1);
                }
                break;
        }

        // WHEN TO MOVE ON:
        if(npcSpring.currentIntensity>=intensityToReach || stateCounter>=statesToCycleThrough){
            sexGameManager.MoveOn();
        }
    }

    public void ChangeState(MovementBehavior movementBehavior){
        Debug.Log("Change state ");
        Debug.Log(movementBehavior);
        npcSpring.ChangeMovementBehavior(movementBehavior);
        stateCounter++;
        ResetMeters();
    }

    public void ChangeIntensity(int i){
        Debug.Log("Change intensity");
        Debug.Log(i);
        npcSpring.ChangeIntensity(i);
        ResetMeters();
        if(npcSpring.currentIntensity<sexIntensityParameters.Length){
            CopyValues(sexIntensityParameters[npcSpring.currentIntensity]);
        }
    }

    void ResetMeters(){
        timeSinceStateChange=0f;
        entanglementMeter=0f;
        distanceMeter=0f;
        speedMeter=0f;
    }

    void UpdateMeters(){
        distanceMeter+=GrowMeter(sexGameManager.headToHeadDistance,meterGrowthSpeed,distanceMinTreshold,distanceMaxTreshold,false);
        entanglementMeter+=GrowMeter(sexGameManager.GetMeanDistance(),meterGrowthSpeed,entanglementMinTreshold,entanglementMaxTreshold,false);
        speedMeter+=GrowMeter(sexGameManager.playerBodyVelocity,meterGrowthSpeed,speedMinTreshold,speedMaxTreshold,true);
        
        distanceMeter=Mathf.Clamp(distanceMeter,0f,100f);
        entanglementMeter=Mathf.Clamp(entanglementMeter,0f,100f);
        speedMeter=Mathf.Clamp(speedMeter,0f,100f);
    }

    //Returns value to add or remove from meter
    // positive: true if when value goes up, meter goes up
    //           false if  when value goes up, meter goes down
    float GrowMeter(float value,float baseGrowthSpeed, float minTreshold, float maxTreshold,bool positive){
        if((value>=minTreshold && positive)||(value<=minTreshold && !positive)){
            float growthSpeed=GetGrowthSpeed(value,meterGrowthSpeed,minTreshold,maxTreshold);
            return growthSpeed*Time.deltaTime;
        }else{
            return -meterDecaySpeed*Time.deltaTime;
        }
    }

    //This is linear, could experiment with exp or logarithmic etc versions
    float GetGrowthSpeed(float value,float baseGrowthSpeed, float minTreshold, float maxTreshold){
        return baseGrowthSpeed*Mathf.Clamp((value-minTreshold)/(maxTreshold-minTreshold),0f,1f);
    }

    void CopyValues(SexAIParameters values){
        distanceMinTreshold=values.distanceMinTreshold;
        distanceMaxTreshold=values.distanceMaxTreshold;
        entanglementMinTreshold=values.entanglementMinTreshold;
        entanglementMaxTreshold=values.entanglementMaxTreshold;
        speedMinTreshold=values.speedMinTreshold;
        speedMaxTreshold=values.speedMaxTreshold;
        meterGrowthSpeed=values.meterGrowthSpeed;
        meterDecaySpeed=values.meterDecaySpeed;
    }
}


[System.Serializable]
public struct SexAIParameters{
    [Tooltip("Distance meter starts going up from this value, slowly.")]
    public float distanceMinTreshold;
    [Tooltip("Distance meter starts going up from this value, fast.")]
    public float distanceMaxTreshold;
    [Tooltip("Entanglement meter starts going up from this value, slowly.")]
    public float entanglementMinTreshold;
    [Tooltip("Entanglement meter starts going up from this value, fast.")]
    public float entanglementMaxTreshold;

    [Tooltip("Speed meter starts going up from this value, slowly.")]
    public float speedMinTreshold;
    [Tooltip("Speed meter starts going up from this value, fast.")]
    public float speedMaxTreshold;

    public float meterGrowthSpeed;
    public float meterDecaySpeed;
}