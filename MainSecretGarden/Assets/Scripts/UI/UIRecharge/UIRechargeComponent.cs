using Company.Cfg;
using Quick.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIRechargeComponent : MonoBehaviour
{
    public Transform _tra;

    public TabGroup tabGroup;

    public Transform _bgMask;
    public Transform _buttonClose;
    public Transform _tabGold;
    public Transform _tabDiamond;

    public LoopVerticalScrollRect lsGoldList;
    public LoopVerticalScrollRect lsDiamondList;

    public List<GoldJewelBuyDefine> listGoldConfig = new List<GoldJewelBuyDefine>();
    public List<GoldJewelBuyDefine> listDiamondConfig = new List<GoldJewelBuyDefine>();

    private bool initFlag = false;
    Action OnReturnClickCallBack;
    public enum RechargeTabTags 
    {
        TabGold = 0,
        TabDiamond = 1
    }

    private void Awake()
    {
        UniversalTool.ReadyPopupAnim(_tra);
    }

    void Start()
    {
        Init();
        GenerateGoldListUI();
    }
    private void Init() 
    {
        if (!initFlag) 
        {
            RegisterEventListener();
            //RegisterTabListener();
            GetGoldJewelBuyConfigData();
            initFlag = true;
        }
    }

    private void OnEnable()
    {
        UniversalTool.StartPopupAnim(_tra);
    }

    private void RegisterTabListener()
    {
        tabGroup = tabGroup.GetComponent<TabGroup>();
        tabGroup.AddTabsClickEvent(OnTabClick);
    }
    private void RegisterEventListener()
    {
        _bgMask.GetComponent<Button>().onClick.RemoveAllListeners();
        _bgMask.GetComponent<Button>().onClick.AddListener(OnHideRechargeUI);
        _buttonClose.GetComponent<Button>().onClick.RemoveAllListeners();
        _buttonClose.GetComponent<Button>().onClick.AddListener(OnHideRechargeUI);
    }
    private void OnHideRechargeUI()
    {
        UniversalTool.CancelPopAnim(_tra, UIClose);
    }

    private void UIClose() 
    {
        UIComponent.HideUI(UIType.UIRecharge);
        OnReturnClickCallBack?.Invoke();
    }

    private void OnTabClick(Tab target, PointerEventData eventDatas)
    {
        if (tabGroup.curTab == target)
        {
            return;
        }
        ResetAllTabTitle(target);
        if (target.tag == RechargeTabTags.TabGold.ToString())
        {
            GenerateGoldListUI();
        }
        else if (target.tag == RechargeTabTags.TabDiamond.ToString())
        {
            GenerateDiamondListUI();
        }
    }

    private void GetGoldJewelBuyConfigData() 
    {
        listGoldConfig.Clear();
        listDiamondConfig.Clear();
        var goldJewelBuyC = StaticData.configExcel.GoldJewelBuy;
        foreach (var elem in goldJewelBuyC)
        {
            
            if (elem.GoodsId == StaticData.configExcel.GetVertical().GoldGoodsId) 
            {
                listGoldConfig.Add(elem);
            }
            else 
            {
                listDiamondConfig.Add(elem);
            }
        }
    }
    private void GenerateGoldListUI() 
    {
        lsGoldList.ClearCells();
        lsGoldList.totalCount = listGoldConfig.Count;
        lsGoldList.RefillCells();
    }
    private void GenerateDiamondListUI()
    {
        lsDiamondList.ClearCells();
        lsDiamondList.totalCount = listDiamondConfig.Count;
        lsDiamondList.RefillCells();
    }
    private void ResetAllTabTitle(Tab curTab)
    {
        var otherTabs = tabGroup.GetOtherTabs(curTab);
        if (otherTabs == null)
        {
            return;
        }
        for (int i = 0; i < otherTabs.Count; i++)
        {
            Tab tab = otherTabs[i];
            tab.page.IsOn = false;
        }
    }
    public void ShowRechargeGoldUI(Action OnReturnClickCallBack = null)
    {
        Init();
        this.OnReturnClickCallBack = OnReturnClickCallBack;
        Tab curTab = _tabGold.GetComponent<Tab>();
        tabGroup.TurnTabOn(RechargeTabTags.TabGold.ToString(), (Tab defTab) =>
        {
            defTab.page.IsOn = true;
        });
        ResetAllTabTitle(curTab);
        GenerateGoldListUI();
    }
    public void ShowRechargeDiamondUI(Action OnReturnClickCallBack = null)
    {
        Init();
        this.OnReturnClickCallBack = OnReturnClickCallBack;
        Tab curTab = _tabDiamond.GetComponent<Tab>();
        tabGroup.TurnTabOn(RechargeTabTags.TabDiamond.ToString(), (Tab defTab) =>
        {
            defTab.page.IsOn = true;
        });
        ResetAllTabTitle(curTab);
        GenerateDiamondListUI();
    }
}
