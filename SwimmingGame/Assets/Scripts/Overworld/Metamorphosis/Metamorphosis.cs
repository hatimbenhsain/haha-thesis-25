using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metamorphosis : MonoBehaviour
{
    [HideInInspector]
    public bool metamorphosing = false;

    public virtual void TriggerMetamorphosis()
    {
        metamorphosing = true;
    }
}
