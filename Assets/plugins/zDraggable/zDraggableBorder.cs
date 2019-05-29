//z2k17

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class zDraggableBorder : zDragResizeRect
{
    [SerializeField]

    zDraggable draggable;
    protected override void OnValidate()
    {
        if (draggable == null) draggable = GetComponentInParent<zDraggable>();
        base.OnValidate();
        if (draggable != null) targetRect = draggable.GetComponent<RectTransform>();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        draggable.OnDragOperation();
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        draggable.OnDragOperationStart();
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        draggable.OnDragOperationEnded();
    }


    public override void OnPointerEnter(PointerEventData e)
    {
        if (resizeDirection == Borders.None) return;
        if (draggable != null)
            draggable.setHoverState(resizeDirection);
        else
            highlight();
        Cursor.SetCursor(hoverCursor, new Vector2(16, 16), CursorMode.Auto);
    }
    public override void OnPointerExit(PointerEventData e)
    {
        if (isDragging) return;
        if (draggable != null) draggable.setHoverState(Borders.None);
        else
            restore();
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

}
