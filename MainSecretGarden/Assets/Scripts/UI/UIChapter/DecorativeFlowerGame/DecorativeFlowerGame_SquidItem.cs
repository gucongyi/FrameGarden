using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 花枝item
/// </summary>
public class DecorativeFlowerGame_SquidItem : DecorativeFlowerGame_CanDragItem
{
    protected override void Start()
    {
        base.Start();
        _itemParent = parent.transform.Find("vase/Squid");
        type = DecorativeFlowerGameItemType.SquidItem;
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
        //判断放到瓶子上
        if (RectTransformUtility.RectangleContainsScreenPoint(vaseRect, Input.mousePosition, Camera.main))
        {
            Drag2SureArea(type, _itemParent, Input.mousePosition);
        }
    }
}
