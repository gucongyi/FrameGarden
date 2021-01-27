using Game.Protocal;
using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 模拟下注状态数据
/// </summary>
public enum guessState
{
    bottompour,
    guess
}



/// <summary>
/// 晚会服务器对接管理器
/// </summary>
public static class PartyServerDockingManager
{
    /// <summary>
    /// 通知服务器请求晚会房间信息
    /// </summary>
    /// <param name="successAction"></param>
    /// <param name="failedAction"></param>
    public static void NotifyServerRequestRoomList(Action<SCRoomListInfo> successAction, Action failedAction)
    {
        CSEmptyRoomList cSEmptyRoomList = new CSEmptyRoomList();
        ProtocalManager.Instance().SendCSEmptyRoomList(cSEmptyRoomList, (SCRoomListInfo sCRoomListInfo) =>
        {
            Debug.Log("通知服务器请求晚会房间信息成功！");
            successAction?.Invoke(sCRoomListInfo);
        },
        (ErrorInfo er) =>
        {
            Debug.Log("通知服务器请求晚会房间信息失败！Error：" + er.ErrorMessage);
            failedAction?.Invoke();
        });
    }

    /// <summary>
    /// 通知服务器快速加入房间
    /// </summary>
    /// <param name="successAction"></param>
    /// <param name="failedAction"></param>
    public static void NotifyServerQuicklyJoinRoom(Action<SCEntranceRoom,guessState> successAction, Action failedAction) 
    {
        EntranceRoom(successAction, failedAction);
    }

    /// <summary>
    /// 通知服务器加入房间
    /// </summary>
    /// <param name="roomID"></param>
    /// <param name="successAction"></param>
    /// <param name="failedAction"></param>
    public static void NotifyServerEntranceRoom(int roomID, Action<SCEntranceRoom,guessState> successAction, Action failedAction)
    {
        EntranceRoom(successAction, failedAction, roomID);
    }

    /// <summary>
    /// 加入房间
    /// </summary>
    /// <param name="successAction"></param>
    /// <param name="failedAction"></param>
    /// <param name="roomID"></param>
    private static void EntranceRoom(Action<SCEntranceRoom,guessState> successAction, Action failedAction, int roomID = -1) 
    {
        CSEntranceRoom cSEntranceRoom = new CSEntranceRoom();
        if (roomID != -1)
            cSEntranceRoom.RoomId = roomID;

        //模拟测试数据
        Debug.Log("获取当前下注时间成功");

        ProtocalManager.Instance().SendCSEntranceRoom(cSEntranceRoom, (SCEntranceRoom sCEntranceRoom) =>
        {
            //sCEntranceRoom.ActivityInfo.GameStates;

            Debug.Log(sCEntranceRoom.ActivityInfo.GameStates);

            Debug.Log("通知服务器加入房间成功！");
            successAction?.Invoke(sCEntranceRoom,guessState.bottompour);
        },
        (ErrorInfo er) =>
        {
            Debug.Log("通知服务器加入房间失败！Error：" + er.ErrorMessage);
            failedAction?.Invoke();
        });
    }


    /// <summary>
    /// 通知服务器玩家移动
    /// </summary>
    /// <param name="moveTarget"></param>
    /// <param name="successAction"></param>
    /// <param name="failedAction"></param>
    public static void NotifyServerPlayerMove(Vector3 moveTarget, Action successAction, Action failedAction) 
    {
        CSMoveLocation cSMoveLocation = new CSMoveLocation();
        cSMoveLocation.Xaxle = moveTarget.x;
        cSMoveLocation.Yaxle = moveTarget.y;

        ProtocalManager.Instance().SendCSMoveLocation(cSMoveLocation, (SCEmtpyMoveLocation sCEmtpyMoveLocation) =>
        {
            Debug.Log("通知服务器玩家移动成功！");
            successAction?.Invoke();
        },
        (ErrorInfo er) =>
        {
            Debug.Log("通知服务器玩家移动失败！Error：" + er.ErrorMessage);
            failedAction?.Invoke();
        });
    }

    /// <summary>
    /// 通知服务器退出房间
    /// </summary>
    /// <param name="successAction"></param>
    /// <param name="failedAction"></param>
    public static void NotifyServerQuitRoom(Action successAction, Action failedAction)
    {
        CSEmptyDepartureRoom cSEmptyDepartureRoom = new CSEmptyDepartureRoom();
        ProtocalManager.Instance().SendCSEmptyDepartureRoom(cSEmptyDepartureRoom, (SCEmptyDepartureRoom sCEmptyDepartureRoom) =>
        {
            Debug.Log("通知服务器退出房间成功！");
            successAction?.Invoke();
        },
        (ErrorInfo er) =>
        {
            Debug.Log("通知服务器退出房间失败！Error：" + er.ErrorMessage);
            failedAction?.Invoke();
        });
    }

    /// <summary>
    /// 通知服务器开始竞猜   修改_SZY_2020/11/3
    /// </summary>
    /// <param name="successAction"></param>
    /// <param name="failedAction"></param>
    public static void NotifyServerStartGuess(Action successAction, Action failedAction)
    {
        //从下注信息里面取值
        GuessingNumber guessWg;
        int guessPolls;
        PartyGuessInfo.Instance.GetGuessInfo(PartyGuessInfo.Instance.Wg_Name, out guessWg, out guessPolls);
        Debug.Log("乌龟：" + guessWg + "票数:" + guessPolls);


        //CSGuessing cSGuessing = new CSGuessing();
        //cSGuessing.TortoiseId = guessWg;
        //cSGuessing.SupportNum = guessPolls;

        //ProtocalManager.Instance().SendCSGuessing(cSGuessing, (SCEmtpyGuessing sCEmtpyGuessing) =>
        //{
        //    Debug.Log("通知服务器开始竞猜成功！");
        //    successAction?.Invoke();
        //},
        //(ErrorInfo er) =>
        //{
        //    Debug.Log("通知服务器开始竞猜失败！Error：" + er.ErrorMessage);
        //   failedAction?.Invoke();
        //});

        successAction.Invoke();

    }

    #region 推送
    /// <summary>
    /// 推送玩家进入房间 回调
    /// </summary>
    public static Action<SCEntranceRoomInfo> PushEntranceRoomCallback;

    /// <summary>
    /// 推送玩家进入房间
    /// </summary>
    /// <param name="msg"></param>
    public static void PushEntranceRoom(IMessage msg)
    {
        var data = msg as SCEntranceRoomInfo;
        PushEntranceRoomCallback?.Invoke(data);
        Debug.Log("推送玩家进入房间");
    }

    /// <summary>
    /// 推送玩家移动 回调
    /// </summary>
    public static Action<SCMoveLocation> PushPlayerMoveCallback;

    /// <summary>
    /// 推送玩家移动
    /// </summary>
    /// <param name="msg"></param>
    public static void PushPlayerMove(IMessage msg) 
    {
        var data = msg as SCMoveLocation;
        PushPlayerMoveCallback?.Invoke(data);
        Debug.Log("推送玩家移动");
    }

    /// <summary> 
    /// 推送有玩家退出房间 回调
    /// </summary>
    public static Action<SCDepartureRoom> PushPlayerQuitRoomCallback;

    /// <summary>
    /// 推送有玩家退出房间
    /// </summary>
    /// <param name="msg"></param>
    public static void PushPlayerQuitRoom(IMessage msg)
    {
        var data = msg as SCDepartureRoom;
        PushPlayerQuitRoomCallback?.Invoke(data);
        Debug.Log("推送有玩家退出房间");
    }


    /// <summary> 
    /// 推送晚会结束 回调
    /// </summary>
    public static Action<SCActivityFinish> PushPartyEndCallback;
    /// <summary>
    /// 推送晚会结束
    /// </summary>
    /// <param name="msg"></param>
    public static void PushPartyEnd(IMessage msg)
    {
        var data = msg as SCActivityFinish;
        PushPartyEndCallback?.Invoke(data);
        Debug.Log("推送晚会结束");
    }

    /// <summary>
    /// 推送竞猜比赛信息 回调 
    /// </summary>
    public static Action<SCpushGuessingInfo> PushPartyGuessInfoCallback;
    /// <summary>
    /// 推送竞猜比赛信息
    /// </summary>
    /// <param name="msg"></param>
    public static void PushPartyGuessInfo(IMessage msg)
    {
        var data = msg as SCpushGuessingInfo;
        PushPartyGuessInfoCallback?.Invoke(data);
        Debug.Log("推送竞猜比赛信息");
    }

    #endregion
}
