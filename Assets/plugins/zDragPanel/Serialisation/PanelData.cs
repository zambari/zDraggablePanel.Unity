using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Z.DragRect
{
    [System.Serializable]
    public class PanelData
    {

        public Vector2 position;
        public Vector2 size;
        public int id;

    }
    [System.Serializable]
    public class PanelDataCollection
    {

        public List<PanelData> panelData = new List<PanelData>();
        public Dictionary<int, PanelData> panelDict;
        public void BuildDictionary()
        {
            panelDict = new Dictionary<int, PanelData>();
            if (panelData == null)
            {
                Debug.Log("no dict");
                return;
            }
            foreach (var p in panelData)
                panelDict.Add(p.id, p);
        }
        public PanelData GetData(int id)
        {
            if (panelDict == null) BuildDictionary();
            PanelData pd = null;
            panelDict.TryGetValue(id, out pd);
            return pd;
        }

    }
}