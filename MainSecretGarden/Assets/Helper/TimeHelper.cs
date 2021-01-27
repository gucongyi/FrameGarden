using System;
using UnityEngine;
public static class TimeHelper
{
    private static readonly long epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
    /// <summary>
    /// 客户端时间
    /// </summary>
    /// <returns></returns>
    public static long ClientNow()
    {
        return (DateTime.UtcNow.Ticks - epoch) / 10000;
    }

    public static long ClientNowSeconds()
    {
        return (DateTime.UtcNow.Ticks - epoch) / 10000000;
    }

    /// <summary>
    /// 登陆前是客户端时间,登陆后是同步过的服务器时间
    /// </summary>
    /// <returns></returns>
    public static long Now()
    {
        return ClientNow();
    }

    static long loginServerMillisecond;
    public static float loginClientTimeSinceSinceStartup;
    public static DateTime loginServerDateTime;
    public static long LoginServerTime
    {
        get
        {
            return loginServerMillisecond;
        }
        set
        {
            loginServerMillisecond = value;
            loginClientTimeSinceSinceStartup = Time.realtimeSinceStartup;
            loginServerDateTime = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc).AddMilliseconds(loginServerMillisecond);
        }
    }

    public static DateTime ServerTime(long milliSecond)
    {
        return new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc).AddMilliseconds(milliSecond);
    }

    public static string ServerTime(long milliSecond, string format)
    {
        var dt = ServerTime(milliSecond);
        return dt.ToString(format);
    }

    public static DateTime ServerDateTimeNow
    {
        get
        {
            float deltaTime = Time.realtimeSinceStartup - loginClientTimeSinceSinceStartup;
            long deltaTicks = (long)deltaTime * 10000000;//1s=1千万ticks 1s=10亿纳秒，1ticks=100纳秒
            TimeSpan ts = new TimeSpan(deltaTicks);
            return loginServerDateTime.Add(ts);
        }
    }

    /// <summary>
    /// 当前服务器时间 改变 后的时间
    /// </summary>
    /// <param name="day"></param>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public static DateTime GetDateTime(int day, int hour, int minute, int second)
    {
        var today = ServerDateTimeNow.Date;
        var todayYear = today.Year;
        var todayMonth = today.Month;
        var todayDay = today.Day;
        return new DateTime(todayYear, todayMonth, todayDay + day, hour, minute, second);
    }

    //计算服务器当前时间戳
    public static long ServerTimeStampNow
    {
        get
        {
            var deltaTime = Time.realtimeSinceStartup - loginClientTimeSinceSinceStartup;
            return loginServerMillisecond + (long)(deltaTime * 1000);
        }
    }

    public static int WeekFrom20180101()
    {
        var pass = ServerDateTimeNow - new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (int)(pass.TotalMilliseconds / MillisecondsPerWeek());
    }

    public static long MillisecondsPerWeek()
    {
        return 7 * 24 * 60 * 60 * 1000;
    }

    /// <summary>
    /// 获取当游戏时间单位天 20200101
    /// </summary>
    /// <returns></returns>
    public static string CurGameTimeDay() 
    {
        string dayTime = string.Empty;
        dayTime = ServerDateTimeNow.Year.ToString();
        int time = ServerDateTimeNow.Month;
        if (time < 10)
            dayTime = dayTime + "0";
        dayTime = dayTime + time;
        time = ServerDateTimeNow.Day;
        if (time < 10)
            dayTime = dayTime + "0";
        dayTime = dayTime + time;
        return dayTime;
    }

    /// <summary>
    /// 是否为同一天
    /// </summary>
    /// <returns></returns>
    public static bool IsTheSameDay(string day) 
    {
        string curDay = CurGameTimeDay();
        if (string.Equals(curDay, day)/*day == curDay*/) 
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 显示时分秒
    /// </summary>
    /// <param name="totalSeconds"></param>
    /// <returns></returns>
    public static string FormatTime(int totalSeconds)
    {
        int hours = totalSeconds / 3600;
        string hh = hours < 10 ? "0" + hours : hours.ToString();
        int minutes = (totalSeconds - hours * 3600) / 60;
        string mm = minutes < 10f ? "0" + minutes : minutes.ToString();
        int seconds = totalSeconds - hours * 3600 - minutes * 60;
        string ss = seconds < 10 ? "0" + seconds : seconds.ToString();
        return string.Format("{0}:{1}:{2}", hh, mm, ss);
    }
}