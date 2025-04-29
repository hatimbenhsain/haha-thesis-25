using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MigrationScreenBorder : MonoBehaviour
{
    private Migration migration;

    private Image img;
    private Animator animator;

    private int numberOfBorderTypes=3;
    private float borderChangeTimer;
    public float borderChangeAverageTime=10f;
    public float borderChangeTimeVariance=5f;
    private float borderChangeTime;
    private float originalBorderHue;

    
    void Start()
    {
        migration=FindObjectOfType<Migration>();
        animator=GetComponent<Animator>();

        float h,s,v;
        borderChangeTime=borderChangeAverageTime+Random.Range(-borderChangeTimeVariance,borderChangeTimeVariance);
        img=GetComponent<Image>();
        Color.RGBToHSV(img.color,out h,out s,out v);
        originalBorderHue=h;
    }

    void Update()
    {
        borderChangeTimer+=Time.deltaTime;
        if(borderChangeTimer>=borderChangeTime){
            animator.SetFloat("type",(float)Random.Range(0,numberOfBorderTypes));
            borderChangeTimer=0f;
            borderChangeTime=borderChangeAverageTime+Random.Range(-borderChangeTimeVariance,borderChangeTimeVariance);
        }
        float h,s,v,h2,s2,v2;
        Color c=img.color;
        Color.RGBToHSV(c,out h,out s,out v);
        Color.RGBToHSV(RenderSettings.fogColor,out h2,out s2,out v2);
        c=Color.Lerp(Color.HSVToRGB(originalBorderHue,s,v),Color.HSVToRGB(Mathf.Clamp(h2-0.5f,0f,1f),s,v),.1f);
        c.a=img.color.a;
        img.color=c;
    }
}
