using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foreplay2 : MonoBehaviour
{
    public int harmonyCounter=0;
    public int harmonyNumberToReach=10;

    public float cooldownTimer=0f;
    [Tooltip("Do not count harmonies during this time")]
    public float cooldownTime=2f;


    void Start()
    {
        
    }

    void Update()
    {
        NPCSinging[] singers=FindObjectsOfType<NPCSinging>();
        cooldownTimer+=Time.deltaTime;
        if(cooldownTimer>cooldownTime){
            foreach(NPCSinging singer in singers){
                if(singer.gameObject.activeInHierarchy && singer.HasHarmonized()){ 
                    harmonyCounter++;
                    if(harmonyCounter==harmonyNumberToReach){
                        FindObjectOfType<LevelLoader>().LoadLevel();
                    }
                    cooldownTimer=0f;
                    break;
                }
            }
        }
    }
}
