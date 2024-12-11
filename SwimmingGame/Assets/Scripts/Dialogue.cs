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
    [HideInInspector]
    public Story story;

    public string currentKnotName;

    public string displayText; //Current text to display

    public int currentChoiceIndex=0; //Currently selected choice index
    [Tooltip("Name of current person talking")]
    public string talker="";
    private float pauseTimer=0f;
    private bool isAmbient=false;
    private float ambientTimer=0f;

    
    private PlayerInput playerInput;   
    private GameManager gameManager;

    public bool startStoryTrigger;

    public bool inDialogue=false;
    private bool choicePicked=false;

    public NPCOverworld npcInterlocutor;
    private Swimmer swimmer;

    [Tooltip("Can the player control choice with buttons?")]
    public bool controlChoice=true;


    [Header("Canvas objects")]
    [Tooltip("Group to completely show or hide when start/stop dialogue.")]
    public GameObject canvasParent;
    [Tooltip("Dialogue views with specific textbox arrangement.")]
    public GameObject[] views;
    public int currentViewIndex=0;
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
    [HideInInspector]
    public TMP_Text[] choiceTMPs;


    [Header("Misc.")]
    public float dialogueSpeedSlow=10f;
    public float dialogueSpeedNormal=30f;
    public float dialogueSpeedFast=80f;

    //Index of progress in showing the current line
    private float currentCharacterIndex=0f;
    private float currentTextSpeed=1f;

    //Inline pause: pausing in the middle of a line when writing \pause
    public float inlinePauseLength=1f; //in seconds
    private float inlinePauseTimer=0f;

    [Header("Additional Text Boxes")]
    public GameObject standardTextBox;
    public GameObject floralTextBox;
    public GameObject boneTextBox;

    private Color defaultTextBoxColor;
    private bool changedColor=false;
    [HideInInspector]
    public float[] defaultChoiceBoxOpacites;

    void Awake()
    {
        playerInput=FindObjectOfType<PlayerInput>();
        gameManager=FindObjectOfType<GameManager>();

        SetUpView();

        Image[] images=choiceTextBoxes[0].GetComponentsInChildren<Image>();
        defaultChoiceBoxOpacites=new float[images.Length];
        for(var i=0;i<defaultChoiceBoxOpacites.Length;i++){
            Color c=images[i].color;
            defaultChoiceBoxOpacites[i]=c.a;
        }

        DialogueAwake();

        HideText();
        HideChoices();

        swimmer=FindObjectOfType<Swimmer>();

        if(inkJSONAsset!=null){
            StartStory();
        }

    }

    public void SetUpView(){
        choiceTMPs=new TMP_Text[choiceTextBoxes.Length];
        for(int i=0;i<choiceTextBoxes.Length;i++){
            choiceTMPs[i]=choiceTextBoxes[i].GetComponentInChildren<TMP_Text>();
        }
        interlocutorLineTMP=interlocutorTextBox.GetComponentInChildren<TMP_Text>();
        playerLineTMP=playerTextBox.GetComponentInChildren<TMP_Text>();

        lineTMP=interlocutorLineTMP;
        textBox=interlocutorTextBox;

        standardTextBox.SetActive(false);
        floralTextBox.SetActive(false);
        boneTextBox.SetActive(false);

    }

    virtual public void DialogueAwake(){

    }

    void Update(){
        DialogueUpdate();

        if(inDialogue){
            if(!isAmbient && pauseTimer<=0f){
                TypeWriter();

                //Handling player input: Continuing/Picking
                if(playerInput.interacting && !playerInput.prevInteracting){
                    if(currentCharacterIndex<displayText.Length){
                        currentCharacterIndex=displayText.Length;
                    }else{
                        if(story.canContinue){
                            Continue();
                            if(story.currentChoices.Count==0){
                                HideChoices();
                            }
                        }else if(story.currentChoices.Count>0 && currentChoiceIndex<=story.currentChoices.Count 
                            && currentChoiceIndex>=0 && controlChoice){
                            PickChoice(currentChoiceIndex);
                        }else if(story.currentChoices.Count>0 && currentChoiceIndex<=story.currentChoices.Count){
                            currentChoiceIndex=0;
                        }else{
                            EndDialogue();
                        }
                        if(displayText=="" && !story.canContinue){
                            EndDialogue();
                        }
                    }
                }

                //Handling player input: picking choice
                if(story.currentChoices.Count>0 && currentCharacterIndex>=displayText.Length && controlChoice){
                    if((playerInput.navigateDown && !playerInput.prevNavigateDown) || 
                        (playerInput.navigateRight && !playerInput.prevNavigateRight)){
                        currentChoiceIndex+=1;
                        currentChoiceIndex=(currentChoiceIndex+story.currentChoices.Count)%story.currentChoices.Count;
                    }
                    if((playerInput.navigateUp && !playerInput.prevNavigateUp) ||
                        (playerInput.navigateLeft && !playerInput.prevNavigateLeft)){
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
                if(inDialogue && pauseTimer<=0f){
                    ShowText();
                    if(story.currentChoices.Count>0 && currentCharacterIndex>=displayText.Length){
                        ShowChoices();
                    }else{

                    }
                }
            }else if(pauseTimer>=0f){
                pauseTimer-=Time.deltaTime;
                HideText();
                if(pauseTimer<=0f){
                    ShowText();
                }
            }else if(isAmbient){ //Dialogue is ambient if it just appears without player control
                TypeWriter();
                
                ambientTimer-=Time.deltaTime;
                if(ambientTimer<=0f){
                    if(story.canContinue){
                        Continue();
                    }else{
                        EndDialogue();
                    }
                }else{  
                    if(inDialogue){
                        ShowText();
                    }
                }
            }

        }

        //Trigger mostly for debug purposes, remove later
        if(startStoryTrigger){
            StartDialogue(inkJSONAsset,currentKnotName);
            startStoryTrigger=false;
        }
    }

    virtual public void DialogueUpdate(){

    }

    public void PickChoice(int choiceIndex){
        currentChoiceIndex=choiceIndex;
        choicePicked=true;
    }

    //Show spoken text (not choices) on UI
    void ShowText(){
        textBox.SetActive(true);
        lineTMP.text=displayText.Substring(0,Mathf.FloorToInt(currentCharacterIndex));
        canvasParent.SetActive(true);
        if(currentCharacterIndex<displayText.Length && inlinePauseTimer<=0f){
            textBox.GetComponentInChildren<Animator>().speed=currentTextSpeed/dialogueSpeedNormal;
        }else{
            textBox.GetComponentInChildren<Animator>().speed=0.5f;
        }
    }

    //Show choices on UI
    public virtual void ShowChoices(){
        for(int i=0;i<Mathf.Min(story.currentChoices.Count,choiceTextBoxes.Length);i++){
            if(!choiceTextBoxes[i].gameObject.activeInHierarchy){
                choiceTextBoxes[i].gameObject.SetActive(true);
                choiceTextBoxes[i].GetComponentInChildren<Animator>().StartPlayback();
                choiceTextBoxes[i].GetComponentInChildren<Animator>().Play("Base Layer.smallDialogueBubbleIdle",-1,Random.Range(0f,1f));
                choiceTextBoxes[i].GetComponentInChildren<Animator>().speed=1f;
            }
            
            choiceTMPs[i].text=story.currentChoices[i].text;
            if(i==currentChoiceIndex){
                Image[] images=choiceTextBoxes[i].GetComponentsInChildren<Image>();
                for(var k=0;k<images.Length;k++){
                    Color c=images[k].color;
                    c.a=1*defaultChoiceBoxOpacites[k];
                    images[k].color=c;
                }
                choiceTextBoxes[i].GetComponentInChildren<Animator>().speed=1.25f;
            }else{
                Image[] images=choiceTextBoxes[i].GetComponentsInChildren<Image>();
                for(var k=0;k<images.Length;k++){
                    Color c=images[k].color;
                    c.a=0.625f*defaultChoiceBoxOpacites[k];
                    images[k].color=c;
                }
                choiceTextBoxes[i].GetComponentInChildren<Animator>().speed=0.5f;
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
    public virtual void HideChoices(){

        for(int i=0;i<choiceTextBoxes.Length;i++){
            choiceTextBoxes[i].gameObject.SetActive(false);
        }
    }
    
    //Progress text writing and watch for pauses
    void TypeWriter(){
        if(inlinePauseTimer<=0f){
            currentCharacterIndex+=currentTextSpeed*Time.deltaTime;
            currentCharacterIndex=Mathf.Clamp(currentCharacterIndex,0f,displayText.Length);
        }else{
            inlinePauseTimer-=Time.deltaTime;
        }

        //Looking for inline pause
        if(displayText.Substring(0,
        Mathf.Min(Mathf.FloorToInt(currentCharacterIndex)+5,displayText.Length)).Contains("\\pause")){
            currentCharacterIndex=displayText.IndexOf("\\pause");
            inlinePauseTimer=inlinePauseLength;
            displayText=displayText.Replace("\\pause","");
        }

        if(displayText.Length-currentCharacterIndex>="\\pause".Length &&
        displayText.Substring(Mathf.FloorToInt(currentCharacterIndex+1),"\\pause".Length).ToLower()==
        "\\pause"){
            inlinePauseTimer=inlinePauseLength;
            displayText=displayText.Replace("\\pause","");
        }
    }

    //Only happens if not currently in dialogue
    public void TryStartDialogue(TextAsset textAsset=null,string knotName="",NPCOverworld interlocutor=null){
        if(!inDialogue){
            StartDialogue(textAsset,knotName,interlocutor);
        }
    }

    public void StartDialogue(TextAsset textAsset=null,string knotName="",NPCOverworld interlocutor=null){
        SetDialogueBubble("standard");
        npcInterlocutor=interlocutor;

        TextAsset prevTextAsset=inkJSONAsset;
        if(textAsset==null){
            textAsset=inkJSONAsset;
        }
        if(knotName==""){
            knotName=currentKnotName;
        }
        if(textAsset!=prevTextAsset) StartStory(textAsset);
        if(currentKnotName!=""){
            StartKnot(knotName);
        }
        inDialogue=true;
        if(displayText==""){
            Continue();
        }
        if(ContainsTag(story.TagsForContentAtPath(currentKnotName),"ambient")){
            isAmbient=true;
        }else{
            isAmbient=false;
        }

        if(ContainsTag(story.TagsForContentAtPath(currentKnotName),"color")){
            Image[] images=textBox.GetComponentsInChildren<Image>();
            defaultTextBoxColor=images[0].color;
            changedColor=true;
            string tag=GetTag(story.TagsForContentAtPath(currentKnotName),"color");
            Color newColor;
            if(ColorUtility.TryParseHtmlString("#"+tag.Replace("color:","").Trim(),out newColor)){
                foreach(Image image in images){
                    newColor.a=image.color.a;
                    image.color=newColor;
                }
            }
        }
        //if(!isAmbient) playerInput.SwitchMap("UI");
        
        if(swimmer!=null) swimmer.StartedDialogue(isAmbient);

        if(displayText=="" && !story.canContinue && story.currentChoices.Count<=0){
            EndDialogue();
        }
    }

    public void EndDialogue(){
        inDialogue=false;
        HideText();
        playerInput.RestoreDefaultMap();
        if(npcInterlocutor!=null){
            npcInterlocutor.FinishedDialogue(isAmbient);
            npcInterlocutor=null;
        }
        if(swimmer!=null) swimmer.FinishedDialogue(isAmbient);
        displayText="";

        if(changedColor){
        Image[] images=textBox.GetComponentsInChildren<Image>();
        foreach(Image image in images){
            defaultTextBoxColor.a=image.color.a;
            image.color=defaultTextBoxColor;
        }
        }
        changedColor=false;
    }

    public void StartStory (TextAsset textAsset=null) {
        if(textAsset==null){
            textAsset=inkJSONAsset;
        }
		story = new Story (textAsset.text);
        BindFunctions(story);
	}

    void StartKnot(string knotName){     // A knot is a section of a story
        currentKnotName=knotName;
        story.ChoosePathString(knotName);
    }

    void Continue(){
        if(story.canContinue){
			displayText="";
            while(displayText=="" && story.canContinue){
                displayText=story.Continue().Trim();
            }
            displayText=FindTalker(displayText); //Remove "NPC: " from display text
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

            // PARSING INLINE TAGS:

            //Get text display time length
            if(ContainsTag(story.currentTags,"time")){
                string tag=GetTag(story.currentTags,"time");
                ambientTimer=float.Parse(tag.Replace("time:","").Trim());
            }

            //Get text speed
            currentTextSpeed=GetTextSpeed(story.currentTags);

            currentCharacterIndex=0;
		}
    }

    //Get speed that we are displaying text (typerwriter effect)
    float GetTextSpeed(List<string> tags){
        float speed=dialogueSpeedNormal;
        if(ContainsTag(story.currentTags,"speed")){
            string tag=GetTag(story.currentTags,"speed");
            tag=tag.Replace("speed:","").Trim().ToLower();
            if(tag=="slow"){
                speed=dialogueSpeedSlow;
            }else if(tag=="fast"){
                speed=dialogueSpeedFast;
            }else if(tag=="normal"){
                speed=dialogueSpeedNormal;
            }else{
                speed=float.Parse(tag);
            }
        }
        return speed;
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

    //Does the story or current line contain this tag?
    bool ContainsTag(List<string> list,string tag){
        if(list!=null && list.Count>0){
            foreach(string s in list){
                if(s.ToLower().Contains(tag.ToLower())){
                    return true;
                }
            }
        }
        return false;
    }

    //Get tag from story by name
    string GetTag(List<string> list,string tag){
        if(list!=null && list.Count>0){
            foreach(string s in list){
                if(s.ToLower().Contains(tag.ToLower())){
                    return s;
                }
            }
        }
        return "";
    }

    //Bind external functions that may be used by dialogue
    void BindFunctions(Story story){
        story.BindExternalFunction("pause",(float time)=>{
            Pause(time);
        });
        story.BindExternalFunction("stopSinging",()=>{
            StopSinging();
        });
        story.BindExternalFunction("continueSinging",()=>{
            ContinueSinging();
        });
        story.BindExternalFunction("restartSinging",()=>{
            RestartSinging();
        });
        story.BindExternalFunction("toggleSingingMode",()=>{
            ToggleSingingMode();
        });
        story.BindExternalFunction("loadLevel",(string destinationScene)=>{
            LoadLevel(destinationScene);
        });
        story.BindExternalFunction("goToNextLevel",()=>{
            GoToNextLevel();
        });
        story.BindExternalFunction("nextBrain",()=>{
            NextBrain();
        });
        story.BindExternalFunction("setDialogueBubble",(string bubble)=>{
            SetDialogueBubble(bubble);
        });
        story.BindExternalFunction("pauseTutorial",(bool b)=>{
            PauseTutorial(b);
        });
        story.BindExternalFunction("finishTutorialPart",(int i)=>{
            FinishTutorialPart(i);
        });
        story.BindExternalFunction("switchObject",(string name, bool b)=>{
            SwitchObject(name,b);
        });
        story.BindExternalFunction("switchInterlocutor",(string name)=>{
            SwitchInterlocutor(name);
        });
        story.BindExternalFunction("overrideRotation",(string targetName)=>{
            OverrideRotation(targetName);
        });
        story.BindExternalFunction("changeDialogueView",(int index)=>{
            ChangeView(index);
        });
    }

    // EXTERNAL FUNCTIONS

    void Pause(float time){
        pauseTimer=time;
        HideText();
    }

    void StopSinging(){
        if(npcInterlocutor!=null){
            npcInterlocutor.GetComponent<NPCSinging>().StopSinging();
        }
    }

    void ContinueSinging(){
        if(npcInterlocutor!=null){
            npcInterlocutor.GetComponent<NPCSinging>().ContinueSinging();
        }
    }

    void RestartSinging(){
        if(npcInterlocutor!=null){
            npcInterlocutor.GetComponent<NPCSinging>().RestartSinging();
        }
    }

    void ToggleSingingMode(){
        if(npcInterlocutor!=null){
            npcInterlocutor.GetComponent<NPCSinging>().ToggleSingingMode();
        }
    }

    void LoadLevel(string destinationScene){
        FindObjectOfType<LevelLoader>().LoadLevel(destinationScene);
    }

    void GoToNextLevel(){
        if(npcInterlocutor!=null){
            FindObjectOfType<LevelLoader>().LoadLevel();
        }
    }

    void NextBrain(){
        NPCSequencer npcSequencer;
        if(npcInterlocutor!=null && npcInterlocutor.transform.parent.TryGetComponent<NPCSequencer>(out npcSequencer)){
            npcSequencer.NextBrain();
        }
    }

    void SetDialogueBubble(string bubble){
        bool isActive=interlocutorTextBox.activeInHierarchy;
        interlocutorTextBox.SetActive(false);
        switch(bubble.ToLower()){
            case "standard":
                interlocutorTextBox=standardTextBox;
                break;
            case "floral":
                interlocutorTextBox=floralTextBox;
                break;
            case "bone":
                interlocutorTextBox=boneTextBox;
                break;
        }
        interlocutorLineTMP=interlocutorTextBox.GetComponentInChildren<TMP_Text>();
        interlocutorTextBox.SetActive(isActive);
    }

    void PauseTutorial(bool b){
        FindObjectOfType<Tutorial>().PauseTutorial(b);
    }

    void FinishTutorialPart(int i){
        FindObjectOfType<Tutorial>().FinishTutorialPart(i);
    }

    void SwitchObject(string name, bool b){
        gameManager.SwitchObject(name,b);
    }

    void SwitchInterlocutor(string name){
        GameObject g=gameManager.FindObject(name);
        NPCOverworld newInterlocutor;
        if(g!=null && g.TryGetComponent<NPCOverworld>(out newInterlocutor)){
            if(npcInterlocutor!=null){
                npcInterlocutor.FinishedDialogue(isAmbient);
            }
        }else if(g==null){
            Debug.Log("Tried switching interlocutor & couldn't find gameobject");
        }else{
            Debug.Log("Tried switching interlocutor & couldn't find NPCOverworld");
        }
    }

    void OverrideRotation(string targetName){
        GameObject g=gameManager.FindObject(targetName);
        if(g!=null){
            swimmer.OverrideRotation(g.transform);
        }else{
            Debug.Log("Tried overriding rotation & couldn't find gameobject");
        }
    }

    virtual public void ChangeView(int i){
        Debug.Log("Change view dialogue");
        views[currentViewIndex].SetActive(false);
        currentViewIndex=Mathf.Clamp(i-1,0,views.Length);
        views[currentViewIndex].SetActive(true);

        DialogueView dialogueView=views[currentViewIndex].GetComponent<DialogueView>();
        interlocutorTextBox=dialogueView.interlocutorTextBox;
        playerTextBox=dialogueView.playerTextBox;
        standardTextBox=dialogueView.standardTextBox;
        boneTextBox=dialogueView.boneTextBox;
        floralTextBox=dialogueView.floralTextBox;

        choiceTextBoxes=dialogueView.choiceTextBoxes;

        SetUpView();

        HideText();
    }

}
