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
    public float bigPulseStrength = -0.03f;
    public float smallPulseStrength = -0.01f;
    private float pulseStrength;
    public float pulseSpeed = 1f;
    private float initialPulseSize;
    private float initialPulseStrength;
    private float initialPulseSpeed;
    private Vector2 initialPulseCenter;
    public Vector2 pulseCenter = new Vector2(0.5f, 0.5f);
    [Tooltip("Before this time we don't see a pulse")]
    public float minTime=0.1f;
    [Tooltip("After this time we see a full pulse")]
    public float maxTime=0.1f;
    [Tooltip("Follow player on screen?")]
    public bool followPlayer=false;
    public Transform player;
    private Vector3 focalPoint;
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
            shockwavMaterial.SetFloat("_Magnification", 0f);
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
                shockwavMaterial.SetFloat("_Magnification", 0f);
            }else{
                shockwavMaterial.SetFloat("_Magnification",Mathf.Clamp((pulseTimer-minTime)/(maxTime-minTime),0f,1f)*pulseStrength);
            }
            Debug.Log("-");
            Debug.Log(Mathf.Clamp((pulseTimer-minTime)/(maxTime-minTime),0f,1f));
            Debug.Log(pulseTimer);
            Debug.Log(minTime);
            Debug.Log((pulseTimer-minTime)/(maxTime-minTime));            
        }
    }

    public void EmitPulse(float strength=1f)
    {
        pulseStrength=Mathf.Lerp(smallPulseStrength,bigPulseStrength,strength);
        shockwavMaterial.SetFloat("_Size", pulseSize);
        shockwavMaterial.SetFloat("_Magnification", pulseStrength);
        shockwavMaterial.SetFloat("_Speed", pulseSpeed);
        shockwavMaterial.SetVector("_FocalPoint", pulseCenter);

        isPulsing = true;
        pulseTimer = 0f;
        if(followPlayer){
            focalPoint=player.position;
            Vector2 pos=Camera.main.WorldToViewportPoint(player.position);
            pulseCenter=pos;
            shockwavMaterial.SetVector("_FocalPoint", pulseCenter);
        }
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
