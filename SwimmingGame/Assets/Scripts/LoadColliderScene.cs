using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadColliderScene : MonoBehaviour
{
    public string sceneToLoad;
    // Start is called before the first frame update
    void Awake(){
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
