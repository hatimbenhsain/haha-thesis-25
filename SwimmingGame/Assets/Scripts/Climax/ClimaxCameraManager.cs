using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ClimaxCameraManager : MonoBehaviour
{
    [Header("Camera & Effects")] public CinemachineVirtualCamera climaxCamera; // Assign a dedicated climax cam
    public float slowMoTimeScale = 0.2f; // Slow motion factor
    public float effectDuration = 2f; // How long the sequence lasts
    public AnimationCurve cameraZoomCurve; // Custom zoom animation curve
    public float maxZoomOut = 10f; // Maximum zoom out during the effect
    public bool isClimaxCompleted = false;
    private float originalTimeScale;
    private Cinemachine3rdPersonFollow camFollow;
    private float effectTimer = 0f;
    private bool effectActive = false;
    private float initialZoom;

    void Start()
    {
        originalTimeScale = Time.timeScale;
        camFollow = climaxCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
    }

    void Update()
    {
        if (isClimaxCompleted && !effectActive)
        {
            StartClimaxCompletion();
        }

        if (effectActive)
        {
            UpdateClimaxCompletionEffect();
        }
    }

    public void TriggerClimaxCompletion()
    {
        isClimaxCompleted = true;
    }

    private void StartClimaxCompletion()
    {
        effectActive = true;
        effectTimer = 0f;
        Time.timeScale = slowMoTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        if (camFollow)
        {
            initialZoom = camFollow.CameraDistance;
        }
    }

    private void UpdateClimaxCompletionEffect()
    {
        effectTimer += Time.unscaledDeltaTime;
        float t = effectTimer / effectDuration;

        if (camFollow)
        {
            camFollow.CameraDistance = Mathf.Lerp(initialZoom, maxZoomOut, cameraZoomCurve.Evaluate(t));
        }

        if (effectTimer >= effectDuration)
        {
            EndClimaxCompletion();
        }
    }

    private void EndClimaxCompletion()
    {
        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = 0.02f;
        effectActive = false;
        LoadNextLevel();
    }

    void LoadNextLevel()
    {
        Debug.Log("Loading next level..."); // Replace with your actual level load function
    }

}
