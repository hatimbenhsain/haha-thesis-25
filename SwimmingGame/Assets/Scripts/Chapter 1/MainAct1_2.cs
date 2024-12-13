using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAct1_2 : MonoBehaviour
{
    private LevelLoader levelLoader;
    private bool saved;

    void Start(){
        levelLoader=FindObjectOfType<LevelLoader>();
        saved=false;
    }

    void Update()
    {
        if(levelLoader.fadingOut && !saved){
            saved=true;
            DialogueValues.Instance.SaveVariable("sexIntensity",FindObjectOfType<NPCSpring>().currentIntensity);
            Debug.Log("Saved intensity");
        }
    }
}
