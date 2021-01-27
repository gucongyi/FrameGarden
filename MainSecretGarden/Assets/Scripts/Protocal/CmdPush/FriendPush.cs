using Game.Protocal;
using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendPush
{
    public void ExecuteFriendApplyCmd(IMessage msg)
    {
        var data = msg as SCFriendApplyPushMsg;
        StaticData.playerInfoData.listApplyInfo.Add(new SCFriendInfo()
        {
            Uid = data.ApplyPushMsg.Uid,
            FriendName = data.ApplyPushMsg.FriendName,
            FriendExperience = data.ApplyPushMsg.FriendExperience,
            //FriendImage = data.ApplyPushMsg.FriendImage,
            Online = data.ApplyPushMsg.Online
        });
        var uiFriendComponent = UIComponent.GetComponentHaveExist<UIFriendComponent>(UIType.UIFriend);
        if (uiFriendComponent != null && uiFriendComponent.lsApplyList.gameObject.activeInHierarchy)
        {
            uiFriendComponent.OnApplyCmdCallBack();
        }
        //多语言，todo
        string tipStr = data.ApplyPushMsg.FriendName + "请求加好友!";
        StaticData.CreateToastTips(tipStr);
    }
    public void ExecuteFriendAcceptCmd(IMessage msg)
    {
        var data = msg as SCFriendAcceptPushMsg;
        StaticData.playerInfoData.listFriendInfo.Add(new SCFriendInfo()
        {
            Uid = data.AcceptPushMsg.Uid,
            FriendName = data.AcceptPushMsg.FriendName,
            FriendExperience = data.AcceptPushMsg.FriendExperience,
            //FriendImage = data.AcceptPushMsg.FriendImage,
            Online = data.AcceptPushMsg.Online
        });
        var uiFriendComponent = UIComponent.GetComponentHaveExist<UIFriendComponent>(UIType.UIFriend);
        if (uiFriendComponent != null && uiFriendComponent.lsFriendList.gameObject.activeInHierarchy)
        {
            uiFriendComponent.OnAgreeCmdCallBack();
        }
        string tipStr = data.AcceptPushMsg.FriendName + "同意了你的好友申请!";
        StaticData.CreateToastTips(tipStr);
    }
    public void ExecuteFriendRepulseCmd(IMessage msg)
    {
        var data = msg as SCFriendRepulsePushMsg;
        string tipStr = data.RepulsePushMsg.FriendName + "拒绝了你的好友申请!";
        StaticData.CreateToastTips(tipStr);
    }
    public void ExecuteFriendDeleteCmd(IMessage msg)
    {
        var data = msg as SCFriendDeletePushMsg;
        for (int i = 0; i < StaticData.playerInfoData.listFriendInfo.Count; i++) 
        {
            if (data.OperationUid == StaticData.playerInfoData.listFriendInfo[i].Uid) 
            {
                StaticData.playerInfoData.listFriendInfo.RemoveAt(i);
                break;
            }
        }
        var uiFriendComponent = UIComponent.GetComponentHaveExist<UIFriendComponent>(UIType.UIFriend);
        if (uiFriendComponent != null && uiFriendComponent.lsFriendList.gameObject.activeInHierarchy) 
        {
            uiFriendComponent.OnDeletedCmdCallBack();
        }
    }

}
