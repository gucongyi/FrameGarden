using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatItemController : MonoBehaviour, InterfaceScrollCell
{
    #region 字段
    RectTransform _thisRect;
    /// <summary>
    /// icon盒子
    /// </summary>
    Transform _iconBoxTra;
    RectTransform _iconBoxRect;
    /// <summary>
    /// 内容排序组件
    /// </summary>
    HorizontalLayoutGroup _itemBoxGroup;
    /// <summary>
    /// 文本内容box排序组件
    /// </summary>
    VerticalLayoutGroup _contentBoxGroup;
    /// <summary>
    /// 玩家icon头像
    /// </summary>
    Image _icon;
    /// <summary>
    /// 头像点击按钮
    /// </summary>
    Button _iconBtn;
    /// <summary>
    /// 名字
    /// </summary>
    Text _showName;
    /// <summary>
    /// 玩家等级
    /// </summary>
    Transform _gradeTra;
    /// <summary>
    /// 等级显示
    /// </summary>
    Text _gradeShowText;
    /// <summary>
    /// 角标
    /// </summary>
    Transform _cornerTra;
    /// <summary>
    /// 角标图片显示
    /// </summary>
    Image _cirnerImage;
    /// <summary>
    /// 文本内容box
    /// </summary>
    RectTransform _contentBoxTra;
    /// <summary>
    /// 文本内容box
    /// </summary>
    RectTransform _contentBoxTraTwo;
    /// <summary>
    /// 文本内容背景
    /// </summary>
    Image _textBoxImage;
    /// <summary>
    /// 文本内容显示
    /// </summary>
    Text _contentShowText;
    /// <summary>
    /// 时间box
    /// </summary>
    Transform _showTimeTra;
    /// <summary>
    /// 时间显示
    /// </summary>
    Text _showTimeText;
    /// <summary>
    /// 当前按钮数据
    /// </summary>
    ChatInfo _currData;
    /// <summary>
    /// 当前数据下标
    /// </summary>
    int _currDataIndex = -1;
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
    /// <summary>
    /// 初始化组件
    /// </summary>
    void Initial()
    {
        _thisRect = GetComponent<RectTransform>();
        // icon盒子
        _iconBoxTra = transform.Find("IconBox");
        _iconBoxRect = _iconBoxTra.Find("IconBox").GetComponent<RectTransform>();
        // 玩家icon头像
        _icon = _iconBoxTra.Find("IconBox/IconBg/Icon").GetComponent<Image>();
        //玩家头像点击按钮
        _iconBtn = _iconBoxTra.Find("IconBox").transform.GetComponent<Button>();
        // 玩家等级
        _gradeTra = _iconBoxTra.Find("IconBox/Grade");
        // 等级显示
        _gradeShowText = _gradeTra.Find("Text").GetComponent<Text>();
        // 角标
        _cornerTra = transform.Find("ContentBox/Content/ShowContent/Corner");
        // 角标图片显示
        _cirnerImage = _cornerTra.GetComponent<Image>();
        // 文本内容box
        _contentBoxTra = transform.Find("ContentBox").GetComponent<RectTransform>();
        _contentBoxTraTwo = _contentBoxTra.Find("Content").GetComponent<RectTransform>();
        _textBoxImage = _contentBoxTraTwo.GetComponent<Image>();
        _showName = _contentBoxTra.Find("NameBox/Text").GetComponent<Text>();
        // 文本内容显示
        _contentShowText = _contentBoxTraTwo.Find("ShowContent").GetComponent<Text>();
        // 时间box
        _showTimeTra = transform.Find("ShowTime");
        // 时间显示
        _showTimeText = _showTimeTra.Find("ShowTime").GetComponent<Text>();

        _itemBoxGroup = transform.GetComponent<HorizontalLayoutGroup>();
        _contentBoxGroup = _contentBoxTra.GetComponent<VerticalLayoutGroup>();

        _iconBtn.onClick.RemoveAllListeners();
        _iconBtn.onClick.AddListener(ClickIcon);
        _isInitial = true;
    }
    /// <summary>
    /// 点击头像
    /// </summary>
    private async void ClickIcon()
    {
        await StaticData.OpenChatRoleInformationPanel(_currData, _iconBoxRect, ChatPanelController._Instance._ItemBoxTra);
    }
    /// <summary>
    /// 获取数据
    /// </summary>
    /// <param name="index"></param>
    public void ScrollCellIndex(int index)
    {
        _currData = ChatPanelController._Instance.GetCurrChatItemData(index);
        IsPlay();

    }
    /// <summary>
    /// 是否是玩家自己发言
    /// </summary>
    void IsPlay()
    {
        if (_currData.IsTimeIntervalSign)
        {
            ChangeDialogBox(0);
        }
        else
        {
            if (ChatTool.IsPlay(_currData._playUid))
            {
                ChangeDialogBox(2);
            }
            else
            {
                ChangeDialogBox(1);
            }
            ShowData();
        }


    }
    /// <summary>
    /// 展示数据
    /// </summary>
    async void ShowData()
    {
        if (string.IsNullOrEmpty(_currData._iconUrl))
        {
            _icon.sprite = ChatTool.GetIcon(_currData._iconId);
        }
        else
        {
            _icon.sprite = await ChatTool.GetIcon(_currData._iconUrl);
        }

        _showName.text = _currData._playName;
        _gradeShowText.text = StaticData.GetPlayerLevelByExp((int)_currData._experience).ToString();
        _contentShowText.text = ChatTool.TextLineFeed(_currData._message, 25);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_contentBoxTraTwo);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_contentBoxTra);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_thisRect);
        SetCornerTra();
        ChatPanelController._Instance.CoerceItemBoxRefresh();
    }
    async void ChangeDialogBox(int type)
    {
        switch (type)
        {
            //时间间隔
            case 0:
                _itemBoxGroup.childAlignment = TextAnchor.UpperCenter;
                _showTimeTra.gameObject.SetActive(true);
                _contentBoxTra.gameObject.SetActive(false);
                //_cornerTra.gameObject.SetActive(false);
                _iconBoxTra.gameObject.SetActive(false);
                _iconBtn.enabled = false;
                ShowTime();
                break;
            //他人
            case 1:
                _showTimeTra.gameObject.SetActive(false);
                _contentBoxTra.gameObject.SetActive(true);
                _iconBoxTra.gameObject.SetActive(true);
                _itemBoxGroup.childAlignment = TextAnchor.UpperLeft;
                _iconBoxTra.SetSiblingIndex(0);
                _contentBoxTra.SetSiblingIndex(1);
                _contentBoxGroup.childAlignment = TextAnchor.MiddleLeft;
                _showName.alignment = TextAnchor.MiddleLeft;
                _contentShowText.alignment = TextAnchor.MiddleLeft;
                _textBoxImage.sprite = await ABManager.GetAssetAsync<Sprite>("lt_paop_1");
                _contentShowText.color = new Color32(232, 189, 139, 255);
                _iconBtn.enabled = true;
                break;
            //自己
            case 2:
                _showTimeTra.gameObject.SetActive(false);
                _contentBoxTra.gameObject.SetActive(true);
                _iconBoxTra.gameObject.SetActive(true);
                _itemBoxGroup.childAlignment = TextAnchor.UpperRight;
                _contentBoxTra.SetSiblingIndex(0);
                _iconBoxTra.SetSiblingIndex(1);
                _contentBoxGroup.childAlignment = TextAnchor.MiddleRight;
                _showName.alignment = TextAnchor.MiddleRight;
                _contentShowText.alignment = TextAnchor.MiddleLeft;
                _textBoxImage.sprite = await ABManager.GetAssetAsync<Sprite>("lt_paop_2");
                _contentShowText.color = new Color32(66, 123, 212, 255);
                _iconBtn.enabled = false;
                break;
        }
    }
    /// <summary>
    /// 设置角标
    /// </summary>
    void SetCornerTra()
    {
        RectTransform showContentParent = _contentShowText.transform.parent.GetComponent<RectTransform>();
        float x = 0;

        if (ChatTool.IsPlay(_currData._playUid))
        {
            x = showContentParent.rect.width / 2;
        }
        else
        {
            x = -(showContentParent.rect.width / 2);
        }
        _cornerTra.localPosition = new Vector3(x, _cornerTra.localPosition.y);
    }
    /// <summary>
    /// 显示时间
    /// </summary>
    void ShowTime()
    {
        _showTimeText.text = _currData.Time;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_thisRect);
    }
    #endregion
}
