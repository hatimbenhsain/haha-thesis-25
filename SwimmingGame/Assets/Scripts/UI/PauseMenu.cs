
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public string mainMenuScene;
    public GameObject[] controls;
    [Tooltip("Overworld 0, mainact 1, climax 2, cuddle 3")]
    public int controlsIndex;
    private CinemachineVirtualCamera playerCamera;
    private Transform playerCameraRoot; // The target position and rotation for the player
    private CursorLockMode myLockState;
    private PlayerInput playerInput;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        GameIsPaused = false;
        playerCamera = GameObject.Find("PlayerFollowCamera")?.GetComponent<CinemachineVirtualCamera>();
        playerCameraRoot = GameObject.Find("PlayerCameraRoot")?.GetComponent<Transform>();
        controls[controlsIndex].gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            myLockState = UnityEngine.Cursor.lockState;
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void Resume()
    {
        // set timescale back to 1, unlock camera
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = myLockState;


    }


    void Pause()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        // set timescale to 0, lock camera
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        UnityEngine.Cursor.visible = true;

    }
}
