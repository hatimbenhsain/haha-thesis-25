using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class StartMenu : Menu
{    
    private bool startedGame=false;

    private float timer=0f;
    

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
            timer+=Time.unscaledDeltaTime;
            Rumble.AddRumble("Start Menu",(Mathf.Sin(timer*Mathf.PI/2f)+1)*.7f/2f+.3f);
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
