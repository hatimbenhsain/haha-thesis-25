using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadColliderScene : MonoBehaviour
{
    public string sceneToLoad;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadColliderCoroutine());
    }
    private IEnumerator LoadColliderCoroutine()
    {
        yield return new WaitForSeconds(0.2f); //wait for a bit to start
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        // Wait for the scene to finish loading
        operation.completed += (_) =>
        {


            Scene newScene = SceneManager.GetSceneByName(sceneToLoad);
            if (newScene.IsValid())
            {
                SceneManager.SetActiveScene(newScene); // Set the new scene as active
                Debug.Log($"Scene '{sceneToLoad}' is now active.");
            }
            else
            {
                Debug.LogError($"Failed to set active scene: {sceneToLoad} is not valid.");
            }
        };
    } 
}
