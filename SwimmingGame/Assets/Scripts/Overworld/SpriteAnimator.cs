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

    public bool matchMusic=false;

    void Start()
    {
        spriteRenderer=GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float period=1f;
        if(matchMusic){
            period=60f/(MusicBeat.GetBPM()*imageSpeed);
        }
        imageIndex+=imageSpeed*Time.unscaledDeltaTime/period;
        imageIndex=imageIndex%sprites.Length;

        spriteRenderer.sprite=sprites[Mathf.FloorToInt(imageIndex)];
    }
}
