using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 礼物购买界面
/// </summary>
public class BuyGiftAmountChoice : MonoBehaviour
{

    public Text Title;
    public Image goodsIcon;
    public Text goodsName;
    public Text n2pAddValue;
    public Text p2nAddValue;
    public Image propIcon;//道具icon
    public Text Price;//实时价格
    public Button cancelBtn;

    int totalPrice;//总价
    int priceAmount;//价格数量
    public ImpulseGiftGoods giftInfo;
    public UISetAmountComponent amountComponent;

    public Action<int> buySucceedCallBack;//购买成功回调
    public Action clickCancelCallBack;//取消回调

    /// <summary>
    /// 购买指定礼物界面
    /// </summary>
    /// <param name="item">指定的礼物</param>
    /// <param name="defultAmount">默认数量</param>
    /// <param name="buySucceedCallBack">购买成功回调</param>
    /// <param name="clickCancelCallBack">点击取消回调</param>
    public async void ShowBuyView(ImpulseGiftGoods item, int defultAmount, Action<int> buySucceedCallBack = null, Action clickCancelCallBack = null)
    {
        this.gameObject.SetActive(true);//显示界面
        this.giftInfo = item;

        Title.text = $"你还没有{ StaticData.GetMultiLanguageByGameItemId(giftInfo.goodsID)}哦，要购买吗";//TODO
        goodsIcon.sprite = await ABManager.GetAssetAsync<Sprite>(giftInfo.goodsIcon);
        goodsName.text = $"{StaticData.GetMultiLanguageByGameItemId(giftInfo.goodsID)}";
        n2pAddValue.text = $"+{giftInfo.NpcAddValue}";
        p2nAddValue.text = $"+{giftInfo.playerAddValue}";

        int id = giftInfo.goodsPriceBuy[0].ID;//取到钻石图片的id
        priceAmount = (int)giftInfo.goodsPriceBuy[0].Count;//取到数量 
        Sprite sprite = await ZillionaireToolManager.LoadItemSprite(id);
        propIcon.sprite = sprite;

        this.buySucceedCallBack = buySucceedCallBack;
        this.clickCancelCallBack = clickCancelCallBack;
        amountComponent._defaultValue = defultAmount.ToString();
        amountComponent.inputFiled.text = defultAmount.ToString();
        amountComponent.act = BuyAmount;
        amountComponent._changeAction = ChangeAct;
        //取消事件
        cancelBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.AddListener(Cancel);
        //初始价格显示 默认数量*指定礼物价格
        Price.text = (priceAmount * defultAmount).ToString();
    }

    public void ChangeAct(int num)
    {
        totalPrice = priceAmount * num;
        Price.text = totalPrice.ToString();
    }

    public void BuyAmount(int amount)
    {
        totalPrice = priceAmount * amount;
        if (StaticData.GetWareHouseDiamond() >= totalPrice)
        {
            //扣除资源
            StaticData.UpdateWareHouseDiamond(-totalPrice);
            CSBuyProp cSBuySection = new CSBuyProp()
            {
                BuyWay = GoodsBuyWay.FirstWay,
                GoodId = giftInfo.goodsID,
                GoodNum = amount
            };
            ProtocalManager.Instance().SendCSBuyProp(cSBuySection, (sCBuyProp) =>
            {
                Debug.Log("购买道具成功");
                //仓库数量增加
                StaticData.UpdateWareHouseItem(giftInfo.goodsID, amount);
                //更新礼物数量
                buySucceedCallBack?.Invoke(amount);
                //更新玩家货币
                StaticData.UpdateBackpackProps(sCBuyProp);
                this.gameObject.SetActive(false);

            }, (ErrorInfo e) =>
            {
                Debug.LogError("购买失败" + e.webErrorCode);
            });
        }
        else
        {
            StaticData.CreateToastTips("购买道具需要消耗的货币不足");
            StaticData.OpenRechargeUI();
        }
    }
    /// <summary>
    /// 点击取消
    /// </summary>
    void Cancel()
    {
        this.gameObject.SetActive(false);
        this.clickCancelCallBack?.Invoke();
    }
}
