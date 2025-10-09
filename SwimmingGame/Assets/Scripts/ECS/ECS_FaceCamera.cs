using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECS_FaceCamera : MonoBehaviour
{
    public bool lockX=false;
    public bool lockY=false;
    public bool lockZ=false;

    public Vector3 originalRotation;
    
    void Start()
    {
        originalRotation=transform.localRotation.eulerAngles;
    }
}
