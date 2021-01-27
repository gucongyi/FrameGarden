using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 聊天玩家详细信息面板
/// </summary>
public class ChatRoleInformationPanelController : MonoBehaviour
{
    #region 字段
    CanvasGroup _thisCanvasGroup;
    /// <summary>
    /// 面板实体
    /// </summary>
    RectTransform _contentRect;
    /// <summary>
    /// 玩家头像icon
    /// </summary>
    Image _icon;
    /// <summary>
    /// 玩家名字
    /// </summary>
    Text _showName;
    /// <summary>
    /// 玩家等级显示
    /// </summary>
    Text _showGrade;
    /// <summary>
    /// 添加好友按钮
    /// </summary>
    Button _makeFriendsBtn;
    /// <summary>
    /// 添加好友按钮Image
    /// </summary>
    Image _makeFriendsBtnImage;
    /// <summary>
    /// 私聊按钮
    /// </summary>
    Button _chatBtn;
    /// <summary>
    /// 私聊按钮image
    /// </summary>
    Image _chatBtnImage;
    /// <summary>
    /// 关闭面板按钮
    /// </summary>
    Button _closeBtn;
    /// <summary>
    /// 星标
    /// </summary>
    RectTransform _asteriskRect;
    /// <summary>
    /// 在线状态图标
    /// </summary>
    Image _typeLabelImage;
    /// <summary>
    /// 在线状态文字
    /// </summary>
    Text _typeText;

    /// <summary>
    /// 当前玩家数据
    /// </summary>
    ChatInfo _currData;
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    bool _isInitial = false;
    #endregion
    #region 函数
    private void Awake()
    {
        if (!_isInitial)
        {
            Initial();
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

    void Initial()
    {
        _thisCanvasGroup = GetComponent<CanvasGroup>();
        _contentRect = transform.Find("Content").GetComponent<RectTransform>();
        _icon = transform.Find("Content/IconBox/IconBox/IconBg/Icon").GetComponent<Image>();
        _showName = transform.Find("Content/Name").GetComponent<Text>();

        _showGrade = transform.Find("Content/IconBox/IconBox/Grade/Text").GetComponent<Text>();
        _makeFriendsBtn = transform.Find("Content/MakeFriends").GetComponent<Button>();
        _makeFriendsBtnImage = _makeFriendsBtn.transform.GetComponent<Image>();
        _chatBtn = transform.Find("Content/Chat").GetComponent<Button>();
        _chatBtnImage = _chatBtn.transform.GetComponent<Image>();
        _closeBtn = transform.Find("CloseBtn").GetComponent<Button>();

        _asteriskRect = transform.Find("Asterisk").GetComponent<RectTransform>();

        _typeLabelImage = _contentRect.Find("TypeLabel").GetComponent<Image>();
        _typeText = _contentRect.Find("TypeText").GetComponent<Text>();
        _makeFriendsBtn.onClick.RemoveAllListeners();
        _makeFriendsBtn.onClick.AddListener(ClickakeFriends);
        _chatBtn.onClick.RemoveAllListeners();
        _chatBtn.onClick.AddListener(ClickChat);
        _closeBtn.onClick.RemoveAllListeners();
        _closeBtn.onClick.AddListener(ClickClose);
        _isInitial = true;
    }
    /// <summary>
    /// 关闭面板
    /// </summary>
    private void ClickClose()
    {
        UIComponent.HideUI(UIType.ChatRoleInformationPanel);
    }
    /// <summary>
    /// 私聊
    /// </summary>
    private void ClickChat()
    {
        Debug.Log("点击私聊");
        ChatPanelController._Instance.OpenPrivateChat(_currData._playUid, _currData._iconId, _currData._iconUrl, _currData._playName, _currData._experience);
        ClickClose();
    }
    /// <summary>
    /// 添加好友
    /// </summary>
    private async void ClickakeFriends()
    {
        if (await StaticData.IsUserFriend(_currData._playUid))
        {
            StaticData.CreateToastTips("对方已经是你的好友了");
        }
        else
        {
            //_makeFriendsBtnImage.color = new Color(255f, 255f, 255f);
            StaticData.ApplyFriendByUid(_currData._playUid);
        }
    }

    public async void Show(ChatInfo data, RectTransform tageTra, RectTransform astrictScope)
    {
        if (!_isInitial)
        {
            Initial();
        }
        _currData = data;
        _thisCanvasGroup.alpha = 0;
        if (string.IsNullOrEmpty(_currData._iconUrl))
        {
            _icon.sprite = ChatTool.GetIcon(_currData._iconId);
        }
        else
        {
            _icon.sprite = await ChatTool.GetIcon(_currData._iconUrl);
        }

        _showName.text = _currData._playName;
        _showGrade.text = StaticData.GetPlayerLevelByExp((int)_currData._experience).ToString();

        //if (await StaticData.IsUserFriend(data._playUid))
        //{
        //    _makeFriendsBtnImage.color = new Color(202f, 202f, 202f);
        //}
        //else
        //{
        //    _makeFriendsBtnImage.color = new Color(255f, 255f, 255f);
        //}

        if (ChatPanelController._Instance == null || (ChatPanelController._Instance._CurrPrivateChatIndex == data._playUid && ChatPanelController._Instance._CurrType == 1))
        {
            //_chatBtnImage.color = new Color(202f, 202f, 202f);
            _chatBtn.enabled = false;
        }
        else
        {
            //_chatBtnImage.color = new Color(255f, 255f, 255f);
            _chatBtn.enabled = true;
        }

        if (StaticData.IsFriendOnline(_currData._playUid))
        {
            _typeText.text = StaticData.GetMultilingual(120110);
            _typeLabelImage.sprite = await ABManager.GetAssetAsync<Sprite>("lt_tips_zx");
        }
        else
        {
            _typeText.text = StaticData.GetMultilingual(120111);
            _typeLabelImage.sprite = await ABManager.GetAssetAsync<Sprite>("lt_tips_lx");
        }

        SetPoint(tageTra, astrictScope);
    }
    /// <summary>
    /// 设置面板位置
    /// </summary>
    /// <param name="astrictScope"></param>
    public void SetPoint(RectTransform tageRect, RectTransform astrictScope)
    {
        Vector3 locaPoint = transform.InverseTransformPoint(tageRect.position);
        float x = locaPoint.x + _contentRect.rect.width / 2;
        float y = locaPoint.y - _contentRect.rect.height / 2;
        Vector3 newVector3 = new Vector3(x, y);
        _contentRect.localPosition = newVector3;
        _asteriskRect.localPosition = new Vector3(_contentRect.localPosition.x - (_contentRect.rect.width / 2) + 5, (_contentRect.localPosition.y + _contentRect.rect.height / 2) - 5);
        bool isBottomOut = ChatTool.AstrictScope(_contentRect, astrictScope);
        if (isBottomOut)
        {
            float yTwo = locaPoint.y + _contentRect.rect.height / 2;
            Debug.Log("触底y轴：" + yTwo);
            _contentRect.localPosition = new Vector3(x, yTwo);
            _asteriskRect.localPosition = new Vector3(_asteriskRect.localPosition.x, (_contentRect.localPosition.y - _contentRect.rect.height / 2) + 5);
        }
        _thisCanvasGroup.alpha = 1;
    }
    #endregion
}
