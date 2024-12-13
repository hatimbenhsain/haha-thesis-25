using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueValues : MonoBehaviour
{
    public static DialogueValues Instance { get; private set; }

    public List<DialogueVariable> dialogueVariables;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 

        dialogueVariables=new List<DialogueVariable>();
    }

    public object LoadVariable(string name){
        foreach(DialogueVariable dialogueVariable in dialogueVariables){
            if(dialogueVariable.name==name){
                return dialogueVariable.value;
            }
        }
        return null;
    }

    public void SaveVariable(string name, object o){
        DialogueVariable dialogueVariable=new DialogueVariable();
        dialogueVariable.name=name;
        dialogueVariable.value=o;
        dialogueVariables.Add(dialogueVariable);
    }

    public void SaveVariable(string name, int i){
        DialogueVariable dialogueVariable=new DialogueVariable();
        dialogueVariable.name=name;
        dialogueVariable.value=i;
        dialogueVariable.type="int";
        dialogueVariables.Add(dialogueVariable);
    }

    public void SaveVariable(string name, string s){
        DialogueVariable dialogueVariable=new DialogueVariable();
        dialogueVariable.name=name;
        dialogueVariable.value=s;
        dialogueVariable.type="string";
        dialogueVariables.Add(dialogueVariable);
    }

    public void SaveVariable(string name, float f){
        DialogueVariable dialogueVariable=new DialogueVariable();
        dialogueVariable.name=name;
        dialogueVariable.value=f;
        dialogueVariable.type="float";
        dialogueVariables.Add(dialogueVariable);
    }
}

public class DialogueVariable{
    public string name;
    public string type;
    public object value;
}