using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SignTool : MonoBehaviour
{
    //是否开启活动红点
    public static bool _isOpenActivityReDot;
    private static long nextSignTime = 0;//下次签到时间戳

    //用于0点过 保存昨天的下次签到时间戳
    private static long signTime = 0;

    //初始化当前玩家活动状态 是否许愿过
    private static void InitActivityState()
    {
        _isOpenActivityReDot = false;
        if (StaticData.playerInfoData.userInfo.SignInInfo.Count == 0)
        {
            //今日0点时间戳
            long zeroTime = TimeHelper.ServerTimeStampNow - (TimeHelper.ServerTimeStampNow + 8 * 3600 * 1000) % 86400000;
            var signInRenewalTimeC = StaticData.configExcel.GetVertical().SignInRenewalTime;
            //签到时间对应的小时时间戳
            long timeDelta = signInRenewalTimeC[0] * 60 * 60 * 1000 + signInRenewalTimeC[1] * 60 * 1000 + signInRenewalTimeC[2] * 1000;
            long todaySignTimeStamp = zeroTime + timeDelta;
            //明天0点时间戳
            nextSignTime = todaySignTimeStamp + 24 * 3600 * 1000;
            //数据保存
            signTime = nextSignTime;

            _isOpenActivityReDot = true;
        }
        else
        {
            long lastSignTimeStamp = 0;//上次签到时间
            for (int i = 0; i < StaticData.playerInfoData.userInfo.SignInInfo.Count; i++)
            {
                var data = StaticData.playerInfoData.userInfo.SignInInfo[i];
                if (data.DayNumber == 1)
                {
                    lastSignTimeStamp = data.SignInTime;
                    break;
                }
            }

            //今日0点时间戳
            long zeroTime = TimeHelper.ServerTimeStampNow - (TimeHelper.ServerTimeStampNow + 8 * 3600 * 1000) % 86400000;
            var signInRenewalTimeC = StaticData.configExcel.GetVertical().SignInRenewalTime;
            //签到时间对应的小时时间戳
            long timeDelta = signInRenewalTimeC[0] * 60 * 60 * 1000 + signInRenewalTimeC[1] * 60 * 1000 + signInRenewalTimeC[2] * 1000;
            long todaySignTimeStamp = zeroTime + timeDelta;
            //明天0点时间戳
            nextSignTime = todaySignTimeStamp + 24 * 3600 * 1000;
            //数据保存
            signTime = nextSignTime;

            if (TimeHelper.ServerTimeStampNow >= lastSignTimeStamp)
            {
                _isOpenActivityReDot = true;
            }
        }
    }


    //是否打开活动红点
    public static bool IsOpenRedDot()
    {
        InitActivityState();

        return _isOpenActivityReDot;
    }

    //0点刷新红点
    public static void UpdateZeroRedDot()
    {
        //重置数据
        _isOpenActivityReDot = false;
        if (StaticData.playerInfoData.userInfo.SignInInfo.Count == 0)
        {
            _isOpenActivityReDot = true;
        }
        else
        {
            for (int i = 0; i < StaticData.playerInfoData.userInfo.SignInInfo.Count; i++)
            {
                var data = StaticData.playerInfoData.userInfo.SignInInfo[i];
                if (data.DayNumber == 1)
                {
                     data.SignInTime=signTime;
                    break;
                }
            }
        }

        //显示红点
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Activity);
    } 
}
