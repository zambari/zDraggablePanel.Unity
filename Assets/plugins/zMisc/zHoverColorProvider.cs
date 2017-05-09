//z2k17

using System;
using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.UI.Extensions;
//using System.Collections;
//using System.Collections.Generic;
public interface IProvideColors
{
     Action getColorsChangedAction();

     Color getNormalColor();
    Color getHoveredColor();
    Color getActiveColor();
   Color getdisabledColor();
  
}
public class zHoverColorProvider : MonoBehaviour,IProvideColors
{
    public Action colorsChanged;

    public Color normalColor = new Color(1, 1, 1, 0.2f);
    public Color hoveredColor = new Color(1, .5f, .1f, 0.2f);
    public Color activeColor = new Color(1, 1, 1, 0.5f);
    public Color disabledColor = new Color(0, 0, 0, 0.2f);
    public Action getColorsChangedAction()    {  return colorsChanged;  }
    public Color getNormalColor()    {   return normalColor;   }
    public Color getHoveredColor()    {  return hoveredColor;   }
    public Color getActiveColor()    {    return activeColor;  }
    public Color getdisabledColor()    {    return disabledColor;   }
    void OnValidate()    {      if (colorsChanged != null) colorsChanged();  }

}
