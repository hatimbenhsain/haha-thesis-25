using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SexCameraManager : MonoBehaviour
{
    public CameraSet[] cameraGroups; // array of sets of cameras
    public int currentGroupIndex = 0; // index to control which group is active

    private int spacePressCount = 0; // count of space presses

    void Start()
    {
        // initialize cameras for the current group
        InitializeCameras();
    }

    void Update()
    {
        // switch cameras based on space presses
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spacePressCount++;
            SwitchCameras();
        }
    }

    // initializes cameras for the current group
    void InitializeCameras()
    {
        if (currentGroupIndex < cameraGroups.Length)
        {
            CameraSet currentSet = cameraGroups[currentGroupIndex];

            // activate main camera, deactivate others
            currentSet.mainCamera.gameObject.SetActive(true);
            currentSet.fixedPOV.gameObject.SetActive(false);
            currentSet.followCamera.gameObject.SetActive(false);
        }
    }

    // switch cameras based on the count of space presses
    void SwitchCameras()
    {
        if (currentGroupIndex < cameraGroups.Length)
        {
            CameraSet currentSet = cameraGroups[currentGroupIndex];

            if (spacePressCount == 1)
            {
                currentSet.fixedPOV.gameObject.SetActive(true);
                currentSet.followCamera.gameObject.SetActive(false);
            }
            else if (spacePressCount == 2)
            {
                currentSet.fixedPOV.gameObject.SetActive(false);
                currentSet.followCamera.gameObject.SetActive(true);
            }
            else if (spacePressCount >= 3)
            {
                currentSet.followCamera.gameObject.SetActive(false);
                spacePressCount = 0; // reset the counter
            }
        }
    }

    // optional: method to change the current camera group
    public void SetCameraGroup(int index)
    {
        if (index >= 0 && index < cameraGroups.Length)
        {
            currentGroupIndex = index;
            spacePressCount = 0; // reset the space press count
            InitializeCameras(); // initialize cameras for the new group
        }
    }
}