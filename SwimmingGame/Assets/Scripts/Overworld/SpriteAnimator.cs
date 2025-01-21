using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimator : MonoBehaviour
{
    private float imageIndex=0f;
    public float imageSpeed=4f;
    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer=GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        imageIndex+=imageSpeed*Time.unscaledDeltaTime;
        imageIndex=imageIndex%sprites.Length;

        spriteRenderer.sprite=sprites[Mathf.FloorToInt(imageIndex)];
    }
}
