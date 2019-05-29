//z2k17

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using UnityEngine.UI.Extensions;
//using System.Collections;
//using System.Collections.Generic;

public class zDraggableMenuController : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    zDraggable draggable;
    public GameObject menu;
    //    RectTransform hoverRect;
    public Text labelText;
    public RectTransform labelRect;
    public bool showMenuPreview;
    public RectTransform menulRect;
    public zAnimateLayout menuAnimation;
    public GameObject hoverButton;

    public GameObject opacitySlider;

    Image ContentBG;
    public void OnPointerEnter(PointerEventData e)
    {
        CancelInvoke("showMenu");
        Invoke("showMenu", 0.1f);

    }
    public void OnPointerExit(PointerEventData e)
    {
        //        Debug.Log("hh");
        CancelInvoke("hideMenu");
        Invoke("hideMenu", 0.1f);

    }

    void showMenu()
    {
        if (menu != null) menu.SetActive(true);
        if (menuAnimation != null)
            menuAnimation.expand();


        CancelInvoke("disabl");
        //   hoverButton.SetActive(false);
    }

    void hideMenu()
    {
        if (menuAnimation != null)
            menuAnimation.contract();
        Invoke("disabl", 1);

        if (hoverButton != null)
            hoverButton.SetActive(true);
        /* for (int i=0;i<objects.Length;i++)
         {
             objects[i].SetActive(false);
         }*/
    }

    void disabl()
    {
        if (menu != null)
            menu.SetActive(false);

    }


    [SerializeField]
    [HideInInspector]
    RectTransform rect;
    GameObject[] objects;
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        draggable = GetComponentInParent<zDraggable>();
        // showMenu();
        // hideMenu();
    }
    void OnValidate()
    {
        if (showMenuPreview) showMenu(); else hideMenu();
    }
    public void toggleMinimize()
    {
        hideMenu();
        draggable.toggleFold();
    }
    public void minimize()
    {
        hideMenu();
        draggable.minimize();
    }
    public void restore()
    {
        hideMenu();
        draggable.restore();
    }
    public void SetHeight(float f)
    {
        if (rect == null) rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(f, f);

    }
    public void save()
    {
        hideMenu();
        draggable.saveLocation();
    }

    public void load()
    {
        hideMenu();
        draggable.loadLocation();
    }
    public void saveAll()
    {
        hideMenu();
        draggable.saveAllLocation();
    }
    int last = -1;
    public void rightAlingment(bool isit)
    {

        if (isit)
        {
            if (last == 1) return;
            last = 1;
            rect.anchorMin = new Vector2(1, 1f);
            rect.anchorMax = new Vector2(1, 1f);
            rect.pivot = new Vector2(1, 0f);

            labelRect.anchorMin = new Vector2(0, 0f);
            labelRect.anchorMax = new Vector2(0, 0f);
            labelRect.pivot = new Vector2(1, 0);
            if (menulRect != null)
            {
                menulRect.anchorMin = new Vector2(1, 1);
                menulRect.anchorMax = new Vector2(1, 1);
                menulRect.pivot = new Vector2(0, 1);
            }
            if (labelText != null)
                labelText.alignment = TextAnchor.LowerRight;
        }
        else
        {
            if (last == 0) return;
            last = 0;
            rect.anchorMin = new Vector2(0, 1f);
            rect.anchorMax = new Vector2(0, 1f);
            rect.pivot = new Vector2(0, 0f);

            labelRect.anchorMin = new Vector2(1, 0);
            labelRect.anchorMax = new Vector2(1, 0);
            labelRect.pivot = new Vector2(0, 0);
            if (menulRect != null)
            {
                menulRect.anchorMin = new Vector2(0, 1);
                menulRect.anchorMax = new Vector2(0, 1);
                menulRect.pivot = new Vector2(1, 1);
            }
            if (labelText != null)


                labelText.alignment = TextAnchor.LowerLeft;
        }


    }

    public void loadAll()
    {
        draggable.loadAllLocation();
    }

    void reLink()
    {
        menu.transform.SetParent(transform);
        menu.transform.localPosition = Vector3.zero;
        //   menu.transform.localScale = Vector3.one;
    }
    public void SetOpacity(float f)
    {
        draggable.SetOpacity(f);

    }
    public void SetScale(float f)
    {
        CancelInvoke("reLink");
        Invoke("reLink", 1.3f);
        menu.transform.SetParent(draggable.transform.parent);
        f = f * 0.3f + 0.7f;
        draggable.SetHorizontalPivot(zDraggable.AnchorModes.min);
        draggable.SetVerticalPivot(zDraggable.AnchorModes.max);
        draggable.rect.localScale = new Vector3(f, f, f);


    }

}

