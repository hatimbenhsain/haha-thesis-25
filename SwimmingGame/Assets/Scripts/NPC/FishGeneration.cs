using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishGeneration : MonoBehaviour
{
    public int numberOfFishToGenerate=16;
    public int numberOfLeadersToGenerate=2;
    public int distanceRange=1;
    public int variance=3;

    public GameObject fishPrefab;

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
        GameObject fish=Instantiate(fishPrefab,pos,Quaternion.identity);
        SpriteRenderer spriteRenderer=fish.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.material.color=Color.HSVToRGB(Random.Range(0f,1f),33f/255f,1f);
        float s=Random.Range(0.9f,1.5f);
        spriteRenderer.transform.localScale=Vector3.one*s;
        return fish.GetComponent<Fish>();
    }

}
