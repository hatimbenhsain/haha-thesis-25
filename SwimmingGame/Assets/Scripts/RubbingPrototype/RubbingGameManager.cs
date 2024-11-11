using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RubbingGameManager : MonoBehaviour
{
    public RopeMeanDistance ropeMeanDistance;
    public TMP_Text meterText;
    public LevelLoader levelLoader;
    public float threshold = 0.5f;
    public float growSpeed = 0.5f;
    public float decaySpeed = 0.5f;
    private float meterValue = 0f;
    private bool startCounting;

    private void Start()
    {
        startCounting = false;
    }

    void Update()
    {
        float meanDistance = ropeMeanDistance.meanDistance;
        if (meanDistance > threshold && startCounting == false)
        {
            startCounting = true;
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
        if (meterValue == 100f)
        {
            levelLoader.LoadLevel();
        }

        meterText.text = $"Meter: {meterValue:F2}";
    }
}
