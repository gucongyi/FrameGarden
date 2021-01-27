using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 聊天界面私聊头像按钮
/// </summary>
public class PrivateChatIconItemController : MonoBehaviour
{
    #region 字段
    /// <summary>
    /// 勾选按钮
    /// </summary>
    Toggle _toggle;
    /// <summary>
    /// 玩家名字显示
    /// </summary>
    Text _showNameText;
    /// <summary>
    /// 玩家icon
    /// </summary>
    Image _icon;
    /// <summary>
    /// 等级
    /// </summary>
    Transform _gradeTra;
    /// <summary>
    /// 等级显示
    /// </summary>
    Text _showGradeText;
    /// <summary>
    /// 有未读消息提示
    /// </summary>
    Transform _updateLabelTra;
    /// <summary>
    /// 玩家数据
    /// </summary>
    PrivateChatSaveInfo _data;
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    bool _isInitial = false;
    #endregion
    #region 属性
    /// <summary>
    /// 玩家数据
    /// </summary>
    public PrivateChatSaveInfo _Data { get { return _data; } }
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
    /// 组件初始化
    /// </summary>
    void Initial()
    {
        _toggle = GetComponent<Toggle>();
        _showNameText = transform.Find("Label").GetComponent<Text>();
        _icon = transform.Find("IconBox/IconBg/Icon").GetComponent<Image>();
        _gradeTra = transform.Find("IconBox/Grade");
        _showGradeText = _gradeTra.Find("Text").GetComponent<Text>();
        _updateLabelTra = transform.Find("IconBox/UpdateLabel");
        _toggle.onValueChanged.RemoveAllListeners();
        _toggle.onValueChanged.AddListener(ClickToggle);
        _isInitial = true;
    }
    /// <summary>
    /// 设置按钮数据
    /// </summary>
    /// <param name="data"></param>
    public void SetData(PrivateChatSaveInfo data, ToggleGroup toggleGroup)
    {
        if (!_isInitial)
        {
            Initial();
        }
        _data = data;
        _toggle.group = toggleGroup;
        gameObject.SetActive(true);
        RefreshDataShow();
    }
    /// <summary>
    /// 刷新数据
    /// </summary>
    public async void RefreshDataShow()
    {
        string playerName = ChatTool.GetPrivateChatName(_data);
        if (string.IsNullOrEmpty(playerName))
        {
            _showNameText.text = _data._playName;
        }
        else
        {
            _showNameText.text = ChatTool.GetPrivateChatName(_data);
        }
        if (string.IsNullOrEmpty(_data._iconUrl))
        {
            _icon.sprite = ChatTool.GetIcon(_data.iconId);
        }
        else
        {
            _icon.sprite = await ChatTool.GetIcon(_data._iconUrl);
        }

        _showGradeText.text = StaticData.GetPlayerLevelByExp(ChatTool.GetPrivateChatExperience(_data)).ToString();
        _updateLabelTra.gameObject.SetActive(!_data.IsRead);
    }
    /// <summary>
    /// 点击勾选按钮
    /// </summary>
    /// <param name="arg0"></param>
    private void ClickToggle(bool arg0)
    {
        if (arg0)
        {
            _data.IsRead = true;
            ChatTool.UpdatePrivateChatIsRead(_data.PrivateChatRoleUid, true);
            ChatPanelController._Instance.ClickPrivateChat(_data.PrivateChatRoleUid);
            NewMessage(false);
        }
    }
    public void NewMessage(bool isOpen)
    {
        _updateLabelTra.gameObject.SetActive(isOpen);
    }
    public void SetSelcet(bool isSelect)
    {
        _toggle.isOn = isSelect;
        _data.IsRead = true;
        ChatTool.UpdatePrivateChatIsRead(_data.PrivateChatRoleUid, true);
    }
    public void Dispose()
    {
        _data = null;
        Destroy(gameObject);
    }
    #endregion
}
