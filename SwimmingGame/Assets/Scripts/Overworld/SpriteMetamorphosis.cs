using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class SpriteMetamorphosis : Metamorphosis
{
    [Tooltip("If 1 or more, overrides sprites & product. If more than 1, picks a random one.")]

    public MetamorphosisData[] metamorphosisData;
    [Tooltip("Animation sprites")]
    public Sprite[] sprites;
    [Tooltip("GameObject to instantiate at the end of the metamorphosis")]
    public GameObject product;

    public bool metamorphosisTrigger;

    [Tooltip("Duration of each frame in seconds")]
    public float frameDuration = 0.2f;

    private int frameNumber = 0;
    public SpriteRenderer spriteRenderer;


    public float delayTime = 0f;
    public bool randomizeDelay = false;

    [Tooltip("Ref of sound to play at the start of the metamorphosis (3D)")]
    public string soundToPlay;

    void Start()
    {
        if (metamorphosisData.Length > 0)
        {
            int i = Random.Range(0, metamorphosisData.Length);
            sprites = metamorphosisData[i].sprites;
            product = metamorphosisData[i].product;
        }

        if (randomizeDelay)
        {
            delayTime = Random.Range(0f, delayTime);
        }
    }

    void Update()
    {
        if (metamorphosisTrigger)
        {
            TriggerMetamorphosis();
            metamorphosisTrigger = false;
        }
    }

    public override void TriggerMetamorphosis()
    {
        StartCoroutine(MetamorphosisCoroutine());
    }

    IEnumerator MetamorphosisCoroutine()
    {
        yield return new WaitForSeconds(delayTime);
        if(soundToPlay!="") Sound.Play3DOneShotVolume(soundToPlay, 1f, transform);
        while (frameNumber < sprites.Length)
        {
            spriteRenderer.sprite = sprites[frameNumber];
            yield return new WaitForSeconds(frameDuration);
            frameNumber++;
        }
        GameObject g = Instantiate(product, transform.position, transform.rotation, transform.parent);
        g.transform.localScale = transform.localScale;
        Destroy(gameObject);
    }


}

