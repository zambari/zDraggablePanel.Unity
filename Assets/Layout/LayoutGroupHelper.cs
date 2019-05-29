using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Z;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Z.DragRect
{
    [ExecuteInEditMode]
    public class LayoutGroupHelper : MonoRect
    {

        //public LayoutElement[] childElements;
        HorizontalLayoutGroup horizontalGroup;
        VerticalLayoutGroup vericalGroup;
        public bool isHorizontal;
        public bool isVertical;
        [HideInInspector] public LayoutElement[] elements;
        public float spacing = 10;
        public HoverableColors colors;
        public bool hideHelpers;
        [SerializeField] [HideInInspector] bool lastHideHelpers;
        List<LayoutHelper> currentHelpers = new List<LayoutHelper>();
        [ReadOnly]
        public bool isSlave;
        public void OnValidate()
        {
            if (spacing < 0) spacing = 0;
            GetLayouts();
            if (onGroupChange != null) onGroupChange.Invoke();
            HandleObjectHiding();
            HandleChildGroups();
        }

        void HandleChildGroups()
        {
            var childGroups = GetComponentsInChildren<LayoutGroupHelper>();
            for (int i = 0; i < childGroups.Length; i++)
            {
                if (childGroups[i] != this)
                {
                    childGroups[i].hideHelpers = hideHelpers;
                    childGroups[i].spacing = spacing;
                    childGroups[i].colors = colors;
                    childGroups[i].isSlave = true;
                    childGroups[i].OnValidate();
                }
            }
        }
        void RemoveNulls()
        {
            for (int i = currentHelpers.Count - 1; i >= 0; i--)
            {
                if (currentHelpers[i] == null)
                {
                    Debug.Log("null at inex" + i);
                    currentHelpers.RemoveAt(i);
                }
            }
        }
        public void HandleObjectHiding()
        {
            RemoveNulls();
#if UNITY_EDITOR
            if (hideHelpers != lastHideHelpers)
            {

                for (int i = 0; i < currentHelpers.Count; i++)
                {
                    Undo.RegisterCompleteObjectUndo(currentHelpers[i].gameObject, "hide flags");
                    currentHelpers[i].gameObject.hideFlags = hideHelpers ? HideFlags.HideInHierarchy : HideFlags.None;
                }
                EditorApplication.RepaintHierarchyWindow();
                lastHideHelpers = hideHelpers;
            }
#endif
        }
        void GetElements()
        {
            elements = gameObject.GetActiveLayoutElements();
        }
        public System.Action onGroupChange;
        void GetLayouts()

        {
            horizontalGroup = GetComponent<HorizontalLayoutGroup>();
            vericalGroup = GetComponent<VerticalLayoutGroup>();
            isHorizontal = (horizontalGroup != null);
            isVertical = (vericalGroup != null);
            if (!isVertical && !isHorizontal) Debug.LogWarning("no layout group here", gameObject);
            else
            {
                if (horizontalGroup != null) horizontalGroup.spacing = spacing;
                if (vericalGroup != null) vericalGroup.spacing = spacing;
            }
        }
        [ExposeMethodInEditor]
        void RemoveHelpers()
        {
            LayoutHelper[] helpers = GetComponentsInChildren<LayoutHelper>();
            for (int i = 0; i < helpers.Length; i++)
            {
                Debug.Log("destroing " + helpers[i].name);
                DestroyImmediate(helpers[i].gameObject);

            }

        }
        [ExposeMethodInEditor]
        void CreateHelpers()
        {
            GetElements();
            var helpers = new LayoutHelper[elements.Length];
            for (int i = 0; i < elements.Length; i++)
            {
                helpers[i] = CreateHelper(elements[i].gameObject.transform);
            }
            HideLast();
            if (onGroupChange != null) onGroupChange.Invoke();
        }
        public void OnElementEnabled(LayoutHelper source)
        {
            //    currentHelpers.Add(source);
            GetElements();
            HideLast();
            if (onGroupChange != null) onGroupChange.Invoke();
        }
        public void OnElementDisabled(LayoutHelper source)
        {
            if (currentHelpers.Contains(source)) currentHelpers.Remove(source);
            if (onGroupChange != null) onGroupChange.Invoke();
            HideLast();
        }
        void HideLast()
        {
            GetElements();
            for (int i = elements.Length - 1; i > 1; i--)
            {
                bool nextActive = true;
                if (i == elements.Length - 1 || !elements[i + 1].gameObject.activeSelf) nextActive = false;

                bool thisActive = elements[i].gameObject.activeSelf;
                bool shouldbevisible = !(thisActive && !nextActive);
                var helper = elements[i].GetComponentInChildren<LayoutHelper>();
                if (helper != null)
                    helper.SetVisible(shouldbevisible);
                //          Debug.Log(" elements "+i+" is activ e"+thisActive+" next  "+nextActive+" outcome "+shouldbevisible);
            }
        }
        [ExposeMethodInEditor]
        public void NormalizeFlexibles()
        {
            GetElements();
            //        Debug.Log("normalizing flex " + elements.Length);
            NormalizeFlex(elements, isVertical);
        }
        [ExposeMethodInEditor]
        public void ResetFlexibles()
        {
            GetElements();
            ResetFlexibles(elements, isVertical);
        }
        void Awake()
        {
            GetElements();
        }

        public static void NormalizeFlex(LayoutElement[] list, bool vertical = true)
        {
            float flexSum = 0;
            for (int i = 0; i < list.Length; i++)
            {
                if (vertical)
                {
                    if (list[i].flexibleHeight != -1)
                        flexSum += list[i].flexibleHeight;
                }
                else
                {
                    if (list[i].flexibleWidth != -1)
                        flexSum += list[i].flexibleWidth;
                }
            }
            for (int i = 0; i < list.Length; i++)
            {
                if (vertical)
                {
                    if (list[i].flexibleHeight != -1)
                        list[i].flexibleHeight = list[i].flexibleHeight / flexSum;
                }
                else
                {
                    if (list[i].flexibleWidth != -1)
                        list[i].flexibleWidth = list[i].flexibleWidth / flexSum;
                }
            }
        }
        public static void ResetFlexibles(LayoutElement[] list, bool vertical = true)
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (vertical)

                    list[i].flexibleHeight = 1f / list.Length;
                else
                    list[i].flexibleWidth = 1f / list.Length;
            }
        }
        public void AdjustFlex(int index, float offset)
        {
            if (index > elements.Length || index < 0)
            {
                Debug.Log("invalid index");
                return;
            }
            LayoutElement thisLayout = elements[index];
            LayoutElement nextLayout = elements[index + 1];
            if (isVertical)
            {
                if (thisLayout.flexibleHeight != -1)
                {
                    thisLayout.flexibleHeight -= offset;
                    if (nextLayout.flexibleHeight != -1)
                        nextLayout.flexibleHeight += offset;
                }
            }
            else
            {
                if (thisLayout.flexibleWidth != -1)
                    thisLayout.flexibleWidth += offset;
                if (nextLayout.flexibleWidth != -1)
                    nextLayout.flexibleWidth -= offset;
            }

        }

        LayoutHelper CreateHelper(Transform target)
        {
            RectTransform child = target.gameObject.AddChild();
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(child.gameObject, "helper");
#endif
            return child.gameObject.AddComponent<LayoutHelper>();

        }


    }
}