using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    [SerializeField]
	private TextAsset inkJSONAsset = null;
    private Story story;

    public string currentKnotName;

    public string displayText; //Current text to display

    public int currentChoiceIndex=0; //Currently selected choice index
    [Tooltip("Name of current person talking")]
    public string talker="";

    [Header("Canvas objects")]
    [Tooltip("Group to completely show or hide when start/stop dialogue.")]
    public GameObject canvasParent;
    [Tooltip("Object to display interlocutor dialogue line.")]
    public GameObject interlocutorTextBox;
    // TMP object for the interlocutor spoken line
    private TMP_Text interlocutorLineTMP;
    [Tooltip("Object to display player dialogue line.")]
    public GameObject playerTextBox;
    // TMP object for the spoken line
    private TMP_Text playerLineTMP;
    private GameObject textBox;
    private TMP_Text lineTMP;
    [Tooltip("Objects to display choices.")]
    public GameObject[] choiceTextBoxes;
    private TMP_Text[] choiceTMPs;

    private PlayerInput playerInput;

    public bool startStoryTrigger;

    public bool inDialogue=false;
    private bool choicePicked=false;

    public NPCOverworld npcInterlocutor;



    void Awake()
    {
        playerInput=FindObjectOfType<PlayerInput>();
        choiceTMPs=new TMP_Text[choiceTextBoxes.Length];
        for(int i=0;i<choiceTextBoxes.Length;i++){
            choiceTMPs[i]=choiceTextBoxes[i].GetComponentInChildren<TMP_Text>();
        }
        interlocutorLineTMP=interlocutorTextBox.GetComponentInChildren<TMP_Text>();
        playerLineTMP=playerTextBox.GetComponentInChildren<TMP_Text>();

        lineTMP=interlocutorLineTMP;
        textBox=interlocutorTextBox;

        HideText();
        HideChoices();
    }

    void Update(){
        if(inDialogue){
            //Handling player input: Continuing/Picking
            if(playerInput.interacting && !playerInput.prevInteracting){
                if(story.canContinue){
                    Continue();
                    if(story.currentChoices.Count==0){
                        HideChoices();
                    }
                    Debug.Log("Continue");
                }else if(story.currentChoices.Count>0 && currentChoiceIndex<=story.currentChoices.Count 
                    && currentChoiceIndex>=0){
                    PickChoice(currentChoiceIndex);
                    Debug.Log("Pick choice");
                }else{
                    EndDialogue();
                    Debug.Log("End Dialogue");
                }
            }

            //Handling player input: picking choice
            if(story.currentChoices.Count>0){
                if((playerInput.movingDown && !playerInput.prevMovingDown) || 
                    (playerInput.movingRight && !playerInput.prevMovingRight)){
                    currentChoiceIndex+=1;
                    currentChoiceIndex=(currentChoiceIndex+story.currentChoices.Count)%story.currentChoices.Count;
                }
                if((playerInput.movingUp && !playerInput.prevMovingUp) ||
                    (playerInput.movingLeft && !playerInput.prevMovingLeft)){
                    currentChoiceIndex-=1;
                    currentChoiceIndex=(currentChoiceIndex+story.currentChoices.Count)%story.currentChoices.Count;
                }
            }

            //Move on with picking
            if(inDialogue && choicePicked && story.currentChoices.Count>0 && 
                currentChoiceIndex<=story.currentChoices.Count && currentChoiceIndex>=0){
                story.ChooseChoiceIndex(currentChoiceIndex);
                Continue();
                choicePicked=false;
                if(story.currentChoices.Count==0){
                    HideChoices();
                    currentChoiceIndex=-1;
                }
            }

            //Showing/Hiding text
            if(inDialogue){
                ShowText();
                if(story.currentChoices.Count>0){
                    ShowChoices();
                }else{

                }
            }

        }

        //Trigger mostly for debug purposes, remove later
        if(startStoryTrigger){
            StartDialogue(inkJSONAsset,currentKnotName);
            startStoryTrigger=false;
        }
    }

    public void PickChoice(int choiceIndex){
        currentChoiceIndex=choiceIndex;
        choicePicked=true;
    }

    //Show spoken text (not choices) on UI
    void ShowText(){
        textBox.SetActive(true);
        lineTMP.text=displayText;
        canvasParent.SetActive(true);
    }

    //Show choices on UI
    void ShowChoices(){
        for(int i=0;i<Mathf.Min(story.currentChoices.Count,choiceTextBoxes.Length);i++){
            choiceTextBoxes[i].gameObject.SetActive(true);
            choiceTMPs[i].text=story.currentChoices[i].text;
            if(i==currentChoiceIndex){
                Color c=choiceTextBoxes[i].GetComponent<Image>().color;
                c.a=0.8f;
                choiceTextBoxes[i].GetComponent<Image>().color=c;
            }else{
                Color c=choiceTextBoxes[i].GetComponent<Image>().color;
                c.a=0.5f;
                choiceTextBoxes[i].GetComponent<Image>().color=c;
            }
        }
    }

    //Remove all UI elements
    void HideText(){
        interlocutorLineTMP.text="";
        playerLineTMP.text="";
        canvasParent.SetActive(false);
        playerTextBox.SetActive(false);
        interlocutorTextBox.SetActive(false);
    }

    //Remove choice UI elements
    void HideChoices(){
        for(int i=0;i<choiceTextBoxes.Length;i++){
            choiceTextBoxes[i].gameObject.SetActive(false);
        }
    }

    public void StartDialogue(TextAsset textAsset=null,string knotName="",NPCOverworld interlocutor=null){
        if(textAsset==null){
            textAsset=inkJSONAsset;
        }
        if(knotName==""){
            knotName=currentKnotName;
        }
        StartStory();
        if(currentKnotName!=""){
            StartKnot(knotName);
        }
        inDialogue=true;
        Continue();
        playerInput.SwitchMap("UI");
        npcInterlocutor=interlocutor;
    }

    public void EndDialogue(){
        inDialogue=false;
        HideText();
        playerInput.SwitchMap("Player");
        if(npcInterlocutor!=null){
            npcInterlocutor.FinishedDialogue();
        }
    }

    public void StartStory () {
		story = new Story (inkJSONAsset.text);
	}

    void StartKnot(string knotName){     // A knot is a section of a story
        story.ChoosePathString(knotName);
    }

    void Continue(){
        if(story.canContinue){
			displayText="";
            while(displayText=="" && story.canContinue){
                displayText=story.Continue().Trim();
            }
            displayText=FindTalker(displayText);
            GameObject prevTextBox=textBox;
            if(talker=="MC"){
                textBox=playerTextBox;
                lineTMP=playerLineTMP;
            }else{
                textBox=interlocutorTextBox;
                lineTMP=interlocutorLineTMP;
            }
            if(textBox!=prevTextBox){
                HideText();
            }
		}
    }

    //Removes "NPC:" from text and sets talker, returns the line without talker
    string FindTalker(string text){
        int i=text.IndexOf(":");
        if(i>0 && text.Substring(i-1,1)!="\\"){
            talker=text.Substring(0,i).Trim();
            text=text.Substring(i+1,text.Length-(i+1)).Trim();
        }
        return text;
    }

}
