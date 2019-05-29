using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Z.DragRect
{
    [System.Serializable]
    public class LayoutData
    {


        public int id;
        public float minH;
        public float minW;

        public float prefH;
        public float prefW;

        public float flexH;
        public float flexW;
        public bool ignorelayout;

    }
    [System.Serializable]
    public class LayoutDataCollection
    {

        public List<LayoutData> panelData = new List<LayoutData>();
        public Dictionary<int, LayoutData> panelDict;
        public void BuildDictionary()
        {
            panelDict = new Dictionary<int, LayoutData>();
            if (panelData == null)
            {
                Debug.Log("no dict");
                return;
            }
            foreach (var p in panelData)
                panelDict.Add(p.id, p);
        }
        public LayoutData GetData(int id)
        {
            if (panelDict == null) BuildDictionary();
            LayoutData pd = null;
            panelDict.TryGetValue(id, out pd);
            return pd;
        }


    }
}