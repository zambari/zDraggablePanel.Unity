using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Z;
namespace Z.DragRect
{

    [ExecuteInEditMode]
    [RequireComponent(typeof(Image))]
    public class zDragBorder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public enum Borders { Top, L, R, B, BR, BL, None }
        public static bool HideWhenFolded(Borders b)
        {
            switch (b)
            {
                case Borders.Top:
                    return false;
                default:
                    return true;
            }
        }
        public Borders borderType;
        Image image;
        [SerializeField]
        zDragPanel dragPanel;
        RectTransform targetRect;

        public void SetHidden(bool hidden)
        {
            image.enabled = !hidden;
        }

        void GetCursor()
        {
            switch (borderType)
            {
                case Borders.L:
                case Borders.R:
                    hoverCursor = zResourceLoader.horizontalCursor;
                    break;
                case Borders.B:
                    //     case Borders.T:
                    hoverCursor = zResourceLoader.vertialCursor;
                    break;
                case Borders.BL:
                    //   case Borders.TR:
                    hoverCursor = zResourceLoader.upRightResizeCursor;
                    break;
                //         case Borders.TL:
                case Borders.BR:
                    hoverCursor = zResourceLoader.upLeftResizeCursor;
                    break;
                case Borders.Top:
                    hoverCursor = zResourceLoader.moveCursor;
                    break;
            }
        }
        void Start()
        {

            GetCursor();
        }
        void OnValidate()
        {
            if (dragPanel == null) dragPanel = GetComponentInParent<zDragPanel>();
        }
        void Reset()
        {
            if (image == null) image = gameObject.AddComponent<Image>();
            dragPanel = GetComponentInParent<zDragPanel>();
        }
        #region drag

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            dragPanel.HandleEndDrag(this);
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            dragPanel.HandleBeginDrag(this);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            dragPanel.HandleDrag(this, eventData.delta);
        }

        #endregion drag

        #region hoversAndBoilerplate
        [SerializeField]
        [HideInInspector]
        protected Texture2D hoverCursor;
        RectTransform _rect;
        public RectTransform rect { get { if (_rect == null) _rect = GetComponent<RectTransform>(); return _rect; } }

        public void OnPointerEnter(PointerEventData e)
        {
            if (borderType == Borders.None) return;
            if (dragPanel != null)
                dragPanel.SetHoverState(borderType);
            else
                Highlight();
            Cursor.SetCursor(hoverCursor, new Vector2(16, 16), CursorMode.Auto);
        }
        public void OnPointerExit(PointerEventData e)
        {
            //    if (isDragging) return;
            if (dragPanel != null) dragPanel.SetHoverState(Borders.None);
            else
                UnHighlight();
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        void OnEnable()
        {
            if (dragPanel == null) dragPanel = GetComponentInParent<zDragPanel>();
            if (dragPanel == null) { Debug.Log("no panel"); return; }
            if (targetRect == null) targetRect = dragPanel.GetComponent<RectTransform>();
            if (image == null) image = GetComponent<Image>();
            dragPanel.RegisterBorder(this);
            UnHighlight();

        }
        void OnDisable()
        {
            if (dragPanel == null) dragPanel = GetComponentInParent<zDragPanel>();
            dragPanel.UnRegisterBorder(this);
        }

        void Highlight()
        {
            if (image != null)
                image.color = dragPanel.colors.hoverColor;
        }

        void UnHighlight()
        {
            if (image != null)
                image.color = dragPanel.colors.normalColor;
        }

        public void ShowHover(Borders b)
        {
            if (b == borderType)
            { Highlight(); return; }
            switch (b)
            {
                //     case Borders.Drag: if (draggable.transparentHeader) image.color=new Color(0,0,0,0); return;
                case Borders.None: UnHighlight(); return;

                case Borders.BL:
                    if (borderType == Borders.B || borderType == Borders.L) Highlight(); else UnHighlight();
                    return;
                case Borders.BR:
                    if (borderType == Borders.B || borderType == Borders.R) Highlight(); else UnHighlight();
                    return;
            }
            UnHighlight();
        }
        #endregion hoversAndBoilerplate
        #region corners
        void SetAnchors()
        {
            switch (borderType)
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
                /*    case Borders.TL:
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
                         break; */
                case Borders.Top:
                    rect.anchorMin = new Vector2(0, 1);
                    rect.anchorMax = new Vector2(1, 1);
                    rect.pivot = new Vector2(0.5f, 0);
                    break;
            }
            /*   if (borderType == Borders.Drag)
                   rect.anchoredPosition = new Vector2(0, headerHeight);
               else
                   rect.anchoredPosition = new Vector2(0, 0);*/
        }
        public void SetDirection(Borders newDir)
        {
            borderType = newDir;
            SetAnchors();
        }
        #endregion corners
    }
}