using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rumble : MonoBehaviour
{
    [Range(0f,1f)]
    public float maxRumbleIntensity=1f;

    private static float currentLeftMotorIntensity=0f;
    private static float currentRightMotorIntensity=0f;

    public RumbleSettings[] rumbleSettings;

    static RumbleSettings[] settings;

    private PlayerInput playerInput;

    void Start()
    {
        settings=rumbleSettings;
        playerInput=GetComponent<PlayerInput>();
    }

    private void LateUpdate() {
        if(playerInput.currentControlScheme=="Gamepad"){
            Gamepad.current.SetMotorSpeeds(currentLeftMotorIntensity*maxRumbleIntensity,currentRightMotorIntensity*maxRumbleIntensity);
            currentLeftMotorIntensity=0f;
            currentRightMotorIntensity=0f;
        }
    }

    public static void AddRumble(string nameOfAction,float modifier=1f){
        foreach(RumbleSettings setting in settings){
            if(setting.name==nameOfAction){
                AddRumble(setting.leftMotorIntensity,setting.rightMotorIntensity,setting.rumbleType,modifier);
                break;
            }
        }
    }

    public static void AddRumble(float leftMotorIntensity,float rightMotorIntensity,RumbleType rumbleType=RumbleType.Max,float modifier=1f){
        switch(rumbleType){
            case RumbleType.Additive:
                currentLeftMotorIntensity+=leftMotorIntensity*modifier;
                currentRightMotorIntensity+=rightMotorIntensity*modifier;
                break;
            case RumbleType.Override:
                currentLeftMotorIntensity=leftMotorIntensity*modifier;
                currentRightMotorIntensity=rightMotorIntensity*modifier;
                break;
            case RumbleType.Max:
                currentLeftMotorIntensity=Mathf.Max(leftMotorIntensity*modifier,currentLeftMotorIntensity);
                currentRightMotorIntensity=Mathf.Max(rightMotorIntensity*modifier,currentRightMotorIntensity);
                break;
        }
    }

    public static void ResetRumble(){
        if(Gamepad.current!=null){
            Gamepad.current.SetMotorSpeeds(0f,0f);
        }
    }

    private void OnDestroy() {
        ResetRumble();
    }

}

[System.Serializable]
public struct RumbleSettings{
    public string name;
    [Range(0f,1f)]
    public float leftMotorIntensity;
    [Range(0f,1f)]
    public float rightMotorIntensity;
    public RumbleType rumbleType;
}

public enum RumbleType{
    Additive, //This rumble intensity gets added to the current one
    Override, //This rumble replaces the current one
    Max //If current intensity bigger than new one then we don't add anything.
}