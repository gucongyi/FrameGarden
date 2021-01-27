using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class DecorativeFlowerGame_FinishItem : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    Button _btnRotate;
    bool _isPointerDown;
    bool _isDrag;
    float _handlerDownTime = 0.2f;//长按时间
    float _currentHandlerDownTime;

    Vector3 _lastRotateBtnPos;

    public Canvas canvas;
    public RectTransform vaseRect;
    public DecorativeFlowerGameComponent component;
    public DecorativeFlowerGame_CanDragItem.DecorativeFlowerGameItemType Type;

    void Start()
    {
        _btnRotate = GetComponentInChildren<Button>();
        _btnRotate.onClick.AddListener(OnClickRotate);
        _btnRotate.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_isPointerDown)
        {
            if ((_currentHandlerDownTime += Time.deltaTime) >= _handlerDownTime)
            {
                _isDrag = true;
            }
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (_isDrag)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                Vector2 localPoint = Input.mousePosition - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
                this.transform.localPosition = localPoint / canvas.transform.localScale.x;
            }
            else
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(this.transform.parent as RectTransform, Input.mousePosition, Camera.main, out Vector2 localPoint);
                this.transform.localPosition = localPoint;
            }
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(vaseRect, Input.mousePosition, Camera.main))
        {
            //在范围内
            if (Type == DecorativeFlowerGame_CanDragItem.DecorativeFlowerGameItemType.SquidItem)
            {
                transform.localPosition = Vector3.zero;//花枝回到原位
            }
        }
        else
        {
            switch (Type)
            {
                case DecorativeFlowerGame_CanDragItem.DecorativeFlowerGameItemType.SquidItem:
                    FlowPoint temp = component._flowerPos.Find(x => x.trans == transform.parent);
                    component._flowerPos[component._flowerPos.IndexOf(temp)].isEnsconce = false;
                    break;
                case DecorativeFlowerGame_CanDragItem.DecorativeFlowerGameItemType.AppliqueItem:
                    component._appliqueItemList.Remove(this.gameObject);
                    break;
                case DecorativeFlowerGame_CanDragItem.DecorativeFlowerGameItemType.DecorativeItem:
                    component._decorativeItemList.Remove(this.gameObject);
                    break;
            }
            Destroy(this.gameObject);
        }
        _isDrag = false;
        _currentHandlerDownTime = 0f;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        _isPointerDown = true;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if (_currentHandlerDownTime <= _handlerDownTime)
        {//短按
            //装饰和贴花 弹出UI  其余不做
            switch (Type)
            {
                case DecorativeFlowerGame_CanDragItem.DecorativeFlowerGameItemType.AppliqueItem:
                case DecorativeFlowerGame_CanDragItem.DecorativeFlowerGameItemType.DecorativeItem:
                    RotateBtnAnim();
                    break;
            }
        }
        _isPointerDown = false;
        _currentHandlerDownTime = 0f;
    }

    void RotateBtnAnim()
    {
        _btnRotate.gameObject.SetActive(true);
        _btnRotate.transform.DOMove(this.transform.TransformPoint(50, 80, 0), 0.3f).SetEase(Ease.InOutQuad, 5).OnComplete(() => { _lastRotateBtnPos = _btnRotate.transform.localPosition; });
    }

    void OnClickRotate()
    {
        this.transform.localScale = new Vector3(this.transform.localScale.x * (-1), 1, 1);
        _btnRotate.transform.localScale = new Vector3(this.transform.localScale.x, 1, 1);
        _btnRotate.transform.localPosition = _lastRotateBtnPos;

    }
}