//z2k17

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UI.Extensions;
//using System.Collections;
//using System.Collections.Generic;

public class zDraggablePanelTracker : MonoBehaviour
{


    public Toggle templateObject;
    List<zDraggable> dragList;

    List<Toggle> toggleList;
    public Toggle toggleAll;
    void OnValidate()
    {
        if (templateObject == null) templateObject = GetComponentInChildren<Toggle>();
        if (templateObject != null) templateObject.gameObject.SetActive(false);

    }

    void Awake()
    {
        toggleList = new List<Toggle>();
        if (zDraggable.draggableList != null)
            for (int i = 0; i < zDraggable.draggableList.Count; i++)
                newPanel(zDraggable.draggableList[i]);

        zDraggable.newPanel += newPanel;
        if (toggleAll != null)
        {
            toggleAll.onValueChanged.AddListener(toggleAllPanels);
        }
    }
    public void toggleAllPanels(bool onoff)
    {
        if (zDraggable.draggableList != null)
            for (int i = 0; i < zDraggable.draggableList.Count; i++)
                zDraggable.draggableList[i].gameObject.SetActive(onoff);
        for (int i = 0; i < toggleList.Count; i++) toggleList[i].isOn = onoff;
    }
    void newPanel(zDraggable panel)
    {
        if (dragList == null) dragList = new List<zDraggable>();
        if (dragList.Contains(panel)) return;
        dragList.Add(panel);
        Toggle t = Instantiate(templateObject, templateObject.transform.parent);
        toggleList.Add(t);
        t.gameObject.SetActive(true);
        t.name = panel.name;
        t.onValueChanged.AddListener((x) => panel.gameObject.SetActive(x));


        Text text = t.GetComponentInChildren<Text>();
        if (text != null) text.text = panel.name;

    }



}
