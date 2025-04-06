using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public bool fadeIn;
    public float fadeInTime=1f;
    public Color fadeInColor;
    public Color fadeOutColor;
    public string destinationScene;
    [Header("Load level after countdown")]
    public bool countdown;
    public float countdownTime = 5f;  // Public variable to set the countdown time in seconds
    [Header("Load level after entering trigger volume")]
    public bool trigger;
    [Header("Load level after pressing P")]
    public bool pressP;
    [Header("Transition animation")]
    public float transitionTime;
    public Image image;
    private float timer;
    private float transitionTimer=0f;
    public bool fadingOut=false;
    public bool useUnscaledTime = false; 
    public Image loadingImage;

    void Start()
    {
        if (loadingImage != null)
        {
            Color c = loadingImage.color;
            c.a = 0f;
            loadingImage.color = c;
        }
        timer = countdownTime;  // Initialize the timer with the countdown time

        if(fadeIn){
            // if fade in color is not black set fade in color
            if (Mathf.Abs(fadeInColor.r+fadeInColor.g+fadeInColor.b)>0f){
                image.color=fadeInColor;
            }
            else{   
            Color c=image.color;
            image.color=new Color(c.r,c.g,c.b,1f);
            }
        }

    }

    void Update()
    {
        float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        transitionTimer += deltaTime;
        
        if(fadeIn){
            Color c=image.color;
            if (fadeInColor.a > 0f)
            {
                c=fadeInColor;
            }
            c.a=(fadeInTime-transitionTimer)/fadeInTime;
            image.color=c;
            if(c.a<=0f){
                fadeIn=false;
            }
        }

        if (fadingOut)
        {
            Color c = image.color;
            if (fadeOutColor.a > 0f)
            {
                image.color = new Color(fadeOutColor.r, fadeOutColor.g, fadeOutColor.b, transitionTimer / transitionTime);
            }
            else
            {
                image.color = new Color(c.r, c.g, c.b, transitionTimer / transitionTime);
            }
            if (c.a <= 0f)
            {
                fadeIn = false;
            }
        }

        // Load level after countdown
        timer -= deltaTime;
        if (timer <= 0f && countdown)
        {
            LoadLevel(); 
        }
        // Load level after pressing P
        if (Input.GetKeyDown(KeyCode.P) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && pressP)
        {
            LoadLevel();
        }
        // Load scene transition after pressing O
        if (Input.GetKeyDown(KeyCode.O) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && pressP)
        {
            SceneLoader.instance.LoadScene(SceneManager.GetActiveScene().name, destinationScene, 2f);
        }


    }

    IEnumerator ScreenFlash(float duration){
        Color originalColor = image.color;

        // Lerp to fully opaque and white
        float timer = 0f;
        while (timer < duration / 2f)
        {
            timer += Time.deltaTime;
            float t = timer / (duration / 2f);

            // Lerp RGB to white and alpha to 1
            image.color = Color.Lerp(originalColor, Color.white, t);
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(0f, 1f, t)); // Lerp alpha
            yield return null;
        }

        // Ensure the image is fully white and opaque
        image.color = new Color(1f, 1f, 1f, 1f);

        // Lerp back to the original color and transparency
        timer = 0f;
        while (timer < duration / 2f)
        {
            timer += Time.deltaTime;
            float t = timer / (duration / 2f);

            // Lerp RGB back to the original color and alpha to 0
            image.color = Color.Lerp(Color.white, originalColor, t);
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(1f, 0f, t)); // Lerp alpha
            yield return null;
        }

        // Ensure the image is fully transparent and back to the original color
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }

    public void FadeIn(){
        fadeIn=true;
        transitionTimer=0f;
        Color c=image.color;
        image.color=new Color(c.r,c.g,c.b,1f);
        fadingOut=false;
    }

    public void FadeOut(){
        fadingOut=true;
        transitionTimer=0f;
        fadeIn=false;
    }

    public void LoadLevel(string destination = "")
    {
        if (destination == "")
        {
            destination = destinationScene;
        }
        StartCoroutine(LoadLevelCoroutine(destination));
    }


    // Load level when enter trigger box
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && trigger)
        {
            LoadLevel();
        }
    }

    IEnumerator LoadLevelCoroutine(string levelIndex)
    {
        fadingOut = true;
        transitionTimer = 0f;

        yield return new WaitForSeconds(transitionTime);

        AsyncOperation operation = SceneManager.LoadSceneAsync(levelIndex);
        operation.allowSceneActivation = false; // Prevent the scene from activating immediately
        // set loading image to not transparent while loading
        while (operation.progress<0.9f)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log("Progress: " + progress);

            if (loadingImage != null)
            {
                Color c = loadingImage.color;
                c.a = 1f;
                loadingImage.color = c;
            }
            yield return null; // Wait for the next frame
        }
        operation.allowSceneActivation = true; // Activate the scene
    }

    IEnumerator CrossFadeLoadLevelCoroutine(string levelIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelIndex);

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }
}
