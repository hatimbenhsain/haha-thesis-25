using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FishGeneration : MonoBehaviour
{
    public int numberOfFishToGenerate=16;
    public int numberOfLeadersToGenerate=2;
    public int distanceRange=1;
    public int variance=3;

    public float minScale=0.75f;
    public float maxScale=2f;
    public float pitchVariance=2f;

    public GameObject[] fishPrefabs;

    void Start()
    {
        numberOfFishToGenerate=numberOfFishToGenerate+Random.Range(-variance,variance);
        numberOfLeadersToGenerate=numberOfLeadersToGenerate+Random.Range(-variance,variance);
        numberOfFishToGenerate=Mathf.Max(1,numberOfFishToGenerate);
        numberOfLeadersToGenerate=Mathf.Max(1,numberOfLeadersToGenerate);

        Fish[] fishes=new Fish[numberOfFishToGenerate];

        for(int i=0;i<numberOfLeadersToGenerate;i++){
            fishes[i]=Generate();
        }

        for(int i=numberOfLeadersToGenerate;i<numberOfFishToGenerate;i++){
            fishes[i]=Generate();
            fishes[i].movementBehavior=MovementBehavior.FollowLeader;
            Debug.Log("made follower");
        }

    }

    Fish Generate(){
        Vector3 pos=transform.position+new Vector3(Random.Range(-distanceRange,distanceRange),
            Random.Range(-distanceRange,distanceRange),Random.Range(-distanceRange,distanceRange));
        GameObject fish=Instantiate(fishPrefabs[Random.Range(0,fishPrefabs.Length)],pos,Quaternion.identity);
        SpriteRenderer spriteRenderer=fish.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.material.color=Color.HSVToRGB(Random.Range(0f,1f),33f/255f,1f);
        float s=Random.Range(minScale,maxScale);
        spriteRenderer.transform.localScale=Vector3.one*s;
        Fish f=fish.GetComponent<Fish>();
        f.pitch=f.pitch+pitchVariance*2f*((s-minScale)/(maxScale-minScale)-0.5f);
        f.transform.parent=transform;
        return f;
    }

}
