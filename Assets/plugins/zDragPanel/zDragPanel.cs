using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Z;
#if UNITY_EDITOR
using UnityEditor;
#endif
[SelectionBase]
public class zDragPanel : MonoBehaviour
{

    [System.Serializable]
    public class HoverColors
    {
        public Color normalColor = new Color(1, 1, 1, 0.4f);
        public Color hoverColor = new Color(1, 1, 1, 0.7f);
    }

    [System.Serializable]
    public class PanelSetup
    {
        public Vector2 defaultSize = new Vector2(100, 70);
        public Text labelText;

        public Vector2 minSize = new Vector2(50, 30);
        public Vector2 maxSize = new Vector2(1920, 1080);
        public Transform frame;
        public Vector2 savedSize;
    }
    public string label = "Panel";
    public PanelSetup panelSetup;
    public HoverColors colors;
    RectTransform _rect;
    public RectTransform rect { get { if (_rect == null) _rect = GetComponent<RectTransform>(); return _rect; } }
    zDragBorder.Borders hoverState;
    bool isDragging;
    List<zDragBorder> borders = new List<zDragBorder>();
    const int screenmargin = 5;
    public bool hideFrameInHierarchy;

    #region folding
    [SerializeField] bool _isFolded;
  
    float animtime = .2f;
    bool isFolding;
    public void ToggleFold(bool b)
    {
        if (b) Fold(); else UnFold();
    }
    IEnumerator Foldroutine(bool dir)
    {
        if (isFolding) yield break;
        isFolding = true;
        if (dir) panelSetup.savedSize = new Vector2(rect.rect.width, rect.rect.height);

        rect.SetPivotY(1);
        float startTime = Time.time;
        float x = 0;
        if (!dir)
            for (int i = 0; i < borders.Count; i++) borders[i].SetHidden(false);
        while (x <= 1)
        {
            x = (Time.time - startTime) / animtime;
            rect.SetSizeY(Mathf.Lerp(0,  panelSetup.savedSize.y, Mathf.SmoothStep(0, 1, !dir ? x : 1 - x)));
            yield return null;
        }
        _isFolded = dir;
        isFolding = false;
        if (dir)
            for (int i = 0; i < borders.Count; i++)
                if (borders[i] != null)
                    borders[i].SetHidden(zDragBorder.HideWhenFolded(borders[i].borderType));
    }
    public void Fold()
    {
        if (_isFolded) return;
        StartCoroutine(Foldroutine(true));
    }
    public void UnFold()
    {
        if (!_isFolded) return;
        StartCoroutine(Foldroutine(false));

    }

    #endregion folding
    public void RegisterBorder(zDragBorder b)
    {

        borders.Add(b);
    }
    public void UnRegisterBorder(zDragBorder b)
    {
        if (!borders.Contains(b))
            borders.Remove(b);
    }
    void SetFrameHidden(bool hide)
    {
        if (panelSetup.frame == null)
            Debug.Log("Frame object not found");
        else
        {
            panelSetup.frame.gameObject.hideFlags = hide ? HideFlags.HideInHierarchy : HideFlags.None;
#if UNITY_EDITOR
            EditorApplication.RepaintHierarchyWindow();
#endif
        }
    }
    void SetDefaultSize()
    {
        rect.anchorMax = Vector2.one / 2;
        rect.anchorMin = Vector2.one / 2;
        rect.sizeDelta = panelSetup.defaultSize;
    }
    void Start()
    {
        //float w=rect.rect.width;
        //float h=rect.rect.height;
        //Debug.Log($" {name}   w{w}   h{h} sizedelta { rect.sizeDelta}");
        // rect.SetSizeWithCurrentAnchors
        //var sd=rect.sizeDelta;

    }
    void Reset()
    {
        var bor = GetComponentsInChildren<zDragBorder>();
        if (bor.Length < 3)
            gameObject.AddComponent<DragPanelCreator>();
        label = name;
    }
    void OnValidate()
    {
        //if (gameObject.scene.isLoaded)
        SetFrameHidden(hideFrameInHierarchy);
        if (panelSetup.frame == null)
            panelSetup.frame = transform.Find("Frame");
        if (panelSetup.labelText == null && panelSetup.frame != null)
            panelSetup.labelText = panelSetup.frame.GetComponentInChildren<Text>();
        if (panelSetup.labelText != null) panelSetup.labelText.text = label;
        SetDefaultSize();
    }

    void SetSizeX(float size)
    {
        if (size < panelSetup.minSize.x) size = panelSetup.minSize.x;
        if (size > panelSetup.maxSize.x) size = panelSetup.maxSize.x;
        rect.SetSizeX(size);
    }
    void SetSizeY(float size)
    {
        if (size < panelSetup.minSize.y) size = panelSetup.minSize.y;
        if (size > panelSetup.maxSize.y) size = panelSetup.maxSize.y;
        rect.SetSizeY(size);
    }
    void SetSizeXY(float sizex, float sizey)
    {
        if (sizex < panelSetup.minSize.x) sizex = panelSetup.minSize.x;
        if (sizex > panelSetup.maxSize.x) sizex = panelSetup.maxSize.x;
        if (sizey < panelSetup.minSize.y) sizey = panelSetup.minSize.y;
        if (sizey > panelSetup.maxSize.y) sizey = panelSetup.maxSize.y;
        rect.SetSizeXY(sizex, sizey);
    }
    public void HandleDrag(zDragBorder source, Vector3 delta)
    {
        if (Input.mousePosition.x < screenmargin || Input.mousePosition.x > Screen.width - screenmargin ||
          Input.mousePosition.y < screenmargin || Input.mousePosition.y > Screen.height - screenmargin) return;
        if (source.borderType == zDragBorder.Borders.Top)
        {
            rect.localPosition += delta;
            return;
        }
        else if (source.borderType == zDragBorder.Borders.B) SetSizeY(rect.sizeDelta.y - delta.y);
        else if (source.borderType == zDragBorder.Borders.L) SetSizeX(rect.sizeDelta.x - delta.x);
        else if (source.borderType == zDragBorder.Borders.R) SetSizeX(rect.sizeDelta.x + delta.x);
        if (source.borderType == zDragBorder.Borders.BL)
            SetSizeXY(rect.sizeDelta.x - delta.x, rect.sizeDelta.y - delta.y);
        else if (source.borderType == zDragBorder.Borders.BR)
            SetSizeXY(rect.sizeDelta.x + delta.x, rect.sizeDelta.y - delta.y);

    }
    public virtual void HandleBeginDrag(zDragBorder source)
    {
        isDragging = true;
        if ((source.borderType == zDragBorder.Borders.B) ||
        (source.borderType == zDragBorder.Borders.BL) ||
        (source.borderType == zDragBorder.Borders.BR))
            rect.SetPivotY(1);

        if ((source.borderType == zDragBorder.Borders.R) ||
            (source.borderType == zDragBorder.Borders.BR))
            rect.SetPivotX(0);

        if ((source.borderType == zDragBorder.Borders.L) ||
        (source.borderType == zDragBorder.Borders.BL))
            rect.SetPivotX(1);
    }
    public void HandleEndDrag(zDragBorder source)
    {
        isDragging = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void SetHoverState(zDragBorder.Borders h)
    {
        hoverState = h;
        if (!isDragging)
        {
            for (int i = 0; i < borders.Count; i++)
            {
                if (borders[i] != null)
                    borders[i].ShowHover(h);
            }
        }
    }

}
