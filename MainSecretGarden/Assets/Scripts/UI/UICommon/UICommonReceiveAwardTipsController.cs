using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Game.Protocal;
using Company.Cfg;
/// <summary>
/// 通用物品领取提示
/// </summary>
public class UICommonReceiveAwardTipsController : MonoBehaviour
{

    #region 变量
    /// <summary>
    /// 遮罩点击按钮
    /// </summary>
    Button _maskBtn;
    /// <summary>
    /// boxRect
    /// </summary>
    RectTransform _backgroundImageRect;
    /// <summary>
    /// 标题
    /// </summary>
    Text _titleText;
    /// <summary>
    /// item展示盒子
    /// </summary>
    RectTransform _showItemBoxRect;
    /// <summary>
    /// item展示滑动组件
    /// </summary>
    ScrollRect _showItemScrollRect;
    /// <summary>
    /// 底部按钮盒子
    /// </summary>
    RectTransform _bottomRect;
    /// <summary>
    /// 取消按钮
    /// </summary>
    Button _btnCancel;
    /// <summary>
    /// 取消按钮文字显示
    /// </summary>
    Text _btnCancelText;
    /// <summary>
    /// 确认按钮
    /// </summary>
    Button _btnAffirm;
    /// <summary>
    /// 确认按钮文字显示
    /// </summary>
    Text _btnAffirmText;
    /// <summary>
    /// item 克隆母体
    /// </summary>
    Transform _itemCloning;
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    bool _isInitial = false;
    /// <summary>
    /// 确认点击回调
    /// </summary>
    Action _affirmAction;
    /// <summary>
    /// 取消点击回调
    /// </summary>
    Action _cancelAction;
    #endregion

    #region 方法

    private void Awake()
    {
        if (!_isInitial)
        {
            Initial();
        }
        if (_backgroundImageRect != null)
            UniversalTool.ReadyPopupAnim(_backgroundImageRect);
    }

    private void OnEnable()
    {
        if (_backgroundImageRect != null)
            UniversalTool.StartPopupAnim(_backgroundImageRect);
    }
    private void OnDisable()
    {
        if (_backgroundImageRect != null)
            UniversalTool.ReadyPopupAnim(_backgroundImageRect);
    }
    /// <summary>
    /// 初始化组件
    /// </summary>
    private void Initial()
    {
        _maskBtn = transform.Find("BG").GetComponent<Button>();
        _backgroundImageRect = transform.Find("BackgroundImage").GetComponent<RectTransform>();
        _titleText = _backgroundImageRect.Find("TitleText").GetComponent<Text>();
        _showItemBoxRect = _backgroundImageRect.Find("ShowItemBox").GetComponent<RectTransform>();
        _showItemScrollRect = _showItemBoxRect.Find("Scroll View").GetComponent<ScrollRect>();
        _bottomRect = _backgroundImageRect.Find("Bottom").GetComponent<RectTransform>();
        _btnCancel = _bottomRect.Find("But_Cancel").GetComponent<Button>();
        _btnCancelText = _btnCancel.transform.Find("Text").GetComponent<Text>();
        _btnAffirm = _bottomRect.Find("But_Affirm").GetComponent<Button>();
        _btnAffirmText = _btnAffirm.transform.Find("Text").GetComponent<Text>();
        _itemCloning = _backgroundImageRect.Find("Item");

        _btnAffirm.onClick.RemoveAllListeners();
        _btnAffirm.onClick.AddListener(ClickAffirmBtn);

        _btnCancel.onClick.RemoveAllListeners();
        _btnCancel.onClick.AddListener(ClickCancelBtn);

        _maskBtn.onClick.RemoveAllListeners();
        _maskBtn.onClick.AddListener(ClickCancelBtn);
        _isInitial = true;
    }
    /// <summary>
    /// 展示弹窗
    /// </summary>
    /// <param name="titelStr">弹窗标题 传空字符串隐藏标题</param>
    /// <param name="affirmBtnName">确认按钮显示文字 传空字符串隐藏确认按钮</param>
    /// <param name="cancelBtnName">取消按钮文字 传空字符串隐藏取消按钮</param>
    /// <param name="affirmAction">确认回调</param>
    /// <param name="cancelAction">取消回调</param>
    /// <param name="datas">需要展示的物品数据</param>
    public void Show(string titelStr, string affirmBtnName, string cancelBtnName, Action affirmAction, Action cancelAction, List<CSWareHouseStruct> datas)
    {
        if (_isInitial)
        {
            Initial();
        }
        _titleText.gameObject.SetActive(!string.IsNullOrEmpty(titelStr));
        _titleText.text = titelStr;
        _btnAffirm.gameObject.SetActive(!string.IsNullOrEmpty(affirmBtnName));
        _btnAffirmText.text = affirmBtnName;
        _btnCancel.gameObject.SetActive(!string.IsNullOrEmpty(cancelBtnName));
        _btnCancelText.text = cancelBtnName;
        _cancelAction = null;
        _affirmAction = null;

        if (datas == null && datas.Count <= 0)
        {
            return;
        }
        _cancelAction = cancelAction;
        _affirmAction = affirmAction;

        CreationItem(datas);
    }
    /// <summary>
    /// 创建item
    /// </summary>
    /// <param name="datas"></param>
    void CreationItem(List<CSWareHouseStruct> datas)
    {
        for (int i = 0; i < datas.Count; i++)
        {
            CSWareHouseStruct data = datas[i];
            UICommonReceiveAwardTipsItem item = new UICommonReceiveAwardTipsItem();
            GameObject itemObj = GameObject.Instantiate(_itemCloning.gameObject, _showItemScrollRect.content);
            item.Initial(itemObj.transform, data);
        }
    }


    /// <summary>
    /// 点击取消
    /// </summary>
    private void ClickCancelBtn()
    {
        if (_backgroundImageRect != null)
            UniversalTool.CancelPopAnim(_backgroundImageRect,()=> {
                _cancelAction?.Invoke();
                UIComponent.RemoveUI(UIType.UICommonReceiveAwardTips);
            });
      
    }
    /// <summary>
    /// 点击确认
    /// </summary>
    private void ClickAffirmBtn()
    {
        if (_backgroundImageRect != null)
            UniversalTool.CancelPopAnim(_backgroundImageRect, () => {
                _affirmAction?.Invoke();
                UIComponent.RemoveUI(UIType.UICommonReceiveAwardTips);
            });
       
    }
    #endregion
}
/// <summary>
/// 通用领取奖励弹窗item类
/// </summary>
public class UICommonReceiveAwardTipsItem
{

    #region 字段
    /// <summary>
    /// item 对象
    /// </summary>
    Transform _tra;
    /// <summary>
    /// icon
    /// </summary>
    Image _icon;
    /// <summary>
    /// 数量展示
    /// </summary>
    Text _showNumber;
    /// <summary>
    /// 当前item 数据
    /// </summary>
    CSWareHouseStruct _currData;
    #endregion
    #region 函数
    /// <summary>
    /// 初始化item
    /// </summary>
    /// <param name="tra"></param>
    /// <param name="data"></param>
    public void Initial(Transform tra, CSWareHouseStruct data)
    {
        _tra = tra;
        _currData = data;
        _icon = _tra.Find("Icon").GetComponent<Image>();
        _showNumber = _tra.Find("Text").GetComponent<Text>();
        Show();
    }
    /// <summary>
    /// 展示数据
    /// </summary>
    public async void Show()
    {
        GameItemDefine dataDefine = WarehouseTool.GetGameItemData(_currData.GoodId);
        if (dataDefine != null)
        {
            Sprite icon = null;
            try
            {
                icon = await ABManager.GetAssetAsync<Sprite>(dataDefine.Icon);
            }
            catch (Exception er)
            {
                Debug.Log("获取该道具icon失败：" + _currData.GoodId);
            }

            _icon.sprite = icon;
            _showNumber.text = _currData.GoodNum.ToString();
            _tra.gameObject.SetActive(true); 
        }
        else
        {
            Debug.Log("没有该道具配置：" + _currData.GoodId);
        }

    }
    #endregion
}