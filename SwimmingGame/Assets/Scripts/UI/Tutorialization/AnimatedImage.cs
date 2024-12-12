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

    void Start()
    {
        image=GetComponent<Image>();
    }

    void Update()
    {
        imageIndex+=imageSpeed*Time.deltaTime;
        imageIndex=imageIndex%sprites.Length;

        image.sprite=sprites[Mathf.FloorToInt(imageIndex)];
    }
}
