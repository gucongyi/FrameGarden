using Company.Cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManorRegionAdIncreaseComponent : MonoBehaviour
{
    public Button ButtonMask;
    public Button ButtonAdIncrease;
    public Button ButtonDiamondIncrease;
    public Text TextH;
    public Text TextM;
    public Text TextS;
    public Image ImageCurrency;
    public Text TextCurrencyNum;
    public Text TextAdIncrease;
    public Text TextDiamondIncrease;
    public Transform transAnim;
    ManorRegionComponent manorRegionComponent;
    DecorateBoardComponent decorateBoardComponent;
    public void SetRegionComponent(ManorRegionComponent manorRegionComponent, DecorateBoardComponent decorateBoardComponent)
    {
        this.manorRegionComponent = manorRegionComponent;
        this.decorateBoardComponent = decorateBoardComponent;
        //设置货币加速的图标和数量
        AreaUnlockDefine areaUnlockDefine = StaticData.configExcel.GetAreaUnlockByID(this.manorRegionComponent.regionId);
        var dealClass = areaUnlockDefine.UseGoods;
        DealClass willCost = dealClass[0];
        var iconCurrency=StaticData.configExcel.GetGameItemByID(willCost.IdGameItem).Icon;
        ImageCurrency.sprite = ABManager.GetAsset<Sprite>(iconCurrency);
        TextCurrencyNum.text = $"{willCost.Price}";
        int adAddMinute = areaUnlockDefine.SpeedTime / 60;
        int diamondAddMinute = areaUnlockDefine.GoodsSpeedTime / 60;
        TextAdIncrease.text = string.Format(TextAdIncrease.text, adAddMinute);
        TextDiamondIncrease.text = string.Format(TextDiamondIncrease.text, diamondAddMinute);
    }
    private void Awake()
    {
        ButtonMask.onClick.RemoveAllListeners();
        ButtonMask.onClick.AddListener(OnButtonMaskClick);
        ButtonAdIncrease.onClick.RemoveAllListeners();
        ButtonAdIncrease.onClick.AddListener(OnButtonAdIncreaseClick);
        ButtonDiamondIncrease.onClick.RemoveAllListeners();
        ButtonDiamondIncrease.onClick.AddListener(OnButtonDiamondIncreaseClick);
    }

    private async void OnButtonDiamondIncreaseClick()
    {
        //钻石判定
        var dealClass = StaticData.configExcel.GetAreaUnlockByID(this.manorRegionComponent.regionId).UseGoods;
        DealClass willCost = dealClass[0];
        int currHaveCount = StaticData.GetWareHouseItem(dealClass[0].IdGameItem).GoodNum;
        if (currHaveCount < dealClass[0].Price)
        {
            string currencyName = StaticData.GetMultiLanguageByGameItemId(dealClass[0].IdGameItem);
            string Tips = string.Format(StaticData.GetMultilingual(120068), currencyName);
            var iconName = StaticData.configExcel.GetGameItemByID(dealClass[0].IdGameItem).Icon;
            Sprite currencyIcon = await ABManager.GetAssetAsync<Sprite>(iconName);
            //只能配货币和道具
            StaticData.OpenCommonTips(Tips, 120123,async () =>
            {
                var gameItemDefine=StaticData.configExcel.GetGameItemByID(willCost.IdGameItem);
                
                switch (gameItemDefine.ItemType)
                {
                    case TypeGameItem.None://货币
                        if (StaticData.configExcel.GetVertical().GoldGoodsId == willCost.IdGameItem)
                        {//金币
                            await StaticData.OpenRechargeUI(0);
                        }
                        else if (StaticData.configExcel.GetVertical().JewelGoodsId == willCost.IdGameItem)
                        {//钻石
                            await StaticData.OpenRechargeUI(1);
                        }
                        break;
                    case TypeGameItem.Prop:
                        //跳转到商店
                        await StaticData.OpenShopUI(1);
                        break;
                    default:
                        await StaticData.OpenRechargeUI(1);
                        break;
                }
            });
            return;
        }
        manorRegionComponent.OnIncreaseClick(Game.Protocal.WorkShedSpeedUpWay.DiamondWay);
    }

    private void OnButtonAdIncreaseClick()
    {
        StaticData.OpenAd("ManorRegionCleanAd", (code, msg) => {
            if (code==1)
            {
                StaticData.DataDot(Company.Cfg.DotEventId.RegionUnLockingAdIncrease);
                manorRegionComponent.OnIncreaseClick(Game.Protocal.WorkShedSpeedUpWay.AdvertisingWay);
            }
        });
    }
    private void OnEnable()
    {
        if (transAnim != null)
            UniversalTool.StartPopupAnim(transAnim);
    }
    private void OnButtonMaskClick()
    {
        UniversalTool.CancelPopAnim(transAnim, () =>
        {
            UIComponent.HideUI(UIType.UIManorRegionAdIncrease);
        });
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (decorateBoardComponent != null)
        {
            float remainTime = decorateBoardComponent.GetRemianTime();
            if (remainTime > 0)
            {
                int h = (int)(remainTime / 3600);
                remainTime = remainTime % 3600;
                int m = (int)(remainTime / 60);
                remainTime = remainTime % 60;
                int s = (int)remainTime;
                TextH.text = String.Format("{0:00}", h);
                TextM.text = String.Format("{0:00}", m);
                TextS.text = String.Format("{0:00}", s);
            }
            else
            {
                UIComponent.RemoveUI(UIType.UIManorRegionAdIncrease);
            }
        }
    }
}
