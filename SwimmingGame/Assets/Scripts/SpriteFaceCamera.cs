using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFaceCamera : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        transform.rotation=Camera.main.gameObject.transform.rotation;
    }
}
