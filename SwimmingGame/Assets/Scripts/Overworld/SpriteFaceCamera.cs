using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFaceCamera : MonoBehaviour
{
    public bool lockX=false;
    public bool lockY=false;
    public bool lockZ=false;

    private Vector3 originalRotation;
    void Start()
    {
        originalRotation=transform.localRotation.eulerAngles;
    }

    void Update()
    {
        transform.rotation=Camera.main.gameObject.transform.rotation;
        if(lockX || lockY || lockZ){
            Vector3 rot=transform.localRotation.eulerAngles;
            if(lockX) rot.x=originalRotation.x;
            if(lockY) rot.y=originalRotation.y;
            if(lockZ) rot.z=originalRotation.z;
            transform.localRotation=Quaternion.Euler(rot);
        }
    }
}
