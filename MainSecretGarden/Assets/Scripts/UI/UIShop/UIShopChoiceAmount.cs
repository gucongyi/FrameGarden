using Company.Cfg;
using Game.Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UIPoolItemGood;

public class UIShopChoiceAmount : MonoBehaviour
{
    public Transform _bgMask;
    public Transform _itemIcon;
    public Transform _limitText;
    public Transform _leftBuyTF;
    public Transform _leftCancelTF;
    public Transform _rightBuyTF;
    public UISetAmountComponent uiSetAmountComponent;
    public UIShopComponent uiShopComponent;

    private Transform _leftBuyBtn;
    private Transform _leftOriginalPrice;
    private Transform _leftCostIcon;
    private Transform _leftCurPrice;

    private Transform _leftCancelBtn;
    private Transform _rightBuyBtn;
    private Transform _rightOriginalPrice;
    private Transform _rightCostIcon;
    private Transform _rightCurPrice;
    private UIPoolItemGood choicedUIPoolItemGood;//当前选择的商品
    private UIOrnament choicedUIOrnament;//当前选择的装饰
    private bool isOrnament=false;//判断是否是装饰品
    private bool isRightBtnOrnament;//判断装饰界面是否从钻石入口进入的

    private StoreDefine curStoreDefine;
    private List<GoodIDCount> listCostItem = new List<GoodIDCount>();
    private bool isPromotion = false;
    private int todayBuyLimit;  //今日购买限制
    private string limitStr;
    private string iconName;
    private int buyAmount = 1;
    private ShopType shopType;
    private int leftOriginalUnitPrice;//左侧原价的单价
    private int leftCurUnitPrice;//左侧现价的单价
    private int rightOriginalUnitPrice;//右侧原价的单价
    private int rightCurUnitPrice;//右侧现价的单价
    private int _popIconName;//物品名字
    private TypeGameItem typeGameItem;//物品类型
    private bool isInit = false;

    public Transform showItemName;
    public GameObject seedInfo;//种子详细信息
    public GameObject propInfo;//道具详细信息
    //public InputField inputField;//价格输入框
    public Transform topCurrency;
    private string maxMoney;//最大金钱
    private int unitPrice;//单价
    private Transform hasgold;//上方金币框
    private Transform hasdiamend;//上方钻石框
    private bool isRightBtnDiamond;
    public Transform _newestFlag;//上新图标
    public Transform _discountFlag;//促销图标
    public Transform _itemGradeBg;//物品等级

    //准备动画
    private void Awake()
    {
        UniversalTool.ReadyPopupAnim(transform);
    }
    //开始动画
    private void OnEnable()
    {
        UniversalTool.StartPopupAnim(transform);
    }
    //还原动画
    private void OnDisable()
    {
        UniversalTool.ReadyPopupAnim(transform);
    }



    void Start()
    {
        Init();
    }
    private void Init() 
    {
        if (!isInit)
        {
            isInit = true;

            _leftBuyBtn = _leftBuyTF.Find("Button");
            _leftOriginalPrice = _leftBuyTF.Find("OriginalPrice");
            _leftCostIcon = _leftBuyTF.Find("Button/CostIcon");
            _leftCurPrice = _leftBuyTF.Find("Button/CurPrice");

            _leftCancelBtn = _leftCancelTF.Find("Button");

            _rightBuyBtn = _rightBuyTF.Find("Button");
            _rightOriginalPrice = _rightBuyTF.Find("OriginalPrice");
            _rightCostIcon = _rightBuyTF.Find("Button/CostIcon");
            _rightCurPrice = _rightBuyTF.Find("Button/CurPrice");

            hasgold = topCurrency.Find("CoinPanel/UIGoldNav(Clone)/Num");
            hasdiamend = topCurrency.Find("DiamondPanel/UIDiamondNav(Clone)/Num");

            RegisterEventListener();
            uiSetAmountComponent._changeAction += RefreshByAmountChange;
        }
        //遮罩显示
        _bgMask.gameObject.SetActive(true);
    }
    private void RegisterEventListener()
    {
        _bgMask.GetComponent<Button>().onClick.RemoveAllListeners();
        _bgMask.GetComponent<Button>().onClick.AddListener(OnHideShopChoiceUI);
        _leftCancelBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        _leftCancelBtn.GetComponent<Button>().onClick.AddListener(OnHideShopChoiceUI);
        _leftBuyBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        _leftBuyBtn.GetComponent<Button>().onClick.AddListener(OnLeftBuyBtnClick);
        _rightBuyBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        _rightBuyBtn.GetComponent<Button>().onClick.AddListener(OnRightBuyBtnClick);
    }
    private void OnHideShopChoiceUI() 
    {
        //返回动画
        UniversalTool.CancelPopAnim(transform, OnHideShopChoice);
    }

    private void OnHideShopChoice()
    {
        //弹窗隐藏
        gameObject.SetActive(false);
        //遮罩隐藏
        _bgMask.gameObject.SetActive(false);
    }

    private void OnLeftBuyBtnClick()
    {
        if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.CurrExecuteGuideLittleStepDefine.Id== 10011)
        {
            GuideCanvasComponent._instance.SetLittleStepFinish();
        }
        listCostItem.Clear();
        listCostItem.Add(new GoodIDCount()
        {
            ID = curStoreDefine.OriginalPrice[0].ID,
            Count = leftCurUnitPrice
        });
        GoodsBuyWay buyWay = GoodsBuyWay.FirstWay;
        CSBuyProp csBuyProp = new CSBuyProp()
        {
            GoodId = curStoreDefine.ShopId,
            GoodNum = buyAmount,
            BuyWay = buyWay
        };
        ProtocalManager.Instance().SendCSBuyProp(csBuyProp, (SCBuyProp) =>
        {
            //道具入库
            StaticData.UpdateWareHouseItem(curStoreDefine.GoodId, (int)curStoreDefine.GoodNum * buyAmount);
            //货币扣除
            foreach (var elem in SCBuyProp.CurrencyInfo)
            {
                StaticData.UpdateWareHouseItems(elem.GoodsId, (int)elem.Count);
            }
            //判断是不是装饰
            if (isOrnament)
            {
                choicedUIOrnament.ChangeBuyLimit(buyAmount);
            }
            else
            {
                choicedUIPoolItemGood.ChangeBuyLimit(buyAmount);
            }
            StaticData.CreateToastTips("购买成功！");
            OnHideShopChoiceUI();
        }, (error) => {
            
            StaticData.OpenCommonTips(StaticData.GetMultilingual(120245), 120010, () =>
            {
                //打开充值界面
                StaticData.OpenRechargeUI(0);
                //关闭弹窗
                transform.gameObject.SetActive(false);
                //关闭遮罩
                _bgMask.gameObject.SetActive(false);
            }, null, 120075);
            //BuyItemError(error, buyWay);
        },false);
    }
    private void OnRightBuyBtnClick()
    {
        if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.CurrExecuteGuideLittleStepDefine.Id == 10011)
        {
            GuideCanvasComponent._instance.SetLittleStepFinish();
        }
        listCostItem.Clear();
        var buyWay = GoodsBuyWay.SecondWay;

        if (shopType == ShopType.Item)
        {
            buyWay = GoodsBuyWay.FirstWay;
            listCostItem.Add(new GoodIDCount()
            {
                ID = curStoreDefine.OriginalPrice[0].ID,
                Count = rightCurUnitPrice
            });
        }
        else
        {
            listCostItem.Add(new GoodIDCount()
            {
                ID = curStoreDefine.DiamondPrice[0].ID,
                Count = rightCurUnitPrice
            });
        }
        CSBuyProp csBuyProp = new CSBuyProp()
        {
            GoodId = curStoreDefine.ShopId,
            GoodNum = buyAmount,
            BuyWay = buyWay
        };
        ProtocalManager.Instance().SendCSBuyProp(csBuyProp, (SCBuyProp) =>
        {
            //道具入库
            StaticData.UpdateWareHouseItem(curStoreDefine.GoodId, (int)curStoreDefine.GoodNum * buyAmount);
            //货币扣除
            foreach (var elem in SCBuyProp.CurrencyInfo)
            {
                StaticData.UpdateWareHouseItems(elem.GoodsId, (int)elem.Count);
            }
            if (isOrnament)
            {
                choicedUIOrnament.ChangeBuyLimit(buyAmount);
            }
            else
            {
                choicedUIPoolItemGood.ChangeBuyLimit(buyAmount);
            }
            StaticData.CreateToastTips("购买成功！");
            OnHideShopChoiceUI();
        }, (error) => {
            StaticData.OpenCommonTips(StaticData.GetMultilingual(120243), 120010, () =>
            {
                //打开充值界面
                StaticData.OpenRechargeUI(1);
                //关闭弹窗
                transform.gameObject.SetActive(false);
                //关闭遮罩
                _bgMask.gameObject.SetActive(false);
            }, null, 120075);
            //BuyItemError(error, buyWay);
        },false);
    }
    private void BuyItemError(ErrorInfo error, GoodsBuyWay buyWay) 
    {
        switch (error.webErrorCode) 
        {
            case WebErrorCode.Good_Consume:
                //判定消耗的的货币类型
                int costItemType = 0;
                int costItemId = curStoreDefine.OriginalPrice[0].ID;
                if (buyWay == GoodsBuyWay.SecondWay) 
                { 
                    costItemId = curStoreDefine.DiamondPrice[0].ID;
                }
                if (costItemId == StaticData.configExcel.GetVertical().JewelGoodsId)
                {
                    costItemType = 1;
                }
                StaticData.OpenRechargeUI(costItemType);
                break;
        }
        StaticData.WebErrorCodeCommonTip(error);
    }
    public void InitShow(StoreDefine curStoreDefineTmp, bool isPromotionTmp, int todayBuyLimitTmp, string limitStrTmp, string iconNameTmp, ShopType shopTypeTmp, UIPoolItemGood uiPoolItemGood,int popIconName,TypeGameItem itemType, bool isRightBtn) 
    {
        isOrnament = false;
        isRightBtnDiamond = isRightBtn;
        Init();
        _popIconName = popIconName;
        typeGameItem = itemType;
        curStoreDefine = curStoreDefineTmp;
        isPromotion = isPromotionTmp;
        todayBuyLimit = todayBuyLimitTmp;
        limitStr = limitStrTmp;
        iconName = iconNameTmp;
        shopType = shopTypeTmp;
        buyAmount = 1;
        uiSetAmountComponent.ResetDefalut();
        uiSetAmountComponent._maxValue = todayBuyLimit;
        RefreshShopChoiceUI();
        choicedUIPoolItemGood = uiPoolItemGood;
    }
    //装饰的重载
    public void InitShow(StoreDefine curStoreDefineTmp, bool isPromotionTmp, int todayBuyLimitTmp, string limitStrTmp, string iconNameTmp, ShopType shopTypeTmp, UIOrnament uiOrnamen, int popIconName, TypeGameItem itemType,bool isRightBtn)
    {
        isOrnament = true;
        isRightBtnOrnament = isRightBtn;
        Init();
        _popIconName = popIconName;
        typeGameItem = itemType;
        curStoreDefine = curStoreDefineTmp;
        isPromotion = isPromotionTmp;
        todayBuyLimit = todayBuyLimitTmp;
        limitStr = limitStrTmp;
        iconName = iconNameTmp;
        shopType = shopTypeTmp;
        buyAmount = 1;
        uiSetAmountComponent.ResetDefalut();
        uiSetAmountComponent._maxValue = todayBuyLimit;
        RefreshShopChoiceUI();
        choicedUIOrnament = uiOrnamen;
    }

    private async void RefreshShopChoiceUI() 
    {
        var itemConfig = StaticData.configExcel.GetGameItemByID(curStoreDefine.GoodId);
        var fruititemConfig = StaticData.configExcel.GetGameItemByID(curStoreDefine.GoodId+1000000);
        var ManorCropConfig = StaticData.configExcel.GetManorCropByCropId(curStoreDefine.GoodId);
        //道具icon
        _itemIcon.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(iconName);
        //商品名称
        showItemName.GetComponent<UILocalize>().SetOtherLanguageId(_popIconName);
        //道具等级
        if (itemConfig.ItemType == TypeGameItem.Seed)
        {
            _itemGradeBg.gameObject.SetActive(true);
            _itemGradeBg.Find("ItemGrade").GetComponent<Text>().text = itemConfig.Grade.ToString();
        }
        else
        {
            _itemGradeBg.gameObject.SetActive(false);
        }
        //上新与促销
        var nowTime = TimeHelper.ServerTimeStampNow;
        if (nowTime > curStoreDefine.PromotionTimeBegin && nowTime < curStoreDefine.PromotionTimeEnd)
        {
            _discountFlag.gameObject.SetActive(true);
        }
        else
        {
            _discountFlag.gameObject.SetActive(false);
        }
        if (nowTime > curStoreDefine.UpNewTimeBegin && nowTime < curStoreDefine.UpNewTimeEnd)
        {
            _newestFlag.gameObject.SetActive(true);
        }
        else
        {
            _newestFlag.gameObject.SetActive(false);
        }

        //商品详细信息
        switch (typeGameItem)
        {
            case TypeGameItem.Seed:
                seedInfo.SetActive(true);
                propInfo.SetActive(false);

                //更新信息
                int time = ManorCropConfig.GrowUp + ManorCropConfig.AdultnessTime;

                seedInfo.transform.Find("GetTime/Text").GetComponent<Text>().text = time.ToString();
                //收获的果实价格
                seedInfo.transform.Find("GetGold/Text").GetComponent<Text>().text=(fruititemConfig.PriceSell[0].Price*ManorCropConfig.ResultsNumber).ToString();
                seedInfo.transform.Find("GetSeed/Text").GetComponent<Text>().text = ManorCropConfig.ResultsNumber.ToString();
                seedInfo.transform.Find("GetExp/Text").GetComponent<Text>().text = ManorCropConfig.GainExperience.ToString();

                break;
            default:
                seedInfo.SetActive(false);
                propInfo.SetActive(true);

                //更新信息
                propInfo.transform.Find("Text").GetComponent<UILocalize>().SetOtherLanguageId(itemConfig.Description);
                break;
        }
        //限购字段
        if (limitStr.Length > 1)
        {
            _limitText.GetComponent<Text>().text = limitStr;
        }
        else
        {
            _limitText.gameObject.SetActive(false);
        }
        //2个按钮的功能 leftBtn是金币 rightBtn是钻石
        BtnFunc();

    }
    private async void BtnFunc() 
    {

        switch (shopType)
        {
            case ShopType.Seed://金币支付
                //支付按钮
                _rightBuyTF.gameObject.SetActive(false);
                _leftBuyTF.gameObject.SetActive(true);
                //花费的道具icon
                int costItemId = curStoreDefine.OriginalPrice[0].ID;
                var costItemConfig = StaticData.configExcel.GetGameItemByID(costItemId);
                _leftCostIcon.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(costItemConfig.Icon);
                if (isPromotion)//如果是促销
                {
                    _leftOriginalPrice.gameObject.SetActive(true);
                    //原价具体数字
                    leftOriginalUnitPrice= (int)curStoreDefine.OriginalPrice[0].Count;
                    //促销价的具体数字(现价)
                    leftCurUnitPrice= (int)curStoreDefine.PromotionPrice[0].Count;
                }
                else
                {
                    _leftOriginalPrice.gameObject.SetActive(false);
                    //促销价显示的当前的价格
                    leftCurUnitPrice = (int)curStoreDefine.OriginalPrice[0].Count;
                }
                break;
            case ShopType.Item://钻石支付 或者 金币支付
                if (isRightBtnDiamond)//如果是钻石支付
                {
                    //支付按钮
                    _rightBuyTF.gameObject.SetActive(true);
                    _leftBuyTF.gameObject.SetActive(false);
                    //花费的道具icon
                    int costItemId1 = curStoreDefine.OriginalPrice[0].ID;
                    var costItemConfig1 = StaticData.configExcel.GetGameItemByID(costItemId1);
                    _rightCostIcon.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(costItemConfig1.Icon);
                    if (isPromotion)
                    {
                        _rightOriginalPrice.gameObject.SetActive(true);
                        //原价具体数字
                        rightOriginalUnitPrice = (int)curStoreDefine.OriginalPrice[0].Count;
                        //促销价的具体数字
                        rightCurUnitPrice = (int)curStoreDefine.PromotionPrice[0].Count;
                    }
                    else
                    {
                        _rightOriginalPrice.gameObject.SetActive(false);
                        //原价的具体数字
                        rightCurUnitPrice = (int)curStoreDefine.OriginalPrice[0].Count;
                    }
                    break;
                }
                else//如果是金币支付
                {
                    //支付按钮
                    _rightBuyTF.gameObject.SetActive(false);
                    _leftBuyTF.gameObject.SetActive(true);
                    //花费的道具icon
                    int costItemId1 = curStoreDefine.OriginalPrice[0].ID;
                    var costItemConfig1 = StaticData.configExcel.GetGameItemByID(costItemId1);
                    _leftCostIcon.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(costItemConfig1.Icon);
                    if (isPromotion)
                    {
                        _leftOriginalPrice.gameObject.SetActive(true);
                        //原价具体数字
                        leftOriginalUnitPrice = (int)curStoreDefine.OriginalPrice[0].Count;
                        //促销价的具体数字
                        leftCurUnitPrice = (int)curStoreDefine.PromotionPrice[0].Count;
                    }
                    else
                    {
                        _leftOriginalPrice.gameObject.SetActive(false);
                        //原价的具体数字
                        leftCurUnitPrice = (int)curStoreDefine.OriginalPrice[0].Count;
                    }
                    break;
                }

            case ShopType.Ornament://混合支付
                //装饰商店
                if (isRightBtnOrnament)//如果是从钻石购买进入
                {
                    _leftBuyTF.gameObject.SetActive(false);
                    _rightBuyTF.gameObject.SetActive(true);
                    //第二种方式花费的道具
                    int costItemId3 = curStoreDefine.DiamondPrice[0].ID;
                    var costItemConfig3 = StaticData.configExcel.GetGameItemByID(costItemId3);
                    _rightCostIcon.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(costItemConfig3.Icon);

                    if (isPromotion)
                    {
                        if (curStoreDefine.PromotionDiamondPrice.Count == 0)//只是金币降价
                        {
                            _rightOriginalPrice.gameObject.SetActive(false);
                            rightCurUnitPrice = (int)curStoreDefine.DiamondPrice[0].Count;
                        }
                        else
                        {
                            _rightOriginalPrice.gameObject.SetActive(true);
                            rightOriginalUnitPrice = (int)curStoreDefine.DiamondPrice[0].Count;
                            rightCurUnitPrice = (int)curStoreDefine.PromotionDiamondPrice[0].Count;
                        }
                    }
                    else
                    {
                        _rightOriginalPrice.gameObject.SetActive(false);
                        rightCurUnitPrice = (int)curStoreDefine.DiamondPrice[0].Count;
                    }
                }
                else
                {
                    _leftBuyTF.gameObject.SetActive(true);
                    _rightBuyTF.gameObject.SetActive(false);
                    //第一种方式花费的道具
                    int costItemId2 = curStoreDefine.OriginalPrice[0].ID;
                    var costItemConfig2 = StaticData.configExcel.GetGameItemByID(costItemId2);
                    _leftCostIcon.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(costItemConfig2.Icon);

                    if (isPromotion)
                    {
                        if (curStoreDefine.PromotionPrice.Count == 0)//只是钻石降价
                        {
                            _leftOriginalPrice.gameObject.SetActive(false);
                            leftCurUnitPrice = (int)curStoreDefine.OriginalPrice[0].Count;
                        }
                        else
                        {
                            _leftOriginalPrice.gameObject.SetActive(true);
                            leftOriginalUnitPrice = (int)curStoreDefine.OriginalPrice[0].Count;
                            leftCurUnitPrice = (int)curStoreDefine.PromotionPrice[0].Count;
                        }
                    }
                    else
                    {
                        _leftOriginalPrice.gameObject.SetActive(false);
                        leftCurUnitPrice = (int)curStoreDefine.OriginalPrice[0].Count;
                    }

                }
                #region
                //if (isPromotion)
                //{
                //    _leftOriginalPrice.gameObject.SetActive(true);
                //    leftOriginalUnitPrice = (int)curStoreDefine.OriginalPrice[0].Count;
                //    leftCurUnitPrice = (int)curStoreDefine.PromotionPrice[0].Count;

                //    _rightOriginalPrice.gameObject.SetActive(true);
                //    rightOriginalUnitPrice = (int)curStoreDefine.DiamondPrice[0].Count;
                //    rightCurUnitPrice = (int)curStoreDefine.PromotionDiamondPrice[0].Count;
                //}
                //else
                //{
                //    _leftOriginalPrice.gameObject.SetActive(false);
                //    _rightOriginalPrice.gameObject.SetActive(false);
                //    leftCurUnitPrice = (int)curStoreDefine.OriginalPrice[0].Count;
                //    rightCurUnitPrice = (int)curStoreDefine.DiamondPrice[0].Count;
                //}

                #endregion
                break;
        }
        SetPriceUI();
        GetMaxMoney();
        InputMaxMoney();
    }
    //private int GetItemAmount() 
    //{
    //    return uiSetAmountComponent.result;
    //}
    public void RefreshByAmountChange(int curAmount) 
    {
        buyAmount = curAmount;
        SetPriceUI();
    }
    private void SetPriceUI() 
    {
        _leftOriginalPrice.GetComponent<Text>().text = (leftOriginalUnitPrice * buyAmount).ToString();
        _leftCurPrice.GetComponent<Text>().text = (leftCurUnitPrice * buyAmount).ToString();
        _rightOriginalPrice.GetComponent<Text>().text = (rightOriginalUnitPrice * buyAmount).ToString();
        _rightCurPrice.GetComponent<Text>().text = (rightCurUnitPrice * buyAmount).ToString();
    }

    //获取拥有的金币或钻石的原价
    private void GetMaxMoney()
    {
        if (shopType == ShopType.Ornament)
        {
            if (isRightBtnOrnament)
            {
                maxMoney = hasdiamend.GetComponent<Text>().text;
                unitPrice = rightCurUnitPrice;
            }
            else
            {
                maxMoney = hasgold.GetComponent<Text>().text;
                unitPrice = leftCurUnitPrice;
            }
        }
        else
        {
            if (isRightBtnDiamond)
            {
                maxMoney = hasdiamend.GetComponent<Text>().text;
                unitPrice = rightCurUnitPrice;
            }
            else
            {
                maxMoney = hasgold.GetComponent<Text>().text;
                unitPrice = leftCurUnitPrice;
            }

        }
        Debug.Log(maxMoney);
        Debug.Log(unitPrice);
    }


    //能够购买的最大数量
    public void InputMaxMoney()
    {
        int a= int.Parse(maxMoney) / unitPrice;

        Debug.Log("单价：" + unitPrice);
        Debug.Log("数量：" + a);

        if (uiSetAmountComponent._maxValue > a)
        {
            uiSetAmountComponent._maxValue = a;
        }

        Debug.Log(uiSetAmountComponent._maxValue);
    }
}
