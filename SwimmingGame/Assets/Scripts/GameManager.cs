using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Tooltip("Various object that we want to be able to manipulate via name through dialogue etc.")]
    public GameObject[] importantObjects;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ActivateObject(string name){
        SwitchObject(name,true);
    }

    public void DeactivateObject(string name){
        SwitchObject(name,false);
    }

    public void SwitchObject(string name, bool b){
        GameObject g=FindObject(name);
        if(g!=null){
            g.SetActive(b);
        }
    }

    public void ToggleObject(string name){
        GameObject g=FindObject(name);
        if(g!=null){
            g.SetActive(!g.activeInHierarchy);
        }
    }

    public GameObject FindObject(string name){
        foreach(GameObject g in importantObjects){
            if(g.name == name){
                return g;
            }
        }
        return null;
    }
}
