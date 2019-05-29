
using UnityEngine;
using UnityEngine.UI;
///Z
public class MonoRect : MonoBehaviour
{  
     Text _text;
     Image _image;

    protected Text text //get component will be called only if reference is requested
    {
        get
        {
            if (_text == null) _text = GetComponentInChildren<Text>();
            return _text;
        }

        set 
        {
            _text=value;
        }
    }
   
    protected Image image
    {
        get
        {
            if (_image == null) _image = GetComponent<Image>();
            return _image;
        }
        set {
            _image=value;
        }
    }

    public RectTransform rect //get component will be called only if reference is requested
    {
        get
        {
            if (_rect == null) _rect = GetComponent<RectTransform>();
            if (_rect == null) _rect = gameObject.AddComponent<RectTransform>();
            return _rect;
        }
        set
        { 
            _rect=value;
            Debug.Log("setting is now automatic, upadte API ", gameObject);
        }
    }
    public RectTransform parentRect //get component will be called only if reference is requested
    {
        get
        {
            if (_parentRect == null)
            {
                if (transform.parent == null)
                {
                    Debug.Log("no parent !", gameObject);
                    return null;
                }
                _parentRect = transform.parent.GetComponent<RectTransform>();
                if (_parentRect == null) _parentRect = transform.parent.gameObject.AddComponent<RectTransform>();

            }
            return _parentRect;
        }
        set
        {
            _parentRect = value;
        }
    }
    private RectTransform _parentRect;
    private RectTransform _rect;
}