using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceShader : MonoBehaviour
{
    public Shader overrideShader;
    public RenderTexture output;
    void Update()
    {
        // Check if there is a current camera rendering
        if (Camera.current != null)
        {
            // Assign the output render texture to the current camera
            Camera.current.targetTexture = output;
            // 
            // Render the scene by replacing all the "RenderType" shaders with the overrideShader
            Camera.current.RenderWithShader(overrideShader, "RenderType");
            // 
            // Set the output render texture back to null to avoid side effects.
            Camera.current.targetTexture = null;
        }
        else
        {
            Debug.LogWarning("No current camera available for rendering.");
        }
    }
}