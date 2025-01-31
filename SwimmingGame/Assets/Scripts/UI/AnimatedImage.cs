using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedImage : MonoBehaviour
{
    private float imageIndex=0f;
    public float imageSpeed=4f;
    public Sprite[] sprites;
    private Image image;
    private RectTransform rect;

    public float pulsePeriod=0f;
    private float pulseTimer=0f;
    public float pulseRotationIntensity=0f;
    private Vector3 initialRotation;
    

    void Start()
    {
        image=GetComponent<Image>();
        rect=GetComponent<RectTransform>();
        if(pulseRotationIntensity!=0f){
            initialRotation=rect.localRotation.eulerAngles;
        }
        pulseTimer=Random.Range(0f,pulsePeriod);
    }

    void Update()
    {
        imageIndex+=imageSpeed*Time.unscaledDeltaTime;
        imageIndex=imageIndex%sprites.Length;

        image.sprite=sprites[Mathf.FloorToInt(imageIndex)];

        pulseTimer+=Time.deltaTime;

        if(pulseRotationIntensity!=0f){
            Vector3 rot=initialRotation;
            rot.z=rot.z+Mathf.Sin(pulseTimer*Mathf.PI*2f/pulsePeriod)*pulseRotationIntensity;
            rect.localRotation=Quaternion.Euler(rot);
        }
    }
}
