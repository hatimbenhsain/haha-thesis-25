using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public string destinationScene;
    [Header("Load level after countdown")]
    public bool countdown;
    public float countdownTime = 5f;  // Public variable to set the countdown time in seconds
    [Header("Load level after entering trigger volume")]
    public bool trigger;
    [Header("Load level after pressing P")]
    public bool pressP;
    [Header("Transition animation")]
    public float transitionTime;
    public Animator transition;
    private float timer;

    void Start()
    {
        timer = countdownTime;  // Initialize the timer with the countdown time
    }

    void Update()
    {
        // Load level after countdown
        timer -= Time.deltaTime;
        if (timer <= 0f && countdown)
        {
            LoadLevel(); 
        }
        // Load level after pressing P
        if (Input.GetKeyDown(KeyCode.P) && pressP)
        {
            LoadLevel();
        }

    }

    public void LoadLevel(string destination = "")
    {
        if (destination == "")
        {
            destination = destinationScene;
        }
        StartCoroutine(LoadLevelCoroutine(destination));
    }


    // Load level when enter trigger box
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && trigger)
        {
            LoadLevel();
        }
    }

    IEnumerator LoadLevelCoroutine(string levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);

    }
}
