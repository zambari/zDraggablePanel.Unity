using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Z
{
    [RequireComponent(typeof(Toggle))]
    public class PanelFoldToggle : MonoBehaviour
    {
        void Start()
        {
            var dragPanel = GetComponentInParent<zDragPanel>();
            if (dragPanel == null)
            {
                Debug.Log("no drag panel in parent ", gameObject);
            }
            else
            {
                var tgl = GetComponent<Toggle>();
                tgl.onValueChanged.AddListener((x) => dragPanel.ToggleFold(x));
            }
        }
    }
}