using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBoxPlacement : MonoBehaviour
{
    public bool placeMe=false;
    public bool bubbles=false;
    public bool moveMCRect=false;

    public RectTransform bubble1;
    public RectTransform bubble2;

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

    //Bubble stuff
    private float bubbleAngle=0f;
    private float bubbleDirection=1f;
    public float bubbleSmoothTime=.5f;
    private Vector2 bubble1targetPos;
    private Vector2 bubble2targetPos;
    private Vector2 bubble1Velocity;
    private Vector2 bubble2Velocity;

    public RectTransform mcRect;

    private float bubbleOutsideBoundsTimer=0f;

    public RectTransform mcBubble1;
    public RectTransform mcBubble2;


    void Start()
    {
        dialogue=FindObjectOfType<Dialogue>();
        rect=GetComponent<RectTransform>();
        originalPosition=rect.anchoredPosition;
        canvasRect=GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        if(bubbles){
            bubble1.transform.parent=transform.parent;
            bubble2.transform.parent=transform.parent;
            bubble2.SetSiblingIndex(0);
            bubble1.SetSiblingIndex(1);
        }
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
            while(((pos.y-minOvalHeight/2f<-canvasRect.sizeDelta.y/2f || pos.y+minOvalHeight/2f>canvasRect.sizeDelta.y/2f || pos.x-minOvalWidth/2f<-canvasRect.sizeDelta.x/2f ||
            pos.x+minOvalWidth/2f>canvasRect.sizeDelta.x/2f) || OverlapsChoiceBoxes(rect,pos+canvasRect.sizeDelta/2f))&& tries<180){
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

            if(bubbles){
                Color c=rect.GetComponentInChildren<Image>().color;
                c.a=c.a*(1f-bubbleOutsideBoundsTimer);
                bubble1.GetComponent<Image>().color=c;
                bubble2.GetComponent<Image>().color=c;
                //Vector2 circleCenter=(npcPos+rect.anchoredPosition)/2f;

                // float alpha=angle+90f;
                // //alpha=0f;
                // float r=Vector2.Distance(npcPos,rect.anchoredPosition)/2f;
                // Debug.Log(alpha);
                // pos=circleCenter;
                // pos.x+=Mathf.Cos(alpha*Mathf.PI/180f)*r;
                // pos.y+=Mathf.Sin(alpha*Mathf.PI/180f)*r;
                bool bubbleOutsideBounds=false;

                if(!success || Vector3.Angle(camera.transform.forward,dialogue.npcInterlocutor.transform.position-camera.transform.position)>90f){
                    bubbleOutsideBoundsTimer+=Time.deltaTime;
                    bubbleOutsideBounds=true;
                }else{
                    bubbleOutsideBoundsTimer-=Time.deltaTime;
                }

                if(!bubbleOutsideBounds){
                    pos=GetRightTrianglePoint(npcPos,rect.anchoredPosition,angle,true);
                    tries=0;
                    while((pos.y-bubble1.sizeDelta.y/2f<-canvasRect.sizeDelta.y/2f || pos.y+bubble1.sizeDelta.y/2f>canvasRect.sizeDelta.y/2f || 
                        pos.x-bubble1.sizeDelta.x/2f<-canvasRect.sizeDelta.x/2f || pos.x+bubble1.sizeDelta.x/2f>canvasRect.sizeDelta.x/2f || 
                        Mathf.Sign(pos.x)!=npcPos.x || Mathf.Sign(pos.y)!=npcPos.y) && tries<2){
                        bubbleDirection=-bubbleDirection;
                        pos=GetRightTrianglePoint(npcPos,rect.anchoredPosition,angle,true);
                        tries+=1;
                    }

                    
                    bubbleOutsideBoundsTimer=Mathf.Clamp(bubbleOutsideBoundsTimer,0f,1f);

                    bubble1targetPos=pos;
                    float offset=0f;
                    if(bubbleDirection==-1f){
                        offset=-90f;
                    }
                    Vector2 circleCenter=(npcPos+bubble1.anchoredPosition)/2f;
                    bubble2targetPos=(circleCenter+GetRightTrianglePoint(npcPos,bubble1.anchoredPosition,angle+45f+offset))/2f;
                }
                bubble1targetPos.x=Mathf.Clamp(bubble1targetPos.x,(-canvasRect.sizeDelta.x+bubble1.sizeDelta.x)/2f,(canvasRect.sizeDelta.x-bubble1.sizeDelta.x)/2f);
                bubble1targetPos.y=Mathf.Clamp(bubble1targetPos.y,(-canvasRect.sizeDelta.y+bubble1.sizeDelta.y)/2f,(canvasRect.sizeDelta.y-bubble1.sizeDelta.y)/2f);
                bubble2targetPos.x=Mathf.Clamp(bubble2targetPos.x,(-canvasRect.sizeDelta.x+bubble2.sizeDelta.x)/2f,(canvasRect.sizeDelta.x-bubble2.sizeDelta.x)/2f);
                bubble2targetPos.y=Mathf.Clamp(bubble2targetPos.y,(-canvasRect.sizeDelta.y+bubble2.sizeDelta.y)/2f,(canvasRect.sizeDelta.y-bubble2.sizeDelta.y)/2f);

                bubble1.anchoredPosition=Vector2.SmoothDamp(bubble1.anchoredPosition,bubble1targetPos,ref bubble1Velocity,bubbleSmoothTime);
                bubble2.anchoredPosition=Vector2.SmoothDamp(bubble2.anchoredPosition,bubble2targetPos,ref bubble2Velocity,bubbleSmoothTime);
            }

            if(moveMCRect){
                float xDir=Mathf.Sign(rect.anchoredPosition.x);
                float yDir=Mathf.Sign(rect.anchoredPosition.y);
                pos=mcRect.anchoredPosition;
                pos.x=-Mathf.Abs(pos.x)*xDir;
                pos.y=-Mathf.Abs(pos.y)*yDir;
                mcRect.anchoredPosition=pos;
                pos=mcBubble1.anchoredPosition;
                pos.x=Mathf.Abs(pos.x)*xDir;
                pos.y=Mathf.Abs(pos.y)*yDir;
                mcBubble1.anchoredPosition=pos;
                pos=mcBubble2.anchoredPosition;
                pos.x=Mathf.Abs(pos.x)*xDir;
                pos.y=Mathf.Abs(pos.y)*yDir;
                mcBubble2.anchoredPosition=pos;
            }
        }
    }

    private Vector2 GetRightTrianglePoint(Vector2 point1,Vector2 point2,float ang,bool useOvalDimensions=false){
        Vector2 pos;
        Vector2 circleCenter=(point1+point2)/2f;

        float alpha=ang+90f;
        if(bubbleDirection==-1f){
            alpha+=180f;
        }

        float r=Vector2.Distance(point1,point2)/2f;
        pos=circleCenter;
        float w=r;
        float h=r;
        if(useOvalDimensions){
            w=minOvalWidth/2f;
            h=minOvalHeight/2f;
        }
        pos.x+=Mathf.Cos(alpha*Mathf.PI/180f)*w;
        pos.y+=Mathf.Sin(alpha*Mathf.PI/180f)*h;

        return pos;
    }

    private void OnEnable() {
        if(!placeMe){
            rect.anchoredPosition=originalPosition;
        }
        if(bubbles){
            bubble1.gameObject.SetActive(true);
            bubble2.gameObject.SetActive(true);
        }
    }

    private void OnDisable() {
        if(bubbles){
            bubble1.gameObject.SetActive(false);
            bubble2.gameObject.SetActive(false);
        }
    }

    //From https://discussions.unity.com/t/ui-how-can-i-find-a-world-position-on-the-canvas/867308/10
    public Vector2 WorldToCanvasPoint(RectTransform rectTransform, Vector3 WorldPosition, [NotNull] Camera WorldCamera, out bool Success, Camera CanvasCamera = null)
    {
        Success = RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, WorldCamera.WorldToScreenPoint(WorldPosition), CanvasCamera, out var CanvasPoint);
        return CanvasPoint;
    }

    bool OverlapsChoiceBoxes(RectTransform rect1,Vector2 position){
        GameObject[] choiceBoxes=dialogue.choiceTextBoxes;
        for(var i=0;i<choiceBoxes.Length;i++){
            RectTransform choiceRect=choiceBoxes[i].GetComponent<RectTransform>();
            if(choiceBoxes[i].activeInHierarchy && RectOverlap(rect1,choiceRect,position,choiceRect.position)){
                Debug.Log("overlap choice box");
                return true;
            }
        }
        return false;
    }

    bool RectOverlap(RectTransform rect1,RectTransform rect2){
        Vector2 pos1=rect1.position;
        Vector2 pos2=rect2.position;
        Vector2 dimensions1=rect1.sizeDelta;
        Vector2 dimensions2=rect2.sizeDelta;
        return !(pos2.x-dimensions2.x/2>=pos1.x+dimensions1.x/2 || pos2.x+dimensions2.x/2<=pos1.x-dimensions1.x/2 || pos2.y-dimensions2.y/2>=pos1.y+dimensions1.y/2 || 
        pos2.y+dimensions2.y/2<=pos1.y-dimensions1.y/2);
    }

    bool RectOverlap(RectTransform rect1,RectTransform rect2,Vector2 pos1,Vector2 pos2){
        Vector2 dimensions1=rect1.sizeDelta;
        Vector2 dimensions2=rect2.sizeDelta;
        return !(pos2.x-dimensions2.x/2>=pos1.x+dimensions1.x/2 || pos2.x+dimensions2.x/2<=pos1.x-dimensions1.x/2 || pos2.y-dimensions2.y/2>=pos1.y+dimensions1.y/2 || 
        pos2.y+dimensions2.y/2<=pos1.y-dimensions1.y/2);
    }
}
