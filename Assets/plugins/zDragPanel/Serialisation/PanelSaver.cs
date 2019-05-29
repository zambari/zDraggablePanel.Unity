using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Z.DragRect
{
    public class PanelSaver : MonoBehaviour
    {

        public bool takeInactive = true;
        public string fileName = "panels.json";


        List<zDragPanel> GetPanels()
        {
            var allpanels = Resources.FindObjectsOfTypeAll(typeof(zDragPanel)) as zDragPanel[];
            var panels = new List<zDragPanel>();
            foreach (var p in allpanels)
            {
                if (p.gameObject.scene.isLoaded && (p.gameObject.activeInHierarchy || takeInactive))
                    panels.Add(p);
            }
            return panels;
        }
        [ExposeMethodInEditor]
        void SaveState()
        {


            PanelDataCollection pd = new PanelDataCollection();
            var panels = GetPanels();
            foreach (var p in panels)
            {
                pd.panelData.Add(p.GetData());
            }
            pd.ToJson(fileName);
            Debug.Log("found " + panels.Count + "p anels");
        }
        [ExposeMethodInEditor]
        void LoadState()
        {


            PanelDataCollection pd = null;
            pd = pd.FromJson(fileName);
            if (pd == null)
            {
                Debug.Log("not loaded");
            }
            pd.BuildDictionary();

            var panels = GetPanels();
            foreach (var p in panels)
            {
                var pp = pd.GetData(p.id);
                if (pp != null) p.SetData(pp);
                else Debug.Log("could not find panel data " + p.id);
                //pd.data.Add(p.GetData());
            }
        }


    }
}