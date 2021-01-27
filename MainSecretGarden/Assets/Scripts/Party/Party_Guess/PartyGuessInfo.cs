using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Protocal;

public class PartyGuessInfo
{
    /// <summary>
    /// 单例字段 
    /// </summary>
    private static PartyGuessInfo instance;
    public static PartyGuessInfo Instance
    {
        get
        {
            if (instance == null)
            {
                return instance = new PartyGuessInfo();
            }
            else
            {
                return instance;
            }
        }
    }

    #region 变量
    //所选择的乌龟
    public string Wg_Name;


    #endregion



    //key为乌龟名字,value为票数
    public static Dictionary<string, int> userGuessInfoDic = new Dictionary<string, int>();


    
    public PartyGuessInfo()
    {
        instance = this;
    }

    /// <summary>
    /// 返回玩家下注的乌龟和票数
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="guessWg"></param>
    /// <param name="guessPolls"></param>
    public void GetGuessInfo(string wgName, out GuessingNumber guessWg,out int guessPolls)
    {
        int polls;
        userGuessInfoDic.TryGetValue(wgName,out polls);


        switch (wgName)
        {
            case "Tog_WG1":
                guessWg = GuessingNumber.GuessingFirst;
                break;
            case "Tog_WG2":
                guessWg = GuessingNumber.GuessingSecond;
                break;
            case "Tog_WG3":
                guessWg = GuessingNumber.GuessingThird;
                break;
            default:
                guessWg = GuessingNumber.GuessingFirst;
                break;
        }

        guessPolls = polls;
    }

    /// <summary>
    /// 清空玩家下注消息
    /// </summary>
    public void ClearPartyGuessInfo()
    {
        userGuessInfoDic.Clear();
        Wg_Name = null;

        //将UI中的临时变量清空
        UIPartyGuessController.Instance._WGName = null;
        UIPartyGuessController.Instance._polls = 0;

        //将玩家是否下注设为false
        PartyGuessController.Instance.isGuessPalyer = false;
    }
}
