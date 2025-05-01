using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainAct1_1 : MonoBehaviour
{
    public SpringController springController;
    public LevelLoader levelLoader;

    public Image overlay;
    public float lerpSpeed=10f;

    public bool endTriggered=false;
    private bool ending=false;

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.tag=="Player" || other.transform.parent.tag=="Player"){
            endTriggered=true;
        }
    }

    private void Update()
    {
        if(endTriggered){
            if(!ending){
                FindObjectOfType<Dialogue>().startStoryTrigger=true;
                springController.SetCanMove(false);
                StartCoroutine(waitAndLoadLevel());
                ending=true;
            }
            Color c=overlay.color;
            c.a=Mathf.Lerp(c.a,1f,lerpSpeed*Time.deltaTime);
            overlay.color=c;
        }
    }

    IEnumerator waitAndLoadLevel(){
        yield return new WaitForSeconds(1f);
        levelLoader.LoadLevel();
    }
}
