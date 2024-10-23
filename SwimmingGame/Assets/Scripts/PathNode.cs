using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    public PathNodeType type;
    public float pauseLength=0f;

    void OnDrawGizmos(){

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.5f);
        int childCount=transform.parent.childCount;
        int index=transform.GetSiblingIndex();
        if(index<childCount-1){
            Transform t=transform.parent.GetChild(index+1);
            Gizmos.DrawLine(transform.position,t.position);
        }
        
        Gizmos.color = Color.white;  //Reset so other Gizmos aren't affected

    }
}

public enum PathNodeType{
    Continue,
    Pause
}