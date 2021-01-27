using Company.Cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 购买体力界面
/// </summary>
public class UIBuyPowerController : MonoBehaviour
{
    #region 变量

    private Transform _bgTra;

    private Text _tits;

    private Image _itemIcon;
    private Text _itemName;

    //购买获得
    private Text _itemObtain;
    private Text _itemObtainValue;

    /// <summary>
    /// 购买剩余次数
    /// </summary>
    private Text _itemRemaining;

    private Text _itemNum;
    private Button _butLess;//减
    private Button _butPlus;//加

    private Button _butCancel;
    private Button _butBuy;
    private Image _butBuyCurrencyIcon;
    private Text _butBuyCurrencyNum;

    private Button _butClose;

    /// <summary>
    /// 物品基础获取值
    /// </summary>
    private int _baseItemObtainNum;

    /// <summary>
    /// 剩余购买次数
    /// </summary>
    private int _buyRemaining;

    /// <summary>
    /// 购买数量
    /// </summary>
    private int _buyItemNum;

    private int _baseBuyCurrencyNum;
    #endregion

    #region 方法

    void Awake()
    {
        _bgTra = transform.Find("BackgroundImage");
        if (_bgTra != null)
            UniversalTool.ReadyPopupAnim(_bgTra);
    }

    private void OnEnable()
    {
        if (_bgTra != null)
            UniversalTool.StartPopupAnim(_bgTra);
    }

    private void OnDisable()
    {
        if (_bgTra != null)
            UniversalTool.ReadyPopupAnim(_bgTra);
    }

    private void Init() 
    {
        if (_tits != null)
            return;

        _butClose = transform.Find("BG").GetComponent<Button>();

        _tits = transform.Find("BackgroundImage/Tips").GetComponent<Text>();

        _itemIcon = transform.Find("BackgroundImage/ItemBG/Icon").GetComponent<Image>();
        _itemName = transform.Find("BackgroundImage/ItemBG/Name").GetComponent<Text>();

        _itemObtain = transform.Find("BackgroundImage/ItemValue").GetComponent<Text>();
        _itemObtainValue = transform.Find("BackgroundImage/ItemValue/Num").GetComponent<Text>();

        _itemRemaining = transform.Find("BackgroundImage/Remaining").GetComponent<Text>();

        _itemNum = transform.Find("BackgroundImage/NumBG/NumBG/Num").GetComponent<Text>();
        _butLess = transform.Find("BackgroundImage/NumBG/Less").GetComponent<Button>();
        _butPlus = transform.Find("BackgroundImage/NumBG/Plus").GetComponent<Button>();

        _butCancel = transform.Find("BackgroundImage/But_Cancel").GetComponent<Button>();
        _butBuy = transform.Find("BackgroundImage/Btn_Buy").GetComponent<Button>();
        _butBuyCurrencyIcon = transform.Find("BackgroundImage/Btn_Buy/TypeButtonIconAndNum/Icon").GetComponent<Image>();
        _butBuyCurrencyNum = transform.Find("BackgroundImage/Btn_Buy/TypeButtonIconAndNum/Num").GetComponent<Text>();
    }

    /// <summary>
    /// 购买回调
    /// </summary>
    private Action<int, int> BuyCallback;
    private Action CancelBuy;

    public void InitValue(Action<int, int> callbakc, Action cancelBuy) 
    {

        BuyCallback = callbakc;
        CancelBuy = cancelBuy;
        _buyItemNum = 1;

        Init();

        int itemID = StaticData.configExcel.GetVertical().PhysicalPower;
        var itemInfo = StaticData.configExcel.GetGameItemByID(itemID);

        _tits.text = LocalizationDefineHelper.GetStringNameById(120014);//体力已经用完啦是否需要补充体力？
        _itemIcon.sprite = ABManager.GetAsset<Sprite>(itemInfo.Icon);
        _itemName.text = LocalizationDefineHelper.GetStringNameById(itemInfo.ItemName);

        _itemObtain.text = _itemName.text + LocalizationDefineHelper.GetStringNameById(120139); //:

        StoreDefine buyInfo = ZillionaireGameMapManager._instance.GetBuyPowerInfo();
        _baseItemObtainNum = buyInfo.GoodNum;
        _itemObtainValue.text = "+" + _baseItemObtainNum;

        _buyRemaining = ZillionaireGameMapManager._instance.GetBuyPowerTime();
        _itemRemaining.text = LocalizationDefineHelper.GetStringNameById(120135) + (_buyRemaining - _buyItemNum);//

        _itemNum.text = _buyItemNum.ToString() ;

        _butLess.onClick.RemoveAllListeners();
        _butLess.onClick.AddListener(OnClickLessBuyNum);

        _butPlus.onClick.RemoveAllListeners();
        _butPlus.onClick.AddListener(OnClickPlusBuyNum);

        //add 策划需求购买体力界面点击黑背景不关闭 ws 2021/1/11 
        //_butClose.onClick.RemoveAllListeners();
        //_butClose.onClick.AddListener(() => { 
        //    UniversalTool.CancelPopAnim(_bgTra, OnClickClose); });

        _butCancel.onClick.RemoveAllListeners();
        _butCancel.onClick.AddListener(() => { 
            UniversalTool.CancelPopAnim(_bgTra, OnClickCancel); });

        _butBuy.onClick.RemoveAllListeners();
        _butBuy.onClick.AddListener(() => { 
            UniversalTool.CancelPopAnim(_bgTra, OnClickBuy); } );

        _baseBuyCurrencyNum = (int)buyInfo.OriginalPrice[0].Count;
        string path = StaticData.configExcel.GetGameItemByID(buyInfo.OriginalPrice[0].ID).Icon;
        _butBuyCurrencyIcon.sprite = ABManager.GetAsset<Sprite>(path);
        _butBuyCurrencyIcon.SetNativeSize();
        _butBuyCurrencyNum.text = _baseBuyCurrencyNum.ToString();
    }

    /// <summary>
    /// 更新界面显示
    /// </summary>
    private void UpdateShow() 
    {
        _itemObtainValue.text = "+" + _baseItemObtainNum * _buyItemNum; 
        _itemRemaining.text = LocalizationDefineHelper.GetStringNameById(120135) + (_buyRemaining - _buyItemNum);//

        _itemNum.text = _buyItemNum.ToString();
        _butBuyCurrencyNum.text = (_baseBuyCurrencyNum * _buyItemNum).ToString();
    }

    /// <summary>
    /// 减
    /// </summary>
    private void OnClickLessBuyNum() 
    {

        if (_buyItemNum <= 1)
            return;
        _buyItemNum -= 1;

        UpdateShow();

        if (_buyItemNum == 0)
            _butLess.interactable = false;
        _butPlus.interactable = true;
    }

    /// <summary>
    /// 加
    /// </summary>
    private void OnClickPlusBuyNum()
    {

        if (_buyItemNum >= _buyRemaining)
            return;
        _buyItemNum += 1;

        UpdateShow();

        if (_buyItemNum == _buyRemaining)
            _butPlus.interactable = false;
        _butLess.interactable = true;
    }

    private void OnClickClose() 
    {
        CancelBuy = null;
        UIComponent.RemoveUI(UIType.UIBuyPower);
    }

    private void OnClickCancel() 
    {
        CancelBuy?.Invoke();
        UIComponent.RemoveUI(UIType.UIBuyPower);
    }

    private void OnClickBuy() 
    {
        BuyCallback?.Invoke(StaticData.configExcel.GetVertical().PhysicalPower, _buyItemNum);
        UIComponent.RemoveUI(UIType.UIBuyPower);
    }


    #endregion
}
