using Company.Cfg;
using Game.Protocal;
using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoolItemGood : MonoBehaviour, InterfaceScrollCell
{
    public UIShopComponent uiShopComponent;
    public ShopType shopType;

    public Transform _newestFlag;
    public Transform _discountFlag;
    public Transform _icon;
    public Transform _itemName;
    public Transform _originalPrice;
    public Transform _topCurrencyIcon;
    public Transform _topPriceText;
    public Transform _topCurrencyIcon2;//用于装饰商店
    public Transform _topPriceText2;//用于装饰商店
    public Transform _buyBtn;
    public Transform _curCurrencyIcon;
    public Transform _curPriceText;
    public Transform _LevelLimit;
    public Transform _LevelLimitText;
    public Transform _bgMask;
    public Transform _lock;
    public Transform _limitText;
    public Transform _iconBg;//背景地板
    public Transform _itemGradeBg;//物品等级
    public Transform _buyQuantity;//物品购买数量

    public UIShopChoiceAmount uiShopChoiceAmount;

    private StoreDefine curStoreDefine;
    private bool isInit = false;
    private bool isPromotion = false;
    private bool isItemUnlock = false;
    private int todayBuyLimit;  //今日购买限制
    private string limitStr;
    private string iconName;
    private int popIconName;//用于弹窗的名字;
    private TypeGameItem typeGameItem;//用于保存判断当前物品的类型;
    public int preGetLimitDay = 0;//上一次刷新限购的day数
    private bool isRightBtnDiamond;//判断钻石入口


    public enum ShopType 
    {
        Seed = 0,
        Item = 1,
        Ornament = 2
    }
    void Start()
    {
        Init();
    }
    private void Init() 
    {
        preGetLimitDay = 0;
        if (!isInit) 
        {
            isInit = true;
            RegisterEventListener();
        }
    }
    private void RegisterEventListener()
    {
        _buyBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        _buyBtn.GetComponent<Button>().onClick.AddListener(OnBuyBtnClick);
    }
    void Update() 
    {
        if (uiShopComponent == null)
        {
            uiShopComponent = UIComponent.GetComponentHaveExist<UIShopComponent>(UIType.UIShop);

            //刷新限购信息
            if (uiShopComponent.preGetLimitDay != 0 && preGetLimitDay != uiShopComponent.preGetLimitDay)
            {
                RefreshBuyLimit();
                preGetLimitDay = uiShopComponent.preGetLimitDay;
            }
        }
        else
        {
            //刷新限购信息
            if (uiShopComponent.preGetLimitDay != 0 && preGetLimitDay != uiShopComponent.preGetLimitDay)
            {
                RefreshBuyLimit();
                preGetLimitDay = uiShopComponent.preGetLimitDay;
            }
        }
    }
    /// <summary>
    /// 点击购买
    /// </summary>
    public void OnBuyBtnClick() 
    {
        if (!isItemUnlock) 
        {
            StaticData.CreateToastTips(_LevelLimitText.GetComponent<Text>().text);
            return;
        }
        if (todayBuyLimit < 1) 
        {
            StaticData.CreateToastTips(limitStr);
            return;
        }
        if (uiShopComponent == null)
        {
            uiShopComponent = UIComponent.GetComponentHaveExist<UIShopComponent>(UIType.UIShop);

        }
        else
        {
            if (uiShopChoiceAmount == null)
            {
                uiShopChoiceAmount = uiShopComponent._buyAmountChoice.GetComponent<UIShopChoiceAmount>();
            }
        }

        uiShopChoiceAmount.gameObject.SetActive(true);
        uiShopChoiceAmount.InitShow(curStoreDefine, isPromotion, todayBuyLimit, limitStr, iconName, shopType, this,popIconName,typeGameItem, isRightBtnDiamond);
    }
    public async void ScrollCellIndex(int idx)
    {
        Init();

        if (uiShopComponent == null)
        {
            uiShopComponent  = UIComponent.GetComponentHaveExist<UIShopComponent>(UIType.UIShop);
        }

        switch (shopType)
        {
            case ShopType.Seed:
                curStoreDefine = uiShopComponent.listShopSeedList[idx];
                break;
            case ShopType.Item:
                curStoreDefine = uiShopComponent.listShopItemList[idx];
                break;
            case ShopType.Ornament:
                curStoreDefine = uiShopComponent.listShopOrnamentList[idx];
                break;
        }
        //判断上新促销状态
        DealPromotionAndNewest();
        var itemConfig = StaticData.configExcel.GetGameItemByID(curStoreDefine.GoodId);
        //var storeConfig = StaticData.configExcel.GetStoreByShopId(curStoreDefine.GoodId);


        //判断这个物品是不是钻石购买
        if (shopType == ShopType.Item)
        {
            if (curStoreDefine.OriginalPrice[0].ID == StaticData.configExcel.GetVertical().GoldGoodsId)
            {
                isRightBtnDiamond = false;
            }
            else
            {
                isRightBtnDiamond = true;
            }
        }
        if (shopType == ShopType.Seed)
        {
            isRightBtnDiamond = false;
        }


        //道具地板
        switch (itemConfig.Rarity)
        {
            case TypeRarity.None:
                _iconBg.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>("sd_sp_k1");
                break;
            case TypeRarity.Primary:
                _iconBg.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>("sd_sp_k1");
                break;
            case TypeRarity.Intermediate:
                _iconBg.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>("sd_sp_k2");
                break;
            case TypeRarity.Senior:
                _iconBg.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>("sd_sp_k3");
                break;
            default:
                break;
        }
        //道具数量 只有道具才显示
        if (curStoreDefine.GoodNum >= 1&&shopType==ShopType.Item)
        {
            _buyQuantity.gameObject.SetActive(true);
            _buyQuantity.Find("Text").GetComponent<Text>().text = curStoreDefine.GoodNum.ToString();
        }


        //道具等级 只有种子才有
        if (shopType==ShopType.Seed)
        {
            _itemGradeBg.gameObject.SetActive(true);
            _itemGradeBg.Find("ItemGrade").GetComponent<Text>().text = itemConfig.Grade.ToString();
        }
        else
        {
            if (_itemGradeBg != null)
            {
                _itemGradeBg.gameObject.SetActive(false);
            }
        }


        //道具图标
        if (itemConfig.ItemType == TypeGameItem.Seed)
        {
            var ManorCropConfig = StaticData.configExcel.ManorCrop;
            //用种子id查找作物id
            var curManorCropConfig = ManorCropConfig.Find((elem) => elem.IdSeed == curStoreDefine.GoodId);
            var cropItemConfig = StaticData.configExcel.GetGameItemByID(curManorCropConfig.IdSeed);
            iconName = cropItemConfig.Icon;
            _icon.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(iconName);
        }
        else 
        {
            iconName = itemConfig.Icon;
            _icon.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(iconName);
        }

        typeGameItem = itemConfig.ItemType;

        //商品名称
        _itemName.GetComponent<UILocalize>().SetOtherLanguageId(itemConfig.ItemName);
        popIconName = itemConfig.ItemName;
        //等级限制
        int levelLimit = itemConfig.Grade;
        var userLevelInfo = StaticData.GetPlayerLevelAndCurrExp();
        int levelNum = userLevelInfo.level;
        //未解锁mask
        _bgMask.gameObject.SetActive(false);
        if (_LevelLimit != null && levelNum < levelLimit)
        {
            _LevelLimit.gameObject.SetActive(true);
            _lock.gameObject.SetActive(true);

            //上新标签
            _newestFlag.gameObject.SetActive(false);
            //促销标签
            _discountFlag.gameObject.SetActive(false);

            //未解锁mask
            //_bgMask.gameObject.SetActive(true);
            isItemUnlock = false;
            string levelStr = LocalizationDefineHelper.GetStringNameById(120042);
            string UnlockStr = LocalizationDefineHelper.GetStringNameById(120096);
            _LevelLimitText.GetComponent<Text>().text = levelLimit.ToString() + levelStr + UnlockStr;
        }
        else { 
            _LevelLimit.gameObject.SetActive(false);
            _lock.gameObject.SetActive(false);
            isItemUnlock = true;
            //未解锁mask
            //_bgMask.gameObject.SetActive(false);
        }
    }
    public void RefreshBuyLimit() 
    {
        //判定限购
        GetGoodLimitStr();
    }
    private void GetGoodLimitStr() 
    {
        //判定全局限购
        GoodsInfoStruct curLimitInfo = uiShopComponent.listGoodsInfoStruct.Find((elem) => elem.ShopId == curStoreDefine.ShopId);
        string limitStrStart = "";
        string limitStrCurNum = "";
        string limitStrTotalNum = "";
        do
        {
            //既有每日限购又有全局限购
            if (curStoreDefine.PurchaseNumLimit > 0 && curStoreDefine.DailyPurchaseNum > 0)
            {
                //全局限购已满
                if (curLimitInfo != null && curLimitInfo.PermanentBuyNum >= curStoreDefine.PurchaseNumLimit)
                {
                    //"限购"的多语言;
                    limitStrStart = LocalizationDefineHelper.GetStringNameById(120072);
                    limitStrCurNum = curStoreDefine.PurchaseNumLimit.ToString();
                    limitStrTotalNum = curStoreDefine.PurchaseNumLimit.ToString();
                    break;
                }
                List<string> listStr = GetDailyBuyLimitInfo(curLimitInfo);
                limitStrStart = listStr[0];
                limitStrCurNum = listStr[1];
                limitStrTotalNum = listStr[2];
            }
            //只有每日限购
            if (curStoreDefine.PurchaseNumLimit == 0 && curStoreDefine.DailyPurchaseNum > 0)
            {
                //"今日限购"的多语言;
                limitStrStart = LocalizationDefineHelper.GetStringNameById(120073);
                List<string> listStr = GetDailyBuyLimitInfo(curLimitInfo);
                limitStrStart = listStr[0];
                limitStrCurNum = listStr[1];
                limitStrTotalNum = listStr[2];
                break;
            }
            //只有全局限购
            if (curStoreDefine.PurchaseNumLimit > 0 && curStoreDefine.DailyPurchaseNum == 0)
            { 
                //"限购"的多语言;
                limitStrStart = LocalizationDefineHelper.GetStringNameById(120072);
                limitStrTotalNum = curStoreDefine.PurchaseNumLimit.ToString();
                if (curLimitInfo != null)
                {
                    if (curLimitInfo.PermanentBuyNum >= curStoreDefine.PurchaseNumLimit)
                    {
                        limitStrCurNum = curStoreDefine.PurchaseNumLimit.ToString();
                    }
                    else
                    {
                        limitStrCurNum = curLimitInfo.PermanentBuyNum.ToString();
                    }
                }
                else
                {
                    limitStrCurNum = "0";
                }
            }
        } while (false);
        int.TryParse(limitStrCurNum, out int limitStrCurNumInt);
        int.TryParse(limitStrTotalNum, out int limitStrTotalNumInt);
        limitStr = limitStrStart + limitStrCurNum + "/" + limitStrTotalNum;
        if (limitStr.Length > 1)
        {
            todayBuyLimit = limitStrTotalNumInt - limitStrCurNumInt;
            _limitText.GetComponent<Text>().text = limitStr;

        }
        else 
        {
            _limitText.gameObject.SetActive(false);
            todayBuyLimit = 99;
        }
    }
    private List<string> GetDailyBuyLimitInfo(GoodsInfoStruct curLimitInfo) 
    {
        List<string> listStr = new List<string>();
        //"今日限购"的多语言;
        listStr.Add(LocalizationDefineHelper.GetStringNameById(120073));
        if (curLimitInfo != null)
        {
            if (curLimitInfo.TodayBuyNum >= curStoreDefine.DailyPurchaseNum)
            {
                listStr.Add(curStoreDefine.DailyPurchaseNum.ToString());
            }
            else 
            {
                listStr.Add(curLimitInfo.TodayBuyNum.ToString());
            }
        }
        else
        {
            listStr.Add("0");
        }
        listStr.Add(curStoreDefine.DailyPurchaseNum.ToString());
        return listStr;
    }
    private async void DealPromotionAndNewest() 
    {
        //判定促销状态
        var nowTime = TimeHelper.ServerTimeStampNow;
        if (nowTime > curStoreDefine.PromotionTimeBegin && nowTime < curStoreDefine.PromotionTimeEnd)
        {
            isPromotion = true;
            _discountFlag.gameObject.SetActive(true);
            //_originalPrice.gameObject.SetActive(true);
            //花费的道具，区别对待装饰
            if (shopType != ShopType.Ornament)
            {
                //花费的原价道具icon
                int costItemId = curStoreDefine.OriginalPrice[0].ID;
                var costItemConfig = StaticData.configExcel.GetGameItemByID(costItemId);
                _topCurrencyIcon.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(costItemConfig.Icon);
                //花费的原价道具数量
                _topPriceText.GetComponent<Text>().text = curStoreDefine.OriginalPrice[0].Count.ToString();
                //促销价消耗的道具icon
                int curCostItemId = curStoreDefine.PromotionPrice[0].ID;
                var curCostItemConfig = StaticData.configExcel.GetGameItemByID(curCostItemId);
                _curCurrencyIcon.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(curCostItemConfig.Icon);
                //促销价消耗的道具数量
                _curPriceText.GetComponent<Text>().text = curStoreDefine.PromotionPrice[0].Count.ToString();
            }
            else
            {
                //装饰的促销
                //货币1
                int costItemId = curStoreDefine.PromotionPrice[0].ID;
                var costItemConfig = StaticData.configExcel.GetGameItemByID(costItemId);
                _topCurrencyIcon.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(costItemConfig.Icon);
                //花费的原价道具数量
                _topPriceText.GetComponent<Text>().text = curStoreDefine.PromotionPrice[0].Count.ToString();
                //货币2
                int costItemId2 = curStoreDefine.PromotionDiamondPrice[0].ID;
                var costItemConfig2 = StaticData.configExcel.GetGameItemByID(costItemId2);
                _topCurrencyIcon2.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(costItemConfig2.Icon);
                //花费的原价道具数量
                _topPriceText2.GetComponent<Text>().text = curStoreDefine.PromotionDiamondPrice[0].Count.ToString();
            }
        }
        else
        {
            isPromotion = false;
            _discountFlag.gameObject.SetActive(false);
            //_originalPrice.gameObject.SetActive(false);
            //花费的道具，区别对待装饰
            if (shopType != ShopType.Ornament)
            {
                //****其他商品的原价****//
                //现价消耗的道具icon
                int curCostItemId = curStoreDefine.OriginalPrice[0].ID;
                var curCostItemConfig = StaticData.configExcel.GetGameItemByID(curCostItemId);
                _curCurrencyIcon.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(curCostItemConfig.Icon);
                //现价消耗的道具数量
                _curPriceText.GetComponent<Text>().text = curStoreDefine.OriginalPrice[0].Count.ToString();
            }
            else
            {
                //_originalPrice.gameObject.SetActive(true);
                //装饰的原价
                //货币1
                int costItemId = curStoreDefine.OriginalPrice[0].ID;
                var costItemConfig = StaticData.configExcel.GetGameItemByID(costItemId);
                _topCurrencyIcon.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(costItemConfig.Icon);
                //花费的原价道具数量
                _topPriceText.GetComponent<Text>().text = curStoreDefine.OriginalPrice[0].Count.ToString();
                //货币2
                int costItemId2 = curStoreDefine.DiamondPrice[0].ID;
                var costItemConfig2 = StaticData.configExcel.GetGameItemByID(costItemId2);
                _topCurrencyIcon2.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(costItemConfig2.Icon);
                //花费的原价道具数量
                _topPriceText2.GetComponent<Text>().text = curStoreDefine.DiamondPrice[0].Count.ToString();
            }
        }
        //判定上新状态
        if (nowTime > curStoreDefine.UpNewTimeBegin && nowTime < curStoreDefine.UpNewTimeEnd)
        {
            _newestFlag.gameObject.SetActive(true);
            _discountFlag.gameObject.SetActive(false);
        }
        else
        {
            _newestFlag.gameObject.SetActive(false);
        }
    }
    public void ChangeBuyLimit(int buyAmount) 
    {
        if (uiShopComponent == null)
        {
            uiShopComponent =  UIComponent.GetComponentHaveExist<UIShopComponent>(UIType.UIShop);
        }
        uiShopComponent.RefreshBuyLimit(curStoreDefine.ShopId, buyAmount, curStoreDefine);
        RefreshBuyLimit();
    }
}
