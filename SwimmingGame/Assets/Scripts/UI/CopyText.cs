using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CopyText : MonoBehaviour
{
    public TMP_Text reference;
    private TMP_Text text;
    public string prefix;
    public string suffix;

    void Start()
    {
        text=GetComponent<TMP_Text>();
    }

    void Update()
    {
        text.text=prefix+reference.text+suffix;
    }
}
