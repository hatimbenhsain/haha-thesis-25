using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageAppearOnEnable : MonoBehaviour
{
    public float lerpSpeed;
    private Image image;
    private bool justEnabled=true;

    private ImageDisappearOnEnable clone;

    private float currentAlpha;
    void Start()
    {
        image=GetComponent<Image>();
    }

    void Update()
    {
        Color c=image.color;
        if (justEnabled)
        {
            GameObject g=Instantiate(gameObject,transform.parent);
            g.transform.parent=image.canvas.transform;
            Destroy(g.GetComponent<ImageAppearOnEnable>());
            clone=g.AddComponent<ImageDisappearOnEnable>();
            g.GetComponent<Image>().enabled=true;
            Color c2=c;
            c2.a=0f;
            clone.lerpSpeed=lerpSpeed;
            clone.GetComponent<Image>().color=c2;
            justEnabled=false;
        }

        if(c.a<1f)
        {
            c.a=Mathf.Lerp(c.a,1f,lerpSpeed*Time.deltaTime);
        }
        c.a=Mathf.Round(c.a*100f)/100f;
        currentAlpha=c.a;
        image.color=c;

        justEnabled=false;
    }

    void OnEnable()
    {
        if(image==null) image=GetComponent<Image>();
        Color c=image.color;
        c.a=0f;

        if (clone != null)
        {
            clone.GetComponent<Image>().color=c;
        }
    }

    void OnDisable()
    {
        Color c=image.color;
        c.a=currentAlpha;
        clone.GetComponent<Image>().color=c;
        Rect rect=clone.GetComponent<RectTransform>().rect;
        rect.position=GetComponent<RectTransform>().rect.position;
        clone.GetComponent<RectTransform>().localScale=GetComponent<RectTransform>().lossyScale;
        clone.GetComponent<ImageDisappearOnEnable>().lerpSpeed=lerpSpeed;

        // if(!justEnabled){
        //     GameObject g=Instantiate(gameObject,image.canvas.transform);
        //     Destroy(g.GetComponent<ImageAppearOnEnable>());
        //     g.AddComponent<ImageDisappearOnEnable>();
        //     g.GetComponent<Image>().enabled=true;
        // }
        // g.GetComponent<ImageAppearOnEnable>().enabled=true;
        // g.GetComponent<Image>().enabled=true;
    }

}
