using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Naming : MonoBehaviour
{
    public bool generateTrigger=false;
    public GameObject letterA;
    public GameObject[] letterObjects;
    public float spaceBetweenLetters=20f;
    public float lineBreakSize=20f;
    public int maxLettersBeforeLineBreak;


    public GameObject[] buttons;
    public ButtonsRow[] buttonRows;
    private GameObject[] currentButtons;
    private TMP_Text[][] buttonsText;
    private PlayerInput playerInput;

    private int buttonIndex=0;

    public Color textButtonHighlightedColor;
    public Color textButtonIdleColor;
    public Color textButtonInteractingColor;

    private string name="";
    public TMP_Text[] nameLetters;

    private float fontSize;

    void Start()
    {
        letterObjects=new GameObject[26];

        playerInput = FindObjectOfType<PlayerInput>();
        buttonsText=new TMP_Text[buttons.Length][];
        for(int i=0;i<buttons.Length;i++){
            buttonsText[i]=buttons[i].GetComponentsInChildren<TMP_Text>();
        }
        currentButtons=buttons;

        for(var i=0;i<nameLetters.Length;i++){
            TMP_Text[] tt=nameLetters[i].GetComponentsInChildren<TMP_Text>();
            for(int k=0;k<tt.Length;k++){
                tt[k].color=textButtonIdleColor;
            }
        }

        fontSize=buttonsText[0][0].fontSize;
    }

    void Update()
    {
        if(generateTrigger){
            Generate();
            generateTrigger=false;
        }

        NavigateButtons();
        CheckButtons();
        ShowButtons();
    }

    void NavigateButtons(){
        if(playerInput.entering){
            buttonIndex=buttons.Length-1;
        }
        if((playerInput.navigateLeft && !playerInput.prevNavigateLeft)){
            buttonIndex-=1;
        }
        if((playerInput.navigateRight && !playerInput.prevNavigateRight)){
            buttonIndex+=1;
        }
        if((playerInput.navigateDown && !playerInput.prevNavigateDown)){
            int currentRow=-1;
            int inRowIndex=-1;
            for(var i=0;i<buttonRows.Length;i++){
                inRowIndex=GetIndex(buttonRows[i].buttons,buttons[buttonIndex]);
                if(inRowIndex!=-1){
                    currentRow=i;
                    break;
                }
            }
            int newRow=(currentRow+1)%buttonRows.Length;
            inRowIndex=inRowIndex+buttonRows[currentRow].offset-buttonRows[newRow].offset;
            inRowIndex=Mathf.Clamp(inRowIndex,0,buttonRows[newRow].buttons.Length-1);
            buttonIndex=GetIndex(buttons,buttonRows[newRow].buttons[inRowIndex]);
            currentRow=newRow;
        }
        if((playerInput.navigateUp && !playerInput.prevNavigateUp)){
            int currentRow=-1;
            int inRowIndex=-1;
            for(var i=0;i<buttonRows.Length;i++){
                inRowIndex=GetIndex(buttonRows[i].buttons,buttons[buttonIndex]);
                if(inRowIndex!=-1){
                    currentRow=i;
                    break;
                }
            }
            int newRow=(buttonRows.Length+currentRow-1)%buttonRows.Length;
            inRowIndex=inRowIndex+buttonRows[currentRow].offset-buttonRows[newRow].offset;
            inRowIndex=Mathf.Clamp(inRowIndex,0,buttonRows[newRow].buttons.Length-1);
            buttonIndex=GetIndex(buttons,buttonRows[newRow].buttons[inRowIndex]);
            currentRow=newRow;
        }
        buttonIndex=(buttonIndex+currentButtons.Length)%currentButtons.Length;
    }

    void ShowButtons(){
        for(int i=0;i<currentButtons.Length;i++){            
            Color c;
            int value=-1;
            if(i==buttonIndex){
                if(!playerInput.interacting){
                    c=textButtonHighlightedColor;
                }else{
                    c=textButtonInteractingColor;
                }
            }else{
                c=textButtonIdleColor;
            }
            for(int k=0;k<buttonsText[i].Length;k++){
                buttonsText[i][k].color=c;
                if(c==textButtonInteractingColor){ 
                    buttonsText[i][k].fontSize=fontSize-1;
                    string buttonName=currentButtons[buttonIndex].name;
                    if(name.Length<nameLetters.Length && buttonName.Length==1){
                        nameLetters[name.Length].text=buttonName;
                        nameLetters[name.Length].fontSize=fontSize-1;
                    }
                }
                else buttonsText[i][k].fontSize=fontSize;
            }
        }
    }

    void CheckButtons(){
        if(playerInput.prevInteracting && !playerInput.interacting){
            string buttonName=currentButtons[buttonIndex].name;
            if(name.Length<nameLetters.Length && buttonName.Length==1){
                name+=currentButtons[buttonIndex].name;
                nameLetters[name.Length-1].text=buttonName;
                nameLetters[name.Length-1].fontSize=fontSize;
                TMP_Text[] tt=nameLetters[name.Length-1].GetComponentsInChildren<TMP_Text>();
                for(int k=0;k<tt.Length;k++){
                    tt[k].color=textButtonHighlightedColor;
                }
            }else if(buttonName.ToLower()=="back" && name.Length>0){
                nameLetters[name.Length-1].text="";
                TMP_Text[] tt=nameLetters[name.Length-1].GetComponentsInChildren<TMP_Text>();
                for(int k=0;k<tt.Length;k++){
                    tt[k].color=textButtonIdleColor;
                }
                name=name.Substring(0,name.Length-1);
            }else if(buttonName.ToLower()=="done" && name.Length>0){
                DialogueValues.Instance.SaveVariable("mcName",name);
                FindObjectOfType<LevelLoader>().LoadLevel();
            }
        }
    }

    void Generate(){
        string[] letters={"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"};

        RectTransform aRect=letterA.GetComponent<RectTransform>();

        bool isLower=false;
        if(letterA.name==letterA.name.ToLower()){
            isLower=true;
        }

        for(var i=1;i<letters.Length;i++){
            if(isLower){
                letters[i]=letters[i].ToLower();
            }else{
                letters[i]=letters[i].ToUpper();
            }
            if(letterObjects[i]!=null){
                Destroy(letterObjects[i]);
            }
            GameObject letter=Instantiate(letterA);
            letter.name=letters[i];
            letterObjects[i]=letter;
            letter.transform.parent=letterA.transform.parent;
            RectTransform rect=letter.GetComponent<RectTransform>();
            letter.GetComponentInChildren<TMP_Text>().text=letters[i];
            float x=aRect.anchoredPosition.x+spaceBetweenLetters*(i%maxLettersBeforeLineBreak);
            float y=aRect.anchoredPosition.y-Mathf.Floor(i/maxLettersBeforeLineBreak)*lineBreakSize;
            rect.anchoredPosition=new Vector2(x,y);
        }
    }

    int GetIndex(GameObject[] array, GameObject key){
        for(var i=0;i<array.Length;i++){
            if(key==array[i]){
                return i;
            }
        }
        return -1;
    }
}

[System.Serializable]
public struct ButtonsRow{
    public GameObject[] buttons;
    public int offset;
}