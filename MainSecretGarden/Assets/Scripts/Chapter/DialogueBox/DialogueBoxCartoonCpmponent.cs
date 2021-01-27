using System;
using System.Collections;
using System.Collections.Generic;
using Company.Cfg;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 条漫摆放式对话框
/// </summary>
public class DialogueBoxCartoonCpmponent : DialogueBoxBasics
{
    #region 字段
    /// <summary>
    /// 背景框
    /// </summary>
    RectTransform _boxBgRect;
    /// <summary>
    /// 展示文本盒子
    /// </summary>
    RectTransform _showTextBoxRect;
    /// <summary>
    /// 三角形
    /// </summary>
    RectTransform _triangleRect;
    /// <summary>
    /// 气泡形
    /// </summary>
    RectTransform _bubbleRect;
    /// <summary>
    /// 文字展示text
    /// </summary>
    Text _showText;
    /// <summary>
    /// 说话前是否等待
    /// </summary>
    bool _roleSpeakAeforeAwait = false;
    /// <summary>
    /// 是否自己启动
    /// </summary>
    [SerializeField]
    bool _isStartUp = true;
    /// <summary>
    /// 是否默认说第一句话
    /// </summary>
    bool _isDefaultShow = true;
    #endregion
    #region 属性
    /// <summary>
    /// 是否自启
    /// </summary>
    public bool _IsStartUp { get { return _isStartUp; } }
    #endregion
    #region 函数
    // Start is called before the first frame update
    void Start()
    {
        if (_isStartUp)
        {
            Initial(null);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Initial(Action endAction, Action<ChapterDialogueTextDefine> roleSpeakAeforeAction = null, Action<ChapterDialogueTextDefine> roleSpeakRearAction = null, Action<ChapterDialogueTextDefine> roleDialogueBoxCloseAction = null, Action<ChapterDialogueTextDefine> roleBoxCloseAeforeAction = null)
    {
        base.Initial(endAction, roleSpeakAeforeAction, roleSpeakRearAction, roleDialogueBoxCloseAction, roleBoxCloseAeforeAction);
        _boxBgRect = GetComponent<RectTransform>();
        _showTextBoxRect = _boxBgRect.Find("Box").GetComponent<RectTransform>();
        _triangleRect = _boxBgRect.Find("Triangle").GetComponent<RectTransform>();
        _bubbleRect = _boxBgRect.Find("Bubble").GetComponent<RectTransform>();
        _showText = _showTextBoxRect.Find("Text").GetComponent<Text>();
        _boxBgRect.localScale = Vector3.zero;
        if (_isDefaultShow)
        {
            Show(_dialogueData._Data);
        }
        
    }

    public  async UniTask DefaultShow()
    {
        base.Show();
        Show(_dialogueData._Data);
    }
    /// <summary>
    /// 重置
    /// </summary>
    /// <param name="dataid"></param>
    public void ResetData(int dataid)
    {
        _dialogueData = new SingleDialogueData(dataid);
        _boxBgRect.gameObject.SetActive(true);
        Show(_dialogueData._Data);
    }
    /// <summary>
    /// 重置
    /// </summary>
    /// <param name="dataid"></param>
    public void ResetFunctionData(int dataid, Action<ChapterFunctionTextDefine> endAction)
    {
        _dialogueData = new SingleDialogueData(dataid, true);
        _boxBgRect.gameObject.SetActive(true);
        _roleSpeakRearTwoAction = endAction;
        Show(_dialogueData._FunctionData);
    }

    public void SetIsDefaultShow(bool isDefaultShow)
    {
        _isDefaultShow = isDefaultShow;
    }
    /// <summary>
    /// 展示对话
    /// </summary>
    /// <param name="data"></param>
    public override async UniTask Show(ChapterDialogueTextDefine data)
    {
      await  base.Show(data);

        _roleSpeakAeforeAction?.Invoke(data);
        await UniTask.WaitUntil(() => !_roleSpeakAeforeAwait);

        _showText.text = ChapterTool.GetDialogueString(_dialogueData._Data);

        _boxBgRect.gameObject.SetActive(true);
        RefreshGroup();
        await ChapterTool.ChangeUiScale(_boxBgRect, Vector3.one, 0.1f, 10);
        _roleSpeakRearAction?.Invoke(_dialogueData._Data);
    }
    /// <summary>
    /// 展示对话
    /// </summary>
    /// <param name="data"></param>
    public async void Show(ChapterFunctionTextDefine data)
    {
        await UniTask.WaitUntil(() => !_roleSpeakAeforeAwait);

        _showText.text = ChapterTool.GetChapterFunctionString(data.ID);

        _boxBgRect.gameObject.SetActive(true);
        RefreshGroup();
        await ChapterTool.ChangeUiScale(_boxBgRect, Vector3.one, 0.1f, 10);
        _roleSpeakRearTwoAction?.Invoke(_dialogueData._FunctionData);
    }
    /// <summary>
    /// 强制刷新布局组件
    /// </summary>
    public void RefreshGroup()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_showTextBoxRect);
        _boxBgRect.sizeDelta = _showTextBoxRect.sizeDelta;
    }

    /// <summary>
    /// 关闭人物对话框
    /// </summary>
    /// <returns></returns>
    public async UniTask CloseShowText()
    {
        if (_boxBgRect.gameObject.activeSelf)
        {
            await ChapterTool.ChangeUiScale(_boxBgRect, Vector3.zero, 0.1f, 10, null, () =>
            {
                _roleDialogueBoxCloseAction?.Invoke(_dialogueData._Data);
                _boxBgRect.gameObject.SetActive(false);
            });
        }
    }
    #endregion

}
