//z2k17

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using UnityEngine.UI.Extensions;
//using System.Collections;
//using System.Collections.Generic;
//namespace zHelpers
//{
public class zDraggableBorder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    [HideInInspector]
    Texture2D hoverCursor;

    [SerializeField]
    [HideInInspector]
    zDraggable draggable;

    public enum ResizeDirs { UpLeft, HorizontalLeft, HorizontalRight, UpRight, VerticalUp, VerticalDown, DownLeft, DownRight, Pan, None }
    public ResizeDirs resizeDirection;
    public bool ignoreLayout;
    [SerializeField]
    [HideInInspector]
    RectTransform dragRect;
    [SerializeField]
    [HideInInspector]
    Image image;
    bool isDragging;

    RectTransform rect;
    public void setHeaderHeight(float f)
    {
        if (ignoreLayout) return;
        headerHeight = f;
        if (rect == null) rect = GetComponent<RectTransform>();
        if (resizeDirection == ResizeDirs.Pan)
        {
            rect.sizeDelta = new Vector2(2 * borderWidth, headerHeight );//+ borderWidth
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(0, borderWidth);
        }

    }
    [SerializeField]
    [HideInInspector]
    float borderWidth = 6;
    [SerializeField]
    [HideInInspector]

    float headerHeight = 20;
    public void setBorderWidth(float f)
    {
        if (ignoreLayout) return;
        borderWidth = f;
        if (rect == null) rect = GetComponent<RectTransform>();

        switch (resizeDirection)
        {
            case ResizeDirs.HorizontalLeft:
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(1, .5f);
                rect.sizeDelta = new Vector2(f, 2 * f);
                break;
            case ResizeDirs.HorizontalRight:
                rect.anchorMin = new Vector2(1, 0);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(0, .5f);
                rect.sizeDelta = new Vector2(f, 2 * f);
                break;
            case ResizeDirs.VerticalDown:
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(0.5f, 1);
                rect.sizeDelta = new Vector2(0, f);
                break;
            case ResizeDirs.VerticalUp:
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(.5f, 0);
                rect.sizeDelta = new Vector2(0, f);
                break;

            case ResizeDirs.DownLeft:
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0, 0);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(2 * f, 2 * f);
                break;

            case ResizeDirs.DownRight:
                rect.anchorMin = new Vector2(1, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(2 * f, 2 * f);
                break;
        }
        rect.anchoredPosition = new Vector2(0, 0);
        if (resizeDirection != ResizeDirs.DownRight && resizeDirection != ResizeDirs.DownLeft)
        if (draggable == null)
            draggable = GetComponentInParent<zDraggable>();
        if (image==null) image=GetComponent<Image>();
        image.color = draggable.neutralColor;
    }


    void OnValidate()
    {
        if (draggable == null)
            draggable = GetComponentInParent<zDraggable>();
        if (dragRect == null) dragRect = draggable.GetComponent<RectTransform>();
        if (image == null) image = GetComponent<Image>();
        switch (resizeDirection)
        {
            case ResizeDirs.HorizontalLeft:
            case ResizeDirs.HorizontalRight:
                hoverCursor = zDraggable.horizontalCursor;
                break;
            case ResizeDirs.VerticalDown:
            case ResizeDirs.VerticalUp:
                hoverCursor = zDraggable.vertialCursor;
                break;
            case ResizeDirs.UpLeft:
            case ResizeDirs.DownRight:
                hoverCursor = zDraggable.upLeftResizeCursor;
                break;
            case ResizeDirs.UpRight:
            case ResizeDirs.DownLeft:

                hoverCursor = zDraggable.upRightResizeCursor;
                break;
            case ResizeDirs.Pan:

                hoverCursor = zDraggable.moveCursor;
                break;
        }
        if (resizeDirection != ResizeDirs.DownRight && resizeDirection != ResizeDirs.DownLeft)
            image.color = draggable.neutralColor;
        else image.color = new Color(0, 0, 0, 0);
        setBorderWidth(borderWidth);
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (resizeDirection==ResizeDirs.None) return;

        if (resizeDirection != ResizeDirs.DownRight && resizeDirection != ResizeDirs.DownLeft)
            image.color = draggable.hoverColor;
        else image.color = new Color(0, 0, 0, 0);

        Cursor.SetCursor(hoverCursor, new Vector2(16, 16), CursorMode.Auto);
    }
    public void OnPointerExit(PointerEventData e)
    {
       if (isDragging) return;
        if (resizeDirection != ResizeDirs.DownRight && resizeDirection != ResizeDirs.DownLeft)
            image.color = draggable.neutralColor;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    Vector3 lastCursorPosition;
    Vector3 startPosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        if (resizeDirection == ResizeDirs.HorizontalRight || resizeDirection == ResizeDirs.UpLeft) { draggable.setHorizontalPivot(zDraggable.AnchorModes.min); }
        if (resizeDirection == ResizeDirs.HorizontalLeft || resizeDirection == ResizeDirs.UpRight) { draggable.setHorizontalPivot(zDraggable.AnchorModes.max); }
        if (resizeDirection == ResizeDirs.VerticalDown) { draggable.setVerticalPivot(zDraggable.AnchorModes.max); }
        if (resizeDirection == ResizeDirs.VerticalUp) { draggable.setVerticalPivot(zDraggable.AnchorModes.min); }
        if (resizeDirection == ResizeDirs.UpRight || resizeDirection == ResizeDirs.UpLeft) { draggable.setVerticalPivot(zDraggable.AnchorModes.max); }
        if (resizeDirection == ResizeDirs.DownRight)
        {
            draggable.setHorizontalPivot(zDraggable.AnchorModes.min);
            draggable.setVerticalPivot(zDraggable.AnchorModes.max);
        }
        if (resizeDirection == ResizeDirs.DownLeft)
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

        Vector3 drag = Input.mousePosition - lastCursorPosition;
        if (resizeDirection == ResizeDirs.Pan)
        {
            draggable.setPosition(startPosition + drag);
            return;
        }
        else if (resizeDirection == ResizeDirs.VerticalUp)
            draggable.setDimensions(new Vector2(dragRect.sizeDelta.x, dragRect.sizeDelta.y + drag.y));
        else if (resizeDirection == ResizeDirs.VerticalDown)
            draggable.setDimensions(new Vector2(dragRect.sizeDelta.x, dragRect.sizeDelta.y - drag.y));
        else if (resizeDirection == ResizeDirs.HorizontalLeft)
            draggable.setDimensions(new Vector2(dragRect.sizeDelta.x - drag.x, dragRect.sizeDelta.y));
        else if (resizeDirection == ResizeDirs.HorizontalRight)
            draggable.setDimensions(new Vector2(dragRect.sizeDelta.x + drag.x, dragRect.sizeDelta.y));
        else if (resizeDirection == ResizeDirs.UpLeft)
            draggable.setDimensions(new Vector2(dragRect.sizeDelta.x + drag.x, dragRect.sizeDelta.y - drag.y));
        else if (resizeDirection == ResizeDirs.UpRight)
            draggable.setDimensions(new Vector2(dragRect.sizeDelta.x - drag.x, dragRect.sizeDelta.y - drag.y));
        if (resizeDirection == ResizeDirs.DownLeft)
            draggable.setDimensions(new Vector2(dragRect.sizeDelta.x - drag.x, dragRect.sizeDelta.y - drag.y));
        else if (resizeDirection == ResizeDirs.DownRight)
            draggable.setDimensions(new Vector2(dragRect.sizeDelta.x + drag.x, dragRect.sizeDelta.y - drag.y));

        lastCursorPosition = Input.mousePosition;
 
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        draggable.OnDragOperationEnded();
        isDragging = false;
    
         if (resizeDirection != ResizeDirs.DownRight && resizeDirection == ResizeDirs.DownRight)
                image.color = draggable.neutralColor;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
  

}
