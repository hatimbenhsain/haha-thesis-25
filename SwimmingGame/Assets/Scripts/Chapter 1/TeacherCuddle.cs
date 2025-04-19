using UnityEngine;

public class TeacherCuddle : MonoBehaviour
{
    public Transform hand;
    public Transform arm;
    public Animator blink;

    public GameObject[] thingsToDeactivate;
    public float handRetractSpeed=5f;
    private CuddleDialogue dialogue;
    private Tutorial tutorial;
    private bool triggeredDialogue=false;
    void Start()
    {
        dialogue=FindObjectOfType<CuddleDialogue>();
        tutorial=FindObjectOfType<Tutorial>();
    }

    void Update()
    {
        if(!triggeredDialogue && tutorial.index>0){
            dialogue.startStoryTrigger=true;
            triggeredDialogue=true;
        }
        if((bool) dialogue.story.variablesState["retractHandTrigger"]){
            foreach(GameObject g in thingsToDeactivate) g.SetActive(false);
            Vector3 pos=hand.transform.localPosition;
            pos.x=pos.x-Time.deltaTime*handRetractSpeed;
            pos.x=Mathf.Max(pos.x,-11f);
            hand.transform.localPosition=pos;
            pos=arm.transform.localPosition;
            pos.z=pos.z-Time.deltaTime*handRetractSpeed/2f;
            pos.z=Mathf.Max(pos.z,-11f);
            arm.transform.localPosition=pos;
        }

        if((bool) dialogue.story.variablesState["blink"]){
            blink.SetBool("Blink", true);
            dialogue.story.variablesState["blink"]=false;
        }
    }
}
