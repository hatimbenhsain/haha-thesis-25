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
}
