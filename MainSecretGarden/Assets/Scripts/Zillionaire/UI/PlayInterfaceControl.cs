using DG.Tweening;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 大富翁游戏界面控制器
/// </summary>
public class PlayInterfaceControl : MonoBehaviour
{
    #region 字段

    private Animator _animator;

    /// <summary>
    /// 顶部
    /// </summary>
    private Transform _topTra;

    //private Transform _left;

    /// <summary>
    /// 中部
    /// </summary>
    private Transform _middleTra;
    ///// <summary>
    ///// 底部
    ///// </summary>
    //Transform _bottomTra;


    /// <summary>
    /// 投掷骰子控制
    /// </summary>
    private CycleSlidingList _cycleSliding;

    /// <summary>
    /// 返回按钮
    /// </summary>
    private Button _outBtn;

    /// <summary>
    /// 当前体力显示
    /// </summary>
    private Text _curPowerShow;

    private Image _curPowerImage;

    /// <summary>
    /// 暂存奖励显示
    /// </summary>
    private GameObject _earnItemObj;
    private Transform _earnContent;

    /// <summary>
    /// 引导滑动图标
    /// </summary>
    private Image _guidIcon;

    /// <summary>
    /// 游戏骰子
    /// </summary>
    private DiceController _diceController;

    /// <summary>
    /// 当前骰子点数
    /// </summary>
    private int _currDiceIndex;

    /// <summary>
    /// 是否需要显示色子按钮
    /// </summary>
    private bool _isShowDice = true;

    /// <summary>
    /// 色子回调事件
    /// </summary>
    private Action _diceCallback;

    /// <summary>
    /// 投掷色子服务器回调信息
    /// </summary>
    private SCDiceResult _diceCallbackInfoNew = null;

    /// <summary>
    /// 激活了的随机事件列表
    /// </summary>
    private List<int> _activeRandomEventList = new List<int>();


    /// <summary>
    /// 选中的骰子的类型
    /// </summary>
    private DiceType _selectDiceType = DiceType.PureDice;

    /// <summary>
    /// 当前获得的奖励
    /// </summary>
    private Dictionary<int, Text> _curEarnRewards = new Dictionary<int, Text>();

    /// <summary>
    /// 购买体力次数
    /// </summary>
    private int _curBuyPorwerNum;

    #endregion

    /// <summary>
    /// 是否需要显示色子按钮
    /// </summary>
    public bool IsShowDice { get { return _isShowDice; } }

    /// <summary>
    /// 当前使用的色子
    /// </summary>
    private DiceController _curDiceController { get { return _diceController; } }

    #region 函数
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Initial()
    {
        _animator = transform.GetComponent<Animator>();
        _topTra = transform.Find("Top");
        //_left = transform.Find("Left");
        _middleTra = transform.Find("Middle");
        //_bottomTra = transform.Find("Bottom");

        StaticData.CreateCoinNav(_topTra.Find("UICurrencyInfo/GoldTra"));
        StaticData.CreateDiamondNav(_topTra.Find("UICurrencyInfo/DiamondTra"));
        StaticData.CreateWaterNav(_topTra.Find("UICurrencyInfo/WaterTra"));

        //_throwLowBtn = _middleTra.Find("Scroll View/Viewport/Content/Throw_Low/ButBG").GetComponent<Button>();
        //_throwLowIcon = _middleTra.Find("Scroll View/Viewport/Content/Throw_Low/Icon").GetComponent<Image>();
        //_throwLowBtn.onClick.RemoveListener(OnClickThrowBtn);
        //_throwLowBtn.onClick.AddListener(OnClickThrowBtn);

        //_throwRandomBtn = _middleTra.Find("Scroll View/Viewport/Content/Throw_Random/ButBG").GetComponent<Button>();
        //_throwRandomIcon = _middleTra.Find("Scroll View/Viewport/Content/Throw_Random/Icon").GetComponent<Image>();
        //_throwRandomBtn.onClick.RemoveListener(OnClickThrowBtn);
        //_throwRandomBtn.onClick.AddListener(OnClickThrowBtn);

        //_throwHighBtn = _middleTra.Find("Scroll View/Viewport/Content/Throw_High/ButBG").GetComponent<Button>();
        //_throwHighIcon = _middleTra.Find("Scroll View/Viewport/Content/Throw_High/Icon").GetComponent<Image>();
        //_throwHighBtn.onClick.RemoveListener(OnClickThrowBtn);
        //_throwHighBtn.onClick.AddListener(OnClickThrowBtn);

        _cycleSliding = _middleTra.Find("ScrollViewDice").GetComponent<CycleSlidingList>();
        _cycleSliding.InitValue(OnClickThrowBtn, CurSelectDice);

        _outBtn = _topTra.Find("Return").GetComponent<Button>();
        _outBtn.onClick.RemoveListener(OnClickOut);
        _outBtn.onClick.AddListener(OnClickOut);

        _curPowerShow = _middleTra.Find("PhysicalStrength/Num").GetComponent<Text>();
        _curPowerImage = _middleTra.Find("PhysicalStrength/BG/Icon").GetComponent<Image>();


        _earnItemObj = _topTra.Find("PanelEarnRewards/EarnItem").gameObject;
        _earnContent = _topTra.Find("PanelEarnRewards/Scroll View").GetComponent<ScrollRect>().content;
        _earnItemObj.transform.parent.gameObject.SetActive(false);

        _guidIcon = _middleTra.Find("GuidIcon").GetComponent<Image>();
        _guidIcon.gameObject.SetActive(false);

        InitialDice();

        UpdatePowerShow(true);

    }

    /// <summary>
    /// 获取暂存奖励的世界位置
    /// </summary>
    /// <returns></returns>
    public Vector3 GetEarnRewardsWorldPos() 
    {
        return _topTra.Find("PanelEarnRewards").transform.position;
    }

    /// <summary>
    /// 当前选中的骰子的下标
    /// </summary>
    /// <param name="index"></param>
    private void CurSelectDice(int index) 
    {
        switch (index)
        {
            case 0:
                _selectDiceType = DiceType.LowDice;
                break;
            case 1:
                _selectDiceType = DiceType.PureDice;
                break;
            case 2:
                _selectDiceType = DiceType.HighDice;
                break;
            default:
                _selectDiceType = DiceType.PureDice;
                break;
        }
        Debug.Log("当前选中的骰子的下标 index = "+ index);
        
        
    }

    /// <summary>
    /// 获取当前骰子的消耗
    /// </summary>
    /// <returns></returns>
    public int GetCurDiceConsume() 
    {
        return ZillionaireToolManager.GetDiceConsume(_selectDiceType);
    }

    /// <summary>
    /// 数据重置
    /// </summary>
    public void ResetData()
    {
        _currDiceIndex = 0;

        //
        _cycleSliding.RestItemPos();

        //销毁已经获得的物品显示
        _curEarnRewards.Clear();
        for (int i = 0; i < _earnContent.childCount; i++)
        {
            Destroy(_earnContent.GetChild(i).gameObject);
        }
        _earnItemObj.transform.parent.gameObject.SetActive(false);

        _curEarnRewards.Clear();

        //更新体力显示
        UpdatePowerShow(true);

        //重新显示骰子
        _isShowDice = true;
        _middleTra.gameObject.SetActive(true);

    }

    /// <summary>
    /// 初始化骰子
    /// </summary>
    private async void InitialDice()
    {
        string dicePath = "Dice";
        GameObject Obj = await ABManager.GetAssetAsync<GameObject>(dicePath);
        Transform tra = ZillionairePlayerManager._instance.CurrentPlayer.transform.parent;
        GameObject diceObj = Instantiate(Obj, tra);
        _diceController = diceObj.GetComponent<DiceController>();
        _diceController.NotifyPlayDiceAnim(6, null);
        _diceController.InitIsThrow();
        //diceObj.SetActive(false);
    }

    #region 随机事件

    /// <summary>
    /// 是否需要播放随机事件动画
    /// </summary>
    /// <param name="randomEventList"></param>
    /// <returns></returns>
    private bool IsNeedPlayRandomEventAnim(List<int> randomEventList) 
    {
        if (_activeRandomEventList.Count <= 0)
            return true;

        //2个list是否相等
        if (_activeRandomEventList.Count == randomEventList.Count) 
        {
            foreach (var item in _activeRandomEventList)
            {
                if (!randomEventList.Contains(item))
                    return false;
            }
            return true;
        }
        if (_activeRandomEventList.Equals(randomEventList)) 
            return false;

        return true;
    }

    /// <summary>
    /// 更新随机事件格子效果
    /// </summary>
    /// <param name="randomEventList"></param>
    private void UpdateRandomEventGritEffect(List<int> randomEventList) 
    {
        //1.移除之前的格子效果
        for (int i = 0; i < _activeRandomEventList.Count; i++)
        {
            if (randomEventList != null && randomEventList.Contains(_activeRandomEventList[i]))
                continue;
            //移除格子效果激活
            ZillionaireGameMapManager._instance.GetGridDataByID(_activeRandomEventList[i]).NotifyUpdateRandomEvent(false);
            _activeRandomEventList.RemoveAt(i);
            i--;
        }

        if (randomEventList == null)
            return;

        for (int i = 0; i < randomEventList.Count; i++)
        {
            if (_activeRandomEventList.Contains(randomEventList[i]))
                continue;
            ZillionaireGameMapManager._instance.GetGridDataByID(randomEventList[i]).NotifyUpdateRandomEvent(true);
        }

        _activeRandomEventList.Clear();
        _activeRandomEventList.AddRange(randomEventList);
    }

    #endregion

    /// <summary>
    /// 点击返回
    /// </summary>
    private void OnClickOut()
    {
        if (!_isShowDice)
            return;
        //
        OpenSettlementTips();
    }

    /// <summary>
    /// 打开结算提示界面
    /// </summary>
    private void OpenSettlementTips()
    {
        string desc = LocalizationDefineHelper.GetStringNameById(120018);

        //120217 结算
        StaticData.OpenCommonTips(desc, 120217, EnterSettlement);
    }

    /// <summary>
    /// 进入结算
    /// </summary>
    private void EnterSettlement()
    {
        Debug.Log("EnterSettlement 进入结算");
        ZillionaireToolManager.NotifyServerGameSettlement(EnterSettlementCallback);
    }

    /// <summary>
    /// 结算回调
    /// </summary>
    /// <param name="isSuccess"></param>
    private void EnterSettlementCallback(bool isSuccess) 
    {
        if (isSuccess)
        {
            ClickOutCallbackSucceeded();
        }
        else 
        {
            ClickOutCallbackFailed();
        }
    }

    /// <summary>
    /// 结算成功返回
    /// </summary>
    private void ClickOutCallbackSucceeded()
    {
        Debug.Log("打开结算界面!!!");
        //
        StaticData.OpenSettlement(ZillionairePlayerManager._instance.CurrentPlayer.CurPlayerRewards);
        //道具入库
        foreach (var item in ZillionairePlayerManager._instance.CurrentPlayer.CurPlayerRewards)
        {
            if (item.Key != 0 && item.Value != 0)
                StaticData.UpdateWareHouseItem(item.Key, item.Value);
        }
        //经验添加 11-26 确定不加经验了
        //StaticData.AddPlayerExp(ZillionaireToolManager.GetMapExp());
    }

    /// <summary>
    /// 结算失败
    /// </summary>
    private void ClickOutCallbackFailed()
    {
        Debug.Log("大富翁结算 失败");
    }


    /// <summary>
    /// 角色移动
    /// </summary>
    /// <param name="isReverseMove"> 是否反向移动 </param>
    public void RoleMove(bool isReverseMove = false)
    {
        if (isReverseMove)
        {
            ZillionaireGameMapManager._instance.SearchRoute(_currDiceIndex *= -1);
            return;
        }
        ZillionaireGameMapManager._instance.SearchRoute(_currDiceIndex);
    }

    /// <summary>
    /// 激活事件完成回调
    /// </summary>
    private void ActionEvenCallback() 
    {
        //当前骰子回调执行完成
        CurDiceActionEnd();
        return;
    }

    /// <summary>
    /// 通过格子id查询格子的下标
    /// </summary>
    /// <param name="dataID"></param>
    /// <returns></returns>
    private int FindGridIndex(int dataID) 
    {
        foreach (var item in ZillionaireGameMapManager._instance.CurZillionaireMapControl.MapGridDic)
        {
            if (item.Value.GridInfo.ID == dataID)
                return item.Key;
        }
        return 0;
    }

    /// <summary>
    /// 当前色子执行完成
    /// </summary>
    private void CurDiceActionEnd() 
    {
        //更新随机激活事件
        List<int> loc = new List<int>();
        //服务器格子id从1开始 客户端从0开始
        for (int i = 0; i < _diceCallbackInfoNew.Info[0].Location.Count; i++)
        {
            int index = FindGridIndex(_diceCallbackInfoNew.Info[0].Location[i]);
            Debug.Log(string.Format("更新随机激活事件 eventActiveID = {0}// index = {1} ", _diceCallbackInfoNew.Info[0].Location[i], index));
            loc.Add(index);
        }
        if (loc.Count> 0 && IsNeedPlayRandomEventAnim(loc))
        {
            //播放获得随机事件效果
            Debug.Log("播放获得随机事件效果!!!");
            ZillionaireGameMapManager._instance.CurZillionaireMapControl.CurMapEventManager.PlaySpawnRandomEvnetEffectTips();
        }
        //更新格子事件效果
        UpdateRandomEventGritEffect(loc);
        //移除本次事件
        _diceCallbackInfoNew.Info.RemoveAt(0);
        //继续向下执行
        CurRoleMoveEnd();
    }

    /// <summary>
    /// 当前玩家移动完成
    /// </summary>
    public void CurRoleMoveEnd()
    {
        //没有可以执行的东西了
        if (_diceCallbackInfoNew == null || _diceCallbackInfoNew.Info == null || _diceCallbackInfoNew.Info.Count <= 0)
        {
            Debug.Log("CurRoleMoveEnd 当前玩家移动完成 通知色子可以继续！！！");
            _isShowDice = true;
            _middleTra.gameObject.SetActive(true);
            //体力不足最小骰子消耗
            if (!ZillionairePlayerManager._instance.CurrentPlayer.IsMinPower())
            {
                Debug.Log("体力不足不可以投掷骰子！！！");
                StaticData.OpenBuyPower(ConfirmBuyItems, CancelBuyPower);
            }

            return;
        }

        //获取移动终点格子
        var grid = ZillionaireGameMapManager._instance.GetGridDataByID(ZillionairePlayerManager._instance.CurrentPlayer.CurGridID);
        Debug.Log("当前移动到的格子id："+ ZillionairePlayerManager._instance.CurrentPlayer.CurGridID);
        Debug.Log("当前移动到的格子的数据的id："+ grid.GridInfo.ID);

        //激活事件 随机事件 //激活普通事件
        if (grid.IsActiveEvent || grid.GridInfo.IsEvent)
        {
            ZillionaireGameMapManager._instance.CurZillionaireMapControl.CurMapEventManager.ActiveEvent(_diceCallbackInfoNew.Info[0], grid, ActionEvenCallback);
            return;
        }
        
        //领取奖励
        if (grid.GridInfo.BasicReward != null && grid.GridInfo.BasicReward.ID != 0)
        {
            Debug.Log("CurRoleMoveEnd 只有奖励");
            //
            int baseNum = (int)grid.GridInfo.BasicReward.Count;
            //金币进行等级计算
            if (grid.GridInfo.BasicReward.ID == StaticData.configExcel.GetVertical().GoldGoodsId)
                baseNum = ZillionaireToolManager.GetGridBaseReward(baseNum);
            //暂存玩家获得的奖励
            ZillionairePlayerManager._instance.CurrentPlayer.AddPlayerRewards(grid.GridInfo.BasicReward.ID, baseNum);
            //发放体力奖励 + 播放角色获得动画
            ZillionairePlayerManager._instance.CurrentPlayer.IssueCurrentRewards();
            //CurDiceActionEnd();
        }

        CurDiceActionEnd();
    }



    #region 骰子

    /// <summary>
    /// 投掷骰子
    /// </summary>
    private void OnClickThrowBtn()
    {
        Debug.Log("投掷骰子 OnClickThrowBtn");
        if (!_isShowDice)
            return;
        //新手引导标记完成
        if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.isCurrStepGuiding)
        {
            GuideCanvasComponent._instance.SetLittleStepFinish();
        }
        //色子可以投掷 角色移动完成
        if (!_curDiceController.IsThrow || !ZillionairePlayerManager._instance.CurrentPlayer.IsEndMove)
        {
            return;
        }
        if (!PowerIsEnough())
        {
            //没有购买体力次数进入结算界面 df
            if (!IsHavePurchases())
            {
                OnClickOut();
                return;  
            }
            Debug.Log("体力不足不可以投掷骰子！！！");
            StaticData.OpenBuyPower(ConfirmBuyItems, CancelBuyPower);
            return;
        }

        //
        ZillionaireToolManager.NotifyServerDice(_selectDiceType, NotifyServerDiceCallback);

    }

    /// <summary>
    /// 通知投掷色子回调
    /// </summary>
    /// <param name="sCDiceResult"></param>
    private void NotifyServerDiceCallback(SCDiceResult sCDiceResult) 
    {
        if (sCDiceResult != null)
        {
            //成功
            GetDiceIndexCallbackSucceeded(sCDiceResult);
        }
        else 
        {
            //失败
            GetDiceIndexCallbackFailed();
        }
    }

    /// <summary>
    /// 获取骰子点数 服务器回调成功
    /// </summary>
    private void GetDiceIndexCallbackSucceeded(SCDiceResult sCDiceResult)
    {
        _isShowDice = false;

        //消耗体力
        DepletePower();
        //存储色子回调信息
        _diceCallbackInfoNew = sCDiceResult;

        _currDiceIndex = _diceCallbackInfoNew.Info[0].DiceNum;
        //掷色子动画
        //PlayDiceBeginAnim(EndThrow);
        PlayDiceAnim(EndThrow);
    }

    /// <summary>
    /// 开始事件投掷色子
    /// </summary>
    public void StartEventDice() 
    {
        _isShowDice = false;
        //移除上一个色子回调数据
        _diceCallbackInfoNew.Info.RemoveAt(0);
        _currDiceIndex = _diceCallbackInfoNew.Info[0].DiceNum;
        //掷色子动画
        //PlayDiceBeginAnim(EndThrow);
        PlayDiceAnim(EndThrow);
    }

    /// <summary>
    /// 播放掷色子动画
    /// </summary>
    /// <param name="callback"></param>
    private void PlayDiceAnim(Action callback) 
    {
        //隐藏按钮
        _middleTra.gameObject.SetActive(false);
        //开始播放色子第二阶段动画
        ThrowDice(callback);
    }


    /// <summary>
    /// 获取骰子点数 服务器回调失败
    /// </summary>
    private void GetDiceIndexCallbackFailed()
    {

        Debug.Log("获取骰子点数 服务器回调失败!");
    }


    /// <summary>
    /// 掷色子 动画
    /// </summary>
    /// <param name="endAction"></param>
    private void ThrowDice(System.Action endAction, int index = -1)
    {
        if (index == -1)
            index = _currDiceIndex;

        _curDiceController.gameObject.SetActive(true);
        _curDiceController.NotifyPlayDiceAnim(index, endAction);
    }

    /// <summary>
    /// 摇色子结束 色子显示点数 //掷色子完成 回调
    /// </summary>
    public void EndThrow()
    {
        //移动
        RoleMove();
    }

    #endregion

    /// <summary>
    /// 消耗体力
    /// </summary>
    private void DepletePower()
    {
        ZillionairePlayerManager._instance.CurrentPlayer.DepletePower();
    }

    /// <summary>
    /// 体力是否住够
    /// </summary>
    /// <returns></returns>
    private bool PowerIsEnough()
    {
        return ZillionairePlayerManager._instance.CurrentPlayer.IsEnoughPower();
    }

    /// <summary>
    /// 是否拥有购买体力的次数
    /// </summary>
    /// <returns></returns>
    private bool IsHavePurchases()
    {
        return ZillionairePlayerManager._instance.CurrentPlayer.BuyPowerNum < ZillionaireGameMapManager._instance.CurZillionaireMapControl.CurSelectMap.PowerBuyTime ? true : false;
    }


    /// <summary>
    /// 确认购买体力
    /// </summary>
    /// <param name="id"></param>
    /// <param name="num"></param>
    private void ConfirmBuyItems(int id, int num) 
    {
        _curBuyPorwerNum = num;
        CSBuyProp buyItem = new CSBuyProp();
        buyItem.GoodId = ZillionaireGameMapManager._instance.CurZillionaireMapControl.CurSelectMap.PowerBuyID;
        buyItem.GoodNum = num;
        buyItem.BuyWay = GoodsBuyWay.FirstWay;
        ZillionaireToolManager.NotifyServerBuyItems(buyItem, BuyPowerSuccess);
    }

    /// <summary>
    /// 取消购买体力
    /// </summary>
    private void CancelBuyPower()
    {
        //进入结算
        //OnClickOut();
        EnterSettlement();
    }

    /// <summary>
    /// 购买体力成功
    /// </summary>
    private void BuyPowerSuccess(SCBuyProp sCBuyProp)
    {
        StaticData.UpdateBackpackProps(sCBuyProp);

        ZillionairePlayerManager._instance.CurrentPlayer.BuyPowerNum += _curBuyPorwerNum;
        
        ZillionairePlayerManager._instance.CurrentPlayer.AddPower(ZillionaireGameMapManager._instance.GetBuyPowerInfo().GoodNum * _curBuyPorwerNum);

        OnClickThrowBtn();
    }

    /// <summary>
    /// 购买体力失败
    /// </summary>
    private void BuyPowerFail()
    {
        Debug.Log("购买体力失败");
        //测试代码 语言国际化
        StaticData.CreateToastTips("购买体力失败！");
    }

    /// <summary>
    /// 更新体力显示
    /// </summary>
    /// <param name="isInit"></param>
    public void UpdatePowerShow(bool isInit = false)
    {
        var curPower = 0;
        if (isInit)
        {
            curPower = ZillionaireGameMapManager._instance.CurZillionaireMapControl.CurSelectMap.BasicPower;
        }
        else
        {
            curPower = ZillionairePlayerManager._instance.CurrentPlayer.CurStamina;
        }
        _curPowerShow.text = curPower + "/" + ZillionaireGameMapManager._instance.CurZillionaireMapControl.CurSelectMap.BasicPower;
        _curPowerImage.fillAmount = (float)curPower / (float)ZillionaireGameMapManager._instance.CurZillionaireMapControl.CurSelectMap.BasicPower;
    }


    #region 播放动画


    /// <summary>
    /// 播放色子开始动画
    /// </summary>
    public void PlayDiceBeginAnim(Action diceCallback)
    {
        _diceCallback = diceCallback;
        _animator.Play("MonopolyGameUIAnim_Dice");
    }

    #endregion

    #region 动画事件

    /// <summary>
    /// 色子按钮开始动画结束事件
    /// </summary>
    public void DiceBeginAnimEndEvent()
    {
        //隐藏按钮
        _middleTra.gameObject.SetActive(false);
        //开始播放色子第二阶段动画
        ThrowDice(_diceCallback);
    }

    #endregion

    /// <summary>
    /// 更新奖励显示
    /// </summary>
    public void UpdateRewardsShow(int id, int num) 
    {
        if (!_earnItemObj.transform.parent.gameObject.activeInHierarchy)
            _earnItemObj.transform.parent.gameObject.SetActive(true);

        if (_curEarnRewards.ContainsKey(id))
        {
            int itemNum = int.Parse(_curEarnRewards[id].text);
            DOTween.To(() => itemNum, rewardNum => { itemNum = rewardNum; _curEarnRewards[id].text = itemNum.ToString(); }, num, 0.6f );
            
        }
        else 
        {
            var obj = Instantiate(_earnItemObj, _earnContent);
            obj.SetActive(true);
            var text = obj.transform.Find("Num").GetComponent<Text>();
            var icon = obj.transform.Find("Icon").GetComponent<Image>();
            var path = StaticData.configExcel.GetGameItemByID(id).Icon;
            icon.sprite = ABManager.GetAsset<Sprite>(path);
            //text.text = num.ToString();
            _curEarnRewards.Add(id, text);

            text.text = "0";
            int itemNum = 0;
            if (num > 2)
            {
                DOTween.To(() => itemNum, rewardNum => { itemNum = rewardNum; text.text = itemNum.ToString(); }, num, 0.3f);
            }
            else 
            {
                text.text = num.ToString();
            }
            
        }
        //
    }

    #endregion

    #region 引导

    #endregion
}
