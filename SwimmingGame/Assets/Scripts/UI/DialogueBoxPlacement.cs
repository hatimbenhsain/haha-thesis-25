using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class DialogueBoxPlacement : MonoBehaviour
{
    public bool placeMe=false;
    private Dialogue dialogue;
    private Camera camera;
    public float angle=0f;
    public float ovalWidth=400f;
    public float ovalHeight=400f;
    public float minOvalWidth=830;
    public float minOvalHeight=400f;
    public float skinWidth=0.2f;

    private Vector2 originalPosition;
    private RectTransform rect;

    private RectTransform canvasRect;

    private Vector2 targetPos;
    public float lerpSpeed=1f;
    public float smoothTime=.5f;
    private Vector2 velocity=Vector2.zero;

    void Start()
    {
        dialogue=FindObjectOfType<Dialogue>();
        rect=GetComponent<RectTransform>();
        originalPosition=rect.anchoredPosition;
        canvasRect=GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    void Update()
    {
        if(placeMe && dialogue.npcInterlocutor!=null){
            camera=Camera.main;
            bool success;
            SpriteRenderer spriteRenderer=dialogue.npcInterlocutor.GetComponentInChildren<SpriteRenderer>();
            if(spriteRenderer==null){
                spriteRenderer=dialogue.npcInterlocutor.transform.parent.GetComponentInChildren<SpriteRenderer>();
            }
            BoxCollider collider;
            spriteRenderer.TryGetComponent<BoxCollider>(out collider);
            Vector3 customBounds;
            if(collider!=null){ 
                customBounds=collider.size*0.5f;
                Debug.Log(customBounds);
                customBounds.x=customBounds.x+skinWidth;
                customBounds.y=customBounds.y+skinWidth;
            }
            else customBounds=spriteRenderer.sprite.bounds.extents;
            customBounds=new Vector3(customBounds.x*spriteRenderer.transform.lossyScale.x,customBounds.y*spriteRenderer.transform.lossyScale.y,
            customBounds.z*spriteRenderer.transform.lossyScale.z);
            customBounds=spriteRenderer.transform.rotation*customBounds;
            if(spriteRenderer!=null){
                Vector2 boundsMin=WorldToCanvasPoint(canvasRect,spriteRenderer.transform.position-customBounds,camera,out success);
                Vector2 boundsMax=WorldToCanvasPoint(canvasRect,spriteRenderer.transform.position+customBounds,camera,out success);
                ovalWidth=Mathf.Abs(boundsMax.x-boundsMin.x)+minOvalWidth;
                ovalHeight=Mathf.Abs(boundsMax.y-boundsMin.y)+minOvalHeight;
            }
            ovalWidth=Mathf.Max(ovalWidth,minOvalWidth);
            ovalHeight=Mathf.Max(ovalHeight,minOvalHeight);
            Vector2 npcPos=WorldToCanvasPoint(canvasRect,dialogue.npcInterlocutor.transform.position,camera,out success);

            Vector2 pos=new Vector2(Mathf.Cos(angle*Mathf.PI/180f)*ovalWidth/2f+npcPos.x,Mathf.Sin(angle*Mathf.PI/180f)*ovalHeight/2f+npcPos.y);

            //Check it off-screen
            int tries=0;
            float initAngle=angle;
            float a=1;
            while((pos.y-minOvalHeight/2f<-canvasRect.sizeDelta.y/2f || pos.y+minOvalHeight/2f>canvasRect.sizeDelta.y/2f || pos.x-minOvalWidth/2f<-canvasRect.sizeDelta.x/2f ||
            pos.x+minOvalWidth/2f>canvasRect.sizeDelta.x/2f) && tries<180){
                a+=Mathf.Sign(a);
                a=-a;
                pos=new Vector2(Mathf.Cos((initAngle+a)*Mathf.PI/180f)*ovalWidth/2f+npcPos.x,Mathf.Sin((initAngle+a)*Mathf.PI/180f)*ovalHeight/2f+npcPos.y);
                tries++;
            }

            if(tries<180){
                targetPos=pos;
                angle=initAngle+a;
            } 
            rect.anchoredPosition=Vector2.SmoothDamp(rect.anchoredPosition,targetPos,ref velocity,smoothTime);
            angle=angle%360f;
        }
    }

    private void OnEnable() {
        if(!placeMe){
            rect.anchoredPosition=originalPosition;
        }
    }

    //From https://discussions.unity.com/t/ui-how-can-i-find-a-world-position-on-the-canvas/867308/10
    public Vector2 WorldToCanvasPoint(RectTransform rectTransform, Vector3 WorldPosition, [NotNull] Camera WorldCamera, out bool Success, Camera CanvasCamera = null)
    {
        Success = RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, WorldCamera.WorldToScreenPoint(WorldPosition), CanvasCamera, out var CanvasPoint);
        return CanvasPoint;
    }
}
