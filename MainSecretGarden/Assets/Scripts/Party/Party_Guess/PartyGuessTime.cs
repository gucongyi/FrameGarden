using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 游戏时间控制
/// </summary>
public class PartyGuessTime
{
    /// <summary>
    /// 单例字段
    /// </summary>
    private static PartyGuessTime instance;
    public static PartyGuessTime Instance
    {
        get
        {
            if (instance == null)
            {
                return instance=new PartyGuessTime();
            }
            else
            {
                return instance;
            }
        }
    }

    #region 变量
    //下注期常量 竞猜期常量 （先假定 秒为单位）
    private long bottompourTimeConstant = 50;
    private long guessTimeConstant = 10;
    //晚会开始时间
    public long startPartyTime = 1605070800000;

    //用于判断的变量 是否是从下注期进入的
    private bool isBottompourFirst = false;

    //保存这个阶段截
    public guessState guessState;

    #endregion

    public PartyGuessTime()
    {
        instance = this;
    }


    /// <summary>
    /// 游戏时间控制 控制每个阶段的时间戳 控制时间间隔
    /// </summary>
    /// <returns></returns>
    public void GuessTimeControl()
    {
        if (PartyManager._instance.stayRoom)
        {
            switch (guessState)
            {
                case guessState.bottompour:
                    PartyGuessManager.Instance.TimeDifference = bottompourTimeConstant;
                    PartyGuessManager.Instance.isCanGuess = true;
                    break;
                case guessState.guess:
                    PartyGuessManager.Instance.TimeDifference = guessTimeConstant;
                    PartyGuessManager.Instance.isCanGuess = false;
                    break;
            }
        }
        else
        {
            switch (guessState)
            {
                case guessState.bottompour://如果进房间就是下注期 那么竞赛期就是固定时间
                    PartyGuessManager.Instance.TimeDifference = bottompourTimeConstant-(PartyManager._instance.dateTime-startPartyTime) / 1000; //转化为秒数
                    PartyGuessManager.Instance.isCanGuess = true;
                    isBottompourFirst = true; 
                    break;
                case guessState.guess:
                    if (isBottompourFirst)
                    {
                        PartyGuessManager.Instance.TimeDifference = guessTimeConstant;
                    }
                    else
                    {
                        PartyGuessManager.Instance.TimeDifference = guessTimeConstant-((PartyManager._instance.dateTime-startPartyTime) / 1000-bottompourTimeConstant); //转化为秒数
                    }
                    PartyGuessManager.Instance.isCanGuess = false;

                    PartyManager._instance.stayRoom = true;
                    break;
            }

        }

    }


    /// <summary>
    /// 设置每天晚会开始时间
    /// </summary>
    private void SetTaskTime()
    {
        DateTime now = DateTime.Now;

        DateTime startGame = DateTime.Today.AddHours(19.0);//晚上7点

        if (now > startGame)
        {
            startGame = startGame.AddDays(1.0);
        }

        Debug.Log(startGame);

    }

    /// <summary>
    /// DateTime时间格式转换为时间戳格式
    /// </summary>
    /// <param name="time">时间</param>
    /// <returns>long</returns>
    public static long ConvertDateTimeToInt(DateTime time)
    {
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
        long t = (time.Ticks - startTime.Ticks) / 10000;  //除10000调整为13位  
        return t;
    }


    /// <summary>   
    /// 时间戳转为DateTime格式时间   
    /// </summary>   
    /// <param name=”timeStamp”></param>   
    /// <returns></returns>   
    public DateTime ConvertStringToDateTime(string timeStamp)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = long.Parse(timeStamp + "0000");
        TimeSpan toNow = new TimeSpan(lTime);
        return dtStart.Add(toNow);
    }

}
