
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public string mainMenuScene;
    public GameObject[] controls;
    [Tooltip("Overworld 0, mainact 1, climax 2, cuddle 3")]
    public int controlsIndex;
    private CinemachineVirtualCamera playerCamera;
    private Transform playerCameraRoot; // The target position and rotation for the player
    private CursorLockMode myLockState;
    private PlayerInput playerInput;

    public GameObject[] buttons;
    private TMP_Text[] buttonsText;
    private int buttonIndex=0;

    public Color textButtonHighlightedColor;
    public Color textButtonIdleColor;
    public Color textButtonInteractingColor;

    public UnityEvent[] buttonEvents;
    

    // Start is called before the first frame update
    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        GameIsPaused = false;
        playerCamera = GameObject.Find("PlayerFollowCamera")?.GetComponent<CinemachineVirtualCamera>();
        playerCameraRoot = GameObject.Find("PlayerCameraRoot")?.GetComponent<Transform>();
        controls[controlsIndex].gameObject.SetActive(true);

        buttonsText=new TMP_Text[buttons.Length];
        for(int i=0;i<buttons.Length;i++){
            buttonsText[i]=buttons[i].GetComponentInChildren<TMP_Text>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput.pausing && !playerInput.prevPausing)
        {
            myLockState = UnityEngine.Cursor.lockState;
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        if(GameIsPaused){
            NavigateButtons();
            CheckButtons();
            ShowButtons();
        }
    }

    void NavigateButtons(){
        if((playerInput.navigateDown && !playerInput.prevNavigateDown) || 
            (playerInput.navigateRight && !playerInput.prevNavigateRight)){
            buttonIndex+=1;
            buttonIndex=(buttonIndex+buttons.Length)%buttons.Length;
        }
        if((playerInput.navigateUp && !playerInput.prevNavigateUp) ||
            (playerInput.navigateLeft && !playerInput.prevNavigateLeft)){
            buttonIndex-=1;
            buttonIndex=(buttonIndex+buttons.Length)%buttons.Length;
        }
    }

    //What happens if button press
    void CheckButtons(){
        if(playerInput.prevInteracting && !playerInput.interacting){
            if(buttonIndex>=0 && buttonIndex<buttonEvents.Length){
                buttonEvents[buttonIndex].Invoke();
            }else{
                Debug.LogWarning("Tried to invoke empty event.");
            }
        }
    }

    public void Respawn(){
        FindObjectOfType<Swimmer>().Respawn();
        Resume();
    }

    public void SkipScene(){
        Resume();
        FindObjectOfType<LevelLoader>().LoadLevel();
    }

    public void Invert(){
        FindObjectOfType<PlayerInput>().InvertYAxis();
    }

    public void Menu(){
        Resume();
        FindObjectOfType<LevelLoader>().LoadLevel("GameStart");
    }

    void ShowButtons(){
        for(int i=0;i<buttons.Length;i++){            
            TMP_Text text=buttonsText[i];
            if(i==buttonIndex){
                if(!playerInput.interacting){
                    text.color=textButtonHighlightedColor;
                }else{
                    text.color=textButtonInteractingColor;
                }
                // Image[] images=buttons[i].GetComponentsInChildren<Image>();
                // for(var k=0;k<images.Length;k++){
                //     Color c=images[k].color;
                //     c.a=1f;
                //     images[k].color=c;
                // }
                // buttons[i].GetComponentInChildren<Animator>().speed=1.25f;
            }else{
                text.color=textButtonIdleColor;
                // Image[] images=buttons[i].GetComponentsInChildren<Image>();
                // for(var k=0;k<images.Length;k++){
                //     Color c=images[k].color;
                //     c.a=0.625f;
                //     images[k].color=c;
                // }
                // buttons[i].GetComponentInChildren<Animator>().speed=0.5f;
            }
        }
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void Resume()
    {
        // set timescale back to 1, unlock camera
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = myLockState;


    }


    void Pause()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        // set timescale to 0, lock camera
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        UnityEngine.Cursor.visible = true;

    }
}
