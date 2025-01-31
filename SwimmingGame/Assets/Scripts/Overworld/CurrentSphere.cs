using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentSphere : MonoBehaviour
{
    public GameObject[] prefabs;
    public float emitRadius=20f;
    [Tooltip("Emit nothing in this direction.")]
    public Vector3 emptyDirection;
    [Tooltip("Max angle from empty direction where we're not emitting.")]
    public float maxAngle=0f;
    public float stepLength=5;

    private Transform swimmer;
    private bool active=true;
    private List<GameObject> children;

    void Start()
    {
        swimmer=FindObjectOfType<Swimmer>().transform;

        children=new List<GameObject>();

        float scale=(transform.lossyScale.x+transform.lossyScale.y+transform.lossyScale.z)/3f;
        emitRadius=emitRadius*scale;

        for(float n=0f;n<180f;n+=stepLength){
            float offset=Random.Range(-stepLength,stepLength);
            for(float k=0f;k<360f;k+=stepLength){
                Vector3 pos=new Vector3(Mathf.Cos((k+offset)*Mathf.PI/180f)*emitRadius,Mathf.Sin((k+offset)*Mathf.PI/180f)*emitRadius,0f);
                Quaternion q=Quaternion.AngleAxis(n,Vector3.up);
                pos=q*pos;
                float angle=Vector3.Angle(pos,emptyDirection);
                if(angle>maxAngle){
                    GameObject g=Instantiate(prefabs[Random.Range(0,prefabs.Length-1)],transform.position+pos,Quaternion.LookRotation(-pos),transform);
                    children.Add(g);
                }
            }
        }
    }

    private void Update() {
        float distance=Vector3.Distance(transform.position,swimmer.position);
        if(distance<emitRadius*1.5f){
            if(!active){
                for(var i=0;i<children.Count;i++){
                    children[i].SetActive(true);
                }
            }
            active=true;
        }else{
            if(active){
                for(var i=0;i<children.Count;i++){
                    children[i].SetActive(false);
                }
            }
            active=false;
        }
    }

}
