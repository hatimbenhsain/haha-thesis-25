using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    [Tooltip("Location to respawn at")]
    public Transform origin;

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag=="Player"){
            FindObjectOfType<Swimmer>().respawnTransform=transform;
        }
    }
}