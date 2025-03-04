using System.Collections;
using System.Collections.Generic;
using Obi;
using Unity.VisualScripting;
using UnityEngine;

public class StopPlayerMovement : MonoBehaviour
{
    private Swimmer swimmer;
    private PlayerInput playerInput;
    private bool swimmerInside=false;

    void Start(){
        playerInput=FindObjectOfType<PlayerInput>();
    }

    void Update()
    {
        if(swimmerInside){
            if((Vector3.Angle(Vector3.up,swimmer.transform.forward)<=90f && !playerInput.movingBackward) ||(Vector3.Angle(Vector3.up,swimmer.transform.forward)>=90f && playerInput.movingBackward)){
                swimmer.canMove=false;
            }else{
                swimmer.canMove=true;
            }
        }
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag=="Player"){
            swimmer=other.gameObject.GetComponentInParent<Swimmer>();
            Debug.Log(other.gameObject);
            swimmer.canMove=false;
            swimmerInside=true;
            Debug.Log("stop p movement");
        }
    }

    void OnTriggerExit(Collider other){
        if(other.gameObject.tag=="Player"){
            swimmerInside=false;
            swimmer.canMove=true;
        }
    }
}
