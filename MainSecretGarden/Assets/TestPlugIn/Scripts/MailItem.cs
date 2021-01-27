using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 邮箱item
/// </summary>
public class MailItem : MonoBehaviour, InterfaceScrollCell
{
    #region 字段
    /// <summary>
    /// Item点击按钮
    /// </summary>
    Button _btn;
    /// <summary>
    /// 邮件icon
    /// </summary>
    Image _icon;
    /// <summary>
    /// 邮件标签
    /// </summary>
    Image _iconLabel;
    /// <summary>
    /// 标题
    /// </summary>
    Text _titleText;
    /// <summary>
    /// 内容
    /// </summary>
    Text _mailContentText;
    /// <summary>
    /// 到期时间显示
    /// </summary>
    Text _timeText;
    /// <summary>
    /// 当前按钮数据
    /// </summary>
    MailData _currData;
    /// <summary>
    /// 红点
    /// </summary>
    RectTransform _redDotRect;
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
    /// <summary>
    /// 初始化组件
    /// </summary>
    void Initial()
    {
        _btn = GetComponent<Button>();
        /// 邮件icon
        _icon = transform.Find("IconBox/Icon").GetComponent<Image>();
        /// 邮件标签
        _iconLabel = transform.Find("ContentBox/TitleBox/Label").GetComponent<Image>();
        /// 标题
        _titleText = transform.Find("ContentBox/TitleBox/Text").GetComponent<Text>();
        /// 内容
        _mailContentText = transform.Find("ContentBox/MailContent/Text").GetComponent<Text>();
        /// 到期时间显示
        _timeText = transform.Find("Tail/Text").GetComponent<Text>();

        _redDotRect = transform.Find("RedDot").GetComponent<RectTransform>();
        _btn.onClick.RemoveListener(OnClickBtn);
        _btn.onClick.AddListener(OnClickBtn);

        _isInitial = true;
    }
    /// <summary>
    /// 按钮点击
    /// </summary>
    private void OnClickBtn()
    {
        MailboxController.Instance.OpenMail(_currData._mailID);
    }

    /// <summary>
    /// 设置item数据
    /// </summary>
    /// <param name="idx"></param>
    public void ScrollCellIndex(int idx)
    {
        if (!_isInitial)
        {
            Initial();
        }
        _currDataIndex = idx;
        if (_currDataIndex == -1)
        {
            return;
        }
        if (_currData != null)
        {
            MailboxController.Instance.CloseCountDown(_currData._mailID);
        }
        _currData = MailboxController.Instance.GetItemData(idx);


        if (_currData != null)
        {
            ShowData(_currData);
        }
    }
    /// <summary>
    /// 显示邮件数据
    /// </summary>
    /// <param name="mailData"></param>
    async void ShowData(MailData mailData)
    {
        _titleText.text = mailData._mailName;

        _mailContentText.text = mailData._subhead;// MailboxTool.GetStringInFrontOfAFew(mailData._mailContent, 10);
        //设置icon
        if (mailData._type == Game.Protocal.MailState.UnReadState)
        {
            _icon.sprite = await ABManager.GetAssetAsync<Sprite>("youjian_yjwd_icon");
            OpenRedDot(true);
        }
        else
        {
            _icon.sprite = await ABManager.GetAssetAsync<Sprite>("youjian_yjyd_icon");
            OpenRedDot(false);
        }
        //设置附件标签
        if (mailData._isHaveAccessory)
        {
            switch (mailData._type)
            {
                case Game.Protocal.MailState.None:
                case Game.Protocal.MailState.UnReadState:
                case Game.Protocal.MailState.ReadUnAlreadyState:
                case Game.Protocal.MailState.DeleteState:
                    _iconLabel.sprite = await ABManager.GetAssetAsync<Sprite>("youjian_fujian_icon");
                    OpenRedDot(true);
                    break;
                case Game.Protocal.MailState.ReadAlreadyState:
                    _iconLabel.sprite = await ABManager.GetAssetAsync<Sprite>("youjian_fujian_icon_2");
                    OpenRedDot(false);
                    break;
            }
            _iconLabel.gameObject.SetActive(true);

        }
        else
        {
            _iconLabel.gameObject.SetActive(false);
        }
        MailboxController.Instance.SetCountDown(_currData._mailID, _timeText);
    }

    public void OpenRedDot(bool isOpen)
    {
        _redDotRect.gameObject.SetActive(isOpen);
    }

    #endregion
}
