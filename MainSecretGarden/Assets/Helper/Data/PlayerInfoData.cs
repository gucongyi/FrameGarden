using Game.Protocal;
using Google.Protobuf.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoData
{
    //好友数据
    public RepeatedField<SCFriendInfo> listFriendInfo = new RepeatedField<SCFriendInfo>();
    public RepeatedField<SCFriendInfo> listApplyInfo = new RepeatedField<SCFriendInfo>();
    public RepeatedField<SCFriendInfo> listRecommendInfo = new RepeatedField<SCFriendInfo>();
    public List<FriendStealInfo> listFriendStealInfo = new List<FriendStealInfo>();
    public SCUserInfo userInfo;
    public List<Favorable> favorableData = new List<Favorable>();

    /// <summary>
    /// 订单数据 当前订单
    /// </summary>
    public SCDealInfo CurrDeals = new SCDealInfo();
    /// <summary>
    /// 获取订单时间
    /// </summary>
    public string GetDealTime = string.Empty;
    //本地存储数据
    public LocalSaveData CurLocalSaveData = new LocalSaveData();
}

/// <summary>
/// 本地存储数据
/// </summary>
public class LocalSaveData 
{
    /// <summary>
    /// 完成的开启共功能id
    /// </summary>
    public List<int> FinishedOpenFunIDs = new List<int>();
}

/// <summary>
/// 庄园好友信息
/// </summary>
public class FriendStealInfo 
{
    public string nickname;
    public long uid;
    public int headIcon;
    public int level;
    public bool isSteal;//是否可偷取
    public StaticData.PlayerLevelAndCurrExp playerLevelAndCurrExp;
}