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
    [Header("Crossfade level after pressing O")]
    public bool pressO;
    [Header("Transition animation")]
    public float transitionTime;
    public Image image;
    private float timer;
    [HideInInspector]
    public float transitionTimer=0f;
    public bool fadingOut=false;
    public bool useUnscaledTime = false; 
    public Image loadingImage;
    [Header("Use crossfade scene loader after loading the level")]
    public bool useCrossFade=false;
    public bool crossFadeReverseColor=true;
    public Texture2D crossFadeTexture;
    public float crossFadeTime = 2f;
    public float overrideTime=-1f;
    public float freezeDuration=0f;

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
            float time=fadeInTime;
            if(overrideTime!=-1f) time=overrideTime;
            c.a=(time-transitionTimer)/time;
            image.color=c;
            if(c.a<=0f){
                fadeIn=false;
                overrideTime=-1f;
            }
        }

        if (fadingOut)
        {
            Color c = image.color;
            float time=transitionTime;
            if(overrideTime!=-1f) time=overrideTime;
            if (fadeOutColor.a > 0f)
            {
                image.color = new Color(fadeOutColor.r, fadeOutColor.g, fadeOutColor.b, transitionTimer / time);
            }
            else
            {
                image.color = new Color(c.r, c.g, c.b, transitionTimer / time);
            }
            if (c.a >= 1f)
            {
                fadingOut = false;
                overrideTime=-1f;
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
        if (Input.GetKeyDown(KeyCode.O) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && pressO)
        {
            EnsureSceneLoaderAndLoad(SceneManager.GetActiveScene().name, destinationScene, crossFadeTime);
        }


    }

    public void FadeIn(float time=-1f){
        fadeIn=true;
        transitionTimer=0f;
        Color c=image.color;
        image.color=new Color(c.r,c.g,c.b,1f);
        fadingOut=false;
        overrideTime=time;
    }

    public void FadeOut(float time=-1f){
        fadingOut=true;
        transitionTimer=0f;
        fadeIn=false;
        overrideTime=time;
    }

    public void LoadLevel(string destination = "")
    {
        if (destination == "")
        {
            destination = destinationScene;
        }

        if (useCrossFade)
        {
            EnsureSceneLoaderAndLoad(SceneManager.GetActiveScene().name, destination, crossFadeTime);
        }
        else
        {
            StartCoroutine(LoadLevelCoroutine(destination));
        }
    }

    void EnsureSceneLoaderAndLoad(string currentScene, string destination, float crossFadeTime)
    {
        StartCoroutine(EnsureSceneLoaderCoroutine(currentScene, destination, crossFadeTime));
    }

    IEnumerator EnsureSceneLoaderCoroutine(string currentScene, string destination, float crossFadeTime)
    {
        if (SceneLoader.instance == null)
        {
            Debug.LogWarning("SceneLoader instance is null. Loading SceneLoader scene...");

            // Load the SceneLoader scene additively
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync("LoadScene", LoadSceneMode.Additive);

            // Wait until the SceneLoader scene is fully loaded
            while (!loadOperation.isDone)
            {
                yield return null;
            }
        }

        // Wait for SceneLoader.instance to be initialized
        while (SceneLoader.instance == null)
        {
            yield return null;
        }
        SceneLoader.instance.LoadScene(currentScene, destination, crossFadeTime, crossFadeTexture, crossFadeReverseColor, freezeDuration);
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
