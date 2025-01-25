using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CulledObject : MonoBehaviour
{
    public bool active;

    private Behaviour[] components;

    void Start()
    {
        components=gameObject.GetComponentsInChildren<Behaviour>();
        Activate(active);
    }

    public void Activate(bool b=true){
        active=b;
        foreach(Behaviour component in components){
            if(component!=this) component.enabled=b;
        }
    }
}
