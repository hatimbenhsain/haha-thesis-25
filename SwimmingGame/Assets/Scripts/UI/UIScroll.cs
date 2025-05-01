using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScroll : MonoBehaviour
{
    public float period=5f;
    public Vector3 targetPosition;
    private float timer;

    private RectTransform rect;
    private Vector3 originalPosition;

    void Start()
    {
        rect=GetComponent<RectTransform>();
        originalPosition=rect.anchoredPosition;
    }

    void Update()
    {
        timer+=Time.deltaTime;

        rect.anchoredPosition=Vector3.Lerp(originalPosition,originalPosition+targetPosition,(timer%period)/period);
    }
}
