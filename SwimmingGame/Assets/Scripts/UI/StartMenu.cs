using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class StartMenu : MonoBehaviour
{
    public bool inSettings=false;
    public bool inChapterSelect=false;
    public GameObject mainMenuUI;
    public GameObject settingMenuUI;
    public GameObject chapterSelectMenuUI;
    public string mainMenuScene;
    public GameObject[] controls;
    [Tooltip("Overworld 0, mainact 1, climax 2, cuddle 3")]
    public int controlsIndex;

    public GameObject[] buttons;
    public GameObject[] settingsButtons;
    public GameObject[] chapterSelectButtons;
    private GameObject[] currentButtons;
    private TMP_Text[][] buttonsText;
    private int buttonIndex = 0;

    public Color textButtonHighlightedColor;
    public Color textButtonIdleColor;
    public Color textButtonInteractingColor;

    public UnityEvent[] buttonEvents;
    public UnityEvent[] settingsEvents;
    public UnityEvent[] chapterSelectEvents;
    private UnityEvent[] events;
    
    private PlayerInput playerInput;
    public static bool GameIsPaused = false;
    private CursorLockMode myLockState;

    FMOD.Studio.Bus masterBus;
    FMOD.Studio.Bus singingBus;


    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();

        controls[controlsIndex].gameObject.SetActive(true);

        buttonsText=new TMP_Text[buttons.Length][];
        for(int i=0;i<buttons.Length;i++){
            buttonsText[i]=buttons[i].GetComponentsInChildren<TMP_Text>();
        }

        currentButtons=buttons;

        //Getting FMOD master bus
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");

        singingBus = FMODUnity.RuntimeManager.GetBus("bus:/Singing");

        

        if(PlayerPrefs.HasKey("soundLevel")){
            masterBus.setVolume(PlayerPrefs.GetFloat("soundLevel"));
        }
        InitiateMenu();
        myLockState = UnityEngine.Cursor.lockState;

    }

    void Update()
    {
        NavigateButtons();
        CheckButtons();
        ShowButtons();
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
            if((playerInput.prevInteracting && !playerInput.interacting) || (playerInput.prevEntering && !playerInput.entering)){
                if(buttonIndex>=0 && buttonIndex<events.Length){
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


    public void Invert(){
        FindObjectOfType<PlayerInput>().InvertYAxis();
    }


    public void LoadLevel(string levelName){
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
        mainMenuUI.SetActive(false);
        if(inSettings){
            settingMenuUI.SetActive(true);
            GetButtons();
            ResetSliderValues();
        }
        else{
            mainMenuUI.SetActive(true);
            GetButtons();
        }
        buttonIndex=0;
    }

    public void ChapterSelect(bool b){
        inChapterSelect=b;
        chapterSelectMenuUI.SetActive(false);
        mainMenuUI.SetActive(false);
        if(inChapterSelect){
            chapterSelectMenuUI.SetActive(true);
            GetChapterSelectButtons();
        }
        else{
            mainMenuUI.SetActive(true);
            GetChapterSelectButtons();
        }
        buttonIndex=0;
    }

    void GetButtons(){
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
                if(!playerInput.interacting || isSlider){
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

    void SetSliderValue(int i,int value){
        for(int k=0;k<buttonsText[i].Length;k++){
            if(buttonsText[i][k].name=="Value"){
                buttonsText[i][k].text=value.ToString();
            }
        }
    }


    public void QuitGame()
    {
        Resume();
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    void InitiateMenu()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        if(inSettings) settingMenuUI.SetActive(true);
        else mainMenuUI.SetActive(true);
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

    public void Resume()
    {
        // set timescale back to 1, unlock camera
        settingMenuUI.SetActive(false);
        chapterSelectMenuUI.SetActive(false);
        Time.timeScale = 1f;
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = myLockState;
        inSettings=false;
        inChapterSelect=false;
        singingBus.setVolume(1f);

    }

}
