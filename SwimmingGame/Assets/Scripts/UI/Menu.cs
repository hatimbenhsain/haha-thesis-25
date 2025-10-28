
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class Menu : MonoBehaviour
{
    public bool active=false;
    public static bool GameIsPaused = false;
    public bool inSettings=false;
    public bool inChapterSelect=false;
    public GameObject menuUI;
    public GameObject settingMenuUI;
    public GameObject chapterSelectMenuUI;
    public string mainMenuScene;
    public GameObject[] controls;
    [Tooltip("Overworld 0, mainact 1, climax 2, cuddle 3")]
    public int controlsIndex;
    private CinemachineVirtualCamera playerCamera;
    private Transform playerCameraRoot; // The target position and rotation for the player
    [HideInInspector]
    public CursorLockMode myLockState;
    [HideInInspector] 
    public PlayerInput playerInput;
    private bool buttonsLocked=false;

    public GameObject[] buttons;
    public GameObject[] settingsButtons;
    public GameObject[] chapterSelectButtons;
    private GameObject[] currentButtons;
    private TMP_Text[][] buttonsText;
    private int buttonIndex=0;

    public Color textButtonHighlightedColor;
    public Color textButtonIdleColor;
    public Color textButtonInteractingColor;

    public UnityEvent[] buttonEvents;
    public UnityEvent[] settingsEvents;
    public UnityEvent[] chapterSelectEvents;
    private UnityEvent[] events;

    FMOD.Studio.Bus masterBus;

    FMOD.Studio.Bus singingBus;

    public TMP_Text desireText;


    [Header("Debug")]
    public TMP_Text debugTMP;

    // Start is called before the first frame update
    public void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        GameIsPaused = false;
        playerCamera = GameObject.Find("PlayerFollowCamera")?.GetComponent<CinemachineVirtualCamera>();
        playerCameraRoot = GameObject.Find("PlayerCameraRoot")?.GetComponent<Transform>();
        controls[controlsIndex].gameObject.SetActive(true);

        buttonsText = new TMP_Text[buttons.Length][];
        for (int i = 0; i < buttons.Length; i++)
        {
            buttonsText[i] = buttons[i].GetComponentsInChildren<TMP_Text>();
        }

        currentButtons = buttons;

        //Getting FMOD master bus
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");

        singingBus = FMODUnity.RuntimeManager.GetBus("bus:/Singing");



        if (PlayerPrefs.HasKey("soundLevel"))
        {
            masterBus.setVolume(PlayerPrefs.GetFloat("soundLevel"));
        }

        Initiate();

    }
    
    public virtual void Initiate(){

    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (active)
        {
            NavigateButtons();
            CheckButtons();
            ShowButtons();
        }

        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.S))
        {
            int i = PlayerPrefs.GetInt("showcaseMode");
            if (i == 0) i = 1;
            else i = 0;
            PlayerPrefs.SetInt("showcaseMode", i);
            debugTMP.text = "showcase mode: " + i;
            debugTMP.gameObject.SetActive(true);
            StartCoroutine(HideDebugTMP());
        }
    }
    
    IEnumerator HideDebugTMP()
    {
        yield return new WaitForSeconds(2f);
        debugTMP.gameObject.SetActive(false);
    }

    void NavigateButtons(){
        if((playerInput.navigateDown && !playerInput.prevNavigateDown)){
            buttonIndex+=1;
        }
        if((playerInput.navigateUp && !playerInput.prevNavigateUp)){
            buttonIndex-=1;
        }
        buttonIndex=(buttonIndex+currentButtons.Length)%currentButtons.Length;
    }

    //What happens if button press
    void CheckButtons(){
        if(!IsSlider(currentButtons[buttonIndex])){
            if(playerInput.prevInteracting && !playerInput.interacting){
                if(buttonIndex>=0 && buttonIndex<events.Length && !buttonsLocked){
                    events[buttonIndex].Invoke();
                }else{
                    Debug.LogWarning("Tried to invoke empty event.");
                }
            }
        }else{
            if((playerInput.navigateLeft && !playerInput.prevNavigateLeft)){
                SetValue(buttonIndex,Mathf.Clamp(GetSliderValue(buttonIndex)-1,0,10));
            }
            if((playerInput.navigateRight && !playerInput.prevNavigateRight)){
                SetValue(buttonIndex,Mathf.Clamp(GetSliderValue(buttonIndex)+1,0,10));
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

    public void MainMenu(){
        Resume();
        FindObjectOfType<LevelLoader>().LoadLevel(ResetManager.GameStartScene);
    }

    public void LoadLevel(string levelName){
        buttonsLocked = true;
        Resume();
        FindObjectOfType<LevelLoader>().LoadLevel(levelName);
    }

    public void SetSoundLevel(float f){
        masterBus.setVolume(f);
    }

    public void SetPhysicsPerformance(){

    }

    public void SetWaterTemperature(){

    }

    public void Settings(bool b){
        inSettings=b;
        settingMenuUI.SetActive(false);
        menuUI.SetActive(false);
        if(inSettings){
            settingMenuUI.SetActive(true);
            GetButtons();
            ResetSliderValues();
        }
        else{
            menuUI.SetActive(true);
            GetButtons();
        }
        buttonIndex=0;
    }

    public void ChapterSelect(bool b){
        inChapterSelect=b;
        chapterSelectMenuUI.SetActive(false);
        menuUI.SetActive(false);
        if(inChapterSelect){
            chapterSelectMenuUI.SetActive(true);
            GetChapterSelectButtons();
        }
        else{
            menuUI.SetActive(true);
            GetChapterSelectButtons();
        }
        buttonIndex=0;
    }

    public void GetButtons(){
        currentButtons=inSettings?settingsButtons:buttons;
        events=inSettings?settingsEvents:buttonEvents;
        buttonsText=new TMP_Text[currentButtons.Length][];
        for(int i=0;i<currentButtons.Length;i++){
            buttonsText[i]=currentButtons[i].GetComponentsInChildren<TMP_Text>();
        }
    }

    void GetChapterSelectButtons()
    {
        currentButtons=inChapterSelect?chapterSelectButtons:buttons;
        events=inChapterSelect?chapterSelectEvents:buttonEvents;
        buttonsText=new TMP_Text[currentButtons.Length][];
        for(int i=0;i<currentButtons.Length;i++){
            buttonsText[i]=currentButtons[i].GetComponentsInChildren<TMP_Text>();
        }
    }

    bool IsSlider(GameObject button){
        if(button.name.Contains("Slider")){
            return true;
        }
        else{
            return false;
        }
    }

    void ShowButtons(){
        for(int i=0;i<currentButtons.Length;i++){            
            Color c;
            bool isSlider=IsSlider(currentButtons[i]);
            int value=-1;
            if(isSlider) value=GetSliderValue(i);
            if(i==buttonIndex){
                if(!playerInput.interacting || isSlider || buttonsLocked){
                    c=textButtonHighlightedColor;
                }else{
                    c=textButtonInteractingColor;
                }
                // Image[] images=buttons[i].GetComponentsInChildren<Image>();
                // for(var k=0;k<images.Length;k++){
                //     Color c=images[k].color;
                //     c.a=1f;
                //     images[k].color=c;
                // }
                // buttons[i].GetComponentInChildren<Animator>().speed=1.25f;
            }else{
                c=textButtonIdleColor;
                // Image[] images=buttons[i].GetComponentsInChildren<Image>();
                // for(var k=0;k<images.Length;k++){
                //     Color c=images[k].color;
                //     c.a=0.625f;
                //     images[k].color=c;
                // }
                // buttons[i].GetComponentInChildren<Animator>().speed=0.5f;
            }
            for(int k=0;k<buttonsText[i].Length;k++){
                buttonsText[i][k].color=c;
                if(isSlider){
                    if((buttonsText[i][k].name=="Left" && value==0)||(buttonsText[i][k].name=="Right" && value==10)){
                        buttonsText[i][k].color=textButtonIdleColor;
                    }
                }
            }
        }
    }

    void ResetSliderValues(){
        for(int i=0;i<currentButtons.Length;i++){ 
            if(IsSlider(currentButtons[i])){
                if(i<events.Length && events[i].GetPersistentEventCount()>0){
                    string eventName=events[i].GetPersistentMethodName(0);
                    int value=10;
                    switch(eventName){
                        case nameof(SetSoundLevel):
                            float v;
                            masterBus.getVolume(out v);
                            value=(int)(v*10);
                            break;
                    }
                    SetSliderValue(i,value);
                }
            }
        }
    }

    int GetSliderValue(int i){
        if(IsSlider(currentButtons[i])){
            for(int k=0;k<buttonsText[i].Length;k++){
                if(buttonsText[i][k].name=="Value"){
                    return int.Parse(buttonsText[i][k].text);
                }
            }
        }
        return -1;
    }

    void SetValue(int i,int value){
        if(buttonIndex>=0 && buttonIndex<events.Length && events[buttonIndex].GetPersistentEventCount()>0){
            string eventName=events[buttonIndex].GetPersistentMethodName(0);
            switch(eventName){
                case nameof(SetSoundLevel):
                    SetSoundLevel(((float)value)/10);
                    break;
            }
        }
        SetSliderValue(i,value);
    }

    void SetSliderValue(int i, int value)
    {
        for (int k = 0; k < buttonsText[i].Length; k++)
        {
            if (buttonsText[i][k].name == "Value")
            {
                buttonsText[i][k].text = value.ToString();
            }
        }
    }
    
    public void QuitGame()
    {
        //Resume();
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void Resume()
    {
        // set timescale back to 1, unlock camera
        menuUI.SetActive(false);
        settingMenuUI.SetActive(false);
        chapterSelectMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = myLockState;
        inSettings = false;
        inChapterSelect=false;
        singingBus.setVolume(1f);

    }


    public void Activate()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        // set timescale to 0, lock camera
        if(inSettings) settingMenuUI.SetActive(true);
        else menuUI.SetActive(true);
        Time.timeScale = 0f;
        UnityEngine.Cursor.visible = true;
        GetButtons();
        singingBus.setVolume(0f);
    }

    void OnDestroy(){
        float v;
        masterBus.getVolume(out v);
        PlayerPrefs.SetFloat("soundLevel",v);
        singingBus.setVolume(1f);
    }

}
