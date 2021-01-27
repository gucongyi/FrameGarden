using Company.Cfg;
using Game.Protocal;
using Quick.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using ShopItemType = Company.Cfg.ShopItemType;
using Cysharp.Threading.Tasks;

public class UIShopComponent : MonoBehaviour
{
    public TabGroup tabGroup;

    public Transform _btnBack;
    public Transform _tabSeed;
    public Transform _tabItem;
    public Transform _tabOrnament;
    public Transform _buyAmountChoice;
    public Transform _coinPanel;
    public Transform _diamondPanel;
    public Transform _buyAmountChoiceBgMask;//购买弹窗遮罩
    public Transform _topCurrency;//顶部金币栏和钻石栏
    private Vector3 currTopCurrencyPosition;//顶部金币栏和钻石栏初始位置 动画的初始位置
    private Vector3 _defPos;//顶部金币栏和钻石栏初始位置
    public Transform LoopSeedShop;
    public Transform LoopItemShop;
    public Transform LoopOrnamentShop;

    public LoopVerticalScrollRect lsSeedList;
    public LoopVerticalScrollRect lsItemList;
    public LoopVerticalScrollRect lsOrnamentList;

    public List<StoreDefine> listShopSeedList;
    public List<StoreDefine> listShopItemList;
    public List<StoreDefine> listShopOrnamentList;
    public List<GoodsInfoStruct> listGoodsInfoStruct = new List<GoodsInfoStruct>();//协议中的购买限制

    private bool initFlag = false;
    public int preGetLimitDay = 0;//上一次获取限购列表的day数
    private Action OnCloseCallBack;


    //种子 道具红点
    public Transform seedRed;
    public Transform itemRed;

    #region 用于商店排序

    int levelNum;//玩家等级

    private List<StoreDefine> listShopSeedNewList=new List<StoreDefine>();//种子商店上新链表
    private List<StoreDefine> listShopSeedPromotionList=new List<StoreDefine>();//种子商店促销链表
    private List<StoreDefine> listShopSeedNorMalList=new List<StoreDefine>();//种子商店正常链表

    private List<StoreDefine> listShopItemNewList=new List<StoreDefine>();//道具商店上新链表
    private List<StoreDefine> listShopItemPromotionList=new List<StoreDefine>();//道具商店促销链表
    private List<StoreDefine> listShopItemNorMalList=new List<StoreDefine>();//道具商店正常链表

    private List<StoreDefine> listShopOrnamentNewList=new List<StoreDefine>();//装饰商店上新链表
    private List<StoreDefine> listShopOrnamentPromotionList=new List<StoreDefine>();//装饰商店促销链表
    private List<StoreDefine> listShopOrnamentNorMalList=new List<StoreDefine>();//装饰商店正常链表

    #endregion


    public enum ShopTabTags
    {
        TabSeed = 0,
        TabItem = 1,
        TabOrnament = 2
    }

    public ShopTabTags CurrshopTabTag= ShopTabTags.TabSeed;

    //开始动画
    private void OnEnable()
    {
        if (_defPos.y == 0)
        {
            return;
        }

        InitAnim();
    }

    //还原动画
    private void OnDisable()
    {
        _topCurrency.localPosition = currTopCurrencyPosition;
    }

    private void InitAnimTra()
    {
        _defPos = _topCurrency.localPosition;
        _topCurrency.localPosition = new Vector3(_defPos.x, _defPos.y + 200, 0);
        currTopCurrencyPosition = _topCurrency.localPosition;
    }

    private void InitAnim()
    {
        _topCurrency.DOLocalMoveY(_defPos.y, 0.4f).SetEase(Ease.Linear);
    }

    //商店退出动画
    private void BackAnim()
    {
        //金币栏和钻石栏退出
        _topCurrency.DOLocalMoveY(_defPos.y + 200, 0.2f).SetEase(Ease.Linear);

        //种子、装饰和道具的退出
        if (LoopSeedShop.gameObject.activeSelf)
        {
            lsSeedList.gameObject.GetComponent<SeedAndItemEnterAnim>().BackAnim();
        }
        else if(LoopItemShop.gameObject.activeSelf)
        {
            lsItemList.gameObject.GetComponent<SeedAndItemEnterAnim>().BackAnim();
        }
        else if(LoopOrnamentShop.gameObject.activeSelf)
        {
            LoopOrnamentShop.GetComponent<OrnamentEnterAnim>().BackAnim();
        }
    }

    void Start()
    {
        Initial();
        InitAnimTra();
        InitAnim();

        LoopSeedShop = transform.Find("LoopSeedShop");
        LoopItemShop = transform.Find("LoopItemShop");
        LoopOrnamentShop = transform.Find("LoopOrnamentShop");
    }
    private void Initial()
    {
        GetBuyLimitByServer();
        if (!initFlag)
        {
            LoopScrollSizeAdjust();
            RegisterEventListener();
            RegisterTabListener();
            GetShopConfigData();
            ShowCurrencyUI();
            ShowRed();
            initFlag = true;
        }
    }
    /// <summary>
    /// 货币UI
    /// </summary>
    private void ShowCurrencyUI() 
    {
        StaticData.CreateCoinNav(_coinPanel);
        StaticData.CreateDiamondNav(_diamondPanel);
    }
    private void RegisterEventListener()
    {
        _btnBack.GetComponent<Button>().onClick.RemoveAllListeners();
        _btnBack.GetComponent<Button>().onClick.AddListener(OnHideShopUI);
    }
    private void RegisterTabListener()
    {
        tabGroup = tabGroup.GetComponent<TabGroup>();
        tabGroup.AddTabsClickEvent(OnTabClick);
    }
    private void OnTabClick(Tab target, PointerEventData eventDatas)
    {
        if (tabGroup.curTab == target)
        {
            return;
        }
        ResetAllTabTitle(target);
        if (target.tag == ShopTabTags.TabSeed.ToString())
        {
            CurrshopTabTag = ShopTabTags.TabSeed;
            GenerateSeedListUI();
        }
        else if (target.tag == ShopTabTags.TabItem.ToString())
        {
            CurrshopTabTag = ShopTabTags.TabItem;
            GenerateItemListUI();
        }
        else if (target.tag == ShopTabTags.TabOrnament.ToString())
        {
            CurrshopTabTag = ShopTabTags.TabOrnament;
            GenerateOrnamentListUI();
        }
    }
    private void ResetAllTabTitle(Tab curTab)
    {
        var otherTabs = tabGroup.GetOtherTabs(curTab);
        for (int i = 0; i < otherTabs.Count; i++)
        {
            Tab tab = otherTabs[i];
            tab.page.IsOn = false;
        }
    }
    /// <summary>
    /// 隐藏商店UI
    /// </summary>
    private async void OnHideShopUI() 
    {
        _buyAmountChoice.gameObject.SetActive(false);
        _buyAmountChoiceBgMask.gameObject.SetActive(false);

        BackAnim();
        await UniTask.Delay(TimeSpan.FromSeconds(0.4));

        LoopSeedShop.gameObject.SetActive(false);
        LoopItemShop.gameObject.SetActive(false);
        LoopOrnamentShop.gameObject.SetActive(false);

        UIComponent.HideUI(UIType.UIShop);
        OnCloseCallBack?.Invoke();
        if (UIComponent.IsHaveUI(UIType.UIManor))
        {
            UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor).RefreshDecorateList();
        }
    }
    /// <summary>
    /// 显示种子商店
    /// </summary>
    public void ShowSeedShopUI(Action OnCloseCallBack = null) 
    {
        Initial();
        this.OnCloseCallBack = OnCloseCallBack;
        Tab curTab = _tabSeed.GetComponent<Tab>();
        tabGroup.TurnTabOn(ShopTabTags.TabSeed.ToString(), (Tab defTab) =>
        {
            CurrshopTabTag = ShopTabTags.TabSeed;
            defTab.page.IsOn = true;
        });
        ResetAllTabTitle(curTab);
        GenerateSeedListUI();
    }
    /// <summary>
    /// 显示道具商店
    /// </summary>
    public void ShowItemShopUI(Action OnCloseCallBack=null)
    {
        Initial();
        this.OnCloseCallBack = OnCloseCallBack;
        Tab curTab = _tabItem.GetComponent<Tab>();
        tabGroup.TurnTabOn(ShopTabTags.TabItem.ToString(), (Tab defTab) =>
        {
            CurrshopTabTag = ShopTabTags.TabItem;
            defTab.page.IsOn = true;
        });
        ResetAllTabTitle(curTab);
        GenerateItemListUI();
    }
    /// <summary>
    /// 显示装饰商店
    /// </summary>
    /// <param name="OnCloseCallBack"></param>
    public void ShowOrnamentShopUI(Action OnCloseCallBack=null)
    {
        Initial();
        this.OnCloseCallBack = OnCloseCallBack;
        Tab curTab = _tabOrnament.GetComponent<Tab>();
        tabGroup.TurnTabOn(ShopTabTags.TabOrnament.ToString(), (Tab defTab) =>
        {
            CurrshopTabTag = ShopTabTags.TabOrnament;
            defTab.page.IsOn = true;
        });
        ResetAllTabTitle(curTab);
        GenerateOrnamentListUI();
    }
    public void GenerateSeedListUI() 
    {
        lsSeedList.ClearCells();
        lsSeedList.totalCount = listShopSeedList.Count;
        lsSeedList.RefillCells();
    }
    public void GenerateItemListUI()
    {
        lsItemList.ClearCells();
        lsItemList.totalCount = listShopItemList.Count;
        lsItemList.RefillCells();
    }
    public void GenerateOrnamentListUI()
    {
        lsOrnamentList.ClearCells();
        lsOrnamentList.totalCount = listShopOrnamentList.Count;
        lsOrnamentList.RefillCells();
    }
    /// <summary>
    /// 获取商店配置
    /// </summary>
    private void GetShopConfigData() 
    {
        #region 链表清空
        listShopSeedList.Clear();
        listShopItemList.Clear();
        listShopOrnamentList.Clear();

        listShopSeedNewList.Clear();
        listShopSeedPromotionList.Clear();
        listShopSeedNorMalList.Clear();

        listShopItemNewList.Clear();
        listShopItemPromotionList.Clear();
        listShopItemNorMalList.Clear();

        listShopOrnamentNewList.Clear();
        listShopOrnamentPromotionList.Clear();
        listShopOrnamentNorMalList.Clear();
        #endregion

        var storeC = StaticData.configExcel.Store;
        //玩家等级
        var userLevelInfo = StaticData.GetPlayerLevelAndCurrExp();
        levelNum = userLevelInfo.level;

        foreach (var elem in storeC)//如果该物品显示才加入列表
        {
            switch (elem.MallType) 
            {
                case ShopItemType.SeedStore:
                    if (elem.IsDisplay)
                    {
                        //listShopSeedList.Add(elem);
                        ShopSeedSort(elem);
                    }
                    break;
                case ShopItemType.PropStore:
                    if (elem.IsDisplay)
                    {
                        //listShopItemList.Add(elem);
                        ShopItemSort(elem);
                    }
                    break;
                case ShopItemType.DecorateStore:
                    if (elem.IsDisplay)
                    {
                        //listShopOrnamentList.Add(elem);
                        ShopOrnamentSort(elem);
                    }
                    break;
            }
        }
        ShopSeedConfig();
        ShopItemConfig();
        ShopOrnamentConfig();

    }
    /// <summary>
    /// 获取服务器限购信息
    /// </summary>
    private void GetBuyLimitByServer() 
    {
        if (preGetLimitDay == TimeHelper.GetDateTime(0,0,0,0).Day) 
        {
            return;
        }
        preGetLimitDay = TimeHelper.GetDateTime(0, 0, 0, 0).Day;
        listGoodsInfoStruct.Clear();
        CSEmptyPromotionsGoodsInfo csEmptyPromotionsGoodsInfo = new CSEmptyPromotionsGoodsInfo();
        ProtocalManager.Instance().SendCSEmptyPromotionsGoodsInfo(csEmptyPromotionsGoodsInfo, (scPromotionsGoodsInfo) =>
        {
            if (scPromotionsGoodsInfo == null)
            {
                return;
            }
            for (int i = 0; i < scPromotionsGoodsInfo.PromotionsGoods.Count; i++) 
            {
                listGoodsInfoStruct.Add(scPromotionsGoodsInfo.PromotionsGoods[i]);
            }
        }, (error) => {});
    }
    /// <summary>
    /// 刷新购买限制
    /// </summary>
    /// <param name="shopId"></param>
    /// <param name="buyAmount"></param>
    /// <param name="curStoreDefine"></param>
    public void RefreshBuyLimit(int shopId, int buyAmount, StoreDefine curStoreDefine) 
    {
        var curLimitInfo = listGoodsInfoStruct.Find((elem) => elem.ShopId == shopId);
        if (curLimitInfo != null)
        {
            if (curStoreDefine.PurchaseNumLimit > 0) 
            { 
                curLimitInfo.PermanentBuyNum += buyAmount;
            }
            if (curStoreDefine.DailyPurchaseNum > 0) 
            {
                curLimitInfo.TodayBuyNum += buyAmount;
            }
        }
        else 
        {
            GoodsInfoStruct newGoodsInfoStruct = new GoodsInfoStruct() 
            {
                ShopId = shopId
            };
            if (curStoreDefine.PurchaseNumLimit > 0)
            {
                newGoodsInfoStruct.PermanentBuyNum += buyAmount;
            }
            if (curStoreDefine.DailyPurchaseNum > 0)
            {
                newGoodsInfoStruct.TodayBuyNum += buyAmount;
            }
            listGoodsInfoStruct.Add(newGoodsInfoStruct);
        }
    }
    /// <summary>
    /// 商品列表大小自适应
    /// </summary>
    private void LoopScrollSizeAdjust() 
    {
        if (Screen.height < 2688) 
        {
            var deltaHeight = (2688 - Screen.height * 1242 / Screen.width) / 2;
            lsSeedList.transform.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, deltaHeight);
            lsItemList.transform.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, deltaHeight);
            //lsOrnamentList.transform.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, deltaHeight);

        }
    }

    /// <summary>
    /// Tab小红点显示
    /// </summary>
    private void ShowRed()
    {
        seedRed.gameObject.SetActive(ShopTool._isOpenSeedRed);
        itemRed.gameObject.SetActive(ShopTool._isOpenItemRed);
    }

    #region 商城排序 只有物品解锁才进行上新或者促销排序
    /// <summary>
    /// 商城种子排序
    /// </summary>
    private void ShopSeedSort(StoreDefine storeDefine)
    {
        //判断物品是否解锁
        var itemConfig = StaticData.configExcel.GetGameItemByID(storeDefine.GoodId);
        int levelLimit = itemConfig.Grade;
        if (levelNum >= levelLimit)//物品已经解锁
        {
            //判断是否上新或者促销   
            var nowTime = TimeHelper.ServerTimeStampNow;
            if (nowTime > storeDefine.PromotionTimeBegin && nowTime < storeDefine.PromotionTimeEnd) //促销
            {
                if (nowTime > storeDefine.UpNewTimeBegin && nowTime < storeDefine.UpNewTimeEnd)//既促销又上新
                {
                    listShopSeedNewList.Add(storeDefine);
                }
                else
                {
                    listShopSeedPromotionList.Add(storeDefine);
                }
            }
            else if (nowTime > storeDefine.UpNewTimeBegin && nowTime < storeDefine.UpNewTimeEnd)//上新
            {
                listShopSeedNewList.Add(storeDefine);
            }
            else
            {
                listShopSeedNorMalList.Add(storeDefine);
            }
        }
        else
        {
            listShopSeedNorMalList.Add(storeDefine);
        }
    }
    /// <summary>
    /// 商城种子配置
    /// </summary>
    private void ShopSeedConfig()
    {
        if (listShopSeedNewList.Count != 0)
        {
            for (int i = 0; i < listShopSeedNewList.Count; i++)
            {
                listShopSeedList.Add(listShopSeedNewList[i]);
            }
        }
        if (listShopSeedPromotionList.Count != 0)
        {
            for (int i = 0; i < listShopSeedPromotionList.Count; i++)
            {
                listShopSeedList.Add(listShopSeedPromotionList[i]);
            }
        }
        if (listShopSeedNorMalList.Count != 0)
        {
            for (int i = 0; i < listShopSeedNorMalList.Count; i++)
            {
                listShopSeedList.Add(listShopSeedNorMalList[i]);
            }
        }
    }

    /// <summary>
    /// 商城道具排序
    /// </summary>
    private void ShopItemSort(StoreDefine storeDefine)
    {
        //判断是否上新或者促销
        var nowTime = TimeHelper.ServerTimeStampNow;
        if (nowTime > storeDefine.PromotionTimeBegin && nowTime < storeDefine.PromotionTimeEnd) //促销
        {
            if (nowTime > storeDefine.UpNewTimeBegin && nowTime < storeDefine.UpNewTimeEnd)//既促销又上新
            {
                listShopItemNewList.Add(storeDefine);
            }
            else
            {
                listShopItemPromotionList.Add(storeDefine);
            }
        }
        else if (nowTime > storeDefine.UpNewTimeBegin && nowTime < storeDefine.UpNewTimeEnd)//上新
        {
            listShopItemNewList.Add(storeDefine);
        }
        else
        {
            listShopItemNorMalList.Add(storeDefine);
        }
    }
    /// <summary>
    /// 商城道具配置
    /// </summary>
    private void ShopItemConfig()
    {
        if (listShopItemNewList.Count != 0)
        {
            for (int i = 0; i < listShopItemNewList.Count; i++)
            {
                listShopItemList.Add(listShopItemNewList[i]);
            }
        }
        if (listShopItemPromotionList.Count != 0)
        {
            for (int i = 0; i < listShopItemPromotionList.Count; i++)
            {
                listShopItemList.Add(listShopItemPromotionList[i]);
            }
        }
        if (listShopItemNorMalList.Count != 0)
        {
            for (int i = 0; i < listShopItemNorMalList.Count; i++)
            {
                listShopItemList.Add(listShopItemNorMalList[i]);
            }
        }
    }

    /// <summary>
    /// 商城装饰排序
    /// </summary>
    private void ShopOrnamentSort(StoreDefine storeDefine)
    {
        //判断是否上新或者促销
        var nowTime = TimeHelper.ServerTimeStampNow;
        if (nowTime > storeDefine.PromotionTimeBegin && nowTime < storeDefine.PromotionTimeEnd) //促销
        {
            if (nowTime > storeDefine.UpNewTimeBegin && nowTime < storeDefine.UpNewTimeEnd)//既促销又上新
            {
                listShopOrnamentNewList.Add(storeDefine);
            }
            else
            {
                listShopOrnamentPromotionList.Add(storeDefine);
            }
        }
        else if (nowTime > storeDefine.UpNewTimeBegin && nowTime < storeDefine.UpNewTimeEnd)//上新
        {
            listShopOrnamentNewList.Add(storeDefine);
        }
        else
        {
            listShopOrnamentNorMalList.Add(storeDefine);
        }
    }
    /// <summary>
    /// 商城装饰配置
    /// </summary>
    private void ShopOrnamentConfig()
    {
        if (listShopOrnamentNewList.Count != 0)
        {
            for (int i = 0; i < listShopOrnamentNewList.Count; i++)
            {
                listShopOrnamentList.Add(listShopOrnamentNewList[i]);
            }
        }
        if (listShopOrnamentPromotionList.Count != 0)
        {
            for (int i = 0; i < listShopOrnamentPromotionList.Count; i++)
            {
                listShopOrnamentList.Add(listShopOrnamentPromotionList[i]);
            }
        }
        if (listShopOrnamentNorMalList.Count != 0)
        {
            for (int i = 0; i < listShopOrnamentNorMalList.Count; i++)
            {
                listShopOrnamentList.Add(listShopOrnamentNorMalList[i]);
            }
        }
    }

    #endregion

}
