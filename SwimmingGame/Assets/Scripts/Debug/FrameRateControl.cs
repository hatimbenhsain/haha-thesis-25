using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateControl : MonoBehaviour
{
    public int frameRate;
    void Start()
    {
#if UNITY_EDITOR
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = frameRate;
#endif
    }
}
