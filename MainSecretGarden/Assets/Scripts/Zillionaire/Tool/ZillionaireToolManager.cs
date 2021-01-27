using Company.Cfg;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
/// <summary>
/// 大富翁工具箱
/// </summary>
public static class ZillionaireToolManager
{
    #region 字段

    #endregion

    #region 函数

    /// <summary>
    /// 加载道具图标
    /// </summary>
    /// <param name="itemID"></param>
    /// <param name="succeedAction"></param>
    public static async Task<Sprite> LoadItemSprite(int itemID)
    {
        GameItemDefine iten = StaticData.configExcel.GetGameItemByID(itemID);
        Sprite icon = await ABManager.GetAssetAsync<Sprite>(iten.Icon);
        return icon;
    }

    /// <summary>
    /// 加载道具图标
    /// </summary>
    /// <param name="itemID"></param>
    /// <param name="succeedAction"></param>
    public static async Task<Sprite> LoadItemSpriteByIconName(string iconName)
    {
        return await ABManager.GetAssetAsync<Sprite>(iconName);
    }

    /// <summary>
    /// 加载道具的大富翁地图显示图标图标
    /// </summary>
    /// <param name="itemID"></param>
    /// <param name="succeedAction"></param>
    public static async Task<Sprite> LoadItemZillionaireSprite(int itemID)
    {
        //
        Debug.Log("LoadItemZillionaireSprite itemID = "+ itemID);
        GameItemDefine iten = StaticData.configExcel.GetGameItemByID(itemID);
        string iconPath = iten.Icon;
        Sprite icon = await ABManager.GetAssetAsync<Sprite>(iconPath);
        return icon;
    }

    /// <summary>
    /// 加载大富翁格子事件icon
    /// </summary>
    /// <param name="eventID"></param>
    /// <returns></returns>
    public static async Task<Sprite> LoadEventZillionaireSprite(int eventID) 
    {
        var eventInfo = StaticData.configExcel.GetZillionaireEventByID(eventID);
        string iconPath = eventInfo.IconName;
        Sprite icon = await ABManager.GetAssetAsync<Sprite>(iconPath);
        return icon;
    }

    /// <summary>
    /// 获取格子基础奖励
    /// </summary>
    /// <param name="cout"></param>
    /// <returns></returns>
    public static int GetGridBaseReward(int count)
    {
        //金币数量=基础参数+基础参数*（等级+基础变量）
        float baseNum = StaticData.configExcel.GetVertical().ZillionaireBasicVariable;
        int lv = StaticData.GetPlayerLevelAndCurrExp().level;
        return count + (int)((float)count * ((float)lv + baseNum));
    }

    /// <summary>
    /// 获取骰子的消耗
    /// </summary>
    /// <param name="diceType"></param>
    /// <returns></returns>
    public static int GetDiceConsume(DiceType diceType)
    {
        int value = 0;
        switch (diceType)
        {
            case DiceType.LowDice:
                value = StaticData.configExcel.GetVertical().LowDicePhysicalPower;
                break;
            case DiceType.HighDice:
                value = StaticData.configExcel.GetVertical().HighDicePhysicalPower;
                break;
            case DiceType.PureDice:
                value = StaticData.configExcel.GetVertical().PureDicePhysicalPower;
                break;
            default:
                value = StaticData.configExcel.GetVertical().PureDicePhysicalPower;
                break;
        }
        return value;
    }

    /// <summary>
    /// 获取骰子最小消耗
    /// </summary>
    /// <returns></returns>
    public static int GetDiceMinConsume() 
    {
        int mixValue = int.MaxValue;
        int value = 0;
        for (int i = 1; i < 4; i++)
        {
            value = GetDiceConsume((DiceType)i);
            if (value < mixValue)
                mixValue = value;
        }
        return mixValue;
    }

    #endregion

    #region 服务器通信函数

    /// <summary>
    /// 购买大富翁地图
    /// </summary>
    /// <param name="mapID"></param>
    /// <param name=""></param>
    public static void NotifyServerBuyMap(int mapID, Action<SCBuyProp> successAction, Action failedAction)
    {

    }

    /// <summary>
    /// 购买角色
    /// </summary>
    /// <param name="roleID"></param>
    /// <param name="successAction"></param>
    /// <param name="failedAction"></param>
    public static void NotifyServerBuyRole(int roleID, Action<SCBuyProp> successAction, Action failedAction)
    {
#if UNITY_EDITOR
        if (StaticData.IsUsedLocalDataNotServer)
        {
            SCBuyProp sCBuyProp = new SCBuyProp();
            successAction?.Invoke(sCBuyProp);
            return;
        }

#endif

        CSRoleBuy cSRoleBuy = new CSRoleBuy();
        cSRoleBuy.RoleId = roleID;
        ProtocalManager.Instance().SendCSRoleBuy(cSRoleBuy, (SCBuyProp sCBuyProp) =>
        {
            successAction?.Invoke(sCBuyProp);
        },
            (ErrorInfo er) =>
            {
                Debug.Log("通知服务器购买购买角色：" + er.ErrorMessage);
                failedAction?.Invoke();
            });
    }

    /// <summary>
    /// 选中角色
    /// </summary>
    /// <param name="roleID"></param>
    /// <param name="successAction"></param>
    /// <param name="failedAction"></param>
    public static void NotifyServerSelectRole(int roleID, Action successAction, Action failedAction)
    {
#if UNITY_EDITOR
        if (StaticData.IsUsedLocalDataNotServer)
        {
            successAction?.Invoke();
            return;
        }

#endif

        CSChoiceRole cSChoiceRole = new CSChoiceRole();
        cSChoiceRole.RoleId = roleID;
        ProtocalManager.Instance().SendCSChoiceRole(cSChoiceRole, (SCEmptChoiceRole sCEmptChoiceRole) =>
        {
            successAction?.Invoke();
        },
            (ErrorInfo er) =>
            {
                Debug.Log("通知服务器购买购买角色：" + er.ErrorMessage);
                failedAction?.Invoke();
            });
    }

    /// <summary>
    /// 购买道具 快捷购买
    /// </summary>
    /// <param name="item"></param>
    /// <param name="successAction"></param>
    /// <param name="failedAction"></param>
    public static void NotifyServerFastBuyItem(int itemID, Action<SCBuyProp> successAction, Action failedAction)
    {
#if UNITY_EDITOR
        if (StaticData.IsUsedLocalDataNotServer)
        {
            SCBuyProp sCBuyProp = new SCBuyProp();
            successAction?.Invoke(sCBuyProp);
            return;
        }

#endif

        //CSShortcut item = new CSShortcut();
        //item.GoodId = itemID;
        //ProtocalManager.Instance().SendCSShortcut(item, (SCBuyProp sCBuyProp) =>
        //{
        //    Debug.Log("购买道具成功！");
        //    successAction?.Invoke(sCBuyProp);
        //},
        //    (ErrorInfo er) =>
        //    {
        //        Debug.Log("通知服务器购买道具：" + er.ErrorMessage);
        //        failedAction?.Invoke();
        //    });
    }

    /// <summary>
    /// 购买道具 商城购买
    /// </summary>
    /// <param name="buyItem"></param>
    /// <param name="actionCallback"></param>
    public static void NotifyServerBuyItems(CSBuyProp buyItem, Action<SCBuyProp> actionCallback) 
    {
        ProtocalManager.Instance().SendCSBuyProp(buyItem, (SCBuyProp sCBuyProp) =>
        {
            Debug.Log("通知服务器购买道具 购物车型成功！");
            actionCallback?.Invoke(sCBuyProp);
        },
        (ErrorInfo er) =>
        {
            Debug.Log("通知服务器购买道具 购物车型失败！Error：" + er.ErrorMessage);
            actionCallback?.Invoke(null);
        });
    }

    /// <summary>
    /// 请求升级
    /// </summary>
    /// <param name="successAction"></param>
    /// <param name="failedAction"></param>
    public static void NotifyServerRequestUpgrade(Action<int, int, int> successAction, Action failedAction, int willUnLockRegionId = -1)
    {
        CSEmptyUserUpGrade cSEmptyUserUpGrade = new CSEmptyUserUpGrade();
        ProtocalManager.Instance().SendCSEmptyUserUpGrade(cSEmptyUserUpGrade, (SCCurrentExperience sCCurrentExperience) =>
        {
            Debug.Log("请求升级成功！");
            successAction?.Invoke(sCCurrentExperience.BefourGrade, sCCurrentExperience.CurrentGrade, willUnLockRegionId);
        },
            (ErrorInfo er) =>
            {
                Debug.Log("通知服务器请求升级：" + er.ErrorMessage);
                failedAction?.Invoke();
            });
    }


    
    #endregion

    #region 11-18 重做大富翁

    /// <summary>
    /// 请求进入大富翁地图
    /// </summary>
    /// <param name="mapID"></param>
    /// <param name="callBack"></param>
    public static void NotifyServerEnterMap(int mapID, Action<bool> callBack)
    {
        CSEnterMap csentermap = new CSEnterMap();
        csentermap.MapId = mapID;
        ProtocalManager.Instance().SendCSEnterMap(csentermap, (SCSearch sCSearch) =>
        {
            Debug.Log("请求进入大富翁地图成功！");
            callBack?.Invoke(true);
        },
        (ErrorInfo er) =>
        {
            Debug.Log("请求进入大富翁地图失败！err：" + er.ErrorMessage);
            callBack?.Invoke(false);
        });
    }

    /// <summary>
    /// 请求大富翁掷色子
    /// </summary>
    /// <param name="diceType"></param>
    /// <param name="callBack"></param>
    public static void NotifyServerDice(DiceType diceType, Action<SCDiceResult> callBack) 
    {
        CSDice cSDice = new CSDice();
        cSDice.Dice = diceType;
        ProtocalManager.Instance().SendCSDice(cSDice, (SCDiceResult sCDiceResult) =>
        {
            Debug.Log("请求大富翁掷色子成功！");
            callBack?.Invoke(sCDiceResult);
        },
        (ErrorInfo er) =>
        {
            Debug.Log("请求大富翁掷色子失败！err：" + er.ErrorMessage);
            callBack?.Invoke(null);
        });
    }

    /// <summary>
    /// 请求大富翁游戏结算
    /// </summary>
    /// <param name="callBack"></param>
    public static void NotifyServerGameSettlement(Action<bool> callBack) 
    {
        CSEmptyRichManFinish csemptyrichmanfinish = new CSEmptyRichManFinish();
        ProtocalManager.Instance().SendCSEmptyRichManFinish(csemptyrichmanfinish, (SCEmptyRichManFinish cCEmptyRichManFinish) =>
        {
            Debug.Log("请求大富翁游戏结算成功！");
            callBack?.Invoke(true);
        },
        (ErrorInfo er) =>
        {
            Debug.Log("请求大富翁游戏结算失败！err：" + er.ErrorMessage);
            callBack?.Invoke(false);
        });
    }

    /// <summary>
    /// 请求大富翁抽奖或者选择
    /// </summary>
    /// <param name="callBack"></param>
    public static void NotifyServerChooseOrLucky(LuckyTyep type, List<int> itemIDList , Action<SCLucky> callBack)
    {
        CSLucky cSLucky = new CSLucky();
        cSLucky.Type = type;
        if (itemIDList != null)
            cSLucky.GoodId.AddRange(itemIDList);
        ProtocalManager.Instance().SendCSLucky(cSLucky, (SCLucky sCLucky) =>
        {
            Debug.Log("请求大富翁抽奖或者选择成功！");
            callBack?.Invoke(sCLucky);
        },
        (ErrorInfo er) =>
        {
            Debug.Log("请求大富翁抽奖或者选择失败！err：" + er.ErrorMessage);
            callBack?.Invoke(null);
        });
    }
    
    #endregion

}
