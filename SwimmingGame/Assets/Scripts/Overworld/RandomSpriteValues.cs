using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class RandomSpriteValues : MonoBehaviour

{
    public bool randomizeScale=false;
    [Tooltip("If this and randomizeScale is true, reset y so that it looks like it has the same ground.")]
    public bool normalizeY=false;
    public float minScale=0.75f;
    public float maxScale=2f;

    public bool randomizeFlipX=false;
    public bool randomizeFlipY=false;

    public bool randomizeSpriteLibraryAsset=false;
    public SpriteLibraryAsset[] spriteLibraryAssets; 

    private SpriteRenderer spriteRenderer;

    public bool randomizeHue=false;
    public float randomizeHueIntensity=33f/255f;

    void Start()
    {
        spriteRenderer=GetComponentInChildren<SpriteRenderer>();

        if(randomizeScale){
            float s=Random.Range(minScale,maxScale);
            transform.localScale=Vector3.one*s;
            if(normalizeY){
                Vector3 pos=transform.localPosition;
                pos.y+=(s-1f)/2f;
                transform.localPosition=pos;
            }
        }

        if(randomizeFlipX){
            if(Random.Range(0f,1f)<.5f){
                spriteRenderer.flipX=true;
            }
        }

        if(randomizeFlipY){
            if(Random.Range(0f,1f)<.5f){
                spriteRenderer.flipY=true;
            }
        }

        if(randomizeSpriteLibraryAsset){
            SpriteLibrary spriteLibrary=GetComponent<SpriteLibrary>();
            spriteLibrary.spriteLibraryAsset=spriteLibraryAssets[Random.Range(0,spriteLibraryAssets.Length)];
        }

        if(randomizeHue){
            spriteRenderer.material.color=Color.HSVToRGB(Random.Range(0f,1f),randomizeHueIntensity,1f);
        }
    }

}
