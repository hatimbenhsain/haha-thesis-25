using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CuddleDialogue : Dialogue
{
    [Header("Cuddle Specific")]
    public bool caressing;
    public float caressRequiredLength=1f;
    public float caressTimer=0f;
    public float caressCancelSpeed=.5f;

    public float scaleSpeed=1f;
    public float opacitySpeed=1f;
    public float notSelectedOpacity=0.625f;
    public float hoveredOpacity=0.9f;
    public float chosenOpacity=1f;
    public float hoveredScale=1.1f;
    public float chosenScale=2f;

    private RectTransform[] choiceRects;

    private int prevChoiceIndex=-1;

    private bool justHovered = false;
    private bool justCaressed = false;

    public override void DialogueAwake()
    {
        base.DialogueAwake();

        ChangeView(currentViewIndex);
    }

    public override void DialogueUpdate()
    {
        base.DialogueUpdate();

    }

    void LateUpdate(){
        if (caressing)
        {
            caressTimer += Time.deltaTime;
        }

        if (caressTimer >= caressRequiredLength && story.currentChoices.Count > 0 &&
            currentChoiceIndex <= story.currentChoices.Count && currentChoiceIndex >= 0)
        {
            PickChoice(currentChoiceIndex);
        }
        else if (story.currentChoices.Count == 0)
        {
            caressTimer = 0f;
            currentChoiceIndex = -1;
        }

        caressTimer -= Time.deltaTime * caressCancelSpeed / caressRequiredLength;
        caressTimer = Mathf.Clamp(caressTimer, 0f, caressRequiredLength);

        if (justHovered)
        {
            justHovered = false;
        }
        else {
            currentChoiceIndex = -1;
        }
        if (justCaressed)
        {
            justCaressed = false;
        }
        else caressing = false;
    }

    public void HoveringChoice(int choiceIndex){
        currentChoiceIndex=choiceIndex;
        if(currentChoiceIndex!=prevChoiceIndex){
            caressTimer=0f;
        }
        prevChoiceIndex=currentChoiceIndex;
        justHovered = true;
    }

    public void CaressingChoice(int choiceIndex){
        caressing=true;
        justCaressed = true;
    }

    public override void ShowChoices()
    {
        for(int i=0;i<Mathf.Min(story.currentChoices.Count,choiceTextBoxes.Length);i++){
            if(!choiceTextBoxes[i].gameObject.activeInHierarchy){
                choiceTextBoxes[i].gameObject.SetActive(true);
                choiceTextBoxes[i].GetComponentInChildren<Animator>().StartPlayback();
                choiceTextBoxes[i].GetComponentInChildren<Animator>().Play("Base Layer.smallDialogueBubbleIdle",-1,Random.Range(0f,1f));
                choiceTextBoxes[i].GetComponentInChildren<Animator>().speed=1f;
            }
            
            choiceTMPs[i].text=story.currentChoices[i].text;
            float targetS=1f;
            float targetA=1f;
            if(i==currentChoiceIndex){
                targetA=hoveredOpacity;
                targetS+=hoveredScale-1f;
                if(caressing){
                    float k=EaseOutSine(caressTimer/caressRequiredLength);
                    targetS=targetS+(chosenScale-hoveredScale)*k;
                    targetA+=k*(chosenOpacity-hoveredOpacity);
                }
                choiceTextBoxes[i].GetComponentInChildren<Animator>().speed=1.25f;
            }else{
                targetA=notSelectedOpacity;
                choiceTextBoxes[i].GetComponentInChildren<Animator>().speed=0.5f;
            }

            Image[] images=choiceTextBoxes[i].GetComponentsInChildren<Image>();
            for(var k=0;k<images.Length;k++){
                Color c=images[k].color;
                c.a=Mathf.Lerp(c.a/defaultChoiceBoxOpacites[k],targetA,opacitySpeed*Time.deltaTime)
                    *defaultChoiceBoxOpacites[k];
                images[k].color=c;
            }

            Vector3 localScale=choiceRects[i].localScale;
            float s=Mathf.Lerp(localScale.x,targetS,scaleSpeed*Time.deltaTime);
            localScale=new Vector3(s,s,s);
            choiceRects[i].localScale=localScale;

        }
    }

    public override void HideChoices()
    {
        base.HideChoices();

        for(int i=0;i<choiceTextBoxes.Length;i++){
            choiceRects[i].localScale=Vector3.one;
            Image[] images=choiceTextBoxes[i].GetComponentsInChildren<Image>();
            for(var k=0;k<images.Length;k++){
                Color c=images[k].color;
                c.a=notSelectedOpacity*defaultChoiceBoxOpacites[k];
                images[k].color=c;
            }
        }
    }

    public override void ChangeView(int i){
        base.ChangeView(i);

        Debug.Log("Change view cuddle");

        choiceRects=new RectTransform[choiceTextBoxes.Length];
        for(var k=0;k<choiceTextBoxes.Length;k++){
            choiceRects[k]=choiceTextBoxes[k].GetComponent<RectTransform>();
        }

        FindObjectOfType<CuddleCameraManager>().shotIndex=currentViewIndex;

        SetUpView();
    }

    float EaseOutSine(float x){
        return Mathf.Sin((x*Mathf.PI)/2f);
    }

}
