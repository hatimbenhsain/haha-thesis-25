using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//This script is for activating/deactivating things when entering the trigger
public class ActivationTrigger : MonoBehaviour
{
    public string targetTag="Player";
    public bool activate=true;
    public bool destroyAfterUse=true;
    public GameObject[] gameObjectsToToggle;

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag==targetTag){
            foreach(GameObject gameObjectToToggle in gameObjectsToToggle){
                gameObjectToToggle.SetActive(activate);
            }
            if(destroyAfterUse) Destroy(gameObject);
        }
    }
}
