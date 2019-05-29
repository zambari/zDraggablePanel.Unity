//z2k17

using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UI.Extensions;
//using System.Collections;
//using System.Collections.Generic;

public class zAnimateLayout : MonoBehaviour
{
    public enum AnimationTypes { none, layoutHeight, width, height, positionY, positionX }
  
    public AnimationTypes animationType;
    public TimeRamp.CurveShapes curveShape;
    [SerializeField]
    [HideInInspector]
    AnimationTypes lastAnimationType;


    [Range(0.1f, 2)]
    public float duration = 0.3f;
    public bool previewEnd;

    public float startValue = 100;
 
    public float endValue = 150;
    [SerializeField]
    [HideInInspector]
    LayoutElement layoutElement;
    [HideInInspector]
    [SerializeField]
    RectTransform rect;
public bool matchValue;
    public TimeRamp tr2;
    void OnValidate()
    {
        bool initial = false;
        if (rect == null) rect = GetComponent<RectTransform>();
        if (lastAnimationType != animationType)
        {
            initial = true;
            lastAnimationType = animationType;
        }

        if (layoutElement == null) layoutElement = GetComponent<LayoutElement>();
        switch (animationType)
        {
            case AnimationTypes.layoutHeight:
                if (layoutElement == null) return;
                if (initial) { startValue = layoutElement.preferredHeight; endValue = startValue; }
                if (previewEnd)
                    layoutElement.preferredHeight = endValue;
                else layoutElement.preferredHeight = startValue;
                break;
            case AnimationTypes.width:
                if (initial) { endValue = rect.sizeDelta.x; }
                if (previewEnd)
                    rect.sizeDelta = new Vector2(endValue, rect.sizeDelta.y);
                else rect.sizeDelta = new Vector2(startValue, rect.sizeDelta.y);
                break;

            case AnimationTypes.height:
                if (initial) { endValue = rect.sizeDelta.y; }
                if (previewEnd)
                    rect.sizeDelta = new Vector2(rect.sizeDelta.x, endValue);
                else
                    rect.sizeDelta = new Vector2(rect.sizeDelta.x, startValue);
                break;
            case AnimationTypes.positionY:
                if (initial) { endValue = rect.anchoredPosition.y; }
                if (previewEnd)
                    rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, endValue);
                else
                    rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, startValue);
                break;
            case AnimationTypes.positionX:
                if (initial) { endValue = rect.anchoredPosition.x; }
                if (previewEnd)
                    rect.anchoredPosition = new Vector2(endValue, rect.anchoredPosition.y);
                else
                    rect.anchoredPosition = new Vector2(startValue, rect.anchoredPosition.y);
                break;
        }
        tr2.duration = duration;
        if (goOneNow)
        {
            goOneNow = false;
            tr2.GoOne();
            //            Debug.Log(tr2.startTime);
        }
        if (goZeroNow) { goZeroNow = false; tr2.GoZero(); }
        if (!deactivateGameObjectOnClose) tr2.CallbackZero(contracted);
    }
    public bool deactivateGameObjectOnClose;
    void Awake()
    {
       
        tr2.duration = duration;
        tr2.curveShape = curveShape;
        tr2.ClearCallbacks();
        if (deactivateGameObjectOnClose) tr2.CallbackZero(contracted);
    }

    public bool goOneNow;
    public bool goZeroNow;
    void Start()
    {
        contract();
    }
    void matchValues()
    {
       switch (animationType)
            {
                case AnimationTypes.positionX:
                               startValue=rect.rect.width;
                    break;
                  case AnimationTypes.positionY:
                           startValue=rect.rect.height;
                   break;
            }
    }
    public void expand()
    {
        if (tr2.value==0)
        {
            if (matchValue) matchValues();
            gameObject.SetActive(true);
            tr2.GoOne();
        }
    }

    public void contract()
    { 
        if (matchValue) matchValues();
         
        
        tr2.GoZero();
    }
    void contracted()
    {
     //   Debug.Log("callabal");
        gameObject.SetActive(false);
    }

    public void toggle()
    {
        if (tr2.value>0)
        tr2.GoZero();
        else tr2.GoOne();
    }
   


    void Update()
    {
        if (tr2.isRunning)
        {
            float f = tr2.value;
            switch (animationType)
            {
                case AnimationTypes.layoutHeight:
                    layoutElement.preferredHeight = (1 - f) * startValue + f * endValue;
                    break;
                case AnimationTypes.width:
                    rect.sizeDelta = new Vector2((1 - f) * startValue + f * endValue, rect.sizeDelta.y);
                    break;
                case AnimationTypes.height:
                    rect.sizeDelta = new Vector2(rect.sizeDelta.x, (1 - f) * startValue + f * endValue);
                    break;
                case AnimationTypes.positionX:
                    rect.anchoredPosition = new Vector2((1 - f) * startValue + f * endValue, rect.anchoredPosition.y);
                    break;
                case AnimationTypes.positionY:
                    rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, (1 - f) * startValue + f * endValue);
                   break;
            }

        }

    }

}

