using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRootFollow : MonoBehaviour
{
    // keep root transform same as character
    public Transform character; 

    void FixedUpdate()
    {
        transform.position = character.position;
    }
}
