using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[ExecuteInEditMode]
public class LayoutSetup : MonoRect
{
#if not
    public LayoutHelper[] helpers;
    public bool controlAll;
    public LayoutElement[] elements;
    VerticalLayoutGroup vg;
    HorizontalLayoutGroup hg;
    [Range(0, 16)]
    public int size = 4;

    public HoverableColors colors;
    public Color baseColor = Color.gray;
    public Color hoverColor = Color.green;
    public Color dragColor = Color.red;
    public bool editAll;
    public bool saveToPrefs = false;
    public bool loadFromrefs = false;
    public bool clearMyPrefs = false;
    bool vertical;
    [Range(0, 1)]
    public float valSet;
    float _val;
    [HideInInspector]
    public long randomId;
    public float val
    {
        get { return _val; }
        set
        {
            if (_val == value) return;
            _val = value;
            valSet = _val;

        }
    }
    ISyncColors[] colorSyncers;
    void Start()
    {
        clearIf();
    }
    void clearIf()
    {
        if (clearMyPrefs)
        {
            clearMyPrefs = false;
            for (int i = 0; i < helpers.Length; i++)

                if (helpers[i] != null)
                {
                    string s = helpers[i].getID();
                    PlayerPrefs.DeleteKey(s);
                }
        }

    }
    public void setColors(HoverableColors c)
    {
        if (c != null)
        {
            colors = c;
        }
    }
    void editColors()
    {
        if (editAll)
        {
            hoverColor = baseColor;
            dragColor = baseColor;
        }

        for (int i = 0; i < helpers.Length; i++)
        {
            // helpers[i].baseColor = baseColor;
        if ( helpers[i]!=null)
        {
            helpers[i].setSize(size);
            helpers[i].setVisibleInHierarchy(helpersVisible);
        }
        }
        if (vg != null)
            vg.spacing = size;
        else
        if (hg != null) hg.spacing = size;
        //  BroadcastMessage("setBaseColor",baseColor);
        //  BroadcastMessage("setHoverColor",hoverColor);
    }
    public bool resetS;
    public bool resetTotallyAll;
    public bool bothDirs;
    public bool helpersVisible=true;
    void allReset()
    {
/*        LayoutElement[] li;
        if (resetTotallyAll) li = GetComponentsInChildren<LayoutElement>();
        else
            li = elements;
*/


        LayoutHelper.reset(elements, vertical);
        if (bothDirs) LayoutHelper.reset(elements, !vertical);
    }

    protected void OnValidate()
    {
    LayoutSetup[] l=GetComponentsInParent<LayoutSetup>();
if (l.Length>1) {
    Debug.Log("child setup, disabling ",gameObject);
        enabled=false;
}
        if (randomId == 0)
        {
            randomId = (long)((long.MaxValue / 1000) * Random.value);
        }
        getHelpers();
        editColors();
       
  colorSyncers = GetComponentsInChildren<ISyncColors>();
        for (int i = 0; i < colorSyncers.Length; i++)
        {
            colorSyncers[i].setColors(colors);

        }
        //setColors
        LayoutSetup[] s = GetComponentsInChildren<LayoutSetup>();
        for (int i = 0; i < s.Length; i++)
        {
            s[i].saveToPrefs = saveToPrefs;
            s[i].loadFromrefs = loadFromrefs;
            s[i].clearMyPrefs = clearMyPrefs;
        }
        resetS = resetS.executeIfTrue(allReset);
      if (colors!=null)
        colors.OnChange.Invoke();
    }
    void getHelpers()
    {
        vg = GetComponent<VerticalLayoutGroup>();
        if (vg == null)
        {
            hg = GetComponent<HorizontalLayoutGroup>();
        }
        vertical = (vg != null);
        if (!controlAll)
        {
            if (vertical)
                elements = vg.getActiveElements();
            else
                elements = hg.getActiveElements();
        } else 
        elements=GetComponentsInChildren<LayoutElement>();
        helpers = new LayoutHelper[elements.Length];
        for (int i = 0; i < elements.Length; i++)
        {
            helpers[i] = elements[i].GetComponentInChildren<LayoutHelper>();
 //     if (helpers[i] == null)
//               Debug.Log(elements[i].name + " has no layout helper");
        }
        val = valSet;
    }


    public string getID()
    {
        return "LayoutHelper_" + randomId.ToString();
    }
    void Awake()
    {
  OnValidate();
        if (helpers == null || helpers.Length == 0) getHelpers();
        clearIf();
        if (loadFromrefs)
        {

            loadPrefs();

        }
    }
    void loadPrefs()
    {
        for (int i = 0; i < helpers.Length; i++)

            if (helpers[i] != null)
            {
                string s = helpers[i].getID();
                float thisflex = PlayerPrefs.GetFloat(s, -1);
                if (thisflex != -1)
                {
                    if (vertical)
                        elements[i].flexibleHeight = thisflex;
                    else
                        elements[i].flexibleWidth = thisflex;
                }
            }
    }

    void OnEnable()
    {
        if (loadFromrefs)
        {
            loadPrefs();
        }
    }

    void savePrefs()
    {
      //  Debug.Log("Saving");
        LayoutHelper.normalizeFlex(elements, vertical);
        for (int i = 0; i < helpers.Length; i++)

            if (helpers[i] != null)
            {
                float val = (vertical ? elements[i].flexibleHeight : elements[i].flexibleWidth);
                string s = helpers[i].getID();
                if (val > 0)
                {
                    PlayerPrefs.SetFloat(s, val);
                }
             //   Debug.Log(s + " " + val);
            }

    }
    void OnApplicationQuit()
    {
        if (saveToPrefs)
        {

            savePrefs();


        }
    }
#endif    

}
