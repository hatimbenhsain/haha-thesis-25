using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MigrationNPC : MonoBehaviour
{
    public float strokeFrequencyVariance=0.2f;
    public RuntimeAnimatorController [] animators;

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
        
    }
}
