using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TightenRope : MonoBehaviour
{
    public Transform target; 
    public float speed = 0.5f; // Speed of the rope tightening
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);
    }
}
