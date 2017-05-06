//zzambari : stereoko 2017

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using UnityEngine.UI.Extensions;
//using System.Collections;
//using System.Collections.Generic;
//namespace zHelpers
//{
[RequireComponent(typeof(Image))]
public class zDraggableBorder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{


    [SerializeField]
    [HideInInspector]
    float borderWidth = 6;
    [SerializeField]
    [HideInInspector]

    float headerHeight = 20;

    [SerializeField]
    [HideInInspector]
    Texture2D hoverCursor;

    [SerializeField]
    [HideInInspector]
    zDraggable draggable;
    public enum Borders { Drag, L, T, R, B, TL, TR, BR, BL, None, Disabled }
    public Borders resizeDirection;
    public void setDirection(Borders newDir)
    {
        resizeDirection = newDir;

    }
    public bool ignoreLayout;
    [SerializeField]
    [HideInInspector]
    RectTransform dragRect;
    [SerializeField]
    [HideInInspector]
    Image image;
    bool isDragging;

    RectTransform rect;
    void Awake()
    {
        draggable = GetComponentInParent<zDraggable>();
        image = GetComponent<Image>();
        if (image == null) image = gameObject.AddComponent<Image>();
    }
    void highlight()
    {
          if (draggable!=null&&image!=null)
        image.color = draggable.hoverColor;
    }
    void restore()
    {  if (draggable!=null&&image!=null)
        image.color = draggable.neutralColor;
         if (resizeDirection==Borders.Drag) if (draggable.transparentHeader) image.color=new Color(0,0,0,0); 
    }
    public void setImage(Sprite newSprite)
    {
        if (image == null) image = GetComponent<Image>();
        image.sprite = newSprite;
     //   Debug.Log("set " + newSprite.name, gameObject);
    }
    public void showHover(Borders b)
    {
        if (b == resizeDirection)
        { highlight(); return; }
        switch (b)
        {
             case Borders.Drag: if (draggable.transparentHeader) image.color=new Color(0,0,0,0); return;
            case Borders.None: restore(); return;
            case Borders.TL:
                if (resizeDirection == Borders.T || resizeDirection == Borders.L) highlight(); else restore();
                return;
            case Borders.TR:
                if (resizeDirection == Borders.T || resizeDirection == Borders.R) highlight(); else restore();
                return;
            case Borders.BL:
                if (resizeDirection == Borders.B || resizeDirection == Borders.L) highlight(); else restore();
                return;
            case Borders.BR:
                if (resizeDirection == Borders.B || resizeDirection == Borders.R) highlight(); else restore();
                return;
        }
        restore();
    }

    public void setBorderWidth(float newBorderWidth, float newHeaderHeight)
    {
        if (ignoreLayout) return;

        borderWidth = newBorderWidth;
        headerHeight = newHeaderHeight;
        if (rect == null) rect = GetComponent<RectTransform>();
        if (image == null) image = GetComponent<Image>();
        if (draggable==null) draggable=GetComponentInParent<zDraggable>();
        switch (resizeDirection)
        {
            case Borders.L:
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(1, 0);
                if (draggable.stretchSidesToTop)
                  rect.sizeDelta = new Vector2(borderWidth, headerHeight);
                else
                    rect.sizeDelta = new Vector2(borderWidth, 0);
            
                rect.anchoredPosition = new Vector2(0, 0);
                break;
            case Borders.R:
                rect.anchorMin = new Vector2(1, 0);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(0, 0);
                if (draggable.stretchSidesToTop)
                  rect.sizeDelta = new Vector2(borderWidth, headerHeight);
                else
                   rect.sizeDelta = new Vector2(borderWidth, 0);

                rect.anchoredPosition = new Vector2(0, 0);
                break;
            case Borders.B:
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(0.5f, 1);
                rect.sizeDelta = new Vector2(0, borderWidth);
                rect.anchoredPosition = new Vector2(0, 0);
                break;

            case Borders.BR:
                rect.anchorMin = new Vector2(1, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(0f, 1f);
                if (draggable.useCorners)
                rect.sizeDelta = new Vector2(borderWidth, borderWidth);
                else rect.sizeDelta = new Vector2(0, 0);
                break;
            case Borders.BL:
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0, 0);
                rect.pivot = new Vector2(1f, 1f);
                        if (draggable.useCorners)
                rect.sizeDelta = new Vector2(borderWidth, borderWidth);
                else rect.sizeDelta = new Vector2(0, 0);
                rect.anchoredPosition = new Vector2(0, 0);
                break;
            case Borders.TL:
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(1, 0);
                        if (draggable.useCorners)
                rect.sizeDelta = new Vector2(borderWidth, borderWidth);
                else rect.sizeDelta = new Vector2(0, 0);
                rect.anchoredPosition = new Vector2(0, 0);
                         if (draggable.stretchSidesToTop)
                rect.anchoredPosition = new Vector2(0, headerHeight);
                else
                 rect.anchoredPosition = new Vector2(0, 0);
                break;
            case Borders.TR:
                rect.anchorMin = new Vector2(1, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(0f, 0f);
               if (draggable.useCorners)
                rect.sizeDelta = new Vector2(borderWidth, borderWidth);
                else rect.sizeDelta = new Vector2(0, 0);
                if (draggable.stretchSidesToTop)
                rect.anchoredPosition = new Vector2(0, headerHeight);
                else
                 rect.anchoredPosition = new Vector2(0, 0);
                break;

            case Borders.T:

                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(.5f, 0);
                rect.sizeDelta = new Vector2(0, borderWidth); // /2 makes the top line thinner
                //rect.sizeDelta = new Vector2(0, borderWidth ); // /2 makes the top line thinner
                rect.anchoredPosition = new Vector2(0, headerHeight);
                break;

            case Borders.Drag:
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(0.5f, 0);

                rect.sizeDelta = new Vector2(0/*2 * borderWidth*/, headerHeight);
                rect.anchoredPosition = new Vector2(0, 0);
                break;
        }
        if (resizeDirection != Borders.BR && resizeDirection != Borders.BL)
            if (draggable == null)
                draggable = GetComponentInParent<zDraggable>();
        if (image == null) image = GetComponent<Image>();


        restore();
       
    }


    void OnValidate()
    {
        if (!gameObject.activeInHierarchy) return;

        if (draggable == null)
            draggable = GetComponentInParent<zDraggable>();
        if (dragRect == null) dragRect = draggable.GetComponent<RectTransform>();
        if (image == null) image = GetComponent<Image>();
        switch (resizeDirection)
        {
            case Borders.L:
            case Borders.R:
                hoverCursor = zDraggable.horizontalCursor;
                break;
            case Borders.B:
            case Borders.T:
                hoverCursor = zDraggable.vertialCursor;
                break;
            case Borders.BL:
            case Borders.TR:
                hoverCursor = zDraggable.upRightResizeCursor;
                break;
            case Borders.TL:
            case Borders.BR:
                hoverCursor = zDraggable.upLeftResizeCursor;
                break;
            case Borders.Drag:

                hoverCursor = zDraggable.moveCursor;
                break;
        }
        showHover(Borders.None);
        setBorderWidth(borderWidth, headerHeight);
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (resizeDirection == Borders.None) return;
        if (draggable==null) return;
        draggable.setHoverState(resizeDirection);

       
        Cursor.SetCursor(hoverCursor, new Vector2(16, 16), CursorMode.Auto);
    }
    public void OnPointerExit(PointerEventData e)
    {
        if (isDragging) return;
     if (draggable==null) return;   
        draggable.setHoverState(Borders.None);
     
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    Vector3 lastCursorPosition;
    Vector3 startPosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        draggable.OnDragOperationStart();
        if (resizeDirection == Borders.B) draggable.setVerticalPivot(zDraggable.AnchorModes.max);
        if (resizeDirection == Borders.T) draggable.setVerticalPivot(zDraggable.AnchorModes.min);
        if (resizeDirection == Borders.R) { draggable.setHorizontalPivot(zDraggable.AnchorModes.min); }
        if (resizeDirection == Borders.L) { draggable.setHorizontalPivot(zDraggable.AnchorModes.max); }

        if (resizeDirection == Borders.TL)
        {
            draggable.setHorizontalPivot(zDraggable.AnchorModes.min);
            draggable.setVerticalPivot(zDraggable.AnchorModes.max);
        }

        if (resizeDirection == Borders.TR)
        {
            draggable.setHorizontalPivot(zDraggable.AnchorModes.min);
            draggable.setVerticalPivot(zDraggable.AnchorModes.min);
        }
        if (resizeDirection == Borders.TL)
        {
            draggable.setVerticalPivot(zDraggable.AnchorModes.min);
            draggable.setHorizontalPivot(zDraggable.AnchorModes.max);
        }
        if (resizeDirection == Borders.BR)
        {
            draggable.setHorizontalPivot(zDraggable.AnchorModes.min);
            draggable.setVerticalPivot(zDraggable.AnchorModes.max);
        }
        if (resizeDirection == Borders.BL)
        {
            draggable.setHorizontalPivot(zDraggable.AnchorModes.max);
            draggable.setVerticalPivot(zDraggable.AnchorModes.max);
        }
        draggable.setAnchorAndPivotBasedOnAnchoring();
        startPosition = dragRect.localPosition;
        lastCursorPosition = Input.mousePosition;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width ||
            Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height) return;
  draggable.OnDragOperation();     
        Vector3 drag = Input.mousePosition - lastCursorPosition;
        if (resizeDirection == Borders.Drag)
        {
            draggable.setPosition(startPosition + drag);
            return;
        }
        else if (resizeDirection == Borders.T)
            draggable.setDimensions(new Vector2(dragRect.sizeDelta.x, dragRect.sizeDelta.y + drag.y));
        else if (resizeDirection == Borders.B)
            draggable.setDimensions(new Vector2(dragRect.sizeDelta.x, dragRect.sizeDelta.y - drag.y));
        else if (resizeDirection == Borders.L)
            draggable.setDimensions(new Vector2(dragRect.sizeDelta.x - drag.x, dragRect.sizeDelta.y));
        else if (resizeDirection == Borders.R)
            draggable.setDimensions(new Vector2(dragRect.sizeDelta.x + drag.x, dragRect.sizeDelta.y));
        else if (resizeDirection == Borders.TL)
            draggable.setDimensions(new Vector2(dragRect.sizeDelta.x - drag.x, dragRect.sizeDelta.y + drag.y));
        else if (resizeDirection == Borders.TR)
            draggable.setDimensions(new Vector2(dragRect.sizeDelta.x + drag.x, dragRect.sizeDelta.y + drag.y));
        if (resizeDirection == Borders.BL)
            draggable.setDimensions(new Vector2(dragRect.sizeDelta.x - drag.x, dragRect.sizeDelta.y - drag.y));
        else if (resizeDirection == Borders.BR)
            draggable.setDimensions(new Vector2(dragRect.sizeDelta.x + drag.x, dragRect.sizeDelta.y - drag.y));

        lastCursorPosition = Input.mousePosition;
           
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        draggable.OnDragOperationEnded();
        isDragging = false;

        if (resizeDirection != Borders.BR && resizeDirection == Borders.BR)
            image.color = draggable.neutralColor;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }


}
