
using Game.Protocal;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Google.Protobuf
{
    public class RigisterCmdPush
    {
        static RigisterCmdPush()
        {
            //好友申请推送
            dic[typeof(SCFriendApplyPushMsg)] = (msg, code) =>
            {
                new FriendPush().ExecuteFriendApplyCmd(msg);
            };
            //好友同意推送
            dic[typeof(SCFriendAcceptPushMsg)] = (msg, code) =>
            {
                new FriendPush().ExecuteFriendAcceptCmd(msg);
            };
            //好友拒绝推送
            dic[typeof(SCFriendRepulsePushMsg)] = (msg, code) =>
            {
                new FriendPush().ExecuteFriendRepulseCmd(msg);
            };
            //好友删除推送
            dic[typeof(SCFriendDeletePushMsg)] = (msg, code) =>
            {
                new FriendPush().ExecuteFriendDeleteCmd(msg);
            };
            //聊天推送
            dic[typeof(SCChat)] = (msg, code) =>
            {
                //世界聊天推送
                if (code == 50011)
                {
                    ChatTool.ExecuteWorldChatMessageCmd(msg);
                }
                else if (code == 50012)// 私聊
                {
                    ChatTool.ExecutePrivateChatMessageCmd(msg);
                }
                else if (code == 50013)//房间聊天
                {
                    ChatTool.ExecuteRoomChatMessageCmd(msg);
                }
                else
                {
                    Debug.LogError("code = " + code + " not register");
                }
            };
            //晚会玩家进入房间 推送
            dic[typeof(SCEntranceRoomInfo)] = (msg, code) =>
            {
                PartyServerDockingManager.PushEntranceRoom(msg);
            };
            //晚会玩家移动 推送
            dic[typeof(SCMoveLocation)] = (msg, code) =>
            {
                PartyServerDockingManager.PushPlayerMove(msg);
            };
            //晚会有玩家退出房间 推送
            dic[typeof(SCDepartureRoom)] = (msg, code) =>
            {
                PartyServerDockingManager.PushPlayerQuitRoom(msg);
            };
            //晚会结束 推送
            dic[typeof(SCActivityFinish)] = (msg, code) =>
            {
                PartyServerDockingManager.PushPartyEnd(msg);
            };
            //晚会竞猜信息 推送
            dic[typeof(SCpushGuessingInfo)] = (msg, code) =>
            {
                PartyServerDockingManager.PushPartyGuessInfo(msg);
            };

            //系统公告 推送
            dic[typeof(SCNotePushMess)] = (msg, code) =>
            {
                StaticData.PushSystemNotification(msg);
            };
            dic[typeof(SCEmtpyMailPushMsg)] = (msg, code) =>
            {
                MailboxTool.ReceptionPushData(msg);
            };
            dic[typeof(SCSendMailPushMsg)] = (msg, code) =>
            {
                MailboxTool.ReceptionPushDataTwo(msg);
            };
        }


        static Dictionary<Type, Action<IMessage, int>> dic = new Dictionary<Type, Action<IMessage, int>>();
        public static void PushMsgHandle(IMessage baseMsg, int code = 0)
        {
            if (baseMsg!=null)
            {
                StaticData.DebugGreen($"收到推送：{baseMsg.ToString()}");
            }
            if (dic.TryGetValue(baseMsg.GetType(), out Action<IMessage, int> action))
            {
                action(baseMsg, code);
            }
            else
            {
                Debug.LogError(baseMsg.GetType() + " not register");
            }
        }

    }
}


