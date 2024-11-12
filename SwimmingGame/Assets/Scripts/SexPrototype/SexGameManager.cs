using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SexGameManager : MonoBehaviour
{
    public RopeMeanDistance ropeMeanDistance; 
    public TMP_Text meterText; 
    public LevelLoader levelLoader;
    public float threshold = 0.5f; 
    public float growSpeed = 0.5f;
    public float decaySpeed = 0.5f; 
    private float meterValue = 0f;
    private bool startCounting;

    [Tooltip("Distance between player organ head and npc head.")]
    public float headToHeadDistance;

    public Transform playerHead;
    public Transform npcHead;

    public bool moveOnAfterTresholdReached=false;

    private void Start()
    {
        startCounting = false;
    }

    void Update()
    {
        headToHeadDistance=Vector3.Distance(npcHead.position, playerHead.position);

        float meanDistance = ropeMeanDistance.meanDistance;
        if (meanDistance > threshold && startCounting == false)
        {
            startCounting=true;
        }
        if (startCounting == true)
        {
            if (meanDistance < threshold)
            {
                meterValue = Mathf.Min(meterValue + growSpeed * Time.deltaTime, 100f);
            }
            else
            {
                meterValue = Mathf.Max(meterValue - decaySpeed * Time.deltaTime, 0f);
            }
        }
        // Load level when the meter build to max 
        if (meterValue == 100f && moveOnAfterTresholdReached)
        {
            levelLoader.LoadLevel();
        }

        meterText.text = $"Meter: {meterValue:F2}";
    }
}