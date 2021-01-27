using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OtherProtoHelper
{
    public static void GetUserInfo(CSEmptyAccountInfo csEmptyAccountInfo, Action<SCUserInfo> ResponseSCUserInfoCallBack)
    {
        if (StaticData.IsUsedLocalDataNotServer)
        {
            SCUserInfo scUserInfo = new SCUserInfo()
            {
                Image = 11111,
                Name = "TestName",
                Experience = 1000,
                PresentTime = TimeHelper.ClientNow()
            };
            ResponseSCUserInfoCallBack(scUserInfo);
        }
        else
        {
            ProtocalManager.Instance().SendCSEmptyAccountInfo(csEmptyAccountInfo, ResponseSCUserInfoCallBack, (errorInfo) => { });
        }
    } 
        
}
