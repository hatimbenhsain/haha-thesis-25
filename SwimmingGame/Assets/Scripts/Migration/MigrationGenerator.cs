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

    public GameObject[] paths;

    public Transform origin;
    public Transform origin2;

    public Transform[] path;

    public Transform player;

    public int backDistance=3;
    public int forwardDistance=3;

    public float maxDistance;

    void Start()
    {
        scale=transform.lossyScale;
        for(int i=0;i<initialLoad;i++){
            Generate(origin);
        }

        timer=0f;
    }

    void Update()
    {
        if(npcNum<maxNPCnum){
            timer+=Time.deltaTime;

            if(timer>=period){
                Generate(origin);
                if(forwardDistance==0 && backDistance==0){
                    period=basePeriod*2+Random.Range(0f,periodVariance);
                }else{
                    period=basePeriod+Random.Range(0f,periodVariance);
                }
                timer=timer%period;

                if(npcNum<maxNPCnum/2){
                    Generate(origin2);
                }
            }
        }

        float minDistance=100f;
        int index=0;

        for(int i=0;i<path.Length;i++){
            float distanceFromPlayer=Vector3.Distance(path[i].transform.position,player.transform.position);
            if(distanceFromPlayer<minDistance){
                minDistance=distanceFromPlayer;
                index=i;
            }
        }

        origin=path[Mathf.Clamp(index-backDistance,0,path.Length-1)];
        float d=Vector3.Distance(origin.position,player.position);
        if(d>maxDistance){
            backDistance-=1;
        }else if(d<maxDistance/2){
            backDistance+=1;
        }
        backDistance=Mathf.Clamp(backDistance,0,3);
        origin2=path[Mathf.Clamp(index+forwardDistance,0,path.Length-1)];
        d=Vector3.Distance(origin2.position,player.position);
        if(d>maxDistance){
            forwardDistance-=1;
        }else if(d<maxDistance/2){
            forwardDistance+=1;
        }
        forwardDistance=Mathf.Clamp(forwardDistance,0,3);

        
    }

    void Generate(Transform t){
        npcNum+=1;
        Vector3 pos=new Vector3(Random.Range(-scale.x,scale.x),Random.Range(-scale.y,scale.y),Random.Range(-scale.z,scale.z));
        GameObject npc=Instantiate(npcPrefab,t.position+pos,npcPrefab.transform.rotation);
        npc.GetComponent<MigrationNPC>().path.transform.position=transform.position+pos;
        //npc.transform.position=pos;
        npc.SetActive(true);
        //npc.transform.position=origin.position+pos;
    }
}
