using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShockwavePulseTrigger : MonoBehaviour
{
    public Material shockwavMaterial;
    public float pulseDuration = 1f;

    private float pulseTimer = 0f;
    public float pulseSize = 0.08f;
    public float pulseStrength = -0.03f;
    public float pulseSpeed = 1f;
    private float initialPulseSize;
    private float initialPulseStrength;
    private float initialPulseSpeed;
    private Vector2 initialPulseCenter;
    public Vector2 pulseCenter = new Vector2(0.5f, 0.5f);
    private bool isPulsing = false;
    public bool triggerPulse = false;


    void Start()
    {
        if (shockwavMaterial != null)
        {
            initialPulseSize = shockwavMaterial.GetFloat("_Size");
            initialPulseStrength = shockwavMaterial.GetFloat("_Magnification");
            initialPulseSpeed = shockwavMaterial.GetFloat("_Speed");
            initialPulseCenter = shockwavMaterial.GetVector("_FocalPoint");
            shockwavMaterial.SetFloat("_Size", pulseSize);
            shockwavMaterial.SetFloat("_Magnification", pulseStrength);
            shockwavMaterial.SetFloat("_Speed", pulseSpeed);
            shockwavMaterial.SetVector("_FocalPoint", pulseCenter);
        }
    }

    void Update()
    {
        if (shockwavMaterial == null) return;

        // Check for external trigger
        if (triggerPulse)
        {
            EmitPulse();
            triggerPulse = false; // reset after triggering
        }

        if (isPulsing)
        {
            pulseTimer += Time.deltaTime;
            shockwavMaterial.SetFloat("_PulseTime", pulseTimer / pulseDuration);

            if (pulseTimer >= pulseDuration)
            {
                isPulsing = false;
                pulseTimer = 0f;
                shockwavMaterial.SetFloat("_PulseTime", 0f);
            }
        }
    }

    public void EmitPulse()
    {
        isPulsing = true;
        pulseTimer = 0f;
    }

    void OnDestroy()
    {
        if(shockwavMaterial != null)
        {
            shockwavMaterial.SetFloat("_Size", initialPulseSize);
            shockwavMaterial.SetFloat("_Magnification", initialPulseStrength);
            shockwavMaterial.SetFloat("_Speed", initialPulseSpeed);
            shockwavMaterial.SetVector("_FocalPoint", initialPulseCenter);
        }
    }
}
