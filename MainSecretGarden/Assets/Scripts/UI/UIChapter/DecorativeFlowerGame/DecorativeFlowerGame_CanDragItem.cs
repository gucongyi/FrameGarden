using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 插花小游戏能移动的item
/// </summary>
public class DecorativeFlowerGame_CanDragItem : DecorativeFlowerGame_Item, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public enum DecorativeFlowerGameItemType
    {
        SquidItem,//花枝     
        AppliqueItem,//贴花
        DecorativeItem,//装饰品  
    }

    protected Canvas canvas;
    protected Image mouseOnImage;
    protected RectTransform vaseRect;
    protected Transform _itemParent;
    protected DecorativeFlowerGameItemType type;

    Vector2 localPoint;//鼠标拖动物体到屏幕的位置

    protected override void Start()
    {
        base.Start();
        canvas = transform.GetComponentInParent<Canvas>();
        mouseOnImage = parent.transform.Find("mouseOnImage").GetComponent<Image>();
        vaseRect = parent.transform.Find("vase/vaseImage") as RectTransform;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        image.color = new Color(1, 1, 1, 0.3f);
        mouseOnImage.sprite = image.sprite;
        mouseOnImage.SetNativeSize();
        mouseOnImage.gameObject.SetActive(true);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            localPoint = Input.mousePosition - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
            mouseOnImage.transform.localPosition = localPoint / canvas.transform.localScale.x;
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mouseOnImage.transform.parent as RectTransform, Input.mousePosition, Camera.main, out localPoint);
            mouseOnImage.transform.localPosition = localPoint;
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        mouseOnImage.gameObject.SetActive(false);
        image.color = new Color(1, 1, 1, 1f);
    }

    /// <summary>
    /// 拖拽到正确区域
    /// </summary>
    protected void Drag2SureArea(DecorativeFlowerGameItemType type, Transform parent, Vector3 screenPos)
    {
        switch (type)
        {
            case DecorativeFlowerGameItemType.SquidItem://直接放到指定位置
                //遍历固定的位置点
                for (int i = 0; i < this.parent._flowerPos.Count; i++)
                {
                    if (!this.parent._flowerPos[i].isEnsconce)
                    {
                        Instaniate(type, mouseOnImage.gameObject, this.parent._flowerPos[i].trans, Vector3.zero);
                        this.parent._flowerPos[i].isEnsconce = true;
                        return;
                    }
                }
                break;
            case DecorativeFlowerGameItemType.AppliqueItem://贴花自由装饰
                if (this.parent._appliqueItemList.Count < this.parent._appliqueItemMaxAmount)
                    Instaniate(type, mouseOnImage.gameObject, parent, localPoint);
                break;
            case DecorativeFlowerGameItemType.DecorativeItem://装饰品自由装饰
                if (this.parent._decorativeItemList.Count < this.parent._decorativeItemMaxAmount)
                    Instaniate(type, mouseOnImage.gameObject, parent, localPoint);
                break;
        }
    }

    void Instaniate(DecorativeFlowerGameItemType type, GameObject gameObject, Transform parent, Vector3 pos)
    {
        GameObject go = Instantiate(gameObject);
        go.transform.SetParent(parent);
        go.transform.localPosition = pos;
        go.transform.localScale = Vector3.one;
        go.GetComponent<Image>().SetNativeSize();//
        go.AddComponent(typeof(DecorativeFlowerGame_FinishItem));
        DecorativeFlowerGame_FinishItem tempComponent = go.GetComponent<DecorativeFlowerGame_FinishItem>();
        tempComponent.Type = type;
        tempComponent.canvas = canvas;
        tempComponent.vaseRect = vaseRect;
        tempComponent.component = this.parent;
        switch (type)
        {
            case DecorativeFlowerGameItemType.AppliqueItem:
                this.parent._appliqueItemList.Add(go);
                break;
            case DecorativeFlowerGameItemType.DecorativeItem:
                this.parent._decorativeItemList.Add(go);
                break;
        }
        go.SetActive(true);
    }
}
