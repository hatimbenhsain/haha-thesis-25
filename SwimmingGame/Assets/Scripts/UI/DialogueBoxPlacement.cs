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
    public bool isMC=false;

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
    private Vector2 mcRectOriginalPosition;

    private float bubbleOutsideBoundsTimer=0f;

    public Transform overrideTarget;
    public Camera secondaryCamera;

    private Transform target;
    private GameObject npc;
    private Vector2 npcPos;

    [HideInInspector]
    public bool isOffscreen=false;

    // public RectTransform mcBubble1;
    // public RectTransform mcBubble2;
    // private Vector2 mcBubble1targetPos;
    // private Vector2 mcBubble2targetPos;
    // private Vector2 mcBubble1Velocity;
    // private Vector2 mcBubble2Velocity;
    // private float mcBubbleDirection=1f;



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

        if(moveMCRect){
            mcRectOriginalPosition=mcRect.anchoredPosition;
        //     if(bubbles){
        //         mcBubble1.transform.parent=transform.parent;
                // mcBubble2.transform.parent=transform.parent;
                // mcBubble2.SetSiblingIndex(0);
        //         mcBubble1.SetSiblingIndex(1);
        //     }
        }
    }

    void Update()
    {
        if(dialogue.npcInterlocutor!=null || overrideTarget!=null){
            Vector2 pos;
            bool success;
            int tries=0;

            // Find position on screen to place this bubble
            if(overrideTarget!=null){
                pos=WorldToCanvasPoint(canvasRect,overrideTarget.position,camera,out success);
            }else{
                npc=dialogue.npcInterlocutor.gameObject;
                if(isMC){
                    npc=FindObjectOfType<Swimmer>().gameObject;
                }

                camera=Camera.main;
                
                SpriteRenderer spriteRenderer=npc.GetComponentInChildren<SpriteRenderer>();
                if(spriteRenderer==null){
                    spriteRenderer=npc.transform.parent.GetComponentInChildren<SpriteRenderer>();
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
                npcPos=WorldToCanvasPoint(canvasRect,npc.transform.position,camera,out success);

                pos=new Vector2(Mathf.Cos(angle*Mathf.PI/180f)*ovalWidth/2f+npcPos.x,Mathf.Sin(angle*Mathf.PI/180f)*ovalHeight/2f+npcPos.y);

            }

            //Check it off-screen
            float initAngle=angle;
            float a=1;

            CheckIfOffscreen(pos);
            
            if(overrideTarget==null){
                while((isOffscreen || OverlapsChoiceBoxes(rect,pos+canvasRect.sizeDelta/2f)) && tries<180){
                    a+=Mathf.Sign(a);
                    a=-a;
                    pos=new Vector2(Mathf.Cos((initAngle+a)*Mathf.PI/180f)*ovalWidth/2f+npcPos.x,Mathf.Sin((initAngle+a)*Mathf.PI/180f)*ovalHeight/2f+npcPos.y);
                    CheckIfOffscreen(pos);
                    tries++;
                }
            }else{
                if(isOffscreen){
                    pos=WorldToCanvasPoint(canvasRect,overrideTarget.position,secondaryCamera,out success);
                    initAngle=Mathf.Atan2(pos.y,pos.x);
                    //pos.x=Mathf.Cos(initAngle)*canvasRect.sizeDelta.x/2f-Mathf.Sign(Mathf.Cos(initAngle))*minOvalWidth/2f;       
                    //pos.y=Mathf.Sin(initAngle)*canvasRect.sizeDelta.y/2f-Mathf.Sign(Mathf.Sin(initAngle))*minOvalHeight/2f;
                    pos.x=Mathf.Clamp(pos.x,-canvasRect.sizeDelta.x/2f+minOvalWidth/2f,canvasRect.sizeDelta.x/2f-minOvalWidth/2f);
                    pos.y=Mathf.Clamp(pos.y,-canvasRect.sizeDelta.y/2f+minOvalHeight/2f,canvasRect.sizeDelta.y/2f-minOvalHeight/2f);
                    targetPos=pos;
                    
                    while(OverlapsChoiceBoxes(rect,pos+canvasRect.sizeDelta/2f) && tries<180){
                        a=-a;
                        pos.x=Mathf.Cos(initAngle+a)*canvasRect.sizeDelta.x/2f-Mathf.Sign(Mathf.Cos(initAngle+a))*minOvalWidth/2f;       
                        pos.y=Mathf.Sin(initAngle+a)*canvasRect.sizeDelta.y/2f-Mathf.Sign(Mathf.Sin(initAngle+a))*minOvalHeight/2f;
                        a+=Mathf.Sign(a);
                        tries++;
                        break;
                    }
                }

                a-=Mathf.Sign(a);
            }


            if(placeMe){
                if(tries<180){
                    targetPos=pos;
                    angle=initAngle+a;
                } 
                rect.anchoredPosition=Vector2.SmoothDamp(rect.anchoredPosition,targetPos,ref velocity,smoothTime);
                angle=angle%360f;
            }

            if(bubbles){
                Color c=rect.GetComponentInChildren<Image>().color;
                Color outlineColor=rect.transform.Find("Image/Outline").GetComponent<Image>().color;
                c.a=c.a*(1f-bubbleOutsideBoundsTimer);
                Image[] bubbleImages=bubble1.GetComponentsInChildren<Image>();
                foreach(Image image in bubbleImages){
                    if(image.gameObject.name=="Outline"){
                        outlineColor.a=image.color.a;
                        image.color=outlineColor;
                    }else{
                        c.a=image.color.a;
                        image.color=c;
                    }
                }
                bubbleImages=bubble2.GetComponentsInChildren<Image>();
                foreach(Image image in bubbleImages){
                    if(image.gameObject.name=="Outline"){
                        outlineColor.a=image.color.a;
                        image.color=outlineColor;
                    }else{
                        c.a=image.color.a;
                        image.color=c;
                    }
                }
                //Vector2 circleCenter=(npcPos+rect.anchoredPosition)/2f;

                // float alpha=angle+90f;
                // //alpha=0f;
                // float r=Vector2.Distance(npcPos,rect.anchoredPosition)/2f;
                // Debug.Log(alpha);
                // pos=circleCenter;
                // pos.x+=Mathf.Cos(alpha*Mathf.PI/180f)*r;
                // pos.y+=Mathf.Sin(alpha*Mathf.PI/180f)*r;
                bool bubbleOutsideBounds=false;

                if(!success || Vector3.Angle(camera.transform.forward,npc.transform.position-camera.transform.position)>90f){
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
                        (Mathf.Sign(pos.x)!=npcPos.x && !isMC) || (Mathf.Sign(pos.y)!=npcPos.y && !isMC)) && tries<2){
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
                pos=mcRectOriginalPosition;
                pos.x=-Mathf.Abs(pos.x)*xDir;
                pos.y=-Mathf.Abs(pos.y)*yDir;
                mcRect.anchoredPosition=pos;

                //Tried moving mc bubbles but failed

                //Vector2 mcPos=WorldToCanvasPoint(canvasRect,dialogue.npcInterlocutor.transform.position,camera,out success);

                // pos=GetRightTrianglePoint(mcPos,mcRect.anchoredPosition,angle,true);
                // tries=0;
                // while((pos.y-mcBubble1.sizeDelta.y/2f<-canvasRect.sizeDelta.y/2f || pos.y+mcBubble1.sizeDelta.y/2f>canvasRect.sizeDelta.y/2f || 
                //     pos.x-mcBubble1.sizeDelta.x/2f<-canvasRect.sizeDelta.x/2f || pos.x+mcBubble1.sizeDelta.x/2f>canvasRect.sizeDelta.x/2f || 
                //     Mathf.Sign(pos.x)!=mcPos.x || Mathf.Sign(pos.y)!=mcPos.y) && tries<2){
                //     mcBubbleDirection=-mcBubbleDirection;
                //     pos=GetRightTrianglePoint(mcPos,mcRect.anchoredPosition,angle,true);
                //     tries+=1;
                // }

                
                // bubbleOutsideBoundsTimer=Mathf.Clamp(bubbleOutsideBoundsTimer,0f,1f);

                // mcBubble1targetPos=pos;
                // float offset=0f;
                // if(mcBubbleDirection==-1f){
                //     offset=-90f;
                // }
                // Vector2 circleCenter=(mcPos+mcBubble1.anchoredPosition)/2f;
                // mcBubble2targetPos=(circleCenter+GetRightTrianglePoint(mcPos,mcBubble1.anchoredPosition,angle+45f+offset))/2f;
                
                // mcBubble1targetPos.x=Mathf.Clamp(mcBubble1targetPos.x,(-canvasRect.sizeDelta.x+mcBubble1.sizeDelta.x)/2f,(canvasRect.sizeDelta.x-bubble1.sizeDelta.x)/2f);
                // mcBubble1targetPos.y=Mathf.Clamp(mcBubble1targetPos.y,(-canvasRect.sizeDelta.y+mcBubble1.sizeDelta.y)/2f,(canvasRect.sizeDelta.y-bubble1.sizeDelta.y)/2f);
                // mcBubble2targetPos.x=Mathf.Clamp(mcBubble2targetPos.x,(-canvasRect.sizeDelta.x+mcBubble2.sizeDelta.x)/2f,(canvasRect.sizeDelta.x-mcBubble2.sizeDelta.x)/2f);
                // mcBubble2targetPos.y=Mathf.Clamp(mcBubble2targetPos.y,(-canvasRect.sizeDelta.y+mcBubble2.sizeDelta.y)/2f,(canvasRect.sizeDelta.y-mcBubble2.sizeDelta.y)/2f);

                // mcBubble1.anchoredPosition=Vector2.SmoothDamp(mcBubble1.anchoredPosition,mcBubble1targetPos,ref mcBubble1Velocity,bubbleSmoothTime);
                // mcBubble2.anchoredPosition=Vector2.SmoothDamp(mcBubble2.anchoredPosition,mcBubble2targetPos,ref mcBubble2Velocity,bubbleSmoothTime);
                
                // pos=mcBubble1.anchoredPosition;
                // pos.x=Mathf.Abs(pos.x)*xDir;
                // pos.y=Mathf.Abs(pos.y)*yDir;
                // mcBubble1.anchoredPosition=pos;
                // pos=mcBubble2.anchoredPosition;
                // pos.x=Mathf.Abs(pos.x)*xDir;
                // pos.y=Mathf.Abs(pos.y)*yDir;
                // pos=(mcBubble1.anchoredPosition+mcPos)/2f;
                // mcBubble2.anchoredPosition=pos;
            }
        }
    }

    private void CheckIfOffscreen(Vector2 pos){
        isOffscreen=pos.y-minOvalHeight/2f<-canvasRect.sizeDelta.y/2f || pos.y+minOvalHeight/2f>canvasRect.sizeDelta.y/2f || pos.x-minOvalWidth/2f<-canvasRect.sizeDelta.x/2f ||
                pos.x+minOvalWidth/2f>canvasRect.sizeDelta.x/2f;
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
        if(rect==null){
            rect=GetComponent<RectTransform>();
            originalPosition=rect.anchoredPosition;
            dialogue=FindObjectOfType<Dialogue>();
        }
        if(!placeMe && !isMC){
            rect.anchoredPosition=originalPosition;
        }
        if(bubbles){
            bubble1.gameObject.SetActive(true);
            bubble2.gameObject.SetActive(true);
        }
        camera=Camera.main;
        canvasRect=GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        if(overrideTarget==null){
            npc=dialogue.npcInterlocutor.gameObject;
            if(isMC){
                npc=FindObjectOfType<Swimmer>().gameObject;
            }
            bool success;
            npcPos=WorldToCanvasPoint(canvasRect,npc.transform.position,camera,out success);
            angle=Mathf.Atan2((rect.anchoredPosition.y-npcPos.y)/minOvalHeight,(rect.anchoredPosition.x-npcPos.x)/minOvalWidth)*180f/Mathf.PI;
        }
        Debug.Log(angle);
        targetPos=rect.anchoredPosition;
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
        // if(Success==false && overrideTarget!=null){
        //     return (Vector2)WorldCamera.WorldToScreenPoint(WorldPosition)-canvasRect.sizeDelta/2f;
        // }
        return CanvasPoint;
    }

    bool OverlapsChoiceBoxes(RectTransform rect1,Vector2 position){
        GameObject[] choiceBoxes=dialogue.choiceTextBoxes;
        for(var i=0;i<choiceBoxes.Length;i++){
            RectTransform choiceRect=choiceBoxes[i].GetComponent<RectTransform>();
            if(choiceBoxes[i].activeInHierarchy && rect1!=choiceRect && RectOverlap(rect1,choiceRect,position,choiceRect.position)){
                Debug.Log("overlap choice box");
                return true;
            }
        }
        return false;
    }

    bool RectOverlap(RectTransform rect1,RectTransform rect2){
        Vector2 pos1=rect1.position;
        Vector2 pos2=rect2.position;
        Vector2 dimensions1=rect1.sizeDelta*rect1.lossyScale;
        Vector2 dimensions2=rect2.sizeDelta*rect2.lossyScale;
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
