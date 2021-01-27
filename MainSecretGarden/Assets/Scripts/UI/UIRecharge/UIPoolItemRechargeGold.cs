using Company.Cfg;
using Game.Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoolItemRechargeGold : MonoBehaviour, InterfaceScrollCell
{
    public UIRechargeComponent uiRechargeComponent;

    public Transform _itemAmountText;
    public Transform _iconImg;
    public Transform _currentPrice;

    private GoldJewelBuyDefine curGoldConfig;

    private bool isInit = false;

    void Start() 
    {
        Initial();
    }
    private void Initial() {
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
            LevelId = curGoldConfig.LevelId
        };
        ProtocalManager.Instance().SendCSGoldJewelBuy(csGoldJewelBuy, (scGoldJewelBuy) =>
        {
            foreach (var elem in scGoldJewelBuy.CurrencyInfo)
            {
                StaticData.UpdateWareHouseItems(elem.GoodsId, (int)elem.Count);
            }
            StaticData.CreateToastTips("购买金币成功！");
        }, (error) => {});
    }
    public async void ScrollCellIndex(int idx)
    {
        Initial();
        uiRechargeComponent = UIComponent.GetComponentHaveExist<UIRechargeComponent>(UIType.UIRecharge);
        curGoldConfig = uiRechargeComponent.listGoldConfig[idx];
        _itemAmountText.GetComponent<Text>().text = "x" + curGoldConfig.GoodsNum.ToString();
        _iconImg.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(curGoldConfig.Icon);
        _currentPrice.GetComponent<Text>().text = curGoldConfig.GoodsPrice[0].Count.ToString();
    }

}
