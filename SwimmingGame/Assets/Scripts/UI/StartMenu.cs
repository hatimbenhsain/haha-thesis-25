using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class StartMenu : Menu
{    
    private bool startedGame=false;

    public Animator canvasAnimator;

    public override void Initiate()
    {
        base.Initiate();
        InitiateMenu();
        active = true;
    }

    void InitiateMenu()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        if(inSettings) settingMenuUI.SetActive(true);
        else menuUI.SetActive(true);
        UnityEngine.Cursor.visible = true;
        GetButtons();
        myLockState = UnityEngine.Cursor.lockState;
    }

    public override void Update()
    {
        base.Update();

        if (!startedGame)
        {
            if (FindObjectOfType<LevelLoader>().loadingLevel)
            {
                startedGame=true;
                canvasAnimator.enabled=true;
                canvasAnimator.SetTrigger("depart");
                fadingOutCanvas=true;
                Sound.StopInstance("Click Button",true);
                Sound.PlayOneShotVolume("event:/One-Time SFX/TitleScreen_Kick",1f);
                Sound.PlayOneShotVolume("event:/One-Time SFX/TitleScreen_Start",1f);
                Sound.StopInstance("Title Screen Ambiance");
            }
        }
    }

}
