using BestHTTP.WebSocket;
using Company.Cfg;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Protocal
{
    public class WebSocketComponent : MonoBehaviour
    {

        //队列用来缓存消息，收到一条消息从队列中移除，下次再来消息，判定队列中有没有，没有了的直接丢掉，避免服务器重连的时候服务器返回多条数据
        public class RequestItem
        {
            public byte[] msgData;//包装好的数据
            public OpCodeType opCode;//数据
            public Action<IMessage> onSuccess;
            public Action<ErrorInfo> onFail;
            public bool isResponsed;
        }
        public static WebSocketComponent _instance;
        private void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        public enum ConnectState
        {
            /// <summary>
            /// 没有连接
            /// </summary>
            UnConnect,
            /// <summary>
            /// 已经连接
            /// </summary>
            Connected,
            /// <summary>
            /// 重新连接
            /// </summary>
            ReConnecting
        }
        /// <summary>
        /// 网络没有到达状态
        /// </summary>
        public enum NetNotReachState
        {
            /// <summary>
            /// 完成
            /// </summary>
            Compelete,
            /// <summary>
            /// 开始
            /// </summary>
            Begin,
            /// <summary>
            /// 进行中
            /// </summary>
            Processing,
        }


        private WebSocket webSocket;
        /// <summary>
        /// 地址
        /// </summary>
        private string address;
        /// <summary>
        /// 数据队列
        /// </summary>
        private Queue<RequestItem> QueueMsgBuiness = new Queue<RequestItem>();


        /// <summary>
        /// 是否已经向服务器发送请求 并且没有回调
        /// </summary>
        private bool isSendRequest;

        /// <summary>
        /// 当前重试次数
        /// </summary>
        private int currRetryTimes;
        /// <summary>
        /// 是否开始重试
        /// </summary>
        private bool isBeginRetry;

        /// <summary>
        /// 当前web连接状态
        /// </summary>
        private ConnectState webCurrConnectState;

        /// <summary>
        /// 网络异常提示 界面是否打开
        /// </summary>
        private bool _isOpenNetworkAnomalyTips = false;

        #region 心跳

        /// <summary>
        /// 是否需要发送心跳
        /// </summary>
        private bool _isNeedSendHeartBeat = false;

        /// <summary>
        /// 心跳计时
        /// </summary>
        private float _heartBeatTimer = 0.0f;

        /// <summary>
        /// 是否自动断开连接 玩家主动断开连接 1.切换账号 2.账号被挤掉 3.注销账号
        /// </summary>
        private bool _isAutoDisconnect = false;

        #endregion

        private void InitData() 
        {
            webCurrConnectState = ConnectState.UnConnect;

            isBeginRetry = false;
            currRetryTimes = 0;

            isSendRequest = false;

            _isNeedSendHeartBeat = false;
            _heartBeatTimer = 0f;

            _isOpenNetworkAnomalyTips = false;
            _isAutoDisconnect = false;
        }

        public void Init()
        {
            QueueMsgBuiness.Clear();
            
            InitData();
            //建立起长连接
            Connect();
        }
        string GetURL()
        {
            return string.Format("ws://{0}:{1}/socket.io/?EIO=4&transport=websocket",
                                 StaticData.ipWebSocket,
                                 StaticData.portWebSocket);
        }
        public void Connect()
        {
            CheckIpSetting();
            address = GetURL();
            StaticData.DebugGreen($"WebSocket address:{address}");
            webSocket = new WebSocket(new Uri(address));
            webSocket.StartPingThread = false;

            webSocket.OnOpen += OnOpen;
            webSocket.OnBinary += OnMessageReceivedBinary;
            webSocket.OnClosed += OnClosed;
            webSocket.OnError += OnError;
            //打开套接字，进行监听
            webSocket.Open();
        }
        void CloseConnect()
        {
            if (webSocket != null)
            {
                webSocket.OnOpen = null;
                webSocket.OnMessage = null;
                webSocket.OnBinary = null;
                webSocket.OnError = null;
                webSocket.OnClosed = null;
                webSocket?.Close();
                webSocket = null;
            }
        }

        void CheckIpSetting()
        {
            if (string.IsNullOrEmpty(StaticData.ipWebSocket))
                throw new Exception("StaticData.ipWebSocket is null, set 'StaticData.ipWebSocket' first");

            if (StaticData.portWebSocket <= 0)
                throw new Exception("StaticData.portWebSocket is null, set 'StaticData.portWebSocket' first");
        }

        //发送消息，先放进队列中
        public void SendMsg(IMessage msg, OpCodeType opCodeType, Action<IMessage> OnSuccess, Action<ErrorInfo> OnFail)//登录的时候记得获取uid
        {
            byte[] byteData = ProtoSerAndUnSer.Serialize(msg);
            CSServerReq serverReq = new CSServerReq() { Data = ByteString.AttachBytes(byteData), OpCode = opCodeType, Uid = StaticData.Uid };
            //序列化serverReq
            byte[] finalData = ProtoSerAndUnSer.Serialize(serverReq);
            StaticData.DebugGreen($"将要发送消息的消息入队:{msg.ToString()}");
            RequestItem item = new RequestItem()
            {
                msgData = finalData,
                opCode = opCodeType,
                onSuccess = OnSuccess,
                onFail = OnFail,
                isResponsed = false
            };
            QueueMsgBuiness.Enqueue(item);
        }

        /// <summary>
        /// 查询消息是否入队
        /// </summary>
        /// <returns></returns>
        public bool FindCodeMsg(OpCodeType opCodeType) 
        {
            foreach (var item in QueueMsgBuiness) 
            {
                if (item.opCode == opCodeType)
                    return true;
            }
            return false;
        }


        /// <summary>
        /// 向服务器 发送请求
        /// </summary>
        private void SendRequest()
        {
            _heartBeatTimer = 0;
            isSendRequest = true;

            RequestItem msgItem = QueueMsgBuiness.Peek();
            StaticData.DebugGreen($"发送消息 opCode :{msgItem.opCode.ToString()}");
            byte[] data = msgItem.msgData;
            webSocket.Send(data);
        }

        //收到消息处理
        /// <summary>
        /// Called when we received a Binary message from the server
        /// </summary>
        void OnMessageReceivedBinary(WebSocket ws, byte[] data)
        {
            if (ws != this.webSocket)
                return;
            if (data == null || data.Length <= 0)
                return;
            ProcessUnSeriMsgs(data);
        }
        public async void Update()
        {
            
            //连接不为空进行心跳操作
            if (webSocket != null)
                SatrtHeartBeat();

            //数据发送
            if (webCurrConnectState == ConnectState.Connected && QueueMsgBuiness.Count >= 1 && !isSendRequest)//只有一条，直接发送
            {
                SendRequest();
            }

        }

        public void ClearWebSocket()
        {
            webCurrConnectState = ConnectState.UnConnect;
            QueueMsgBuiness.Clear();
        }

        public void Dispose()
        {
            webSocket?.Close();
            webSocket = null;
            webCurrConnectState = ConnectState.UnConnect;
            QueueMsgBuiness.Clear();
        }

        #region WebSocket Event Handlers

        /// <summary>
        /// Called when the web socket is open, and we are ready to send and receive data
        /// </summary>
        void OnOpen(WebSocket ws)
        {
            StaticData.DebugGreen($"WebSocket OnOpen:{address}");
            //WaitManager.EndRotate();
            webCurrConnectState = ConnectState.Connected;
            //
            isBeginRetry = false;
            currRetryTimes = 0;
            //重置变量，为了可以重新发送消息
            isSendRequest = false;
            _isNeedSendHeartBeat = true;
            _heartBeatTimer = StaticData.configExcel.GetVertical().HeartBeatTime;
        }



        private void ProcessUnSeriMsgs(byte[] data)
        {

            var unSeriMsgs = ProtoSerAndUnSer.UnSerialize<SCServerRes>(data);
            OpCodeType opCode = unSeriMsgs.RtCode;
            StaticData.DebugGreen($"WebSocket Receive opCode:{opCode.ToString()}");
            IMessage msg = ListOPRelation.GetRealMessageByOpCodeType(opCode, unSeriMsgs.Data.ToByteArray());

            Debug.Log("ProcessUnSeriMsgs opCode = "+ (int)opCode);
            Debug.Log("unSeriMsgs.Code = " + unSeriMsgs.Code);
            int code = (int)opCode;
            if (code >= 50000)
            {
                //账号被挤掉了
                switch (code) 
                {
                    case 50020://账户被挤掉
                        PushSystemAccountWasSqueezed();
                        break;
                    case 60000://服务器强制更新
                        ServerForcedUpdate();
                        break;
                    default://服务器推送
                        RigisterCmdPush.PushMsgHandle(msg, (int)opCode);
                        break;
                } 
            }
            else
            {
                //判定队列中是否有，如果没有，直接丢弃
                if (QueueMsgBuiness.Count > 0)
                {
                    if (QueueMsgBuiness.Peek().opCode != opCode)
                    {
                        return;
                    }
                }

                if (QueueMsgBuiness.Count <= 0)//队列中没有东西，但是收到了返回
                {//重置
                    SetOnceMessageCompelete();
                    return;
                }
                
                //出队
                if (msg != null)
                {
                    StaticData.DebugGreen($"WebSocket msg:{msg.ToString()} Dequeue");
                }
                else
                {
                    StaticData.DebugGreen($"==========消息返回：{QueueMsgBuiness.Peek().opCode}为null!");
                }

                var msgItem = QueueMsgBuiness.Dequeue();
                SetOnceMessageCompelete();

                //正常请求
                if (unSeriMsgs.Code != (int)WebErrorCode.Success)
                {
                    ErrorInfo error = new ErrorInfo() { webErrorCode = (WebErrorCode)unSeriMsgs.Code, ErrorMessage = $"服务器异常{unSeriMsgs.Code}" };
                    msgItem.onFail(error);
                }
                else
                {
                    msgItem.onSuccess((msg));
                }
                
            }
        }

        /// <summary>
        /// 设置一次消息请求回调完成
        /// </summary>
        private void SetOnceMessageCompelete()
        {
            isSendRequest = false;
            WaitManager.EndRotate();
        }

        /// <summary>
        /// Called when the web socket closed
        /// 不需要重新连接 通知服务器异常
        /// 1.服务器主动关闭连接 被调用
        /// </summary>
        void OnClosed(WebSocket ws, UInt16 code, string message)
        {
            StaticData.DebugGreen($"WebSocket OnClosed message:{message}");

            //不做任何处理，不然可能影响到断线重连
            StartReconnect( true );
        }

        /// <summary>
        /// Called when an error occured on client side  
        /// 需要进行重新连接
        /// 本机网络终断 会被调用
        /// 服务器宕机 会被调用 //远程主机强迫关闭了一个现有的连接。
        /// 由于目标计算机积极拒绝，无法连接。（没有连接上服务器）
        /// </summary>
        async void OnError(WebSocket ws, Exception ex)
        {
            if (ex != null)
            {
                StaticData.DebugGreen($"WebSocket OnError Exception:{ex.ToString()}");
            }
            //主动断开不在重连
            if (_isAutoDisconnect)
                return;

            Debug.Log("OnError 即为网络有问题了，重试");
            //Error即为网络有问题了，重试
            StartReconnect();
        }

        #endregion

        #region 心跳

        /// <summary>
        /// 开始心跳
        /// </summary>
        private void SatrtHeartBeat() 
        {

            _heartBeatTimer += Time.deltaTime * 1000;

            //超过2个心跳时间
            if (_heartBeatTimer >= 2 * StaticData.configExcel.GetVertical().HeartBeatTime)
            {
                if (isBeginRetry)//重连等待时间 超过2个心跳时间
                {
                    Debug.Log("心跳超时，退回到登录页面！");
                    //退回到登录页面
                    QuitUILogin();
                    return;
                }
            }

            //一个心跳时间
            if (_heartBeatTimer >= StaticData.configExcel.GetVertical().HeartBeatTime)
            {
                //正在从新连接
                if (isBeginRetry)
                    return;

                //有消息没有回调
                if (isSendRequest) 
                {
                    //进入重连
                    StartReconnect();
                    return;
                }

                //没有连接 或者 已经发送心跳 或者 不需要发送心跳/不进行心跳发送判断 或者 请求进行中
                if (webCurrConnectState != ConnectState.Connected  || !_isNeedSendHeartBeat)
                    return;

                Debug.Log("向服务器发送心跳");
                _heartBeatTimer -= StaticData.configExcel.GetVertical().HeartBeatTime;
                NotifyServerEmptyHeartBeat();
            }
            
        }

        /// <summary>
        /// 通知服务器 发送心跳
        /// </summary>
        private void NotifyServerEmptyHeartBeat()
        {
            ProtocalManager.Instance().SendCSEmptyHeartBeat(new CSEmptyHeartBeat(), (SCEmtpyHeartBeat sCEmtpyHeartBeat) =>
            {
                Debug.Log("通知服务器 发送心跳成功！");
            },
            (ErrorInfo er) =>
            {
                //默认心跳不可能失败
                Debug.Log("通知服务器 发送心跳失败！Error：" + er.ErrorMessage);
            });
        }

        #endregion

        #region 自动重连

        /// <summary>
        /// 开始重新连接 //设置重新链接
        /// </summary>
        private async void StartReconnect( bool isServerClosed = false) 
        {
            _isNeedSendHeartBeat = false;

            //服务器关闭 直接退到 登录界面
            if (isServerClosed)
            {
                QuitUILogin();
                return;
            }

            isBeginRetry = true;

            //网络异常提示界面已经打开
            if (_isOpenNetworkAnomalyTips)
                return;

            //判断重连次数
            if (currRetryTimes >= StaticData.configExcel.GetVertical().ReconnectTimer) 
            {
                Debug.Log(string.Format("判断重连次数 currRetryTimes = {0}", currRetryTimes));
                //打开提示界面 提示网络异常
                OpenNetworkAnomalyTips();
                return;
            }

            currRetryTimes++;

            //打开等待界面
            WaitManager.BeginRotate();

            //是否有网络
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                //无网络
                //等待重连
                await UniTask.Delay(StaticData.configExcel.GetVertical().ReconnectDelayTime);
                StartReconnect();
                return;
            }

            //重连
            EnterReconnect();
        }

        /// <summary>
        /// 进入重新连接
        /// </summary>
        private async void EnterReconnect() 
        {
            webCurrConnectState = ConnectState.UnConnect;
            if (currRetryTimes > 1) 
            {
                //等待 
                await UniTask.Delay(StaticData.configExcel.GetVertical().ReconnectDelayTime);
            }

            webCurrConnectState = ConnectState.ReConnecting;
            //重连处理...
            CloseConnect();
            Connect();

            //等待 
            await UniTask.Delay(StaticData.configExcel.GetVertical().ReconnectDelayTime);
            //是否已经重新连接上 重连等待中
            if (isBeginRetry && webCurrConnectState != ConnectState.UnConnect && !_isOpenNetworkAnomalyTips)
            {
                //没有
                StartReconnect();
            }
                
        }


        /// <summary>
        /// 退出 到登录页面
        /// </summary>
        public void QuitUILogin(bool isAccountWasSqueezed = false) 
        {
            _isAutoDisconnect = true;
            CloseNetworkAnomalyTips();
            ClearWebSocket();
            CloseConnect();
            InitData();
            //
            WaitManager.EndRotate();
            StaticData.ReturnUILogin(isAccountWasSqueezed);
        }

        /// <summary>
        /// 打开网络异常提示
        /// </summary>
        private void OpenNetworkAnomalyTips() 
        {
            _isOpenNetworkAnomalyTips = true;
            //
            WaitManager.EndRotate();
            //打开异常提示界面
            Debug.Log("打开异常提示界面");
            //string desc = "网络开小差了，重新连接一下吧";
            string desc = LocalizationDefineHelper.GetStringNameById(120219);
            //120075 //取消
            StaticData.OpenCommonTips(desc, 120220, NetworkAnomalyTipsReconnect, NetworkAnomalyTipsQuitLogin, 120075);
        }

        private void CloseNetworkAnomalyTips() 
        {
            _isOpenNetworkAnomalyTips = false;
            //关闭异常提示界面
            Debug.Log("关闭异常提示界面");
            UIComponent.RemoveUI(UIType.UICommonPopupTips);
        }

        /// <summary>
        /// 网络异常提示界面回调 重新连接
        /// </summary>
        private void NetworkAnomalyTipsReconnect() 
        {
            _isOpenNetworkAnomalyTips = false;
            currRetryTimes = 0;
            StartReconnect();
        }

        /// <summary>
        /// 网络异常提示界面回调 返回登录页
        /// </summary>
        private void NetworkAnomalyTipsQuitLogin() 
        {
            _isOpenNetworkAnomalyTips = false;
            QuitUILogin();
        }
        #endregion

        #region 账号被被挤掉

        /// <summary>
        /// 推送账号被被挤掉
        ///</summary>
        private void PushSystemAccountWasSqueezed()
        {
            //退出 到登录页面
            QuitUILogin(true);
        }

        #endregion

        /// <summary>
        /// 服务器强制更新
        /// </summary>
        private void ServerForcedUpdate() 
        {
            _isAutoDisconnect = true;
            ResourcesHelper.InstantiatePrefabFromResourceSetDefault("UITipUpdateVersion", UIRoot.instance.GetUIRootCanvas().transform);
        }
    }
}
