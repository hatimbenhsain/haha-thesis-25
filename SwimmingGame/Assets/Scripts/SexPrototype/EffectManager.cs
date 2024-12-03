using UnityEngine;
using UnityEngine.Rendering;

public class EffectManager : MonoBehaviour
{

    public Volume globalVolume;
    public Light directionalLight;
    public Camera mainCamera;
    public Material material1;
    public Material material2;
    [Header("Test Values")]
    public bool testModeOn;
    public Color fogColorTest;
    public Color directionalLightColorTest;

    private Color fogColor;

    void Start()
    {
        if (RenderSettings.fog)
        {
            fogColor = RenderSettings.fogColor;
        }
    }

    private void Update()
    {
        if (testModeOn)
        {
            RenderSettings.fogColor = fogColorTest;
            directionalLight.color = directionalLightColorTest;
        }

    }
}

