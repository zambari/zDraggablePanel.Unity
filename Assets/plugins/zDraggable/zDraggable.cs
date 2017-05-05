
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;

public class zDraggable : MonoBehaviour
{
    [Header("Disables moving")]
    public bool groupSlave;
    public int groupID;
    [Range(1, 15)]
    public float borderWidth = 6;
    [Range(10, 35)]
    public float headerHeight = 15;

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
    //protected static Color _hoverColor;
    public Color hoverColor = Color.red;
    public Color neutralColor = new Color(0, 0, 0, 0.1f);
     public Action folding; // not used in this version
     public Action unFolding; // not used in this version
     public Action folded; // not used in this version
    Vector3[] corners = new Vector3[4];

    [SerializeField]
    [HideInInspector]
    Image image;
    public enum AnchorModes { noSnap, min, max };
    public AnchorModes anchorHorizontal;
    public AnchorModes anchorVertical;

    public Vector2 minimalSize = new Vector2(200, 100);

    //    Vector2 startDimensions;
    //    Vector2 scaledDelta;
    RectTransform rect;
    const float minWidth = 60;
//    bool isFolded;

    [SerializeField]
    [HideInInspector]
    RectTransform content;
    [SerializeField]
    [HideInInspector]
    Transform textTransform;
     TimeRamp timeRamp;
     
    Vector2 savedSize;
       Vector2 foldedSize;
    bool isMinimized;
    bool isMaxed;
    Vector2 savedMax;
    Vector2 savedMin;
    public void setHorizontalPivot(AnchorModes newAnchoring)
    {

        anchorHorizontal = newAnchoring;
        if (!groupSlave&&groupID!=0)
          for (int i=0;i<draggableList.Count;i++)
            {
                zDraggable d=draggableList[i];
                if (d.groupID == groupID && d.groupSlave && d != this)
                    d.setHorizontalPivot(newAnchoring);
            }

    }
    public void setVerticalPivot(AnchorModes newAnchoring)
    {

        anchorVertical = newAnchoring;
        setAnchorAndPivotBasedOnAnchoring();
       if (!groupSlave&&groupID!=0)
            for (int i=0;i<draggableList.Count;i++)
            {
                zDraggable d=draggableList[i];
                if (d.groupID == groupID && d.groupSlave && d != this)
                    d.setHorizontalPivot(newAnchoring);
            }

    }
    void setAnchoringCorners()
    {
        rect.GetWorldCorners(corners);
        if (corners[0].x < Screen.width - corners[3].x) anchorHorizontal = AnchorModes.min; else anchorHorizontal = AnchorModes.max;
        if (corners[0].y < Screen.height - corners[1].y) anchorVertical = AnchorModes.min; else anchorVertical = AnchorModes.max;
        setAnchorAndPivotBasedOnAnchoring();

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
    }
public void setBorderWidth(float v)
{
    /// <summary>
    /// THIS IS A DEMO FUNCtON, NEEDS TO BE REMOVED 
    /// </summary>
      borderWidth=v;
              Transform t = transform.Find("CONTENT");
        if (t != null) content = t.GetComponent<RectTransform>();
        if (content != null)
            content.offsetMax = new Vector2(0, -headerHeight+borderWidth);
      zDraggableBorder[] borders = GetComponentsInChildren<zDraggableBorder>();
        foreach (zDraggableBorder b in borders)
        {
            b.setBorderWidth(borderWidth);
            b.setHeaderHeight(headerHeight);
        }
}
    void OnValidate()
    {
        if (textTransform == null)
        {
            var b = GetComponentInChildren<zDraggableNameHelper>();
            b.setName(gameObject.name);
            if (b != null) textTransform = b.transform;
        }
        loadResources();
        if (image == null) image = GetComponent<Image>();

        if (rect == null) rect = GetComponent<RectTransform>();
        setAnchorAndPivotBasedOnAnchoring();
        Transform t = transform.Find("CONTENT");
        if (t != null) content = t.GetComponent<RectTransform>();
        if (content != null)
            content.offsetMax = new Vector2(0, -headerHeight+borderWidth);
        zDraggableBorder[] borders = GetComponentsInChildren<zDraggableBorder>();
        foreach (zDraggableBorder b in borders)
        {
            b.setBorderWidth(borderWidth);
            b.setHeaderHeight(headerHeight);
        }
    }

    public void setAnchorAndPivotBasedOnAnchoring()
    {
        float x = 0;
        float y = 0;
        if (anchorHorizontal == AnchorModes.noSnap) x = 0.5f;
        if (anchorHorizontal == AnchorModes.min) x = 0f;
        if (anchorHorizontal == AnchorModes.max) x = 1f;
        if (anchorVertical == AnchorModes.noSnap) y = 0.5f;
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
        if (groupSlave) 
        {    rect.pivot=new Vector2(rect.pivot.x,1);
            rect.anchoredPosition=new Vector2(rect.anchoredPosition.x,-5);
        }
    }

    Vector3 getOffset()
    {
        return new Vector3(rect.sizeDelta.x * transform.localScale.x * rect.pivot.x,
                        rect.sizeDelta.y * transform.localScale.y * rect.pivot.y, 0);
    }
    void timeRampsetup()
    {
           float t = .6f;
          timeRamp = new TimeRamp(t);
          timeRamp.smoothStep=true;
        timeRamp.CallbackZero(callbackWhenFolded);
    }
    private void Awake()
    {
         timeRampsetup();
        rect = GetComponent<RectTransform>();
        OnValidate();
        if (draggableList == null) draggableList = new List<zDraggable>();
        draggableList.Add(this);
    }

    public void setWidth(float newWidth)
    {
        rect.sizeDelta = new Vector2(newWidth, rect.sizeDelta.y);

    }
    public void setDimensions(Vector2 newDim)
    {
        if (groupSlave) newDim.x = rect.sizeDelta.x;

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

        if (!groupSlave && groupID!=0)
            foreach (zDraggable d in draggableList)
                if (d.groupID == groupID && d.groupSlave && d != this)
                    d.setWidth(newDim.x);
       
    }

    public void setScale(float scale)
    {
        //   if (parent == null)
        transform.localScale = new Vector3(scale, scale, scale);

    }
    public void sizeSmall()
    {
        if (groupSlave) return;

        if (isMinimized) return; else savedSize = rect.sizeDelta;
        // 
        rect.localScale = rect.localScale * 0.75f;
        if (rect.localScale.x < 0.6f)
        {
            isMinimized = true;
            rect.sizeDelta = new Vector2(150, headerHeight);
            if (textTransform != null) textTransform.localScale = Vector3.one * 1.5f;
        }
    }
    public void sizeNormal()
    {
        if (groupSlave) return;
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
        if (groupSlave) return;
        if (isMaxed) return;
        if (isMinimized)
        {
            sizeNormal(); return;
        }
        rect.sizeDelta = savedSize;
        foldedSize=new Vector2(savedSize.x,headerHeight+2);
        rect.localScale = rect.localScale * 1.5f;

        if (rect.localScale.x > 2)
        {
            rect.localScale = Vector3.one;
            isMaxed = true;
            savedMax = rect.offsetMax;
            savedMin = rect.offsetMin;
            rect.offsetMax = new Vector2(rect.offsetMax.x, 0);
            rect.offsetMin = new Vector2(rect.offsetMin.x, 0);
        }
    }
    public void OnDragOperationEnded()
    {
        setAnchoringCorners();
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
            savedSize=rect.sizeDelta;
            foldedSize=new Vector2(savedSize.x,headerHeight+2);
            unFold();
            //timeRamp.Start();
    }

    public void updateLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }
 
    public void fold()
    {   Debug.Log("folding");
        
        setVerticalPivot(AnchorModes.max);
    //     savedSize=rect.sizeDelta;
         foldedSize=new Vector2(savedSize.x,headerHeight+2);
        
        if (timeRamp == null) timeRampsetup();
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
        Invoke("fold",t);
    }
    public void unFoldInS(float t)
    {
        Invoke("unFold",t);
    }
    public void toggleFold()
    {
        if (timeRamp.value > 0)
            {
                fold();
     if (!groupSlave)
            foreach (zDraggable d in draggableList)
                if (d.groupID == groupID && d != this)
                    d.foldInS(UnityEngine.Random.value/5);
            }
        else
           { unFold();
        //   if (!groupSlave)
            foreach (zDraggable d in draggableList)
                if (d.groupID == groupID && d != this)
                    d.unFoldInS(UnityEngine.Random.value/5);
           }
    }

    public static float overShootCurve(float f)
    {
         return (2 - f) * (2 - f) * f;
    }

    bool checkIfOutsideScreenH()
    {
        return (corners[0].x < borderWidth ||
             corners[2].x + borderWidth > Screen.width
          );
    }
    bool checkIfOutsideScreenV()
    {

        return
             corners[1].y + borderWidth > Screen.height ||

             corners[3].y < borderWidth;
    }
    bool checkIfOutsideScreen()
    {
        float k = 2 * borderWidth;
        return (corners[0].x < k ||
             corners[1].y + k > Screen.height ||
             corners[2].x + k > Screen.width ||
             corners[3].y < k);
    }
    public void moveRelative(Vector3 relPos)
    {
        Vector3 t = rect.localPosition;
        rect.localPosition = t + relPos;
        //rect.GetWorldCorners(corners);
    }
    public void setPosition(Vector3 newPos)
    {
        if (groupSlave) return;
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

    }
    /*if (group != 0)
    {
        int k=0;
        while (k<draggableList.Count && draggableList[k]!=this) k++;

        for (int i = k; i < draggableList.Count; i++)
        {
            if (draggableList[i].group == group && draggableList[i] != this)
                draggableList[i].moveRelative(newPos - t);
        }
    }

*/


    private void Update()
    {
        if (timeRamp.isRunning)
        {
            float k=overShootCurve(timeRamp.value);
            setDimensions((1-k)*foldedSize+savedSize*k);
        }
    }

}