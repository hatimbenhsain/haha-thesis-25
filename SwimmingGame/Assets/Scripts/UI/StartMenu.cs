using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class StartMenu : Menu
{    
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

}
