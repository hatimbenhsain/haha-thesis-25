using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Migration : MonoBehaviour
{
    private Swimmer swimmer;
    public Color[] colors;
    public float progress;
    public float targetProgress;
    public float progressSpeed=0.1f;

    private Camera camera;

    private Volume volume;

    private SplitToning splitToning;
    private Color splitToningShadows;
    float shadowValue, highlightValue;
    private Color splitToningHighlights;

    private MigrationGenerator migrationGenerator;

    public Transform lightTransform;
    public float lightRotationSpeed=1f;

    public Vector3 targetAngle;
    private bool activatedMovement=false;

    public float currentForce=1f;
    [Tooltip("Time before current starts")]
    public float waitTime=5f;
    private float waitTimer=0f;
    public Transform[] migrationNodes;


    void Start()
    {
        camera=Camera.main;

        volume=FindObjectOfType<Volume>();
        volume.profile.TryGet<SplitToning>(out splitToning);
        splitToningShadows=splitToning.shadows.value;

        float h,s,v;
        Color.RGBToHSV(RenderSettings.fogColor,out h,out s,out v);
        float h1,s1,v1;
        Color.RGBToHSV(splitToningShadows,out h1,out s1,out v1);
        shadowValue=h1/h;

        splitToningHighlights=splitToning.highlights.value;
        Color.RGBToHSV(splitToningHighlights,out h1,out s1,out v1);
        highlightValue=h1/h;

        migrationGenerator=FindObjectOfType<MigrationGenerator>();

        swimmer=FindObjectOfType<Swimmer>();

    }

    void Update()
    {
        // progress+=Time.deltaTime*progressSpeed;
        // progress=Mathf.Clamp(progress,0f,colors.Length);
        float prevProgress=targetProgress;
        targetProgress=colors.Length*(float)migrationGenerator.closestIndex/(float)migrationGenerator.path.Length;
        targetProgress=Mathf.Max(prevProgress,targetProgress);
        progress=Mathf.Lerp(progress,targetProgress,Time.deltaTime*progressSpeed);

        Color startingColor=colors[Mathf.Clamp(Mathf.FloorToInt(progress),0,colors.Length-1)];
        Color endColor=colors[Mathf.Clamp(Mathf.CeilToInt(progress),0,colors.Length-1)];

        float h,s,v;
        float h1,s1,v1;
        Color.RGBToHSV(startingColor,out h1,out s1,out v1);
        float h2,s2,v2;
        Color.RGBToHSV(endColor,out h2,out s2,out v2);

        h=Mathf.Lerp(h1,h2,progress%1);
        s=Mathf.Lerp(s1,s2,progress%1);
        v=Mathf.Lerp(v1,v2,progress%1);

        Color newColor=Color.HSVToRGB(h,s,v);  

        RenderSettings.fogColor=newColor;
        camera.backgroundColor=newColor;      

        float shadowH=shadowValue*h;
        float highlightH=highlightValue*h;

        Color.RGBToHSV(splitToningShadows,out h1,out s1,out v1);
        splitToning.shadows.value=Color.HSVToRGB(shadowH,s1,v1);

        Color.RGBToHSV(splitToningHighlights,out h1,out s1,out v1);
        splitToning.highlights.value=Color.HSVToRGB(highlightH,s1,v1);

        
        Vector3 rot=new Vector3(Time.time*lightRotationSpeed,0f,0f);
        lightTransform.rotation=Quaternion.Euler(rot);

        waitTimer+=Time.deltaTime;

        if(!activatedMovement && (Vector3.Angle(swimmer.transform.forward,targetAngle)<=45f || waitTimer>waitTime)){
            swimmer.canMove=true;
            activatedMovement=true; 
        }

        if(activatedMovement && waitTimer>waitTime){
            //Current pushing swimmer
            float minDistance=1000f;
            int nodeIndex=0;
            for(var i=0;i<migrationNodes.Length;i++){
                float distance=Vector3.Distance(swimmer.transform.position,migrationNodes[i].position);
                if(distance<minDistance){
                    minDistance=distance;
                    nodeIndex=i;
                }
            }
            if(nodeIndex+1<migrationNodes.Length){
                Vector3 swimmerVelocity=swimmer.GetVelocity();
                Vector3 current=migrationNodes[nodeIndex+1].position-swimmer.transform.position;
                Vector3 velocityToCurrent=Vector3.Project(swimmerVelocity,current);
                if(velocityToCurrent.normalized==-current.normalized || velocityToCurrent.magnitude<swimmer.coastingSpeed){
                    swimmer.Boost(current.normalized*currentForce*Time.deltaTime);
                }
            }
        }

    }

}
