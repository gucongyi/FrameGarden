using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 红点控制
/// 2021/1/7 huangjiangdong
/// </summary>
public static class RedDotManager
{
    #region 字段
    static TimeCountDownComponent _timeCountDownComponent;
    /// <summary>
    /// 红点钥匙
    /// </summary>
    public enum RedDotKey
    {
        /// <summary>
        /// 任务
        /// </summary>
        Task,
        /// <summary>
        /// 仓库
        /// </summary>
        Warehouse,
        /// <summary>
        /// 商城
        /// </summary>
        Shopping,
        /// <summary>
        /// 庄园
        /// </summary>
        Manor,
        /// <summary>
        /// 邮件
        /// </summary>
        Mailbox,
        /// <summary>
        /// 庄园装扮
        /// </summary>
        ManorDecorate,
        /// <summary>
        /// 订单
        /// </summary>
        Order,
        /// <summary>
        /// 活动
        /// </summary>
        Activity,
        /// <summary>
        /// 章节
        /// </summary>
        Chapter
    }
    /// <summary>
    /// 零点更新通知回调
    /// </summary>
    static Dictionary<RedDotKey, Action> _redDotActionDic = new Dictionary<RedDotKey, Action>();
    /// <summary>
    /// 开关红点回调
    /// </summary>
    static Dictionary<RedDotKey, Func<bool>> _redOpenDotActionDic = new Dictionary<RedDotKey, Func<bool>>();
    /// <summary>
    /// 红点对象字典
    /// </summary>
    static Dictionary<RedDotKey, Transform> _redDotTraDic = new Dictionary<RedDotKey, Transform>();
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    public static bool _isInitial = false;
    #endregion
    #region 函数
    /// <summary>
    /// 初始化红点管理器
    /// </summary>
    public static void Initial()
    {
        if (_isInitial)
        {
            return;
        }
        //添加所有开关函数
        AddAllOpenAction();
        //开始计时
        StarTime();
        _isInitial = true;
    }

    /// <summary>
    /// 添加所有的开关红带你回调
    /// </summary>
    static void AddAllOpenAction()
    {
        if (!_redOpenDotActionDic.ContainsKey(RedDotKey.Task))
            _redOpenDotActionDic.Add(RedDotKey.Task, TaskPanelTool.IsOpenDot);
        if (!_redOpenDotActionDic.ContainsKey(RedDotKey.Order))
            _redOpenDotActionDic.Add(RedDotKey.Order, StaticData.IsSubmintDeal);
        //仓库红点功能
        if (!_redOpenDotActionDic.ContainsKey(RedDotKey.Warehouse))
            _redOpenDotActionDic.Add(RedDotKey.Warehouse, StaticData.IsWarehouseRedDotOpen);
        if (!_redOpenDotActionDic.ContainsKey(RedDotKey.Activity))
            _redOpenDotActionDic.Add(RedDotKey.Activity, SignTool.IsOpenRedDot);
        if (!_redOpenDotActionDic.ContainsKey(RedDotKey.Mailbox))
            _redOpenDotActionDic.Add(RedDotKey.Mailbox, MailboxTool.IsOpenDot);
        if (!_redOpenDotActionDic.ContainsKey(RedDotKey.Shopping))
            _redOpenDotActionDic.Add(RedDotKey.Shopping, ShopTool.IsOpenRedDot);
        if (!_redOpenDotActionDic.ContainsKey(RedDotKey.Manor))
            _redOpenDotActionDic.Add(RedDotKey.Manor, ManorRedDotTool.IsOpenManorRedDotInLobby);
        if (!_redOpenDotActionDic.ContainsKey(RedDotKey.ManorDecorate))
            _redOpenDotActionDic.Add(RedDotKey.ManorDecorate, ManorRedDotTool.IsOpenManorDecorateRedDot);
        //章节
        if (!_redOpenDotActionDic.ContainsKey(RedDotKey.Chapter))
            _redOpenDotActionDic.Add(RedDotKey.Chapter, ChapterHelper.ChpaterUpdateRedDot);
    }
    /// <summary>
    /// 添加对应红点
    /// </summary>
    /// <param name="key"></param>
    /// <param name="tage"></param>
    public static void AddDotTra(RedDotKey key, Transform tage)
    {
        if (_redDotTraDic.ContainsKey(key))
        {
            _redDotTraDic[key] = tage;
        }
        else
        {
            _redDotTraDic.Add(key, tage);
        }
    }
    /// <summary>
    /// 更新红点
    /// </summary>
    /// <param name="key"></param>
    /// <param name="dotTra"></param>
    public static void UpdateRedDot(RedDotKey key)
    {
        if (!_isInitial)
        {
            Initial();
        }
        if (_redOpenDotActionDic.ContainsKey(key))
        {

            if (_redOpenDotActionDic[key] != null)
            {
                bool isOpen = _redOpenDotActionDic[key].Invoke();
                if (_redDotTraDic.ContainsKey(key))
                {
                    if (_redDotTraDic[key] != null)
                    {
                        _redDotTraDic[key].gameObject.SetActive(isOpen);
                    }
                }
            }
        }
        //如果是商城，任务，订单，仓库,装饰更新红点 需要同时更新大厅庄园的红点
        switch (key)
        {
            case RedDotKey.Task:
            case RedDotKey.Warehouse:
            case RedDotKey.Shopping:
            case RedDotKey.ManorDecorate:
            case RedDotKey.Order:
                UpdateRedDot(RedDotKey.Manor);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 添加回调
    /// </summary>
    /// <param name="redDotKey">key</param>
    /// <param name="action">回调</param>
    static void AddAction(RedDotKey redDotKey, Action action)
    {
        if (_redDotActionDic.ContainsKey(redDotKey))
        {
            _redDotActionDic[redDotKey] = action;
        }
        else
        {
            _redDotActionDic.Add(redDotKey, action);
        }
    }
    /// <summary>
    /// 移除回调
    /// </summary>
    /// <param name="redDotKey">key</param>
    static void RemoveAction(RedDotKey redDotKey)
    {
        if (_redDotActionDic.ContainsKey(redDotKey))
        {
            _redDotActionDic.Remove(redDotKey);
        }
    }
    /// <summary>
    /// 添加开关红点回调
    /// </summary>
    /// <param name="key"></param>
    /// <param name="openAction"></param>
    static void AddOpenDotAction(RedDotKey key, Func<bool> openAction)
    {
        if (_redOpenDotActionDic.ContainsKey(key))
        {
            _redOpenDotActionDic[key] = openAction;
        }
        else
        {
            _redOpenDotActionDic.Add(key, openAction);
        }
    }
    /// <summary>
    /// 移除开关红点回调
    /// </summary>
    /// <param name="key"></param>
    static void RemoveDotAction(RedDotKey key)
    {
        if (_redOpenDotActionDic.ContainsKey(key))
        {
            _redOpenDotActionDic.Remove(key);
        }
    }
    /// <summary>
    /// 调用零点更新回调
    /// </summary>
    static void CallAction()
    {
        foreach (var item in _redDotActionDic)
        {
            item.Value?.Invoke();
        }
    }
    /// <summary>
    /// 开始计时
    /// </summary>
    static void StarTime()
    {
        if (_timeCountDownComponent == null)
        {
            CreationTimer();
        }
        StartCountingTime(GetTimeDifference());
    }
    /// <summary>
    /// 创建空白计时器
    /// </summary>
    static void CreationTimer()
    {
        _timeCountDownComponent = StaticData.CreateTimer(100000 / 1000f, false, (go) =>
        {

        },
        (remainTime) =>
        {

        }, "RedDotTime");
    }
    /// <summary>
    /// 开始计时
    /// </summary>
    static void StartCountingTime(float cdValue)
    {
        _timeCountDownComponent.InitSecondsOnFrameCallBack(cdValue, false, (go) =>
        {
            CallAction();
            StartCountingTime(GetTimeDifference());
        },
        (remainTime) =>
        {
            //Debug.Log("++++++++++++++++++++++++++++++++++红点正在计时：" + remainTime);
        });

    }
    /// <summary>
    /// 获取时间差
    /// </summary>
    /// <returns></returns>
    static float GetTimeDifference()
    {
        //获取当前服务器时间
        DateTime dateTime = TimeHelper.ServerDateTimeNow;
        //Debug.Log("++++++++++++++++++++++++++++++++++获取时间" + TimeHelper.ServerDateTimeNow);
        //0点更新时间
        DateTime newTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day + 1, 0, 0, 0);//new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second+10);//
        //计算时间差
        TimeSpan timeDifference = newTime - dateTime;
        //将时间差转换为秒
        float time = (float)timeDifference.TotalSeconds;

        return time;
    }
    #endregion
}
