using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
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
            c.a=(fadeInTime-transitionTimer)/fadeInTime;
            image.color=c;
            if(c.a<=0f){
                fadeIn=false;
            }
        }

        if(fadingOut){
            Color c=image.color;
            if (Mathf.Abs(fadeOutColor.r+fadeOutColor.g+fadeOutColor.b) > 0f)
            {
                image.color = fadeOutColor;
            }
            else{
                image.color=new Color(c.r,c.g,c.b,transitionTimer/transitionTime);
            }
            if(c.a<=0f){
                fadeIn=false;
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
        //transition.SetTrigger("Start");
        fadingOut=true;
        transitionTimer=0f;

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);

    }
}
