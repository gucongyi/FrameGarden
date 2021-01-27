using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 竞猜管理器
/// </summary>
public class PartyGuessManager
{
    #region 变量
    /// <summary>
    /// 单例字段
    /// </summary>
    private static PartyGuessManager instance;
     
    //是否能够下注
    public bool isCanGuess=false;

    //竞猜时间差
    public float TimeDifference;

    #endregion

    #region 属性
    public static PartyGuessManager Instance
    {
        get
        {
            if (instance==null)
            {
                return instance = new PartyGuessManager();
                   
            }

            return instance;
        }
    }
    #endregion

    public PartyGuessManager()
    {
        instance = this;
    }


}
