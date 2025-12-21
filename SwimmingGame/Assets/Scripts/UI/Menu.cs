
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using UnityAtoms.Editor;
using System.Text.RegularExpressions;
using VLB;

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
    private float[] currentButtonsAnimationTimes;
    private TMP_Text[][] buttonsText;
    private int buttonIndex=0;

    [Header("Animation Values")]

    public float buttonHighlightedScale=1.1f;

    public Color textButtonHighlightedColor;
    public Color textButtonIdleColor;
    public Color textButtonInteractingColor;

    [Tooltip("If -1 don't change")]
    public float textButtonhighlightedFontSize=-1f;
    public float textButtonIdleFontsize=-1f;
    [Tooltip("If -1, instanteneous")]
    public float textButtonLerpSpeed=-1f;
    [Tooltip("Used with animation curve for when unselected")]
    public float textButtonLerpOutSpeedModifier=-1f;

    public AnimationCurve animationCurve;

    public float submenu_buttonHighlightedScale=1.1f;
    public float submenu_textButtonLerpSpeed=1.1f;


    [Header("Other Data")]

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

        GetButtons();

        //Getting FMOD master bus
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");

        singingBus = FMODUnity.RuntimeManager.GetBus("bus:/Singing");



        if (PlayerPrefs.HasKey("soundLevel"))
        {
            masterBus.setVolume(PlayerPrefs.GetFloat("soundLevel"));
        }

        Initiate();

        if (textButtonhighlightedFontSize != -1f && textButtonIdleFontsize==-1f)
        {
            textButtonIdleFontsize=buttonsText[0][0].fontSize;
        }

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
            ResetManager.reset=(PlayerPrefs.GetInt("showcaseMode")==1);
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
            GetButtons(settingsButtons,settingsEvents);
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
            GetButtons(chapterSelectButtons,chapterSelectEvents);
        }
        else{
            menuUI.SetActive(true);
            GetButtons();
        }
        buttonIndex=0;
    }

    public void GetButtons(GameObject[] nextButtons=null, UnityEvent[] nextEvents=null){
        if (nextButtons == null)
        {
            nextButtons=buttons;
            nextEvents=buttonEvents;
        }
        currentButtons=nextButtons;
        events=nextEvents;
        buttonsText=new TMP_Text[currentButtons.Length][];
        currentButtonsAnimationTimes=new float[currentButtons.Length];
        for(int i=0;i<currentButtons.Length;i++){
            buttonsText[i]=currentButtons[i].GetComponentsInChildren<TMP_Text>();
            currentButtonsAnimationTimes[i]=0f;
        }
    }

    bool IsInSubMenu()
    {
        if (currentButtons == buttons)
        {
            return false;
        }
        else
        {
            return true;
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
            float targetFontSize;
            float targetScale;
            bool isSlider=IsSlider(currentButtons[i]);
            int value=-1;

            // The following value is used to gauge progress from base to target form (for eg scale).
            // This is used to determine how much sketchiness effect is applied
            float progress=1f;

            float highlightedScale=buttonHighlightedScale;
            float lerpSpeed=textButtonLerpSpeed;
            if(IsInSubMenu()){
                highlightedScale=submenu_buttonHighlightedScale;
                lerpSpeed=submenu_textButtonLerpSpeed;
            }

            if(isSlider) value=GetSliderValue(i);
            if(i==buttonIndex){
                if(!playerInput.interacting || isSlider || buttonsLocked){
                    c=textButtonHighlightedColor;
                }else{
                    c=textButtonInteractingColor;
                }
                targetFontSize=textButtonhighlightedFontSize;
                // Image[] images=buttons[i].GetComponentsInChildren<Image>();
                // for(var k=0;k<images.Length;k++){
                //     Color c=images[k].color;
                //     c.a=1f;
                //     images[k].color=c;
                // }
                // buttons[i].GetComponentInChildren<Animator>().speed=1.25f;
                targetScale=highlightedScale;
                currentButtons[i].transform.SetAsLastSibling();
                currentButtonsAnimationTimes[i]+=Time.deltaTime*lerpSpeed;
            }else{
                c=textButtonIdleColor;
                targetFontSize=textButtonIdleFontsize;
                // Image[] images=buttons[i].GetComponentsInChildren<Image>();
                // for(var k=0;k<images.Length;k++){
                //     Color c=images[k].color;
                //     c.a=0.625f;
                //     images[k].color=c;
                // }
                // buttons[i].GetComponentInChildren<Animator>().speed=0.5f;
                targetScale=1f;
                currentButtonsAnimationTimes[i]-=Time.deltaTime*lerpSpeed*textButtonLerpOutSpeedModifier;
            }

            currentButtonsAnimationTimes[i]=Mathf.Clamp(currentButtonsAnimationTimes[i],0f,1f);

            RectTransform rectTransform=currentButtons[i].GetComponent<RectTransform>();



            if(textButtonLerpSpeed>-1){
                //rectTransform.localScale=Vector3.Lerp(rectTransform.localScale,Vector3.one*targetScale,textButtonLerpSpeed*Time.unscaledDeltaTime);
                rectTransform.localScale=Vector3.Lerp(Vector3.one,Vector3.one*highlightedScale,animationCurve.Evaluate(currentButtonsAnimationTimes[i]));
            }
            else rectTransform.localScale=Vector3.one*targetScale;

            // if(isSlider){
            //     currentButtons[i].transform.Find("Slider").rectTransform.localScale
            // }


            // if(buttonHighlightedScale!=-1f){
            //     progress=1-Mathf.Abs((rectTransform.localScale.x-targetScale)/(buttonHighlightedScale-1f));
            // }

            for(int k=0;k<buttonsText[i].Length;k++){
                // Change button text color
                // if(textButtonLerpSpeed>-1) buttonsText[i][k].color=Color.Lerp(buttonsText[i][k].color,c,textButtonLerpSpeed*Time.unscaledDeltaTime);
                // else{ 
                    buttonsText[i][k].color=c;
                //}

                if(isSlider){
                    if((buttonsText[i][k].name=="Left" && value==0)||(buttonsText[i][k].name=="Right" && value==10)){
                        buttonsText[i][k].color=textButtonIdleColor;
                    }
                }

                // Change font size
                if (targetFontSize != -1)
                {
                    if(textButtonLerpSpeed>-1){
                        buttonsText[i][k].fontSize=Mathf.Lerp(buttonsText[i][k].fontSize,targetFontSize,textButtonLerpSpeed*Time.unscaledDeltaTime);
                        if (Mathf.Abs(buttonsText[i][k].fontSize - targetFontSize) < 1f)
                        {
                            buttonsText[i][k].fontSize=targetFontSize;
                        }
                    }
                    else buttonsText[i][k].fontSize=targetFontSize;
                    //buttonsText[i][k].fontSize=Mathf.Round(buttonsText[i][k].fontSize);
                }

                if(buttonsText[i][k].text.Contains("<") && buttonsText[i][k].text.Substring(buttonsText[i][k].text.IndexOf("<")).Contains(">")){
                    int bracketIndex=buttonsText[i][k].text.IndexOf(">");
                    if (progress <= .625)
                    {
                        buttonsText[i][k].text="<sketchy3>"+buttonsText[i][k].text.Substring(bracketIndex+1);
                    }
                    else if (i == buttonIndex)
                    {
                        buttonsText[i][k].text="<sketchy2>"+buttonsText[i][k].text.Substring(bracketIndex+1);
                    }
                    else
                    {
                        buttonsText[i][k].text="<sketchy>"+buttonsText[i][k].text.Substring(bracketIndex+1);
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
                    string txt=buttonsText[i][k].text;
                    if(txt.Contains("<") && txt.Substring(txt.IndexOf("<")).Contains(">"))
                    {
                        txt=txt.Substring(txt.IndexOf(">")+1);
                    }
                    txt=Regex.Replace(txt, "[^0-9]", "");
                    return int.Parse(txt);
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
                string txt=buttonsText[i][k].text;
                if(txt.Contains("<") && txt.Substring(txt.IndexOf("<")).Contains(">"))
                {
                    buttonsText[i][k].text=txt.Substring(0,txt.IndexOf(">")+1)+value.ToString();
                }else{
                    buttonsText[i][k].text = value.ToString();
                }
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
