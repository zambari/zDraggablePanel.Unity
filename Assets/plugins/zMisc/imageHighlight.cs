//z2k17

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//using UnityEngine.UI.Extensions;
//using System.Collections;
//using System.Collections.Generic;

public class imageHighlight : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {
[SerializeField]
[HideInInspector]
Image image;
Color c;
[Range(0,3)]
public float alphaMultiplier=2;
public void OnPointerEnter(PointerEventData d)
{
    image.color=new Color(c.r,c.g,c.b,c.a*alphaMultiplier);

}
public void OnPointerExit(PointerEventData d)
{
    image.color=c;
    
}
void OnValidate()

{
    if (image==null) image=GetComponent<Image>();
    if (image!=null) c=image.color;
}
void Awake()
{
    OnValidate();
}
	
}
