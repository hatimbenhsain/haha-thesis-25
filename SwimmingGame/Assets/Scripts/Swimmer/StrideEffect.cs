using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrideEffect : MonoBehaviour
{
    public Animator animator;
    private bool orphaned=false;
    public float timeBeforeOrphaning=.5f;

    void Update()
    {
        if(!orphaned && animator.GetCurrentAnimatorStateInfo(0).normalizedTime*animator.GetCurrentAnimatorStateInfo(0).length>=timeBeforeOrphaning){
            orphaned=true;
            transform.parent=transform.parent.parent.parent;
        }
    }
}
