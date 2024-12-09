using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MigrationNPC : MonoBehaviour
{
    public float strokeFrequencyVariance=0.2f;
    public RuntimeAnimatorController [] animators;

    public float maxDistance;

    public Transform player;
    public MigrationGenerator migrationGenerator;
    public GameObject path;

    void Start()
    {
        GetComponentInChildren<SpriteRenderer>().material.color=Color.HSVToRGB(Random.Range(0f,1f),33f/255f,1f);
        NPCOverworld npcOverworld=GetComponent<NPCOverworld>();
        npcOverworld.strokeFrequency=npcOverworld.strokeFrequency+Random.Range(-strokeFrequencyVariance,strokeFrequencyVariance);
        GetComponent<Animator>().runtimeAnimatorController=animators[Random.Range(0,animators.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        float distanceFromPlayer=Vector3.Distance(transform.position,player.transform.position);

        if(distanceFromPlayer>maxDistance){
            migrationGenerator.npcNum-=1;
            Destroy(path);
            Destroy(gameObject);
        }
    }
}
