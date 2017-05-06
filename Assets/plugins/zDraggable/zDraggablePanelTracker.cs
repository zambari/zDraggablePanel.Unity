//z2k17

using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UI.Extensions;
//using System.Collections;
//using System.Collections.Generic;

public class zDraggablePanelTracker : MonoBehaviour
{

    public Toggle templateObject;

    void OnValidate()
    {
        if (templateObject == null) templateObject = GetComponentInChildren<Toggle>();
        if (templateObject != null) templateObject.gameObject.SetActive(false);
    }

    void Awake()
    {
       
        if (zDraggable.draggableList != null)
            for (int i = 0; i < zDraggable.draggableList.Count; i++)
                newPanel(zDraggable.draggableList[i]);

    zDraggable.newPanel += newPanel;
    }
    void newPanel(zDraggable panel)
    {
        Toggle t = Instantiate(templateObject, transform);
        t.gameObject.SetActive(true);
        t.onValueChanged.AddListener((x) => panel.gameObject.SetActive(x));


        Text text=t.GetComponentInChildren<Text>();
        if (text!=null) text.text=panel.name;

    }



}
