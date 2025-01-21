
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
    public bool inSettings=false;
    public GameObject pauseMenuUI;
    public GameObject settingMenuUI;
    public string mainMenuScene;
    public GameObject[] controls;
    [Tooltip("Overworld 0, mainact 1, climax 2, cuddle 3")]
    public int controlsIndex;
    private CinemachineVirtualCamera playerCamera;
    private Transform playerCameraRoot; // The target position and rotation for the player
    private CursorLockMode myLockState;
    private PlayerInput playerInput;

    public GameObject[] buttons;
    public GameObject[] settingsButtons;
    private GameObject[] currentButtons;
    private TMP_Text[][] buttonsText;
    private int buttonIndex=0;

    public Color textButtonHighlightedColor;
    public Color textButtonIdleColor;
    public Color textButtonInteractingColor;

    public UnityEvent[] buttonEvents;
    public UnityEvent[] settingsEvents;
    private UnityEvent[] events;

    FMOD.Studio.Bus masterBus;

    public TMP_Text desireText;
    

    // Start is called before the first frame update
    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        GameIsPaused = false;
        playerCamera = GameObject.Find("PlayerFollowCamera")?.GetComponent<CinemachineVirtualCamera>();
        playerCameraRoot = GameObject.Find("PlayerCameraRoot")?.GetComponent<Transform>();
        controls[controlsIndex].gameObject.SetActive(true);

        buttonsText=new TMP_Text[buttons.Length][];
        for(int i=0;i<buttons.Length;i++){
            buttonsText[i]=buttons[i].GetComponentsInChildren<TMP_Text>();
        }

        currentButtons=buttons;

        //Getting FMOD master bus
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");

        if(PlayerPrefs.HasKey("soundLevel")){
            masterBus.setVolume(PlayerPrefs.GetFloat("soundLevel"));
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
        pauseMenuUI.SetActive(false);
        if(inSettings){
            settingMenuUI.SetActive(true);
            GetButtons();
            ResetSliderValues();
        }
        else{
            pauseMenuUI.SetActive(true);
            GetButtons();
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

    public void LoadMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void Resume()
    {
        // set timescale back to 1, unlock camera
        pauseMenuUI.SetActive(false);
        settingMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = myLockState;
        inSettings=false;

    }


    void Pause()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        // set timescale to 0, lock camera
        if(inSettings) settingMenuUI.SetActive(true);
        else pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        UnityEngine.Cursor.visible = true;
        GetButtons();
    }

    void OnDestroy(){
        float v;
        masterBus.getVolume(out v);
        PlayerPrefs.SetFloat("soundLevel",v);
    }

    public void ChangeDesire(string s){
        desireText.text=s;
    }
}
