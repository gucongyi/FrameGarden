using Company.Cfg;
using Cysharp.Threading.Tasks;
using Game.Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 订单项
/// </summary>
public class UIDealItem : MonoBehaviour, InterfaceScrollCell
{
    #region 变量

    [SerializeField]
    private GameObject _defDealItem;
    [SerializeField]
    private GameObject _dealLevel1BG;
    [SerializeField]
    private Text _dealLevel1Desc;
    [SerializeField]
    private GameObject _dealLevel2BG;
    [SerializeField]
    private Text _dealLevel2Desc;
    [SerializeField]
    private GameObject _dealLevel3BG;
    [SerializeField]
    private Text _dealLevel3Desc;

    [SerializeField]
    private Image _roleIcon;
    [SerializeField]
    private Text _roleName;

    [SerializeField]
    private Image _currency1Icon;
    [SerializeField]
    private Text _currency1Value;

    [SerializeField]
    private Image _currency2Icon;
    [SerializeField]
    private Text _currency2Value;

    [SerializeField]
    private GameObject _demandItem;
    [SerializeField]
    private GameObject _demandItem1;
    [SerializeField]
    private GameObject _demandItem2;

    [SerializeField]
    private Image _refreshIcon;// 刷新需求值Icon
    [SerializeField]
    private Text _refreshValue;// 刷新需求值

    [SerializeField]
    private Button _butRefresh;// 刷新
    [SerializeField]
    private Button _butSubmint;// 提交

    [SerializeField]
    private GameObject _waitRefresh;

    [SerializeField]
    private Text _waitRefreshTime;

    [SerializeField]
    private Button _butWaitRefresh;//加速


    [SerializeField]
    private GameObject _waitBGLevel1;
    [SerializeField]
    private GameObject _waitBGLevel2;
    [SerializeField]
    private GameObject _waitBGLevel3;


    /// <summary>
    /// 当前订单
    /// </summary>
    private SCCreateDeal _currDeal = new SCCreateDeal();

    private float _timer;

    /// <summary>
    /// 等待刷新总时间
    /// </summary>
    private int _waitTotalSeconds;

    /// <summary>
    /// 是否可以提交订单
    /// </summary>
    private bool _isSubmitDeal = false;
    /// <summary>
    /// 缺少的需要的物品的名称
    /// </summary>
    private List<string> _lackNeedGoodNames = new List<string>();

    #endregion

    #region 方法
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //
        UpdateWaitTime();
    }

    private void Init() 
    {
        _butRefresh.onClick.RemoveAllListeners();
        _butRefresh.onClick.AddListener(OnClickRefreshDeal);

        _butSubmint.onClick.RemoveAllListeners();
        _butSubmint.onClick.AddListener(OnClickSubmintDeal);

        _butWaitRefresh.onClick.RemoveAllListeners();
        _butWaitRefresh.onClick.AddListener(OnClickAdvSkipDeal);

    }



    public void ScrollCellIndex(int idx)
    {
        //
        Init();

        //获取订单数据
        _currDeal = null;
        if (StaticData.playerInfoData.CurrDeals.DealInfo.Count > idx)
            _currDeal = StaticData.playerInfoData.CurrDeals.DealInfo[idx];

        //添加监听
        if (UIDealController.Instance != null)
        {
            UIDealController.Instance.RefreshNotify -= DealControllerNotifyRefreshShow;
            UIDealController.Instance.RefreshNotify += DealControllerNotifyRefreshShow;
        }

        UpdateShow(true);
    }

    /// <summary>
    /// 更新界面显示
    /// </summary>
    private void UpdateShow(bool isInit = false) 
    {
        //更新排序
        if (!isInit) 
        {
            UIDealController.Instance.DealSorting();
            return;
        }
            
        _waitTotalSeconds = 0;
        //判断等待刷新还是默认
        if (_currDeal.DealRefreshTime < TimeHelper.ServerTimeStampNow) //订单刷新时间 对比 服务器当前时间
        {
            _defDealItem.SetActive(true);
            _waitRefresh.SetActive(false);
            SetDefDeal();
        }
        else
        {
            _defDealItem.SetActive(false);
            _waitRefresh.SetActive(true);
            SetWaitRefresh();
        }

    }

    private async void SetDefDeal() 
    {
        //订单等级
        _dealLevel1BG.SetActive(false);
        _dealLevel2BG.SetActive(false);
        _dealLevel3BG.SetActive(false);
        Text desc = null;
        switch (_currDeal.DealType) 
        {
            case DealType.LowLevel:
                _dealLevel1BG.SetActive(true);
                desc = _dealLevel1Desc;
                break;
            case DealType.MiddleLevel:
                _dealLevel2BG.SetActive(true);
                desc = _dealLevel2Desc;
                break;
            case DealType.HighLevel:
                _dealLevel3BG.SetActive(true);
                desc = _dealLevel3Desc;
                break;
            default:
                _dealLevel1BG.SetActive(true);
                desc = _dealLevel1Desc;
                break;
        }

        //获取订单角色
        var role = StaticData.configExcel.GetDealRoleByDealRoleID(_currDeal.RoleID);

        desc.text = LocalizationDefineHelper.GetStringNameById(_currDeal.DescID);
        _roleIcon.sprite = await ABManager.GetAssetAsync<Sprite>(role.Icon);
        _roleName.text = LocalizationDefineHelper.GetStringNameById(role.Name);


        _currency1Icon.transform.parent.gameObject.SetActive(false);
        _currency2Icon.transform.parent.gameObject.SetActive(false);

        GameItemDefine item = null;
        //奖励
        for (int i = 0; i < _currDeal.DealAwardInfo.Count; i++)
        {
            if (i == 0)
            {
                _currency1Icon.transform.parent.gameObject.SetActive(true);
                item = StaticData.configExcel.GetGameItemByID(_currDeal.DealAwardInfo[i].GoodId);
                _currency1Icon.sprite = await ABManager.GetAssetAsync<Sprite>(item.Icon);
                _currency1Value.text = _currDeal.DealAwardInfo[i].GoodNum.ToString();
            }
            else if (i == 1) 
            {
                _currency2Icon.transform.parent.gameObject.SetActive(true);
                item = StaticData.configExcel.GetGameItemByID(_currDeal.DealAwardInfo[i].GoodId);
                _currency2Icon.sprite = await ABManager.GetAssetAsync<Sprite>(item.Icon);
                _currency2Value.text = _currDeal.DealAwardInfo[i].GoodNum.ToString();
            }
        }

        //需求物品
        UpdateDealNeedGoods();

        //刷新需求值Icon
        //刷新需求值
        var need = GetRefreshDealConsume(_currDeal.DealType);
        item = StaticData.configExcel.GetGameItemByID(need.ID);
        //itemNum = StaticData.GetWareHouseItem(need.ID).GoodNum;
        _refreshIcon.sprite = await ABManager.GetAssetAsync<Sprite>(item.Icon);
        _refreshValue.text = need.Count.ToString();
    }

    /// <summary>
    /// 获取需求物品数量显示颜色变换
    /// </summary>
    /// <param name="haveNum"></param>
    /// <param name="needNum"></param>
    /// <returns></returns>
    private string GetNeedNumShow(int haveNum, int needNum, string itemName = null) 
    {
        string desc = string.Empty;
        if (haveNum < needNum)
        {
            desc = "<color=#ff5494>"+ haveNum + "</color>"+"/"+ needNum;
            _isSubmitDeal = false;
            //
            if (itemName != null)
                _lackNeedGoodNames.Add(itemName);
        }
        else 
        {
            desc = haveNum + "/" + needNum;
        }
        return desc;
    }

    /// <summary>
    /// 更新需求
    /// </summary>
    private void UpdateDealNeedGoods()
    {
        _demandItem.SetActive(false);
        _demandItem1.SetActive(false);
        _demandItem2.SetActive(false);

        _isSubmitDeal = true;
        _lackNeedGoodNames.Clear();

        GameItemDefine item = null;
        string ownAndDemand = string.Empty;
        int itemNum = 0;
        //需求
        for (int i = 0; i < _currDeal.DealNeedGoods.Count; i++)
        {
            item = StaticData.configExcel.GetGameItemByID(_currDeal.DealNeedGoods[i].GoodId);
            itemNum = StaticData.GetWareHouseItem(_currDeal.DealNeedGoods[i].GoodId).GoodNum;
            ownAndDemand = GetNeedNumShow(itemNum, _currDeal.DealNeedGoods[i].GoodNum, LocalizationDefineHelper.GetStringNameById(item.ItemName));

            if (i == 0)
            {
                _demandItem.SetActive(true);
                _demandItem.GetComponent<UIItemShow>().InitValue(item, ownAndDemand);
            }
            else if (i == 1)
            {
                _demandItem1.SetActive(true);
                _demandItem1.GetComponent<UIItemShow>().InitValue(item, ownAndDemand);
            }
            else if (i == 2)
            {
                _demandItem2.SetActive(true);
                _demandItem2.GetComponent<UIItemShow>().InitValue(item, ownAndDemand);
            }
        }
        //用于处理需求不满足修改按钮颜色
        //if (!_isSubmitDeal)
        //{
        //    _butSubmint.gameObject.GetComponent<Image>().color = Color.white * 0.8f;//Color.gray;
        //}
        //else 
        //{
        //    _butSubmint.gameObject.GetComponent<Image>().color = Color.white;
        //}
    }

    private void SetWaitRefresh() 
    {
        _waitBGLevel1.SetActive(false);
        _waitBGLevel2.SetActive(false);
        _waitBGLevel3.SetActive(false);
        switch (_currDeal.DealType)
        {
            case DealType.LowLevel:
                _waitBGLevel1.SetActive(true);
                break;
            case DealType.MiddleLevel:
                _waitBGLevel2.SetActive(true);
                break;
            case DealType.HighLevel:
                _waitBGLevel3.SetActive(true);
                break;
            default:
                _waitBGLevel1.SetActive(true);
                break;
        }

        //单位毫秒
        var waitTime = _currDeal.DealRefreshTime - TimeHelper.ServerTimeStampNow;
        _waitTotalSeconds = (int)(waitTime / 1000f);
        _waitTotalSeconds += 3;
        _waitRefreshTime.text = TimeHelper.FormatTime(_waitTotalSeconds);
    }

    /// <summary>
    /// 更新等待时间
    /// </summary>
    private void UpdateWaitTime() 
    {
        if (_waitTotalSeconds > 0)
        {
            _timer += Time.deltaTime;
            if (_timer >= 1)
            {
                _waitTotalSeconds -= 1;
                _timer -= 1;
                //
                _waitRefreshTime.text = TimeHelper.FormatTime(_waitTotalSeconds);
                if (_waitTotalSeconds <= 0) 
                {
                    _waitTotalSeconds = 0;
                    UpdateToDefDeal();
                }
            }
        }
    }
    /// <summary>
    /// 更新到默认订单
    /// </summary>
    private async void UpdateToDefDeal() 
    {
        //等待0.2f
        await UniTask.Delay(200);
        UpdateShow();
    }


    /// <summary>
    /// 提交订单
    /// </summary>
    private void OnClickSubmintDeal()
    {
        //条件是否满足
        if (!_isSubmitDeal) 
        {
            //取消 120075
            //去种植 120281
            string name = string.Empty;
            if (_lackNeedGoodNames.Count > 0) 
            {
                name = _lackNeedGoodNames[0];
            }
            name = "<b><size=46><color=#FDF3BB>" + name + "</color></size></b>";

            //提示内容
            string desc = LocalizationDefineHelper.GetStringNameById(120282);
            desc = "<size=36><color=#427CD2>" + desc + "</color></size>";
            desc = string.Format(desc, name) ;//120282
            //条件不满足
            StaticData.OpenCommonTips(desc, 120281, UIDealController.Instance.ToPlant, null, 120075);
            return;
        }
        //新手引导标记完成
        if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.isCurrStepGuiding)
        {
            GuideCanvasComponent._instance.SetLittleStepFinish();
        }
        _butSubmint.interactable = false;
        StaticData.NotifyServerSubmintDeal(_currDeal.DealId, SubmintDealCallback);
    }


    private List<CSWareHouseStruct> _showAwardDatas = new List<CSWareHouseStruct>();

    /// <summary>
    /// 当前订单类型
    /// </summary>
    private DealType _curDealType = DealType.None;
    private async void SubmintDealCallback(SCSubmintDeal sCSubmintDeal) 
    {

        if (sCSubmintDeal == null) 
        {
            _butSubmint.interactable = true; 
            return;
        }
        _showAwardDatas.Clear();

        _curDealType = _currDeal.DealType;

        //发放奖励 道具入库
        for (int i = 0; i < _currDeal.DealAwardInfo.Count; i++)
        {
            //需要展示的物品
            CSWareHouseStruct cSWareHouseStruct = new CSWareHouseStruct();
            cSWareHouseStruct.GoodId = _currDeal.DealAwardInfo[i].GoodId;
            cSWareHouseStruct.GoodNum = _currDeal.DealAwardInfo[i].GoodNum;
            cSWareHouseStruct.IsLock = true;
            _showAwardDatas.Add(cSWareHouseStruct);

            var item = StaticData.configExcel.GetGameItemByID(cSWareHouseStruct.GoodId);
            var itemSprite = await ABManager.GetAssetAsync<Sprite>(item.Icon);
            //世界坐标转换为屏幕坐标
            var pos = Camera.main.WorldToScreenPoint(transform.position);
            StaticData.OpenPickupItemEffectNew(itemSprite, 0.35f, pos, cSWareHouseStruct.GoodId, cSWareHouseStruct.GoodNum, null/*PickUpItemEffectComplete*/);
        }
        //移除需要的物品
        for (int i = 0; i < _currDeal.DealNeedGoods.Count; i++)
        {
            StaticData.UpdateWareHouseItem(_currDeal.DealNeedGoods[i].GoodId, _currDeal.DealNeedGoods[i].GoodNum * -1);
        }

        //更新完成订单数
        int dealLevel = (int)_currDeal.DealType - 1;
        dealLevel = dealLevel >= 0 ? dealLevel : 0;
        StaticData.AddDealCompleteValue(dealLevel);

        //更新订单数据
        _currDeal = sCSubmintDeal.NewDealInfo;
        StaticData.UpdateDeal(_currDeal);

        //道具入库
        ItemPropsWarehousing();

    }

    /// <summary>
    /// 拾取物品特效完成
    /// </summary>
    /// <param name="itemID"></param>
    private void PickUpItemEffectComplete(int itemID)
    {
        //_butSubmint.interactable = true;
        //for (int i = 0; i < _showAwardDatas.Count; i++)
        //{
        //    if (_showAwardDatas[i].GoodId == itemID)
        //    {
        //        if (_showAwardDatas[i].GoodId == StaticData.configExcel.GetVertical().ExpId)
        //        {
        //            //奖励为经验 
        //            StaticData.AddPlayerExp(_currDeal.DealAwardInfo[i].GoodNum);
        //        }
        //        else 
        //        {
        //            StaticData.UpdateWareHouseItem(_showAwardDatas[i].GoodId, _showAwardDatas[i].GoodNum);
        //        }
        //        _showAwardDatas.RemoveAt(i);
        //        break;
        //    }
        //}

        //if (_showAwardDatas.Count == 0)
        //{
        //    SubmintDealComplete();
        //}
    }

    /// <summary>
    /// 道具入库
    /// </summary>
    private void ItemPropsWarehousing()
    {
        _butSubmint.interactable = true;
        for (int i = 0; i < _showAwardDatas.Count; i++)
        {
            if (_showAwardDatas[i].GoodId == StaticData.configExcel.GetVertical().ExpId)
            {
                //奖励为经验 
                StaticData.AddPlayerExp(_currDeal.DealAwardInfo[i].GoodNum);
            }
            else
            {
                StaticData.UpdateWareHouseItem(_showAwardDatas[i].GoodId, _showAwardDatas[i].GoodNum);
            }
        }

        _showAwardDatas.Clear();
        SubmintDealComplete();
    }

    /// <summary>
    /// 提交订单完成
    /// </summary>
    private void SubmintDealComplete() 
    {
        //更新显示
        UpdateShow();
        //更新主界面显示
        if (UIDealController.Instance != null)
        {
            switch (_curDealType)
            {
                case DealType.MiddleLevel:
                    UIDealController.Instance.UpdateIntermediateDealNum();
                    break;
                case DealType.HighLevel:
                    UIDealController.Instance.UpdateAdvancedDealNum();
                    break;
                default:
                    UIDealController.Instance.UpdateOrdinaryDealNum();
                    break;
            }

            //通知提交订单完成
            UIDealController.Instance.NotifySubmintDealComplete();
            _curDealType = DealType.None;
            //提示获取的物品
            //StaticData.OpenCommonReceiveAwardTips(StaticData.GetMultilingual(120290), StaticData.GetMultilingual(120195), "", ShowAwardCallback, null, _showAwardDatas);

        }
    }

    


    /// <summary>
    /// 刷新订单
    /// </summary>
    private void OnClickRefreshDeal() 
    {
        //1.刷新条件是否满足
        var need = GetRefreshDealConsume(_currDeal.DealType);
        int itemNum = StaticData.GetWareHouseItem(need.ID).GoodNum;
        if (itemNum < need.Count) 
        {
            //道具不足提示 //只有钻石
            UnlockView();
            return;
        }

        StaticData.NotifyServerRefreshDeal(_currDeal.DealId, RefreshDealCallback);
    }

    private void UnlockView()
    {
        string str = StaticData.GetMultilingual(120243);//钻石不足，需要购买吗
        StaticData.OpenCommonTips(str, 120010, async () =>
        {
            await StaticData.OpenRechargeUI(1);
        }, null, 120075);
    }

    private void RefreshDealCallback(SCRefreshDeal sCRefreshDeal) 
    {
        if (sCRefreshDeal == null)
        {
            return;
        }

        //扣除需求 钻石
        var need = GetRefreshDealConsume(_currDeal.DealType);
        StaticData.UpdateWareHouseDiamond((int)need.Count * -1);

        _currDeal.DealAwardInfo.Clear();
        _currDeal.DealAwardInfo.AddRange(sCRefreshDeal.DealAwardInfo);
        _currDeal.DealNeedGoods.Clear();
        _currDeal.DealNeedGoods.AddRange(sCRefreshDeal.DealNeedGoods);
        _currDeal.RoleID = sCRefreshDeal.RoleID;
        _currDeal.DescID = sCRefreshDeal.DescID;
        //更新订单数据
        StaticData.UpdateDeal(_currDeal);
        //更新显示
        UpdateShow();
    }

    /// <summary>
    /// 订单广告跳过刷新时间
    /// </summary>
    private void OnClickAdvSkipDeal() 
    {
        StaticData.OpenAd("GuessAd", (code, msg) => {
            if (code == 1)
            {
                //数据打点
                StaticData.DataDot(DotEventId.GuessBeanAd);
                //
                AdvSkipDeal();
            }
            else
            {
                //已经统一处理了
            }
        });
    }

    /// <summary>
    /// 广告成功
    /// </summary>
    private void AdvSkipDeal() 
    {
        StaticData.NotifyServerAdvSkipDeal(_currDeal.DealId, AdvSkipDealCallback);
    }

    private void AdvSkipDealCallback(bool isSuccess) 
    {
        if (!isSuccess) 
        {
            return;
        }
        //更新数据
        _currDeal.DealRefreshTime = 0;
        StaticData.UpdateDeal(_currDeal);
        //更新显示
        UpdateShow();
    }

    /// <summary>
    /// 获取刷新订单消耗
    /// </summary>
    /// <returns></returns>
    private GoodIDCount GetRefreshDealConsume(DealType dealType) 
    {

        var curDealInf = StaticData.configExcel.Deal.Find( dealInf => dealInf.DealLevel == (int)dealType);
        if (curDealInf != null) 
        {
            return curDealInf.RefreshDealUseGoods;
        }
        return null;
    }

    /// <summary>
    /// 订单控制器通知订单订单需求刷新
    /// </summary>
    private void DealControllerNotifyRefreshShow()
    {
        //判断等待刷新还是默认
        if (_currDeal.DealRefreshTime > TimeHelper.ServerTimeStampNow) //订单刷新时间 对比 服务器当前时间
        {
            return;
        }
        UpdateDealNeedGoods();
    }

    #endregion
}
