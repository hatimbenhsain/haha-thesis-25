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
        if(playerInput.entering && !playerInput.prevEntering && !levelLoader.fadingOut){
            FindObjectOfType<LevelLoader>().LoadLevel();
        }
    }
}
