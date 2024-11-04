using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CuddleGameManager : MonoBehaviour
{
    public TMP_Text dialogueText;
    public LevelLoader levelLoader;

    private void Start()
    {

    }

    void Update()
    {
        dialogueText.text = "Dialogue stuff";
    }
}
