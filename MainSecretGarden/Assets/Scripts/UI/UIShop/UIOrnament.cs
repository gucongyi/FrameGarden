using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Company.Cfg;
using Game.Protocal;
using static UIPoolItemGood;

public class UIOrnament : MonoBehaviour, InterfaceScrollCell
{
    public Transform _itemBg;//装饰物品背景
    public Transform content;
    public Transform DecoratePop;//装饰详细弹窗

    public Transform iconPop;//左侧图标
    public Transform iconNamePop;//左侧图标名字

    #region 新加的变量
    public UIShopComponent uiShopComponent;
    public ShopType shopType;
    public Transform _newestFlag;//上新标签
    public Transform _discountFlag;//促销标签
    public Transform _icon;//右侧滑动图标
    public Transform _itemName;//右侧滑动图标名字
    public Transform _topCurrencyIcon;//左边价格图标金币 用于大弹窗
    public string _topPriceText;//金币价格
    public Text _popGoldPriceText;
    public Transform _topCurrencyIcon2;//右边价格图标钻石 
    public string _topPriceText2;//钻石价格   用于大弹窗
    public Text _popDiamondPriceText;
    public Transform _bgMask;//遮罩

    public UIShopChoiceAmount uiShopChoiceAmount;//购买弹窗
    public UIDecoratePop uiDecoratePop;//左边详细窗口

    public StoreDefine curStoreDefine;
    public bool isInit = false;
    public bool isPromotion = false;
    public int todayBuyLimit;  //今日购买限制
    public string limitStr;
    public string iconName;
    public int popIconName;//用于弹窗的名字;
    public TypeGameItem typeGameItem;//用于保存判断当前物品的类型;
    public int preGetLimitDay = 0;//上一次刷新限购的day数
    public bool isClick=false;//用于判断装饰框是否点击

    public int index;//该物品的下标

    public GameItemDefine itemDefine;//该物品详情
    #endregion


    private void Start()
    {
        Init();
    }


    //点击装饰物品
    public void OnClick()
    {
        if (content == null)
        {
            content = UIComponent.GetComponentHaveExist<UIShopComponent>(UIType.UIShop).LoopOrnamentShop.Find("LoopVerticalScroll").Find("Content");
        }

        //点击显示背景
        for (int i = 0; i < content.childCount; i++)
        {
            content.GetChild(i).Find("Background").gameObject.SetActive(false);
            content.GetChild(i).GetComponent<UIOrnament>().isClick = false;
        }
        _itemBg.gameObject.SetActive(true);
        isClick = true;

        //点击显示详细框
        if (DecoratePop == null)
        {
            DecoratePop = UIComponent.GetComponentHaveExist<UIShopComponent>(UIType.UIShop).LoopOrnamentShop.Find("DecoratePop");
        }

        if (DecoratePop.gameObject.activeSelf)
        {
            UpdatePopInfo();
        }
        else
        {
            DecoratePop.gameObject.SetActive(true);
            UpdatePopInfo();
        }

        //有装饰物被点击
        uiDecoratePop.isItemClick = true;
        uiDecoratePop.clickName = _itemName.GetComponent<Text>().text;
        
    }

    //更新装饰弹窗信息
    private void UpdatePopInfo()
    {
        //判断是否检测到
        if (uiShopComponent == null)
        {
            uiShopComponent = UIComponent.GetComponentHaveExist<UIShopComponent>(UIType.UIShop);

            if (_popGoldPriceText == null)
            {
                _popGoldPriceText = uiShopComponent.LoopOrnamentShop.Find("DecoratePop").Find("BuyBtn_gold").Find("CurPrice").GetComponent<Text>();
            }

            if (_popDiamondPriceText == null)
            {
                _popDiamondPriceText = uiShopComponent.LoopOrnamentShop.Find("DecoratePop").Find("BuyBtn_diamond").Find("CurPrice").GetComponent<Text>();
            }

            if (iconPop == null)
            {
                iconPop = uiShopComponent.LoopOrnamentShop.Find("DecoratePop").Find("Icon");
            }
            if (iconNamePop == null)
            {
                iconNamePop= uiShopComponent.LoopOrnamentShop.Find("DecoratePop").Find("IconName");
            }

        }
        else
        {
            if (_popGoldPriceText == null)
            {
                _popGoldPriceText = uiShopComponent.LoopOrnamentShop.Find("DecoratePop").Find("BuyBtn_gold").Find("CurPrice").GetComponent<Text>();
            }

            if (_popDiamondPriceText == null)
            {
                _popDiamondPriceText = uiShopComponent.LoopOrnamentShop.Find("DecoratePop").Find("BuyBtn_diamond").Find("CurPrice").GetComponent<Text>();
            }

            if (iconPop == null)
            {
                iconPop = uiShopComponent.LoopOrnamentShop.Find("DecoratePop").Find("Icon");
            }
            if (iconNamePop == null)
            {
                iconNamePop = uiShopComponent.LoopOrnamentShop.Find("DecoratePop").Find("IconName");
            }
        }
        

        //更新图片
        iconPop.GetComponent<Image>().sprite = _icon.GetComponent<Image>().sprite;
        //更新名字
        iconNamePop.GetComponent<Text>().text = _itemName.GetComponent<Text>().text;
        //更新价格
        _popGoldPriceText.text = _topPriceText;//金币价格
        _popDiamondPriceText.text = _topPriceText2;//钻石价格

        //传递该物品详情
        UIDecoratePop.itemDefine = itemDefine;

        //判断是否有旋转
        uiShopComponent.LoopOrnamentShop.Find("DecoratePop").GetComponent<UIDecoratePop>().IsHasReversal();

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

    /// <summary>
    /// 登记事件监听
    /// </summary>
    private void RegisterEventListener()
    {
        transform.GetComponent<Button>().onClick.RemoveAllListeners();
        transform.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void Update()
    {
        if (uiShopComponent == null)
        {
            uiShopComponent = UIComponent.GetComponentHaveExist<UIShopComponent>(UIType.UIShop);

            if (uiDecoratePop == null)
            {
                uiDecoratePop = uiShopComponent.LoopOrnamentShop.Find("DecoratePop").GetComponent<UIDecoratePop>();
            }

            //刷新限购信息
            if (uiShopComponent.preGetLimitDay != 0 && preGetLimitDay != uiShopComponent.preGetLimitDay)
            {
                RefreshBuyLimit();
                preGetLimitDay = uiShopComponent.preGetLimitDay;

                if (uiDecoratePop.isFirstUpdatePopInfo) return;
                //初始化左边详细界面
                uiDecoratePop.GetComponent<UIDecoratePop>().Init();
            }
        }
        else
        {
            if (uiDecoratePop == null)
            {
                uiDecoratePop = uiShopComponent.LoopOrnamentShop.Find("DecoratePop").GetComponent<UIDecoratePop>();
            }
            //刷新限购信息
            if (uiShopComponent.preGetLimitDay != 0 && preGetLimitDay != uiShopComponent.preGetLimitDay)
            {
                RefreshBuyLimit();
                preGetLimitDay = uiShopComponent.preGetLimitDay;

                if (uiDecoratePop.isFirstUpdatePopInfo) return;
                //初始化左边详细界面
                uiDecoratePop.GetComponent<UIDecoratePop>().Init();
            }
        }
    }

    /// <summary>
    /// 更新购买限制
    /// </summary>
    public void RefreshBuyLimit()
    {
        //判定限购
        GetGoodLimitStr();
    }

    public async void ScrollCellIndex(int idx)
    {
        Init();

        if (uiShopComponent == null)
        {
            uiShopComponent = UIComponent.GetComponentHaveExist<UIShopComponent>(UIType.UIShop);
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
        //下标赋值
        index = idx;
        DealPromotionAndNewest();
        var itemConfig = StaticData.configExcel.GetGameItemByID(curStoreDefine.GoodId);

        //保存数据
        itemDefine = itemConfig;

        //道具图标
        if (itemConfig.ItemType == TypeGameItem.Seed)
        {
            var ManorCropConfig = StaticData.configExcel.ManorCrop;
            //用种子id查找作物id
            var curManorCropConfig = ManorCropConfig.Find((elem) => elem.IdSeed == curStoreDefine.GoodId);
            var cropItemConfig = StaticData.configExcel.GetGameItemByID(curManorCropConfig.IdGainGameItem);
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
        //未解锁mask
        _bgMask.gameObject.SetActive(false);
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
        }
        else
        {
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
        //判断是否检测到
        if (uiShopComponent == null)
        {
            uiShopComponent = UIComponent.GetComponentHaveExist<UIShopComponent>(UIType.UIShop);

            if (_topCurrencyIcon == null)
            {
                _topCurrencyIcon = uiShopComponent.LoopOrnamentShop.Find("DecoratePop").Find("BuyBtn_gold").Find("CurrencyIcon");
            }

            if (_topCurrencyIcon2==null)
            {
                _topCurrencyIcon2 = uiShopComponent.LoopOrnamentShop.Find("DecoratePop").Find("BuyBtn_diamond").Find("CurrencyIcon");
            }
        }
        else
        {
            if (_topCurrencyIcon == null)
            {
                _topCurrencyIcon = uiShopComponent.LoopOrnamentShop.Find("DecoratePop").Find("BuyBtn_gold").Find("CurrencyIcon");
            }

            if (_topCurrencyIcon2 == null)
            {
                _topCurrencyIcon2 = uiShopComponent.LoopOrnamentShop.Find("DecoratePop").Find("BuyBtn_diamond").Find("CurrencyIcon");
            }
        }


        //判定促销状态
        var nowTime = TimeHelper.ServerTimeStampNow;
        if (nowTime > curStoreDefine.PromotionTimeBegin && nowTime < curStoreDefine.PromotionTimeEnd)
        {
            isPromotion = true;
            _discountFlag.gameObject.SetActive(true);
            //装饰的促销

            //货币1
            if (curStoreDefine.PromotionPrice.Count == 0)//只是钻石促销
            {
                int costItemId = curStoreDefine.OriginalPrice[0].ID;
                var costItemConfig = StaticData.configExcel.GetGameItemByID(costItemId);
                _topCurrencyIcon.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(costItemConfig.Icon);
                _topPriceText = curStoreDefine.OriginalPrice[0].Count.ToString();//金币显示原价
            }
            else
            {
                int costItemId = curStoreDefine.PromotionPrice[0].ID;
                var costItemConfig = StaticData.configExcel.GetGameItemByID(costItemId);
                _topCurrencyIcon.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(costItemConfig.Icon);
                _topPriceText = curStoreDefine.PromotionPrice[0].Count.ToString();
            }
            
             //货币2
            if (curStoreDefine.PromotionDiamondPrice.Count==0)//只是金币促销
            {
                int costItemId2 = curStoreDefine.DiamondPrice[0].ID;
                var costItemConfig2 = StaticData.configExcel.GetGameItemByID(costItemId2);
                _topCurrencyIcon2.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(costItemConfig2.Icon);
                _topPriceText2 = curStoreDefine.DiamondPrice[0].Count.ToString();//钻石显示原价
            }
            else
            {
                int costItemId2 = curStoreDefine.PromotionDiamondPrice[0].ID;
                var costItemConfig2 = StaticData.configExcel.GetGameItemByID(costItemId2);
                _topCurrencyIcon2.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(costItemConfig2.Icon);
                _topPriceText2 = curStoreDefine.PromotionDiamondPrice[0].Count.ToString();
            }
            
        }
        else
        {
            isPromotion = false;
            _discountFlag.gameObject.SetActive(false);
             //装饰的原价
             //货币1
              int costItemId = curStoreDefine.OriginalPrice[0].ID;
              var costItemConfig = StaticData.configExcel.GetGameItemByID(costItemId);
              _topCurrencyIcon.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(costItemConfig.Icon);
              //花费的原价道具数量
              _topPriceText = curStoreDefine.OriginalPrice[0].Count.ToString();
              //货币2
              int costItemId2 = curStoreDefine.DiamondPrice[0].ID;
              var costItemConfig2 = StaticData.configExcel.GetGameItemByID(costItemId2);
              _topCurrencyIcon2.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(costItemConfig2.Icon);
              //花费的原价道具数量
               _topPriceText2 = curStoreDefine.DiamondPrice[0].Count.ToString();
        }
        //判定上新状态
        if (nowTime > curStoreDefine.UpNewTimeBegin && nowTime < curStoreDefine.UpNewTimeEnd)
        {
            _newestFlag.gameObject.SetActive(true);
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
            uiShopComponent = UIComponent.GetComponentHaveExist<UIShopComponent>(UIType.UIShop);
        }
        uiShopComponent.RefreshBuyLimit(curStoreDefine.ShopId, buyAmount, curStoreDefine);
        RefreshBuyLimit();
    }

}
