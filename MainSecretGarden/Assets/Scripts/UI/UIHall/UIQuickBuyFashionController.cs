using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 快捷购买时装控制类
/// </summary>
public class UIQuickBuyFashionController : MonoBehaviour
{
    #region 变量

    private Image _icon;
    private Text _butBuyName;
    private Text _name;
    private Text _desc;
    private Image _butBuyCurrencyIcon;
    private Text _butBuyCurrencyNum;

    private Button _butBuy;
    private Button _butReturn;

    #endregion

    #region 方法

    private void Init() 
    {
        if (_icon != null)
            return;
        _icon = transform.Find("Panel/IconBG/Icon").GetComponent<Image>();
        _butBuyName = transform.Find("Panel/But_Buy/Text").GetComponent<Text>();
        _butBuyCurrencyIcon = transform.Find("Panel/But_Buy/Icon").GetComponent<Image>();
        _butBuyCurrencyNum = transform.Find("Panel/But_Buy/Num").GetComponent<Text>();

        _name = transform.Find("Panel/Name").GetComponent<Text>();
        _desc = transform.Find("Panel/Desc").GetComponent<Text>();

        _butBuy = transform.Find("Panel/But_Buy").GetComponent<Button>();
        _butReturn = transform.Find("Panel/But_Close").GetComponent<Button>();

        _butBuy.onClick.RemoveAllListeners();
        _butBuy.onClick.AddListener(OnClickBuy);
        _butReturn.onClick.RemoveAllListeners();
        _butReturn.onClick.AddListener(OnClickClose);
    }

    private Action BuyCallback;

    public void InitValue(int roleFashionID, bool isNeedWear, Action buyCallback) 
    {
        Init();

        BuyCallback = buyCallback;
        //获取时装数据
        var fashion = StaticData.configExcel.GetCostumeByCostumeId(roleFashionID);

        if (isNeedWear)
        {
            _butBuyName.text = StaticData.GetMultilingual(120158);//购买并穿戴 
        }
        else
        {
            _butBuyName.text = StaticData.GetMultilingual(120157);//购买 
        }
        //购买数据
        //var buy = StaticData.configExcel.GetStoreByShopId(fashion.ShopId);
        //var currencyIcon = StaticData.configExcel.GetGameItemByID(buy.OriginalPrice[0].ID).Icon;
        //_butBuyCurrencyIcon.sprite = ABManager.GetAsset<Sprite>(currencyIcon);
        //_butBuyCurrencyNum.text = buy.OriginalPrice[0].Count.ToString();

        //_icon.sprite = ABManager.GetAsset<Sprite>(fashion.DefIcon);
        //_name.text = StaticData.GetMultilingual(fashion.MultilingualNameID);
        //_desc.text = StaticData.GetMultilingual(fashion.MultilingualDescID);
        
    }

    private void OnClickClose()
    {
        StaticData.CloseUIQuickBuyFashion();
    }

    private void OnClickBuy() 
    {
        BuyCallback?.Invoke();
        OnClickClose();
    }


    #endregion
}
