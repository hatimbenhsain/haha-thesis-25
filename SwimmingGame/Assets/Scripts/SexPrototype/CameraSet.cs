using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraSet
{
    public Camera mainCamera; // main camera
    public Camera fixedPOV; // fixed POV camera
    public Camera followCamera; // follow camera
}
