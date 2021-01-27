using Company.Cfg;
using Cysharp.Threading.Tasks;
using Game.Protocal;
using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


/// <summary>
/// 登录类型
/// </summary>
public enum LoginType
{
    LoginDef,
    LoginWechat,
    LoginQQ
}

/// <summary>
/// 登录界面控制
/// </summary>

public class UILoginController : MonoBehaviour
{
    #region 变量

    private Button _butLogin;//游客登录按钮
    private Button _butLogin_WeChat;
    private Button _butLogin_QQ;

    //private Vector3 _butLogin_DefPos;
    private Vector3 _butLogin_WeChat_DefPos;
    private Vector3 _butLogin_QQ_DefPos;

    private GameObject _panelNick; //设置昵称
    private Button _butRandomNick;
    private Button _butComplete;

    private InputField _inputFieldNick;
    private Text _inputNickText;

    private LinkImageText _mtext;

    private Toggle _privacyToggle;

    private GameObject _privacyUI;
    private Button _butAgree;
    private Button _butDisagree;



    private Action _loginbCallback;

    /// <summary>
    /// 随机名称
    /// </summary>
    private string _randomNick = null;

    private string _inputNick = null;

    /// <summary>
    /// 昵称重复
    /// </summary>
    private bool _isDuplicateNick = false;

    //正则规范 //https://www.cnblogs.com/hehehehehe/p/6043710.html
    //中文、英文、数字但不包括下划线等符号
    private Regex _reg = new Regex(@"^[\u4E00-\u9FA5A-Za-z0-9]+$");

    private LoginType _loginType;

    /// <summary>
    /// 头像地址
    /// </summary>
    private string _iconPath = string.Empty;

    #region 服务器链接

    /// <summary>
    /// 链接的服务器类型/端口类型
    /// </summary>
    private TypeGateWay _typeGateWay;

    /// <summary>
    /// 是否开启请求
    /// </summary>
    private bool _isStartRequst = false;

    //
    private bool isOnPrivacy = false;

    #endregion


    #endregion

    #region 方法

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        var num = PlayerPrefs.GetInt("isAgreePrivacy");
        isOnPrivacy = (num == 1) ? true : false;
        SpawnAnimEffect();
    }

    private void Init() 
    {
        if (_butLogin != null)
            return;

        _butLogin = transform.Find("But_Login").GetComponent<Button>();
        _butLogin.onClick.RemoveAllListeners();
        _butLogin.onClick.AddListener(OnClickLogin);

        _butLogin_WeChat = transform.Find("But_Login_WeChat").GetComponent<Button>();
        _butLogin_WeChat.onClick.RemoveAllListeners();
        _butLogin_WeChat.onClick.AddListener(OnClickAuthorizedLoginWeixin);

        _butLogin_QQ = transform.Find("But_Login_QQ").GetComponent<Button>();
        _butLogin_QQ.onClick.RemoveAllListeners();
        _butLogin_QQ.onClick.AddListener(OnClickAuthorizedLoginQQ);

        //_butLogin_DefPos = _butLogin.transform.localPosition;
        _butLogin_WeChat_DefPos = _butLogin_WeChat.transform.localPosition;
        _butLogin_QQ_DefPos = _butLogin_QQ.transform.localPosition;

        _panelNick = transform.Find("Panel_Nick").gameObject;

        _inputFieldNick = transform.Find("Panel_Nick/InputNick").GetComponent<InputField>();
        _inputNickText = _inputFieldNick.transform.Find("Text").GetComponent<Text>();
        _inputFieldNick.onValueChanged.RemoveAllListeners();
        _inputFieldNick.onValueChanged.AddListener(InputOnValueChanged);

        _butRandomNick = transform.Find("Panel_Nick/ButRandomNick").GetComponent<Button>();
        _butRandomNick.onClick.RemoveAllListeners();
        _butRandomNick.onClick.AddListener(OnClickRandomNick);

        _butComplete = transform.Find("Panel_Nick/ButComplete").GetComponent<Button>();
        _butComplete.onClick.RemoveAllListeners();
        _butComplete.onClick.AddListener(OnClickComplete);
        _panelNick.SetActive(false);


        _mtext = transform.Find("Label").GetComponent<LinkImageText>();
        //_mtext.onHrefClick.RemoveAllListeners();
        _mtext.onHrefClick.AddListener((desc) => { OnClickOpenPrivacy(desc); });

        _privacyToggle = transform.Find("Toggle").GetComponent<Toggle>();
        _privacyToggle.onValueChanged.RemoveAllListeners();
        _privacyToggle.onValueChanged.AddListener(OnTogglePrivacy);

        //不再卸载
        var _rolePos = GameObject.Find("RolePos").transform;
        DontDestroyOnLoad(_rolePos.gameObject);

        //
        _privacyUI = transform.Find("UIUserPrivacy").gameObject;
        _privacyUI.SetActive(false);
        _butAgree = _privacyUI.transform.Find("BG/ButAgree").GetComponent<Button>();
        _butAgree.onClick.RemoveAllListeners();
        _butAgree.onClick.AddListener(OnClickPrivacyAgree);
        _butDisagree = _privacyUI.transform.Find("BG/ButDisagree").GetComponent<Button>();
        _butDisagree.onClick.RemoveAllListeners();
        _butDisagree.onClick.AddListener(OnClickPrivacyDisagree);

        
    }

    private void InitShowLoginBut() 
    {
        bool isShowQQ = Sdk.SdkFuc.IsQQInstall();
        bool isShowWeChat = Sdk.SdkFuc.IsWXInstal();
        if (isShowQQ && isShowWeChat)
        {
            _butLogin.gameObject.SetActive(false);
            _butLogin_WeChat.gameObject.SetActive(true);
            _butLogin_QQ.gameObject.SetActive(true);
        } else if (isShowQQ && !isShowWeChat)
        {
            _butLogin.gameObject.SetActive(true);
            _butLogin_WeChat.gameObject.SetActive(false);
            _butLogin_QQ.gameObject.SetActive(true);
            _butLogin.transform.localPosition = _butLogin_WeChat_DefPos;
        } else if (!isShowQQ && isShowWeChat) 
        {
            _butLogin.gameObject.SetActive(true);
            _butLogin_WeChat.gameObject.SetActive(true);
            _butLogin_QQ.gameObject.SetActive(false);
            _butLogin.transform.localPosition = _butLogin_QQ_DefPos;
        } else 
        {
            _butLogin.gameObject.SetActive(true);
            _butLogin_WeChat.gameObject.SetActive(false);
            _butLogin_QQ.gameObject.SetActive(false);
        }
    }

    private void SetLoginButInteractable(bool isTrue) 
    {
        _butLogin.interactable = isTrue;
        _butLogin_QQ.interactable = isTrue;
        _butLogin_WeChat.interactable = isTrue;
    }

    private void SetLoginButActive(bool isTrue) 
    {
        _butLogin.gameObject.SetActive(isTrue);
        _butLogin_QQ.gameObject.SetActive(isTrue);
        _butLogin_WeChat.gameObject.SetActive(isTrue);
    }

    public void InitValue(TypeGateWay typeGate, Action loginCallback, bool isAccountWasSqueezed = false) 
    {
        Init();
        InitShowLoginBut();
        SetLoginButInteractable(true);

        _typeGateWay = typeGate;
        _loginbCallback = loginCallback;

        if (isAccountWasSqueezed)
        {
            OpenAccountWasSqueezedTips();
        }
        else 
        {
            DetectionPrivacy();
        }

        //关闭聊天弹窗
        UIComponent.RemoveUI(UIType.ChatMini);
        UIComponent.RemoveUI(UIType.ChatPanel);
    }

    /// <summary>
    /// 打开用于隐私协议
    /// </summary>
    private void OnClickOpenPrivacy(string desc) 
    {
        //
        //Sdk.SdkFuc.IsAdReady
        Debug.Log("打开用于隐私协议 OnClickOpenPrivacy");
        OpenUIPrivacy();
    }

    /// <summary>
    /// 是否勾选隐私协议
    /// </summary>
    /// <param name="isOn"></param>
    private void OnTogglePrivacy(bool isOn)
    {
        isOnPrivacy = isOn;

        PlayerPrefs.SetInt("isAgreePrivacy", isOnPrivacy?1:0);
        Debug.Log("是否勾选隐私协议 isOn:"+ isOn);
    }

    private void OpenUIPrivacy() 
    {
        _privacyUI.SetActive(true);
    }

    private void CloseUIPrivacy()
    {
        _privacyUI.SetActive(false);
    }

    /// <summary>
    /// 同意
    /// </summary>
    private void OnClickPrivacyAgree()
    {
        isOnPrivacy = true;
        _privacyToggle.isOn = isOnPrivacy;
        CloseUIPrivacy();
    }

    /// <summary>
    /// 不同意
    /// </summary>
    private void OnClickPrivacyDisagree()
    {
        isOnPrivacy = false;
        _privacyToggle.isOn = isOnPrivacy;
        CloseUIPrivacy();
    }

    #region 登录

    /// <summary>
    /// 微信授权登录
    /// </summary>
    private void OnClickAuthorizedLoginWeixin() 
    {
        if (!DetectionPrivacy())
        {
            return;
        }

        _loginType = LoginType.LoginWechat;
        if (!NetworkDetection())
            return;
        Debug.Log("微信授权登录!!!");
        Sdk.SdkFuc.WXLogin(AuthorizedLoginWeixinCallback);
    }

    private void AuthorizedLoginWeixinCallback(string callbackInfo) 
    {
        if (string.IsNullOrEmpty(callbackInfo))
        {
            //第三方授权失败请重新登录//
            string desc = LocalizationDefineHelper.GetStringNameById(120314);
            //
            StaticData.CreateToastTips(desc);
            //
            return;
        }
        Debug.Log("微信授权登录回调!!!");
        //解析回调数据

        string unionID = string.Empty;
        _iconPath = string.Empty;
        RequestShortLink(unionID, RegisterPlatform.WeChat);
    }

    /// <summary>
    /// QQ授权登录
    /// </summary>
    private void OnClickAuthorizedLoginQQ()
    {
        if (!DetectionPrivacy())
        {
            return;
        }

        _loginType = LoginType.LoginQQ;
        if (!NetworkDetection())
            return;
        Sdk.SdkFuc.QQLogin(AuthorizedLoginQQCallback);
    }

    private void AuthorizedLoginQQCallback(string callbackInfo)
    {
        if (string.IsNullOrEmpty(callbackInfo))
        {
            //第三方授权失败请重新登录//
            string desc = LocalizationDefineHelper.GetStringNameById(120314);
            //
            StaticData.CreateToastTips(desc);
            //
            return;
        }

        //解析回调数据

        string unionID = string.Empty;
        _iconPath = string.Empty;
        RequestShortLink(unionID, RegisterPlatform.Qq);
    }


    /// <summary>
    /// 游客登录
    /// </summary>
    private void OnClickLogin() 
    {

        if (!DetectionPrivacy()) 
        {
            return;
        }

        _loginType = LoginType.LoginDef;
        if (!NetworkDetection())
            return;

        RequestShortLink();
    }

    private bool DetectionPrivacy() 
    {
        if (isOnPrivacy) 
        {
            _privacyToggle.isOn = isOnPrivacy;
            return true;
        }
        //打开 隐私协议弹窗
        OpenUIPrivacy();
        //
        //StaticData.CreateToastTips(LocalizationDefineHelper.GetStringNameById(120202));
        return false;
        
    }

    #endregion

    #region 设置昵称和选择头像

    /// <summary>
    /// 进入设置昵称和头像
    /// </summary>
    private void EnterSetNickAndIcon() 
    {
        Debug.Log("登录界面！ 进入设置昵称和头像！");
        _randomNick = null;
        //
        SetLoginButActive(false);
        _panelNick.SetActive(true);

        //数据打点
        StaticData.DataDot(DotEventId.CreateNickName);
    }


    /// <summary>
    /// 输入改变
    /// </summary>
    /// <param name="text"></param>
    private void InputOnValueChanged(string text)
    {
        Debug.Log("输入改变 :" + text);
        if (!string.IsNullOrEmpty(text) && (!_reg.IsMatch(text) || GetBytesOfString(text) > 12))////输入长度是否合格
        {
            _inputNickText.text = _inputNick;
            _inputFieldNick.text = _inputNick;
            return;
        }
        _inputNick = text;
    }

    /// <summary>
    /// 获取输入数据字节大小 返回大小
    /// </summary>
    /// <param name="Text"></param>
    /// <returns></returns>
    public int GetBytesOfString(string Text)
    {
        int nByte = 0;
        byte[] bytes = Encoding.Unicode.GetBytes(Text);
        
        for (int i = 0; i < bytes.GetLength(0); i++)
        {
            //Debug.Log("bytes = " + bytes[i]);
            //  偶数位置，如0、2、4等，为UCS2编码中两个字节的第一个字节
            if (i % 2 == 0)
            {
                nByte++;      //  在UCS2第一个字节时n加1
            }
            else
            {
                //  当UCS2编码的第二个字节大于0时，该UCS2字符为汉字，一个汉字算两个字节
                if (bytes[i] > 0)
                {
                    nByte++;
                }
            }
        }

        return nByte;
    }

    /// <summary>
    /// 随机昵称
    /// </summary>
    private void OnClickRandomNick() 
    {

        bool addUid = false;
        //if (string.IsNullOrEmpty(_randomNick))
        if (_isDuplicateNick)
            addUid = true;

        _randomNick = GetRandomNick(addUid);
        _inputNickText.text = _randomNick;
        _inputFieldNick.text = _randomNick;

        Debug.Log("随机昵称 _randomNick = "+ _randomNick);
    }

    /// <summary>
    /// 完成输入 创建昵称
    /// </summary>
    private void OnClickComplete() 
    {
        if (!DetectionPrivacy())
        {
            return;
        }

        //是否为空
        if (string.IsNullOrEmpty(_inputNick)) 
        {
            //昵称不能为空！
            StaticData.CreateToastTips(LocalizationDefineHelper.GetStringNameById(120201));
            return;
        }
        //
        if (_randomNick != _inputNick) 
        {
            bool isBlockFont = ShieldWordTool.BlockFont(ref _inputNick);
            //bool isBlockFont = false;
            //是否有屏蔽字符
            if (isBlockFont) 
            {
                //
                Debug.Log("昵称含有敏感字符！");
                StaticData.CreateToastTips(LocalizationDefineHelper.GetStringNameById(120202));
                _inputNick = string.Empty;
                _inputNickText.text = _inputNick;
                _inputFieldNick.text = _inputNick;
                return;
            }
        }
        //发起请求
        RequestNiceAndIconVerification();
    }

    /// <summary>
    /// 请求昵称和头像验证
    /// </summary>
    private void RequestNiceAndIconVerification() 
    {
        Debug.Log("请求昵称和头像验证 _inputNick = "+ _inputNick);
        StaticData.NotifyServerRequestNiceAndIconVerification(_inputNick, 0, _iconPath, VerificationCallback);
    }

    /// <summary>
    /// 验证回调
    /// </summary>
    /// <param name="isSuccess"></param>
    private void VerificationCallback(bool isSuccess) 
    {
        if (isSuccess)
        {
            //数据打点
            StaticData.DataDot(DotEventId.CreateNickSucc);

            StaticData.UpdateNickAndIcon(_inputNick, 0, _iconPath);
            LoginCompleteEnterManor();
        }
        else 
        {
            Debug.Log("昵称已经被使用！！！");
            _isDuplicateNick = true;
            _inputNick = string.Empty;
            _inputNickText.text = _inputNick;
            _inputFieldNick.text = _inputNick;
        }
    }


    /// <summary>
    /// 获取随机昵称
    /// </summary>
    /// <param name="addUid"></param>
    /// <returns></returns>
    private string GetRandomNick(bool addUid = false) 
    {
        int maxIndex = StaticData.configExcel.RandomName.Count;
        int firstNameIndex = UnityEngine.Random.Range(0, maxIndex);
        int secondName = UnityEngine.Random.Range(0, maxIndex);
        string nick = StaticData.configExcel.RandomName[firstNameIndex].FirstName + StaticData.configExcel.RandomName[secondName].SecondName;
        if (addUid)
            return nick + StaticData.Uid.ToString().Substring(3, 4); //截取uid后四位

        return nick;
    }

    #endregion

    /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
    string GetComputerName()
    {
        try
        {
#if UNITY_EDITOR
            return System.Environment.GetEnvironmentVariable("ComputerName");
#else
            return "";
#endif
        }
        catch
        {
            return "";
        }
        finally
        {
        }
    }
#region 服务器链接请求 1.请求短链接获取长连接id 端口 2.请求长连接

    /// <summary>
    /// 请求短链接
    /// </summary>
    /// <param name="sole"> 授权使用的唯一id </param>
    /// <param name="platform">授权平台</param>
    private async void RequestShortLink(string sole = null, RegisterPlatform platform = RegisterPlatform.None) 
    {

        if (_isStartRequst)
            return;
        Debug.Log("请求短链接 RequestShortLink _typeGateWay ="+ _typeGateWay);

        // 设置请求短链接参数
        //加载网关
        TextAsset gatewayInfo = await ABManager.GetAssetAsync<TextAsset>("GateWayInfo");
        switch (_typeGateWay)
        {
            case TypeGateWay.Stable:
                gatewayInfo = await ABManager.GetAssetAsync<TextAsset>("GateWayInfo");
                break;
            case TypeGateWay.MZQ:
                gatewayInfo = await ABManager.GetAssetAsync<TextAsset>("GateWayInfoMZQ");
                break;
            case TypeGateWay.YQX:
                gatewayInfo = await ABManager.GetAssetAsync<TextAsset>("GateWayInfoYQX");
                break;
            case TypeGateWay.JLF:
                gatewayInfo = await ABManager.GetAssetAsync<TextAsset>("GateWayInfoJLF");
                break;
            case TypeGateWay.Extranet:
                gatewayInfo = await ABManager.GetAssetAsync<TextAsset>("GateWayInfoExtranet");
                if (StaticData.isUsePlatformUpdatingGateWay)
                {
                    gatewayInfo= await ABManager.GetAssetAsync<TextAsset>("GateWayInfoExtranetUpdating");
                }
                break;
        }

        GateWayInfo gateWayInfo = LitJson.JsonMapper.ToObject<GateWayInfo>(gatewayInfo.text);
        StaticData.DebugGreen($"gateWayInfo ip:{gateWayInfo.ipGateWay} port:{gateWayInfo.portGateWay} timeout:{gateWayInfo.timeout}");
        
        // 0 等待中 1 请求成功 2 请求失败
        int gateWayReturn = 0;
        string urlGateWay = $"{gateWayInfo.ipGateWay}:{gateWayInfo.portGateWay}";
        if (!StaticData.IsUsedLocalDataNotServer)
        {
            CSLinkInfo csLinkInfo = new CSLinkInfo();// { Sole  = SystemInfo.deviceUniqueIdentifier};
            if (platform != RegisterPlatform.None)
            {
                csLinkInfo.Sole = sole;//授权使用的唯一id
                csLinkInfo.Platform = platform;//授权平台
            }
            else 
            {
                csLinkInfo.Sole = SystemInfo.deviceUniqueIdentifier+ GetComputerName();
                Debug.LogWarning($"==============csLinkInfo.Sole:{csLinkInfo.Sole}");
            }

            Console.WriteLine("OnHeartbeatUpdate: Ping2");

            _isStartRequst = true;
            SetLoginButInteractable(false);

            //发起链接请求
            StartCoroutine(HttpGateWayManager.Post(urlGateWay, csLinkInfo, gateWayInfo.timeout, (ip, port, Uid) =>
            {
                StaticData.ipWebSocket = ip;
                StaticData.portWebSocket = port;
                StaticData.Uid = Uid;
                StaticData.DebugGreen($"玩家的uid：{Uid}");
                gateWayReturn = 1;
            }, (WebErrorCode webErrorCode) =>
            {
                StaticData.DebugGreen($"webErrorCode:{webErrorCode.ToString()}");
                gateWayReturn = 2;
            }));

            await UniTask.WaitUntil(()=> gateWayReturn != 0);

            if (gateWayReturn == 1)
            {
                //进行长连接 使用WebSocket
                RequestLongLink();
            }
            else 
            {
                //打开提示服务器正在维护中
                OpenTipesServerIsUnderMaintenance();
                _isStartRequst = false;
                SetLoginButInteractable(true);
            }
        }
    }

    //string ip, int port, long uid
    /// <summary>
    /// 请求长连接
    /// </summary>
    private void RequestLongLink() 
    {
        //进行长连接 使用WebSocket
        WebSocketComponent._instance.Init();
        //
        _isStartRequst = false;

        //获取用户信息
        GetUserInfo();
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    private async void GetUserInfo() 
    {
        //获取用户信息
        bool isGetUserInfo = false;
        CSEmptyAccountInfo csEmptyAccountInfo = new CSEmptyAccountInfo();
        OtherProtoHelper.GetUserInfo(csEmptyAccountInfo, (userInfo) => {
            StaticData.playerInfoData.userInfo = userInfo;
            TimeHelper.LoginServerTime = StaticData.playerInfoData.userInfo.PresentTime;
            StaticData.DebugGreen($"ServerTime:{TimeHelper.ServerTime(TimeHelper.LoginServerTime)}");
            isGetUserInfo = true;
        });
        await UniTask.WaitUntil(() => isGetUserInfo == true);

        //数据打点
        StaticData.DataDot(DotEventId.LoginSucc);

        //加载本地存储的私聊信息/文件
        ChatTool.LoadPrivateChatFile();

        //清空订单时间 //订单数据
        StaticData.playerInfoData.GetDealTime = string.Empty;

        SetLoginButInteractable(true);

        //昵称是否为空 是否为老玩家
        if (string.IsNullOrEmpty(StaticData.playerInfoData.userInfo.Name))
        {
            //设置玩家昵称+选择头像
            EnterSetNickAndIcon();
        }
        else 
        {
            LoginCompleteEnterManor();
        }
    }
#endregion


    /// <summary>
    /// 登录+ 设置昵称完成完成进入主页
    /// </summary>
    private async void LoginCompleteEnterManor()
    {
        //await ChatTool.InitialChat();

        HideAnimEffect();

        if (!ChapterHelper.IsFinishSection())
        {
            await ChapterHelper.NewUserJoinChapter();
        }
        else
        {
            //关闭登录界面
            UIComponent.RemoveUI(UIType.UILogin);
            //加载自己庄园
            await StaticData.ToManorSelf();
            StaticData.DataDot(Company.Cfg.DotEventId.LoginToLobby);
            //登录完成初始化角色
            //HallRoleManager.Instance.InitRole();
        }
        //加载本地存储数据
        UniversalTool.LoadLocalSaveData();

        RedDotManager.Initial();
        //是否请求每日订单
        if (StaticData.playerInfoData.userInfo.SectionId >= 1000) //1000 序章
        {
            //请求每日订单
            StaticData.RequestDeals();
        }

        //打开公告界面
        StaticData.OpenUISystemNotification();
        //更新任务图标红点标记 2020/12/18
        TaskPanelTool.InitialUpdateTaskTag();
        //更新邮件图标红点标记 2020/12/21
        MailboxTool.InitialUpdateTaskTag();
        StaticData.SetSceneState(StaticData.SceneState.ManorSelf);

        List<int> GuideIdList = new List<int>();
        if (StaticData.playerInfoData.userInfo.Guidance!=null)
        {
            for (int i = 0; i < StaticData.playerInfoData.userInfo.Guidance.Count; i++)
            {
                GuideIdList.Add(StaticData.playerInfoData.userInfo.Guidance[i]);
            }
        }
        GuideCanvasComponent._instance.SetGuideFinishListId(GuideIdList);

        //关闭登录界面
        UIComponent.RemoveUI(UIType.UILogin);
    }

#endregion

#region 网络异常

    
    /// <summary>
    /// 网络检测
    /// </summary>
    /// <returns></returns>
    private bool NetworkDetection() 
    {
        //网络判断
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //打开网络连接异常提示界面
            OpenNetworkAnomalyTips();
            return false;
        }
        return true;
    }

    /// <summary>
    /// 打开网络异常提示
    /// </summary>
    private void OpenNetworkAnomalyTips() 
    {
        Debug.Log("UILoginController OpenNetworkAnomalyTips 打开网络异常提示！");
        string desc = LocalizationDefineHelper.GetStringNameById(120219);
        //120075 //取消
        StaticData.OpenCommonTips(desc, 120220, NetworkAnomalyTipsReconnect, null, 120075);
    }

    private void NetworkAnomalyTipsReconnect() 
    {
        switch (_loginType)
        {
            case LoginType.LoginDef:
                OnClickLogin();
                break;
            case LoginType.LoginWechat:
                OnClickAuthorizedLoginWeixin();
                break;
            case LoginType.LoginQQ:
                OnClickAuthorizedLoginQQ();
                break;
            default:
                break;
        }
    }

#endregion

#region 账号被挤掉

    /// <summary>
    /// 账号被被挤掉
    /// </summary>
    private void OpenAccountWasSqueezedTips()
    {
        Debug.Log("UILoginController OpenAccountWasSqueezedTips 账号被被挤掉！");
        string desc = LocalizationDefineHelper.GetStringNameById(120221);
        //120075 //取消
        StaticData.OpenCommonTips(desc, 120220, NetworkAnomalyTipsReconnect, null, 120075);
    }
#endregion

    /// <summary>
    /// 服务器正在维护
    /// </summary>
    private void OpenTipesServerIsUnderMaintenance() 
    {
        //服务器维护中，请稍后再试。
        string desc = LocalizationDefineHelper.GetStringNameById(120237);

        StaticData.OpenCommonTips(desc, 120016, null, null, 120075);
    }

    private GameObject _animEffect;
    public async void SpawnAnimEffect()
    {

        Transform parent = UIRoot.instance.GetUIRootCanvas().transform.parent;
        string perfabName = "LoginEffect";
        var obj = await ABManager.GetAssetAsync<GameObject>(perfabName);
        _animEffect = Instantiate(obj, parent);
    }

    private void HideAnimEffect()
    {
        if (_animEffect != null)
        {
            _animEffect.SetActive(false);
            Destroy(_animEffect);
            _animEffect = null;
        }
    }
}
