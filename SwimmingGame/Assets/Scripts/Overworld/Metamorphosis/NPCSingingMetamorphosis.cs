using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSingingMetamorphosis : Metamorphosis
{
    private NPCSinging npcSinging;

    public float lerpValue;
    public float targetMaxVolume;


    void Start()
    {
        
    }

    void Update()
    {
        if (metamorphosing)
        {
            npcSinging.maxSingingVolume = Mathf.Lerp(npcSinging.maxSingingVolume, targetMaxVolume, Time.deltaTime * lerpValue);
            if (Mathf.Abs(npcSinging.maxSingingVolume - targetMaxVolume) <= 0.1f)
            {
                npcSinging.maxSingingVolume = targetMaxVolume;
                Destroy(this);
            }
        }
    }
    
    public override void TriggerMetamorphosis()
    {
        base.TriggerMetamorphosis();
        npcSinging = GetComponent<NPCSinging>();
    }
}
