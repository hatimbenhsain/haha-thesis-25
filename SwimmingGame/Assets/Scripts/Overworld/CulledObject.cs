using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CulledObject : MonoBehaviour
{
    public bool active;

    private Behaviour[] components;

    private bool started=false;

    void Start()
    {
        components=gameObject.GetComponentsInChildren<Behaviour>();
        Activate(active);
    }

    void LateUpdate()
    {

    }

    public void Activate(bool b=true){
        active=b;
        foreach(Behaviour component in components){
            if(component!=this) component.enabled=b;
        }
    }
}
