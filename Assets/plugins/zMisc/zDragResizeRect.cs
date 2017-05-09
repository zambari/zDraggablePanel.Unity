
//zzambari : stereoko 2017

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[ExecuteInEditMode]
public class zDragResizeRect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    [SerializeField]

    public float borderWidth = 6;
    [SerializeField]

    public bool hasColorProvider;
    public float headerHeight = 20;

    [SerializeField]
    [HideInInspector]
    protected Texture2D hoverCursor;
    protected IProvideColors colorProvider;
    [SerializeField]
    [HideInInspector]
    public enum Borders { Drag, L, T, R, B, TL, TR, BR, BL, None, Disabled }
    public Borders resizeDirection;



    public RectTransform targetRect;
    [SerializeField]
    [HideInInspector]
    protected Image image;
    protected bool isDragging;
    protected RectTransform rect;
    protected virtual void Awake()
    {
        checkReferences();
    }
    void checkReferences()
    {
        if (rect == null) rect = GetComponent<RectTransform>();
        if (rect == null) rect = gameObject.AddComponent<RectTransform>();
        if (targetRect == null) targetRect = GetComponentInParent<RectTransform>();
        if (image == null) image = GetComponent<Image>();
        if (image == null) image = gameObject.AddComponent<Image>();
        if (colorProvider == null) colorProvider = GetComponentInParent<IProvideColors>();
        if (colorProvider != null)
        {

            var a = colorProvider.getColorsChangedAction();
            a -= colorChanged;
            a += colorChanged;

        }
        hasColorProvider = (colorProvider == null);
        colorChanged();
    }
    public void setTargetRect(RectTransform target)
    {
        targetRect = target;
    }
    protected void highlight()
    {
        if (colorProvider != null) image.color = colorProvider.getHoveredColor();
    }
    void colorChanged()
    {
        if (colorProvider != null)
            image.color = colorProvider.getNormalColor();
   
    }
    protected void restore()
    {
        if (colorProvider != null) image.color = colorProvider.getNormalColor();
    }
    public void setImage(Sprite newSprite)
    {
        if (image == null) image = GetComponent<Image>();
        image.sprite = newSprite;
    }
    public virtual void showHover(Borders b)
    {
        if (b == resizeDirection)
        { highlight(); return; }
        switch (b)
        {
            //     case Borders.Drag: if (draggable.transparentHeader) image.color=new Color(0,0,0,0); return;
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
    void setAnchors()

    {
        if (rect == null) rect = GetComponent<RectTransform>();
        switch (resizeDirection)
        {
            case Borders.L:
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(1, 0);
                // rect. setAnchorsY(0,1);
                break;
            case Borders.R:
                rect.anchorMin = new Vector2(1, 0);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(0, 0);
                break;
            case Borders.B:
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(0.5f, 1);
                break;
            case Borders.BR:
                rect.anchorMin = new Vector2(1, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(0f, 1f);
                break;
            case Borders.BL:
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0, 0);
                rect.pivot = new Vector2(1f, 1f);
                break;
            case Borders.TL:
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(1, 0);
                break;
            case Borders.TR:
                rect.anchorMin = new Vector2(1, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(0f, 0f);
                break;
            case Borders.T:
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(.5f, 0);
                //  rect.anchoredPosition = new Vector2(0, headerHeight);
                break;
            case Borders.Drag:
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(0.5f, 0);
                break;
        }
        if (resizeDirection == Borders.Drag)
            rect.anchoredPosition = new Vector2(0, headerHeight);
        else
            rect.anchoredPosition = new Vector2(0, 0);
    }
    public void setDirection(Borders newDir)
    {
        resizeDirection = newDir;
        setAnchors();
    }
    public void setBorderWidth(float newBorderWidth, float newHeaderHeight)
    {
        checkReferences();
        borderWidth = newBorderWidth;
        headerHeight = newHeaderHeight;
        if (rect == null) rect = GetComponent<RectTransform>();
        if (image == null) image = GetComponent<Image>();
        switch (resizeDirection)
        {
            case Borders.L:
                //rect.sizeDelta = new Vector2(borderWidth, headerHeight);
                rect.sizeDelta = new Vector2(borderWidth, 0);
                //   rect.sizeDelta = new Vector2(borderWidth, targetRect.sizeDelta.y);
                break;
            case Borders.R:
                rect.sizeDelta = new Vector2(borderWidth, 0);
                break;
            case Borders.B:
                rect.sizeDelta = new Vector2(0, borderWidth);
                break;
            case Borders.BR:
                rect.sizeDelta = new Vector2(borderWidth, borderWidth);
                break;
            case Borders.BL:
                rect.sizeDelta = new Vector2(borderWidth, borderWidth);
                break;
            case Borders.TL:
                rect.sizeDelta = new Vector2(borderWidth, borderWidth);
                break;
            case Borders.TR:
                rect.sizeDelta = new Vector2(borderWidth, borderWidth);
                break;

            case Borders.T:
                rect.sizeDelta = new Vector2(0, borderWidth); // /2 makes the top line thinner
                break;

            case Borders.Drag:
                rect.sizeDelta = new Vector2(2 * borderWidth, headerHeight);
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, borderWidth);
                break;
        }
        if (resizeDirection != Borders.BR && resizeDirection != Borders.BL)
            if (image == null) image = GetComponent<Image>();

        restore();

    }


    protected virtual void OnValidate()
    {
        if (!gameObject.activeInHierarchy) return;
        checkReferences();

        switch (resizeDirection)
        {
            case Borders.L:
            case Borders.R:
                hoverCursor = zResourceLoader.horizontalCursor;
                break;
            case Borders.B:
            case Borders.T:
                hoverCursor = zResourceLoader.vertialCursor;
                break;
            case Borders.BL:
            case Borders.TR:
                hoverCursor = zResourceLoader.upRightResizeCursor;
                break;
            case Borders.TL:
            case Borders.BR:
                hoverCursor = zResourceLoader.upLeftResizeCursor;
                break;
            case Borders.Drag:

                hoverCursor = zResourceLoader.moveCursor;
                break;
        }
        name = "Drag_" + resizeDirection.ToString();
        showHover(Borders.None);
        setBorderWidth(borderWidth, headerHeight);
        setAnchors();
        LayoutElement l = GetComponent<LayoutElement>();
        if (l == null) l = gameObject.AddComponent<LayoutElement>();
        l.ignoreLayout = true;
    }

    public virtual void OnPointerEnter(PointerEventData e)
    {
        if (resizeDirection == Borders.None) return;
        highlight();
        Cursor.SetCursor(hoverCursor, new Vector2(16, 16), CursorMode.Auto);
    }
    public virtual void OnPointerExit(PointerEventData e)
    {
        if (isDragging) return;
        restore();
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    Vector3 lastCursorPosition;
    Vector3 startPosition;

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        if ((resizeDirection == Borders.B) ||
        (resizeDirection == Borders.BL) ||
        (resizeDirection == Borders.BR))

            targetRect.setPivotX(1);
        if ((resizeDirection == Borders.T) ||
        (resizeDirection == Borders.TL) ||
        (resizeDirection == Borders.TR))

            targetRect.setPivotY(0);
        if ((resizeDirection == Borders.R) ||
        (resizeDirection == Borders.BR) ||
        (resizeDirection == Borders.TR))

            targetRect.setPivotX(0);
        if ((resizeDirection == Borders.L) ||
        (resizeDirection == Borders.TL) ||
        (resizeDirection == Borders.BL))

            targetRect.setPivotY(1);

        startPosition = targetRect.localPosition;

        lastCursorPosition = Input.mousePosition;
    }
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width ||
            Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height) return;
        // draggable.OnDragOperation();     
        Vector3 drag = Input.mousePosition - lastCursorPosition;
        if (resizeDirection == Borders.Drag)
        {
            targetRect.localPosition = startPosition + drag;
            //  draggable.setPosition(startPosition + drag);
            return;
        }
        else if (resizeDirection == Borders.T) targetRect.setSizeY(targetRect.sizeDelta.y + drag.y);
        else if (resizeDirection == Borders.B) targetRect.setSizeY(targetRect.sizeDelta.y - drag.y);
        else if (resizeDirection == Borders.L) targetRect.setSizeX(targetRect.sizeDelta.x - drag.x);
        else if (resizeDirection == Borders.R) targetRect.setSizeX(targetRect.sizeDelta.x + drag.x);
        else if (resizeDirection == Borders.TL)


            targetRect.setSizeXY(targetRect.sizeDelta.x - drag.x, targetRect.sizeDelta.y + drag.y);
        else if (resizeDirection == Borders.TR)
            targetRect.setSizeXY(targetRect.sizeDelta.x + drag.x, targetRect.sizeDelta.y + drag.y);
        if (resizeDirection == Borders.BL)
            targetRect.setSizeXY(targetRect.sizeDelta.x - drag.x, targetRect.sizeDelta.y - drag.y);
        else if (resizeDirection == Borders.BR)
            targetRect.setSizeXY(targetRect.sizeDelta.x + drag.x, targetRect.sizeDelta.y - drag.y);

        lastCursorPosition = Input.mousePosition;

    }
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        // draggable.OnDragOperationEnded();
        isDragging = false;

        //    if (resizeDirection != Borders.BR && resizeDirection == Borders.BR)
        //      image.color = draggable.neutralColor;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }


}
