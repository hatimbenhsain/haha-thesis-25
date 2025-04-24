using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using UnityEditor;

public class SceneLoader : MonoBehaviour
{
    public UnityEngine.UI.RawImage transitionImage;
    public static SceneLoader instance;
    public UnityEngine.UI.Image screenFlashImage;
    private Texture2D initialCrossfadeTexture;
    private bool initialReverseColor = false;

    private void Awake()
    {
        instance = this;
        transitionImage.gameObject.SetActive(false);
        if (transitionImage.material != null){
            initialCrossfadeTexture = transitionImage.material.GetTexture("_Noise") as Texture2D;
            initialReverseColor = transitionImage.material.GetInt("_ReverseColor") > 0.5f; // Check if the reverse color is set
        }
    }


    public void LoadScene(string from, string to, float fadeInDuration, Texture2D crossfadeTexture, bool reverseColor, float freezeDuration)
    {
        if (transitionImage.material != null){
            if (crossfadeTexture != null){
                    transitionImage.material.SetTexture("_Noise", crossfadeTexture);
            }
            transitionImage.material.SetInt("_ReverseColor", reverseColor ? 1 : 0); // Set the reverse color
        }

       
        StartCoroutine(HandleSceneTransition(from, to, fadeInDuration, freezeDuration));
        StartCoroutine(ScreenFlash(0.2f)); // Flash the screen before loading the new scene
    }

    private IEnumerator HandleSceneTransition(string from, string to, float fadeInDuration, float freezeDuration)
    {
        // Wait for the screenshot coroutine to finish
        yield return StartCoroutine(GetCameraScreenshotCoroutine());
        yield return new WaitForSeconds(freezeDuration); // Wait for the freeze duration

        // Unload the previous scene
        Scene s = SceneManager.GetSceneByName(from);
        if (s != null && s.IsValid())
        {
            SceneManager.UnloadSceneAsync(s);
        }

        // Load the new scene asynchronously
        AsyncOperation operation = SceneManager.LoadSceneAsync(to, LoadSceneMode.Additive);

        // Wait for the scene to finish loading
        operation.completed += (_) =>
        {


            Scene newScene = SceneManager.GetSceneByName(to);
            StartCoroutine(FadeInCoroutine(fadeInDuration));
            if (newScene.IsValid())
            {
                SceneManager.SetActiveScene(newScene); // Set the new scene as active
                Debug.Log($"Scene '{to}' is now active.");
            }
            else
            {
                Debug.LogError($"Failed to set active scene: {to} is not valid.");
            }
        };
    }


    private IEnumerator GetCameraScreenshotCoroutine()
    {
        //EditorApplication.isPaused = true;
        yield return new WaitForEndOfFrame();

        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
        Texture2D newScreenshot = new Texture2D(screenshot.width, screenshot.height, TextureFormat.RGB24, false);
        newScreenshot.SetPixels(screenshot.GetPixels());
        newScreenshot.Apply();

        Destroy(screenshot); // Destroy the original screenshot to free memory
        //transitionImage.texture = newScreenshot; // Set the texture of the RawImage to the new screenshot
        if (transitionImage != null && transitionImage.material != null){
            transitionImage.material.SetTexture("_MainTex", newScreenshot);
        }
        transitionImage.gameObject.SetActive(true);
        transitionImage.material.SetFloat("_DissolveAmount", 0f);
    }

    private IEnumerator FadeInCoroutine(float duration)
    {
        yield return new WaitForSeconds(1f); 
        Debug.Log("Fading in...");
        // Lerp the DissolveAmount property from 0 to 1
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float dissolveValue = Mathf.Lerp(0f, 1f, timer / duration);
            transitionImage.material.SetFloat("_DissolveAmount", dissolveValue); // Update the material property
            yield return null;
        }
        Debug.Log("Fade in complete.");
        transitionImage.material.SetFloat("_DissolveAmount", 1f);
    }

    IEnumerator ScreenFlash(float duration){
        yield return new WaitForEndOfFrame();
        Color originalColor = screenFlashImage.color;

        // Lerp to fully opaque and white
        float timer = 0f;
        while (timer < duration / 2f)
        {
            timer += Time.deltaTime;
            float t = timer / (duration / 2f);

            // Lerp RGB to white and alpha to 1
            screenFlashImage.color = Color.Lerp(originalColor, Color.white, t);
            screenFlashImage.color = new Color(screenFlashImage.color.r, screenFlashImage.color.g, screenFlashImage.color.b, Mathf.Lerp(0f, 1f, t)); // Lerp alpha
            yield return null;
        }

        // Ensure the image is fully white and opaque
        screenFlashImage.color = new Color(1f, 1f, 1f, 1f);

        // Lerp back to the original color and transparency
        timer = 0f;
        while (timer < duration / 2f)
        {
            timer += Time.deltaTime;
            float t = timer / (duration / 2f);

            // Lerp RGB back to the original color and alpha to 0
            screenFlashImage.color = Color.Lerp(Color.white, originalColor, t);
            screenFlashImage.color = new Color(screenFlashImage.color.r, screenFlashImage.color.g, screenFlashImage.color.b, Mathf.Lerp(1f, 0f, t)); // Lerp alpha
            yield return null;
        }

        // Ensure the image is fully transparent and back to the original color
        screenFlashImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }

    void OnDestroy()
    {
        if (transitionImage.material != null){
            transitionImage.material.SetTexture("_Noise", initialCrossfadeTexture); // Reset the texture to the original one
            transitionImage.material.SetFloat("_ReverseColor", initialReverseColor ? 1f : 0f); // Reset the reverse color
        }
    }
}
