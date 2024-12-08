using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MaterialPreset
{
    [Header("Material Parameters")]
    public Color color;
    public float shiftingSpeedX;
    public float shiftingSpeedY;
    public Vector2 tiling;
    public float noiseValue;
    public Color emission;
    public float contrast;
    public float saturation;
    public float smoothness;
    public float normal;
    public float add;
}

public class SexMaterialManager : MonoBehaviour
{
    [Header("Material Pairs")]
    public List<Material> bodyMaterials; // Body material list
    public List<Material> headMaterials; // Head material list

    [Header("Material Presets")]
    public List<MaterialPreset> bodyDefaultPresets;  // Default presets for body
    public List<MaterialPreset> headDefaultPresets;  // Default presets for head
    public List<MaterialPreset> bodyExcitedPresets;  // Excited presets for body
    public List<MaterialPreset> headExcitedPresets;  // Excited presets for head

    [Header("Excitement")]
    public List<float> excitementLevels; // Excitement level for each pair, 0 is MC
    public float excitementLerpSpeed = 1f;

    private void Start()
    {
        // Initialize all materials with their default presets
        for (int i = 0; i < bodyMaterials.Count && i < headMaterials.Count; i++)
        {
            if (i < bodyDefaultPresets.Count)
                ApplyPreset(bodyMaterials[i], bodyDefaultPresets[i]);

            if (i < headDefaultPresets.Count)
                ApplyPreset(headMaterials[i], headDefaultPresets[i]);
        }
    }

    private void Update()
    {
        // Lerp material parameters towards excited presets based on excitement levels
        for (int i = 0; i < bodyMaterials.Count && i < headMaterials.Count; i++)
        {
            Material bodyMaterial = bodyMaterials[i];
            Material headMaterial = headMaterials[i];
            float excitement = Mathf.Clamp01(excitementLevels[i]);

            // Lerp for body materials
            if (i < bodyDefaultPresets.Count && i < bodyExcitedPresets.Count)
            {
                LerpMaterial(bodyMaterial, bodyDefaultPresets[i], bodyExcitedPresets[i], excitement);
            }

            // Lerp for head materials
            if (i < headDefaultPresets.Count && i < headExcitedPresets.Count)
            {
                LerpMaterial(headMaterial, headDefaultPresets[i], headExcitedPresets[i], excitement);
            }
        }
    }

    private void ApplyPreset(Material material, MaterialPreset preset)
    {
        material.SetColor("_Color", preset.color);
        material.SetFloat("_ShiftingSpeedX", preset.shiftingSpeedX);
        material.SetFloat("_ShiftingSpeedY", preset.shiftingSpeedY);
        material.SetVector("_Tiling", preset.tiling);
        material.SetFloat("_NoiseValue", preset.noiseValue);
        material.SetColor("_Emission", preset.emission);
        material.SetFloat("_Contrast", preset.contrast);
        material.SetFloat("_Saturation", preset.saturation);
        material.SetFloat("_Smoothness", preset.smoothness);
        material.SetFloat("_Normal", preset.normal);
        material.SetFloat("_Add", preset.add);
    }

    private void LerpMaterial(Material material, MaterialPreset fromPreset, MaterialPreset toPreset, float t)
    {
        material.SetColor("_Color", Color.Lerp(fromPreset.color, toPreset.color, t));
        material.SetFloat("_ShiftingSpeedX", Mathf.Lerp(fromPreset.shiftingSpeedX, toPreset.shiftingSpeedX, t));
        material.SetFloat("_ShiftingSpeedY", Mathf.Lerp(fromPreset.shiftingSpeedY, toPreset.shiftingSpeedY, t));
        material.SetVector("_Tiling", Vector2.Lerp(fromPreset.tiling, toPreset.tiling, t));
        material.SetFloat("_NoiseValue", Mathf.Lerp(fromPreset.noiseValue, toPreset.noiseValue, t));
        material.SetColor("_Emission", Color.Lerp(fromPreset.emission, toPreset.emission, t));
        material.SetFloat("_Contrast", Mathf.Lerp(fromPreset.contrast, toPreset.contrast, t));
        material.SetFloat("_Saturation", Mathf.Lerp(fromPreset.saturation, toPreset.saturation, t));
        material.SetFloat("_Smoothness", Mathf.Lerp(fromPreset.smoothness, toPreset.smoothness, t));
        material.SetFloat("_Normal", Mathf.Lerp(fromPreset.normal, toPreset.normal, t));
        material.SetFloat("_Add", Mathf.Lerp(fromPreset.add, toPreset.add, t));
    }
}