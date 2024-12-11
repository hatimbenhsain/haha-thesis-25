using UnityEngine;

public class TeacherCuddle : MonoBehaviour
{
    public Transform hand;
    public float handRetractSpeed=5f;
    private CuddleDialogue dialogue;
    void Start()
    {
        dialogue=FindObjectOfType<CuddleDialogue>();
    }

    void Update()
    {
        if((bool) dialogue.story.variablesState["retractHandTrigger"]){
            Vector3 pos=hand.transform.localPosition;
            pos.z=pos.z-Time.deltaTime*handRetractSpeed;
            pos.z=Mathf.Max(pos.z,-11f);
            hand.transform.localPosition=pos;
        }
    }
}
