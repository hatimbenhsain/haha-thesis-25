using UnityEngine;
using UnityEngine.Rendering;

public class EffectManager : MonoBehaviour
{

    public Volume globalVolume;
    public Light directionalLight;
    public Camera mainCamera;
    public Material material1;
    public Material material2;

    private Color fogColor;

    void Start()
    {
        if (RenderSettings.fog)
        {
            fogColor = RenderSettings.fogColor;
        }
    }

}

