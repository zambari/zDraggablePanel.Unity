//zbr 2017-
// v.0.2b po merge z dragrect, moglo cos odpasc, namespace


using UnityEngine;
namespace Z{

public static class zRectExtensions
{
  /*  public static Vector2 topLeft2(RectTransform r)
    {// return r.offsetMin;
        return new Vector2(r.offsetMin.x, r.offsetMax.x);
    }
    public static Vector2 topLeft(this RectTransform r)
    { 
        return new Vector2(r.offsetMin.x, r.offsetMax.x);
    }
    public static Vector2 bottomRight(this RectTransform r)
    {
        return new Vector2(r.anchorMin.y, r.anchorMax.y);
    }
*/
    public static void SetSizeXY(this RectTransform rect, float x,float y)
    {

        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,x);
         rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, y);

    }
    public static void SetSizeX(this RectTransform rect, float v)
    {

        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, v);

    }
    public static void SetSizeY(this RectTransform rect, float v)
    {

        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, v);

    }
      public static void stretchVertical(this RectTransform rect)
    {
      //  rect.anchorMin = new Vector2(v, rect.anchorMin.y);
       // rect.anchorMax = new Vector2(v, rect.anchorMax.y);
    }
       public static void SetAnchorsY(this RectTransform rect, float min,float max)
    {
        rect.anchorMin = new Vector2(min, rect.anchorMin.y);
        rect.anchorMax = new Vector2(max , rect.anchorMax.y);
    }
        public static void SetAnchorsX(this RectTransform rect, float min,float max)
    {
        rect.anchorMin = new Vector2( rect.anchorMin.x,min);
        rect.anchorMax = new Vector2( rect.anchorMax.y,max);
    }
    public static void SetAnchorX(this RectTransform rect, float v)
    {
        rect.anchorMin = new Vector2(v, rect.anchorMin.y);
        rect.anchorMax = new Vector2(v, rect.anchorMax.y);
    }
    public static void SetPivotX(this RectTransform rect, float v)
    {
        float deltaPivot = rect.pivot.x - v;
        Vector2 temp = rect.localPosition;
        rect.pivot = new Vector2(v, rect.pivot.y);
        rect.localPosition = temp - new Vector2(deltaPivot * rect.rect.width * rect.localScale.x, 0);
    }
    public static void SetPivotY(this RectTransform rect, float v)
    {
        float deltaPivot = rect.pivot.y - v;
        Vector2 temp = rect.localPosition;
        rect.pivot = new Vector2(rect.pivot.x, v);
        rect.localPosition = temp - new Vector2(0, deltaPivot * rect.rect.height * rect.localScale.y);
    }
    public static void SetTopLeftAnchor(this RectTransform rect, Vector2 newAnchor)
    {
        Vector2 temp = rect.sizeDelta;
        rect.anchorMin = new Vector2(newAnchor.x, rect.anchorMin.y);
        rect.anchorMax = new Vector2(rect.anchorMax.x, newAnchor.y);
        rect.sizeDelta = temp;

    }
    public static void SetBottomRightAnchor(this RectTransform rect, Vector2 newAnchor)
    {
        rect.anchorMin = new Vector2(rect.anchorMin.x, newAnchor.y);
        rect.anchorMax = new Vector2(newAnchor.x, rect.anchorMax.y);

    }
}
}