
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;
//[RequireComponent(typeof(zDraggableBorderController))]

[ExecuteInEditMode]
public class zDraggable : MonoBehaviour
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
    public Color hoverColor = new Color(1, .5f, .1f, 0.2f);
    public Color neutralColor = new Color(0, 0, 0, 0.1f);
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
    Sprite lineH;
    [SerializeField]
    [HideInInspector]
    Sprite lineV;

    [SerializeField]
    [HideInInspector]
    zDraggableMenuController menuController;
    public enum AnchorModes { min, max };//,noSnap 
    public AnchorModes anchorHorizontal;
    public AnchorModes anchorVertical;
    public Vector2 minimalSize = new Vector2(100, 30);


    [SerializeField]
    [HideInInspector]
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
    Vector2 savedMax;
    Vector2 savedMin;

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

    [SerializeField]
    public static Texture2D horizontalCursor;
    [SerializeField]
    public static Texture2D vertialCursor;
    [SerializeField]
    public static Texture2D moveCursor;
    [SerializeField]
    public static Texture2D upLeftResizeCursor;
    [SerializeField]
    public static Texture2D upRightResizeCursor;
    public static List<zDraggable> draggableList;
    [SerializeField]
    protected static GameObject draggableMenu;
    // [SerializeField]
    // protected static GameObject draggableLabel;
    public static Action<zDraggable> newPanel;
    public void setHoverState(zDraggableBorder.Borders h)
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

    Image bgImage;
    RawImage bgImageRaw;
    Color startColor;
    public void setOpacity(float f)
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
        setHorizontalPivot(AnchorModes.min);
        setVerticalPivot(AnchorModes.max);
        if (refresh) refresh = false;
        if (!gameObject.activeInHierarchy || !enabled) return;
        isDragging = false;
        if (headerHeight < borderWidth) headerHeight = borderWidth;
        loadResources();

        setAnchorAndPivotBasedOnAnchoring();

        createBorders();

        if (layoutElement == null) layoutElement = GetComponent<LayoutElement>();
        if (layoutElement == null) layoutElement = gameObject.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = rect.rect.width;
        layoutElement.preferredWidth = rect.rect.height;


        if (menuController == null) menuController = GetComponentInChildren<zDraggableMenuController>();
        if (menuController != null) menuController.setHeight(headerHeight);
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
        setHorizontalPivot(AnchorModes.min);
        setVerticalPivot(AnchorModes.max);
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
        setHorizontalPivot(AnchorModes.min);
        setVerticalPivot(AnchorModes.max);


        if (PlayerPrefsX.GetBool(name + "horiz")) setHorizontalPivot(AnchorModes.max); else setHorizontalPivot(AnchorModes.min);
        if (PlayerPrefsX.GetBool(name + "vert")) setVerticalPivot(AnchorModes.max); else setHorizontalPivot(AnchorModes.min);
        setDimensions(PlayerPrefsX.GetVector3(name + "size"));
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

                border.setDirection((zDraggableBorder.Borders)i);
                border.setBorderWidth(borderWidth, headerHeight);
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
            borders[i].setDirection((zDraggableBorder.Borders)i);
            if (i == (int)zDraggableBorder.Borders.L || i == (int)zDraggableBorder.Borders.R)
                if (useLineGraphics) borders[i].setImage(lineV); else borders[i].setImage(null);
            if (i == (int)zDraggableBorder.Borders.T || i == (int)zDraggableBorder.Borders.B)
                if (useLineGraphics) borders[i].setImage(lineH); else borders[i].setImage(null);
            borders[i].setBorderWidth(borderWidth, headerHeight);
        }
    }


    public void setHorizontalPivot(AnchorModes newAnchoring)
    {
        anchorHorizontal = newAnchoring;
        if (menuController != null) menuController.rightAlingment(newAnchoring == AnchorModes.min);
    }
    public void setVerticalPivot(AnchorModes newAnchoring)
    {
        anchorVertical = newAnchoring;
        setAnchorAndPivotBasedOnAnchoring();
    }
    void setAnchoringCorners()
    {
        rect.GetWorldCorners(corners);

        if (!isDragging)
        {
            if (corners[0].x < Screen.width - corners[3].x) setHorizontalPivot(AnchorModes.min); else setHorizontalPivot(AnchorModes.max);
            if (corners[0].y < Screen.height - corners[1].y) setVerticalPivot(AnchorModes.min); else setVerticalPivot(AnchorModes.max);
            if (menuController != null) menuController.rightAlingment(anchorHorizontal == AnchorModes.min);

            setAnchorAndPivotBasedOnAnchoring();
        }
        else if (menuController != null)
            if (corners[0].x < Screen.width - corners[3].x) menuController.rightAlingment(true); else menuController.rightAlingment(false);



    }

    void loadResources()
    {
        if (horizontalCursor == null) horizontalCursor = Resources.Load("ResizeHorizontal") as Texture2D;
        if (vertialCursor == null) vertialCursor = Resources.Load("ResizeVertical") as Texture2D;
        if (horizontalCursor == null || vertialCursor == null)
        { }
        else
        {
            if (moveCursor == null) moveCursor = Resources.Load("PanView") as Texture2D;
            if (upLeftResizeCursor == null) upLeftResizeCursor = Resources.Load("ResizeUpLeft") as Texture2D; ;
            if (upRightResizeCursor == null) upRightResizeCursor = Resources.Load("ResizeUpRight") as Texture2D;
        }
        if (lineH == null) lineH = Resources.Load<Sprite>("lineH") as Sprite;
        if (lineV == null) lineV = Resources.Load<Sprite>("lineV") as Sprite;

    }
    public void setAnchorAndPivotBasedOnAnchoring()
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
        if (rect.pivot != t)
        {
            Vector3 pos = transform.localPosition;
            Vector3 offset = getOffset();
            rect.anchorMax = t;
            rect.anchorMin = t;
            rect.pivot = t;
            transform.localPosition = pos + getOffset() - offset;
        }

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
    void timeRampsetup()
    {
        float t = .6f;
        timeRamp.duration = t;
        timeRamp.smoothStep = true;
        timeRamp.CallbackZero(callbackWhenFolded);
    }
    private void Awake()
    {
        bgImage = GetComponentInChildren<Image>();
        bgImageRaw = GetComponentInChildren<RawImage>();
        if (bgImageRaw != null) startColor = bgImageRaw.color;
        else
       if (bgImage != null) startColor = bgImage.color;

        timeRampsetup();
        rect = GetComponent<RectTransform>();
        setHorizontalPivot(AnchorModes.min);
        setVerticalPivot(AnchorModes.max);
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
        setDimensions(savedSize);
      
    }
    public void setWidth(float newWidth)
    {
        rect.sizeDelta = new Vector2(newWidth, rect.sizeDelta.y);

    }
    public void setDimensions(Vector2 newDim)
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

    public void setScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }
    public void minimize()
{

    minimalSize = new Vector2(20, 0);
    savedSize=rect.rect.size;
    timeRamp.duration=0.5f;
    timeRamp.curveShape=TimeRamp.CurveShapes.smooth;

    timeRamp.JumpOne(); 
    timeRamp.GoZero();

} 
 public void restore()
{
timeRamp.GoOne();


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
        if (isMaxed)
        {
            rect.offsetMax = savedMin;
            rect.offsetMin = savedMax;

        }
        isMaxed = false;
        isMinimized = false;

        if (textTransform != null) textTransform.localScale = Vector3.one;
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
    public bool isDragging;
    public void OnDragOperation()
    {
        setAnchoringCorners();
    }
    public void OnDragOperationStart()
    {
        isDragging = true;

    }
    public void OnDragOperationEnded()
    {

        isDragging = false;
        setAnchoringCorners();
        savedSize = rect.sizeDelta;
        setHoverState(hoverState);

    }
    private void Start()
    {
        timeRampsetup();

#if USE_SETTINGS
                {
                        scaleSettingSlider =zSettings.addSlider("PanelSize", "Scales");
                        scaleSettingSlider.setRange(0.2f, 3);
                        scaleSettingSlider.valueChanged+=setScale;
                        scaleSettingSlider.defValue=(0.5f);
                }
#endif
        //  if (startFolded) timeRamp.JumpZero();
        //     else 
        savedSize = rect.sizeDelta;
   //     foldedSize = new Vector2(savedSize.x, 0);
        unFold();
        rect.GetWorldCorners(corners);
        //timeRamp.Start();
        setAnchoringCorners();
        if (newPanel != null) newPanel(this);

          if (loadFromPreferencesOnStart) loadLocation();
    }

    public void updateLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    public void fold()
    { //  Debug.Log("folding");

        setVerticalPivot(AnchorModes.max);
        //     savedSize=rect.sizeDelta;
      //  foldedSize = new Vector2(savedSize.x, 0);


        timeRamp.GoZero();
        if (folding != null) folding.Invoke();
    }
    public void unFold()
    {
        setVerticalPivot(AnchorModes.max);
        // isFolded = false;
        timeRamp.GoOne();
        if (unFolding != null) unFolding.Invoke();
    }
    void callbackWhenFolded()
    {
        if (folded != null)
        {
            folded.Invoke();
            //  isFolded = true;
        }
        //    //    if (disableOnFold) gameObject.SetActive(false);
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
        //rect.GetWorldCorners(corners);
    }
    public void setPosition(Vector3 newPos)
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
        if (timeRamp.isRunning)
        {
            float k = overShootCurve(timeRamp.value);
            setDimensions((1 - k) * minimalSize + savedSize * k);
        }
        //    if (Input.GetKeyDown("s")) saveLocation();
        //     if (Input.GetKeyDown("l")) loadLocation();
    }

}