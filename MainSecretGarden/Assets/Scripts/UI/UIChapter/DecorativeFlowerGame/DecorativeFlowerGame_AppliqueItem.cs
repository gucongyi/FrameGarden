using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 贴花item
/// </summary>
public class DecorativeFlowerGame_AppliqueItem : DecorativeFlowerGame_CanDragItem
{
    protected override void Start()
    {
        base.Start();
        _itemParent = parent.transform.Find("vase/Applique");
        type = DecorativeFlowerGameItemType.AppliqueItem;
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
        if (RectTransformUtility.RectangleContainsScreenPoint(vaseRect, Input.mousePosition, Camera.main))
        {
            Drag2SureArea(type, _itemParent, Input.mousePosition);
        }
    }
}
