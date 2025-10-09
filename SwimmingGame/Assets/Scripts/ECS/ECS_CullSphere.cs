using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECS_CullSphere : MonoBehaviour
{
    private List<ECS_CulledObject> objectsToActivate;
    private List<ECS_CulledObject> objectsToDeactivate;
    static List<ECS_CulledObject> objectsInPlay;
    public int maxObjectsToCullInAFrame=20;

    public Transform targetTransform;

    [Tooltip("Do work every (this number) frames.")]
    public int frameFrequency=1;

    private int frame=0;


    public delegate void OnCullObject(GameObject gameObject);
    public static event OnCullObject OnAddObject;
    public static event OnCullObject OnRemoveObject;


    void Start(){
        objectsToActivate=new List<ECS_CulledObject>();
        objectsToDeactivate=new List<ECS_CulledObject>();
        objectsInPlay=new List<ECS_CulledObject>();
    }

    void Update(){

        transform.position=targetTransform.position;

        int k=0;

        frame+=1;

        if(frame%frameFrequency==0){
            while(k<maxObjectsToCullInAFrame && objectsToActivate.Count>0){
                objectsInPlay.Add(objectsToActivate[0]);
                OnAddObject?.Invoke(objectsToActivate[0].gameObject);
                objectsToActivate.RemoveAt(0);
                k++;
            }

            while(k<maxObjectsToCullInAFrame && objectsToDeactivate.Count>0){
                OnRemoveObject?.Invoke(objectsToDeactivate[0].gameObject);
                objectsInPlay.Remove(objectsToDeactivate[0]);
                objectsToDeactivate.RemoveAt(0);
                k++;
            }
        }
        
    }

    private void OnTriggerEnter(Collider other) {
        ECS_CulledObject c=other.gameObject.GetComponent<ECS_CulledObject>();
        if(c==null){
            c=other.gameObject.GetComponentInParent<ECS_CulledObject>();
        }
        objectsToActivate.Add(c);
    }

    private void OnTriggerExit(Collider other) {
        ECS_CulledObject c=other.gameObject.GetComponent<ECS_CulledObject>();
        if(c==null){
            c=other.gameObject.GetComponentInParent<ECS_CulledObject>();
        }
        objectsToDeactivate.Add(c);
    }

    public static List<ECS_CulledObject> GetObjectsInPlay(){
        return objectsInPlay;
    }
}
