using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCylinderManager : MonoBehaviour
{
    public GameObject cylinderCollider;
    public GameObject organHead;
    public float distanceThreshold; // the distance between the cylinder and the organ head to deactivate head
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(cylinderCollider.transform.position, organHead.transform.position) > distanceThreshold)
        {
            cylinderCollider.SetActive(false);
        }

    }
}
