using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RetriggerLayoutGroup : MonoBehaviour
{


    IEnumerator Start()
    {
        yield return null;
        var hg = GetComponent<HorizontalLayoutGroup>();
        var vg = GetComponent<HorizontalLayoutGroup>();
        if (hg != null) hg.enabled = false;
        if (vg != null) vg.enabled = false;
        yield return null;
        if (hg != null) hg.enabled = true;
        if (vg != null) vg.enabled = true;
    }


}
