using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;
    public float smoothTime=0.25f;
    Vector3 currentVelocity;
    Vector3 rotationVelocity;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {  
        transform.position = Vector3.SmoothDamp(transform.position,target.position,ref currentVelocity,smoothTime);
        transform.rotation=Quaternion.Euler(Vector3.SmoothDamp(transform.rotation.eulerAngles,target.rotation.eulerAngles,ref rotationVelocity,smoothTime));
    }
}
