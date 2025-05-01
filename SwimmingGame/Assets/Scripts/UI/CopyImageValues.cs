using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class CopyImageValues : MonoBehaviour

{
    public Image imgToCopy;
    public bool copyScale=false;

    public bool copySpriteLibraryAsset=false;
    public SpriteLibraryAsset[] spriteLibraryAssets; 

    private Image img;

    public bool copyHue=false;

    public bool copyColor=false;

    [Tooltip("Match copied sprite's opacity while maintaining original color.")]
    public bool matchOpacity=false;

    public bool copyImage=false;
    private Color originalColor;

    private bool copied=false;
    [Tooltip("If permanent copy throughout runtime, not just at the start of scene.")]
    public bool permanent=false;

    void Start()
    {
        if(matchOpacity){
            originalColor=GetComponentInChildren<Image>().color;
        }   
    }

    void LateUpdate()
    {
        if(!copied || permanent){
            img=GetComponentInChildren<Image>();

            if(copyScale){
                transform.localScale=imgToCopy.transform.localScale;
            }


            if(copySpriteLibraryAsset){
                SpriteLibrary spriteLibrary=GetComponent<SpriteLibrary>();
                spriteLibrary.spriteLibraryAsset=imgToCopy.GetComponent<SpriteLibrary>().spriteLibraryAsset;
            }

            if(copyHue){
                img.material.color=imgToCopy.material.color;
            }

            if(copyColor){
                img.color=imgToCopy.color;
            }

            if(matchOpacity){
                Color c=originalColor;
                c.a=imgToCopy.color.a*c.a;
                img.color=c;
            }

            if(copyImage){
                img.sprite=imgToCopy.sprite;
            }

            copied=true;
        }
    }

}
