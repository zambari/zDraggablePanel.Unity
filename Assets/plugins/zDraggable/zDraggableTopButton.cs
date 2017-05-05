//z2k17

using UnityEngine;
using UnityEngine.UI;


public class zDraggableTopButton : MonoBehaviour
{

    public enum Functions { normal, bigger, smaller,toggleFold }
    public Functions function;
    [SerializeField]
    zDraggable draggable;

    void Start()
    {   draggable = GetComponentInParent<zDraggable>();
        Button b = GetComponent<Button>();
        b.onClick.AddListener(OnPress);
    }
    public void OnPress()
    {
        switch (function)
        { case Functions.toggleFold:
                draggable.toggleFold();
            break;
            case Functions.bigger:
                draggable.sizeBig();
                break;
            case Functions.normal:
                draggable.sizeNormal();
                break;
            case Functions.smaller:
                draggable.sizeSmall();
                break;
        }
    }
}
