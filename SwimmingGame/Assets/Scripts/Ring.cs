using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class Ring : MonoBehaviour
{
    public float targetOpacity=0.2f;
    public float minIntensity=0f;
    public float maxIntensity=1f;

    private float currentIntensity=0;

    public float opacityChangeSpeed=0.5f;

    private Renderer renderer;
    private Color color;
    // Start is called before the first frame update
    void Start()
    {
        renderer=GetComponent<Renderer>();
        color=renderer.material.GetColor("_EmissionColor");
        currentIntensity=minIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        currentIntensity=Mathf.Lerp(currentIntensity,targetOpacity,opacityChangeSpeed*Time.deltaTime);
        renderer.material.SetColor("_EmissionColor",color*currentIntensity);
    }

    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag=="Player"){
            targetOpacity=maxIntensity;
        }
    }

    void OnTriggerExit(Collider other){
        if(other.gameObject.tag=="Player"){
            targetOpacity=minIntensity;
        }
    }

    
}
