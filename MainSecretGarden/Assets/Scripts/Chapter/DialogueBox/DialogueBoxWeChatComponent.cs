using System;
using System.Collections;
using System.Collections.Generic;
using Company.Cfg;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 微信聊天
/// </summary>
public class DialogueBoxWeChatComponent : DialogueBoxBasics
{
    #region 函数
    CanvasGroup _thisCanvasGroup;
    /// <summary>
    /// 多人对话数据
    /// </summary>
    MultiDialogueData _multiDialogueData;
    /// <summary>
    /// 主角id
    /// </summary>
    [SerializeField]
    int _protagonistID;
    /// <summary>
    /// 聊天对象id
    /// </summary>
    [SerializeField]
    int _anotherId;
    /// <summary>
    /// 是否正常退出
    /// </summary>
    [SerializeField]
    public bool _isOut = true;
    /// <summary>
    /// 自身Rect
    /// </summary>
    RectTransform _thisRect;
    /// <summary>
    /// 顶部栏
    /// </summary>
    RectTransform _topRect;
    /// <summary>
    /// 名字展示
    /// </summary>
    Text _showNameText;
    /// <summary>
    /// 退出按钮
    /// </summary>
    Button _outBtn;
    Transform _guidanceTra;
    /// <summary>
    /// 内容栏
    /// </summary>
    RectTransform _chatWindowRect;
    /// <summary>
    /// 滑动组件
    /// </summary>
    ScrollRect _scrollRect;
    /// <summary>
    /// 对话item生成位置
    /// </summary>
    RectTransform _contentRect;
    /// <summary>
    /// 底部栏
    /// </summary>
    RectTransform _bottomRect;
    /// <summary>
    /// 底部栏整体控制组件
    /// </summary>
    CanvasGroup _bottomCanvasGroup;
    /// <summary>
    /// 底部栏功能栏
    /// </summary>
    RectTransform _controlStripRect;
    /// <summary>
    /// 文字形式按钮盒子
    /// </summary>
    RectTransform _btnBoxRect;
    /// <summary>
    /// 图片形式按钮盒子
    /// </summary>
    RectTransform _ImagBoxRect;
    /// <summary>
    /// 文字形式按钮母体
    /// </summary>
    RectTransform _btnItem;
    /// <summary>
    /// 图片形式按钮母体
    /// </summary>
    RectTransform _imageBtnItem;
    /// <summary>
    /// 信息item母体
    /// </summary>
    RectTransform _messageItem;
    /// <summary>
    /// 文字按钮itme集合
    /// </summary>
    List<DialogueBoxWeChatTextBtnItem> _currTextItemBtns = new List<DialogueBoxWeChatTextBtnItem>();
    /// <summary>
    /// 图片按钮集合
    /// </summary>
    List<DialogueBoxWeChatImageBtnItem> _currImageBtns = new List<DialogueBoxWeChatImageBtnItem>();
    /// <summary>
    /// 聊天信息item集合
    /// </summary>
    List<DialogueBoxWeChatMessageItem> _dialogueBoxWeChatMessageItems = new List<DialogueBoxWeChatMessageItem>();

    Action<ChapterDialogueTextDefine> _imageItemCkickAction;
    /// <summary>
    /// 是否可以点击
    /// </summary>
    bool _isClikc = true;
    /// <summary>
    /// 接话间隔
    /// </summary>
    float _replyInterval = 500f;
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

    public override void Initial(Action endAction, Action<ChapterDialogueTextDefine> roleSpeakAeforeAction = null, Action<ChapterDialogueTextDefine> roleSpeakRearAction = null, Action<ChapterDialogueTextDefine> roleDialogueBoxCloseAction = null, Action<ChapterDialogueTextDefine> roleBoxCloseAeforeAction = null)
    {
        base.Initial(endAction, roleSpeakAeforeAction, roleSpeakRearAction, roleDialogueBoxCloseAction, roleBoxCloseAeforeAction);
        _thisRect = GetComponent<RectTransform>();
        _topRect = transform.Find("top").GetComponent<RectTransform>();
        _guidanceTra = _topRect.Find("ChapterClickGuidance");
        _outBtn = _topRect.Find("back_btn").GetComponent<Button>();
        _showNameText = _outBtn.transform.Find("name").GetComponent<Text>();

        _chatWindowRect = transform.Find("chatWindow").GetComponent<RectTransform>();
        _scrollRect = _chatWindowRect.GetComponent<ScrollRect>();
        _contentRect = _scrollRect.content;

        _bottomRect = transform.Find("bottom").GetComponent<RectTransform>();
        _bottomCanvasGroup = _bottomRect.GetComponent<CanvasGroup>();
        _controlStripRect = _bottomRect.Find("ControlStrip").GetComponent<RectTransform>();
        _btnBoxRect = _bottomRect.Find("BtnBox").GetComponent<RectTransform>();
        _ImagBoxRect = _bottomRect.Find("ImagBox").GetComponent<RectTransform>();

        _btnItem = transform.Find("BtnItem").GetComponent<RectTransform>();
        _imageBtnItem = transform.Find("ImageBtnItem").GetComponent<RectTransform>();
        _messageItem = transform.Find("MessageItem").GetComponent<RectTransform>();

        _thisCanvasGroup = GetComponent<CanvasGroup>();

        _outBtn.onClick.RemoveAllListeners();
        _outBtn.onClick.AddListener(ClickOut);
        _imageItemCkickAction = roleDialogueBoxCloseAction;
        _outBtn.transform.Find("back_image").GetComponent<Image>().color = new Color32(255, 255, 255, 127);
    }



    /// <summary>
    /// 设置初始id 和主角id,聊天对象id
    /// </summary>
    /// <param name="starId">初始对话id</param>
    /// <param name="protagonistID">主角id</param>
    /// <param name="anotherId">聊天对象id</param>
    public void SetInitialData(int starId, int protagonistID, int anotherId)
    {
        _startDialogueId = starId;
        _protagonistID = protagonistID;
        _anotherId = anotherId;
        SetProtagonistName();
        _dialogueData = new SingleDialogueData(_startDialogueId);
        _multiDialogueData = new MultiDialogueData(_startDialogueId);
        Show();
    }
    /// <summary>
    /// 设置聊天主角
    /// </summary>
    public void SetProtagonistName()
    {
        _showNameText.text = ChapterTool.GetChapterFunctionString(_anotherId);
    }
    /// <summary>
    /// 默认展示第一句
    /// </summary>
    public override void Show()
    {
        base.Show();
        OpenOutBtn(false);
        List<ChapterDialogueTextDefine> datas = new List<ChapterDialogueTextDefine>();

        if (_dialogueData._Data.NextDialogId == 0 && (_dialogueData._Data.DialogIdList != null && _dialogueData._Data.DialogIdList.Count > 0))
        {
            for (int i = 0; i < _dialogueData._Data.DialogIdList.Count; i++)
            {
                datas.Add(ChapterTool.GetChapterData(_dialogueData._Data.DialogIdList[i]));
            }

        }
        else
        {
            datas.Add(_dialogueData._Data);
        }
        if (IsProtagonist(_dialogueData._Data.NameID))
        {
            CreationBtn(datas);
        }
        else
        {
            Show(_dialogueData._Data);
        }

    }
    public void CreationBtn(List<ChapterDialogueTextDefine> datas)
    {
        if (datas[0].IsImage)
        {
            _ImagBoxRect.gameObject.SetActive(true);
            _btnBoxRect.gameObject.SetActive(false);
            CreationImageBtn(datas);
        }
        else
        {
            _ImagBoxRect.gameObject.SetActive(false);
            _btnBoxRect.gameObject.SetActive(true);
            CreationTextBtn(datas);
        }
    }
    /// <summary>
    /// 创建文字按钮
    /// </summary>
    /// <param name="data"></param>
    public DialogueBoxWeChatTextBtnItem CreationTextBtn(ChapterDialogueTextDefine data)
    {
        RectTransform itemTra = GameObject.Instantiate(_btnItem.gameObject, _btnBoxRect).GetComponent<RectTransform>();
        DialogueBoxWeChatTextBtnItem item = new DialogueBoxWeChatTextBtnItem();
        item.Initial(itemTra, data, ClickItem);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_btnBoxRect);
        return item;
    }
    /// <summary>
    /// 创建文字按钮
    /// </summary>
    /// <param name="data"></param>
    public async void CreationTextBtn(List<ChapterDialogueTextDefine> datas)
    {
        ClearItemBtn();
        for (int i = 0; i < datas.Count; i++)
        {
            DialogueBoxWeChatTextBtnItem item = CreationTextBtn(datas[i]);
            _currTextItemBtns.Add(item);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(_btnBoxRect);
        await RefreshPanel(0);
    }
    /// <summary>
    /// 创建图片按钮
    /// </summary>
    /// <param name="data"></param>
    public DialogueBoxWeChatImageBtnItem CreationImageBtn(ChapterDialogueTextDefine data)
    {
        RectTransform itemTra = GameObject.Instantiate(_imageBtnItem.gameObject, _ImagBoxRect).GetComponent<RectTransform>();
        DialogueBoxWeChatImageBtnItem dialogueBoxWeChatImageBtnItem = new DialogueBoxWeChatImageBtnItem();
        dialogueBoxWeChatImageBtnItem.Initial(itemTra, data, ClickItem);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_ImagBoxRect);
        return dialogueBoxWeChatImageBtnItem;
    }
    /// <summary>
    /// 创建图片按钮
    /// </summary>
    /// <param name="data"></param>
    public async void CreationImageBtn(List<ChapterDialogueTextDefine> datas)
    {
        ClearItemBtn();
        for (int i = 0; i < datas.Count; i++)
        {
            DialogueBoxWeChatImageBtnItem item = CreationImageBtn(datas[i]);
            _currImageBtns.Add(item);
            await UniTask.DelayFrame(1);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(_ImagBoxRect);
        await UniTask.DelayFrame(1);
        Debug.Log("图片按钮Box尺寸：" + _ImagBoxRect.sizeDelta);
        await RefreshPanel(1);
    }
    /// <summary>
    /// 清理旧item按钮
    /// </summary>
    public void ClearItemBtn()
    {
        for (int i = 0; i < _currTextItemBtns.Count; i++)
        {
            _currTextItemBtns[i].Dispose();
        }
        for (int i = 0; i < _currImageBtns.Count; i++)
        {
            _currImageBtns[i].Dispose();
        }
        _currImageBtns.Clear();
        _currTextItemBtns.Clear();
    }

    /// <summary>
    /// 刷新底部栏高度
    /// </summary>
    /// <param name="type"></param>
    public async UniTask RefreshBottom(int type)
    {
        RectTransform tageRect = null;
        switch (type)
        {
            case 0:
                tageRect = _btnBoxRect;
                break;
            case 1:
                tageRect = _ImagBoxRect;
                break;
            case 2:
                Debug.Log("还原底部栏");
                break;
        }
        float height = 0;
        if (tageRect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(tageRect);
            Debug.Log("底部按钮内容栏高：" + tageRect.sizeDelta);
            await UniTask.DelayFrame(1);
            height = _controlStripRect.sizeDelta.y + tageRect.sizeDelta.y;
        }
        else
        {
            height = _controlStripRect.sizeDelta.y;
        }
        RefreshContentRectSize(height);
        await ChapterTool.ChangeUiSize(_bottomRect, new Vector2(_bottomRect.sizeDelta.x, height), 50f, 0.01f, null, () => { Debug.Log("尺寸更改完毕"); });
    }
    /// <summary>
    /// 刷新内容栏尺寸
    /// </summary>
    public async void RefreshContentRectSize(float bottomRectHeight)
    {
        float a = 1242f * Screen.height / Screen.width;

        //最大高度
        float maxheight = a;// _thisRect.rect.height;
        float height = maxheight - _topRect.sizeDelta.y - bottomRectHeight;
        await ChapterTool.ChangeUiSize(_chatWindowRect, new Vector2(_chatWindowRect.sizeDelta.x, height), 100f, 0.01f, null, () =>
        {
            _scrollRect.verticalScrollbar.value = 0;
        });
    }
    /// <summary>
    /// 刷新面板
    /// </summary>
    /// <param name="type"></param>
    /// <param name="endAction"></param>
    /// <returns></returns>
    public async UniTask RefreshPanel(int type, Action endAction = null)
    {
        _bottomCanvasGroup.blocksRaycasts = false;
        await RefreshBottom(type);
        endAction?.Invoke();
        _bottomCanvasGroup.blocksRaycasts = true;
    }
    /// <summary>
    /// 底部按钮点击
    /// </summary>
    /// <param name="data"></param>
    public async void ClickItem(ChapterDialogueTextDefine data)
    {
        Debug.Log("底部按钮点击");

        await RefreshPanel(2, () =>
        {
            Show(data);
            ClearItemBtn();
            _btnBoxRect.gameObject.SetActive(false);
            _ImagBoxRect.gameObject.SetActive(false);
        });
    }
    /// <summary>
    /// 接下一句
    /// </summary>
    /// <param name="datas"></param>
    public void ConnectTheNextLine(List<ChapterDialogueTextDefine> datas)
    {
        if (IsProtagonist(datas[0].NameID))
        {
            CreationBtn(datas);
        }
        else
        {
            Show(datas[0]);
        }
    }
    /// <summary>
    /// 展示聊天数据
    /// </summary>
    /// <param name="data"></param>
    public override async UniTask Show(ChapterDialogueTextDefine data)
    {
        await base.Show(data);
        DialogueBoxWeChatMessageItem item = CreationDialogueBoxWeChatMessageItem(data);
        _dialogueBoxWeChatMessageItems.Add(item);
        await UniTask.Delay(TimeSpan.FromMilliseconds(300));
        _roleSpeakRearAction?.Invoke(data);
        //_scrollRect.verticalScrollbar.value = 0;
        await BottomOut();
        if (data.IsClick)
        {
            _isClikc = false;
        }
        //Debug.Log("关闭旧对话");
        await UniTask.WaitUntil(() => _isClikc);

        if (data.NextDialogId == 0 && (data.DialogIdList == null || data.DialogIdList.Count <= 0))
        {
            Debug.Log("对话结束");
            if (_isOut)
            {
                OpenOutBtn(true);
            }
            else
            {
                _endAction?.Invoke();
            }

        }
        else
        {
            List<ChapterDialogueTextDefine> datas = new List<ChapterDialogueTextDefine>();

            if (data.NextDialogId != 0)
            {
                datas.Add(ChapterTool.GetChapterData(data.NextDialogId));
            }
            else if (data.DialogIdList != null && data.DialogIdList.Count >= 0)
            {
                for (int i = 0; i < data.DialogIdList.Count; i++)
                {
                    ChapterDialogueTextDefine dataDefine = ChapterTool.GetChapterData(data.DialogIdList[i]);
                    datas.Add(dataDefine);
                }
            }
            await UniTask.Delay(TimeSpan.FromMilliseconds(_replyInterval));
            ConnectTheNextLine(datas);
        }

    }
    /// <summary>
    /// 创建信息item
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public DialogueBoxWeChatMessageItem CreationDialogueBoxWeChatMessageItem(ChapterDialogueTextDefine data)
    {
        RectTransform itemTra = GameObject.Instantiate(_messageItem.gameObject, _contentRect).GetComponent<RectTransform>();
        DialogueBoxWeChatMessageItem dialogueBoxWeChatMessageItem = new DialogueBoxWeChatMessageItem();
        dialogueBoxWeChatMessageItem.Initial(itemTra, IsProtagonist(data.NameID), data, ClickIcon, ImageClick);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRect);
        return dialogueBoxWeChatMessageItem;
    }
    /// <summary>
    /// 点击头像
    /// </summary>
    /// <param name="data"></param>
    public void ClickIcon(ChapterDialogueTextDefine data)
    {

    }
    /// <summary>
    /// 是否是主角
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool IsProtagonist(int id)
    {
        return id == _protagonistID;
    }
    /// <summary>
    /// 点击退出
    /// </summary>
    private void ClickOut()
    {
        UIComponent.RemoveUI(UIType.DialogueBox_WeChat);
        _guidanceTra.gameObject.SetActive(false);
        _endAction?.Invoke();
    }
    public void OpenOutBtn(bool isOpen)
    {
        if (isOpen)
        {
            _outBtn.transform.Find("back_image").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            _guidanceTra.gameObject.SetActive(true);
        }

        _outBtn.enabled = isOpen;
    }
    /// <summary>
    /// 收起底部栏
    /// </summary>
    /// <returns></returns>
    public async UniTask BottomOut()
    {
        float speed = 0.1f;
        while (_scrollRect.verticalScrollbar.value != 0)
        {
            speed += 0.05f;
            _scrollRect.verticalScrollbar.value = Mathf.Lerp(_scrollRect.verticalScrollbar.value, 0, speed);
            await UniTask.Delay(TimeSpan.FromMilliseconds(0.001f));
        }

    }
    /// <summary>
    /// 暂停退出
    /// </summary>
    public async void PauseOut(bool isFadeOut, int alpha = 0)
    {
        _isClikc = false;
        if (isFadeOut)
        {
            await ChapterTool.FadeInFadeOut(_thisCanvasGroup, 0, 0.1f, null, () =>
             {
                 _thisCanvasGroup.interactable = false;
                 _thisCanvasGroup.blocksRaycasts = false;
             });
        }
        else
        {
            _thisCanvasGroup.alpha = alpha;
            _thisCanvasGroup.interactable = false;
            _thisCanvasGroup.blocksRaycasts = false;
        }
    }
    /// <summary>
    /// 重新进入
    /// </summary>
    /// <param name="isFadeOut"></param>
    public async void PauseIn(bool isFadeOut)
    {
        _isClikc = true;
        if (isFadeOut)
        {
            await ChapterTool.FadeInFadeOut(_thisCanvasGroup, 1, 0.1f, null, () =>
            {
                _thisCanvasGroup.interactable = true;
                _thisCanvasGroup.blocksRaycasts = true;
            });
        }
        else
        {
            _thisCanvasGroup.alpha = 1;
            _thisCanvasGroup.interactable = true;
            _thisCanvasGroup.blocksRaycasts = true;
        }
    }
    /// <summary>
    /// 点击图片
    /// </summary>
    /// <param name="data"></param>
    public void ImageClick(ChapterDialogueTextDefine data)
    {
        _imageItemCkickAction?.Invoke(data);
    }
    #endregion
}
