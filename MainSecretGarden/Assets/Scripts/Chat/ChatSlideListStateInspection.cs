using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// 聊天滑动列表状态检测脚本
/// </summary>
public class ChatSlideListStateInspection : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{
    #region 字段
    /// <summary>
    /// 是否开始检测
    /// </summary>
    bool _isStart = false;
    /// <summary>
    /// 是否拖动了列表
    /// </summary>
    bool _isDrag = false;
    /// <summary>
    /// 拖动初始值
    /// </summary>
    float _initialValue;
    #endregion
    #region 属性
    /// <summary>
    /// 是否拖动了列表
    /// </summary>
    public bool _IsDrag
    {
        get { return _isDrag; }
        set
        {
            //Debug.LogError("状态更改:-------------------" + value);
            _isDrag = value;
        }
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_isStart)
        {
            if (_initialValue > ChatPanelController._Instance._LoopScrollRect.verticalNormalizedPosition || ChatPanelController._Instance._LoopScrollRect.verticalNormalizedPosition < 0.95f)
            {
                _IsDrag = true;
            }
            else
            {
                _IsDrag = false;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isStart = true;
        _initialValue = ChatPanelController._Instance._LoopScrollRect.verticalNormalizedPosition;
    }

    public void ResetData()
    {
        _isStart = false;
        _IsDrag = false;
        _initialValue = ChatPanelController._Instance._LoopScrollRect.verticalNormalizedPosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("鼠标离开");
        _isStart = false;
        if (ChatPanelController._Instance._LoopScrollRect.verticalNormalizedPosition > 0.95f)
        {
            Debug.Log("鼠标离开:没有拖拽");
            _IsDrag = false;
            _initialValue = ChatPanelController._Instance._LoopScrollRect.verticalNormalizedPosition;
        }
    }
}
