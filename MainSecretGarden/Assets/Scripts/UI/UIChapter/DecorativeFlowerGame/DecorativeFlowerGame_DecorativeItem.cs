using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 装饰品item
/// </summary>
public class DecorativeFlowerGame_DecorativeItem : DecorativeFlowerGame_CanDragItem
{
    protected override void Start()
    {
        base.Start();
        _itemParent = parent.transform.Find("vase/Decorative");
        type = DecorativeFlowerGameItemType.DecorativeItem;
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
    }
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        //鼠标在指定rect范围内
        if (RectTransformUtility.RectangleContainsScreenPoint(vaseRect, Input.mousePosition, Camera.main))
        {
            Drag2SureArea(type, _itemParent, Input.mousePosition);
        }
    }
}
