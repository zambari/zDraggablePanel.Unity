
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

[ExecuteInEditMode]

public class zDraggable : MonoBehaviour,IProvideColors
{
    public bool refresh = true;
    [Header("Config")]
    [Range(1, 16)]
    public float borderWidth = 6;
    [Range(1, 16)]
    public float headerHeight = 12;
    public bool stretchSidesToTop = true;
    public bool useCorners = true;
    public bool useLineGraphics = false;
    //protected static Color _hoverColor;

    public Action folding; // not used in this version
    public Action unFolding; // not used in this version
    public Action folded; // not used in this version
    public bool transparentHeader;
    public bool loadFromPreferencesOnStart;
    [HideInInspector]
    public Vector3[] corners;
    [SerializeField]
    [HideInInspector]
    Image image;
    [SerializeField]
    [HideInInspector]


    zDraggableMenuController menuController;
    public enum AnchorModes { min, max, stretch };//,noSnap 
    public AnchorModes anchorHorizontal;
    public AnchorModes anchorVertical;
    public Vector2 minimalSize = new Vector2(100, 30);


    [SerializeField]
   // [HideInInspector]
    public RectTransform rect;
    const float minWidth = 60;

    [SerializeField]
    [HideInInspector]

    Transform textTransform;
    TimeRamp timeRamp;
    [SerializeField]
    [HideInInspector]
    LayoutElement layoutElement;
    Vector2 savedSize;
    //    Vector2 foldedSize;
    bool isMinimized;
    bool isMaxed;
//    Vector2 savedMax;
  //  Vector2 savedMin;

    [SerializeField]

    [HideInInspector]
    GameObject frame;
    [SerializeField]
    [HideInInspector]

    RectTransform[] borderRects;
    [SerializeField]
    [HideInInspector]
    zDraggableBorder[] borders;

    public zDraggableBorder.Borders hoverState;

  
    public static List<zDraggable> draggableList;
    [SerializeField]
    protected static GameObject draggableMenu;
    // [SerializeField]
    // protected static GameObject draggableLabel;
    public static Action<zDraggable> newPanel;
    #region colors
    [Header ("Color Provide")]
       Action colorsChanged;

    public Color normalColor = new Color(1, 1, 1, 0.2f);
    public Color hoveredColor =new  Color(1, .5f, .1f, 0.2f);

   
    public Action getColorsChangedAction()    {  return colorsChanged;  }
    public Color getNormalColor()    {   return normalColor;   }
    public Color getHoveredColor()    {  return hoveredColor;   }
    public Color getActiveColor()    {    return hoveredColor;  }
    public Color getdisabledColor()    {    return normalColor;   }
    #endregion colors
    public void SetHoverState(zDraggableBorder.Borders h)
    {
        hoverState = h;
        if (!isDragging)
        {
            for (int i = 0; i < borders.Length; i++)
            {
                if (borders[i] != null)
                    borders[i].showHover(h);
            }
        }
    }
    bool isMinimizing;
    Image bgImage;
    RawImage bgImageRaw;
    Color startColor;
    public void SetOpacity(float f)
    {
        if (bgImageRaw != null)
            bgImageRaw.color = new Color(startColor.r, startColor.g, startColor.b, f);
        else
           if (bgImage != null)
            bgImage.color = new Color(startColor.r, startColor.g, startColor.b, f);
    }

    void OnValidate()
    {
        if (image == null) image = GetComponent<Image>();

        if (rect == null) rect = GetComponent<RectTransform>();
        if (!enabled) return;
        if (!gameObject.activeInHierarchy) return;
        SetHorizontalPivot(anchorHorizontal);
        SetVerticalPivot(anchorVertical);
        if (refresh) refresh = false;
        if (!gameObject.activeInHierarchy || !enabled) return;
        isDragging = false;
        // if (headerHeight < borderWidth) headerHeight = borderWidth;

        SetAnchorAndPivotBasedOnAnchoring();

        createBorders();

        if (layoutElement == null) layoutElement = GetComponent<LayoutElement>();
        if (layoutElement == null) layoutElement = gameObject.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = rect.rect.width;
        layoutElement.preferredWidth = rect.rect.height;


        if (menuController == null) menuController = GetComponentInChildren<zDraggableMenuController>();
        if (menuController != null) menuController.SetHeight(headerHeight);
        if (colorsChanged != null) colorsChanged();
    }

    public void saveAllLocation()
    {
        for (int i = 0; i < draggableList.Count; i++)
            draggableList[i].saveLocation();
    }
    public void loadAllLocation()
    {
        for (int i = 0; i < draggableList.Count; i++)
            draggableList[i].loadLocation();
    }
    public void saveLocation()
    {
        SetHorizontalPivot(AnchorModes.min);
        SetVerticalPivot(AnchorModes.max);
        PlayerPrefsX.SetVector3(name + "position", rect.position);
        PlayerPrefsX.SetVector3(name + "size", rect.sizeDelta);
        PlayerPrefsX.SetBool(name + "horiz", (anchorHorizontal == AnchorModes.max));
        PlayerPrefsX.SetBool(name + "vert", (anchorVertical == AnchorModes.max));
        PlayerPrefsX.SetVector3(name + "scale", rect.localScale);
        PlayerPrefsX.SetBool(name + "saved", true);
        Debug.Log(name + " saved ", gameObject);
    }
    public void loadLocation()
    {
        bool isSaved = PlayerPrefsX.GetBool(name + "saved");
        if (!isSaved)
        {
            Debug.Log(name + "  no Saved preferences", gameObject);
            return;
        }
        SetHorizontalPivot(AnchorModes.min);
        SetVerticalPivot(AnchorModes.max);


        if (PlayerPrefsX.GetBool(name + "horiz")) SetHorizontalPivot(AnchorModes.max); else SetHorizontalPivot(AnchorModes.min);
        if (PlayerPrefsX.GetBool(name + "vert")) SetVerticalPivot(AnchorModes.max); else SetHorizontalPivot(AnchorModes.min);
        SetDimensions(PlayerPrefsX.GetVector3(name + "size"));
        rect.position = PlayerPrefsX.GetVector3(name + "position");
        rect.localScale = PlayerPrefsX.GetVector3(name + "scale");

        Debug.Log(name + " loaded ", gameObject);
    }
    void createBorders()
    {
        int numBorders = System.Enum.GetNames(typeof(zDraggableBorder.Borders)).Length - 2;
        if (frame == null)
        {
            RectTransform frameRect;
            frame = new GameObject("Frame");
            frameRect = frame.AddComponent<RectTransform>();
            frameRect.SetParent(transform);
            frameRect.offsetMax = new Vector2(0, 0);
            frameRect.offsetMin = new Vector2(0, 0);
            frameRect.anchorMin = new Vector2(0, 0);
            frameRect.anchorMax = new Vector2(1, 1);
            frameRect = frame.GetComponent<RectTransform>();
            borderRects = new RectTransform[numBorders];
            borders = new zDraggableBorder[numBorders];
            GameObject thisSegment;
            Transform header = null;
            RectTransform segRect;
            var le = frame.AddComponent<LayoutElement>();
            le.ignoreLayout = true;
            for (int i = 0; i < numBorders; i++)
            {
                thisSegment = new GameObject(((zDraggableBorder.Borders)i).ToString());
                segRect = thisSegment.AddComponent<RectTransform>();
                segRect.SetParent(frameRect);
                segRect.offsetMax = new Vector2(0, 0);
                segRect.offsetMin = new Vector2(0, 0);
                segRect.anchorMin = new Vector2(0, 0);
                segRect.anchorMax = new Vector2(1, 1);
                zDraggableBorder border = thisSegment.AddComponent<zDraggableBorder>();

                border.SetDirection((zDraggableBorder.Borders)i);
                border.SetBorderWidth(borderWidth, headerHeight);
                border.SetTargetRect(rect);
                borders[i] = border;
                borderRects[i] = segRect;

                if (i == (int)zDraggableBorder.Borders.Drag)
                {
                    header = thisSegment.transform;
                    HorizontalLayoutGroup headerLayout = thisSegment.AddComponent<HorizontalLayoutGroup>();
                    headerLayout.childControlWidth = true;
                    headerLayout.childForceExpandWidth = true;
                }

            }
            if (draggableMenu == null) draggableMenu = Resources.Load("zDraggableMenu") as GameObject;

            if (draggableMenu != null)
            {
                GameObject thisMenu = Instantiate(draggableMenu, frame.transform);

                thisMenu.transform.SetParent(frame.transform);
                thisMenu.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                menuController = thisMenu.GetComponent<zDraggableMenuController>();

            }
            else Debug.Log("no menu");

        }
        for (int i = 0; i < numBorders; i++)
        {
            borders[i].SetDirection((zDraggableBorder.Borders)i);
            if (i == (int)zDraggableBorder.Borders.L || i == (int)zDraggableBorder.Borders.R)
                if (useLineGraphics) borders[i].SetImage(zResourceLoader.lineV); else borders[i].SetImage(null);
            if (i == (int)zDraggableBorder.Borders.T || i == (int)zDraggableBorder.Borders.B)
                if (useLineGraphics) borders[i].SetImage(zResourceLoader.lineH); else borders[i].SetImage(null);
            borders[i].SetBorderWidth(borderWidth, headerHeight);
        }
    }


    public void SetHorizontalPivot(AnchorModes newAnchoring)
    {
        anchorHorizontal = newAnchoring;
        if (menuController != null) menuController.rightAlingment(newAnchoring == AnchorModes.min);
    }
    public void SetVerticalPivot(AnchorModes newAnchoring)
    {
        anchorVertical = newAnchoring;
        SetAnchorAndPivotBasedOnAnchoring();
    }
    void SetAnchoringCorners()
    {
        rect.GetWorldCorners(corners);

       if (!isDragging)
        {
            if (anchorHorizontal != AnchorModes.stretch) if (corners[0].x < Screen.width - corners[3].x) SetHorizontalPivot(AnchorModes.min); else SetHorizontalPivot(AnchorModes.max);
            if (anchorVertical != AnchorModes.stretch) if (corners[0].y < Screen.height - corners[1].y) SetVerticalPivot(AnchorModes.min); else SetVerticalPivot(AnchorModes.max);
            if (menuController != null) menuController.rightAlingment(anchorHorizontal == AnchorModes.min);

            SetAnchorAndPivotBasedOnAnchoring();
        }
        else
       
         if (menuController != null)
            if (corners[0].x < Screen.width - corners[3].x) menuController.rightAlingment(true); else menuController.rightAlingment(false);



    }

    public void SetAnchorAndPivotBasedOnAnchoring()
    {
        float x = 0;
        float y = 0;
        // if (anchorHorizontal == AnchorModes.noSnap) x = 0.5f;
        if (anchorHorizontal == AnchorModes.min) x = 0f;
        if (anchorHorizontal == AnchorModes.max) x = 1f;
        //  if (anchorVertical == AnchorModes.noSnap) y = 0.5f;
        if (anchorVertical == AnchorModes.min) y = 0f;
        if (anchorVertical == AnchorModes.max) y = 1f;
        Vector2 t = new Vector2(x, y);
        //   if (rect.pivot != t)
        //  {
        Vector3 pos = transform.localPosition;
        Vector3 offset = getOffset();

        rect.anchorMax = t;
        rect.anchorMin = t;
        if (anchorVertical == AnchorModes.stretch)
        {
            rect.anchorMin = new Vector2(rect.anchorMin.x, 0);
            rect.anchorMax = new Vector2(rect.anchorMax.x, 1);
        }
        if (anchorVertical == AnchorModes.stretch)
        {
            rect.anchorMin = new Vector2(0, rect.anchorMin.y);
            rect.anchorMax = new Vector2(1, rect.anchorMax.y);
        }
        rect.pivot = t;
        transform.localPosition = pos + getOffset() - offset;
        // }

    }

    Vector3 getOffset()
    {
        return new Vector3(rect.sizeDelta.x * transform.localScale.x * rect.pivot.x,
                        rect.sizeDelta.y * transform.localScale.y * rect.pivot.y, 0);
    }

    void OnEnable()
    {
        OnValidate();
    }

    void OnDisable()
    {
        //    if (frame != null) if (Application.isPlaying) Destroy(frame); else DestroyImmediate(frame);
        //        if (layoutElement != null) if (Application.isPlaying) Destroy(layoutElement); else DestroyImmediate(layoutElement);
    }
    void timeRampSetup()
    {
        float t = .6f;
        timeRamp.duration = t;
        timeRamp.smoothStep = true;
        timeRamp.CallbackZero(callbackWhenFolded);
        timeRamp.CallbackOne(stopMinimizing);
    }
    private void Awake()
    {
        bgImage = GetComponentInChildren<Image>();
        bgImageRaw = GetComponentInChildren<RawImage>();
        if (bgImageRaw != null) startColor = bgImageRaw.color;
        else
       if (bgImage != null) startColor = bgImage.color;

        timeRampSetup();
        rect = GetComponent<RectTransform>();
        SetHorizontalPivot(AnchorModes.min);
        SetVerticalPivot(AnchorModes.max);
        savedSize = new Vector2(rect.rect.width, rect.rect.height);

        Vector3 pos = rect.position;
        /*   rect.pivot=new Vector2(0,1);
          
              
        rect.anchorMin=new Vector2(0,0);
        rect.anchorMax=new Vector2(0,0);*/


        corners = new Vector3[4];
        OnValidate();
        if (draggableList == null) draggableList = new List<zDraggable>();
        draggableList.Add(this);
        rect.position = pos;
        SetDimensions(savedSize);

    }
    public void SetWidth(float newWidth)
    {
        rect.sizeDelta = new Vector2(newWidth, rect.sizeDelta.y);

    }
    public void SetDimensions(Vector2 newDim)
    {

        if (newDim.x < minimalSize.x) newDim.x = minimalSize.x;
        if (newDim.y < minimalSize.y) newDim.y = minimalSize.y;
        rect.sizeDelta = newDim;

        if (textTransform != null)
            if (newDim.x < 150)
            {
                float t = newDim.x / 150;
                textTransform.localScale = new Vector3(t, t, t);
            }
            else textTransform.localScale = Vector3.one;
        if (layoutElement != null)
        {
            layoutElement.preferredHeight = rect.sizeDelta.x;
            layoutElement.preferredWidth = rect.sizeDelta.y;
        }
    }

    public void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }
    public void minimize()
    {
        minimalSize = new Vector2(20, 0);
        savedSize = rect.rect.size;
        timeRamp.duration = 0.5f;
        timeRamp.curveShape = TimeRamp.CurveShapes.smooth;
        timeRamp.JumpOne();
        timeRamp.GoZero();

        isMinimizing = true;

    }
    public void restore()
    {
        timeRamp.GoOne();
        isMinimizing = true;

    }
    void stopMinimizing()
    {
        isMinimizing = false;
    }
    public void sizeSmall()
    {

        if (isMinimized) return; else savedSize = rect.sizeDelta;
        // 
        rect.localScale = rect.localScale * 0.75f;
        if (rect.localScale.x < 0.6f)
        {
            isMinimized = true;
            rect.sizeDelta = new Vector2(150, 0);
            if (textTransform != null) textTransform.localScale = Vector3.one * 1.5f;
        }
    }
    public void sizeNormal()
    {
        if (isMinimized)
            rect.sizeDelta = savedSize;
        rect.localScale = Vector3.one;
    /*   if (isMaxed)
        {
            rect.offsetMax = savedMin;
            rect.offsetMin = savedMax;
        }
        isMaxed = false;
        isMinimized = false;

        if (textTransform != null) textTransform.localScale = Vector3.one;*/
    }

    public void sizeBig()
    {
        if (isMaxed) return;
        if (isMinimized)
        {
            sizeNormal(); return;
        }
        rect.sizeDelta = savedSize;

        //rect.localScale = rect.localScale * 1.5f;
        //  OnDragOperationEnded();
        if (rect.localScale.x > 2)
        {
            rect.localScale = Vector3.one;
            isMaxed = true;
            //   savedMax = rect.offsetMax;
            //    savedMin = rect.offsetMin;
            // rect.offsetMax = new Vector2(rect.offsetMax.x, 0);
            // rect.offsetMin = new Vector2(rect.offsetMin.x, 0);
        }
    }
     bool isDragging;
    public void OnDragOperation()
    {
        SetAnchoringCorners();
    }
    public void OnDragOperationStart()
    {
        isDragging = true;
    }
    public void OnDragOperationEnded()
    {

        isDragging = false;
        SetAnchoringCorners();
        savedSize = rect.sizeDelta;
        SetHoverState(hoverState);

    }
    private void Start()
    {
        timeRampSetup();
        savedSize = rect.sizeDelta;
          unFold();
        rect.GetWorldCorners(corners);
        SetAnchoringCorners();
        if (newPanel != null) newPanel(this);
        if (loadFromPreferencesOnStart) loadLocation();
    }

    public void updateLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    public void fold()
    { //  Debug.Log("folding");

        SetVerticalPivot(AnchorModes.max);
    
        timeRamp.GoZero();
        if (folding != null) folding.Invoke();
    }
    public void unFold()
    {
        SetVerticalPivot(AnchorModes.max);
        timeRamp.GoOne();
        if (unFolding != null) unFolding.Invoke();
    }
    void callbackWhenFolded()
    {
        if (folded != null)
        {
            folded.Invoke();
        }
    }
    public void foldInS(float t)
    {
        Invoke("fold", t);
    }
    public void unFoldInS(float t)
    {
        Invoke("unFold", t);
    }
    public void toggleFold()
    {
        if (timeRamp.value > 0)
            fold();
    }

    public static float overShootCurve(float f)
    {
        return (2 - f) * (2 - f) * f;
    }


    public void moveRelative(Vector3 relPos)
    {
        Vector3 t = rect.localPosition;
        rect.localPosition = t + relPos;
    }
    public void SetPosition(Vector3 newPos)
    {
        Vector3 t = rect.localPosition;
        rect.localPosition = new Vector2(newPos.x, t.y);
        rect.GetWorldCorners(corners);
        if (corners[0].x < borderWidth ||
                corners[2].x + borderWidth > Screen.width) newPos.x = t.x;
        rect.localPosition = newPos;
        rect.GetWorldCorners(corners);
        if (corners[1].y + borderWidth > Screen.height ||
              corners[3].y < borderWidth)
        {
            newPos.y = t.y;
            rect.localPosition = newPos;
        }
        rect.GetWorldCorners(corners);
        findMatchingEdges();
    }

    void findMatchingEdges()
    {
        /*
                Vector2 TL = corners[1];
                Vector2 TR = corners[2];
        //        zDraggable close;
                for (int i = 0; i < draggableList.Count; i++)
                {
                    zDraggable d = draggableList[i];
                    if (d == this) continue;
                    Vector3 BL = d.corners[0];
                    Vector3 BR = d.corners[3];

               //     Debug.Log("distance between " + name + " and " + d.name + " is vertical " + (BL.y - TL.y) + " h1 = " + (TL.x - BL.x) + " h2" + (TR.x - BR.x));

                }*/
    }

    private void Update()
    {
        if (!Application.isPlaying) return;
        if (isMinimizing && timeRamp.isRunning)
        {
            float k = overShootCurve(timeRamp.value);
            Debug.Log(k);
            SetDimensions((1 - k) * minimalSize + savedSize * k);
        }
        //    if (Input.GetKeyDown("s")) saveLocation();
        //     if (Input.GetKeyDown("l")) loadLocation();
    }

}