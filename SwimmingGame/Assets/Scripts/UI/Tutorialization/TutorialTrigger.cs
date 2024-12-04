using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag=="Player" || other.transform.parent.tag=="Player"){
            FindObjectOfType<Tutorial>().EnteredTrigger(GetComponent<Collider>());
            Debug.Log("Entered player");
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.tag=="Player" || other.transform.parent.tag=="Player"){
            FindObjectOfType<Tutorial>().ExitedTrigger(GetComponent<Collider>());
            Debug.Log("Exited player");
        }
    }

}
