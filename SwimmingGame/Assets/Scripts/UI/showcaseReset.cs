using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public static class ResetManager
{
    public static bool reset = true; // Static bool to track reset state
}

public class showcaseReset : MonoBehaviour
{
    public bool enableReset = false; // Bool to control whether reset is enabled

    public void SetResetEnabled(bool isEnabled)
    {
        enableReset = isEnabled; // Set the reset flag
    }

    void Update()
    {
        // If enableReset is true, set ResetManager.reset to true
        if (enableReset)
        {
            ResetManager.reset = true;
            Debug.Log("Reset is enabled.");
        }

      
    }

  
}
