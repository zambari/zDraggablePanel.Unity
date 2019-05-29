using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if not
public static class LayoutExtensions
{

    public static int getActiveElementCount(this VerticalLayoutGroup layout)
    {
        int count = 0;
        if (layout == null) return count;
        for (int i = 0; i < layout.transform.childCount; i++)
        {
            GameObject thisChild = layout.transform.GetChild(i).gameObject;
            if (thisChild != null)
            {
                LayoutElement le = thisChild.GetComponent<LayoutElement>();
                if (le != null)
                {
                    if (!le.ignoreLayout) count++;
                }
            }

        }

        return count;
    }


    public static LayoutElement getLayout(RectTransform rect)
    {
        LayoutElement le = null;
        LayoutGroup lg = rect.GetComponent<LayoutGroup>();
        if (lg != null)
        {
            le = rect.gameObject.AddComponent<LayoutElement>();

            le.flexibleHeight = 1;
            le.flexibleWidth = 1;
        }
        return le;
    }


    public static void setChildControl(this HorizontalLayoutGroup layout, float spacing = 0)

    {
        if (layout == null) return;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = false;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.spacing = spacing;
    }

    public static void setChildControl(this VerticalLayoutGroup layout, float spacing = 0)

    {
        if (layout == null) return;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = false;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.spacing = spacing;
    }
    public static LayoutElement[] getActiveElements(this VerticalLayoutGroup layout)
    {
        List<LayoutElement> elements = new List<LayoutElement>();
        if (layout == null) return elements.ToArray();
        for (int i = 0; i < layout.transform.childCount; i++)
        {
            GameObject thisChild = layout.transform.GetChild(i).gameObject;
            LayoutElement le = thisChild.GetComponent<LayoutElement>();
            if (le != null && !le.ignoreLayout) elements.Add(le);
        }
        return elements.ToArray();
    }


    public static LayoutElement[] getActiveElements(this GameObject g)
    {
        List<LayoutElement> elements = new List<LayoutElement>();
Debug.Log("seacrihg "+g.transform.childCount);
        for (int i = 0; i < g.transform.childCount; i++)
        {
            GameObject thisChild = g.transform.GetChild(i).gameObject;
            LayoutElement le = thisChild.GetComponent<LayoutElement>();
            if (le == null) Debug.Log(" NO LAYUT ELEMENT ON GAMEOBJECT " + thisChild.name, thisChild);
            else
            {
                LayoutHelper lh = thisChild.GetComponent<LayoutHelper>();
                if (lh != null) Debug.Log("lh present");
                else
            if (!le.ignoreLayout)
                    elements.Add(le);
            }
     
        }


        return elements.ToArray();
    }
    public static Image AddImageChild(this GameObject g, float opacity = 0.3f)
    {
        Image image = g.AddComponent<Image>();
        image.color = new Color(Random.value * 0.3f + 0.7f,
             Random.value * 0.3f + 0.7f,
         Random.value * 0.2f, opacity);


        image.name = "Image";
        Debug.Log("added image to " + g.name, g);
        return image;
    }


    public static Image AddImageChild(this RectTransform rect, float opacity = 0.3f)
    {
        return rect.gameObject.AddImageChild(opacity);
    }

}
#endif