using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
    private MusicBeat musicBeat;

    public float animationSpeedFactor=.5f;

    public float offset;
    public float ratio=1f;
    
    [Tooltip("In degrees")]
    public float angle=45f;

    private Vector3 axisToRotateAround1;
    private Vector3 axisToRotateAround2;

    Quaternion initialRotation;

    void Start()
    {
        axisToRotateAround1=transform.forward;
        axisToRotateAround2=transform.up;
        initialRotation=transform.rotation;
        musicBeat=FindObjectOfType<MusicBeat>();
    }

    void Update()
    {
        float currentAngle=0f;

        float period=60f/(musicBeat.timelineInfo.currentTempo*animationSpeedFactor);
        float value;
        if(Mathf.Floor(musicBeat.timelineInfo.currentTime*0.001f/period+offset)%(1/ratio)==0f){
            value=(((musicBeat.timelineInfo.currentTime*0.001f)%(period))/period+offset)%1;
        }else{
            value=0f;
        }
        currentAngle=value*360f;

        //Quaternion rotation=Quaternion.AngleAxis(currentAngle,axisToRotateAround1)*Quaternion.AngleAxis(90f-angle,axisToRotateAround2);
        //transform.rotation=rotation;
        transform.rotation=initialRotation;
        transform.RotateAround(transform.position,axisToRotateAround2,angle);
        transform.RotateAround(transform.position,axisToRotateAround1,currentAngle);
    }
}
