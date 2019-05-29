//z2k17

using System;
using UnityEngine;
using UnityEngine.Events;
//using UnityEngine.UI;
//using UnityEngine.UI.Extensions;
//using System.Collections;
//using System.Collections.Generic;
public interface IProvideColors
{

    HoverableColors getColors();


}
public interface ISyncColors
{

    //  HoverableColors getColors();
    void setColors(HoverableColors colors);
    /*
 	public  void setColors(HoverableColors colors)
            {
                if (colors!=null) {
                    this.colors=colors;
                    this.colors.OnChange-=updateColors;
                    this.colors.OnChange+=updateColors;
                }
            }
     */

/*     void updateColors()
{
 image.color=colors.editedColor;
}*/

    /*
        public void setColors(HoverableColors newcolors)
      {
          colors=newcolors;
          colors.OnChange.AddListener(onColorChange);
      }
      void onColorChange()
      {
          image.color=colors.normalColor;
      }

   */
}
[System.Serializable]
public class HoverableColors
{

    public Color normalColor = new Color(1, 1, 1, 0.2f);
    public Color hoveredColor = new Color(1, .5f, .1f, 0.2f);
    public Color activeColor = new Color(1, 1, 1, 0.5f);
    public Color activeHoveredColor = new Color(1, 1, 1, 0.8f);
    public Color disabledColor = new Color(0, 0, 0, 0.2f);

    public int channel;
    //public UnityEvent OnChange;
    public Action OnChange;
    public bool useBlock;
 //   public bool editTogether;
    public void setAllColorsTo(Color c)
    {
        normalColor = c;
        hoveredColor = c;
        activeColor = c;
        disabledColor = c;

    }
    public Color baseColor { get { return normalColor; } }
    public Color editedColor { get {
         if (previewColor==ColorType.hoveredColor) return hoveredColor;
         if (previewColor==ColorType.activeColor) return activeColor;
         if (previewColor==ColorType.activeHoveredColor) return activeHoveredColor;
         if (previewColor==ColorType.hoveredColor) return disabledColor;
         if (previewColor==ColorType.disabledColor) return disabledColor;
         return normalColor; 
         }
        }
   
   public enum ColorType { normalColor,hoveredColor,  activeColor,  activeHoveredColor, disabledColor}
  public ColorType previewColor;
    public void OnValidate(MonoBehaviour source)
    {
        ISyncColors[] colorSyncers = source.GetComponentsInChildren<ISyncColors>();
            for (int i = 0; i < colorSyncers.Length; i++)
            {
                colorSyncers[i].setColors(this);
            }
        //    Debug.Log("sent to "+colorSyncers.Length);
            if (OnChange!=null) OnChange.Invoke();



    }
}
public class zHoverColorProvider : MonoBehaviour, IProvideColors
{
    public HoverableColors colors;
    public void updateColors()
    {

    }
    public HoverableColors getColors() { return colors; }
    void OnValidate()
    {
        if (colors.OnChange != null)
            colors.OnChange.Invoke();
    }

}
