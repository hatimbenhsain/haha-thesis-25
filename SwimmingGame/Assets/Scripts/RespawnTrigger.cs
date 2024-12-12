using Unity.VisualScripting;
using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    [Tooltip("Location to respawn at")]
    public Transform origin;

    public bool triggered;

    void Update(){
        if(triggered){
            FindObjectOfType<Swimmer>().respawnTransform=transform;
            triggered=false;
        }
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag=="Player"){
            triggered=true;
        }
    }
}