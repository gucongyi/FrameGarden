using Company.Cfg;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


/// <summary>
/// 购买紫金币页面
/// </summary>
public class BuyPurpleGoldAmountChoice : MonoBehaviour
{
    public Transform view;
    public Text Title;
    public Text goodsName;
    public Image propIcon;//道具icon
    public Text Price;//实时价格
    public Button cancelBtn;
    public Button maskBtn;

    int totalPrice;//总价
    int priceAmount;//价格数量
    public UISetAmountComponent amountComponent;

    public Action buySucceedCallBack;//购买成功回调
    public Action clickCancelCallBack;//取消回调
    public Action buyFailureCallBack;//购买失败回调

    private void Awake()
    {
        if (view != null)
            UniversalTool.ReadyPopupAnim(view);
        //取消事件
        maskBtn.onClick.RemoveAllListeners();
        maskBtn.onClick.AddListener(() =>
        {
            UniversalTool.CancelPopAnim(view, Cancel);
        });

        cancelBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.AddListener(() =>
        {
            UniversalTool.CancelPopAnim(view, Cancel);
        });
    }

    private void OnEnable()
    {
        if (view != null)
            UniversalTool.StartPopupAnim(view);
    }
    private void OnDisable()
    {
        if (view != null)
            UniversalTool.ReadyPopupAnim(view);
    }

    /// <summary>
    /// 购买指定礼物界面
    /// </summary>
    /// <param name="item">指定的礼物</param>
    /// <param name="defultAmount">默认数量</param>
    /// <param name="buySucceedCallBack">购买成功回调</param>
    /// <param name="clickCancelCallBack">点击取消回调</param>
    public async void ShowBuyView(int defultAmount, Action buySucceedCallBack = null, Action clickCancelCallBack = null, Action buyFailure = null)
    {
        this.gameObject.SetActive(true);//显示界面

        Title.text = $"{ StaticData.GetMultilingual(120074)}{StaticData.GetMultilingual(2000002)}";
        goodsName.text = $"{StaticData.GetMultiLanguageByGameItemId(StaticData.configExcel.GetVertical().PurpleGoldsId)}";

        var zjbData = StaticData.configExcel.GetStoreByShopId(StaticData.configExcel.GetVertical().PurpleGoldsId);

        int id = zjbData.OriginalPrice[0].ID;//取到钻石图片的id
        priceAmount = (int)zjbData.OriginalPrice[0].Count;//取到数量 
        Sprite sprite = await ZillionaireToolManager.LoadItemSprite(id);

        this.buySucceedCallBack = buySucceedCallBack;
        this.clickCancelCallBack = clickCancelCallBack;
        this.buyFailureCallBack = buyFailure;
        amountComponent._defaultValue = defultAmount.ToString();
        amountComponent.inputFiled.text = defultAmount.ToString();
        amountComponent.act = BuyAmount;
        amountComponent._changeAction = ChangeAct;


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
            CSBuyProp cSBuyPurple = new CSBuyProp()
            {
                BuyWay = GoodsBuyWay.FirstWay,
                GoodId = StaticData.configExcel.GetVertical().PurpleGoldsId,
                GoodNum = amount
            };
            ProtocalManager.Instance().SendCSBuyProp(cSBuyPurple, (sCBuyProp) =>
            {
                Debug.Log("购买紫金币成功");
                //仓库数量增加
                StaticData.UpdateWareHouseItem(StaticData.configExcel.GetVertical().PurpleGoldsId, amount);
                //回调
                buySucceedCallBack?.Invoke();
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
            //StaticData.CreateToastTips("购买道具需要消耗的货币不足");
            buyFailureCallBack.Invoke();
            //StaticData.OpenRechargeUI();
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
