using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullingArea : MonoBehaviour
{
    private List<CulledObject> objectsToActivate;
    private List<CulledObject> objectsToDeactivate;
    public int maxObjectsToCullInAFrame=5;


    void Start(){
        objectsToActivate=new List<CulledObject>();
        objectsToDeactivate=new List<CulledObject>();
    }

    void Update(){

        int k=0;
        while(k<maxObjectsToCullInAFrame && objectsToActivate.Count>0){
            objectsToActivate[0].Activate(true);
            objectsToActivate.RemoveAt(0);
            k++;
        }

        while(k<maxObjectsToCullInAFrame && objectsToDeactivate.Count>0){
            objectsToDeactivate[0].Activate(false);
            objectsToDeactivate.RemoveAt(0);
            k++;
        }
        
    }

    private void OnTriggerEnter(Collider other) {
        CulledObject c=other.gameObject.GetComponent<CulledObject>();
        if(c==null){
            c=other.gameObject.GetComponentInParent<CulledObject>();
        }
        Debug.Log(c.gameObject);
        objectsToActivate.Add(c);
    }

    private void OnTriggerExit(Collider other) {
        CulledObject c=other.gameObject.GetComponent<CulledObject>();
        if(c==null){
            c=other.gameObject.GetComponentInParent<CulledObject>();
        }
        Debug.Log(c.gameObject);
        objectsToDeactivate.Add(c);
    }
}
