using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECS_CameraRotator : MonoBehaviour
{
    static ECS_CameraRotator cameraRotator;
    static List<ECS_FaceCamera> objectsInPlay;

    void OnEnable(){
        ECS_CullSphere.OnAddObject+=AddedObject;
        ECS_CullSphere.OnRemoveObject+=RemovedObject;
    }

    void OnDisable(){
        ECS_CullSphere.OnAddObject-=AddedObject;
        ECS_CullSphere.OnRemoveObject-=RemovedObject;
    }

    void Start()
    {
        cameraRotator=this;
        objectsInPlay=new List<ECS_FaceCamera>();
    }

    void Update()
    {
        foreach(ECS_FaceCamera fc in objectsInPlay){
            RotateToFaceCamera(fc);
        }
    }

    public static void RotateToFaceCamera(ECS_FaceCamera fc){
        fc.transform.rotation=Camera.main.gameObject.transform.rotation;
        if(fc.lockX || fc.lockY || fc.lockZ){
            Vector3 rot=fc.transform.localRotation.eulerAngles;
            if(fc.lockX) rot.x=fc.originalRotation.x;
            if(fc.lockY) rot.y=fc.originalRotation.y;
            if(fc.lockZ) rot.z=fc.originalRotation.z;
            fc.transform.localRotation=Quaternion.Euler(rot);
        }
    }

    public static void AddedObject(GameObject o){
        ECS_FaceCamera fc=o.GetComponentInChildren<ECS_FaceCamera>();
        if(fc!=null){
            objectsInPlay.Add(fc);
        }
    }

    public static void RemovedObject(GameObject o){
        ECS_FaceCamera fc=o.GetComponentInChildren<ECS_FaceCamera>();
        if(fc!=null){
            objectsInPlay.Remove(fc);
        }
    }
}
