using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    private PlayerInput playerInput;
    private LevelLoader levelLoader;

    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        levelLoader=FindObjectOfType<LevelLoader>();
    }

    void Update()
    {
        if(!levelLoader.fadingOut){
            if(playerInput.entering && !playerInput.prevEntering){
                FindObjectOfType<LevelLoader>().LoadLevel();
            }else if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)){
                FindObjectOfType<LevelLoader>().LoadLevel("Chapter1");
            }else if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)){
                FindObjectOfType<LevelLoader>().LoadLevel("Chapter2");
            }
        }
    }
}
