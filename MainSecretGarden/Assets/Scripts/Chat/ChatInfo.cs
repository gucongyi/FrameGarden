using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 聊天数据结构信息
/// </summary>
public class ChatInfo
{
    /// <summary>
    /// 是否为时间间隔签
    /// </summary>
    public bool IsTimeIntervalSign;
    /// <summary>
    /// 当前消息时间
    /// </summary>
    public string Time;
    /// <summary>
    /// 单位秒
    /// </summary>
    public long TimeTicks;
    /// <summary>
    /// 玩家头像id
    /// </summary>
    public int _iconId;
    /// <summary>
    /// 玩家头像地址
    /// </summary>
    public string _iconUrl;
    /// <summary>
    /// 玩家信息内容
    /// </summary>
    public string _message;
    /// <summary>
    /// 玩家名字
    /// </summary>
    public string _playName;
    /// <summary>
    /// 玩家uid
    /// </summary>
    public long _playUid;
    /// <summary>
    /// 玩家经验
    /// </summary>
    public int _experience;

    public ChatInfo()
    {
    }

    public ChatInfo(SCChat chat)
    {
        InitTime();
        _iconId = chat.Image;
        _message = chat.Message;
        _playName = chat.Name;
        _playUid = chat.Uid;
        _experience = (int)chat.Experience;
    }

    public ChatInfo(bool isTimeIntervalSign)
    {
        InitTime();
        IsTimeIntervalSign = isTimeIntervalSign;
    }

    /// <summary>
    /// 初始化时间
    /// </summary>
    public void InitTime()
    {
        Time = DateTime.Now.ToString();//Debug.Log( DateTime.Now.ToString("yyyyMMdd_HH"));//年月日格式     20190604_10
        TimeTicks = TimeHelper.ClientNowSeconds();
    }
    /// <summary>
    /// 更新数据
    /// </summary>
    /// <param name="chat"></param>
    public void RefreshData(ChatInfo chat)
    {
        _iconId = chat._iconId;
        _playName = chat._playName;
        _experience = chat._experience;
    }
}

/// <summary>
/// 私聊存储信息
/// </summary>
public class PrivateChatSaveInfo
{
    /// <summary>
    /// 私聊玩家的uid
    /// </summary>
    public long PrivateChatRoleUid;
    /// <summary>
    /// 最后一次发消息的时间
    /// </summary>
    public long LastTimeTicks;
    /// <summary>
    /// 是否已经阅读
    /// </summary>
    public bool IsRead;
    /// <summary>
    /// iconId
    /// </summary>
    public int iconId;
    /// <summary>
    /// 头像地址
    /// </summary>
    public string _iconUrl;
    /// <summary>
    /// 私聊对象昵称
    /// </summary>
    public string _playName;
    /// <summary>
    /// 玩家经验
    /// </summary>
    public int _experience;
    /// <summary>
    /// 聊天信息
    /// </summary>
    public List<ChatInfo> ChatInfos;

    public void RefreshData(ChatInfo chat)
    {

        if (chat._playUid == PrivateChatRoleUid)
        {
            LastTimeTicks = chat.TimeTicks;
            iconId = chat._iconId;
            _playName = chat._playName;
            _experience = chat._experience;
        }

        if (ChatInfos != null && ChatInfos.Count > 0)
        {
            for (int i = 0; i < ChatInfos.Count; i++)
            {

                if (ChatInfos[i]._playUid == chat._playUid)
                {
                    ChatInfos[i].RefreshData(chat);
                }
            }
        }
    }
}

/// <summary>
/// 系统公告
/// </summary>
public class SystemNotification
{
    public PushNoticeSource NoticeSource;
    public string Desc;
}