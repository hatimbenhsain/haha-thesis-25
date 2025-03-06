using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceAroundTarget : MonoBehaviour
{
    public Transform target;
    public float minDistance=5f;
    public float maxDistance=8f;

    void Start()
    {
        float distance=Random.Range(minDistance,maxDistance);
        Vector3 dir=new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f),Random.Range(-1f,1f));
        dir=dir.normalized;
        transform.position=target.position+dir*distance;
        Destroy(this);
    }


}
