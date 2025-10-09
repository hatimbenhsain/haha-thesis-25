using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullingArea : MonoBehaviour
{
    private List<CulledObject> objectsToActivate;
    private List<CulledObject> objectsToDeactivate;
    public int maxObjectsToCullInAFrame=5;

    [Tooltip("Do work every (this number) frames.")]
    public int frameFrequency=1;

    private int frame=0;


    void Start(){
        objectsToActivate=new List<CulledObject>();
        objectsToDeactivate=new List<CulledObject>();
    }

    void Update(){

        int k=0;

        frame+=1;

        if(frame%frameFrequency==0){
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
        
    }

    private void OnTriggerEnter(Collider other) {
        CulledObject c=other.gameObject.GetComponent<CulledObject>();
        if(c==null){
            c=other.gameObject.GetComponentInParent<CulledObject>();
        }
        objectsToActivate.Add(c);
    }

    private void OnTriggerExit(Collider other) {
        CulledObject c=other.gameObject.GetComponent<CulledObject>();
        if(c==null){
            c=other.gameObject.GetComponentInParent<CulledObject>();
        }
        objectsToDeactivate.Add(c);
    }
}
