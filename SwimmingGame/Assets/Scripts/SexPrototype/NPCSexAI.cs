using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script decides the NPC's state based on player input and regulates intensity
public class NPCSexAI : MonoBehaviour
{
    private NPCSpring npcSpring;
    private SexGameManager sexGameManager;

    private int stateCounter=0; //How many different states the NPC has gone through
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

    [Header("Debug Values")]
    [Tooltip("Controlled by how close the two heads are to each other.")]
    public float distanceMeter=0f;
    [Tooltip("Controlled by how entangled the two organs are.")]
    public float entanglementMeter=0f;
    [Tooltip("Controlled by how fast the player is moving.")]
    public float speedMeter=0f;
    public float timeSinceStateChange;
    void Start()
    {
        npcSpring=GetComponent<NPCSpring>();
        sexGameManager=FindObjectOfType<SexGameManager>();
    }

    void Update()
    {
        timeSinceStateChange+=Time.deltaTime;
        UpdateMeters();

        switch(npcSpring.movementBehavior){
            case MovementBehavior.Wander:
                break;
        }
    }

    void ChangeState(MovementBehavior movementBehavior){
        npcSpring.ChangeMovementBehavior(movementBehavior);
        ResetMeters();
    }

    void ResetMeters(){
        timeSinceStateChange=0f;
        entanglementMeter=0f;
        distanceMeter=0f;
        speedMeter=0f;
    }

    void UpdateMeters(){
        distanceMeter+=GrowMeter(sexGameManager.headToHeadDistance,meterGrowthSpeed,speedMinTreshold,speedMaxTreshold,false);
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
}
