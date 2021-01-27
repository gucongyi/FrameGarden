using Company.Cfg;
using Cysharp.Threading.Tasks;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 地图事件管理者 新的 11/19
/// </summary>
public class MapEventManagerNew : MonoBehaviour
{

    #region 变量

    /// <summary>
    /// 当前的色子回调信息
    /// </summary>
    private SCDiceStruct _curDiceCallbackInfo;

    /// <summary>
    /// 激活事件回调
    /// </summary>
    private Action ActionEventCallback;

    private GameObject _prefabOrdinary;
    private GameObject _prefabRandom;

    #endregion

    #region 方法

    /// <summary>
    /// 激活事件
    /// </summary>
    /// <param name="eventID"></param>
    public void ActiveEvent(SCDiceStruct curEvent, ZillionaireGameMapGridDefInfo gridInfo, Action callback) 
    {
        ActionEventCallback = callback;
        _curDiceCallbackInfo = curEvent;

        SortEvent(gridInfo);
    }

    /// <summary>
    /// 分类事件/将事件进行分类处理
    /// </summary>
    /// <param name="eventID"></param>
    private async void SortEvent(ZillionaireGameMapGridDefInfo gridInfo) 
    {
        int eventID = 0;
        if (gridInfo.IsActiveEvent) 
        {
            eventID = gridInfo.GridInfo.EventSecond;
        }else    
        {
            eventID = gridInfo.GridInfo.EventFirst;
        }
        Debug.Log("SortEvent 事件 id = " + eventID);
        ZillionaireEventDefine curEvent = StaticData.configExcel.GetZillionaireEventByID(eventID);
        //提示事件名称
        //StaticData.CreateToastTips(LocalizationDefineHelper.GetStringNameById(curEvent.EventNameID));
        PlayEffectTips(gridInfo, curEvent);
        //延时等待
        await UniTask.Delay(1200);
        switch (eventID)
        {
            case 1001:
                PhysicalStrengthEvent(curEvent);
                break;
            case 1002:
                PhysicalStrengthEvent(curEvent);
                break;
            case 1003:
                LotteryEvent(curEvent);
                break;
            case 1004:
                ChooseAPrizeEvent(curEvent);
                break;
            case 1006:
                VoteAgainEvent(curEvent);
                break;
            case 1007:
                RewardDoubledEvent(curEvent, gridInfo);
                break;
            case 2001:
                PhysicalStrengthEvent(curEvent);
                break;
            case 2002:
                LotteryEvent(curEvent);
                break;
            case 2004:
                RewardDoubledEvent(curEvent, gridInfo);
                break;
            case 2005:
                ChooseAPrizeEvent(curEvent);
                break;
            case 2006:
                RewardDoubledEvent(curEvent, gridInfo);
                break;
            case 2007:
                RandomDeliveryEvent(curEvent);
                break;
            case 2008:
                GrandSlamEvent(curEvent);
                break;
            case 2009:
                RewardDoubledEvent(curEvent, gridInfo);
                break;
            default:
                Debug.Log("StartEvent 没有注册的事件！ eventID ="+ eventID);
                break;
        }
    }

    /// <summary>
    /// 体力事件
    /// </summary>
    private void PhysicalStrengthEvent(ZillionaireEventDefine curEvent) 
    {
        //1.播放动画效果
        //2.数据处理
        ZillionairePlayerManager._instance.CurrentPlayer.AddPower(curEvent.Effect);
        //完成
        EventEnd();
    }

    /// <summary>
    /// 抽奖事件 娃娃机
    /// </summary>
    /// <param name="curEvent"></param>
    private void LotteryEvent(ZillionaireEventDefine curEvent) 
    {
        //1.播放动画效果
        //2.打开弹窗
        Debug.Log("抽奖事件!!!");
        StaticData.OpenCraneMachine(curEvent.Effect, ActionEventCallback);
        //完成
        //EventEnd();
    }

    /// <summary>
    /// 选择奖励事件 3选1
    /// </summary>
    private void ChooseAPrizeEvent(ZillionaireEventDefine curEvent) 
    {
        //1.播放动画效果
        //2.打开弹窗

        Debug.Log("选择奖励事件!!!");
        StaticData.OpenGuess(curEvent.Effect, ActionEventCallback);
        //完成
        //EventEnd();
    }

    /// <summary>
    /// 再投一次色子事件
    /// </summary>
    private void VoteAgainEvent(ZillionaireEventDefine curEvent) 
    {
        //1.播放动画效果
        //2.进行播放色子效果/下一个流程
        ZillionaireUIManager._instance.PlayInterfaceControl.StartEventDice();
    }

    /// <summary>
    /// 随机传送事件 //传送不能传送到传送格子
    /// </summary>
    private void RandomDeliveryEvent(ZillionaireEventDefine curEvent) 
    {
        //1.播放动画效果
        //2.将人物移动到目标点
        int gridID = _curDiceCallbackInfo.PresentLocation;
        gridID -= 1;
        if (gridID < 0)
            gridID = ZillionaireGameMapManager._instance.CurZillionaireMapControl.CurSelectMap.MapGridMax.Count - 1;

        Debug.Log("随机传送事件 gridID："+ gridID);
        var gridInfo = ZillionaireGameMapManager._instance.GetGridDataByID(gridID);
        //显示格子上的图标
        ZillionairePlayerManager._instance.CurrentPlayer.CurrGridData.ShowIcon();

        //将玩家角色移动的目标格子上
        ZillionairePlayerManager._instance.CurrentPlayer.PlayerEnterGrid(gridInfo, true);
        //玩家完成
        ZillionaireUIManager._instance.PlayInterfaceControl.CurRoleMoveEnd();
    }

    /// <summary>
    /// 大满贯事件
    /// </summary>
    private async void GrandSlamEvent(ZillionaireEventDefine curEvent) 
    {
        //1.播放动画效果
        List<ZillionaireGameMapGridDefInfo> needPlayAnimList = new List<ZillionaireGameMapGridDefInfo>();
        //2.有大满贯触发格子播放获得道具效果
        foreach (var item in ZillionaireGameMapManager._instance.CurZillionaireMapControl.MapGridDic)
        {
            if (!item.Value.GridInfo.IsTrigger)
                continue;

            needPlayAnimList.Add(item.Value);

            //添加奖励
            int baseNum = (int)item.Value.GridInfo.BasicReward.Count;
            //金币进行等级计算
            if (item.Value.GridInfo.BasicReward.ID == StaticData.configExcel.GetVertical().GoldGoodsId)
                baseNum = ZillionaireToolManager.GetGridBaseReward(baseNum);
            //暂存玩家获得的奖励
            ZillionairePlayerManager._instance.CurrentPlayer.AddPlayerRewards(item.Value.GridInfo.BasicReward.ID, baseNum);

        }
        while (needPlayAnimList.Count > 0) 
        {
            Debug.Log("通知格子播放获得奖励效果!");
            //通知格子播放获得奖励效果
            needPlayAnimList[0].PlayPickupItemEffect(true);
            await UniTask.Delay(200);
            needPlayAnimList.RemoveAt(0);
        }

        Debug.Log("大满贯事件 end");
        //完成
        EventEnd();
    }


    /// <summary>
    /// 奖励翻倍事件
    /// </summary>
    private void RewardDoubledEvent(ZillionaireEventDefine curEvent, ZillionaireGameMapGridDefInfo gridInfo) 
    {
        //1.播放动画效果 翻倍数量在动效中表现
        //2.获得奖励效果
        int baseNum = (int)gridInfo.GridInfo.BasicReward.Count;
        //金币进行等级计算
        if (gridInfo.GridInfo.BasicReward.ID == StaticData.configExcel.GetVertical().GoldGoodsId)
            baseNum = ZillionaireToolManager.GetGridBaseReward(baseNum);

        baseNum *= curEvent.Effect;
        //暂存玩家获得的奖励
        ZillionairePlayerManager._instance.CurrentPlayer.AddPlayerRewards(gridInfo.GridInfo.BasicReward.ID, baseNum);
        //发放体力奖励 + 播放角色获得动画
        ZillionairePlayerManager._instance.CurrentPlayer.IssueCurrentRewards();
        //通知格子播放获得奖励效果
        gridInfo.PlayPickupItemEffect(true);
        //完成
        EventEnd();
    }


    /// <summary>
    /// 事件结束
    /// </summary>
    private void EventEnd() 
    {
        ActionEventCallback?.Invoke();
    }

    /// <summary>
    /// 播放事件提示
    /// </summary>
    /// <param name="curEvent"></param>
    private async void PlayEffectTips(ZillionaireGameMapGridDefInfo gridInfo, ZillionaireEventDefine curEvent)
    {
        Debug.Log("播放事件提示 11");
        GameObject eventEffect = null;
        if (!gridInfo.IsActiveEvent)
        {
            Debug.Log("播放事件提示 12");
            eventEffect = await SpawnPrefabOrdinary();
            Vector3 pos = gridInfo.GetItemEffectStartPos();
            //将屏幕坐标转换到transform的局部坐标中
            Vector2 V2InAt;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)eventEffect.transform.parent, pos, Camera.main, out V2InAt);
            eventEffect.transform.localPosition = V2InAt;
        }
        else 
        {
            Debug.Log("播放事件提示 13");
            eventEffect = await SpawnPrefabRandom();
        }
        eventEffect.GetComponent<ZillionaireOrdinaryEventEffectController>().InitValue(curEvent.ID);
        Debug.Log("播放事件提示 14");
    }

    /// <summary>
    /// 播放随机事件获得效果
    /// </summary>
    public async void PlaySpawnRandomEvnetEffectTips() 
    {
        GameObject eventEffect = await SpawnPrefabRandom();
        eventEffect.GetComponent<ZillionaireOrdinaryEventEffectController>().InitValue(9999);
    }

    /// <summary>
    /// 获取普通事件 提示
    /// </summary>
    /// <returns></returns>
    private async Task<GameObject> SpawnPrefabOrdinary()
    {
        if (_prefabOrdinary == null) 
        {
            GameObject obj = await ABManager.GetAssetAsync<GameObject>("ZillionaireOrdinaryEventEffect") as GameObject;
            _prefabOrdinary = Instantiate(obj, UIRoot.instance.GetUIRootCanvas().transform);
            _prefabOrdinary.SetActive(false);
        }

        return _prefabOrdinary;
    }

    private async Task<GameObject> SpawnPrefabRandom()
    {
        if (_prefabRandom == null)
        {
            GameObject obj = await ABManager.GetAssetAsync<GameObject>("ZillionaireRandomEventEffect") as GameObject;
            _prefabRandom = Instantiate(obj, UIRoot.instance.GetUIRootCanvas().transform);
            _prefabRandom.SetActive(false);
        }

        return _prefabRandom;
    }

    #endregion

}

