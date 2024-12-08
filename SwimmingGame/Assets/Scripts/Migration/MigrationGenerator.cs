using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MigrationGenerator : MonoBehaviour
{
    public float period=0.1f;
    public float initialLoad=20f;
    public float basePeriod=0.1f;
    public float periodVariance=2f;

    public GameObject npcPrefab;
    public int npcNum;

    private Vector3 scale;
    private float timer;

    public int maxNPCnum=300;
    void Start()
    {
        scale=transform.lossyScale;
        for(int i=0;i<initialLoad;i++){
            Generate();
        }

        timer=0f;
    }

    void Update()
    {
        if(npcNum<maxNPCnum){
            timer+=Time.deltaTime;

            if(timer>=period){
                Generate();
                period=basePeriod+Random.Range(0f,periodVariance);
                timer=timer%period;
            }
        }
    }

    void Generate(){
        npcNum+=1;
        
        Vector3 pos=transform.position+new Vector3(Random.Range(-scale.x,scale.x),Random.Range(-scale.y,scale.y),Random.Range(-scale.z,scale.z));
        GameObject npc=Instantiate(npcPrefab,pos,npcPrefab.transform.rotation);
        //npc.transform.position=pos;
    }
}
