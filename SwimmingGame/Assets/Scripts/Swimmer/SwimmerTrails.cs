using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SwimmerTrails : MonoBehaviour
{
    private Swimmer swimmer;
    public ParticleSystem[] trails;

    [Tooltip("Minimum speed at which to start showing each trail")]
    public float[] minSpeeds;
    [Tooltip("Maximum speed at which to start showing each trail")]
    public float[] maxSpeeds;
    private float[] defaultRatesOverTime;

    public GameObject[] pixelLinesHorizontal;
    public GameObject[] pixelLinesVertical;
    public float pixelLineMaxStretch=3f;
    public float pixelLineMinSpeed=1f;
    public float pixelLineMaxSpeed=3f;

    void Start()
    {
        swimmer=FindObjectOfType<Swimmer>();
        defaultRatesOverTime=new float[trails.Length];
        for(int i=0;i<trails.Length;i++){
            var emission=trails[i].emission;
            defaultRatesOverTime[i]=emission.rateOverTime.constant;
        }
    }

    void Update()
    {
        float playerVelocity=swimmer.GetVelocity().magnitude;
        for(int i=0;i<trails.Length;i++){
            UpdateTrail(trails[i],defaultRatesOverTime[i],playerVelocity,minSpeeds[i],maxSpeeds[i]);
        }
    }

    //The trail's rateovertime increases with the object velocity
    void UpdateTrail(ParticleSystem ps,float defaultRateOverTime,float velocity,float minSpeed,float maxSpeed){
        var emission=ps.emission;
        var r=Mathf.Clamp(velocity,minSpeed,maxSpeed);
        r=(r-minSpeed)/(maxSpeed-minSpeed);
        emission.rateOverTime=Mathf.Clamp(r*defaultRateOverTime,0,defaultRateOverTime);
    }

    public void DashTrail(Directions direction=Directions.RIGHT,float speed=1f){
        if(speed>pixelLineMinSpeed){
            GameObject pl;
            if(direction==Directions.UP || direction==Directions.DOWN) pl=pixelLinesVertical[Random.Range(0,pixelLinesVertical.Length)];
            else pl=pixelLinesHorizontal[Random.Range(0,pixelLinesHorizontal.Length)];
            GameObject g=Instantiate(pl,pl.transform.position,pl.transform.rotation);
            g.transform.SetParent(transform.parent.parent);
            Vector3 scale=g.transform.localScale;
            scale.x=scale.x*(1f+(pixelLineMaxStretch-1f)*Mathf.Clamp((speed-pixelLineMinSpeed)/(pixelLineMaxSpeed-pixelLineMinSpeed),0f,1f));
            switch(direction){
                case Directions.LEFT:
                    scale=new Vector3(-scale.x,scale.y,scale.z);
                    break;
                case Directions.UP:
                    scale=new Vector3(-scale.x,scale.y,scale.z);
                    break;
            }
            g.transform.localScale=scale;
            g.SetActive(true);
        }
    }
}
