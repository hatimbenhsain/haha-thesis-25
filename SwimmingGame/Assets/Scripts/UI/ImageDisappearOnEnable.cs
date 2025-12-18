using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageDisappearOnEnable : ImageAppearOnEnable
{
    private Image image;
    void Start()
    {
        image=GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Color c=image.color;
        c.a=Mathf.Lerp(c.a,0f,lerpSpeed*Time.deltaTime);
        image.color=c;
        if (c.a <= 0.01f)
        {
            //Destroy(gameObject);
        }
    }
}
