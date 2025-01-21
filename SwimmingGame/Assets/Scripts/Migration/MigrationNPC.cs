using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class MigrationNPC : MonoBehaviour
{
    public float strokeFrequencyVariance=0.2f;
    public RuntimeAnimatorController [] animators;
    public SpriteLibraryAsset[] spriteLibraryAssets;

    public float maxDistance;

    public Transform player;
    public MigrationGenerator migrationGenerator;
    public GameObject path;

    void Start()
    {
        SpriteRenderer spriteRenderer=GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.material.color=Color.HSVToRGB(Random.Range(0f,1f),33f/255f,1f);
        NPCOverworld npcOverworld=GetComponent<NPCOverworld>();
        npcOverworld.strokeFrequency=npcOverworld.strokeFrequency+Random.Range(-strokeFrequencyVariance,strokeFrequencyVariance);
        //GetComponent<Animator>().runtimeAnimatorController=animators[Random.Range(0,animators.Length)];
        GetComponentInChildren<SpriteLibrary>().spriteLibraryAsset=spriteLibraryAssets[Random.Range(0,spriteLibraryAssets.Length)];
        float s=Random.Range(0.9f,1.5f);
        spriteRenderer.transform.localScale=Vector3.one*s;
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
