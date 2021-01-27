using Company.Cfg;
using Game.Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoolItemRechargeDiamond : MonoBehaviour, InterfaceScrollCell
{
    public UIRechargeComponent uiRechargeComponent;
    public Transform _itemAmountText;
    public Transform _iconImg;
    public Transform _originalPrice;
    public Transform _currentPriceText;
    public Transform _firstCharge;
    public LoopVerticalScrollRect lsRechargeDiamond;

    private GoldJewelBuyDefine curDiamondConfig;

    private bool isInit = false;

    void Start()
    {
        Initial();
    }
    private void Initial()
    {
        if (!isInit)
        {
            RegisterEventListener();
            isInit = true;
        }
    }
    private void RegisterEventListener()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(OnRechargeBtnClick);
    }
    private void OnRechargeBtnClick()
    {
        CSGoldJewelBuy csGoldJewelBuy = new CSGoldJewelBuy()
        {
            LevelId = curDiamondConfig.LevelId
        };
        ProtocalManager.Instance().SendCSGoldJewelBuy(csGoldJewelBuy, (scGoldJewelBuy) =>
        {
            foreach (var elem in scGoldJewelBuy.CurrencyInfo)
            {
                StaticData.UpdateWareHouseItems(elem.GoodsId, (int)elem.Count);
            }
            StaticData.CreateToastTips("购买钻石成功！");
            StaticData.playerInfoData.userInfo.IsFirstDiscounts = false;
            foreach (Transform child in lsRechargeDiamond.content) 
            {
                child.GetComponent<UIPoolItemRechargeDiamond>().setFirstFlagActive(false);
            }
            
        }, (error) => {});
    }
    public async void ScrollCellIndex(int idx)
    {
        Initial();
        curDiamondConfig = uiRechargeComponent.listDiamondConfig[idx];
        _itemAmountText.GetComponent<Text>().text = curDiamondConfig.GoodsNum.ToString();
        _iconImg.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(curDiamondConfig.Icon);
        _originalPrice.GetComponent<Text>().text = curDiamondConfig.OriginalPrice.ToString();
        _currentPriceText.GetComponent<Text>().text = curDiamondConfig.FirstPrice.ToString();
        _firstCharge.gameObject.SetActive(StaticData.playerInfoData.userInfo.IsFirstDiscounts);
    }
    public void setFirstFlagActive(bool isActive) 
    {
        _firstCharge.gameObject.SetActive(isActive);
    }
}
