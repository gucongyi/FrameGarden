using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 每日领取好感礼物
/// </summary>
public class ReceiveDailyFavorable : MonoBehaviour
{
    //(int) StaticData.configExcel.GetVertical().LoginGetFavorableValue.Count //单次领取数量
    //StaticData.configExcel.GetVertical().GetLoginFavorableAdvertMul         //领取倍率

    //按钮资源名称
    string canGet;//可领取
    string notCanGet;//不可领取

    public Button get_Btn;//领取按钮
    public Button doubleGet_Btn;//双倍领取按钮

    int surplusSecond;//剩余秒数
    TimeCountDownComponent TempTimer;//获取计时器的引用

    private void OnEnable()//Init???
    {
        Debug.LogError($"执行Enable");

        if (TempTimer == null)
        {
            Debug.LogError($"TempTimer:{TempTimer}");
            StartCountDown();
        }
    }

    /// <summary>
    /// 开始倒计时
    /// </summary>
    void StartCountDown()
    {
        //判断服务器时间和状态显示按钮的sprite
        var morning = StaticData.configExcel.GetVertical().GetFavorableTimeMorning;
        var noon = StaticData.configExcel.GetVertical().GetFavorableTimeAfternoon;
        var night = StaticData.configExcel.GetVertical().GetFavorableTimeEvening;

        //获得服务器早中晚三个时间                               (0/1/2:时分秒)
        var morningTime = TimeHelper.GetDateTime(0, morning[0], morning[1], morning[2]);//2020.10.22.6.0.0
        var noonTime = TimeHelper.GetDateTime(0, noon[0], noon[1], noon[2]);
        var nightTime = TimeHelper.GetDateTime(0, night[0], night[1], night[2]);
        //三个时间段
        List<DateTime> timeQuantum = new List<DateTime>();
        timeQuantum.Add(morningTime);
        timeQuantum.Add(noonTime);
        timeQuantum.Add(nightTime);

        //获取领取好感状态信息
        var GetFavorableStateInfo = StaticData.playerInfoData.userInfo.GetFavorableStateInfo;

        //如果该时段未领
        if ((GetFavorableState)GetFavorableStateInfo.GetFavorableState == GetFavorableState.NeverGetType)
        {
            CanGetSet();//可领
            //doubleGet_Btn.onClick.AddListener(OnclickGetBtn);
        }
        else
        {
            NotCanGetSet();
        }

        //判断到下一个时段 是哪一个时段且还有多少秒
        for (int i = 0; i < timeQuantum.Count; i++)
        {
            if (TimeHelper.ServerDateTimeNow < timeQuantum[i])
            {
                surplusSecond = GetSubSeconds(TimeHelper.ServerDateTimeNow, timeQuantum[i]);
                break;
            }
        }
        //倒计时
        TempTimer = StaticData.CreateTimer(surplusSecond, true,
        async (go) =>
        {//时间到了就让按钮可点击
            //TempTimer = go;
            StaticData.playerInfoData.userInfo.GetFavorableStateInfo.GetFavorableState = (int)GetFavorableState.NeverGetType;//时间到了设置未领
            await CanGetSet();
            StartCountDown();//这段计时完成后开始下一段计时
        },
        (timeCount) =>
        {
            Debug.LogError($"计时器剩余时间{timeCount}");
        }, "ReceiveDailyFavorable");
    }

    //private void OnDisable()
    //{
    //    Debug.LogError("执行Disable");
    //    CloseActive();
    //}

    /// <summary>
    /// 销毁倒计时器
    /// </summary>
    public void CloseActive()
    {
        Destroy(TempTimer.gameObject);
        TempTimer = null;
    }

    /// <summary>
    /// 领取奖励(奖励进库)
    /// </summary>
    /// <param name="isDouble">是否双倍奖励</param>
    void GetRaward(bool isDouble = false)
    {
#if UNITY_EDITOR
        CSGetLoginFavorable cSGetLoginFavorable = new CSGetLoginFavorable() { IsAdvert = isDouble };
        ProtocalManager.Instance().SendCSGetLoginFavorable(cSGetLoginFavorable, (ResponseSCEmptyLoginGetFavorableCallBack) =>
        {
            if (isDouble)
                StaticData.UpdateWareHouseItem(StaticData.configExcel.GetVertical().LoginGetFavorableValue.ID, (int)StaticData.configExcel.GetVertical().LoginGetFavorableValue.Count * StaticData.configExcel.GetVertical().GetLoginFavorableAdvertMul);
            else
                StaticData.UpdateWareHouseItem(StaticData.configExcel.GetVertical().LoginGetFavorableValue.ID, (int)StaticData.configExcel.GetVertical().LoginGetFavorableValue.Count);
            //前端设置为已领状态
            StaticData.playerInfoData.userInfo.GetFavorableStateInfo.GetFavorableState = (int)GetFavorableState.AlreadyGetType;
            this.NotCanGetSet();
            StaticData.CreateToastTips("领取成功");
        }, (ErrorInfo e) => { Debug.Log("领取好感错误"); StaticData.CreateToastTips("领取失败"); });
#else//走广告逻辑

#endif
    }

    /// <summary>
    /// 可领取设置
    /// </summary>
    async System.Threading.Tasks.Task CanGetSet()
    {
        //get_Btn.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(canGet);
        get_Btn.transform.Find("Text").GetComponent<Text>().text = "领取";
        get_Btn.onClick.RemoveAllListeners();
        get_Btn.onClick.AddListener(() => GetRaward());
        get_Btn.enabled = true;

        //doubleGet_Btn.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(canGet);
        doubleGet_Btn.transform.Find("Text").GetComponent<Text>().text = "双倍领取";
        doubleGet_Btn.onClick.RemoveAllListeners();
        doubleGet_Btn.onClick.AddListener(() => GetRaward(true));//要先看广告
        doubleGet_Btn.enabled = true;
    }

    /// <summary>
    /// 不可领取设置
    /// </summary>
    async void NotCanGetSet()
    {
        //get_Btn.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(notCanGet);
        get_Btn.transform.Find("Text").GetComponent<Text>().text = "已领取";
        get_Btn.enabled = false;

        //doubleGet_Btn.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(notCanGet);
        doubleGet_Btn.transform.Find("Text").GetComponent<Text>().text = "双倍领取";

        doubleGet_Btn.enabled = false;
    }

    /// <summary>
    /// 计算两时间差异
    /// </summary>
    /// <param name="startTime">起始时间的时间戳</param>
    /// <param name="endTime">结束时间的时间戳</param>
    public void CalculateTimeDiff(long startTime, long endTime)
    {
        var timeDiff = endTime - startTime;
        var day = timeDiff / 86400;
        var hours = (timeDiff % 86400) / 3600;
        var mins = (timeDiff % 3600) / 60;
        var secs = timeDiff % 60;
        var todate = $"{day}天，{hours}时，{mins}分，{secs}秒";
    }

    /// <summary>
    /// 返回两时间相差的总秒数
    /// </summary>
    /// <param name="startTimer"></param>
    /// <param name="endTimer"></param>
    /// <returns></returns>
    public static int GetSubSeconds(DateTime startTimer, DateTime endTimer)
    {
        TimeSpan startSpan = new TimeSpan(startTimer.Ticks);

        TimeSpan nowSpan = new TimeSpan(endTimer.Ticks);

        TimeSpan subTimer = nowSpan.Subtract(startSpan).Duration();

        //return subTimer.Seconds;

        //返回相差时长（算上分、时的差值，返回相差的总秒数）
        return (int)subTimer.TotalSeconds;
    }




}

/****
 *
 *
 * 越新的时间 其时间戳越大
 * 
 *
 */
