using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeacherSexAI : NPCSexAI
{
    // This should override the function from NPCSexAI (CURRENTLY UNTESTED)
    public override void AIBehavior(){

        //CURRENTLY JUST COPY AND PASTED FROM NPCSEXAI STANDARD BEHAVIOR

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
}
