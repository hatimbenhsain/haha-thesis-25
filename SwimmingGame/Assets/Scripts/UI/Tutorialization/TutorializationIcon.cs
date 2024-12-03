using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;

public class TutorializationIcon : MonoBehaviour
{
    public Sprite[] activeSprites;
    public Sprite[] idleSprites;
    public Sprite[] joystickSprites;

    private float imageIndex=0f;

    public float imageSpeed=4f;

    private Sprite[] sprites;

    public bool active=false;

    [Tooltip("Equivalent of index of of the input in \"values\" array in PlayerInput")]
    public int[] inputNums;

    public bool isButton=true;
    public bool isJoystick=false;

    private Image image;
    private PlayerInput playerInput;

    public RectTransform joystick;
    private Vector2 joystickInitialPosition;
    public float joystickRadius;

    public bool ignoreYAxisInverted=false;

    void Start()
    {
        image=GetComponent<Image>();
        playerInput=FindObjectOfType<PlayerInput>();
        joystickInitialPosition=joystick.anchoredPosition;
    }

    void Update()
    {
        if(isButton){
            active=false;
            foreach(int inputNum in inputNums){
                if((bool)playerInput.values[inputNum]){
                    active=true;
                    break;
                }
            }
        }else if(isJoystick){
            Vector2 v=joystickInitialPosition;
            v.x=v.x+Mathf.Clamp(((float)playerInput.values[inputNums[0]]),-1f,1f)*joystickRadius;
            if(playerInput.yAxisInverted && !ignoreYAxisInverted){
                v.y=v.y-Mathf.Clamp(((float)playerInput.values[inputNums[0]]),-1f,1f)*joystickRadius;
            }else{
                v.y=v.y+Mathf.Clamp(((float)playerInput.values[inputNums[1]]),-1f,1f)*joystickRadius;
            }
            joystick.anchoredPosition=v;
            if(Mathf.Abs((float)playerInput.values[inputNums[0]])>0.2f || Mathf.Abs((float)playerInput.values[inputNums[1]])>0.2f){
                active=true;
            }else{
                active=false;
            }
        }

        if(active){
            sprites=activeSprites;
            image.color=new Color(1f,1f,1f,0.5f);
        }else{
            sprites=idleSprites;
            image.color=new Color(1f,1f,1f,1f);
        }

        imageIndex+=imageSpeed*Time.deltaTime;
        imageIndex=imageIndex%sprites.Length;

        image.sprite=sprites[Mathf.FloorToInt(imageIndex)];
        if(isJoystick){
            joystick.GetComponent<Image>().sprite=joystickSprites[Mathf.FloorToInt(imageIndex)];
        }

    }
}
