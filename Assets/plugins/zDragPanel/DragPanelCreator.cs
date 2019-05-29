using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Z;
public class DragPanelCreator : MonoBehaviour
{

    public bool createDragBorders;
    [Range(1, 16)]
    public float headerHeight = 12;
    [Range(1, 16)]
    public float borderWidth = 6;
    // Start is called before the first frame update
    void OnValidate()
    {
        if (createDragBorders) CreateBorders();
    }

    void CreateBorders()
    {
        createDragBorders = false;
#if UNITY_EDITOR

        int numBorders = System.Enum.GetNames(typeof(zDragBorder.Borders)).Length - 1;
        GameObject frame = null;
        var dp = GetComponent<zDragPanel>();
        if (dp == null) gameObject.AddComponent<zDragPanel>();
        var fr = transform.Find("Frame");
        var borders = new zDragBorder[numBorders];
        if (fr != null) frame = fr.gameObject;

        if (frame == null)
        {
            RectTransform frameRect;
            frame = new GameObject("Frame");

            Undo.RegisterCreatedObjectUndo(frame, "Drag rect creator");
            frameRect = frame.AddComponent<RectTransform>();
            frameRect.SetParent(transform);
            frameRect.offsetMax = new Vector2(0, 0);
            frameRect.offsetMin = new Vector2(0, 0);
            frameRect.anchorMin = new Vector2(0, 0);
            frameRect.anchorMax = new Vector2(1, 1);
            frameRect = frame.GetComponent<RectTransform>();
            var borderRects = new RectTransform[numBorders];

            GameObject thisSegment;
            var le = frame.AddComponent<LayoutElement>();
            le.ignoreLayout = true;
            for (int i = 0; i < numBorders; i++)
            {
                thisSegment = new GameObject("Drag " + ((zDragBorder.Borders)i).ToString());
                //
                zDragBorder border = thisSegment.AddComponent<zDragBorder>();
                var segRect = thisSegment.GetComponent<RectTransform>();
                segRect.SetParent(frameRect);
                segRect.offsetMax = new Vector2(0, 0);
                segRect.offsetMin = new Vector2(0, 0);
                segRect.anchorMin = new Vector2(0, 0);
                segRect.anchorMax = new Vector2(1, 1);
                border.SetDirection((zDragBorder.Borders)i);
                //     SetBorderWidth(border,borderWidth, headerHeight);
                //     border.SetTargetRect(rect);
                borders[i] = border;
                borderRects[i] = segRect;

                /*   if (i == (int)zDraggableBorder.Borders.Drag)
                      {
                          header = thisSegment.transform;
                          HorizontalLayoutGroup headerLayout = thisSegment.AddComponent<HorizontalLayoutGroup>();
                          headerLayout.childControlWidth = true;
                          headerLayout.childForceExpandWidth = true;
                      }*/
            }
            for (int i = 0; i < numBorders; i++)
            {
                borders[i].SetDirection((zDragBorder.Borders)i);
               // if (i == (int)zDraggableBorder.Borders.L || i == (int)zDraggableBorder.Borders.R)
                SetBorderWidth(borders[i], borderWidth, headerHeight);
                EditorApplication.delayCall += () => Undo.DestroyObjectImmediate(this);
            }
            frame.gameObject.SetActive(false);
            frame.gameObject.SetActive(true);
        }
        else

        {
            Debug.Log("Frame already exists, exiting ", frame);
        }



#endif

    }

    public void SetBorderWidth(zDragBorder border, float newBorderWidth, float newHeaderHeight)
    {
        // checkReferences();
        borderWidth = newBorderWidth;
        headerHeight = newHeaderHeight;
        var rect = border.GetComponent<RectTransform>();
        switch (border.borderType)
        {
            case zDragBorder.Borders.L:
            case zDragBorder.Borders.R:
            case zDragBorder.Borders.BR:
            case zDragBorder.Borders.BL:
                rect.SetSizeX(borderWidth);
                break;
        }
        switch (border.borderType)
        {
            case zDragBorder.Borders.B:
            case zDragBorder.Borders.BR:
            case zDragBorder.Borders.BL:

                rect.SetSizeY(borderWidth);
                break;

            case zDragBorder.Borders.Top:
                rect.SetSizeY(headerHeight);
                rect.sizeDelta=new Vector2(2*borderWidth,headerHeight);
                //     rect.sizeDelta = new Vector2(2 * borderWidth, headerHeight);
                //     rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 0);
                break;
        }
/*
        switch (border.borderType)
        {
            case zDragBorder.Borders.L:
                //rect.sizeDelta = new Vector2(borderWidth, headerHeight);
                rect.sizeDelta = new Vector2(borderWidth, 0);
                //   rect.sizeDelta = new Vector2(borderWidth, targetRect.sizeDelta.y);
                break;
            case zDragBorder.Borders.R:
                rect.sizeDelta = new Vector2(borderWidth, 0);
                break;
            case zDragBorder.Borders.B:
                rect.sizeDelta = new Vector2(0, borderWidth);
                break;
            case zDragBorder.Borders.BR:
                rect.sizeDelta = new Vector2(borderWidth, borderWidth);
                break;
            case zDragBorder.Borders.BL:
                rect.sizeDelta = new Vector2(borderWidth, borderWidth);
                break;
            /*           case zDragBorder.Borders.TL:
                           rect.sizeDelta = new Vector2(borderWidth, borderWidth);
                           break;
                       case zDragBorder.Borders.TR:
                           rect.sizeDelta = new Vector2(borderWidth, borderWidth);
                           break;

                       case Borders.T:
                           rect.sizeDelta = new Vector2(0, borderWidth); // /2 makes the top line thinner
                           break;
           */
         //   case zDragBorder.Borders.Top:
        //        rect.sizeDelta = new Vector2(2 * borderWidth, headerHeight);
   //     rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 0);
       // break;
   // }

    }

}
