using Company.Cfg;
using Cysharp.Threading.Tasks;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 大地块信息
/// </summary>
public class ManorRegionComponent : MonoBehaviour
{
    public int regionId;
    public DecorateBoardComponent decorateBoardComponent;
    
    [HideInInspector]
    //用来标识自己庄园的5个状态
    public AreaState areaState;

    Vector2 regionMousePos;
    bool isDrag = false;
    float currPressTime = 0f;
    bool isLongPress;
    //自己的庄园的这个地块是否解锁
    public bool isSelfRegionUnLock;
    private void Awake()
    {
        if (decorateBoardComponent!=null)
        {
            var areaDefine = StaticData.configExcel.AreaUnlock.Find(x => x.ID == regionId);
            decorateBoardComponent.SetTips(areaDefine.Grade);
        }
    }

    internal void OnIncreaseClick(WorkShedSpeedUpWay workShedSpeedUpWay)
    {
        CSWorkShedSpeedUp csWorkShedSpeedUp = new CSWorkShedSpeedUp()
        {
            AreaId=regionId,
            SpeedUpWay= workShedSpeedUpWay
        };
        ManorProtocalHelper.WorkLoadSpeedUp(csWorkShedSpeedUp, (succ) => {
            StaticData.CreateToastTips($"加速成功");
            //UIComponent.HideUI(UIType.UIManorRegionAdIncrease);
            decorateBoardComponent.SetRegionTimeChange(succ.UnlockTime,this);
            //广告加速钻石不变
            if (workShedSpeedUpWay == WorkShedSpeedUpWay.DiamondWay)
            {
                //更新道具
                var dealClass = StaticData.configExcel.GetAreaUnlockByID(regionId).UseGoods;
                DealClass willCost = dealClass[0];
                StaticData.UpdateWareHouseItem(willCost.IdGameItem, -willCost.Price);
            }
        });
    }

    public void PressDownRegion()
    {
        //todo
        isDrag = false;
        regionMousePos = Input.mousePosition;
        currPressTime = 0f;
        isLongPress = false;
    }
    public void OnlySetAreaState(AreaState areaState)
    {
        this.areaState = areaState;
    }

     void OnRegionClick()
    {
        StaticData.DebugGreen($"大地块regionId:{regionId}");
        ClickRegion();
    }

    public bool GetRegionIsFinishUnLock()
    {
        return areaState == AreaState.RemoveWorkShed;
    }
    public void SetManorRegionAreaState(AreaState areaState,long workTimeStamp=0)
    {
        this.areaState = areaState;
        if (Root2dSceneManager._instance.isFriendManor)
        {
            switch (areaState)
            {
                case AreaState.NoUnlock:
                case AreaState.Conversation:
                case AreaState.Pay:
                case AreaState.RoadWork:
                case AreaState.WorkShed:
                    SetFriendLockState();
                    break;
                case AreaState.RemoveWorkShed:
                    SetFriendUnLockState();
                    break;
            }
        }
        else
        {
            isSelfRegionUnLock = false;
            switch (areaState)
            {
                case AreaState.NoUnlock:
                    SetSelfLockState();
                    break;
                case AreaState.Conversation:
                    SetSelfDialogState();
                    break;
                case AreaState.Pay:
                    SetSelfPayState();
                    break;
                case AreaState.RoadWork:
                    SetSelfRoadWorkState(workTimeStamp);
                    break;
                case AreaState.WorkShed:
                    SetSelfWorkShedState();
                    break;
                case AreaState.RemoveWorkShed:
                    isSelfRegionUnLock = true;
                    SetSelfRemoveWorkShedState();
                    break;
            }
        }
    }

    //设置好友解锁状态
    private void SetFriendUnLockState()
    {
        //解锁情况下，大地块不能点击
        GetComponent<Collider2D>().enabled = false;
        ClearCanClear();
        SetDecorateDrag(false);
    }
    //设置好友未解锁状态
    private void SetFriendLockState()
    {
        //锁定情况下，大地块可以点击
        GetComponent<Collider2D>().enabled = true;
        SetDecorateDrag(false);
    }
    //自己未解锁状态
    private void SetSelfLockState()
    {
        GetComponent<Collider2D>().enabled = true;
        SetDecorateDrag(false);
        OnlySetAreaState(AreaState.NoUnlock);
    }

    //自己对话阶段
    private void SetSelfDialogState()
    {
        GetComponent<Collider2D>().enabled = true;
        SetDecorateDrag(false);
        OnlySetAreaState(AreaState.Conversation);
    }
    //自己支付阶段
    private void SetSelfPayState()
    {
        GetComponent<Collider2D>().enabled = true;
        SetDecorateDrag(false);
        OnlySetAreaState(AreaState.Pay);
    }
    //自己施工状态
    private void SetSelfRoadWorkState(long workTimeStamp)
    {
        OnlySetAreaState(AreaState.RoadWork);
        GetComponent<Collider2D>().enabled = true;
        SetDecorateDrag(false);
        Root2dSceneManager._instance.manorUnLockWorkComponent.SetWorker(regionId);
        BeginRoadWork(workTimeStamp);
    }
    //自己工棚状态
    private void SetSelfWorkShedState()
    {
        OnlySetAreaState(AreaState.WorkShed);
        ClearCanClear();
        GetComponent<Collider2D>().enabled = true;
        SetDecorateDrag(false);
        Root2dSceneManager._instance.manorUnLockWorkShedComponent.SetWorkShed(regionId);
    }
    private void SetSelfRemoveWorkShedState()
    {
        OnlySetAreaState(AreaState.RemoveWorkShed);
        ClearCanClear();
        if (GetComponent<Collider2D>() != null)
        {
            GetComponent<Collider2D>().enabled = false;
        }
        SetDecorateDrag(true);
        //移除工棚
        Root2dSceneManager._instance.manorUnLockWorkShedComponent.ClearWorkShed();
    }

    //清理掉杂物
    private void ClearCanClear()
    {
        var allTileCom = Root2dSceneManager._instance.transAllManorPlant.GetComponentsInChildren<TileComponent>();
        //解锁清除对应地块上杂物
        for (int eachTile = 0; eachTile < allTileCom.Length; eachTile++)
        {
            if (allTileCom[eachTile].regionId == regionId && allTileCom[eachTile].typeTile == Company.Cfg.TypeManorDecorate.CanClear)
            {
                Destroy(allTileCom[eachTile].gameObject);
            }
        }
    }

    //设置装饰物是否能拖动
    void SetDecorateDrag(bool isCanDrag)
    {
        var allTileCom = Root2dSceneManager._instance.transAllManorPlant.GetComponentsInChildren<TileComponent>();
        for (int eachTile = 0; eachTile < allTileCom.Length; eachTile++)
        {
            if (allTileCom[eachTile].regionId == regionId)
            {
                allTileCom[eachTile].HandleColliders(isCanDrag);
            }
        }
        //装饰物状态和地块状态一致
        SetTileDrag(isCanDrag);
    }
    //设置装饰物是否能拖动
    void SetTileDrag(bool isCanDrag)
    {
        var allTileCom = Root2dSceneManager._instance.transTile.GetComponentsInChildren<TileComponent>();
        for (int eachTile = 0; eachTile < allTileCom.Length; eachTile++)
        {
            if (allTileCom[eachTile].regionId == regionId)
            {
                allTileCom[eachTile].HandleColliders(isCanDrag);
            }
        }
    }

    private void ClickRegion()
    {
        if (Root2dSceneManager._instance.isFriendManor)//好友
        {
            //好友庄园不做任何提示
            //switch (areaState)
            //{
            //    case AreaState.NoUnlock:
            //    case AreaState.Conversation:
            //    case AreaState.Pay:
            //    case AreaState.RoadWork:
            //    case AreaState.WorkShed:
            //        StaticData.CreateToastTips($"区域未解锁！");
            //        break;
            //    case AreaState.RemoveWorkShed:
            //        break;
            //}
        }
        else//自己
        {
            //关闭操作UI
            StaticData.GetUIWorldHandleComponent().SetHandleTileUIClose();
            switch (areaState)
            {
                case AreaState.NoUnlock:
                    var areaDefine=StaticData.configExcel.AreaUnlock.Find(x => x.ID == regionId);
                    if (StaticData.GetPlayerLevelAndCurrExp().level >= areaDefine.Grade)
                    {
                        int willUnLockRegionId = -1;
                        var unlockInfoSelf=Root2dSceneManager._instance.UnlockAreaInfoSelf;
                        for (int i = 0; i < unlockInfoSelf.Count; i++)
                        {
                            if (unlockInfoSelf[i].State != AreaState.RemoveWorkShed)
                            {
                                willUnLockRegionId = unlockInfoSelf[i].AreaId;
                                break;
                            }
                        }
                        if (regionId == willUnLockRegionId)
                        {
                            StaticData.TiggerUnLockArea(willUnLockRegionId);
                        }
                        else
                        {
                            StaticData.CreateToastTips(string.Format(StaticData.GetMultilingual(120214), willUnLockRegionId));
                        }
                    }
                    else
                    {
                        StaticData.OpenCommonThinkAgainTips(string.Format(StaticData.GetMultilingual(120213), areaDefine.Grade), 120016, null);
                    }
                    break;
                case AreaState.Conversation:
                    BeginDialog();
                    break;
                case AreaState.Pay:
                    BeginPayUI();
                    break;
                case AreaState.RoadWork:
                    var goUIManor=UIComponent.CreateUI(UIType.UIManorRegionAdIncrease);
                    if (decorateBoardComponent != null)
                    {
                        goUIManor.GetComponent<UIManorRegionAdIncreaseComponent>().SetRegionComponent(this, decorateBoardComponent);
                    }
                    break;
                case AreaState.WorkShed:
                    ClickWorkShed();
                    break;
                case AreaState.RemoveWorkShed:
                    ////解锁了，主人走向固定位置
                    //Root2dSceneManager._instance.femaleManorManager.PlayAnim(Input.mousePosition);
                    break;
            }
        }
    }

    private void ClickWorkShed()
    {
        CSUnlockArea csUnlockArea = new CSUnlockArea()
        {
            AreaId = regionId,
            State = AreaState.WorkShed
        };
        ManorProtocalHelper.UnlockRegion(csUnlockArea, (regionUnLockDialogEndSucc) =>
        {
            //播放音效点击
            GameSoundPlayer.Instance.PlaySoundEffect(MusicHelper.SoundEffectAreaUnlockedSuccessfully);
            SetDataDotRegionUnLockSucc();
            ////工棚新手引导完成
            //if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.isCurrStepGuiding)
            //{
            //    GuideCanvasComponent._instance.SetLittleStepFinish();
            //}
            //展示动画,临时替代
            List<CSWareHouseStruct> awardIds = new List<CSWareHouseStruct>();
            for (int i = 0; i < regionUnLockDialogEndSucc.GoodsInfo.Count; i++)
            {
                var wareHouseStruct=StaticData.SCBuyGoodsStructToCSWareHouseStruct(regionUnLockDialogEndSucc.GoodsInfo[i]);
                awardIds.Add(wareHouseStruct);
            }
            var goReward=UIComponent.CreateUI(UIType.UIManorUnlockReward);
            goReward.GetComponent<UIManorUnlockRewardComponent>().SetRewards(this,awardIds, regionUnLockDialogEndSucc);
        });
    }

    private void SetDataDotRegionUnLockSucc()
    {
        switch (regionId)
        {
            case 2:
                StaticData.DataDot(DotEventId.RegionUnLock2Succ);
                break;
            case 3:
                StaticData.DataDot(DotEventId.RegionUnLock3Succ);
                break;
            case 4:
                StaticData.DataDot(DotEventId.RegionUnLock4Succ);
                break;
            case 5:
                StaticData.DataDot(DotEventId.RegionUnLock5Succ);
                break;
            case 6:
                StaticData.DataDot(DotEventId.RegionUnLock6Succ);
                break;
            case 7:
                StaticData.DataDot(DotEventId.RegionUnLock7Succ);
                break;
            case 8:
                StaticData.DataDot(DotEventId.RegionUnLock8Succ);
                break;
            case 9:
                StaticData.DataDot(DotEventId.RegionUnLock9Succ);
                break;
            case 10:
                StaticData.DataDot(DotEventId.RegionUnLock10Succ);
                break;
            default:
                break;
        }
    }

    public void OnWorkSheldRewardGet(List<CSWareHouseStruct> awardIds, SCUnlockArea regionUnLockDialogEndSucc)
    {
        //更新仓库
        for (int i = 0; i < awardIds.Count; i++)
        {
            StaticData.UpdateWareHouseItem(awardIds[i].GoodId, awardIds[i].GoodNum);
        }
        StaticData.DebugGreen($"点击工棚返回状态：{regionUnLockDialogEndSucc.State.ToString()}");
        //点击宝箱，继续解锁
        TriggerNextCanUnLockRegion();
        if (regionUnLockDialogEndSucc.State == AreaState.RemoveWorkShed)
        {
            SetSelfRemoveWorkShedState();
        }
    }

    public async void BeginDialog()
    {
        //关闭操作UI
        StaticData.GetUIWorldHandleComponent().SetHandleTileUIClose();
        SetSelfDialogState();
        //关闭UI
        UIComponent.HideUI(UIType.UIFriend);
        //UIComponent.HideUI(UIType.UIManor);
        await StaticData.PlayManorQuitAnim();
        var willLocalPos=Root2dSceneManager._instance.manorUnLockDialogComponent.SetDialog(regionId);
        Root2dSceneManager._instance.worldCameraComponent.PlayCameraToUnLockRegion(willLocalPos);
        await UniTask.Delay(1000);
        //打开对话框
        var UIDialogGo = await UIComponent.CreateUIAsync(UIType.UIManorUnLockRegionDialog);
        UIDialogGo.GetComponent<UIManorUnLockRegionDialogComponent>().SetRegionComponent(this);
    }

    public async UniTask EndDialog()
    {//触发支付
        SetSelfPayState();
        Root2dSceneManager._instance.manorUnLockDialogComponent.FinishDialog();
        //Root2dSceneManager._instance.worldCameraComponent.PlayCameraToUnLockRegionOld();
        await UniTask.Delay(1000);
        //UIComponent.CreateUI(UIType.UIManor);
        UIComponent.RemoveUI(UIType.UICommonPopupTips);
        
        CSUnlockArea csUnlockArea = new CSUnlockArea()
        {
            AreaId = regionId,
            State = AreaState.Conversation//上一阶段对话状态
        };
        ManorProtocalHelper.UnlockRegion(csUnlockArea, (regionUnLockDialogEndSucc) =>
        {
            StaticData.DebugGreen($"对话结束返回状态：{regionUnLockDialogEndSucc.State.ToString()}");
            //对话状态开始
            if (regionUnLockDialogEndSucc.State == AreaState.Pay)
            {
                BeginPayUI();
            }
        });
    }

    void BeginPayUI()
    {
        var goUIPay=UIComponent.CreateUI(UIType.UIManorRegionPay);
        goUIPay.GetComponent<UIManorRegionPay>().SetManorRegionComponent(this);
    }
    public void BeginPay()
    {
        //道具判定
        var dealClass = StaticData.configExcel.GetAreaUnlockByID(regionId).ConsumptionGood;
        bool isEnough = true;
        DealClass willCost=null;
        for (int i = 0; i < dealClass.Count; i++)
        {
            willCost = dealClass[i];
            int currHaveCount = StaticData.GetWareHouseItem(willCost.IdGameItem).GoodNum;
            if (currHaveCount < willCost.Price)
            {
                isEnough = false;
                break;
            }
        }
        if (isEnough==false)
        {
            string CoinNameNotEnough = StaticData.configExcel.GetLocalizeByID(willCost.IdGameItem).SimplifiedChinese;
            string Tips = string.Format(StaticData.GetMultilingual(120068), CoinNameNotEnough);
            StaticData.OpenCommonTips(Tips, 120010, async () =>
            {
                UIComponent.RemoveUI(UIType.UIManorRegionPay);
                if (willCost.IdGameItem == 1000001)//表示钻石
                {
                    //跳转充值
                    await StaticData.OpenRechargeUI(1, BeginPayUI);
                }
                else if (willCost.IdGameItem == 1000002)//表示金币
                {
                    //跳转充值
                    await StaticData.OpenRechargeUI(0, BeginPayUI);
                }
                else
                {
                    //跳转商城
                    await StaticData.OpenShopUI(1, BeginPayUI);
                }
                
            });
            return;
        }
        CSUnlockArea csUnlockArea = new CSUnlockArea()
        {
            AreaId = regionId,
            State = AreaState.Pay//上一阶段对话状态
        };
        ManorProtocalHelper.UnlockRegion(csUnlockArea, (regionUnLockPaySucc) =>
        {
            StaticData.DebugGreen($"对话结束返回状态：{regionUnLockPaySucc.State.ToString()}");
            //对话状态开始
            if (regionUnLockPaySucc.State == AreaState.RoadWork)
            {
                //关闭UI
                UIComponent.RemoveUI(UIType.UIManorRegionPay);
                //更新道具
                StaticData.UpdateWareHouseItem(willCost.IdGameItem, -willCost.Price);
                SetSelfRoadWorkState(regionUnLockPaySucc.RoadworkTime);
            }
        });
    }

    private void BeginRoadWork(long roadworkTime)
    {
        decorateBoardComponent.GenerateRegionTimer();
        decorateBoardComponent.SetRegionTimeChange(roadworkTime,this);
    }

    /// <summary>
    /// 根据当前等级获取最大解锁区域
    /// </summary>
    /// <param name="curLv"></param>
    /// <returns></returns>
    private AreaUnlockDefine GetMaxAreaUnlock(int curLv) 
    {
        AreaUnlockDefine maxAreaUnlock = null;
        foreach (var item in StaticData.configExcel.AreaUnlock)
        {
            if (item.Grade <= curLv) 
            {
                if (maxAreaUnlock == null)
                {
                    maxAreaUnlock = item;
                }
                else 
                {
                    if (item.Grade > maxAreaUnlock.Grade)
                        maxAreaUnlock = item;
                }      
            }
        }

        return maxAreaUnlock;
    }

    public void EndRoadWork()
    {
        //不和服务器交互，隐藏工人
        Root2dSceneManager._instance.manorUnLockWorkComponent.FinishWork();
        //进入工棚阶段
        SetSelfWorkShedState();
    }

    private void TriggerNextCanUnLockRegion()
    {
        //触发下一块地的解锁
        int currLevel = StaticData.GetPlayerLevelAndCurrExp().level;
        var areaDefine = GetMaxAreaUnlock(currLevel); //StaticData.configExcel.AreaUnlock.Find( x =>  x.Grade == currLevel);
        int willUnLockMaxRegionId = areaDefine.ID;
        int willUnLockRegionId = regionId + 1;
        if (willUnLockRegionId <= willUnLockMaxRegionId)
        {//继续解锁
         //解锁
            StaticData.TiggerUnLockArea(willUnLockRegionId);
        }
    }

    void OnRegionLongPress()
    {
        ClickRegion();
    }
    // Start is called before the first frame update
    void Start()
    {
        isDrag = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(regionMousePos, Input.mousePosition) > 10f)//10f做为滑动的阈值
        {
            isDrag = true;
        }
        if (Input.GetMouseButtonUp(0))//鼠标抬起
        {
            isLongPress = false;
            if (!isDrag&& currPressTime<0.2f)
            {
                OnRegionClick();
            }
        }
        if (!isDrag&&!isLongPress&& Input.GetMouseButton(0))//按住的时候
        {
            currPressTime += Time.deltaTime;

            if (currPressTime > 0.2f)
            {
                OnRegionLongPress();
                isLongPress = true;
            }
        }

    }

    
}
