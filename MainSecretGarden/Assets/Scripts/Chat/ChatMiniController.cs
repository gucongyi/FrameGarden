using Company.Cfg;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 聊天按钮控制器
/// 2020/10/28 HuangJiangDong
/// </summary>
public class ChatMiniController : MonoBehaviour
{
    #region 字段
    public static ChatMiniController _Instance;
    RectTransform _thisRect;
    /// <summary>
    /// 按钮组件
    /// </summary>
    UIPanelDrag _btn;
    /// <summary>
    /// 按钮位置属性
    /// </summary>
    RectTransform _btnRect;
    /// <summary>
    /// 新消息标签
    /// </summary>
    Image _label;
    /// <summary>
    /// 群组显示组件
    /// </summary>
    CanvasGroup _canvasGroup;
    CanvasScaler _canvasScaler;
    /// <summary>
    /// 鼠标按下时位置
    /// </summary>
    Vector3 _pointerDownVector3;
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    bool _isInitial = false;
    #endregion
    #region 函数
    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }
    public void Initial()
    {
        _thisRect = GetComponent<RectTransform>();
        _btn = transform.Find("Button").GetComponent<UIPanelDrag>();
        _btnRect = _btn.transform.GetComponent<RectTransform>();
        _label = _btn.transform.Find("Label").GetComponent<Image>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _btn.m_DragPlane = GetComponent<RectTransform>();
        _canvasScaler = UIRoot.instance.GetUIRootCanvasTop().transform.GetComponent<CanvasScaler>();
        _btn.actionOnClick = ClickBtn;
        _btn.actionOnPointerUp = PointerUp;
        _btn.actionOnPointerDown = PointerDown;
        InitialPoint();
        ChatTool.EnrollChatMiniAction(OpenLabelAction);
        _canvasGroup.alpha = 0.5f;
        OpenLabel(ChatTool.IsNewMessagePrivateChat());
        _isInitial = true;
    }


    /// <summary>
    /// 初始化位置
    /// </summary>
    void InitialPoint()
    {
        float x = (_canvasScaler.referenceResolution.x / 2) - (_btnRect.sizeDelta.x / 2);
        Vector3 initializeVector = new
            Vector3(-x, (_thisRect.rect.height / 2)-(_btnRect.sizeDelta.y / 2)-400);
        _btnRect.localPosition = initializeVector;
    }
    /// <summary>
    /// 消息按钮点击
    /// </summary>
    public async void ClickBtn(PointerEventData pointerEventData)
    {
        if (_pointerDownVector3 != _btnRect.localPosition)
        {
            PointerUp(null);
            return;
        }
        StaticData.DataDot(DotEventId.ChatIcon);
        await StaticData.OpenChatPanel();
        _canvasGroup.alpha = 0.5f;
        OpenLabel(false);
    }
    /// <summary>
    /// 拖拽中鼠标放开回调
    /// </summary>
    /// <param name="obj"></param>
    private void PointerUp(PointerEventData obj)
    {
        _btn.localPos = _btnRect.localPosition;
        IsLeftAnRight(_btn.localPos);
    }
    /// <summary>
    /// 鼠标按下回调
    /// </summary>
    /// <param name="obj"></param>
    private void PointerDown(PointerEventData obj)
    {
        _canvasGroup.alpha = 1f;
        _pointerDownVector3 = _btnRect.localPosition;
    }
    /// <summary>
    /// 开关新消息标签
    /// </summary>
    void OpenLabelAction()
    {
        if (ChatPanelController._Instance != null && ChatPanelController._Instance.gameObject.activeSelf)
        {
            return;
        }
        OpenLabel(true);
    }
    /// <summary>
    /// 打开提示标签
    /// </summary>
    public void OpenLabel(bool isOpen)
    {
        _label.gameObject.SetActive(isOpen);
    }
    /// <summary>
    /// 判读左右吸附
    /// </summary>
    /// <param name="vector3"></param>
    public void IsLeftAnRight(Vector3 vector3, bool isShake = true)
    {
        Vector3 left = new Vector3(-(_canvasScaler.referenceResolution.x / 2) + (_btnRect.sizeDelta.x / 2), vector3.y);
        Vector3 right = new Vector3((_canvasScaler.referenceResolution.x / 2) - (_btnRect.sizeDelta.x / 2), vector3.y);

        float radiusY = _thisRect.rect.height / 2;
        float maxY = radiusY - _btnRect.sizeDelta.y / 2;
        float minY = (-radiusY) + _btnRect.sizeDelta.y / 2;

        float distanceLeft = Vector3.Distance(vector3, left);
        float distanceRight = Vector3.Distance(vector3, right);

        if (vector3.y > maxY)
        {
            vector3 = new Vector3(vector3.x, maxY);
        }
        else if (vector3.y < minY)
        {
            vector3 = new Vector3(vector3.x, minY);
        }

        if (distanceLeft < distanceRight)
        {
            _btn.localPos = new Vector2(left.x, vector3.y);
            _btnRect.localPosition = new Vector2(left.x, vector3.y);
        }
        else
        {
            _btn.localPos = new Vector2(right.x, vector3.y);
            _btnRect.localPosition = new Vector2(right.x, vector3.y);
        }

        if (isShake)
        {
            StartCoroutine(ChatTool.BtnShake(_btnRect, new Vector3(10, 0, 0), 1, 500, () =>
            {

                _canvasGroup.alpha = 0.5f;
                IsLeftAnRight(_btnRect.localPosition, false);
                StopAllCoroutines();
            }));
        }

    }

    #endregion
}
