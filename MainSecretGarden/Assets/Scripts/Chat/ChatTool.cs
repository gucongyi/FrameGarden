using Company.Cfg;
using Cysharp.Threading.Tasks;
using Game.Protocal;
using Google.Protobuf;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
/// <summary>
/// 聊天工具箱
/// 2020/10/28 HuangJiangDong
/// </summary>
public static class ChatTool
{
    #region 字段
    /// <summary>
    /// 世界频道聊天信息 队列
    /// </summary>
    private static List<ChatInfo> _worldChannelMessages = new List<ChatInfo>();
    /// <summary>
    /// 上次接收频道聊天信息 时间
    /// </summary>
    private static long _lastReceiveWorldMessageTime = 0;
    /// <summary>
    /// 私聊频道消息
    /// </summary>
    private static List<PrivateChatSaveInfo> _privateChannelMessages = new List<PrivateChatSaveInfo>();
    /// <summary>
    /// 房间聊天消息列表
    /// </summary>
    static List<ChatInfo> _roomChats = new List<ChatInfo>();
    /// <summary>
    /// 收到世界消息回调
    /// </summary>
    static Action _receiveInformationAction;
    /// <summary>
    /// 收到房间消息回调
    /// </summary>
    static Action _receiveRoomInformationAction;
    /// <summary>
    /// 收到私聊消息回调
    /// </summary>
    static Action<long> _receivePrivateInformationAction;
    /// <summary>
    /// 弹幕接收消息回调 
    /// </summary>
    static Action<ChatInfo> _takePushDataAction;
    /// <summary>
    /// 聊天小按钮接收到消息回调
    /// </summary>
    static Action _chatMiniAction;
    /// <summary>
    /// 聊天框接收消息回调 
    /// </summary>
    static Action<ChatInfo, bool> _chatFtameTakePushDataAction;
    /// <summary>
    /// 弹幕开关本地储存标签
    /// </summary>
    public const string BULLET_SCREEN_ON_OFF_SAVE = "BULLET_SCREEN_ON_OFF_SAVE";
    /// <summary>
    /// 聊天弹幕控制器
    /// </summary>
    static ChatBulletScreenController _chatBulletScreenController;
    /// <summary>
    /// 信息发送cd剩余时间
    /// </summary>
    public static float _sendRemainingTime = 0f;
    /// <summary>
    /// 离开聊天面板时间戳
    /// </summary>
    public static long _chatLeaveTime;
    /// <summary>
    /// 是否已经结束cd计时
    /// </summary>
    public static bool _isOverCd = false;
    /// <summary>
    /// 是否处于晚会
    /// </summary>
    static bool _isBeRoom = false;
    #endregion
    #region 属性
    public static List<PrivateChatSaveInfo> _PrivateChannelMessages { get { return _privateChannelMessages; } }
    /// <summary>
    /// 是否处于晚会
    /// </summary>
    public static bool _IsBeRoom { get { return _isBeRoom; } }
    #endregion
    #region 函数
    #region 与服务器交互
    /// <summary>
    /// 服务推送来的消息 -- 世界
    /// </summary>
    public static void ExecuteWorldChatMessageCmd(IMessage msg)
    {
        var data = msg as SCChat;
        ChatInfo chatInfo = new ChatInfo(data);
        EnqueuetWorldChannelMessages(chatInfo);
        _chatMiniAction?.Invoke();
    }
    /// <summary>
    /// 服务推送来的消息 -- 私聊
    /// </summary>
    public static void ExecutePrivateChatMessageCmd(IMessage msg)
    {
        Debug.Log("接受私聊信息推送");
        var data = msg as SCChat;
        int uID = (int)data.Uid;
        ChatInfo chatInfo = new ChatInfo(data);
        ReceivePrivateChatMessage((int)chatInfo._playUid, chatInfo);
        _chatMiniAction?.Invoke();
    }
    /// <summary>
    /// 服务推送来的消息 -- 房间
    /// </summary>
    public static void ExecuteRoomChatMessageCmd(IMessage msg)
    {
        var data = msg as SCChat;
        ChatInfo chatInfo = new ChatInfo(data);
        EnqueuetRoomChannelMessages(chatInfo);
        _chatMiniAction?.Invoke();
    }
    /// <summary>
    /// 发送聊天信息 世界聊天
    /// </summary>
    /// <param name="successAction"></param>
    /// <param name="failedAction"></param>
    public static void NotifyServerWorldChatMessage(string message, Action successAction, Action failedAction)
    {
#if UNITY_EDITOR
        if (StaticData.IsUsedLocalDataNotServer)
        {
            successAction?.Invoke();
            return;
        }

#endif
        CSWorldChat cSChatMessage = new CSWorldChat();
        cSChatMessage.Message = message;
        ProtocalManager.Instance().SendCSWorldChat(cSChatMessage, (SCEmptyWorldChat sCEmptyWorldChat) =>
        {
            Debug.Log("发送聊天信息成功！");
            successAction?.Invoke();
        },
            (ErrorInfo er) =>
            {
                Debug.Log("通知服务器发送聊天信息：" + er.ErrorMessage);
                failedAction?.Invoke();
            });
    }
    /// <summary>
    /// 发送聊天信息 私聊
    /// </summary>
    /// <param name="successAction"></param>
    /// <param name="failedAction"></param>
    public static void NotifyServerPrivateChatMessage(long uid, string message, Action successAction, Action failedAction)
    {
#if UNITY_EDITOR
        if (StaticData.IsUsedLocalDataNotServer)
        {
            successAction?.Invoke();
            return;
        }

#endif
        CSPrivateChat cSChatMessage = new CSPrivateChat();
        cSChatMessage.Message = message;
        cSChatMessage.Uid = uid;
        ProtocalManager.Instance().SendCSPrivateChat(cSChatMessage, (SCEmptyPrivateChat sCEmptyPrivateChat) =>
        {
            Debug.Log("发送聊天信息成功！");
            successAction?.Invoke();
        },
            (ErrorInfo er) =>
            {
                Debug.Log("通知服务器发送聊天信息：" + er.ErrorMessage);
                failedAction?.Invoke();
            });
    }
    /// <summary>
    /// 发送聊天信息 房间
    /// </summary>
    /// <param name="successAction"></param>
    /// <param name="failedAction"></param>
    public static void NotifyServerRoomChatMessage(string message, Action successAction, Action failedAction)
    {
#if UNITY_EDITOR
        if (StaticData.IsUsedLocalDataNotServer)
        {
            successAction?.Invoke();
            return;
        }

#endif

        CSChannelChat cSChannelChat = new CSChannelChat();
        cSChannelChat.Message = message;
        ProtocalManager.Instance().SendCSChannelChat(cSChannelChat, (SCEmtpyChannelChat sCEmtpyChannelChat) =>
        {
            Debug.Log("发送聊天信息成功！");
            successAction?.Invoke();
        },
            (ErrorInfo er) =>
            {
                Debug.Log("通知服务器发送聊天信息：" + er.ErrorMessage);
                failedAction?.Invoke();
            });
    }
    #endregion
    /// <summary>
    /// 初始化聊天
    /// </summary>
    public static async UniTask InitialChat()
    {
        //读取本地保存的私聊信息
        LoadPrivateChatFile();

        await StaticData.CreateChatMini();

        if (GetBulletScreenOnOff())
        {
            await StaticData.CreateChatBulletScreen();
        }

    }
    /// <summary>
    /// 将消息加入房间聊天列表
    /// </summary>
    /// <param name="chatInfo"></param>
    public static void EnqueuetRoomChannelMessages(ChatInfo chatInfo)
    {
        //判断是否需要加入时间间隔条
        if (_roomChats.Count > 0)
        {
            long lastTime = 0;
            lastTime = _roomChats[_roomChats.Count - 1].TimeTicks;
            //间隔时间大于300s
            if (chatInfo.TimeTicks - lastTime >= StaticData.configExcel.GetVertical().ChatMessageInterval)
            {
                ChatInfo timeChatInfo = new ChatInfo(true);
                _roomChats.Add(timeChatInfo);
            }
        }
        GuaranteedWorldMessageLength(ref _roomChats, StaticData.configExcel.GetVertical().WorldChatDataNumber);
        UpdateData(ref _roomChats, chatInfo);
        _roomChats.Add(chatInfo);
        _receiveRoomInformationAction?.Invoke();
    }
    /// <summary>
    /// 将消息加入世界聊天列表
    /// </summary>
    /// <param name="chatInfo"></param>
    public static void EnqueuetWorldChannelMessages(ChatInfo chatInfo)
    {
        //判断是否需要加入时间间隔条
        if (_worldChannelMessages.Count > 0)
        {
            //间隔时间大于300s
            if (chatInfo.TimeTicks - _lastReceiveWorldMessageTime >= StaticData.configExcel.GetVertical().ChatMessageInterval)
            {
                ChatInfo timeChatInfo = new ChatInfo(true);
                _worldChannelMessages.Add(timeChatInfo);
            }
        }
        GuaranteedWorldMessageLength(ref _worldChannelMessages, StaticData.configExcel.GetVertical().WorldChatDataNumber);
        UpdateData(ref _worldChannelMessages, chatInfo);
        _lastReceiveWorldMessageTime = chatInfo.TimeTicks;
        _worldChannelMessages.Add(chatInfo);
        _receiveInformationAction?.Invoke();
        _takePushDataAction?.Invoke(chatInfo);
        _chatFtameTakePushDataAction?.Invoke(chatInfo, false);
    }
    /// <summary>
    /// 确保消息长度
    /// </summary>
    /// <param name="chat"></param>
    /// <param name="length"></param>
    private static void GuaranteedWorldMessageLength(ref List<ChatInfo> datas, int length)
    {
        while (datas.Count > length)
        {
            datas.RemoveAt(0);
        }
    }
    /// <summary>
    /// 更新数据
    /// </summary>
    /// <param name="datas"></param>
    /// <param name="data"></param>
    public static void UpdateData(ref List<ChatInfo> datas, ChatInfo data)
    {
        for (int i = 0; i < datas.Count; i++)
        {
            if (datas[i]._playUid == data._playUid)
            {
                datas[i].RefreshData(data);
            }
        }
    }
    /// <summary>
    /// 获取世界聊天消息 存储的最早的一条
    /// </summary>
    /// <returns></returns>
    public static ChatInfo GetWorldChannelMessage()
    {
        ChatInfo worldChat = null;
        if (_worldChannelMessages.Count > 0)
            worldChat = _worldChannelMessages[0];
        return worldChat;
    }
    /// <summary>
    /// 根据下标获取世界聊天列表中的数据
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static ChatInfo GetWorldData(int index)
    {
        if (index < _worldChannelMessages.Count)
        {
            return _worldChannelMessages[index];
        }
        else
        {
            return _worldChannelMessages[_worldChannelMessages.Count - 1];
        }

    }
    /// <summary>
    /// 根据下标获取世界聊天列表中的数据
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static ChatInfo GetRoomData(int index)
    {
        if (index < _roomChats.Count)
        {
            return _roomChats[index];
        }
        else
        {
            return _roomChats[_roomChats.Count - 1];
        }

    }
    /// <summary>
    /// 获取世界聊天数据数量
    /// </summary>
    /// <returns></returns>
    public static int GetWorkdNumber()
    {
        return _worldChannelMessages.Count;
    }
    /// <summary>
    /// 获取世界聊天数据数量
    /// </summary>
    /// <returns></returns>
    public static int GetRoomNumber()
    {
        return _roomChats.Count;
    }
    /// <summary>
    /// 获取对应私聊消息数据量
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public static int GetPrivateChatNumber(long uid)
    {
        PrivateChatSaveInfo privateChatSaveInfo = FindMessagePrivateChat_UID(uid);
        if (privateChatSaveInfo != null)
        {
            return privateChatSaveInfo.ChatInfos.Count;
        }
        else
        {
            return 0;
        }
    }
    /// <summary>
    /// 根据下标获取世界聊天列表中的数据
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static ChatInfo GetPrivateChatData(long uid, int index)
    {
        ChatInfo chatInfo = new ChatInfo();
        PrivateChatSaveInfo privateChatSaveInfo = FindMessagePrivateChat_UID(uid);
        if (privateChatSaveInfo != null && index < privateChatSaveInfo.ChatInfos.Count)
        {
            chatInfo = privateChatSaveInfo.ChatInfos[index];
        }
        return chatInfo;
    }
    /// <summary>
    /// 接收私聊消息
    /// </summary>
    public static void ReceivePrivateChatMessage(long playUid, ChatInfo chatInfo)
    {
        long lastTime = 0;

        var privateChat = _privateChannelMessages.Find((priChat) =>
        {
            if (priChat.PrivateChatRoleUid == playUid)
                return true;
            return false;
        });

        //1.判断私聊是否建立过
        if (privateChat != null)
        {
            //获取上次消息时间
            if (privateChat.ChatInfos.Count > 0)
            {
                lastTime = privateChat.ChatInfos[privateChat.ChatInfos.Count - 1].TimeTicks;
            }
            //间隔时间大于300s
            if (chatInfo.TimeTicks - lastTime >= StaticData.configExcel.GetVertical().ChatMessageInterval)
            {
                ChatInfo timeChatInfo = new ChatInfo(true);
                privateChat.ChatInfos.Add(timeChatInfo);
            }
            //更新时间
            privateChat.LastTimeTicks = chatInfo.TimeTicks;
            //更新是否阅读
            //UpdatePrivateChatIsRead(ref privateChat);
            UpdatePrivateChatIsRead(privateChat.PrivateChatRoleUid, false);

            privateChat.ChatInfos.Add(chatInfo);

            privateChat.RefreshData(chatInfo);

            if ((privateChat.iconId == 0 && chatInfo._playUid == playUid) || (chatInfo._playUid == playUid && privateChat.iconId != chatInfo._iconId))
            {
                privateChat.iconId = chatInfo._iconId;
            }
            //判断消息数量是否大于 20
            GuaranteedPrivateMessageLength(ref privateChat, StaticData.configExcel.GetVertical().PrivateChatDataNumber);
            _privateChannelMessages.Remove(privateChat);
            _privateChannelMessages.Insert(0, privateChat);
            _receivePrivateInformationAction?.Invoke(privateChat.PrivateChatRoleUid);
        }
        else //没有建立过私聊
        {
            PrivateChatSaveInfo priChat = new PrivateChatSaveInfo();
            priChat.PrivateChatRoleUid = playUid;
            priChat.RefreshData(chatInfo);
            UpdatePrivateChatIsRead(priChat.PrivateChatRoleUid, false);
            priChat.ChatInfos = new List<ChatInfo>();
            priChat.ChatInfos.Add(chatInfo);

            _privateChannelMessages.Insert(0, priChat);
            //_privateChannelMessages.Add(priChat);
            //判断建立聊天人数是否大于规定的上线 
            GuaranteedPrivateChatNumber(StaticData.configExcel.GetVertical().ChatPrivateRoleMaxNumber);
            _receivePrivateInformationAction?.Invoke(priChat.PrivateChatRoleUid);
        }

        //接收消息就存储
        PrivateChatSaveFile();
        _chatFtameTakePushDataAction?.Invoke(chatInfo, true);
    }
    /// <summary>
    /// 插入一条空白私聊信息
    /// </summary>
    /// <param name="playUid"></param>
    public static void CreationNullPrivateChatMessage(long playUid, int iconId, string iconUrl, string playName, int experience)
    {
        var privateChat = _privateChannelMessages.Find((data) =>
        {
            if (data.PrivateChatRoleUid == playUid)
                return true;
            return false;
        });
        if (privateChat == null)
        {
            PrivateChatSaveInfo priChat = new PrivateChatSaveInfo();
            priChat.PrivateChatRoleUid = playUid;
            priChat.iconId = iconId;
            priChat._iconUrl = iconUrl;
            priChat._playName = playName;
            priChat._experience = experience;
            priChat.LastTimeTicks = TimeHelper.ClientNowSeconds();
            priChat.ChatInfos = new List<ChatInfo>();
            _privateChannelMessages.Insert(0, priChat);
        }
        else
        {
            privateChat._playName = playName;
            privateChat.iconId = iconId;
            privateChat._iconUrl = iconUrl;
            privateChat._experience = experience;
            _privateChannelMessages.Remove(privateChat);
            _privateChannelMessages.Insert(0, privateChat);
        }
    }
    /// <summary>
    /// 清理空白的私聊信息
    /// </summary>
    public static void ClearBlankPrivateChat()
    {
        for (int i = 0; i < _privateChannelMessages.Count; i++)
        {
            PrivateChatSaveInfo data = _privateChannelMessages[i];
            if (data.ChatInfos == null || data.ChatInfos.Count <= 0)
            {
                _privateChannelMessages.Remove(data);
            }
        }
    }
    /// <summary>
    /// 更新聊天消息是否已经阅读
    /// </summary>
    /// <param name="priChat"></param>
    private static void UpdatePrivateChatIsRead(ref PrivateChatSaveInfo priChat)
    {
        if (priChat.PrivateChatRoleUid == StaticData.Uid)
        {
            priChat.IsRead = true;
        }
        else
        {
            priChat.IsRead = false;
        }
    }
    /// <summary>
    /// 更新聊天消息是否已经阅读
    /// </summary>
    /// <param name="priChat"></param>
    public static void UpdatePrivateChatIsRead(long uid, bool isread)
    {
        var privateChat = _privateChannelMessages.Find((data) =>
        {
            if (data.PrivateChatRoleUid == uid)
                return true;
            return false;
        });
        if (privateChat != null)
        {
            if (ChatPanelController._Instance != null && ChatPanelController._Instance._CurrType == 1 && ChatPanelController._Instance._CurrPrivateChatIndex == uid)
            {
                return;
            }
            privateChat.IsRead = isread;
        }
    }
    /// <summary>
    /// 确保私聊人数长度
    /// </summary>
    private static void GuaranteedPrivateChatNumber(int length)
    {
        //排序根据时间排序 最新的在最后
        _privateChannelMessages.Sort((chat1, chat2) =>
        {
            return (chat1.LastTimeTicks > chat2.LastTimeTicks) ? 1 : -1;
        });

        while (_privateChannelMessages.Count > length)
        {
            _privateChannelMessages.RemoveAt(0);
        }
    }
    /// <summary>
    /// 确保私聊消息长度
    /// </summary>
    /// <param name="chat"></param>
    /// <param name="length"></param>
    private static void GuaranteedPrivateMessageLength(ref PrivateChatSaveInfo chat, int length)
    {
        while (chat.ChatInfos.Count > length)
        {
            chat.ChatInfos.RemoveAt(0);
        }
    }
    /// <summary>
    /// 私聊信息存储
    /// </summary>
    /// <param name="fileName"></param>
    public static void PrivateChatSaveFile()
    {
        //需要保存的文件名称
        string path = UniversalTool.GetSaveFilePath("PrivateChatMessages.json");

        string jisonSre = JsonMapper.ToJson(_privateChannelMessages);
        UniversalTool.SaveJson(path, jisonSre);

        Debug.Log("接收消息就存储 存储地址：" + path);
    }
    /// <summary>
    /// 加载私聊文件
    /// </summary>
    public static void LoadPrivateChatFile()
    {
        string path = UniversalTool.GetSaveFilePath("PrivateChatMessages.json", false);
        if (String.IsNullOrEmpty(path))
            return;

        string json = UniversalTool.LoadJson(path);

        if (!String.IsNullOrEmpty(json))
        {
            _privateChannelMessages = null;
            _privateChannelMessages = JsonMapper.ToObject<List<PrivateChatSaveInfo>>(json);
        }
    }
    /// <summary>
    /// 是否有新的聊天消息 私聊
    /// </summary>
    public static bool IsNewMessagePrivateChat()
    {
        if (_privateChannelMessages == null)
            return false;
        foreach (var item in _privateChannelMessages)
        {
            if (!item.IsRead)
                return true;
        }
        return false;
    }
    /// <summary>
    /// 查找消息 私聊 根据uid获取
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public static PrivateChatSaveInfo FindMessagePrivateChat_UID(long uid)
    {
        if (_privateChannelMessages == null)
            return null;

        return _privateChannelMessages.Find((mes) =>
        {
            if (mes.PrivateChatRoleUid == uid)
                return true;
            return false;
        });
    }
    /// <summary>
    /// 阅读消息 私聊 根据uid
    /// </summary>
    /// <param name="uid"></param>
    public static void ReadMessagePrivateChat_UID(int uid)
    {
        var chat = _privateChannelMessages.Find((mes) =>
        {
            if (mes.PrivateChatRoleUid == uid)
                return true;
            return false;
        });
        chat.IsRead = true;
    }


    /// <summary>
    /// 获取聊天信息
    /// </summary>
    /// <returns></returns>
    public static SCChat GetChatMessage()
    {
        //return _chatMessages.Peek();
        //if (_chatMessages.Count > 0)
        //    return _chatMessages.Dequeue();
        return null;
    }
    /// <summary>
    /// 获取第一条聊天消息
    /// </summary>
    /// <returns></returns>
    public static string GetChatPeekMessage()
    {
        string mess = null;
        SCChat message = null;// _chatMessages.Peek();
        mess = message.Message;
        if (mess.Length > 8)
        {
            mess = mess.Substring(0, 8) + "...";
        }
        return mess;
    }
    /// <summary>
    /// 聊天内容是否为空
    /// </summary>
    /// <returns></returns>
    public static bool ChatMessagesIsNull()
    {
        return true;// (_chatMessages.Count <= 0) ? true : false;
    }
    /// <summary>
    /// 插入聊天消息队列
    /// </summary>
    /// <param name="mes"></param>
    public static void EnqueuetChatMessage(SCChat mes)
    {
        //_chatMessages.Enqueue(mes);
        EnqueuetMiniChatMessage(mes);
        MessageReceived?.Invoke();
    }
    /// <summary>
    /// 接受到消息
    /// </summary>
    public static Action MessageReceived;
    /// <summary>
    /// mini聊天插入消息
    /// </summary>
    /// <param name="mes"></param>
    public static void EnqueuetMiniChatMessage(SCChat mes)
    {
        //_miniChatMessages.Enqueue(mes);
    }
    /// <summary>
    /// Mini聊天内容是否为空
    /// </summary>
    /// <returns></returns>
    public static bool MiniChatMessagesIsNull()
    {
        return true;//(_miniChatMessages.Count <= 0) ? true : false;
    }
    /// <summary>
    /// 获取mini聊天第一条消息
    /// </summary>
    /// <returns></returns>
    public static string GetMiniChatPeekMessage()
    {
        string mess = null;

        //if (_miniChatMessages.Count > 0) 
        //{
        //    SCChat message = _miniChatMessages.Dequeue();//120139
        //    mess = message.Name + LocalizationDefineHelper.GetStringNameById(120139) + message.Message;
        //    if (mess.Length > 8)
        //    {
        //        mess = mess.Substring(0, 8) + "...";
        //    }
        //}
        return mess;
    }
    /// <summary>
    /// 注册接受信息回调
    /// </summary>
    /// <param name="receiveInformationAction"></param>
    /// <param name="receivePrivateInformationAction"></param>
    public static void EnrollAction(Action receiveInformationAction, Action receiveRoomInformationAction, Action<long> receivePrivateInformationAction)
    {
        _receiveInformationAction = receiveInformationAction;
        _receiveRoomInformationAction = receiveRoomInformationAction;
        _receivePrivateInformationAction = receivePrivateInformationAction;
    }
    /// <summary>
    /// 清除聊天面板接收数据回调
    /// </summary>
    public static void ClearAction()
    {

        _receiveInformationAction = null;
        _receiveRoomInformationAction = null;
        _receivePrivateInformationAction = null;
    }
    /// <summary>
    /// 接受消息回调
    /// </summary>
    /// <param name="action"></param>
    public static void EnrollTakePushDataAction(Action<ChatInfo> action)
    {
        _takePushDataAction = action;
    }
    /// <summary>
    /// 清楚弹幕接收消息回调
    /// </summary>
    public static void ClearTakePushDataAction()
    {
        _takePushDataAction = null;
    }
    /// <summary>
    /// 注册聊天小按钮监听
    /// </summary>
    /// <param name="action"></param>
    public static void EnrollChatMiniAction(Action action)
    {
        _chatMiniAction = action;
    }
    /// <summary>
    /// 清理聊天小按钮回调
    /// </summary>
    public static void ClearChatMiniAction()
    {
        _chatMiniAction = null;
    }
    /// <summary>
    /// 注册聊天框监听
    /// </summary>
    /// <param name="action"></param>
    public static void EnrollChatFrameAction(Action<ChatInfo, bool> action)
    {
        _chatFtameTakePushDataAction = action;
    }
    /// <summary>
    /// 清理聊天框回调
    /// </summary>
    public static void ClearChatFrameAction()
    {
        _chatFtameTakePushDataAction = null;
    }
    /// <summary>
    /// 根据头像url 获取头像图片资源
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static async System.Threading.Tasks.Task<Sprite> GetIcon(string url)
    {
        Sprite icon = null;
        icon = await GetTexture(url);
        return icon;
    }
    /// <summary>
    /// 根据头像id 获取头像图片资源
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static Sprite GetIcon(int iconId)
    {
        Sprite icon = null;

        PlayerAvatarDefine playerAvatarDefine = StaticData.configExcel.GetPlayerAvatarByID(iconId);

        if (playerAvatarDefine != null)
        {
            string path = StaticData.configExcel.GetPlayerAvatarByID(iconId).Icon;
            try
            {
                icon = ABManager.GetAsset<Sprite>(path);
            }
            catch (Exception er)
            {
                Debug.Log("找不到头像：" + path);
            }
        }

        return icon;
    }

    /// <summary>
    /// 请求图片
    /// </summary>
    /// <param name="url">图片地址,</param>
    static async System.Threading.Tasks.Task<Sprite> GetTexture(string url)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        DownloadHandlerTexture downloadTexture = new DownloadHandlerTexture(true);
        uwr.downloadHandler = downloadTexture;
        await uwr.SendWebRequest();
        Sprite icon = null;
        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.Log(uwr.error);
        }
        else
        {
            Texture2D t = downloadTexture.texture;
            icon = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.one);
        }
        return icon;
    }

    /// <summary>
    /// 获取玩家数据
    /// </summary>
    /// <returns></returns>
    public static ChatInfo GetPlayData(string message)
    {
        ChatInfo chatInfo = new ChatInfo();
        chatInfo._experience = StaticData.playerInfoData.userInfo.Experience;
        chatInfo._iconId = StaticData.playerInfoData.userInfo.Image;
        chatInfo._playName = StaticData.playerInfoData.userInfo.Name;
        chatInfo._playUid = StaticData.Uid;
        chatInfo._message = message;
        chatInfo.InitTime();
        return chatInfo;
    }
    /// <summary>
    /// 获取私聊玩家名字
    /// </summary>
    public static string GetPrivateChatName(PrivateChatSaveInfo data)
    {
        string playName = data._playName;
        for (int i = 0; i < data.ChatInfos.Count; i++)
        {
            ChatInfo ChatInfodata = data.ChatInfos[i];
            if (ChatInfodata._playUid == data.PrivateChatRoleUid)
            {
                playName = ChatInfodata._playName;
            }
        }
        return playName;
    }
    /// <summary>
    /// 获取私聊玩家头像id
    /// </summary>
    public static int GetPrivateChatIconId(PrivateChatSaveInfo data)
    {
        int iconId = data.iconId;
        for (int i = 0; i < data.ChatInfos.Count; i++)
        {
            ChatInfo ChatInfodata = data.ChatInfos[i];
            if (ChatInfodata._playUid == data.PrivateChatRoleUid)
            {
                iconId = ChatInfodata._iconId;
            }
        }
        return iconId;
    }
    /// <summary>
    /// 获取私聊玩家经验
    /// </summary>
    public static int GetPrivateChatExperience(PrivateChatSaveInfo data)
    {
        int experience = data._experience;
        for (int i = 0; i < data.ChatInfos.Count; i++)
        {
            ChatInfo ChatInfodata = data.ChatInfos[i];
            if (ChatInfodata._playUid == data.PrivateChatRoleUid)
            {
                experience = (int)ChatInfodata._experience;
            }
        }
        return experience;
    }
    /// <summary>
    /// 判断是否是玩家
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public static bool IsPlay(long uid)
    {
        return StaticData.Uid == uid;
    }
    /// <summary>
    /// 设置弹幕开关
    /// </summary>
    /// <param name="isOpen"></param>
    public static void SetBulletScreenOnOff(bool isOpen)
    {
        int saveValue = 0;
        if (isOpen)
        {
            saveValue = 0;
        }
        else
        {
            saveValue = 1;
        }
        PlayerPrefs.SetInt(BULLET_SCREEN_ON_OFF_SAVE, saveValue);
        OpenBulletScreenOnOff(isOpen);
    }
    /// <summary>
    /// 获取弹幕开关
    /// </summary>
    /// <returns></returns>
    public static bool GetBulletScreenOnOff()
    {
        bool isOpen = true;
        int saveValue = PlayerPrefs.GetInt(BULLET_SCREEN_ON_OFF_SAVE, 0);
        if (saveValue == 0)
        {
            isOpen = true;
        }
        else
        {
            isOpen = false;
        }
        return isOpen;
    }
    /// <summary>
    /// 开关聊天弹幕
    /// </summary>
    /// <param name="isOpne"></param>
    public static async void OpenBulletScreenOnOff(bool isOpne)
    {
        if (_chatBulletScreenController == null)
        {
            GameObject obj = await UIComponent.CreateUIAsync(UIType.ChatBulletScreen);

            _chatBulletScreenController = obj.GetComponent<ChatBulletScreenController>();
        }
        _chatBulletScreenController.OpenShow(isOpne);
        //if (isOpne)
        //{

        //    await UIComponent.CreateUIAsync(UIType.ChatBulletScreen);
        //}
        //else
        //{
        //    UIComponent.HideUI(UIType.ChatBulletScreen);
        //}

    }
    /// <summary>
    /// 进入房间
    /// </summary>
    public static void EnterRoom()
    {
        _isBeRoom = true;
    }
    /// <summary>
    /// 退出房间
    /// </summary>
    public static void QuitRoom()
    {
        _isBeRoom = false;
        _roomChats.Clear();
    }
    /// <summary>
    /// 物体抖动
    /// </summary>
    /// <param name="tageTra">目标物体</param>
    /// <param name="shakeValue">抖动值</param>
    /// <param name="number">抖动次数</param>
    /// <param name="endAction">结束回调</param>
    /// <returns></returns>
    public static IEnumerator BtnShake(Transform tageTra, Vector3 shakeValue, int number, float speed, Action endAction)
    {
        int index = number;
        Vector3 oldVector = tageTra.localPosition;
        float speedValue = speed;
        int glintIndex = 0;
        if (tageTra == null)
        {
            yield return null;
        }
        float minX = tageTra.localPosition.x - shakeValue.x;
        float maxX = tageTra.localPosition.x + shakeValue.x;
        float miny = tageTra.localPosition.y - shakeValue.y;
        float maxy = tageTra.localPosition.y + shakeValue.y;


        while (tageTra != null)
        {


            Vector3 newVector = new Vector3(minX, tageTra.localPosition.y);

            if (Vector3.Distance(tageTra.localPosition, newVector) >= 1)
            {
                while (Vector3.Distance(tageTra.localPosition, newVector) >= 1)
                {
                    tageTra.localPosition = Vector3.MoveTowards(tageTra.localPosition, newVector, Time.deltaTime * speedValue);
                    yield return new WaitForSeconds(Time.deltaTime * 0.01f);
                }

            }
            else
            {
                Vector3 newVectorTwo = new Vector3(maxX, tageTra.localPosition.y);
                while (Vector3.Distance(tageTra.localPosition, newVectorTwo) >= 1f)
                {
                    tageTra.localPosition = Vector3.MoveTowards(tageTra.localPosition, newVectorTwo, Time.deltaTime * speedValue);
                    yield return new WaitForSeconds(Time.deltaTime * 0.01f);
                }
            }
            glintIndex++;
            speedValue = speed / glintIndex;
            yield return new WaitForSeconds(Time.deltaTime * 0.1f);

            if (index != 0)
            {
                if (glintIndex / 2 == index)
                {
                    endAction?.Invoke();
                    break;
                }
            }
        }
    }
    /// <summary>
    /// 文本换行
    /// </summary>
    /// <param name="str">文本内容</param>
    /// <param name="number">多少个字符换行</param>
    /// <returns></returns>
    public static string TextLineFeed(string str, int number)
    {
        char[] charArray = str.ToCharArray();
        string newStr = "";
        for (int i = 0; i < charArray.Length; i++)
        {
            newStr = newStr + charArray[i];
            if (i != 0 && i % number == 0)
            {
                newStr = newStr + "\n";
            }
        }
        return newStr;
    }
    /// <summary>
    /// 限制在目标范围内
    /// 2019/12/3 huangjiangdong
    /// </summary>
    /// <param name="tra"></param>
    public static bool AstrictScope(Transform tra, RectTransform restrict)
    {
        var pos = tra.GetComponent<RectTransform>();
        Vector3 locaV3 = tra.parent.InverseTransformPoint(restrict.position);
        float xMin = (locaV3.x - restrict.rect.width * 0.5f) + pos.rect.width * 0.5f;
        float xMax = (locaV3.x + restrict.rect.width * 0.5f) - pos.rect.width * 0.5f;

        float yMin = (locaV3.y - restrict.rect.height * 0.5f) + pos.rect.height * 0.5f;
        float yMax = (locaV3.y + restrict.rect.height * 0.5f) - pos.rect.height * 0.5f;

        float x = 0;
        float y = 0;

        bool isBottomOut = false;
        if (tra.localPosition.x < xMin || tra.localPosition.x > xMax)
        {
            if (tra.localPosition.x < xMin)
            {
                x = xMin;
            }
            if (tra.localPosition.x > xMax)
            {
                x = xMax;
            }
        }
        else
        {
            x = pos.localPosition.x;
        }

        if (tra.localPosition.y < yMin || tra.localPosition.y > yMax)
        {
            if (tra.localPosition.y < yMin)
            {
                y = yMin;
                isBottomOut = true;
            }
            if (tra.localPosition.y > yMax)
            {
                y = yMax;
                isBottomOut = false;
            }
        }
        else
        {
            y = pos.localPosition.y;
        }

        pos.localPosition = new Vector2(x, y);
        return isBottomOut;
    }
    /// <summary>
    /// 关闭聊天相关面板
    /// </summary>
    public static void CloseChat()
    {
        UIComponent.HideUI(UIType.ChatPanel);
        UIComponent.HideUI(UIType.ChatRoleInformationPanel);
        UIComponent.HideUI(UIType.ChatMini);
        UIComponent.HideUI(UIType.ChatBulletScreen);
    }


    #endregion
}
