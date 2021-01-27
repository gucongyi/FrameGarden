using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 条漫特殊模块基础类
/// </summary>
public class CaricaturePlayerSpecialModuleBasics : MonoBehaviour
{
    #region 字段
    /// <summary>
    /// 结束回调
    /// </summary>
    Action _endAction;
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    bool _isInitial = false;
    /// <summary>
    /// 是否开启点击
    /// </summary>
    protected bool _isClick = true;
    protected Action<bool> _openBtnAction;
    /// <summary>
    /// 同步按钮
    /// </summary>
    protected Button _synchronizationBtn;
    /// <summary>
    /// 是否需要操作
    /// </summary>
    [SerializeField]
    public bool _isOperation = false;
    #endregion
    #region 属性
    #endregion
    #region 函数
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_synchronizationBtn != null)
        {
            OpenClick(_synchronizationBtn.enabled);
        }
    }
    /// <summary>
    /// 设置结束回调
    /// </summary>
    /// <param name="endAction"></param>
    public void SetEndAction(Action<bool> openBtnAction, Action endAction)
    {
        _openBtnAction = openBtnAction;
        _endAction = endAction;
    }
    /// <summary>
    /// 初始化
    /// </summary>
    public virtual void Initial()
    {
        _isInitial = true;
    }

    public virtual void OpenClick(bool isOpen)
    {
        _openBtnAction?.Invoke(isOpen);
    }
    /// <summary>
    /// 移动结束
    /// </summary>
    public virtual void MoveEnd()
    {

    }
    /// <summary>
    /// 点击
    /// </summary>
    public virtual void Click()
    {
        if (!_isClick)
        {
            return;
        }
    }
    /// <summary>
    /// 结束
    /// </summary>
    public virtual void Over()
    {
        _synchronizationBtn = null;
        _endAction?.Invoke();
    }
    #endregion
}
