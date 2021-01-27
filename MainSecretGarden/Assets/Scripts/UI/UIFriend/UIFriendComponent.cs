using Quick.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Game.Protocal;
using Google.Protobuf.Collections;
using UnityEngine.UI;
using System;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Linq;
using Google.Protobuf;

public class UIFriendComponent : MonoBehaviour
{
    public TabGroup tabGroup;
    private List<Tab> otherTabs = new List<Tab>();
    public SCSearch scSearchInfo;
    public bool isRecommendUI = true;  //当前是推荐UI还是搜索UI
    [HideInInspector]
    public GameObject focusDeleteItem;

    public Transform _tipDeleteFriendTF;
    public Transform _friendAmountTF;
    public Transform _containerTF;
    public Transform _searchTF;
    public Transform _buttonRefresh;

    public LoopVerticalScrollRect lsFriendList;
    public LoopVerticalScrollRect lsApplyList;
    public LoopVerticalScrollRect lsRecommendList;
    public LoopVerticalScrollRect lsFriendStealList;

    private Transform _bgMask;
    private Transform _buttonClose;
    private Transform _buttonConfirm;
    private Transform _buttonCancle;
    private Transform _buttonMask;
    private Transform _tabFriendList;
    private Transform _tabRecommend;
    private Transform _textFriendAmount;
    private Transform _textNumInfo;
    private Transform _friendManorAddBtn;
    private Transform _friendManorContainer;
    private Transform _buttonSearchGrey;
    private Transform _buttonRefreshGrey;
    private Transform _QCharacter;
    private Transform _QCharacterEmptyFriend;
    private Transform _QCharacterEmptyApply;
    private Transform _QCharacterEmptySearch;

    private RepeatedField<SCFriendInfo> onlineFriendList = new RepeatedField<SCFriendInfo>();
    private RepeatedField<SCFriendInfo> outlineFriendList = new RepeatedField<SCFriendInfo>();
    private RepeatedField<SCFriendInfo> onlineOtherList = new RepeatedField<SCFriendInfo>();
    private RepeatedField<SCFriendInfo> outlineOtherList = new RepeatedField<SCFriendInfo>();

    private int limitAmountFriend = 10;
    public bool isFriendUIOpened = false;//庄园好友是否显示
    private bool isFirstRequestFriendInfo = false;//是否请求过好友列表数据
    private bool isRegisterTabEvent = false;//是否注册过tab事件监听
    private bool isShowFriendManorUI = false;
    private TimeCountDownComponent refreshTimeCountDown;
    private TimeCountDownComponent searchTimeCountDown;
    #region 庄园好友
    public Button ButtonSwitchFriendManor;
    #endregion

    public enum FriendTabTags
    {
        TabFriendList = 0,
        TabFriendApply = 1,
        TabRecommend = 2
    };
    private void Awake()
    {
        Initial();
        UniversalTool.ReadyUIAnimTwo(GetComponent<CanvasGroup>(), _containerTF);
    }
    void Start()
    {
        RegisterEventListener();
        RegisterTabListener();
    }
    private void OnEnable()
    {
        UniversalTool.StartUIAnimTwo(GetComponent<CanvasGroup>(), _containerTF);
    }
    private void OnDisable()
    {
        UniversalTool.ReadyUIAnimTwo(GetComponent<CanvasGroup>(), _containerTF);
    }
    private void Initial()
    {
        _bgMask = transform.Find("Container/BgMask");
        _buttonClose = transform.Find("Container/ButtonClose");
        _buttonConfirm = _tipDeleteFriendTF.Find("ButtonConfirm");
        _buttonCancle = _tipDeleteFriendTF.Find("ButtonCancle");
        _buttonMask = _tipDeleteFriendTF.Find("ButtonMask");
        _tabFriendList = tabGroup.transform.Find("TabFriendList");
        _tabRecommend = tabGroup.transform.Find("TabRecommend");
        _textFriendAmount = _friendAmountTF.Find("TextFriendAmount");
        _textNumInfo = _friendAmountTF.Find("TextNumInfo");
        _friendManorContainer = transform.Find("FriendManor/Container");
        _friendManorAddBtn = transform.Find("FriendManor/Container/ButtonAdd");
        _buttonSearchGrey = _searchTF.Find("ButtonSearchGrey");
        _buttonRefreshGrey = _searchTF.Find("ButtonRefreshGrey");
        _QCharacter = transform.Find("Container/QCharacter");
        _QCharacterEmptyFriend = _QCharacter.Find("EmptyFriend");
        _QCharacterEmptyApply = _QCharacter.Find("EmptyApply");
        _QCharacterEmptySearch = _QCharacter.Find("EmptySearch");
        //ButtonSwitchFriendManor.onClick.RemoveAllListeners();
        //ButtonSwitchFriendManor.onClick.AddListener(OpenAndCloseFriendUI);
        SetMultilingual();
    }
    /// <summary>
    /// 显示好友UI或者庄园好友UI
    /// </summary>
    /// <param name="isShowFriendManor">是否显示庄园好友</param>
    public void ShowFriendUI(bool isShowFriendManor = false)
    {
        StaticData.RefreshFriendManorList(() => { ShowFriendUITwo(isShowFriendManor); });
    }
    /// <summary>
    /// 显示好友UI或者庄园好友UI
    /// </summary>
    /// <param name="isShowFriendManor">是否显示庄园好友</param>
    public void ShowFriendUITwo(bool isShowFriendManor = false)
    {
        isShowFriendManorUI = isShowFriendManor;
        if (false)
        {
            SetFriendUIAndManorActive(false);
            if (isFriendUIOpened)
            {
                StaticData.RefreshFriendManorList(() =>
                {
                    if (StaticData.playerInfoData.listFriendStealInfo.Count == 0)
                    {
                        _friendManorAddBtn.gameObject.SetActive(true);
                        //ShowRecommendUI();
                    }
                    else
                    {
                        _friendManorAddBtn.gameObject.SetActive(false);
                    }
                });
            }
        }
        else
        {
            SetFriendUIAndManorActive(true);
            Tab curTab = _tabFriendList.GetComponent<Tab>();
            tabGroup.TurnTabOn(FriendTabTags.TabFriendList.ToString());
            ResetAllTabTitle(tabGroup.curTab);
            RefreshFriendList();
        }
    }
    /// <summary>
    /// 切换庄园好友和普通好友UI的激活状态
    /// </summary>
    /// <param name="containerActive"></param>
    private void SetFriendUIAndManorActive(bool containerActive)
    {
        //_containerTF.gameObject.SetActive(containerActive);
        //transform.Find("Image").gameObject.SetActive(containerActive);
        //_friendManorTF.gameObject.SetActive(!containerActive);
    }
    /// <summary>
    /// 在庄园好友为空时显示推荐
    /// </summary>
    public void ShowRecommendUI()
    {
        SetFriendUIAndManorActive(true);
        RegisterEventListener();
        RegisterTabListener();
        Tab curTab = _tabRecommend.GetComponent<Tab>();
        tabGroup.TurnTabOn(FriendTabTags.TabRecommend.ToString(), (Tab defTab) =>
        {
            defTab.page.IsOn = true;
        });
        ResetAllTabTitle(tabGroup.curTab);
        RefreshRecommendList();
    }
    private void RegisterEventListener()
    {
        _bgMask.GetComponent<Button>().onClick.RemoveAllListeners();
        _bgMask.GetComponent<Button>().onClick.AddListener(OnHideFriendUI);
        _buttonClose.GetComponent<Button>().onClick.RemoveAllListeners();
        _buttonClose.GetComponent<Button>().onClick.AddListener(OnHideFriendUI);
        _buttonConfirm.GetComponent<Button>().onClick.RemoveAllListeners();
        _buttonConfirm.GetComponent<Button>().onClick.AddListener(OnClickDeleteFriend);
        _buttonCancle.GetComponent<Button>().onClick.RemoveAllListeners();
        _buttonCancle.GetComponent<Button>().onClick.AddListener(OnClickCancleDelete);
        _buttonMask.GetComponent<Button>().onClick.RemoveAllListeners();
        _buttonMask.GetComponent<Button>().onClick.AddListener(OnClickCancleDelete);
        //_friendManorAddBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        //_friendManorAddBtn.GetComponent<Button>().onClick.AddListener(ShowRecommendUI);
    }
    private void RegisterTabListener()
    {
        if (tabGroup.IsActive() && !isRegisterTabEvent)
        {
            tabGroup = tabGroup.GetComponent<TabGroup>();
            tabGroup.AddTabsClickEvent(OnTabClick);
            isRegisterTabEvent = true;
        }
    }
    /// <summary>
    /// 点击关闭好友界面
    /// </summary>
    private void OnHideFriendUI()
    {
        UniversalTool.CancelUIAnimTwo(GetComponent<CanvasGroup>(), _containerTF, () =>
        {
            if (isShowFriendManorUI)
            {
                //_friendManorTF.gameObject.SetActive(isShowFriendManorUI);
                //_containerTF.gameObject.SetActive(!isShowFriendManorUI);
                UIComponent.HideUI(UIType.UIFriend); 
            }
            else
            {
                UIComponent.HideUI(UIType.UIFriend);
            }
        });


    }
    /// <summary>
    /// Tab点击
    /// </summary>
    /// <param name="target"></param>
    /// <param name="eventDatas"></param>
    private void OnTabClick(Tab target, PointerEventData eventDatas)
    {
        if (tabGroup.curTab == target)
        {
            return;
        }
        ResetAllTabTitle(target);
        if (target.tag == FriendTabTags.TabFriendList.ToString())
        {
            //点击好友列表
            RefreshFriendList();
        }
        else if (target.tag == FriendTabTags.TabFriendApply.ToString())
        {
            //点击申请列表
            RefreshApplyList();
        }
        else if (target.tag == FriendTabTags.TabRecommend.ToString())
        {
            //点击推荐列表
            RefreshRecommendList();
        }
    }
    /// <summary>
    /// 点击删除好友
    /// </summary>
    public void OnClickDeleteFriend()
    {
        if (focusDeleteItem != null)
        {
            var scFriendInfo = focusDeleteItem.GetComponent<UIPoolItemFriendInfo>().scFriendInfo;
            // 请求删除好友
            CSDelFriend csDelFriend = new CSDelFriend()
            {
                OperationUid = scFriendInfo.Uid
            };
            ProtocalManager.Instance().SendCSDelFriend(csDelFriend, (serverRes) =>
            {
                StaticData.playerInfoData.listFriendInfo.RemoveAt(focusDeleteItem.GetComponent<UIPoolItemFriendInfo>().itemIndex);
                //Destroy(focusDeleteItem);
                GenerateFriendListUI();
                _tipDeleteFriendTF.gameObject.SetActive(false);
                focusDeleteItem = null;
                string tips = LocalizationDefineHelper.GetStringNameById(120143);
                StaticData.CreateToastTips(tips);
                GenerateFriendAmountUI(true);
            }, (error) => { });
        }
    }
    /// <summary>
    /// 点击取消删除
    /// </summary>
    public void OnClickCancleDelete()
    {
        StaticData.DebugGreen("取消删除好友~~~");
        _tipDeleteFriendTF.gameObject.SetActive(false);
        focusDeleteItem = null;
    }
    /// <summary>
    /// 重置所有tab的标题状态
    /// </summary>
    /// <param name="curTab"></param>
    private void ResetAllTabTitle(Tab curTab)
    {
        var otherTabs = tabGroup.GetOtherTabs(curTab);
        for (int i = 0; i < otherTabs.Count; i++)
        {
            Tab tab = otherTabs[i];
            tab.page.IsOn = false;
            tab.GraphicSetActive();
        }

        //otherTabs = tabGroup.GetOtherTabs(curTab);
        //bool isFirstNotThirdTab = true;
        //if (curTab.tag == "TabRecommend")
        //{
        //    isFirstNotThirdTab = false;
        //}
        //StaticData.DebugGreen(curTab.tag);
        ////启用其他tab的title未选中状态，禁用自己的title未选中状态
        ////foreach (Tab tab in otherTabs)
        //for (int i = 0; i < otherTabs.Count; i++)
        //{
        //    Tab tab = otherTabs[i];
        //    Transform textIsNotOnTF = tab.transform.GetChild(0).Find("TextIsNotOn");
        //    if (textIsNotOnTF != null)
        //    {
        //        textIsNotOnTF.gameObject.SetActive(true);
        //    }
        //    if (tab.tag == "TabFriendApply")
        //    {
        //        tab.transform.Find("Background").gameObject.SetActive(!isFirstNotThirdTab);
        //        tab.transform.Find("Background2").gameObject.SetActive(isFirstNotThirdTab);
        //    }
        //    tab.transform.SetSiblingIndex(otherTabs.Count - 1 - i);
        //    tab.page.IsOn = false;
        //}
        //curTab.transform.SetSiblingIndex(2);
        //curTab.page.IsOn = true;
        //Transform curTextIsNotOnTF = curTab.transform.GetChild(0).Find("TextIsNotOn");
        //if (curTextIsNotOnTF != null)
        //{
        //    curTextIsNotOnTF.gameObject.SetActive(true);
        //}
    }

    /// <summary>
    /// 刷新好友UI列表
    /// </summary>
    private void RefreshFriendList()
    {
        RefreshContentTopUI(true);
        GetFriendList();
        //if (!isFirstRequestFriendInfo)
        //{
        //    GetFriendList();
        //}
        //else
        //{
        //    GenerateFriendListUI();
        //}
    }
    /// <summary>
    /// 刷新申请列表
    /// </summary>
    private void RefreshApplyList()
    {
        RefreshContentTopUI(true);
        GetApplyList();
        //if (!isFirstRequestApplyInfo)
        //{
        //    GetApplyList();
        //}
        //else
        //{
        //    GenerateApplyListUI();
        //}
    }
    /// <summary>
    /// 刷新推荐列表
    /// </summary>
    public void RefreshRecommendList()
    {
        _buttonRefresh.gameObject.SetActive(true);
        _QCharacter.gameObject.SetActive(false);
        RefreshContentTopUI(false);
        if (GenerateRefreshTimerCount())
        {
            GetRecommendList();
        }
    }
    /// <summary>
    /// 生成刷新推荐定时器
    /// </summary>
    private bool GenerateRefreshTimerCount()
    {
        bool isRecovery = false;
        if (refreshTimeCountDown == null)
        {
            isRecovery = true;
            RecoveryRefresh(false);
            refreshTimeCountDown = StaticData.CreateTimerRebackMilliSeconds(5, true, (go) =>
            {
                Destroy(go);
                Destroy(refreshTimeCountDown.gameObject);
                RecoveryRefresh(true);
            },
            (remainTime) =>
            {
                float fillAmount = remainTime / 5.0f;
                _buttonRefreshGrey.GetComponent<Image>().fillAmount = fillAmount;
            });
        }
        return isRecovery;
    }
    /// <summary>
    /// 恢复推荐刷新按钮
    /// </summary>
    /// <param name="isRecovery"></param>
    private void RecoveryRefresh(bool isRecovery)
    {
        _buttonRefreshGrey.gameObject.SetActive(!isRecovery);
    }
    /// <summary>
    /// 搜索玩家，直接在按钮上调用
    /// </summary>
    /// <param name="inputTF"></param>
    public void RefreshSearchdList(Transform inputTF)
    {
        GenerateSearchTimerCount();
        GetSearchList(inputTF);
    }
    /// <summary>
    /// 生成搜索定时器
    /// </summary>
    private void GenerateSearchTimerCount()
    {
        if (searchTimeCountDown == null)
        {
            RecoverySearch(false);
            searchTimeCountDown = StaticData.CreateTimerRebackMilliSeconds(1, true, (go) =>
            {
                Destroy(go);
                Destroy(searchTimeCountDown.gameObject);
                RecoverySearch(true);
            },
            (remainTime) =>
            {
                float fillAmount = remainTime / 1.0f;
                _buttonSearchGrey.GetComponent<Image>().fillAmount = fillAmount;
            });
        }
    }
    /// <summary>
    /// 恢复搜索按钮
    /// </summary>
    /// <param name="isRecovery"></param>
    private void RecoverySearch(bool isRecovery)
    {
        _buttonSearchGrey.gameObject.SetActive(!isRecovery);
    }
    /// <summary>
    /// 刷新好友列表和申请列表顶部的文字提示
    /// </summary>
    /// <param name="isFriendAmount"></param>
    private void RefreshContentTopUI(bool isFriendAmount)
    {
        _friendAmountTF.gameObject.SetActive(isFriendAmount);
        _searchTF.gameObject.SetActive(!isFriendAmount);

    }
    /// <summary>
    /// 从服务器获取玩家推荐列表数据
    /// </summary>
    private void GetRecommendList()
    {
        StaticData.DebugGreen("请求玩家推荐列表数据~~~");
        StaticData.playerInfoData.listRecommendInfo.Clear();
        // 正式请求
        CSEmptyRecommendList csEmptyRecommendList = new CSEmptyRecommendList();
        ProtocalManager.Instance().SendCSEmptyRecommendList(csEmptyRecommendList, (recommendList) =>
        {
            if (recommendList == null)
            {
                GenerateRecommendListUI();
                return;
            }
            StaticData.DebugGreen("收到推荐回复~~~");
            StaticData.playerInfoData.listRecommendInfo.AddRange(recommendList.RecommendListInfo);
            SortRecommendList();
            GenerateRecommendListUI();
        }, (error) =>
        {
            StaticData.DebugGreen($"{error}");
        }, false);
    }
    /// <summary>
    /// 从服务器获取好友申请列表数据
    /// </summary>
    private void GetApplyList()
    {
        StaticData.DebugGreen("请求好友申请列表数据~~~");
        StaticData.playerInfoData.listApplyInfo.Clear();
        // 正式请求
        CSEmptyApplyList csEmptyApplyList = new CSEmptyApplyList();
        ProtocalManager.Instance().SendCSEmptyApplyList(csEmptyApplyList, (applyList) =>
        {
            //isFirstRequestApplyInfo = true;
            if (applyList == null)
            {
                GenerateApplyListUI();
                return;
            }
            StaticData.DebugGreen("收到好友申请回复~~~");
            StaticData.playerInfoData.listApplyInfo.AddRange(applyList.ApplyListInfo);
            SortApplyList();
            GenerateApplyListUI();
        }, (error) =>
        {
            StaticData.DebugGreen($"收到好友申请列表错误码:{error.ToString()}~~~");
        }, false);
    }
    /// <summary>
    /// 从服务器获取好友列表数据
    /// </summary>
    private void GetFriendList()
    {
        StaticData.isGetFriendList = true;
        StaticData.DebugGreen("请求好友列表数据~~~");
        StaticData.playerInfoData.listFriendInfo.Clear();
        // 正式请求
        CSEmptySCFriendList csemptyscfriendlist = new CSEmptySCFriendList();
        ProtocalManager.Instance().SendCSEmptySCFriendList(csemptyscfriendlist, (friendList) =>
        {
            //isFirstRequestFriendInfo = true;
            if (friendList == null)
            {
                GenerateFriendListUI();
                return;
            }

            StaticData.playerInfoData.listFriendInfo.AddRange(friendList.FriendListInfo);
            limitAmountFriend = friendList.FriendAmountLimit;
            SortFriendList();
            GenerateFriendListUI();
        }, (error) => { });
    }
    /// <summary>
    /// 好友排序
    /// </summary>
    private void SortFriendList()
    {
        onlineFriendList.Clear();
        outlineFriendList.Clear();
        foreach (var elem in StaticData.playerInfoData.listFriendInfo)
        {
            if (elem.Online)
            {
                onlineFriendList.Add(elem);
            }
            else
            {
                outlineFriendList.Add(elem);
            }
        }
        onlineFriendList = StaticData.RepeatedFieldSortT(onlineFriendList, FriendExpSort, true);
        outlineFriendList = StaticData.RepeatedFieldSortT(outlineFriendList, FriendExpSort, true);
        StaticData.playerInfoData.listFriendInfo.Clear();
        StaticData.playerInfoData.listFriendInfo.AddRange(onlineFriendList);
        StaticData.playerInfoData.listFriendInfo.AddRange(outlineFriendList);
    }
    /// <summary>
    /// 好友经验排序
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private int FriendExpSort(SCFriendInfo x, SCFriendInfo y)
    {
        if (x.FriendExperience < y.FriendExperience)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }
    /// <summary>
    /// 申请列表排序
    /// </summary>
    private void SortApplyList()
    {
        onlineOtherList.Clear();
        outlineOtherList.Clear();
        foreach (var elem in StaticData.playerInfoData.listApplyInfo)
        {
            if (elem.Online)
            {
                onlineOtherList.Add(elem);
            }
            else
            {
                outlineOtherList.Add(elem);
            }
        }
        onlineOtherList = StaticData.RepeatedFieldSortT(onlineOtherList, OtherExpSort, true);
        outlineOtherList = StaticData.RepeatedFieldSortT(outlineOtherList, OtherExpSort, true);
        StaticData.playerInfoData.listApplyInfo.Clear();
        StaticData.playerInfoData.listApplyInfo.AddRange(onlineOtherList);
        StaticData.playerInfoData.listApplyInfo.AddRange(outlineOtherList);
    }
    /// <summary>
    /// 推荐列表排序
    /// </summary>
    private void SortRecommendList()
    {
        onlineOtherList.Clear();
        outlineOtherList.Clear();
        foreach (var elem in StaticData.playerInfoData.listRecommendInfo)
        {
            if (elem.Online)
            {
                onlineOtherList.Add(elem);
            }
            else
            {
                outlineOtherList.Add(elem);
            }
        }
        onlineOtherList = StaticData.RepeatedFieldSortT(onlineOtherList, OtherExpSort, true);
        outlineOtherList = StaticData.RepeatedFieldSortT(outlineOtherList, OtherExpSort, true);
        StaticData.playerInfoData.listRecommendInfo.Clear();
        StaticData.playerInfoData.listRecommendInfo.AddRange(onlineOtherList);
        StaticData.playerInfoData.listRecommendInfo.AddRange(outlineOtherList);
    }
    /// <summary>
    /// 比较玩家经验，用的other结构
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private int OtherExpSort(SCFriendInfo x, SCFriendInfo y)
    {
        if (x.FriendExperience < y.FriendExperience)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }
    /// <summary>
    /// 从服务器获取玩家搜索数据
    /// </summary>
    private void GetSearchList(Transform inputTF)
    {
        StaticData.DebugGreen("请求玩家搜索数据~~~");
        //获取输入框内容
        string inputText = inputTF.GetComponent<Text>().text;
        if (inputText.Length == 0)
        {
            //提示弹窗
            string tipStr = LocalizationDefineHelper.GetStringNameById(120125);
            StaticData.CreateToastTips(tipStr);
            return;
        }
        //if (inputText == StaticData.Uid.ToString()) 
        //{
        //    string searchSelfTip = LocalizationDefineHelper.GetStringNameById(120132);
        //    StaticData.CreateToastTips(searchSelfTip);
        //    return;
        //}
        _buttonRefresh.gameObject.SetActive(false);
        _buttonRefreshGrey.gameObject.SetActive(false);
        scSearchInfo = new SCSearch();
        CSSearch csSearch = new CSSearch()
        {
            SearchContext = inputText
        };
        ProtocalManager.Instance().SendCSSearch(csSearch, (scSearch) =>
        {
            if (scSearch == null)
            {
                StaticData.DebugGreen("搜索玩家结果为空~~~");
                ShowEmptyTip(FriendTabTags.TabRecommend);
                GenerateSearchListUI(0);
                return;
            }
            scSearchInfo = scSearch;
            int playerNum = scSearchInfo.Search.Count;
            GenerateSearchListUI(playerNum);
        }, (error) => { });
    }
    /// <summary>
    /// 生成玩家搜索列表UI
    /// </summary>
    private void GenerateSearchListUI(int playerCount)
    {
        isRecommendUI = false;
        lsRecommendList.ClearCells();
        lsRecommendList.totalCount = playerCount;
        if (lsRecommendList.totalCount > 0)
        {
            _QCharacter.gameObject.SetActive(false);
        }
        else
        {
            ShowEmptyTip(FriendTabTags.TabRecommend);
        }
        lsRecommendList.RefillCells();
    }
    /// <summary>
    /// 生成好友推荐列表UI
    /// </summary>
    private void GenerateRecommendListUI()
    {
        isRecommendUI = true;
        lsRecommendList.ClearCells();
        lsRecommendList.totalCount = StaticData.playerInfoData.listRecommendInfo.Count;
        lsRecommendList.RefillCells();
    }
    /// <summary>
    /// 生成好友申请列表UI
    /// </summary>
    public void GenerateApplyListUI()
    {
        lsApplyList.ClearCells();
        lsApplyList.totalCount = StaticData.playerInfoData.listApplyInfo.Count;
        if (lsApplyList.totalCount > 0)
        {
            _QCharacter.gameObject.SetActive(false);
        }
        else
        {
            ShowEmptyTip(FriendTabTags.TabFriendApply);
        }
        lsApplyList.RefillCells();
        GenerateFriendAmountUI(false);
    }
    /// <summary>
    /// 生成好友列表UI
    /// </summary>
    private void GenerateFriendListUI()
    {
        lsFriendList.ClearCells();
        lsFriendList.totalCount = StaticData.playerInfoData.listFriendInfo.Count;
        if (lsFriendList.totalCount > 0)
        {
            _QCharacter.gameObject.SetActive(false);
        }
        else
        {
            ShowEmptyTip(FriendTabTags.TabFriendList);
        }
        lsFriendList.RefillCells();
        GenerateFriendAmountUI(true);
    }

    /// <summary>
    /// 生成好友数量UI
    /// </summary>
    /// <param name="isFriendList"></param>
    private void GenerateFriendAmountUI(bool isFriendList)
    {
        if (isFriendList)
        {
            int onlineNum = GetOnlineAmount();
            _textFriendAmount.GetComponent<Text>().text = LocalizationDefineHelper.GetStringNameById(120109);
            string textNumInfo = onlineNum.ToString() + "/" + StaticData.playerInfoData.listFriendInfo.Count.ToString();
            _textNumInfo.GetComponent<Text>().text = textNumInfo;
            //_friendAmountTF.gameObject.SetActive(true);
            //_textFriendAmount.GetComponent<Text>().text = "已有好友";
            //string textNumInfo = "(" + StaticData.playerInfoData.listFriendInfo.Count.ToString() + "/" + limitAmountFriend.ToString() + ")";
            //_textNumInfo.GetComponent<Text>().text = textNumInfo;
        }
        else
        {
            //_friendAmountTF.gameObject.SetActive(false);
            _textFriendAmount.GetComponent<Text>().text = LocalizationDefineHelper.GetStringNameById(120124);
            string textNumInfo = StaticData.playerInfoData.listFriendInfo.Count.ToString() + "/" + limitAmountFriend.ToString();
            _textNumInfo.GetComponent<Text>().text = textNumInfo;
        }
    }
    /// <summary>
    /// 获取好友在线数量
    /// </summary>
    /// <returns></returns>
    private int GetOnlineAmount()
    {
        int onlineNum = 0;
        foreach (var friendInfo in StaticData.playerInfoData.listFriendInfo)
        {
            if (friendInfo.Online)
            {
                onlineNum += 1;
            }
        }
        return onlineNum;
    }
    /// <summary>
    /// 关闭好友UI，按钮上直接调用
    /// </summary>
    public void CloseFriendUI()
    {
        UIComponent.RemoveUI(UIType.UIFriend);
    }
    /// <summary>
    /// 删除好友推送的回调
    /// </summary>
    public void OnDeletedCmdCallBack()
    {
        RefreshFriendList();
    }
    /// <summary>
    /// 收到好友申请推送的回调
    /// </summary>
    public void OnApplyCmdCallBack()
    {
        RefreshApplyList();
    }
    /// <summary>
    /// 收到好友同意推送的回调
    /// </summary>
    public void OnAgreeCmdCallBack()
    {
        RefreshFriendList();
    }
    /// <summary>
    /// 列表中数据为空时显示对应的提示UI
    /// </summary>
    /// <param name="friendTabTags"></param>
    private void ShowEmptyTip(FriendTabTags friendTabTags)
    {
        _QCharacter.gameObject.SetActive(true);
        //隐藏所有的提示语句
        _QCharacterEmptyFriend.gameObject.SetActive(false);
        _QCharacterEmptyApply.gameObject.SetActive(false);
        _QCharacterEmptySearch.gameObject.SetActive(false);
        //显示需要的一个
        switch (friendTabTags)
        {
            case FriendTabTags.TabFriendList:
                _QCharacterEmptyFriend.gameObject.SetActive(true);
                break;
            case FriendTabTags.TabFriendApply:
                _QCharacterEmptyApply.gameObject.SetActive(true);
                break;
            case FriendTabTags.TabRecommend:
                _QCharacterEmptySearch.gameObject.SetActive(true);
                break;
        }
    }
    /// <summary>
    /// 设置多语言
    /// </summary>
    public void SetMultilingual()
    {
        _tabFriendList.Find("Background/TextIsNotOn").GetComponent<Text>().text = StaticData.GetMultilingual(120106);
        _tabFriendList.Find("Checkmark/TextIsOn").GetComponent<Text>().text = StaticData.GetMultilingual(120106);
        _tabRecommend.Find("Background/TextIsNotOn").GetComponent<Text>().text = StaticData.GetMultilingual(120108);
        _tabRecommend.Find("Checkmark/TextIsOn").GetComponent<Text>().text = StaticData.GetMultilingual(120108);
        tabGroup.transform.Find("TabFriendApply/Background/TextIsNotOn").GetComponent<Text>().text = StaticData.GetMultilingual(120107);
        tabGroup.transform.Find("TabFriendApply/Background2/TextIsNotOn").GetComponent<Text>().text = StaticData.GetMultilingual(120107);
        tabGroup.transform.Find("TabFriendApply/Checkmark/TextIsOn").GetComponent<Text>().text = StaticData.GetMultilingual(120107);
        _searchTF.Find("InputField/Placeholder").GetComponent<Text>().text = StaticData.GetMultilingual(120116);

        _QCharacterEmptyFriend.GetComponent<Text>().text = StaticData.GetMultilingual(120144);
        _QCharacterEmptyApply.GetComponent<Text>().text = StaticData.GetMultilingual(120145);
        _QCharacterEmptySearch.GetComponent<Text>().text = StaticData.GetMultilingual(120146);
    }
}
