using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnOffThirdOptionBox : MonoBehaviour
{
    public bool activated = false;
    public bool passedFirstDialogue = false;
    public GameObject thirdDialogueOption;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (thirdDialogueOption.activeSelf)
        {
            activated = true;
        }
        if (activated){
            if (!thirdDialogueOption.activeSelf)
            {
                passedFirstDialogue = true;
            }
            if (passedFirstDialogue){
                gameObject.SetActive(false);
            }
        }
    }
}
