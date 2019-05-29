using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Z.DragRect
{
    public class LayoutSaver : MonoBehaviour
    {

        public bool takeInactive = true;
        public string fileName = "layouts.json";


        List<LayoutHelper> GetLayouts()
        {
            var allpanels = Resources.FindObjectsOfTypeAll(typeof(LayoutHelper)) as LayoutHelper[];
            var panels = new List<LayoutHelper>();
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


            LayoutDataCollection pd = new  LayoutDataCollection();
            var layouts = GetLayouts();
            foreach (var p in layouts)
            {
                pd.panelData.Add(p.GetData());
            }
            pd.ToJson(fileName);
            Debug.Log("found " + layouts.Count + "p anels");
        }
        [ExposeMethodInEditor]
        void LoadState()
        {

            LayoutDataCollection pd = null;
            pd = pd.FromJson(fileName);
            if (pd == null)
            {
                Debug.Log("not loaded");
            }
            pd.BuildDictionary();
            var layouts = GetLayouts();
            foreach (var p in layouts)
            {
                var pp = pd.GetData(p.id);
                if (pp != null) p.SetData(pp);
                else Debug.Log("could not find panel data " + p.id);
                //pd.data.Add(p.GetData());
            }
        }
    }

}
