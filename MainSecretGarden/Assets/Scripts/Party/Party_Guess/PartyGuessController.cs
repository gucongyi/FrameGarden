using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyGuessController
{
    /// <summary>
    /// 单例字段
    /// </summary>
    private static PartyGuessController instance;

    #region 变量
    //玩家是否下注
    public bool isGuessPalyer;
    #endregion


    #region 属性
    public static PartyGuessController Instance
    {
        get
        {
            if (instance == null)
            {
                return instance = new PartyGuessController();
            }
            else
            {
                return instance;
            }
        }
    }
    #endregion

    public PartyGuessController()
    {
        instance = this;
        isGuessPalyer = false;
    }


    /// <summary>
    /// 获取玩家当前拥有的票数
    /// </summary>
    public int GetNowPlayerPolls()
    {
        // StaticData.GetWareHouseItem(); 获取应援币

        //模拟数据
        int nowpolls = 6;

        UIPartyGuessController.Instance.SetHasPolls(nowpolls.ToString());

        return nowpolls;
    }


    /// <summary>
    /// 开始竞猜
    /// </summary>
    public void StartGuess()
    {
        //请求服务器 接收乌龟竞猜过程和结果
        PartyServerDockingManager.NotifyServerStartGuess(StartGuessSuccess, StartGuessFailed);
        
    }
    private async void StartGuessSuccess()
    {
        Debug.Log("玩家有下注:现在开始竞猜");
        //关闭下注界面
        StaticData.CloseGuess();

        //显示相对应乌龟赛跑UI


        //显示竞猜结果界面
        await StaticData.OpenGuessResultUI();



    }
    private void StartGuessFailed()
    {

    }


    /// <summary>
    /// 结束竞猜
    /// </summary>
    public void EndGuess()
    {
        Debug.Log("玩家有下注：现在结束竞猜，开始计时下注");
        //关闭竞猜结果界面
        StaticData.CloseGuessResultUI();

        //发放奖励

        //清空玩家下注信息(本地)
        PartyGuessInfo.Instance.ClearPartyGuessInfo();



        Debug.Log("清空下注消息");
    }


    /// <summary>
    /// 开始竞猜,玩家没下注
    /// </summary>
    public async void StartGuessNoData()
    {
        Debug.Log("玩家没下注:现在开始竞猜");

        //关闭下注界面
        StaticData.CloseGuess();

        //显示相对应乌龟赛跑UI


        //显示竞猜结果界面
        await StaticData.OpenGuessResultUI();
    }
    /// <summary>
    /// 结束竞猜，玩家没下注
    /// </summary>
    public void EndGuessNoData()
    {
        Debug.Log("玩家没下注:现在结束竞猜，开始计时下注");
        //关闭竞猜结果界面
        StaticData.CloseGuessResultUI();
    }

}
