using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public static class ResetManager
{
    public static bool reset; // Static bool to track reset state
    public static string GameStartScene = "GameStart"; // Static string to track the scene to load

    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeInitialized()
    {
        Debug.Log("Runtime initialized: First scene loaded: After Awake is called.");
        reset=(PlayerPrefs.GetInt("showcaseMode")==1);
        Debug.Log("showcase mode is "+reset);
    }
}