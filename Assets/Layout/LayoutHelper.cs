using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Z.DragRect
{
    [RequireComponent(typeof(Image))]
    [ExecuteInEditMode]
    public class LayoutHelper : MonoRect, IBeginDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler // ISyncColors, 
    {

        public LayoutElement layoutElement;
        LayoutElement _myLayoutElement;
        LayoutElement myLayoutElement
        {
            get
            {
                if (_myLayoutElement == null)
                    _myLayoutElement = GetComponent<LayoutElement>();
                if (_myLayoutElement == null)
                    _myLayoutElement = gameObject.AddComponent<LayoutElement>();
                return _myLayoutElement;
            }
        }
        public VerticalLayoutGroup verticalGroup;
        public HorizontalLayoutGroup horizontalGroup;
        public bool horizontal;
        public bool vertical;
        HoverableColors colors = new HoverableColors();

        [HideInInspector]
        public int id;
        public int index;
        LayoutGroupHelper groupHelper;
        public float size = 10;
        void Reset()
        {
            // getSibling();
            id = Random.Range(0, System.Int32.MaxValue);
            LayoutElement le = GetComponent<LayoutElement>();
            if (le == null) le = gameObject.AddComponent<LayoutElement>();
        }
        public void SetVisible(bool b)
        {
            image.enabled = b;
        }
        void OnGroupChange()
        {
            index = FindIndex();
            SetSize(groupHelper.spacing);
            name = (horizontal ? "H" : "V") + "-" + index + "-to-" + (index + 1) + "-";
        }
        void OnEnable()
        {
            if (groupHelper == null) GetGroup();
            if (image == null) image = GetComponent<Image>();

            if (groupHelper != null)
            {
                groupHelper.onGroupChange += OnGroupChange;
                groupHelper.OnElementEnabled(this);
                colors = groupHelper.colors;
                if (image != null) image.color = colors.baseColor;

            }
            transform.SetAsLastSibling();
        }
        void OnDisable()
        {
            if (groupHelper != null)
            {
                groupHelper.onGroupChange -= OnGroupChange;
                groupHelper.OnElementDisabled(this);
            }
            else Debug.Log("no group helper");
        }
        protected void OnValidate()
        {
            if (id == 0)
                id = Random.Range(0, System.Int32.MaxValue);
            if (image == null)
                image.color = colors.normalColor;

            layoutElement = transform.parent.GetComponent<LayoutElement>();
            if (transform.parent == null) Debug.Log("no parent");
            SetSize(size);
        }
        void GetGroup()
        {
            if (groupHelper == null)
                groupHelper = transform.parent.parent.GetComponent<LayoutGroupHelper>();
            if (groupHelper == null)
                groupHelper = GetComponentInParent<LayoutGroupHelper>();
        }
        void CheckDirection()
        {
            {
                verticalGroup = transform.parent.parent.GetComponent<VerticalLayoutGroup>();
                horizontalGroup = transform.parent.parent.GetComponent<HorizontalLayoutGroup>();
                vertical = (verticalGroup != null);
                horizontal = (horizontalGroup != null);
            }
        }


        public void SetSize(float f)
        {
            CheckDirection();

            size = f;
            if (vertical) rect.sizeDelta = new Vector2(0, f);
            else rect.sizeDelta = new Vector2(f, 0);
            if (vertical)
            {
                myLayoutElement.flexibleWidth = 1;
                myLayoutElement.flexibleHeight = -1;
                myLayoutElement.minHeight = size;
            }
            else
            {
                myLayoutElement.flexibleWidth = -1;
                myLayoutElement.flexibleHeight = 1;
                myLayoutElement.minWidth = size;
            }

            if (vertical)
            {

                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(0.5f, 0);
                rect.anchoredPosition = new Vector2(0, -f);///2
                //child.sizeDelta=new Vector2(13,15);
                //    child.offsetMin = new Vector2(ofs2, 0);
                //    child.offsetMax = new Vector2(ofs, 0);
            }
            else
            {
                rect.anchorMin = new Vector2(1, 0);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(0, .5f);
                rect.anchoredPosition = new Vector2(0, 0); //f2
                                                           //   child.offsetMin = new Vector2(ofs2, 0);
                                                           //         child.offsetMax = new Vector2(0, ofs);

            }


        }

        public void OnBeginDrag(PointerEventData e)
        {
            image.color = colors.activeColor;
            groupHelper.NormalizeFlexibles();
        }
        public void OnEndDrag(PointerEventData e)
        {
            image.color = colors.baseColor;
            groupHelper.NormalizeFlexibles();
        }

        public void OnDrag(PointerEventData e)
        {
            groupHelper.AdjustFlex(index, horizontal ? e.delta.x / Screen.width : e.delta.y / Screen.height);
        }

        public void OnPointerEnter(PointerEventData e)
        {
            image.color = colors.hoveredColor;
        }
        public void OnPointerExit(PointerEventData e)
        {
            image.color = colors.baseColor;
        }
        int FindIndex()
        {
            for (int i = 0; i < groupHelper.elements.Length; i++)
            {
                if (groupHelper.elements[i].transform == transform.parent)
                {

                    index = i;
                    //    image.enabled=(i!=groupHelper.elements.Length-1) ;
                    return i;
                }
            }
            return -1;
        }
        void Start()
        {
            GetGroup();
            OnGroupChange();
        }
        public LayoutData GetData()
        {
            var le = transform.parent.GetComponent<LayoutElement>();
            if (le == null) return null;
            return new LayoutData()
            {
                id = this.id,
                minH = le.minHeight,
                minW = le.minWidth,
                prefH = le.preferredHeight,
                prefW = le.preferredWidth,
                flexH = le.flexibleHeight,
                flexW = le.flexibleWidth
            };
        }
        public void SetData(LayoutData data)
        {
            var le = transform.parent.GetComponent<LayoutElement>();
            if (le == null)
            {
                Debug.Log("no layoutelement");
            }
            else
            {
                le.minHeight = data.minH;
                le.minWidth = data.minW;
                le.preferredHeight = data.prefH;
                le.preferredWidth = data.prefH;
                le.flexibleHeight = data.flexH;
                le.flexibleWidth = data.flexW;
            }
        }

    }
}