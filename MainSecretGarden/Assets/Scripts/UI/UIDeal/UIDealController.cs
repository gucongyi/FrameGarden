using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 订单控制类
/// </summary>
public class UIDealController : MonoBehaviour
{
    #region 变量
    [SerializeField]
    private Transform _dealBG;
    [SerializeField]
    private CanvasGroup _dealCG;

    [SerializeField]
    private LoopVerticalScrollRect loopVerticalScrollRect;

    [SerializeField]
    private Button _butHelper;

    [SerializeField]
    private Button _butClose;

    [SerializeField]
    private Button _butBGClose;

    [SerializeField]
    private Text _level1CompleteDeal;
    [SerializeField]
    private Text _level2CompleteDeal;
    [SerializeField]
    private Text _level3CompleteDeal;

    #region 规则说明

    [SerializeField]
    private GameObject _ruleDesc;
    [SerializeField]
    private Transform _ruleDescBG;
    [SerializeField]
    private Button _ruleDescButClose;
    #endregion


    private static UIDealController _instance;
    public static UIDealController Instance
    {
        get
        {
            return _instance;
        }
    }

    #endregion

    #region 方法

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        UniversalTool.ReadyUIAnimTwo(_dealCG, _dealBG);
        UniversalTool.StartUIAnimTwo(_dealCG, _dealBG);
    }


    private void Init()
    {
        _butHelper.onClick.RemoveAllListeners();
        _butHelper.onClick.AddListener(OnClickHelper);

        _butClose.onClick.RemoveAllListeners();
        _butClose.onClick.AddListener(OnClickClose);

        _butBGClose.onClick.RemoveAllListeners();
        _butBGClose.onClick.AddListener(OnClickClose);
    }

    public void InitValue()
    {
        Init();

        DealSorting();
        UpdateOrdinaryDealNum();
        UpdateIntermediateDealNum();
        UpdateAdvancedDealNum();
    }

    /// <summary>
    /// 零点更新订单数据
    /// </summary>
    public void UpdateDealUI() 
    {
        //UpdateLoopLength(StaticData.playerInfoData.CurrDeals.DealInfo.Count);
        DealSorting();
    }


    /// <summary>
    /// 普通
    /// </summary>
    public void UpdateOrdinaryDealNum()
    {
        int num = 0;
        if (StaticData.playerInfoData.CurrDeals.DealNum.Count > 0)
            num = StaticData.playerInfoData.CurrDeals.DealNum[0];

        string desc = LocalizationDefineHelper.GetStringNameById(120081);
        desc += LocalizationDefineHelper.GetStringNameById(120139);
        desc += num;
        _level1CompleteDeal.text = desc;
    }

    /// <summary>
    /// 中级
    /// </summary>
    public void UpdateIntermediateDealNum()
    {
        int num = 0;
        if (StaticData.playerInfoData.CurrDeals.DealNum.Count > 1)
            num = StaticData.playerInfoData.CurrDeals.DealNum[1];

        string desc = LocalizationDefineHelper.GetStringNameById(120270);
        desc += LocalizationDefineHelper.GetStringNameById(120139);
        desc += num;
        _level2CompleteDeal.text = desc;
    }

    /// <summary>
    /// 高级
    /// </summary>
    public void UpdateAdvancedDealNum()
    {
        int num = 0;
        if (StaticData.playerInfoData.CurrDeals.DealNum.Count > 2)
            num = StaticData.playerInfoData.CurrDeals.DealNum[2];

        string desc = LocalizationDefineHelper.GetStringNameById(120271);
        desc += LocalizationDefineHelper.GetStringNameById(120139);
        desc += num;
        _level3CompleteDeal.text = desc;
    }

    /// <summary>
    /// 更新循环列表长度且重新刷新
    /// </summary>
    /// <param name="length"></param>
    private void UpdateLoopLength(int length)
    {
        loopVerticalScrollRect.totalCount = length;
        loopVerticalScrollRect.RefillCells();
    }

    private void OnClickClose()
    {

        if (_ruleDesc.activeInHierarchy)
            OnClickRuleDescClose();

        UniversalTool.CancelUIAnimTwo(_dealCG, _dealBG, UIClose);
    }

    private void OnClickHelper()
    {
        OpenRuleDesc();
    }

    private void UIClose() 
    {
        //当前没有红点 通知庄园刷新红点
        if (!StaticData.IsSubmintDeal())
        {
            Root2dSceneManager._instance.NotifyUpdateDealRedDot();
        }

        UIComponent.RemoveUI(UIType.UIDeal);
    }

    /// <summary>
    /// 去种植
    /// </summary>
    public void ToPlant()
    {
        OnClickClose();
        //打开一个可以种植的空地块
        StaticData.OnOpenPlantFromDeal();
    }


    /// <summary>
    /// 刷新通知
    /// </summary>
    public Action RefreshNotify;

    /// <summary>
    /// 通知提交订单完成
    /// </summary>
    public void NotifySubmintDealComplete() 
    {
        RefreshNotify?.Invoke();
    }

    /// <summary>
    /// 订单排序
    /// </summary>
    public void DealSorting() 
    {
        //可以提交的订单
        List<SCCreateDeal> submintDeals = new List<SCCreateDeal>();
        //不可以提交的订单
        List<SCCreateDeal> notSubmintDeals = new List<SCCreateDeal>();
        //等待刷新的订单
        List<SCCreateDeal> waitRefreshDeals = new List<SCCreateDeal>();

        for (int i = 0; i < StaticData.playerInfoData.CurrDeals.DealInfo.Count; i++)
        {
            //等待刷新订单
            if (StaticData.playerInfoData.CurrDeals.DealInfo[i].DealRefreshTime > TimeHelper.ServerTimeStampNow)
            {
                waitRefreshDeals.Add(StaticData.playerInfoData.CurrDeals.DealInfo[i]);
            }
            else 
            {
                //是否可以提交
                bool isSubmint = false;
                for (int j = 0; j < StaticData.playerInfoData.CurrDeals.DealInfo[i].DealNeedGoods.Count; j++)
                {
                    int GoodNum = StaticData.GetWareHouseItem(StaticData.playerInfoData.CurrDeals.DealInfo[i].DealNeedGoods[j].GoodId).GoodNum;
                    if (GoodNum < StaticData.playerInfoData.CurrDeals.DealInfo[i].DealNeedGoods[j].GoodNum)
                    {
                        isSubmint = false;
                        break;
                    }
                    else
                    {
                        isSubmint = true;
                    }
                }
                if (isSubmint)
                {
                    submintDeals.Add(StaticData.playerInfoData.CurrDeals.DealInfo[i]);
                }
                else 
                {
                    notSubmintDeals.Add(StaticData.playerInfoData.CurrDeals.DealInfo[i]);
                }
            }
        }

        StaticData.playerInfoData.CurrDeals.DealInfo.Clear();
        StaticData.playerInfoData.CurrDeals.DealInfo.AddRange(submintDeals);
        StaticData.playerInfoData.CurrDeals.DealInfo.AddRange(notSubmintDeals);
        StaticData.playerInfoData.CurrDeals.DealInfo.AddRange(waitRefreshDeals);

        UpdateLoopLength(StaticData.playerInfoData.CurrDeals.DealInfo.Count);
    }

    #region 订单规则

    /// <summary>
    /// 打开规则说明
    /// </summary>
    private void OpenRuleDesc() 
    {
        _ruleDescButClose.onClick.RemoveAllListeners();
        _ruleDescButClose.onClick.AddListener(OnClickRuleDescClose);
        UniversalTool.ReadyPopupAnim(_ruleDescBG);
        _ruleDesc.SetActive(true);
        UniversalTool.StartPopupAnim(_ruleDescBG);
    }

    private void OnClickRuleDescClose() 
    {
        UniversalTool.StartPopupAnim(_ruleDescBG, RuleDescUIClose);
    }
    private void RuleDescUIClose() 
    {
        _ruleDesc.SetActive(false);
    }

    #endregion

    #endregion
}
