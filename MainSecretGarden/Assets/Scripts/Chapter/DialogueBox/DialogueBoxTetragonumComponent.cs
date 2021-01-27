using Company.Cfg;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 方形对话框
/// </summary>
public class DialogueBoxTetragonumComponent : DialogueBoxBasics
{
    #region 字段
    /// <summary>
    /// 名字框rect
    /// </summary>
    RectTransform _nameBoxRect;
    /// <summary>
    /// 名字展示text
    /// </summary>
    Text _nameText;
    /// <summary>
    /// 文本内容ui组件
    /// </summary>
    RectTransform _showTextRect;
    /// <summary>
    /// 头像展示icon
    /// </summary>
    Image _icon;
    /// <summary>
    /// 头像ui组件
    /// </summary>
    RectTransform _iconRect;
    /// <summary>
    /// 文字内容展示部分布局组件
    /// </summary>
    HorizontalLayoutGroup _horizontalLayoutGroup;
    /// <summary>
    /// 是否处于打印机中
    /// </summary>
    bool _isPrinTing = false;
    /// <summary>
    /// 名字框离边缘间隔
    /// </summary>
    float _nameBoxMarginInterval = 10f;
    /// <summary>
    /// 说话前是否等待
    /// </summary>
    bool _roleSpeakAeforeAwait = false;
    /// <summary>
    /// 是否接受点击
    /// </summary>
    [SerializeField]
    bool _isAcceptClick = true;
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
    /// 初始化对话框
    /// </summary>
    public virtual void Initial(Action endAction, Action<ChapterDialogueTextDefine> roleSpeakAeforeAction = null, Action<ChapterDialogueTextDefine> roleSpeakRearAction = null, Action<ChapterDialogueTextDefine> roleDialogueBoxCloseAction = null)
    {
        base.Initial(endAction, roleSpeakAeforeAction, roleSpeakRearAction, roleDialogueBoxCloseAction);
        _bgBoxRect = transform.Find("Box").GetComponent<RectTransform>();
        _horizontalLayoutGroup = _bgBoxRect.Find("Bg").GetComponent<HorizontalLayoutGroup>();
        _showTextRect = _bgBoxRect.Find("Bg/ShowText").GetComponent<RectTransform>();
        _showText = _showTextRect.GetComponent<Text>();
        _bgButton = GetComponent<Button>();
        _nameBoxRect = _bgBoxRect.Find("Name").GetComponent<RectTransform>();
        _nameText = _nameBoxRect.Find("ShowNameText").GetComponent<Text>();

        _iconRect = _bgBoxRect.Find("Bg/Icon").GetComponent<RectTransform>();
        _icon = _iconRect.GetComponent<Image>();
        _nameBoxRect.gameObject.SetActive(false);
        _showText.text = "";
        _bgButton.onClick.RemoveAllListeners();
        _bgButton.onClick.AddListener(() => { ClickBtn(); });
        _bgButton.enabled = _isAcceptClick;
    }
    public override void OpenClickBtn(bool isOpen)
    {
        if (!_isAcceptClick)
        {
            return;
        }
        base.OpenClickBtn(isOpen);
    }
    public override async void Show()
    {
        base.Show();
        _bgBoxRect.localScale = Vector3.zero;
        gameObject.SetActive(true);
        OpenClickBtn(false);
        Debug.Log("Show");
        await ChapterTool.ChangeUiScale(_bgBoxRect, Vector3.one, 0.1f, 10, null, () =>
          {
              OpenClickBtn(true);
          });
        ClickBtn();
    }
    /// <summary>
    /// 展示文字
    /// </summary>
    /// <param name="str"></param>
    /// <param name="nameStr"></param>
    public override async UniTask ShowStr(string str, string nameStr)
    {
        await base.ShowStr(str, nameStr);

        if (string.IsNullOrEmpty(nameStr))
        {
            _nameBoxRect.gameObject.SetActive(false);
        }
        else
        {
            _nameBoxRect.gameObject.SetActive(true);
            _nameText.text = nameStr;
        }
        _isPrinTing = true;
        _cancellationTokenSource = new System.Threading.CancellationTokenSource();
        //说话前回调
        _roleSpeakAeforeAction?.Invoke(_dialogueData._Data);
        await UniTask.WaitUntil(() => !_roleSpeakAeforeAwait);
        //文字打印机
        await ChapterTool.PrinTing(_showText, str, _cancellationTokenSource, ShowEnd);

    }
    /// <summary>
    /// 设置对话框类型
    /// </summary>
    /// <param name="type"></param>
    public override async void SetDialogType(ChapterDialogueTextDefine data)
    {
        base.SetDialogType(data);
        //获取对话框人物名字 设置对话框名字
        string dialogueNameData = ChapterTool.GetChapterFunctionString(data.NameID);
        if (string.IsNullOrEmpty(dialogueNameData))
        {
            _nameBoxRect.gameObject.SetActive(false);
        }
        else
        {
            _nameBoxRect.gameObject.SetActive(true);
            _nameText.text = dialogueNameData;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_nameBoxRect);
        }
        //设置对话框头像
        string iconName = data.RoleIcon;

        if (string.IsNullOrEmpty(iconName))
        {
            _icon.gameObject.SetActive(false);
        }
        else
        {
            Sprite icon = null;
            try
            {
                icon = await ABManager.GetAssetAsync<Sprite>(iconName);
            }
            catch (Exception er)
            {

                Debug.Log("获取对话icon失败:" + iconName);
            }

            _icon.gameObject.SetActive(true);
            _icon.sprite = icon;
            _icon.SetNativeSize();
        }
        SetNameBoxPoint(data.BoxAdditional);
        SetShowTextSize(data.BoxAdditional);
    }
    /// <summary>
    /// 设置名字框位置
    /// </summary>
    /// <param name="type"></param>
    public void SetNameBoxPoint(bool type)
    {
        float bgXRadius = _bgBoxRect.rect.width / 2;
        float nameBoxXRadius = _nameBoxRect.rect.width / 2;

        float iconWidth = _iconRect.rect.width;
        if (_icon.gameObject.activeSelf)
        {
            iconWidth = _iconRect.rect.width;
        }
        else
        {
            iconWidth = _nameBoxMarginInterval;
        }
        float y = _bgBoxRect.rect.height / 2;
        float x = 0;
        if (type)
        {
            x = (-bgXRadius) + nameBoxXRadius + iconWidth;
        }
        else
        {
            x = bgXRadius - nameBoxXRadius - iconWidth;
        }

        _nameBoxRect.localPosition = new Vector3(x, _nameBoxRect.localPosition.y);
    }
    /// <summary>
    /// 设置文本内容展示框尺寸
    /// </summary>
    public void SetShowTextSize(bool leftOrRinght)
    {
        bool isHaveIcon = _iconRect.gameObject.activeSelf;
        if (isHaveIcon)
        {
            if (leftOrRinght)
            {
                _horizontalLayoutGroup.childAlignment = TextAnchor.LowerLeft;
                _iconRect.SetAsFirstSibling();
            }
            else
            {
                _horizontalLayoutGroup.childAlignment = TextAnchor.LowerRight;
                _iconRect.SetAsLastSibling();
            }
            _showTextRect.sizeDelta = new Vector2((_bgBoxRect.rect.width - (_iconRect.rect.width + _horizontalLayoutGroup.spacing)) * 0.9f, _bgBoxRect.rect.height - _horizontalLayoutGroup.padding.bottom);
        }
        else
        {
            _horizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            _showTextRect.sizeDelta = new Vector2(_bgBoxRect.rect.width * 0.8f, _bgBoxRect.rect.height * 0.8f);
        }
    }
    /// <summary>
    /// 打印机完毕
    /// </summary>
    public void ShowEnd()
    {
        _roleSpeakRearAction?.Invoke(_dialogueData._Data);
        if (_dialogueData._IsEnd)
        {
            _isEnd = true;
        }
        else
        {
            _dialogueData = new SingleDialogueData(_dialogueData._Data.NextDialogId);
        }
        _isPrinTing = false;

    }
    /// <summary>
    /// 点击背景
    /// </summary>
    public override async UniTask ClickBtn()
    {
        base.ClickBtn();

        if (_isPrinTing)
        {
            if (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
              
            }

            _showText.text = ChapterTool.GetDialogueString(_dialogueData._Data);
            ShowEnd();
        }
        else
        {
            if (_isEnd)
            {
                _endAction?.Invoke();
            }
            else
            {
                SetDialogType(_dialogueData._Data);

                string dialogueNameData = ChapterTool.GetChapterFunctionString(_dialogueData._Data.NameID);
                await ShowStr(ChapterTool.GetDialogueString(_dialogueData._Data), dialogueNameData);
            }
        }
    }
    /// <summary>
    /// 设置是否等待说话前回调
    /// </summary>
    /// <param name="isAwait"></param>
    public void SetRoleSpeakAeforeAwait(bool isAwait)
    {
        _roleSpeakAeforeAwait = isAwait;
    }
    #endregion
}
