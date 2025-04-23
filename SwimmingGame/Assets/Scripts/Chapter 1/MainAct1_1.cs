using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAct1_1 : MonoBehaviour
{
    public SpringController springController;
    private void OnTriggerExit(Collider other) {
        if(other.gameObject.tag=="Player" || other.transform.parent.tag=="Player"){
            FindObjectOfType<Dialogue>().startStoryTrigger=true;
            springController.SetCanMove(false);
        }
    }
}
