using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueView : MonoBehaviour
{
    [Tooltip("Object to display interlocutor dialogue line.")]
    public GameObject interlocutorTextBox;
    // TMP object for the interlocutor spoken line
    [Tooltip("Object to display player dialogue line.")]
    public GameObject playerTextBox;
    [Tooltip("Textboxes for individual characters")]
    public InterlocutorBox[] interlocutorsTextBoxes;

    public GameObject standardTextBox;
    public GameObject floralTextBox;
    public GameObject boneTextBox;
    public GameObject[] choiceTextBoxes;
}
