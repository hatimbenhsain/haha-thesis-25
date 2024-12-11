using UnityEngine;

public class TeacherCuddle : MonoBehaviour
{
    public Transform hand;
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
            Vector3 pos=hand.transform.localPosition;
            pos.z=pos.z-Time.deltaTime*handRetractSpeed;
            pos.z=Mathf.Max(pos.z,-11f);
            hand.transform.localPosition=pos;
        }
    }
}
