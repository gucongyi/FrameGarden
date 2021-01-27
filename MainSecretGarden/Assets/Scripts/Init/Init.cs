using Company;
using Company.Cfg;
using Cysharp.Threading.Tasks;
using Game.Protocal;
using System;
using UnityEngine;
using static StaticData;

public enum TypeGateWay
{
    Stable,//稳定的
    MZQ,//马自强
    YQX,//杨玘希
    JLF,//蒋林峰
    Extranet//外网
}
public class Init : MonoBehaviour
{
    public float versionGameCode;
    public string GameShowVersion;
    public GameObject goDebug;
    public bool isShowDebugWindow;
    public bool isShowSelfLog;
    [Header("服务器网关类型")]
    public TypeGateWay typeGate;
    public LinguisticType LanguageType;
    public bool IsUsedLocalDataNotServer;
    /// <summary>
    /// 是否显示FPS
    /// </summary>
    public bool isShowFPS;
    public bool isUseAssetBundle;
    //默认不选，从服务器下载
    public bool isABNotFromServer;
    public bool isABUsedYunSever;
    public string selfResourceServerIpAndPort;
    public int TargetFrameRate;
    public bool isOpenGuide;
    private async void Start()
    {
        //启动UniTask Player Loop 注册事件
        PlayerLoopHelper.Init();
        if (isShowDebugWindow)
        {
            goDebug.SetActive(true);
        }
        else
        {
            goDebug.SetActive(false);
        }

        if (isShowFPS)
        {
            ResourcesHelper.InstantiatePrefabFromResourceSetDefault("UIFrameShow", UIRoot.instance.GetUIRootCanvasTop().transform);
        }

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.runInBackground = true;
        Application.targetFrameRate = TargetFrameRate;
        StaticData.IsABNotFromServer = isABNotFromServer;
        StaticData.isABUsedYunSever = isABUsedYunSever;
        StaticData.SelfResourceServerIpAndPort = selfResourceServerIpAndPort;
        StaticData.localVersionGameCode = versionGameCode;
        StaticData.isOpenGuide = isOpenGuide;
#if UNITY_EDITOR
        StaticData.linguisticType = LanguageType;
#else
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseSimplified:
                StaticData.linguisticType = LinguisticType.Simplified;
                break;
            case SystemLanguage.ChineseTraditional:
                StaticData.linguisticType = LinguisticType.Complex;
                break;
            case SystemLanguage.English:
            case SystemLanguage.Unknown:
                StaticData.linguisticType = LinguisticType.English;
                break;
            default:
                StaticData.linguisticType = LinguisticType.English;
                break;
        }
#endif
        StaticData.GameShowVersion = GameShowVersion;
        StaticData.isShowSelfLog = isShowSelfLog;
        try
        {
            //播放背景音乐
            GameSoundPlayer.Instance.PlayBgMusic(MusicHelper.BgMusicLogin);
            StaticData.isUseAssetBundle = isUseAssetBundle;
            DontDestroyOnLoad(gameObject);
            gameObject.AddComponentIfNull<GlobalConfigComponent>();
            gameObject.AddComponentIfNull<ResourcesComponent>();
            BundleDownloaderComponent bundleDownloaderComponent = gameObject.AddComponentIfNull<BundleDownloaderComponent>();
            var IsGameVersionCodeEqual = await BundleHelper.IsGameVersionCodeEqual();
            if (!IsGameVersionCodeEqual)
            {
                return;
            }
            // 下载ab包
            await BundleHelper.DownloadBundle();
            GameObject.Destroy(bundleDownloaderComponent);
            //初始化ABManager
            ABManager.Init();
            //数据表格解析
            ParseExcelData parseExcelData = new ParseExcelData();
            StaticData.configExcel = parseExcelData.Init();
            //用来拿取服务器
            StaticData.IsiosRelease = StaticData.configExcel.GetVertical().IsiosRelease;
            StaticData.IsAndoridRelease = StaticData.configExcel.GetVertical().IsAndoridRelease;
            //Test Excel
            int WarehouseTotal = StaticData.configExcel.GetVertical().WarehouseTotal;
            Debug.Log($"WarehouseTotal:{WarehouseTotal}");
            ////下载好了，声音ab绑定到固定的地方
            //await ResetSoundPlayers();
            //GameSoundPlayer.Instance.PlayBgMusic("BGM");
            //GameSoundPlayer.Instance.PlaySoundEffect("PrintPhotos");
            //ResourcesHelper.CreateUI("UIWait");
            await UniTask.DelayFrame(1);

            //添加SceneManager
            GameObject goSceneManager = new GameObject("SceneManager");
            goSceneManager.transform.parent = transform;
            goSceneManager.AddComponent<SceneManagerComponent>();

            // 设置请求短链接参数
            StaticData.IsUsedLocalDataNotServer = IsUsedLocalDataNotServer;

            /*
            //加载网关
            TextAsset gatewayInfo = await ABManager.GetAssetAsync<TextAsset>("GateWayInfo");
            switch (typeGate)
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
                    break;
            }

            GateWayInfo gateWayInfo=LitJson.JsonMapper.ToObject<GateWayInfo>(gatewayInfo.text);
            //StaticData.DebugGreen($"gateWayInfo ip:{gateWayInfo.ipGateWay} port:{gateWayInfo.portGateWay} timeout:{gateWayInfo.timeout}");
            bool isGateWayReturn = false;
            string urlGateWay = $"{gateWayInfo.ipGateWay}:{gateWayInfo.portGateWay}";
            if (!StaticData.IsUsedLocalDataNotServer)
            {
                CSLinkInfo csLinkInfo = new CSLinkInfo() { Sole = SystemInfo.deviceUniqueIdentifier };
                csLinkInfo.Sole = string.Empty;//授权使用的唯一id
                csLinkInfo.Platform = RegisterPlatform.None;//授权平台

                //发起链接请求
                StartCoroutine(HttpGateWayManager.Post(urlGateWay, csLinkInfo, gateWayInfo.timeout, (ip, port, Uid) =>
                {
                    StaticData.ipWebSocket = ip;
                    StaticData.portWebSocket = port;
                    StaticData.Uid = Uid;
                    StaticData.DebugGreen($"玩家的uid：{Uid}");
                    isGateWayReturn = true;
                }, (WebErrorCode webErrorCode) =>
                {
                    StaticData.DebugGreen($"webErrorCode:{webErrorCode.ToString()}");
                    isGateWayReturn = false;
                }));

                await UniTask.WaitUntil(() => isGateWayReturn == true);
                //进行长连接 使用WebSocket
                WebSocketComponent._instance.Init();
            }
            
            //获取用户信息
            bool isGetUserInfo = false;
            CSEmptyAccountInfo csEmptyAccountInfo = new CSEmptyAccountInfo();
            OtherProtoHelper.GetUserInfo(csEmptyAccountInfo,(userInfo)=> {
                StaticData.playerInfoData.userInfo = userInfo;
                TimeHelper.LoginServerTime = StaticData.playerInfoData.userInfo.PresentTime;
                StaticData.DebugGreen($"ServerTime:{TimeHelper.ServerTime(TimeHelper.LoginServerTime)}");
                isGetUserInfo = true;
            });
            await UniTask.WaitUntil(() => isGetUserInfo == true);
            //加载本地存储的私聊信息/文件
            StaticData.LoadPrivateChatFile();
            */
            
            //登录界面
            await StaticData.OpenUILogin(typeGate, null);

            //关闭Init背景
            UIRoot.instance.GoBgInit.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private static async System.Threading.Tasks.Task ResetSoundPlayers()
    {
        //更新音乐
        var goMusicPlayer = GameObject.Find("Global/GameSoundPlayer/MusicPlayer");
        var goEffectPlayer = GameObject.Find("Global/GameSoundPlayer/SFXPlayer");
        var soundPlayerMusic = goMusicPlayer.GetComponent<SoundPlayer>();
        var soundPlayerEffect = goEffectPlayer.GetComponent<SoundPlayer>();
        var goMusicList = await ABManager.GetAssetAsync<GameObject>("MusicList");
        goMusicList = GameObject.Instantiate(goMusicList);
        goMusicList.transform.parent = goMusicPlayer.transform;
        soundPlayerMusic.soundLists[0] = goMusicList.GetComponent<SoundList>();
        var goSound2dEffectList = await ABManager.GetAssetAsync<UnityEngine.GameObject>("SoundList2D");
        var goSound3dEffectList = await ABManager.GetAssetAsync<UnityEngine.GameObject>("SoundList3D");
        goSound2dEffectList = GameObject.Instantiate(goSound2dEffectList);
        goSound3dEffectList = GameObject.Instantiate(goSound3dEffectList);
        goSound2dEffectList.transform.parent = goEffectPlayer.transform;
        goSound3dEffectList.transform.parent = goEffectPlayer.transform;
        soundPlayerEffect.soundLists[0] = goSound2dEffectList.GetComponent<SoundList>();
        soundPlayerEffect.soundLists[1] = goSound3dEffectList.GetComponent<SoundList>();
        //重新初始话
        soundPlayerMusic.soundLists[0].InitializeAudioSourcePools();
        soundPlayerEffect.soundLists[0].InitializeAudioSourcePools();
        soundPlayerEffect.soundLists[1].InitializeAudioSourcePools();
    }


    private void Update()
    {

    }

    private void LateUpdate()
    {

    }

    private void OnApplicationQuit()
    {

    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {

        }
    }
}