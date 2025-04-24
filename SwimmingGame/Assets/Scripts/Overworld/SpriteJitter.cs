using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteJitter : MonoBehaviour
{
    public float period=1f;
    private float timer=0f;
    public float jitterIntensity=.1f;
    private Vector3 originalPosition;

    public bool lockX=false;
    public bool lockY=false;
    public bool lockZ=false;

    void Start()
    {
        originalPosition=transform.localPosition;
    }

    void Update()
    {
        timer+=Time.deltaTime;
        if(timer%period<(timer-Time.deltaTime)%period){
            Vector3 pos=originalPosition+new Vector3(Random.Range(-jitterIntensity,jitterIntensity),Random.Range(-jitterIntensity,jitterIntensity),Random.Range(-jitterIntensity,jitterIntensity));
            if(lockX) pos.x=originalPosition.x;
            if(lockY) pos.y=originalPosition.y;
            if(lockZ) pos.z=originalPosition.z;
            transform.localPosition=pos;
        }
    }
}
