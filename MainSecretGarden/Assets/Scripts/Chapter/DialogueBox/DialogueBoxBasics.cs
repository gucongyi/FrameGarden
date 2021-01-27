using Company.Cfg;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 对话框基础类
/// </summary>
public class DialogueBoxBasics : MonoBehaviour
{
    #region 字段
    /// <summary>
    /// 展示文字text
    /// </summary>
    protected Text _showText;
    /// <summary>
    /// 背景点击按钮
    /// </summary>
    protected Button _bgButton;
    /// <summary>
    /// 文字背景框
    /// </summary>
    protected RectTransform _bgBoxRect;
    /// <summary>
    /// 对话完毕
    /// </summary>
    protected Action _endAction;
    /// <summary>
    /// 对话框关闭前
    /// </summary>
    protected Action<ChapterDialogueTextDefine> _roleBoxCloseAeforeAction;
    /// <summary>
    /// 人物说话前回调
    /// </summary>
    protected Action<ChapterDialogueTextDefine> _roleSpeakAeforeAction;
    /// <summary>
    /// 人物说话后回调
    /// </summary>
    protected Action<ChapterDialogueTextDefine> _roleSpeakRearAction;
    /// <summary>
    /// 人物说话后回调Two
    /// </summary>
    protected Action<ChapterFunctionTextDefine> _roleSpeakRearTwoAction;
    /// <summary>
    /// 对话框关闭后回调
    /// </summary>
    protected Action<ChapterDialogueTextDefine> _roleDialogueBoxCloseAction;
    /// <summary>
    /// 是否为功能文本
    /// </summary>
    bool _isFunction = false;
    /// <summary>
    /// 对话id
    /// </summary>
    [SerializeField]
    protected int _startDialogueId;
    /// <summary>
    /// 对话数据
    /// </summary>
    protected SingleDialogueData _dialogueData;

    protected CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    /// <summary>
    /// 是否初始化完毕
    /// </summary>
    bool _isInitial = false;
    /// <summary>
    /// 是否结束
    /// </summary>
    protected bool _isEnd = false;
    #endregion
    #region 属性
    /// <summary>
    /// 是否初始化完毕
    /// </summary>
    public bool _IsInitial { get { return _isInitial; } }
    #endregion
    #region 函数
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// 初始化组件
    /// </summary>
    public virtual void Initial(Action endAction, Action<ChapterDialogueTextDefine> roleSpeakAeforeAction = null, Action<ChapterDialogueTextDefine> roleSpeakRearAction = null, Action<ChapterDialogueTextDefine> roleDialogueBoxCloseAction = null, Action<ChapterDialogueTextDefine> roleBoxCloseAeforeAction = null)
    {
        _endAction = endAction;
        _roleSpeakAeforeAction = roleSpeakAeforeAction;
        _roleSpeakRearAction = roleSpeakRearAction;
        _roleDialogueBoxCloseAction = roleDialogueBoxCloseAction;
        _roleBoxCloseAeforeAction = roleBoxCloseAeforeAction;
        _dialogueData = new SingleDialogueData(_startDialogueId, _isFunction);
        _isInitial = true;
    }

    public virtual async void Show()
    {

    }
    /// <summary>
    /// 展示文字
    /// </summary>
    /// <param name="str"></param>
    public virtual void ShowStr(string str)
    {

    }
    /// <summary>
    /// 展示文字
    /// </summary>
    /// <param name="str"></param>
    public virtual async UniTask ShowStr(string str, string nameStr)
    {

    }
    /// <summary>
    /// 展示数据
    /// </summary>
    /// <param name="chapterDialogueTextDefine"></param>
    public virtual async UniTask Show(ChapterDialogueTextDefine chapterDialogueTextDefine)
    {

    }
    /// <summary>
    /// 设置对话框类型
    /// </summary>
    /// <param name="type"></param>
    public virtual async void SetDialogType(ChapterDialogueTextDefine data)
    {

    }
    /// <summary>
    /// 点击
    /// </summary>
    public virtual async UniTask ClickBtn()
    {

    }
    /// <summary>
    /// 关闭
    /// </summary>
    public virtual void Close()
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 开关点击
    /// </summary>
    /// <param name="isOpen"></param>
    public virtual void OpenClickBtn(bool isOpen)
    {

        if (_bgButton != null)
        {
            _bgButton.enabled = isOpen;
        }
    }
    /// <summary>
    /// 设置初始id
    /// </summary>
    /// <param name="id"></param>
    public virtual void SetStartDialogueId(int id)
    {
        _startDialogueId = id;
        _isEnd = false;
    }
    /// <summary>
    /// 设置初始id
    /// </summary>
    /// <param name="id"></param>
    public virtual void SetIsFunction(bool isFunction)
    {
        _isFunction = isFunction;
    }
    #endregion
}
