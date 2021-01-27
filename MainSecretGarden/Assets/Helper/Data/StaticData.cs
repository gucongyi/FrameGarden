using BestHTTP.Extensions;
using Company.Cfg;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using Game.Protocal;
using Google.Protobuf;
using Google.Protobuf.Collections;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public static class StaticData
{
    #region 判定是不是正式服务器，从服务器拿ip用，anroid,ios,正式，测试4个服务器
    public static bool IsiosRelease;
    public static bool IsAndoridRelease;
    #endregion
    public static int intParentResABDirectory;
    public static bool isUsePlatformUpdatingGateWay;
    public static float localVersionGameCode;
    public static string GameShowVersion;
    /// <summary>
    /// 下载buddle,Editor也用ab.
    /// </summary>
    public static bool isUseAssetBundle = false;
    public static bool IsABNotFromServer = false;
    public static string SelfResourceServerIpAndPort;
    public static bool isUseStreamingAssetRes = false;
    public static bool isNotWifiDownloadClick;
    public static bool isNotWifiDownload;
    public static Config configExcel;
    public static bool isABUsedYunSever;
    public static bool isShowSelfLog;
    public static bool isOpenGuide;
    //websocketIp
    public static string ipWebSocket;
    public static int portWebSocket;
    public static bool IsUsedLocalDataNotServer;
    public static long Uid = 0;//登录的时候获取，后边一致要用


    //用户信息数据
    public static PlayerInfoData playerInfoData = new PlayerInfoData();
    //进入庄园跳转场景是用来保存好友庄园的id
    public static long idPreEnterManor = -1;
    //标记是否请求过好友列表
    public static bool isGetFriendList = false;
    //当前进入好友庄园的好友信息
    public static FriendStealInfo curFriendStealInfo;

    //国际化
    public enum TypeLanguage
    {
        SimplifiedChinese,
        TraditionalChinese,
        English
    }
    public static LinguisticType linguisticType;


    public const float HeightScreenDesign = 2688f;
    public const float WidthScreenDesign = 1242f;

    #region 大厅到其它界面跳转



    //仓库红点功能
    internal static bool IsWarehouseRedDotOpen()
    {
        //todo
        bool isOpen = WarehouseTool.IsOpenDot();
        return isOpen;
    }

    /// <summary>
    /// 大富翁返回到庄园
    /// </summary>
    /// <returns></returns>
    public static async UniTask RichManReturnToManor()
    {
        DebugGreen($"HomePageControl ComebackMainUI() 返回到庄园");
        //先关闭，否则引导会跟随进度条
        UIComponent.RemoveUI(UIType.UIHomePage);
        UIComponent.RemoveUI(UIType.UIHomePageBG);
        UIComponent.RemoveUI(UIType.UIPlayInterface);
        UIComponent.RemoveUI(UIType.UISettlement);
        //加载场景
        await SceneManagerComponent._instance.ChangeSceneAsync(EnumScene.Manor, true);

        await UIComponent.CreateUIAsync(UIType.UIManor);
        UIManorComponent uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
        uiManorComponent.OpenManorRoot(true);

        //播放背景音乐
        GameSoundPlayer.Instance.PlayBgMusic(MusicHelper.BgMusicManor);

        if (Root2dSceneManager._instance != null)
        {
            await Root2dSceneManager._instance.EnterManor(-1);
        }

        StaticData.SetSceneState(SceneState.ManorSelf);
        //移动到特定视角时间
        await UniTask.Delay(1000);
        UIComponent.HideUI(UIType.UISceneLoading);
        await StaticData.PlayManorEnterAnim();
    }


    //庄园打开订单
    internal static async UniTask OpenOrder()
    {
        //todo
        EnterUIDeal();
    }

    //庄园世界的点转UI anchor pos
    public static Vector2 ManorWorldPointToUICameraAnchorPos(Vector3 worldPos)
    {
        Vector3 ScreenPoint = Root2dSceneManager._instance.worldCameraComponent.cameraWorld.WorldToScreenPoint(worldPos);
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UIRoot.instance.GetUIRootCanvas().GetComponent<RectTransform>(), ScreenPoint, Camera.main, out localPoint);
        return localPoint;
    }
    public static Vector2 RectTransWorldPointToUICameraAnchorPos(Vector3 worldPos)
    {
        Vector3 ScreenPoint = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UIRoot.instance.GetUIRootCanvas().GetComponent<RectTransform>(), ScreenPoint, Camera.main, out localPoint);
        return localPoint;
    }
    public static Vector2 ScreenPointToUICameraAnchorPos(Vector3 screenPos)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UIRoot.instance.GetUIRootCanvas().GetComponent<RectTransform>(), screenPos, Camera.main, out localPoint);
        return localPoint;
    }
    public static UIWorldHandleManager GetUIWorldHandleComponent()
    {
        UIManorComponent uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
        if (uiManorComponent != null)
        {
            return uiManorComponent.uIWorldHandleManager;
        }
        return null;
    }
    /// <summary>
    /// 大厅到庄园
    /// </summary>
    /// <returns></returns>
    public static async UniTask ToManorSelf()
    {
        //加载场景
        await SceneManagerComponent._instance.ChangeSceneAsync(EnumScene.Manor, true);
        await UIComponent.CreateUIAsync(UIType.UIManor);
        UIManorComponent uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
        uiManorComponent.OpenManorRoot(true);
        //播放背景音乐
        GameSoundPlayer.Instance.PlayBgMusic(MusicHelper.BgMusicManor);
        StaticData.SetSceneState(SceneState.ManorSelf);
        if (Root2dSceneManager._instance != null)
        {
            await Root2dSceneManager._instance.EnterManor(-1);
        }
        //移动到特定视角时间
        await UniTask.Delay(1000);
        UIComponent.HideUI(UIType.UISceneLoading);
        //是否触发Avg动画
        if (LocalInfoData.localManorInfoData != null && LocalInfoData.localManorInfoData.isFirstAvgDialogFinish == false)
        {
            //不做任何事情
        }
        else
        {
            //打开对话框 AVG动画不打开庄园界面
            await StaticData.PlayManorEnterAnim();
        }
    }

    #endregion

    public enum SceneState
    {
        RichMan,
        ManorSelf,
        ManorFriend
    }
    //用来进行仓库的跳转
    static SceneState preSceneState = SceneState.ManorSelf;

    public static void SetSceneState(SceneState sceneState)
    {
        preSceneState = sceneState;
    }

    public static async UniTask JumpToSceneState(Action jumpPreStateCallback, SceneState sceneState, Action jumpAfterStateCallBack)
    {
        jumpPreStateCallback?.Invoke();
        if (preSceneState != sceneState)
        {
            switch (sceneState)
            {
                case SceneState.RichMan:
                    switch (preSceneState)
                    {
                        case SceneState.ManorSelf:
                            //设置相机叠加方式
                            UICameraManager._instance.SetDefault();
                            UIComponent.RemoveUI(UIType.UIFriend);
                            UIComponent.RemoveUI(UIType.Warehouse);
                            UIComponent.RemoveUI(UIType.UIShop);
                            UIComponent.RemoveUI(UIType.UIManor);
                            await OpenMonopoly();
                            break;
                        case SceneState.ManorFriend:
                            //设置相机叠加方式
                            UICameraManager._instance.SetDefault();
                            UIComponent.RemoveUI(UIType.UIFriend);
                            UIComponent.RemoveUI(UIType.Warehouse);
                            UIComponent.RemoveUI(UIType.UIShop);
                            UIComponent.RemoveUI(UIType.UIManor);
                            await OpenMonopoly();
                            break;
                    }
                    break;
                case SceneState.ManorSelf:
                    switch (preSceneState)
                    {
                        case SceneState.RichMan:
                            await ToManorSelf();
                            break;
                    }
                    break;
            }
            SetSceneState(sceneState);
        }
        jumpAfterStateCallBack?.Invoke();
    }

    public static string IntToIp(long ipInt)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append((ipInt >> 24) & 0xFF).Append(".");
        sb.Append((ipInt >> 16) & 0xFF).Append(".");
        sb.Append((ipInt >> 8) & 0xFF).Append(".");
        sb.Append(ipInt & 0xFF);
        return sb.ToString();
    }

    public static void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
    }


    public static void DebugGreen(string logInfo)
    {
        if (isShowSelfLog)
        {
            Debug.Log($"<color=green>{logInfo}</color>");
        }
    }

    public static void DestroySafe(MonoBehaviour monoComponent)
    {
        if (monoComponent != null)
        {
            GameObject.Destroy(monoComponent.gameObject);
        }
    }
    /// <summary>
    /// 获得当前点击到的UI物体
    /// </summary>
    public static GameObject UI_GetCurrentSelect()
    {
        GameObject obj = null;

        List<GraphicRaycaster> listGraphicRaycasters = new List<GraphicRaycaster>();
        listGraphicRaycasters.Add(UIRoot.instance.GetUIRootCanvasGuide().GetComponent<GraphicRaycaster>());
        listGraphicRaycasters.Add(UIRoot.instance.GetUIRootCanvasTop().GetComponent<GraphicRaycaster>());
        listGraphicRaycasters.Add(UIRoot.instance.GetUIRootCanvas().GetComponent<GraphicRaycaster>());
        listGraphicRaycasters.Add(UIRoot.instance.GetUIRootCanvasBG().GetComponent<GraphicRaycaster>());

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.pressPosition = Input.mousePosition;
        eventData.position = Input.mousePosition;
        List<RaycastResult> list = new List<RaycastResult>();

        bool isFind = false;
        foreach (var item in listGraphicRaycasters)
        {
            item.Raycast(eventData, list);
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    obj = list[i].gameObject;
                    isFind = true;
                    break;//找到一个就是最上层的，后边的是下边的
                }
            }
            if (isFind)
            {
                break;
            }
        }
        return obj;
    }

    public static TimeCountDownComponent CreateTimer(float timeCountDown, bool isUnscaledDeltaTime, Action<GameObject> OnTimeCountDownCompelete, Action<float> OnActionOnSecondPass, string TimeName)
    {
        GameObject go = new GameObject($"Temp Timer-{TimeName}");
        TimeCountDownComponent countDownCom = go.AddComponent<TimeCountDownComponent>();
        countDownCom.Init(timeCountDown, isUnscaledDeltaTime, OnTimeCountDownCompelete, OnActionOnSecondPass);
        return countDownCom;
    }
    public static TimeCountDownComponent CreateTimerRebackMilliSeconds(float timeCountDown, bool isUnscaledDeltaTime, Action<GameObject> OnTimeCountDownCompelete, Action<float> OnActionOnMilliSecondPass)
    {
        GameObject go = new GameObject("Temp Timer");
        TimeCountDownComponent countDownCom = go.AddComponent<TimeCountDownComponent>();
        countDownCom.InitSecondsOnFrameCallBack(timeCountDown, isUnscaledDeltaTime, OnTimeCountDownCompelete, OnActionOnMilliSecondPass);
        return countDownCom;
    }
    /// <summary>
    /// 获取多语言对应文字
    /// </summary>
    /// <param name="id">语言id</param>
    /// <returns></returns>
    public static string GetMultilingual(int id)
    {
        LocalizeDefine localizeDefine = null;
        for (int i = 0; i < configExcel.Localize.Count; i++)
        {

            if (configExcel.Localize[i].ID == id)
            {
                localizeDefine = configExcel.Localize[i];
            }
        }
        string str = "";
        if (localizeDefine != null)
        {
            switch (StaticData.linguisticType)
            {
                case LinguisticType.Simplified:
                    str = localizeDefine.SimplifiedChinese;
                    break;
                case LinguisticType.Complex:
                    str = localizeDefine.TraditionalChinese;
                    break;
                case LinguisticType.English:
                    str = localizeDefine.English;
                    break;
            }
        }
        return str;
    }

    /// <summary>
    /// 获取道具来源名称/描述
    /// </summary>
    /// <param name="accessType"></param>
    /// <returns></returns>
    public static string GetAccessDesc(ItemAccessType accessType)
    {
        int localizeID = 0;
        switch (accessType)
        {
            case ItemAccessType.None:
                break;
            case ItemAccessType.QuickPurchase:
                localizeID = 120159;
                break;
            case ItemAccessType.PointsStore:
                localizeID = 120160;
                break;
            case ItemAccessType.ActivityGift:
                localizeID = 120161;
                break;
            default:
                break;
        }
        if (localizeID == 0)
            return string.Empty;
        return StaticData.GetMultilingual(localizeID);
    }

    /// <summary>
    /// 根据道具id获取对应道具的名称
    /// </summary>
    /// <param name="gameItemId"></param>
    /// <param name="linguisticType"></param>
    /// <returns></returns>
    public static string GetMultiLanguageByGameItemId(int gameItemId)
    {
        var gameItemDefine = StaticData.configExcel.GetGameItemByID(gameItemId);
        return GetMultilingual(gameItemDefine.ItemName);
    }

    /// <summary>
    /// 根据道具id获取道具对应的国际化语言id
    /// </summary>
    /// <param name="gameItemId"></param>
    /// <returns></returns>
    public static int GetMultiLanguageIDByGameItemId(int gameItemId)
    {
        return StaticData.configExcel.GetGameItemByID(gameItemId).ItemName;
    }
    /// <summary>
    /// 获取玩家的等级
    /// </summary>
    /// <param name="playerExp">此参数不为-1时，根据传入的经验值计算等级</param>
    /// <returns></returns>
    public static int GetPlayerLevelByExp(int playerExp = -1)
    {
        int level = 1;
        int exp;
        if (playerExp == -1)
        {
            exp = StaticData.playerInfoData.userInfo.Experience;
        }
        else
        {
            exp = playerExp;
        }
        var playerGradeConfig = StaticData.configExcel.PlayerGrade;
        int maxExp = 0;
        for (int i = 0; i < playerGradeConfig.Count; i++)
        {
            if (exp >= playerGradeConfig[i].TotalExperience && maxExp < playerGradeConfig[i].TotalExperience)
            {
                maxExp = playerGradeConfig[i].TotalExperience;
                level = playerGradeConfig[i].Grade + 1;
            }
        }
        return level;
    }

    /// <summary>
    /// 添加玩家经验 并且判断是否需要升级 发出升级请求
    /// </summary>
    /// <param name="exp"></param>
    public static void AddPlayerExp(int exp)
    {
        int lastLv = GetPlayerLevelAndCurrExp().level;
        playerInfoData.userInfo.Experience += exp;
        ManorProtocalHelper.TriggerManorRegionUnLock(lastLv);
    }

    /// <summary>
    /// 检测升级
    /// </summary>
    /// <param name="lastLv"></param>
    /// <param name="willUnLockRegionId">解锁地块id 默认-1</param>
    public static void DetectionUpgrade(int lastLv, bool isTriggerUnLockArea = false, int willUnLockRegionId = -1)
    {
        int curLv = GetPlayerLevelAndCurrExp().level;

        //等级提升
        if (curLv > lastLv)
        {
            if (isTriggerUnLockArea)
            {
                RequestUpgrade(willUnLockRegionId);
            }
            else
            {
                RequestUpgrade();
            }


        }
        //更新任务图标红点标记 2020/12/18
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Task);
    }


    //获取地块
    public static int GetPlotUseNumCount()
    {
        return playerInfoData.userInfo.PlotUseNum;
    }
    //拖出装饰物数量减1
    public static void DecorateMinusOne(int idDecorate)
    {
        if (StaticData.GetWareHouseItem(idDecorate).GoodNum > 0)
        {
            StaticData.GetWareHouseItem(idDecorate).GoodNum -= 1;
        }
    }

    public static int GetCurrWareHouseFruitCount()
    {
        int count = 0;
        List<CSWareHouseStruct> listSeed = new List<CSWareHouseStruct>();
        var listAllProp = playerInfoData.userInfo.WareHoseInfo;
        for (int i = 0; i < listAllProp.Count; i++)
        {
            var itemDefine = StaticData.configExcel.GetGameItemByID(listAllProp[i].GoodId);
            if (itemDefine != null && itemDefine.ItemType == TypeGameItem.Fruit && listAllProp[i].GoodNum > 0)
            {
                count += listAllProp[i].GoodNum;
            }
        }
        return count;
    }

    //拿仓库中的种子
    public static List<CSWareHouseStruct> GetWareHouseSeeds()
    {
        List<CSWareHouseStruct> listSeed = new List<CSWareHouseStruct>();
        var listAllProp = playerInfoData.userInfo.WareHoseInfo;
        for (int i = 0; i < listAllProp.Count; i++)
        {
            var itemDefine = StaticData.configExcel.GetGameItemByID(listAllProp[i].GoodId);
            if (itemDefine != null && itemDefine.ItemType == TypeGameItem.Seed && listAllProp[i].GoodNum > 0)
            {
                listSeed.Add(listAllProp[i]);
            }
        }
        return listSeed;
    }
    public class PlantSeed
    {
        public int idSeed;
        public int level;
        //是否是稀有种子
        public bool isRarity;
        public string iconName;
        //种子的数量
        public int numSeed;
        //单价
        public int coinPriceId;
        public string coinName;
        public long price;
        //成熟时间
        public int timeSeconds;
        public string timeShow;
        //是否是订单需要的种子
        public bool isDealNeed;
    }
    //获取订单中缺的果实的种子id
    static List<int> GetSeedsFromDealNeed()
    {
        List<int> listSeedDealNeed = new List<int>();
        var dealInfo = StaticData.playerInfoData.CurrDeals.DealInfo;
        if (dealInfo.Count > 0)
        {
            for (int i = 0; i < dealInfo.Count; i++)
            {
                if (dealInfo[i].DealNeedGoods.Count > 0)
                {
                    for (int eachDealNeedGood = 0; eachDealNeedGood < dealInfo[i].DealNeedGoods.Count; eachDealNeedGood++)
                    {
                        int idFruit = dealInfo[i].DealNeedGoods[eachDealNeedGood].GoodId;
                        int goodHaveNum = StaticData.GetWareHouseItem(idFruit).GoodNum;
                        int goodNeedNum = dealInfo[i].DealNeedGoods[eachDealNeedGood].GoodNum;
                        if (goodHaveNum < goodNeedNum)
                        {
                            //找对应的种子id
                            var manorCropDefine = StaticData.configExcel.ManorCrop.Find(x => x.IdGainGameItem == idFruit);
                            if (manorCropDefine != null)
                            {
                                listSeedDealNeed.Add(manorCropDefine.IdSeed);
                            }
                        }
                    }
                }
            }
        }

        return listSeedDealNeed;
    }
    //拿种植时候的种子
    public static List<PlantSeed> GetPlantSeeds()
    {
        List<PlantSeed> listSeed = new List<PlantSeed>();
        //1.获取作物表所有的种子
        List<ManorCropDefine> listCropDefine = StaticData.configExcel.ManorCrop;
        //2.获取仓库中的种子
        List<CSWareHouseStruct> listSeedHave = new List<CSWareHouseStruct>();
        var listAllProp = playerInfoData.userInfo.WareHoseInfo;
        for (int i = 0; i < listAllProp.Count; i++)
        {
            var itemDefine = StaticData.configExcel.GetGameItemByID(listAllProp[i].GoodId);
            if (itemDefine != null && itemDefine.ItemType == TypeGameItem.Seed && listAllProp[i].GoodNum > 0)
            {
                listSeedHave.Add(listAllProp[i]);
            }
        }

        for (int i = 0; i < listCropDefine.Count; i++)
        {
            //道具表
            var itemDefine = StaticData.configExcel.GetGameItemByID(listCropDefine[i].IdSeed);
            if (itemDefine == null)
            {
                Debug.LogError($"========种子id:{listCropDefine[i].IdSeed}在道具表种不存在！");
            }
            var SeedHave = listSeedHave.Find(x => x.GoodId == listCropDefine[i].IdSeed);
            //商店表拿单价
            var storeDefine = StaticData.configExcel.Store.Find(x => x.GoodId == listCropDefine[i].IdSeed);
            if ((itemDefine.Rarity == TypeRarity.Intermediate || itemDefine.Rarity == TypeRarity.Senior) && SeedHave == null)
            {
                //中级或者以上稀有度的 并且仓库中没有的 不进入显示列表
                continue;
            }
            //没解锁的跳过,不进入显示列表
            if (itemDefine.Grade > StaticData.GetPlayerLevelAndCurrExp().level)
            {
                continue;
            }
            PlantSeed plantSeed = new PlantSeed();
            //赋值
            plantSeed.idSeed = listCropDefine[i].IdSeed;
            plantSeed.level = itemDefine.Grade;
            plantSeed.isRarity = (itemDefine.Rarity == TypeRarity.Intermediate || itemDefine.Rarity == TypeRarity.Senior) ? true : false;
            plantSeed.iconName = itemDefine.Icon;
            plantSeed.numSeed = SeedHave == null ? 0 : SeedHave.GoodNum;
            if (plantSeed.isRarity == false)
            {
                if (storeDefine.OriginalPrice.Count <= 0)
                {
                    Debug.LogError($"商店表ShopId={storeDefine.ShopId}的种子没有价格！");
                }
                plantSeed.coinPriceId = storeDefine.OriginalPrice[0].ID;
                plantSeed.coinName = StaticData.configExcel.GetGameItemByID(plantSeed.coinPriceId).Icon;
                plantSeed.price = storeDefine.OriginalPrice[0].Count;
            }
            plantSeed.timeSeconds = listCropDefine[i].GrowUp + listCropDefine[i].AdultnessTime;
            if (plantSeed.timeSeconds / 3600 > 0)//h
            {
                plantSeed.timeShow = string.Format("{0:N1}h", (float)plantSeed.timeSeconds / 3600);
            }
            else if (plantSeed.timeSeconds / 60 > 0)//m
            {
                plantSeed.timeShow = string.Format("{0:N1}m", (float)plantSeed.timeSeconds / 60);
            }
            else//s
            {
                plantSeed.timeShow = string.Format("{0}s", plantSeed.timeSeconds);
            }
            listSeed.Add(plantSeed);
        }

        //3.排序
        if (listSeed.Count > 1)
        {
            listSeed.Sort(SortSeedByRule);
        }
        else if (listSeed.Count == 1)//只有一个种子
        {
            //3.获取订单中需要的种子列表
            List<int> listSeedDealNeed = GetSeedsFromDealNeed();
            if (listSeedDealNeed.Contains(listSeed[0].idSeed))
            {
                listSeed[0].isDealNeed = true;
            }
        }
        return listSeed;
    }

    private static int SortSeedByRule(PlantSeed x, PlantSeed y)
    {
        //3.获取订单中需要的种子列表
        List<int> listSeedDealNeed = GetSeedsFromDealNeed();
        if (listSeedDealNeed.Contains(x.idSeed))
        {
            x.isDealNeed = true;
        }
        if (listSeedDealNeed.Contains(y.idSeed))
        {
            y.isDealNeed = true;
        }
        //现根据订单是否需要进行排序
        if ((x.isDealNeed && y.isDealNeed) || (x.isDealNeed == false && y.isDealNeed == false))
        {
            return y.level.CompareTo(x.level);
        }
        else
        {
            if (x.isDealNeed)
            {
                return -1;
            }
            if (y.isDealNeed)
            {
                return 1;
            }
        }
        return 0;
    }

    //拿仓库中的化肥
    public static List<CSWareHouseStruct> GetWareHouseFertilizer()
    {
        List<CSWareHouseStruct> listFertilizer = new List<CSWareHouseStruct>();
        var listAllProp = playerInfoData.userInfo.WareHoseInfo;
        for (int i = 0; i < listAllProp.Count; i++)
        {
            var itemDefine = StaticData.configExcel.GetGameItemByID(listAllProp[i].GoodId);
            if (itemDefine != null && itemDefine.ItemType == TypeGameItem.Fertilizers)
            {
                listFertilizer.Add(listAllProp[i]);
            }
        }
        return listFertilizer;
    }
    //庄园作物获取第几种化肥的数量
    public static CSWareHouseStruct GetFertilizerCountByWhich(int whichFertilizer)//第几种化肥，0，1，2
    {
        var listFertilizer = StaticData.GetWareHouseFertilizer();
        List<int> Fertilizers = StaticData.configExcel.GetVertical().StageRelation;
        int fertilizerId = Fertilizers[whichFertilizer];
        CSWareHouseStruct csWareHouseStruct = new CSWareHouseStruct() { GoodId = fertilizerId, GoodNum = 0 };
        var Fertilizerdata = listFertilizer.Find(x => x.GoodId == fertilizerId);
        if (Fertilizerdata != null)
        {
            csWareHouseStruct.GoodNum = Fertilizerdata.GoodNum;
        }
        return csWareHouseStruct;
    }

    //拿仓库中的装饰物
    public static List<CSWareHouseStruct> GetWareHouseDecorate()
    {
        List<CSWareHouseStruct> listDecorate = new List<CSWareHouseStruct>();
        var listAllProp = playerInfoData.userInfo.WareHoseInfo;
        for (int i = 0; i < listAllProp.Count; i++)
        {
            var itemDefine = StaticData.configExcel.GetGameItemByID(listAllProp[i].GoodId);
            if (itemDefine != null && itemDefine.ItemType == TypeGameItem.Decorate)
            {
                if (listAllProp[i].GoodNum > 0)//没有数量就不显示了
                {
                    listDecorate.Add(listAllProp[i]);
                }
            }
        }
        return listDecorate;
    }

    /// <summary>
    /// 获取仓库中的道具
    /// </summary>
    /// <param name="idCoin"></param>
    /// <returns></returns>
    public static CSWareHouseStruct GetWareHouseItem(int idItem)
    {
        CSWareHouseStruct Item = new CSWareHouseStruct()
        {
            GoodId = idItem,
            GoodNum = 0
        };
        var listAllProp = playerInfoData.userInfo.WareHoseInfo;
        for (int i = 0; i < listAllProp.Count; i++)
        {
            if (listAllProp[i].GoodId == idItem)
            {
                Item = listAllProp[i];
                break;
            }
        }
        return Item;
    }
    /// <summary>
    /// 获取仓库中的道具 金币数量
    /// </summary>
    /// <returns></returns>
    public static int GetWareHouseGold()
    {
        var item = GetWareHouseItem(StaticData.configExcel.GetVertical().GoldGoodsId);
        if (item == null)
            return 0;
        return item.GoodNum;
    }

    /// <summary>
    /// 更新仓库中的道具 金币
    /// </summary>
    /// <param name="itemNum"> 大于0+ 小于0-</param>
    public static void UpdateWareHouseGold(int itemNum)
    {
        UpdateWareHouseItem(StaticData.configExcel.GetVertical().GoldGoodsId, itemNum);
    }

    /// <summary>
    /// 获取仓库中的道具 钻石数量
    /// </summary>
    /// <returns></returns>
    public static int GetWareHouseDiamond()
    {
        var item = GetWareHouseItem(StaticData.configExcel.GetVertical().JewelGoodsId);
        if (item == null)
            return 0;
        return item.GoodNum;
    }

    /// <summary>
    /// 更新仓库中的道具 钻石
    /// </summary>
    /// <param name="itemNum"> 大于0+ 小于0-</param>
    public static void UpdateWareHouseDiamond(int itemNum)
    {
        UpdateWareHouseItem(StaticData.configExcel.GetVertical().JewelGoodsId, itemNum);
    }


    /// <summary>
    /// 更新仓库中的道具
    /// </summary>
    /// <param name="itemID"></param>
    /// <param name="itemNum"> 大于0+ 小于0-</param>
    public static void UpdateWareHouseItem(int itemID, int itemNum, bool isLock = false, bool isChangLock = false)
    {
        //增加装饰物红点判定
        var itemDefine = StaticData.configExcel.GetGameItemByID(itemID);
        if (itemDefine == null)
        {
            Debug.LogError("道具表中没有查询到道具，id：" + itemID);
            return;
        }
        if (itemDefine.ItemType == TypeGameItem.Decorate)
        {
            ManorRedDotTool.isOpenManorDecorateRedDot = true;
            RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.ManorDecorate);
        }
        bool isFind = false;
        var listAllProp = playerInfoData.userInfo.WareHoseInfo;
        for (int i = 0; i < listAllProp.Count; i++)
        {
            if (listAllProp[i].GoodId == itemID)
            {
                isFind = true;
                listAllProp[i].GoodNum += itemNum;
                if (isChangLock)
                {
                    listAllProp[i].IsLock = isLock;
                }
                if (listAllProp[i].GoodNum == 0)
                    listAllProp.RemoveAt(i);
                break;
            }
        }
        //物品不存在仓库中
        if (itemNum > 0 && !isFind)
        {
            CSWareHouseStruct newItem = new CSWareHouseStruct();
            newItem.GoodId = itemID;
            newItem.GoodNum = itemNum;
            if (isChangLock)
            {
                newItem.IsLock = isLock;
            }
            listAllProp.Add(newItem);
        }

        //更新物品提示
        UpdateItemTips(itemID);
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Warehouse);
    }

    /// <summary>
    /// 更新物品提示
    /// </summary>
    private static void UpdateItemTips(int itemID)
    {
        var item = StaticData.configExcel.GetGameItemByID(itemID);
        if (item == null)
            return;
        //作物
        if (item.ItemType == TypeGameItem.Fruit)
        {
            NotifyUpdateDealTips();
        }
    }


    /// <summary>
    /// 更新仓库中的道具
    /// </summary>
    /// <param name="itemID"></param>
    /// <param name="newItemNum">道具新的数量</param>
    public static void UpdateWareHouseItems(int itemID, int newItemNum)
    {
        bool isFind = false;
        var listAllProp = playerInfoData.userInfo.WareHoseInfo;
        for (int i = 0; i < listAllProp.Count; i++)
        {
            if (listAllProp[i].GoodId == itemID)
            {
                isFind = true;
                listAllProp[i].GoodNum = newItemNum;
                break;
            }
        }
        //物品不存在仓库中
        if (!isFind)
        {
            CSWareHouseStruct newItem = new CSWareHouseStruct();
            newItem.GoodId = itemID;
            newItem.GoodNum = newItemNum;
            listAllProp.Add(newItem);
        }
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Warehouse);
    }

    /// <summary>
    /// 获取仓库中的道具 紫金币数量
    /// </summary>
    /// <returns></returns>
    public static int GetWareHousePurpleGold()
    {
        var item = GetWareHouseItem(StaticData.configExcel.GetVertical().PurpleGoldsId);
        if (item == null)
            return 0;
        return item.GoodNum;
    }

    public static void UpdateFertilizerMinus1(int idFertilizer)
    {
        var listAllProp = playerInfoData.userInfo.WareHoseInfo;
        for (int i = 0; i < listAllProp.Count; i++)
        {
            if (listAllProp[i].GoodId == idFertilizer)
            {
                listAllProp[i].GoodNum -= 1;
                if (listAllProp[i].GoodNum <= 0)
                {
                    listAllProp[i].GoodNum = 0;
                }

                break;
            }
        }
    }
    //种植后更新种子
    public static void UpdateSeedMinus1(CSPlantData plantData)
    {
        var listAllProp = playerInfoData.userInfo.WareHoseInfo;
        for (int i = 0; i < plantData.PlantInfo.Count; i++)
        {
            for (int eachProp = 0; eachProp < listAllProp.Count; eachProp++)
            {
                if (listAllProp[eachProp].GoodId == plantData.PlantInfo[i].CropGoodId)
                {
                    listAllProp[eachProp].GoodNum -= 1;
                    if (listAllProp[eachProp].GoodNum <= 0)
                    {
                        listAllProp[eachProp].GoodNum = 0;
                    }
                    break;
                }
            }
        }
    }

    //根据SeedGrowComponent 获取当前作物的当前时期的总时间
    public static long GetSeedGrowComponentTotalSecond(SeedGrowComponent seedGrowComponent, SeedGrowComponent.PeriodGrow cropPeriod)
    {
        long remainSeconds = 100;
        ManorCropDefine manorCropDefine = StaticData.configExcel.GetManorCropByCropId(seedGrowComponent.tileComponent.CropGoodId);
        if (manorCropDefine == null)
        {
            //Debug.LogError($"作物 {seedGrowComponent.tileComponent.CropGoodId} 为null，请检查！");
            return remainSeconds;
        }
        switch (cropPeriod)
        {
            case SeedGrowComponent.PeriodGrow.Seed:
                remainSeconds = StaticData.configExcel.GetManorCropByCropId(seedGrowComponent.tileComponent.CropGoodId).GerminationPeriod;
                break;
            case SeedGrowComponent.PeriodGrow.Germinate:
                remainSeconds = StaticData.configExcel.GetManorCropByCropId(seedGrowComponent.tileComponent.CropGoodId).GrowUp;
                break;
            case SeedGrowComponent.PeriodGrow.GrowUp:
                remainSeconds = StaticData.configExcel.GetManorCropByCropId(seedGrowComponent.tileComponent.CropGoodId).AdultnessTime;
                break;
        }
        return remainSeconds;
    }
    //化肥的缩减时间
    public static long GetFertilizerAddTimeMilliSeconds(int idFertilizer)
    {
        var allFertilizerTime = StaticData.configExcel.GetVertical().Fertilizers;
        long addMilliSeconds = allFertilizerTime.Find(x => x.ID == idFertilizer).Count;
        return addMilliSeconds;
    }
    //收获增加玩家经验
    public static void AddPlayerExp(SCHarvestData scHarvestData)
    {
        int lastLv = GetPlayerLevelAndCurrExp().level;
        for (int i = 0; i < scHarvestData.HarvestResult.Count; i++)
        {
            int addExp = scHarvestData.HarvestResult[i].HarvestExperience;
            playerInfoData.userInfo.Experience += addExp;
            StaticData.DebugGreen($"收获SoilId_{scHarvestData.HarvestResult[i].SoilId}_HarvestId_{scHarvestData.HarvestResult[i].HarvestId}_HarvestNum_{scHarvestData.HarvestResult[i].HarvestNum}_HarvestExperience_{scHarvestData.HarvestResult[i].HarvestExperience} 总经验：{playerInfoData.userInfo.Experience}");
        }
        if (UIComponent.IsHaveUI(UIType.UIPersonalInformation))
        {
            UIPersonalInformationController personInfoCom = UIComponent.GetComponentHaveExist<UIPersonalInformationController>(UIType.UIPersonalInformation);
            personInfoCom.ShowLevelInfo();
        }
        ManorProtocalHelper.TriggerManorRegionUnLock(lastLv);
        //if (isTriggerUnLockArea)
        //{
        //    StaticData.TiggerUnLockArea(willUnLockRegionId);
        //}
    }
    //获取玩家经验
    public static int GetPlayerTotalExp()
    {
        return playerInfoData.userInfo.Experience;
    }
    public class PlayerLevelAndCurrExp
    {
        public int level;
        //当前算完等级剩下的经验
        public int currLevelHaveExp;
        //当前等级需要经验
        public int currLevelNeed;
    }
    //获取玩家等级
    public static PlayerLevelAndCurrExp GetPlayerLevelAndCurrExp()
    {
        PlayerLevelAndCurrExp playerLevelAndCurrExp = new PlayerLevelAndCurrExp()
        {
            level = 1,
            currLevelHaveExp = 0
        };
        int totalExp = GetPlayerTotalExp();
        for (int i = 0; i < StaticData.configExcel.PlayerGrade.Count; i++)
        {
            PlayerGradeDefine playerGradeDefine = StaticData.configExcel.PlayerGrade[i];
            if (totalExp < playerGradeDefine.TotalExperience)
            {
                playerLevelAndCurrExp.level = playerGradeDefine.Grade;
                int preLevelNeedTotalExp = 0;
                if (playerLevelAndCurrExp.level > 1)
                {
                    PlayerGradeDefine playerGradeDefinePre = StaticData.configExcel.GetPlayerGradeByGrade(playerLevelAndCurrExp.level - 1);
                    preLevelNeedTotalExp = playerGradeDefinePre.TotalExperience;
                }
                playerLevelAndCurrExp.currLevelHaveExp = totalExp - preLevelNeedTotalExp;
                playerLevelAndCurrExp.currLevelNeed = playerGradeDefine.Upgrade;
                break;
            }
        }
        //StaticData.DebugGreen($"玩家Level_{playerLevelAndCurrExp.level}_currLevelNeed_{playerLevelAndCurrExp.currLevelNeed}_currLevelHaveExp_{playerLevelAndCurrExp.currLevelHaveExp}");
        return playerLevelAndCurrExp;
    }

    //根据玩家经验获取玩家等级经验信息
    public static PlayerLevelAndCurrExp GetPlayerLevelAndCurrExp(int totalExp)
    {
        PlayerLevelAndCurrExp playerLevelAndCurrExp = new PlayerLevelAndCurrExp()
        {
            level = 1,
            currLevelHaveExp = 0
        };
        for (int i = 0; i < StaticData.configExcel.PlayerGrade.Count; i++)
        {
            PlayerGradeDefine playerGradeDefine = StaticData.configExcel.PlayerGrade[i];
            if (totalExp < playerGradeDefine.TotalExperience)
            {
                playerLevelAndCurrExp.level = playerGradeDefine.Grade;
                int preLevelNeedTotalExp = 0;
                if (playerLevelAndCurrExp.level > 1)
                {
                    PlayerGradeDefine playerGradeDefinePre = StaticData.configExcel.GetPlayerGradeByGrade(playerLevelAndCurrExp.level - 1);
                    preLevelNeedTotalExp = playerGradeDefinePre.TotalExperience;
                }
                playerLevelAndCurrExp.currLevelHaveExp = totalExp - preLevelNeedTotalExp;
                playerLevelAndCurrExp.currLevelNeed = playerGradeDefine.Upgrade;
                break;
            }
        }
        //StaticData.DebugGreen($"玩家Level_{playerLevelAndCurrExp.level}_currLevelNeed_{playerLevelAndCurrExp.currLevelNeed}_currLevelHaveExp_{playerLevelAndCurrExp.currLevelHaveExp}");
        return playerLevelAndCurrExp;
    }

    //创建附加物体
    static GameObject CreateAdditionalGo(string willLoadABName, Transform transParent)
    {
        var goAB = ABManager.GetAsset<GameObject>(willLoadABName);
        var go = ResourcesHelper.InstantiatePrefabFromABSetDefaultNotStretch(goAB, transParent);
        return go;
    }
    public static GameObject CreatePlayerImage(Transform transParent)
    {
        return CreateAdditionalGo("UIPlayerImageButton", transParent);
    }
    public static GameObject CreateCoinNav(Transform transParent)
    {
        return CreateAdditionalGo("UIGoldNav", transParent);
    }
    public static GameObject CreateDiamondNav(Transform transParent)
    {
        return CreateAdditionalGo("UIDiamondNav", transParent);
    }
    public static GameObject CreateWaterNav(Transform transParent)
    {
        return CreateAdditionalGo("UIWaterNav", transParent);
    }
    public static GameObject CreateUIChat(Transform transParent)
    {
        return CreateAdditionalGo("ChatFrame", transParent);
    }
    //创建飘字提示
    public static void CreateToastTips(string content)
    {
        var prefab = ABManager.GetAsset<GameObject>("UIToastTip");
        var parent = UIRoot.instance.GetUIRootCanvas().transform;
        var go = ResourcesHelper.InstantiatePrefabFromABSetDefault(prefab, parent);
        go.GetComponent<UIToastTip>().Init(content);
    }
    /// <summary>
    /// 打开仓库
    /// </summary>
    /// <param name="type"> 0:种子 1：果实 2：道具 3：装饰</param>
    /// <returns></returns>
    public static async UniTask OpenWareHouse(int type = 0)
    {
        GameObject obj = await UIComponent.CreateUIAsync(UIType.Warehouse);
        WarehouseController warehouseController = obj.GetComponent<WarehouseController>();
        warehouseController.Show(type);
        StaticData.DataDot(DotEventId.OpenWarehouse);
    }
    /// <summary>
    /// 打开聊天界面
    /// </summary>
    /// <returns></returns>
    public static async UniTask OpenChatPanel()
    {
        GameObject obj = await UIComponent.CreateUIAsync(UIType.ChatPanel);
        ChatPanelController chatPanelController = obj.GetComponent<ChatPanelController>();
        chatPanelController.InitialPanel();
        StaticData.DataDot(DotEventId.OpenChat);
    }
    /// <summary>
    /// 打开私聊界面
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public static async UniTask OpenPrivateChat(long uid, int iconId, string iconUrl, string playName, int experience)
    {
        GameObject obj = await UIComponent.CreateUIAsync(UIType.ChatPanel);
        ChatPanelController chatPanelController = obj.GetComponent<ChatPanelController>();
        chatPanelController.InitialPanel();
        ChatPanelController._Instance.OpenPrivateChat(uid, iconId, iconUrl, playName, experience);
    }
    /// <summary>
    /// 打开详情面板
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public static async UniTask OpenChatRoleInformationPanel(ChatInfo data, RectTransform tageRect, RectTransform AstrictScope)
    {
        GameObject obj = await UIComponent.CreateUIAsync(UIType.ChatRoleInformationPanel);
        ChatRoleInformationPanelController chatRoleInformationPanelController = obj.GetComponent<ChatRoleInformationPanelController>();
        chatRoleInformationPanelController.Show(data, tageRect, AstrictScope);
    }
    /// <summary>
    /// 创建聊天小图标
    /// </summary>
    /// <returns></returns>
    public static async UniTask CreateChatMini()
    {
        GameObject obj = await UIComponent.CreateUIAsync(UIType.ChatMini, true);
        ChatMiniController chatMiniController = obj.GetComponent<ChatMiniController>();
        chatMiniController.Initial();
    }
    /// <summary>
    /// 创建聊天弹幕
    /// </summary>
    /// <returns></returns>
    public static async UniTask CreateChatBulletScreen()
    {
        GameObject obj = await UIComponent.CreateUIAsync(UIType.ChatBulletScreen, true);
        ChatBulletScreenController chatBulletScreenController = obj.GetComponent<ChatBulletScreenController>();
    }

    /// <summary>
    /// 打开宝箱领取界面
    /// </summary>
    /// <param name="awardIds">奖品id集合</param>
    /// <param name="gameItemId">宝箱的配置道具id</param>
    public static void OpenReceiveAward(List<CSWareHouseStruct> awardIds, int gameItemId, Action endAction = null)
    {
        var packDefine = StaticData.configExcel.Package.Find(x => x.BoxID == gameItemId);
        int treasureChestId = packDefine.ID;
        GameObject gameObject = UIComponent.CreateUI(UIType.ReceiveAward);
        ReceiveAward receiveAward = gameObject.GetComponent<ReceiveAward>();
        receiveAward.ShowPrize(awardIds, treasureChestId, endAction);
    }
    /// <summary>
    /// 开启宝箱解锁界面
    /// </summary>
    /// <param name="id"></param>
    /// <param name="KeyUnlock"></param>
    /// <param name="freeUnlock"></param>
    /// <param name="speedAction"></param>
    public static void OpenTreasureUnlock(int id, Action KeyUnlock, Action<Action<long>> freeUnlock, Action<Action<long>> speedAction)
    {
        GameObject gameObject = UIComponent.CreateUI(UIType.GiftBoxUnlocking);
        GiftBoxUnlockingController giftBoxUnlockingController = gameObject.GetComponent<GiftBoxUnlockingController>();
        giftBoxUnlockingController.ShowUnlock(id, KeyUnlock, freeUnlock, speedAction);
    }
    /// <summary>
    /// 打开宝箱加速界面
    /// </summary>
    /// <param name="id"></param>
    /// <param name="timeStamp"></param>
    /// <param name="speedUpAction"></param>
    public static void OpenTreasureSpeedUp(int id, long timeStamp, Action<Action<long>> speedAction)
    {
        GameObject gameObject = UIComponent.CreateUI(UIType.GiftBoxUnlocking);
        GiftBoxUnlockingController giftBoxUnlockingController = gameObject.GetComponent<GiftBoxUnlockingController>();
        giftBoxUnlockingController.ShowSpeedUp(id, timeStamp, speedAction);
    }
    /// <summary>
    /// 打开任务面板
    /// </summary>
    /// <returns></returns>
    public static async UniTask OpenTaskPanel()
    {
        GameObject obj = await UIComponent.CreateUIAsync(UIType.TaskPanel);
        TaskPanelController taskPanelController = obj.GetComponent<TaskPanelController>();
        taskPanelController.Show();
    }
    #region 大富翁

    /// <summary>
    /// 获取大富翁地图列表
    /// </summary>
    /// <returns></returns>
    public static List<int> GetZillionaireMapList()
    {
        List<int> maps = new List<int>();
        var strings = playerInfoData.userInfo.ZillionaireMap.Split(new char[] { ',' });
        foreach (var str in strings)
        {
            int mapID = str.ToInt32();
            if (!maps.Contains(mapID))
                maps.Add(mapID);
        }
        return maps;
    }

    /// <summary>
    /// 解锁大富翁地图 更新地图数据
    /// </summary>
    /// <param name="mapID"></param>
    public static void UnlockZillionaireMap(int mapID)
    {
        playerInfoData.userInfo.ZillionaireMap = playerInfoData.userInfo.ZillionaireMap + "," + mapID.ToString();
    }

    /// <summary>
    /// 获取大富翁地图进入次数
    /// </summary>
    /// <returns></returns>
    public static int GetZillionaireMapEnterCount()
    {
        return playerInfoData.userInfo.ZillionaireCount;
    }

    /// <summary>
    /// 更新大富翁地图进入次数
    /// </summary>
    /// <param name="updateNum"> 需要更新的值 </param>
    /// <param name="isClear">是否需要清空</param>
    public static void UpdateZillionaireMapEnterCount(int updateNum, bool isClear = false)
    {
        if (isClear)
        {
            playerInfoData.userInfo.ZillionaireCount = 0;
        }
        else
        {
            playerInfoData.userInfo.ZillionaireCount += updateNum;
        }
    }

    /// <summary>
    /// 获取角色列表
    /// </summary>
    /// <returns></returns>
    public static List<int> GetRoleList()
    {
        List<int> roles = new List<int>();
        foreach (var role in playerInfoData.userInfo.RoleSInfo)
        {
            roles.Add(role.RoleId);
        }
        return roles;
    }

    /// <summary>
    /// 获取默认角色
    /// </summary>
    /// <returns></returns>
    public static int GetDefRole()
    {
        foreach (var role in playerInfoData.userInfo.RoleSInfo)
        {
            if (role.PitchOn)
                return role.RoleId;
        }
        return -1;
    }

    /// <summary>
    /// 打开 大富翁界面
    /// </summary>
    public static async UniTask OpenMonopoly()
    {
        //加载场景
        await SceneManagerComponent._instance.ChangeSceneAsync(EnumScene.Zillionaire);
        //播放背景音乐
        GameSoundPlayer.Instance.PlayBgMusic(MusicHelper.BgMusicRichManHome);
        StaticData.SetSceneState(SceneState.RichMan);

        //大富翁富翁界面管理器 是否加载出来
        while (ZillionaireUIManager._instance == null)
        {
            Debug.Log("等待大富翁界面管理器加载完成！");
            await UniTask.WaitForEndOfFrame();
        }
        //展示主界面
        await ZillionaireUIManager._instance.EnterHomePage();

    }

    /// <summary>
    /// 打开购买体力界面
    /// </summary>
    public static async void OpenBuyPower(Action<int, int> callbakc, Action cancelBuy)
    {
        GameObject obj = await UIComponent.CreateUIAsync(UIType.UIBuyPower);
        obj.GetComponent<UIBuyPowerController>().InitValue(callbakc, cancelBuy);
    }

    /// <summary>
    /// 打开结算界面
    /// </summary>
    /// <param name="rewards"></param>
    public static async void OpenSettlement(Dictionary<int, int> rewards)
    {
        //播放音效获得奖励
        GameSoundPlayer.Instance.PlaySoundEffect(MusicHelper.SoundEffectEarnRewards);

        GameObject obj = await UIComponent.CreateUIAsync(UIType.UISettlement);
        obj.GetComponent<UISettlement>().Initial(rewards);
    }

    /// <summary>
    /// 打开大富翁 3选1 游戏 界面
    /// </summary>
    /// <param name="lotterNum"></param>
    /// <param name="CloseCallback"></param>
    public static async void OpenGuess(int lotterNum, Action CloseCallback)
    {
        GameObject obj = await UIComponent.CreateUIAsync(UIType.UIGuess);
        obj.GetComponent<UIGuessController>().InitValue(lotterNum, CloseCallback);
    }

    /// <summary>
    /// 打开大富翁 娃娃机 游戏 界面
    /// </summary>
    /// <param name="lotterNum"></param>
    /// <param name="CloseCallback"></param>
    public static async void OpenCraneMachine(int lotterNum, Action CloseCallback)
    {
        GameObject obj = await UIComponent.CreateUIAsync(UIType.UICraneMachine);
        obj.GetComponent<UICraneMachineController>().InitValue(lotterNum, CloseCallback);
    }

    /// <summary>
    /// 获得奖励展示界面
    /// </summary>
    /// <param name="lotterNum"></param>
    /// <param name="CloseCallback"></param>
    public static async void OpenEarnRewards(Sprite icon, string name, int num = 1)
    {
        //播放音效获得奖励
        GameSoundPlayer.Instance.PlaySoundEffect(MusicHelper.SoundEffectEarnRewards);

        GameObject obj = await UIComponent.CreateUIAsync(UIType.UIEarnRewards);
        obj.GetComponent<UIEarnRewardsController>().InitValue(icon, name, num);
    }

    #endregion


    /// <summary>
    /// 打开邮箱
    /// </summary>
    /// <returns></returns>
    public static async UniTask OpenMailbox()
    {
        GameObject obj = await UIComponent.CreateUIAsync(UIType.Mailbox);
        MailboxController mailboxController = obj.GetComponent<MailboxController>();
        mailboxController.Show();
    }
    /// <summary>
    /// 获取仓库中的道具 入场卷数量
    /// </summary>
    /// <returns></returns>
    public static int GetWareHouseAdmission()
    {
        var item = GetWareHouseItem(StaticData.configExcel.GetVertical().AdmissionGoodsId);
        if (item == null)
            return 0;
        return item.GoodNum;
    }

    /// <summary>
    /// 更新仓库中的道具 入场卷数量
    /// </summary>
    /// <param name="itemNum"> 大于0+ 小于0-</param>
    public static void UpdateWareHouseAdmission(int itemNum)
    {
        UpdateWareHouseItem(StaticData.configExcel.GetVertical().AdmissionGoodsId, itemNum);
    }
    /// <summary>
    /// 根据种子id获取种子收获经验
    /// </summary>
    /// <param name="seedId">种子id</param>
    /// <returns></returns>
    public static int GetSeedExperience(int seedId)
    {
        int GainExperience = 0;
        for (int i = 0; i < StaticData.configExcel.ManorCrop.Count; i++)
        {
            ManorCropDefine data = StaticData.configExcel.ManorCrop[i];
            if (data.IdSeed == seedId)
            {
                GainExperience = data.GainExperience;
                return GainExperience;
            }
        }
        return GainExperience;
    }
    /// <summary>
    /// 根据种子id获取种子成熟时间
    /// </summary>
    /// <param name="seedId">种子id</param>
    /// <returns></returns>
    public static string GetSeedRipeningTime(int seedId)
    {
        string timeStr = "";
        List<string> strs = new List<string>();
        float ripeningTime = 0;
        for (int i = 0; i < StaticData.configExcel.ManorCrop.Count; i++)
        {
            ManorCropDefine data = StaticData.configExcel.ManorCrop[i];
            if (data.IdSeed == seedId)
            {
                ripeningTime = ripeningTime + data.GerminationPeriod + data.GrowUp + data.AdultnessTime;
                //时
                int hour = 0;
                //分
                int minute = 0;

                if (ripeningTime >= 60)
                {
                    minute = (int)ripeningTime / 60;
                    ripeningTime = (int)ripeningTime % 60;
                }
                if (minute >= 60)
                {
                    hour = minute / 60;
                    minute = minute % 60;
                    if (minute > 0)
                    {
                        timeStr = hour + StaticData.GetMultilingual(120098) + minute + StaticData.GetMultilingual(120099);
                    }
                    else
                    {
                        timeStr = hour + StaticData.GetMultilingual(120098);
                    }

                }
                else
                {
                    timeStr = minute.ToString() + StaticData.GetMultilingual(120099);
                }

                return timeStr;

            }
        }
        return timeStr;
    }
    /// <summary>
    /// 根据种子id获取种子结果数量
    /// </summary>
    /// <param name="seedId">种子id</param>
    /// <returns></returns>
    public static int GetSeedFruitNumber(int seedId)
    {
        int FruitNumber = 0;
        for (int i = 0; i < StaticData.configExcel.ManorCrop.Count; i++)
        {
            ManorCropDefine data = StaticData.configExcel.ManorCrop[i];
            if (data.IdSeed == seedId)
            {
                FruitNumber = data.ResultsNumber;
                return FruitNumber;
            }
        }
        return FruitNumber;
    }
    /// <summary>
    /// 根据种子id获取果实单价
    /// </summary>
    /// <param name="seedId">种子id</param>
    /// <returns></returns>
    public static int GetSeedFruitPrice(int seedId)
    {
        int FruitNumber = 0;
        for (int i = 0; i < StaticData.configExcel.ManorCrop.Count; i++)
        {
            ManorCropDefine data = StaticData.configExcel.ManorCrop[i];
            if (data.IdSeed == seedId)
            {
                FruitNumber = WarehouseTool.GetGameItemData(data.IdGainGameItem).PriceSell[0].Price;

                return FruitNumber;
            }
        }
        return FruitNumber;
    }
    /// <summary>
    /// 根据种子id获取种子预估收益
    /// </summary>
    /// <param name="seedId">种子id</param>
    /// <returns></returns>
    public static int GetSeedEstimateTheValueOf(int seedId)
    {
        int estimateTheValueOf = 0;
        for (int i = 0; i < StaticData.configExcel.ManorCrop.Count; i++)
        {
            ManorCropDefine data = StaticData.configExcel.ManorCrop[i];
            if (data.IdSeed == seedId)
            {
                int number = data.ResultsNumber;
                int price = WarehouseTool.GetGameItemData(data.IdGainGameItem).PriceSell[0].Price;

                estimateTheValueOf = number * price;
                return estimateTheValueOf;
            }
        }
        return estimateTheValueOf;
    }
    public static void TiggerUnLockArea(int willUnLockRegionId)
    {
        //解锁
        CSUnlockArea csUnlockArea = new CSUnlockArea()
        {
            AreaId = willUnLockRegionId,
            State = AreaState.NoUnlock//上一个阶段
        };
        ManorProtocalHelper.UnlockRegion(csUnlockArea, (regionUnLockDialogBeginSucc) =>
        {
            StaticData.DebugGreen($"开始对话返回状态：{regionUnLockDialogBeginSucc.State.ToString()}");
            //对话状态开始
            if (regionUnLockDialogBeginSucc.State == AreaState.Conversation)
            {
                ManorRegionComponent[] regionComs = Root2dSceneManager._instance.GetAllManorRegionComponent();
                for (int i = 0; i < regionComs.Length; i++)
                {
                    if (regionComs[i].regionId == willUnLockRegionId)
                    {
                        regionComs[i].BeginDialog();
                    }
                }
            }
        });
    }

    /// <summary>
    /// 服务器返回数据 更新背包道具
    /// </summary>
    /// <param name="sCBuyProp"></param>
    public static void UpdateBackpackProps(SCBuyProp sCBuyProp)
    {
        foreach (var item in sCBuyProp.CurrencyInfo)
        {
            UpdateWareHouseItems(item.GoodsId, (int)item.Count);
        }
    }


    #region 升级


    /// <summary>
    /// 请求升级
    /// </summary>
    public static void RequestUpgrade(int willUnLockRegionId = -1)
    {
        //if (WebSocketComponent._instance == null)
        //    return;

        //Debug.Log("******RequestUpgrade 需要 请求升级！！！");

        ////判断消息是否已经入队了
        //if (WebSocketComponent._instance.FindCodeMsg(OpCodeType.UserUpGrage))
        //    return;

        Debug.Log("RequestUpgrade 请求升级！");
        ZillionaireToolManager.NotifyServerRequestUpgrade(RequestUpgradeSuccess, RequestUpgradeFailed, willUnLockRegionId);
    }

    /// <summary>
    /// 请求升级成功
    /// </summary>
    /// <param name="lastLv"></param>
    /// <param name="curLv"></param>
    public static async void RequestUpgradeSuccess(int lastLv, int curLv, int willUnLockRegionId = -1)
    {
        //不在庄园内 不弹出升级
        if (!UIComponent.IsHaveUI(UIType.UIManor) || (UIComponent.IsHaveUI(UIType.UISceneLoading) && UIComponent.GetExistUI(UIType.UISceneLoading).activeInHierarchy))
        {
            return;
        }
        //播放音效升级
        GameSoundPlayer.Instance.PlaySoundEffect(MusicHelper.SoundEffectUpgrade);

        Debug.Log("请求升级 成功");
        GameObject ui = await UIComponent.CreateUIAsync(UIType.UILevelUp, true);
        //这个时间比较长，可能在庄园内点击了其它界面，关掉
        CloseOtherUINotManor();
        ui.GetComponent<UILevelUpController>().InitValue(lastLv, curLv, willUnLockRegionId);
    }

    private static void CloseOtherUINotManor()
    {
        UIComponent.RemoveUI(UIType.Warehouse);
        UIComponent.RemoveUI(UIType.UIShop);
        UIComponent.RemoveUI(UIType.TaskPanel);
        UIComponent.RemoveUI(UIType.UIPersonalInformation);
        UIComponent.RemoveUI(UIType.UIRecharge);
        UIComponent.RemoveUI(UIType.UIOpenFunctionTips);
        UIComponent.RemoveUI(UIType.UIOpenFunction);
        UIComponent.RemoveUI(UIType.UICommonPopupTips);
        UIComponent.RemoveUI(UIType.ChatPanel);
        UIComponent.RemoveUI(UIType.UIDeal);
        UIComponent.RemoveUI(UIType.UICommonReceiveAwardTips);
    }

    /// <summary>
    /// 请求升级失败
    /// </summary>
    private static void RequestUpgradeFailed()
    {
        Debug.Log("请求升级 失败");
    }

    /// <summary>
    /// 升级解锁章节
    /// </summary>
    /// <param name="chapterID"></param>
    public static async void OpenChapterUnlock(int chapterID)
    {
        GameObject ui = await UIComponent.CreateUIAsync(UIType.UIChapterUnlock, true);
        ui.GetComponent<UIChapterUnlockController>().InitValue(chapterID);
    }

    /// <summary>
    /// 升级解锁种子
    /// </summary>
    public static async void OpenSeedUnlock(List<GameItemDefine> unlockCrops)
    {
        GameObject ui = await UIComponent.CreateUIAsync(UIType.UISeedUnlock, true);
        ui.GetComponent<UISeedUnlockController>().InitValue(unlockCrops);
    }


    #endregion
    #region 充值
    /// <summary>
    /// 打开充值界面
    /// </summary>
    /// <param name="channel">频道</param>
    /// <param name="OnReturnClickCallBack">返回回调，用来刷新调整之前的界面信息</param>
    public static async UniTask OpenRechargeUI(int channel = 0, Action OnReturnClickCallBack = null)
    {
        //功能是否开启
        if (channel == 1)//钻石功能未开放
        {
            if (!StaticData.IsOpenFunction(20001))
            {
                return;
            }
        }
        GameObject ui = await UIComponent.CreateUIAsync(UIType.UIRecharge);
        switch (channel)
        {
            case 0:
                ui.GetComponent<UIRechargeComponent>().ShowRechargeGoldUI(OnReturnClickCallBack);
                break;
            case 1:
                ui.GetComponent<UIRechargeComponent>().ShowRechargeDiamondUI(OnReturnClickCallBack);
                break;
            default:
                ui.GetComponent<UIRechargeComponent>().ShowRechargeGoldUI(OnReturnClickCallBack);
                break;
        }
    }
    #endregion

    #region 商店
    public static async UniTask OpenShopUI(int channel = 0, Action OnCloseCallBack = null)
    {
        //功能没开放，不能调整商店
        if (!StaticData.IsOpenFunction(10008))
        {
            return;
        }
        GameObject ui = await UIComponent.CreateUIAsync(UIType.UIShop);
        switch (channel)
        {
            case 0:
                ui.GetComponent<UIShopComponent>().ShowSeedShopUI(OnCloseCallBack);
                break;
            case 1:
                ui.GetComponent<UIShopComponent>().ShowItemShopUI(OnCloseCallBack);
                break;
            case 2:
                ui.GetComponent<UIShopComponent>().ShowOrnamentShopUI(OnCloseCallBack);
                break;
            default:
                //ui.GetComponent<UIShopComponent>().ShowSeedShopUI(OnCloseCallBack);
                ui.GetComponent<UIShopComponent>().ShowOrnamentShopUI(OnCloseCallBack);
                break;
        }
    }
    #endregion

    #region 通用界面 ui通用弹窗提示

    /// <summary>
    /// 打开 购买+货币显示 弹窗提示
    /// </summary>
    /// <param name="desc"> 描述 </param>
    /// <param name="icon"> 货币icon</param>
    /// <param name="num"> 消耗货币数量 </param>
    /// <param name="onClick"></param>
    /// <param name="onCancel"></param>
    /// <param name="idLocalizeButCancel">取消按钮 国际化id</param>
    /// <param name="btnPosExchange">按钮位置交换</param>
    public static async void OpenCommonBuyTips(string desc, Sprite icon, int num, Action onClick, Action onCancel = null, int idLocalizeButCancel = -1, bool btnPosExchange = false)
    {
        GameObject buyTipsUI = await UIComponent.CreateUIAsync(UIType.UICommonPopupTips);
        buyTipsUI.GetComponent<UICommonPopupTipsControl>().InitialButtonBuyIconAndNum(desc, icon, num, onClick, onCancel, idLocalizeButCancel, btnPosExchange);
    }

    /// <summary>
    /// 打开 纯提示界面 弹窗提示
    /// </summary>
    /// <param name="desc"> 描述 </param>
    /// <param name="idUILocalizeButtonName"> 确认按钮名称国际化id</param>
    /// <param name="confirmBuy">点击确认回调</param>
    /// <param name="cancelBuy">点击取消回调</param>
    /// <param name="idUILocalizeButCancelName"> 取消按钮名称国际化id</param>
    /// <param name="btnPosExchange">按钮位置交换</param>
    public static async void OpenCommonTips(string desc, int idUILocalizeButtonName, Action confirmBuy = null, Action cancelBuy = null, int idUILocalizeButCancelName = -1, bool btnPosExchange = false)
    {
        GameObject buyTipsUI = await UIComponent.CreateUIAsync(UIType.UICommonPopupTips);
        buyTipsUI.GetComponent<UICommonPopupTipsControl>().InitialButtonBuyOnlyText(desc, confirmBuy, cancelBuy, idUILocalizeButtonName, idUILocalizeButCancelName, btnPosExchange);
    }
    /// <summary>
    /// 打开 只有一个按钮的 弹窗提示
    /// </summary>
    /// <param name="desc"> 描述 </param>
    /// <param name="idUILocalizeButtonName"> 确认按钮名称国际化id</param>
    /// <param name="confirmBuy">点击确认回调</param>
    /// <param name="cancelBuy">点击取消回调</param>
    /// <param name="idUILocalizeButCancelName"> 取消按钮名称国际化id</param>
    public static async void OpenCommonThinkAgainTips(string desc, int idUILocalizeButtonName, Action confirmBuy = null)
    {
        GameObject buyTipsUI = await UIComponent.CreateUIAsync(UIType.UICommonPopupTips);
        buyTipsUI.GetComponent<UICommonPopupTipsControl>().InitialOneBtnPane(desc, confirmBuy, idUILocalizeButtonName);
    }

    /// <summary>
    /// 后端返回的错误码对应的语言提示
    /// </summary>
    /// <param name="error">服务器返回的error</param>
    public static void WebErrorCodeCommonTip(ErrorInfo error)
    {
        var defineErrorCode = StaticData.configExcel.ServerErrorCode.Find(x => x.errorCode == error.webErrorCode);
        if (defineErrorCode.isShowTips)
        {
            switch (StaticData.linguisticType)
            {
                case LinguisticType.Simplified:
                    StaticData.CreateToastTips($"{defineErrorCode.SimplifiedChinese}");
                    break;
                case LinguisticType.English:
                    StaticData.CreateToastTips($"{defineErrorCode.English}");
                    break;
                case LinguisticType.Complex:
                    StaticData.CreateToastTips($"{defineErrorCode.TraditionalChinese}");
                    break;
                default:
                    StaticData.CreateToastTips($"{defineErrorCode.SimplifiedChinese}");
                    break;
            }
        }
    }
    /// <summary>
    /// 开启通用奖励领取弹窗
    /// </summary>
    /// <param name="titelStr">弹窗标题 传空字符串隐藏标题</param>
    /// <param name="affirmBtnName">确认按钮显示文字 传空字符串隐藏确认按钮</param>
    /// <param name="cancelBtnName">取消按钮文字 传空字符串隐藏取消按钮</param>
    /// <param name="affirmAction">确认回调</param>
    /// <param name="cancelAction">取消回调</param>
    /// <param name="datas">需要展示的物品数据</param>
    public static async void OpenCommonReceiveAwardTips(string titelStr, string affirmBtnName, string cancelBtnName, Action affirmAction, Action cancelAction, List<CSWareHouseStruct> datas)
    {
        GameObject Obj = await UIComponent.CreateUIAsync(UIType.UICommonReceiveAwardTips);
        Obj.GetComponent<UICommonReceiveAwardTipsController>().Show(titelStr, affirmBtnName, cancelBtnName, affirmAction, cancelAction, datas);
    }

    #endregion

    #region 通用界面 ui通用使用提示

    /// <summary>
    /// 打开 通用物品使用提示
    /// </summary>
    /// <param name="itemID"> 物品的id</param>
    /// <param name="icon"> 物品的图片</param>
    /// <param name="itemNum">物品剩余数量</param>
    /// <param name="tips"> 使用提示</param>
    /// <param name="useDesc"> 使用描述</param>
    /// <param name="useCallback"> 使用回调</param>
    public static async void OpenCommonUseTips(int itemID, Sprite icon, int itemNum, string tips, Action<int> useCallback, string useDesc = null)
    {
        //120090 //使用
        if (string.IsNullOrEmpty(useDesc))
            useDesc = LocalizationDefineHelper.GetStringNameById(120090);
        GameObject useTipsUI = await UIComponent.CreateUIAsync(UIType.UICommonUseTips);
        useTipsUI.GetComponent<UICommonUseTipsController>().InitValue(itemID, icon, itemNum, tips, useDesc, useCallback);
    }

    #endregion

    #region 登录界面

    /// <summary>
    /// 使用的网关类型
    /// </summary>
    private static TypeGateWay _useTypeGateWay = TypeGateWay.Extranet;

    /// <summary>
    /// 打开登录界面
    /// </summary>
    public static async UniTask OpenUILogin(TypeGateWay typeGate, Action loginCallback, bool isAccountWasSqueezed = false)
    {
        _useTypeGateWay = typeGate;
        var obj = await UIComponent.CreateUIAsync(UIType.UILogin);
        obj.GetComponent<UILoginController>().InitValue(typeGate, loginCallback, isAccountWasSqueezed);
    }

    #endregion
    #region 好友
    /// <summary>
    /// 打开好友UI
    /// </summary>
    /// <param name="isManor">是否打开庄园好友</param>
    /// <returns></returns>
    public static async UniTask OpenFriend(bool isManor)
    {
        var uiFriendGO = await UIComponent.CreateUIAsync(UIType.UIFriend);
        uiFriendGO.GetComponent<UIFriendComponent>().ShowFriendUI(isManor);
    }
    public static void GetFriendListFromServer(Action OnInfoCallBack)
    {
        if (!isGetFriendList)
        {
            StaticData.playerInfoData.listFriendInfo.Clear();
            //请求好友列表
            CSEmptySCFriendList csemptyscfriendlist = new CSEmptySCFriendList();
            ProtocalManager.Instance().SendCSEmptySCFriendList(csemptyscfriendlist, (friendList) =>
            {
                isGetFriendList = true;
                if (friendList == null)
                {
                    return;
                }
                StaticData.playerInfoData.listFriendInfo.AddRange(friendList.FriendListInfo);
                OnInfoCallBack?.Invoke();
            }, (error) => { });
        }
        else
        {
            OnInfoCallBack?.Invoke();
        }
    }
    /// <summary>
    /// 打开章节UI界面
    /// </summary>
    public static async UniTask OpenChapterUI()
    {
        var uiChapterGO = await UIComponent.CreateUIAsync(UIType.UIChapter);
        uiChapterGO.GetComponent<UIChapterComponent>().OpenChapterView();
    }
    /// <summary>
    /// 打开心动时刻UI界面
    /// </summary>
    public static async UniTask OpenImpulseUI()
    {
        var uiImpulseGO = await UIComponent.CreateUIAsync(UIType.UIImpulse);
        uiImpulseGO.GetComponent<UIImpulseComponent>().OpenImpulseView();
    }

    /// <summary>
    /// 判断是否是好友关系
    /// </summary>
    /// <param name="frinedUid"></param>
    /// <returns></returns>
    public static async UniTask<bool> IsUserFriend(long frinedUid)
    {
        bool isFriend = false;
        bool actionFlag = false;
        GetFriendListFromServer(() =>
        {
            for (int i = 0; i < StaticData.playerInfoData.listFriendInfo.Count; i++)
            {
                if (StaticData.playerInfoData.listFriendInfo[i].Uid == frinedUid)
                {
                    isFriend = true;
                    break;
                }
            }
            actionFlag = true;
        });
        await UniTask.WaitUntil(() => actionFlag == true);
        return isFriend;
    }
    /// <summary>
    /// 好友是否在线
    /// </summary>
    /// <param name="frinedUid"></param>
    /// <returns></returns>
    public static bool IsFriendOnline(long frinedUid)
    {
        bool isOnline = false;
        for (int i = 0; i < StaticData.playerInfoData.listFriendInfo.Count; i++)
        {
            if (StaticData.playerInfoData.listFriendInfo[i].Uid == frinedUid)
            {
                isOnline = StaticData.playerInfoData.listFriendInfo[i].Online;
                break;
            }
        }
        return isOnline;
    }
    /// <summary>
    /// 通过uid申请添加好友
    /// </summary>
    /// <param name="frinedUid"></param>
    public static void ApplyFriendByUid(long frinedUid)
    {
        StaticData.DebugGreen($"申请添加好友~~~:{frinedUid}");
        CSApply csApply = new CSApply()
        {
            OperationUid = frinedUid
        };
        ProtocalManager.Instance().SendCSApply(csApply, (serverRes) =>
        {
            string tips = LocalizationDefineHelper.GetStringNameById(120147);
            StaticData.CreateToastTips(tips);
        }, (error) => { });
    }
    /// <summary>
    /// 获取好友头像id
    /// </summary>
    /// <param name="frinedUid"></param>
    /// <returns></returns>
    public static int GetFriendIconId(long frinedUid)
    {
        int iconId = 0;
        for (int i = 0; i < StaticData.playerInfoData.listFriendInfo.Count; i++)
        {
            if (StaticData.playerInfoData.listFriendInfo[i].Uid == frinedUid)
            {
                iconId = StaticData.playerInfoData.listFriendInfo[i].FriendImage;
                break;
            }
        }
        return iconId;
    }

    /// <summary>
    /// 获取好友数据
    /// </summary>
    /// <param name="frinedUid"></param>
    /// <returns></returns>
    public static SCFriendInfo GetFriendData(long frinedUid)
    {
        SCFriendInfo sCFriendInfo = null;
        for (int i = 0; i < StaticData.playerInfoData.listFriendInfo.Count; i++)
        {
            if (StaticData.playerInfoData.listFriendInfo[i].Uid == frinedUid)
            {
                sCFriendInfo = StaticData.playerInfoData.listFriendInfo[i];
                break;
            }
        }
        return sCFriendInfo;
    }
    #endregion

    /// <summary>
    /// RepeatedField排序
    /// </summary>
    /// <typeparam name="T">具体协议类</typeparam>
    /// <param name="repeatedField">排序数据</param>
    /// <param name="comparison">排序方法</param>
    /// <param name="isReverse">是否反序</param>
    /// <returns></returns>
    public static RepeatedField<T> RepeatedFieldSortT<T>(RepeatedField<T> repeatedField, Comparison<T> comparison, bool isReverse = false) where T : IMessage
    {
        RepeatedField<T> newRepeatedField = new RepeatedField<T>();
        List<T> repeatedListT = new List<T>();
        repeatedListT.AddRange(repeatedField);
        repeatedListT.Sort(comparison);
        if (isReverse)
            repeatedListT.Reverse();
        newRepeatedField.AddRange(repeatedListT);
        return newRepeatedField;
    }

    #region 拾取物品效果

    /// <summary>
    /// 拾取物品效果
    /// </summary>
    /// <param name="sprite">物品图片</param>
    /// <param name="scale">尺寸</param>
    public static async void OpenPickupItemEffect(Sprite sprite, float scale, Vector3 loc, Vector3 targetPos, int itemID = 0, bool isEvent = false)
    {
        var obj = await ObjectPoolManager.Instance.CreatObject(UIType.PickupItem, UIRoot.instance.GetUIRootCanvas().transform);
        obj.GetComponent<PickupItemEffect>().InitValue(sprite, scale, loc, targetPos, itemID, isEvent);
    }

    /// <summary>
    /// 拾取物品并且有数量显示 飞到对应的位置
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="scale"></param>
    /// <param name="loc"></param>
    /// <param name="itemID"></param>
    /// <param name="itemNum"></param>
    public static async void OpenPickupItemEffectNew(Sprite sprite, float scale, Vector3 loc, int itemID, int itemNum, Action<int> effectComplete)
    {
        var obj = await ObjectPoolManager.Instance.CreatObject(UIType.PickupItem, UIRoot.instance.GetUIRootCanvas().transform);
        obj.GetComponent<PickupItemEffect>().InitValue(sprite, scale, loc, itemID, itemNum, effectComplete);
    }

    #endregion

    #region 晚会

    /// <summary>
    /// 打开晚会选择房间界面
    /// </summary>
    public static async UniTask OpenUIPartyChooseRoom(SCRoomListInfo roomList)
    {
        var obj = await UIComponent.CreateUIAsync(UIType.UIPartyChooseRoom);
        obj.GetComponent<UIPartyChooseRoomController>().InitValue(roomList);
    }

    /// <summary>
    /// 进入晚会场景
    /// </summary>
    /// <param name="sCEntranceRoom"></param>
    /// <returns></returns>
    public static async UniTask EnterParty(SCEntranceRoom sCEntranceRoom)
    {
        //隐藏自己
        UIComponent.HideUI(UIType.UIPartyChooseRoom);
        //加载场景
        await SceneManagerComponent._instance.ChangeSceneAsync(EnumScene.Party);
        //播放背景音乐
        //GameSoundPlayer.Instance.PlayBgMusic(MusicHelper.BgMusicRichMan);
        //manager初始化值
        if (PartyManager._instance != null)
            PartyManager._instance.InitValue(sCEntranceRoom);
        ChatTool.EnterRoom();
        StaticData.SetSceneState(SceneState.RichMan);
    }

    /// <summary>
    /// 晚会场景回到主页
    /// </summary>
    /// <returns></returns>
    public static async UniTask PartyReturnLobby()
    {
        DebugGreen($"HomePageControl ComebackMainUI() 返回主页");
        //加载场景
        await StaticData.ToManorSelf();
        //播放大厅背景音乐
        GameSoundPlayer.Instance.PlayBgMusic(MusicHelper.BgMusicLobby);
        //UIComponent.RemoveUI(UIType.UIPartyChooseRoom);
        UIComponent.RemoveUI(UIType.UIPartyMain);
        ChatTool.QuitRoom();
        StaticData.SetSceneState(SceneState.ManorSelf);

        //重置相机位置
        if (UICameraManager._instance != null)
            UICameraManager._instance.RestoreUiCamera();

        //晚会释放结束
        if (PartyManager.IsEndParty(TimeHelper.ServerDateTimeNow))
        {
            //删除晚会选择列表
            UIComponent.RemoveUI(UIType.UIPartyChooseRoom);
        }
        else
        {
            //加载晚会选择列表
            await UIComponent.CreateUIAsync(UIType.UIPartyChooseRoom);
        }

    }

    /// <summary>
    /// 打开竞猜UI界面
    /// </summary>
    /// <returns></returns>
    public static async UniTask OpenGuess()
    {
        await UIComponent.CreateUIAsync(UIType.UIPartyGuess);
        Debug.Log("显示竞猜界面成功");
    }

    /// <summary>
    /// 关闭竞猜UI界面
    /// </summary>
    /// <returns></returns>
    public static void CloseGuess()
    {
        UIComponent.RemoveUI(UIType.UIPartyGuess);
        Debug.Log("关闭竞猜界面成功");
    }


    /// <summary>
    /// 打开竞猜结果UI界面
    /// </summary>
    /// <returns></returns>
    public static async UniTask OpenGuessResultUI()
    {
        await UIComponent.CreateUIAsync(UIType.UIPartyGuessResult);
        Debug.Log("显示竞猜结果界面成功");
    }

    /// <summary>
    /// 关闭竞猜结果UI界面
    /// </summary>
    /// <returns></returns>
    public static void CloseGuessResultUI()
    {
        UIComponent.RemoveUI(UIType.UIPartyGuessResult);
        Debug.Log("关闭竞猜结果界面成功");
    }

    #endregion

    #region 大厅角色 换装/换角色界面

    /// <summary>
    /// 获取大厅选中/默认 角色 服装   
    /// </summary>
    /// <returns></returns>
    public static int GetHallDefRoleID(ref List<int> choicesId)
    {

        foreach (var role in playerInfoData.userInfo.HallRoleInfo)
        {
            if (role.PitchOn)
            {
                choicesId.AddRange(role.ChoiceId);
                return role.RoleId;
            }
        }

        return 0;
    }

    /// <summary>
    /// 获取玩家角色的数据 
    /// </summary>
    /// <param name="roleID"></param>
    /// <returns></returns>
    public static CSHallRoleStruct GetRoleDefChoiceID(int roleID)
    {
        for (int i = 0; i < playerInfoData.userInfo.HallRoleInfo.Count; i++)
        {
            if (roleID == playerInfoData.userInfo.HallRoleInfo[i].RoleId)
                return playerInfoData.userInfo.HallRoleInfo[i];
        }
        return null;
    }

    /// <summary>
    /// 更新使用的角色和时装
    /// </summary>
    /// <param name="roleID"></param>
    /// <param name="fashionIDs"></param>
    public static void UpdateUsedRoleAndFashion(int roleID, List<int> fashionIDs)
    {
        for (int i = 0; i < playerInfoData.userInfo.HallRoleInfo.Count; i++)
        {
            if (playerInfoData.userInfo.HallRoleInfo[i].PitchOn)
                playerInfoData.userInfo.HallRoleInfo[i].PitchOn = false;
            if (roleID == playerInfoData.userInfo.HallRoleInfo[i].RoleId)
            {
                playerInfoData.userInfo.HallRoleInfo[i].PitchOn = true;
                playerInfoData.userInfo.HallRoleInfo[i].ChoiceId.Clear();
                playerInfoData.userInfo.HallRoleInfo[i].ChoiceId.AddRange(fashionIDs);
                return;
            }
        }
    }

    /// <summary>
    /// 打开换装/换角色界面
    /// </summary>
    public static async UniTask OpenUIRoleSwitching()
    {
        await StaticData.PlayManorQuitAnim();
        var obj = await UIComponent.CreateUIAsync(UIType.UIRoleSwitching);
        obj.GetComponent<UIRoleSwitchingController>().InitValue();
        await UIComponent.CreateUIAsync(UIType.UIRoleSwitchingBG, false, true);
        ////角色移动换装页位置 + 角色缩放
        //await UniTask.WaitForEndOfFrame();
        //HallRoleManager.Instance.UpdateRoleToSwitching();
    }

    /// <summary>
    /// 隐藏换装/换角色界面
    /// </summary>
    public static async void HideUIRoleSwitching()
    {
        UIComponent.RemoveUI(UIType.UIRoleSwitching);
        UIComponent.RemoveUI(UIType.UIRoleSwitchingBG);
        await StaticData.ToManorSelf();
        //角色回到主页位置 + 角色缩放
        //await UniTask.WaitForEndOfFrame();
        //HallRoleManager.Instance.UpdateRoleToHall();

    }

    /// <summary>
    /// 打开分享界面
    /// </summary>
    /// <param name="sprite"></param>
    public static async void OpenUIShare(Sprite sprite)
    {
        var obj = await UIComponent.CreateUIAsync(UIType.UIShare);
        obj.GetComponent<UIShareController>().InitValue(sprite);
    }

    /// <summary>
    /// 关闭分享界面
    /// </summary>
    public static void CloseUIShare()
    {
        UIComponent.RemoveUI(UIType.UIShare);
    }

    /// <summary>
    /// 打开快捷购买时装
    /// </summary>
    /// <param name="roleFashionID"></param>
    /// <param name="isNeedWear"></param>
    /// <param name="buyCallback"></param>
    public static async void OpenUIQuickBuyFashion(int roleFashionID, bool isNeedWear, Action buyCallback)
    {
        var obj = await UIComponent.CreateUIAsync(UIType.UIQuickBuyFashion);
        obj.GetComponent<UIQuickBuyFashionController>().InitValue(roleFashionID, isNeedWear, buyCallback);
    }

    /// <summary>
    /// 关闭快捷购买时装
    /// </summary>
    public static void CloseUIQuickBuyFashion()
    {
        UIComponent.RemoveUI(UIType.UIQuickBuyFashion);
    }

    #endregion

    #region 更新昵称和icon

    /// <summary>
    /// 更新昵称和头像
    /// </summary>
    /// <param name="nick"></param>
    /// <param name="icon"></param>
    public static void UpdateNickAndIcon(string nick = null, int icon = 0, string iconPath = null)
    {
        if (!string.IsNullOrEmpty(nick))
            playerInfoData.userInfo.Name = nick;
        if (icon != 0)
            playerInfoData.userInfo.Image = icon;
        //if (!string.IsNullOrEmpty(iconPath))
        //    playerInfoData.userInfo.ImageAddress = iconPath;
    }


    /// <summary>
    /// 通知服务器请求昵称和头像验证
    /// </summary>
    /// <param name="successAction"></param>
    /// <param name="failedAction"></param>
    public static void NotifyServerRequestNiceAndIconVerification(string nick, int iconId, string imageAddress, Action<bool> successAction)
    {
        CSSetBasicsInfo cSSetBasicsInfo = new CSSetBasicsInfo();
        if (!string.IsNullOrEmpty(nick))
            cSSetBasicsInfo.Name = nick;
        if (iconId != 0)
            cSSetBasicsInfo.Image = iconId;
        if (string.IsNullOrEmpty(imageAddress))
            cSSetBasicsInfo.ImageAddress = imageAddress;

        ProtocalManager.Instance().SendCSSetBasicsInfo(cSSetBasicsInfo, (SCEmtpySetBasicsInfo sCEmtpySetBasicsInfo) =>
        {
            Debug.Log("通知服务器请求昵称和头像验证成功！");
            successAction?.Invoke(true);
        },
        (ErrorInfo er) =>
        {
            Debug.Log("通知服务器请求昵称和头像验证失败！Error：" + er.ErrorMessage);
            successAction?.Invoke(false);
        });
    }

    #endregion

    #region 系统公告


    /// <summary>
    /// 通知服务器使用喇叭道具
    /// </summary>
    /// <param name="mes"></param>
    public static void NotifyServerUseHornMessage(int hornID, string mes, Action<bool> callback)
    {
        CSUseWarehouseGoods cSUseWarehouseGoods = new CSUseWarehouseGoods();
        cSUseWarehouseGoods.GoodsId = hornID;
        cSUseWarehouseGoods.GoodNum = 1;
        cSUseWarehouseGoods.Message = mes;

        ProtocalManager.Instance().SendCSUseWarehouseGoods(cSUseWarehouseGoods, (SCUseWarehouseGoods sCUseWarehouseGoods) =>
        {
            Debug.Log("通知服务器使用喇叭道具成功！");
            PlayerNotificationsConversion(playerInfoData.userInfo.Name, mes);
            callback?.Invoke(true);
        },
        (ErrorInfo er) =>
        {
            Debug.Log("通知服务器使用喇叭道具失败！Error：" + er.ErrorMessage);
            callback?.Invoke(false);
        });
    }

    /// <summary>
    /// 公告 角色公告转换
    /// </summary>
    /// <param name="name"></param>
    /// <param name="mes"></param>
    private static void PlayerNotificationsConversion(string name, string mes)
    {
        name = "<color = red>" + name + "</color>";
        string symbol = "<color = red>" + GetMultilingual(120139) + "</color>";//120139
        mes = name + symbol + mes;

        SystemNotification system = new SystemNotification();
        system.NoticeSource = PushNoticeSource.PlayerType;
        system.Desc = mes;

        EnqueueSystemNotification(system);
    }

    /// <summary>
    /// 公告 系统公告转换
    /// </summary>
    /// <param name="name"></param>
    /// <param name="mes"></param>
    private static void SystemNotificationsConversion(SCNotePushMess sCNotePush)
    {
        string name = "<color = red>" + sCNotePush.Account + "</color>";
        string path = "<color = red>" + sCNotePush.GoodsSource + "</color>";
        string itemName = string.Empty;
        string dese = string.Empty;
        for (int i = 0; i < sCNotePush.GoodsId.Count; i++)
        {
            itemName = GetMultilingual(configExcel.GetGameItemByID(sCNotePush.GoodsId[i]).ItemName);
            itemName = "<color = red>" + itemName + "</color>";

            dese = GetMultilingual(120148);
            dese.Replace("*A", name);
            dese.Replace("*B", path);
            dese.Replace("*C", itemName);

            SystemNotification system = new SystemNotification();
            system.NoticeSource = sCNotePush.NoticeSource;
            system.Desc = dese;
            EnqueueSystemNotification(system);
        }
    }


    /// <summary>
    /// 推送的系统公告
    ///</summary>
    public static void PushSystemNotification(IMessage msg)
    {
        var data = msg as SCNotePushMess;

        if (data == null)
            return;
        switch (data.NoticeSource)
        {
            case PushNoticeSource.PlayerType:
                PlayerNotificationsConversion(data.Account, data.VoiceMess);
                break;
            case PushNoticeSource.SystemType:
                SystemNotificationsConversion(data);
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// 系统公告
    /// </summary>
    private static Queue<SystemNotification> _systemNotifications = new Queue<SystemNotification>();

    /// <summary>
    /// 添加系统公告
    /// </summary>
    /// <param name="noticeSource"></param>
    /// <param name="mes"></param>
    public static void EnqueueSystemNotification(SystemNotification system)
    {
        _systemNotifications.Enqueue(system);
    }

    /// <summary>
    /// 获取最先一条数据 系统公告 
    /// </summary>
    /// <returns></returns>
    public static SystemNotification DequeueSystemNotification()
    {
        if (_systemNotifications.Count > 0)
            return _systemNotifications.Dequeue();
        return null;
    }

    /// <summary>
    /// 打开公告界面
    /// </summary>
    public static async void OpenUISystemNotification()
    {
        //await UIComponent.CreateUIAsync(UIType.UISystemNotification, true);
    }

    /// <summary>
    /// 移除公告界面
    /// </summary>
    public static void RemoveUISystemNotification()
    {
        UIComponent.RemoveUI(UIType.UISystemNotification);
    }

    #endregion

    #region 账号被被挤掉

    /// <summary>
    /// 返回登录界面
    /// </summary>
    /// <param name="isAccountWasSqueezed"> 是否为账号被挤掉</param>
    public async static void ReturnUILogin(bool isAccountWasSqueezed = false)
    {
        UIComponent.RemoveUIEnterLogin();
        //设置相机
        UICameraManager._instance.SetDefault();
        //判断是否是在大厅界面
        await SceneManagerComponent._instance.ChangeSceneAsync(EnumScene.Empty);
        RemoveUISystemNotification();
        //打开登录界面
        await OpenUILogin(_useTypeGateWay, null, isAccountWasSqueezed);
        //播放大厅背景音乐
        GameSoundPlayer.Instance.PlayBgMusic(MusicHelper.BgMusicLogin);
        StaticData.SetSceneState(SceneState.ManorSelf);
    }

    #endregion

    #region SCBuyGoodsStruct to CSWareHouseStruct
    public static CSWareHouseStruct SCBuyGoodsStructToCSWareHouseStruct(SCBuyGoodsStruct scBuyGoodsStruct)
    {
        CSWareHouseStruct cSWareHouseStruct = new CSWareHouseStruct() { GoodId = scBuyGoodsStruct.GoodsId, GoodNum = scBuyGoodsStruct.Count };
        return cSWareHouseStruct;
    }
    #endregion

    #region 升级奖励入库

    /// <summary>
    /// 升级奖励入库
    /// </summary>
    /// <param name="rewards">物品奖励</param>
    /// <param name="addWarehouseCount">仓库格子</param>
    /// <param name="maxChapter">章节开启</param>
    public static void LevelUpRewardEntrance(List<GoodIDCount> rewards, int addWarehouseCount, int maxChapter)
    {
        for (int i = 0; i < rewards.Count; i++)
        {
            UpdateWareHouseItem(rewards[i].ID, (int)rewards[i].Count);
        }
        playerInfoData.userInfo.WarehouseCount += addWarehouseCount;
        playerInfoData.userInfo.UnlockSectionId = maxChapter;
    }

    #endregion

    #region 功能开放

    /// <summary>
    /// 通知界面隐藏或者移除
    /// </summary>
    public static void NotifyUIHideOrRemove(string lastName)
    {
        Debug.Log("通知you界面隐藏或者移除");
        if (UIType.UIEventMask != lastName && UIType.UIWait != lastName && UIType.UISceneLoading != lastName)
        {
            DetectionNeedOpenFunction();
        }

    }

    /// <summary>
    /// 功能是否开放
    /// </summary>
    /// <param name="funcID"></param>
    /// <returns></returns>
    public static bool IsOpenFunction(int funcID, bool isNeedTips = true)
    {
        var openFunc = configExcel.GetOpenFunctionByFunctionID(funcID);
        if (openFunc == null)
            return true;

        if (openFunc.IsInDevelopment)
        {
            if (isNeedTips)
                OpenOpenFunctionTips();
            return false;
        }
        else if (openFunc.OpenLevel > GetPlayerLevelAndCurrExp().level)
        {
            if (isNeedTips)
                OpenOpenFunctionTips(openFunc.OpenLevel);
            return false;
        }

        return true;
    }

    /// <summary>
    /// 打开功能开放提示 
    /// needLevel = 0 //拼命开发中
    /// needLevel !=0 //x等级开放
    /// </summary>
    /// <param name="needLevel"></param>
    public static async void OpenOpenFunctionTips(int needLevel = 0)
    {
        var obj = await UIComponent.CreateUIAsync(UIType.UIOpenFunctionTips);
        obj.GetComponent<UIOpenFunctionTipsController>().InitValue(needLevel);
    }

    /// <summary>
    /// 检测是否需要打开功能开启提示
    /// </summary>
    public static void DetectionNeedOpenFunction()
    {
        //
        if (configExcel == null || configExcel.OpenFunction == null || playerInfoData == null)
        {
            return;
        }

        if (playerInfoData.userInfo == null)
            return;

        if (string.IsNullOrEmpty(playerInfoData.userInfo.Name))
            return;

        //1.获取全部功能开启
        for (int i = 0; i < configExcel.OpenFunction.Count; i++)
        {
            //2.剔除不需要提示的
            if (!configExcel.OpenFunction[i].IsOpenTips)
            {
                continue;
            }

            //等级判定
            if (GetPlayerLevelAndCurrExp().level < configExcel.OpenFunction[i].OpenLevel)
            {
                continue;
            }

            //3.剔除已经提示的
            if (playerInfoData.CurLocalSaveData != null && playerInfoData.CurLocalSaveData.FinishedOpenFunIDs != null)
            {
                if (playerInfoData.CurLocalSaveData.FinishedOpenFunIDs.Contains(configExcel.OpenFunction[i].FunctionID))
                {
                    continue;
                }
            }

            //4.判断条件是否满足
            //上一个开启功能
            if (configExcel.OpenFunction[i].TipsNeedLastFuncID != 0)
            {
                if (playerInfoData.CurLocalSaveData != null && playerInfoData.CurLocalSaveData.FinishedOpenFunIDs != null)
                {
                    continue;
                }
                if (!playerInfoData.CurLocalSaveData.FinishedOpenFunIDs.Contains(configExcel.OpenFunction[i].TipsNeedLastFuncID))
                {
                    continue;
                }

            }

            //所在界面
            if (string.IsNullOrEmpty(configExcel.OpenFunction[i].TipsNeedLocationUI))
            {
                if (!UIComponent.IsHaveUI(configExcel.OpenFunction[i].TipsNeedLocationUI))
                {
                    continue;
                }
                var ui = UIComponent.GetExistUI(configExcel.OpenFunction[i].TipsNeedLocationUI);
                if (!UIIsLastSibling(ui.transform))
                {
                    continue;
                }
            }
            //5.打开功能开启提示界面
            OpenOpenFunction(configExcel.OpenFunction[i]);
            //保存提示
            StaticData.SaveOpenFunctionTips(configExcel.OpenFunction[i].FunctionID);
            return;
        }
    }

    /// <summary>
    /// ui是否为最上层且开启
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    public static bool UIIsLastSibling(Transform trans)
    {
        if (!trans.gameObject.activeInHierarchy)
        {
            return false;
        }
        if (UIIsOpen(UIType.UISeedUnlock) || UIIsOpen(UIType.UIChapterUnlock) || UIIsOpen(UIType.UILevelUp))
        {
            return false;
        }
        if (IsLastSibling(trans))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 当前界面是否为最上层
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    public static bool IsLastSibling(Transform trans)
    {
        for (int i = trans.parent.childCount - 1; i >= 0; i--)
        {
            if (trans.parent.GetChild(i).gameObject.activeInHierarchy)
            {
                if (trans.parent.GetChild(i) != trans)
                {
                    return false;
                }
                return true;
            }
        }
        return false;

    }

    /// <summary>
    /// 界面是否打开
    /// </summary>
    /// <param name="uiName"></param>
    /// <returns></returns>
    public static bool UIIsOpen(string uiName)
    {
        if (!UIComponent.IsHaveUI(uiName))
        {
            return false;
        }
        GameObject goUI = UIComponent.GetExistUI(uiName);
        if (goUI.activeInHierarchy)
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// 打开功能开启展示界面
    /// </summary>
    /// <param name="funcID"></param>
    public static async void OpenOpenFunction(OpenFunctionDefine funcInfo)
    {
        var obj = await UIComponent.CreateUIAsync(UIType.UIOpenFunction, true);
        obj.GetComponent<UIOpenFunctionController>().InitValue(funcInfo);
    }

    /// <summary>
    /// 存储提示 且保存到本地
    /// </summary>
    public static void SaveOpenFunctionTips(int funcID)
    {
        if (!playerInfoData.CurLocalSaveData.FinishedOpenFunIDs.Contains(funcID))
        {
            playerInfoData.CurLocalSaveData.FinishedOpenFunIDs.Add(funcID);
            UniversalTool.SaveLocalSaveData();
        }

    }

    #endregion

    #region 注销账号

    /// <summary>
    /// 通知服务器注销账号注销账号
    /// </summary>
    public static void NotifyServerLogoutAccount(Action<bool> callback)
    {
        CSEmptyLogoutAccount cSEmptyLogoutAccount = new CSEmptyLogoutAccount();
        ProtocalManager.Instance().SendCSEmptyLogoutAccount(cSEmptyLogoutAccount, (SCEmptyLogoutAccount sCEmptyLogoutAccount) =>
        {
            Debug.Log("请求通知服务器注销账号注销账号成功！");
            callback?.Invoke(true);
        },
        (ErrorInfo er) =>
        {
            Debug.Log("请求通知服务器注销账号注销账号失败！err：" + er.ErrorMessage);
            callback?.Invoke(false);
        });
    }

    #endregion

    #region sdk 广告 + 打点

    /// <summary>
    /// 接入广告
    /// </summary>
    /// <param name="adButton">英文注释，是哪个地方的什么功能的按钮</param>
    /// <param name="action">广告回调，1成功，其它失败</param>
    public static void OpenAd(string adButton, Action<int, string> action)
    {
        WaitManager.BeginRotate();
        Sdk.SdkFuc.OpenAd(adButton, (code, msg) =>
        {
            WaitManager.EndRotate();
            if (code != 1)
            {
                StaticData.CreateToastTips($"观看广告失败");
            }
            action(code, msg);
        });
    }
    /// <summary>
    /// 打点
    /// </summary>
    /// <param name="DotEventId">数据表事件枚举类型</param>
    public static void DataDot(DotEventId typeDotEventSubId)
    {
        var listData = StaticData.configExcel.DataAnalytics;
        var eachData = listData.Find(x => x.EventSubId == typeDotEventSubId);
        if (eachData != null)
        {
            Dictionary<string, string> subEvent = new Dictionary<string, string>();
            subEvent.Add(eachData.EventSubId.ToString(), eachData.EventName);
            Sdk.SdkFuc.LogEvent(eachData.EventMainId.ToString(), subEvent);
        }
    }

    #endregion
    public static async UniTask PlayManorEnterAnim()
    {
        UIManorComponent uiManorCom = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
        if (uiManorCom != null)
        {
            await uiManorCom.uiManorAnim.PlayAnimEnterManor();
        }
    }

    public static async UniTask PlayManorQuitAnim()
    {
        UIManorComponent uiManorCom = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
        if (uiManorCom != null)
        {
            await uiManorCom.uiManorAnim.PlayAnimQuitManor();
        }
    }

    public static void SetPlayerIconInfo()
    {
        var uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
        if (uiManorComponent != null)
        {
            var uiPlayerImageComponent = uiManorComponent.goPlayerIcon.GetComponent<UIPlayerImageComponent>();
            if (uiPlayerImageComponent != null)
            {
                uiPlayerImageComponent.SetInitInfo();
            }
        }
    }

    #region 订单

    /// <summary>
    /// 打开订单界面
    /// </summary>
    private static async void OpenUIDeal()
    {
        var obj = await UIComponent.CreateUIAsync(UIType.UIDeal);
        obj.GetComponent<UIDealController>().InitValue();
    }

    /// <summary>
    /// 进入订单界面
    /// </summary>
    public static void EnterUIDeal()
    {
        //1.判断是否已经获得订单 || 订单请求时间是否过期
        if (playerInfoData.CurrDeals.DealInfo.Count <= 0 || !TimeHelper.IsTheSameDay(playerInfoData.GetDealTime))
        {
            Debug.Log("进入订单界面 请求订单");
            //请求订单
            NotifyServerRequestDeals(SaveDealsAndOpenUIDeal);
        }
        else
        {
            OpenUIDeal();
        }
    }

    /// <summary>
    /// 请求订单
    /// </summary>
    public static void RequestDeals()
    {
        Debug.Log(" 请求订单");
        if (playerInfoData.CurrDeals.DealInfo.Count <= 0 || !TimeHelper.IsTheSameDay(playerInfoData.GetDealTime))
        {
            //请求订单
            NotifyServerRequestDeals(SaveDeals);
        }
    }

    /// <summary>
    /// 保存订单和打开订单界面
    /// </summary>
    /// <param name="sCDealInfo"></param>
    private static void SaveDealsAndOpenUIDeal(SCDealInfo sCDealInfo)
    {
        if (sCDealInfo == null)
        {
            return;
        }
        SaveDeals(sCDealInfo);
        OpenUIDeal();
    }

    /// <summary>
    /// 零点通知向服务器更新订单数据
    /// </summary>
    public static void NotifyUpdateDealData()
    {
        //清空订单数据
        playerInfoData.GetDealTime = string.Empty;
        playerInfoData.CurrDeals.DealInfo.Clear();
        //请求订单数据
        RequestDeals();

    }

    /// <summary>
    /// 保存订单数据到角色信息中
    /// </summary>
    private static void SaveDeals(SCDealInfo sCDealInfo)
    {
        if (sCDealInfo == null)
        {
            return;
        }
        //
        playerInfoData.CurrDeals = sCDealInfo;
        playerInfoData.GetDealTime = TimeHelper.CurGameTimeDay();

        //更新订单界面数据
        if (UIDealController.Instance != null)
            UIDealController.Instance.UpdateDealUI();
        //更新订单提示/红点
        NotifyUpdateDealTips();
    }

    /// <summary>
    /// 更新订单信息
    /// </summary>
    /// <param name="newDeal"></param>
    public static void UpdateDeal(SCCreateDeal newDeal)
    {

        for (int i = 0; i < playerInfoData.CurrDeals.DealInfo.Count; i++)
        {
            if (playerInfoData.CurrDeals.DealInfo[i].DealId == newDeal.DealId)
            {
                //playerInfoData.CurrDeals.DealInfo[i] = null;
                playerInfoData.CurrDeals.DealInfo[i] = newDeal;
                break;
            }
        }
        //通知提示
        NotifyUpdateDealTips();
    }

    /// <summary>
    /// 添加订单完成值
    /// </summary>
    /// <param name="num"></param>
    public static void AddDealCompleteValue(int dealLevel, int num = 1)
    {
        if (playerInfoData.CurrDeals.DealNum.Count > dealLevel)
        {
            playerInfoData.CurrDeals.DealNum[dealLevel] += num;
        }
    }

    /// <summary>
    /// 判断是否有订单可以提交 add 规则修改为可以提交可以完成就给提示
    /// </summary>
    /// <returns></returns>
    public static bool IsSubmintDeal()
    {
        //bool isSubmint = false;
        for (int i = 0; i < playerInfoData.CurrDeals.DealInfo.Count; i++)
        {
            if (playerInfoData.CurrDeals.DealInfo[i].DealRefreshTime > TimeHelper.ServerTimeStampNow) //订单刷新时间 对比 服务器当前时间
            {
                continue;
            }
            return true;
            //for (int j = 0; j < playerInfoData.CurrDeals.DealInfo[i].DealNeedGoods.Count; j++)
            //{
            //    int GoodNum = GetWareHouseItem(playerInfoData.CurrDeals.DealInfo[i].DealNeedGoods[j].GoodId).GoodNum;
            //    if (GoodNum < playerInfoData.CurrDeals.DealInfo[i].DealNeedGoods[j].GoodNum)
            //    {
            //        isSubmint = false;
            //        break;
            //    }
            //    else
            //    {
            //        isSubmint = true;
            //    }

            //}
            //if (isSubmint)
            //{
            //    //Debug.Log("IsSubmintDeal 判断是否有订单可以提交 true");
            //    return true;
            //}

        }
        //Debug.Log("IsSubmintDeal 判断是否有订单可以提交 false");
        return false;
    }

    /// <summary>
    /// 操作更新订单提示
    /// </summary>
    private static Action OperatingUpdateDealTips;
    public static void AddUpdateDealTipsAction(Action updateDealTips)
    {
        OperatingUpdateDealTips = updateDealTips;
    }
    /// <summary>
    /// 通知更新订单提示
    /// </summary>
    public static void NotifyUpdateDealTips()
    {
        OperatingUpdateDealTips?.Invoke();
    }



    /// <summary>
    /// 通知服务器请求订单
    /// </summary>
    public static void NotifyServerRequestDeals(Action<SCDealInfo> successAction)
    {
        CSEmptyDealInfo cSEmptyDealInfo = new CSEmptyDealInfo();
        ProtocalManager.Instance().SendCSEmptyDealInfo(cSEmptyDealInfo, (SCDealInfo sCDealInfo) =>
        {
            Debug.Log("请求订单成功！");

            successAction?.Invoke(sCDealInfo);
        },
        (ErrorInfo er) =>
        {
            Debug.Log("通知服务器请求订单：" + er.ErrorMessage);
            successAction?.Invoke(null);
        });
    }

    /// <summary>
    /// 通知服务器提交订单
    /// </summary>
    public static void NotifyServerSubmintDeal(int dealID, Action<SCSubmintDeal> successAction)
    {
        CSSubmintDeal cSSubmintDeal = new CSSubmintDeal();
        cSSubmintDeal.DealId = dealID;
        ProtocalManager.Instance().SendCSSubmintDeal(cSSubmintDeal, (SCSubmintDeal sCSubmintDeal) =>
        {
            Debug.Log("提交订单成功！");
            successAction?.Invoke(sCSubmintDeal);
        },
        (ErrorInfo er) =>
        {
            Debug.Log("通知服务器提交订单：" + er.ErrorMessage);
            successAction?.Invoke(null);
        });
    }

    /// <summary>
    /// 通知服务器刷新订单
    /// </summary>
    public static void NotifyServerRefreshDeal(int dealID, Action<SCRefreshDeal> successAction)
    {

        Debug.Log("通知服务器刷新订单 dealID:" + dealID);
        CSRefreshDeal cSRefreshDeal = new CSRefreshDeal();
        cSRefreshDeal.DealId = dealID;

        ProtocalManager.Instance().SendCSRefreshDeal(cSRefreshDeal, (SCRefreshDeal sCRefreshDeal) =>
        {
            Debug.Log("刷新订单成功！");
            successAction?.Invoke(sCRefreshDeal);
        },
        (ErrorInfo er) =>
        {
            Debug.Log("通知服务器刷新订单：" + er.ErrorMessage);
            successAction?.Invoke(null);
        });
    }

    /// <summary>
    /// 通知服务器 订单广告跳过刷新时间
    /// </summary>
    public static void NotifyServerAdvSkipDeal(int dealID, Action<bool> successAction)
    {
        CSAdvSkipDeal cSAdvSkipDeal = new CSAdvSkipDeal();
        cSAdvSkipDeal.DealId = dealID;

        ProtocalManager.Instance().SendCSAdvSkipDeal(cSAdvSkipDeal, (SCEmptyAdvSkipDeal sCEmptyAdvSkipDeal) =>
        {
            Debug.Log("订单广告跳过刷新时间成功！");
            successAction?.Invoke(true);
        },
        (ErrorInfo er) =>
        {
            Debug.Log("通知服务器订单广告跳过刷新时间：" + er.ErrorMessage);
            successAction?.Invoke(false);
        });
    }

    #endregion

    #region 刷新好友列表
    /// <summary>
    /// 刷新庄园好友UI列表
    /// </summary>
    /// <param name="OnInfoCallBack"></param>
    public static void RefreshFriendManorList(Action OnInfoCallBack)
    {
        StaticData.DebugGreen("请求庄园好友列表数据~~~");
        StaticData.playerInfoData.listFriendStealInfo.Clear();
        // 正式请求
        CSEmptyFriendSteal csEmptyFriendSteal = new CSEmptyFriendSteal();
        ProtocalManager.Instance().SendCSEmptyFriendSteal(csEmptyFriendSteal, (friendStealList) =>
        {
            StaticData.DebugGreen($"请求庄园好友列表数据成功~~~");
            for (int i = 0; i < StaticData.playerInfoData.listFriendInfo.Count; i++)
            {
                var curFriendInfo = StaticData.playerInfoData.listFriendInfo[i];
                FriendStealInfo friendStealInfo = new FriendStealInfo()
                {
                    nickname = curFriendInfo.FriendName,
                    //nickname = curFriendInfo.Uid.ToString(),
                    uid = curFriendInfo.Uid,
                    headIcon = curFriendInfo.FriendImage,
                    level = StaticData.GetPlayerLevelByExp(curFriendInfo.FriendExperience),
                    playerLevelAndCurrExp = StaticData.GetPlayerLevelAndCurrExp(curFriendInfo.FriendExperience),
                    isSteal = false
                };
                if (friendStealList == null)
                {
                    StaticData.playerInfoData.listFriendStealInfo.Add(friendStealInfo);
                    continue;
                }
                if (friendStealList.Info == null)
                {
                    StaticData.playerInfoData.listFriendStealInfo.Add(friendStealInfo);
                    continue;
                }
                for (int j = 0; j < friendStealList.Info.Count; j++)
                {
                    var curFriendStealInfo = friendStealList.Info[j];
                    if (curFriendInfo.Uid == curFriendStealInfo.Uid)
                    {
                        friendStealInfo.isSteal = curFriendStealInfo.IsSteal;
                        break;
                    }
                }
                StaticData.playerInfoData.listFriendStealInfo.Add(friendStealInfo);
            }
            OnInfoCallBack?.Invoke();
        }, (error) =>
        {
            StaticData.DebugGreen($"请求庄园好友列表数据错误~~~");
        }, false);
    }
    #endregion

    #region 道具判断

    /// <summary>
    /// 道具是否足够
    /// </summary>
    /// <param name="propID"></param>
    /// <param name="propNum"></param>
    /// <returns></returns>
    public static bool IsProps(int propID, int propNum)
    {
        var item = GetWareHouseItem(propID);
        if (item == null)
            return false;

        //拥有的小于需要的
        if (item.GoodNum < propNum)
        {
            return false;
        }

        return true;
    }
    /// <summary>
    /// 从订单中打开地块种植信息
    /// </summary>
    public static async UniTask OnOpenPlantFromDeal()
    {
        List<TileComponent> listTileComponentUnLock = new List<TileComponent>();
        int maxUnLockRegionId = GetMaxManorRegionAreaId();
        //收集解锁的地块
        for (int i = 0; i < Root2dSceneManager._instance.objPool.transform.childCount; i++)
        {
            var tileGo = Root2dSceneManager._instance.objPool.transform.GetChild(i);
            TileComponent tileCom = tileGo.GetComponent<TileComponent>();
            if (tileGo.gameObject.activeInHierarchy //激活的地块
                && tileCom.regionId <= maxUnLockRegionId//在解锁区域
                && tileCom.CropGoodId <= 0//没有种植
                )
            {
                listTileComponentUnLock.Add(tileCom);
            }
        }
        if (listTileComponentUnLock.Count > 0)
        {
            //默认为第一个符合条件的
            bool isHaveInCamera = false;
            TileComponent willSetTileComponent = listTileComponentUnLock[0];
            for (int i = 0; i < listTileComponentUnLock.Count; i++)
            {
                if (isInManorCamera(listTileComponentUnLock[i]))
                {
                    //找到一个在相机范围内的
                    willSetTileComponent = listTileComponentUnLock[i];
                    isHaveInCamera = true;
                    break;
                }
            }
            if (isHaveInCamera == false)
            {
                Root2dSceneManager._instance.worldCameraComponent.SetCameraToTilePos(willSetTileComponent.transform.localPosition);
                await UniTask.Delay(100);
            }
            await willSetTileComponent.OnTileClick();
        }
    }
    /// <summary>
    /// 获取庄园的最大解锁区域id，默认为1
    /// </summary>
    /// <returns></returns>
    static int GetMaxManorRegionAreaId()
    {
        int UnLockRegionIdMax = 1;
        var unlockInfoSelf = Root2dSceneManager._instance.UnlockAreaInfoSelf;
        for (int i = 0; i < unlockInfoSelf.Count; i++)
        {
            if (unlockInfoSelf[i].State == AreaState.RemoveWorkShed)
            {
                if (UnLockRegionIdMax < unlockInfoSelf[i].AreaId)
                {
                    UnLockRegionIdMax = unlockInfoSelf[i].AreaId;
                }
            }
        }
        return UnLockRegionIdMax;
    }
    /// <summary>
    /// 获取地块位置是否在相机范围内
    /// </summary>
    /// <returns></returns>
    static bool isInManorCamera(TileComponent tileComponent)
    {
        bool isInCamera = false;
        Camera mCamera = Root2dSceneManager._instance.worldCameraComponent.cameraWorld;
        Vector3 pos = tileComponent.transform.position;
        //转化为视角坐标
        Vector3 viewPos = mCamera.WorldToViewportPoint(pos);
        // x,y取值在 0~1之外时代表在视角范围外；
        if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
        {
            isInCamera = true;
        }
        return isInCamera;
    }
    /// <summary>
    /// 打开物品不足提示
    /// </summary>
    /// <param name="propID"></param>
    public static void OpenPropsShortageTips(int propID)
    {
        string str = string.Empty;
        int rechargeType = 0; //充值类型
        int tipsNum = 0;

        //金币
        if (propID == StaticData.configExcel.GetVertical().GoldGoodsId)
        {
            tipsNum = 120245;
            rechargeType = 0;
        }
        else if (propID == StaticData.configExcel.GetVertical().JewelGoodsId) //钻石
        {
            tipsNum = 120243;
            rechargeType = 1;
        }
        else //其它  暂时不予提示
        {
            //str = LocalizationDefineHelper.GetStringNameById(120068);
            //var item = StaticData.configExcel.GetGameItemByID(propID);
            //if (item == null)
            //    return;
            //str = string.Format(str, LocalizationDefineHelper.GetStringNameById(item.ItemName));
            //StaticData.OpenCommonTips(str, 120010, async () =>
            //{
            //    //打开商城界面
            //    await StaticData.OpenShopUI();
            //}, null, 120075);
            return;
        }

        str = LocalizationDefineHelper.GetStringNameById(tipsNum);

        //120243 //钻石不足，要进行购买吗
        //120245 //金币不足，要进行购买吗
        //120068 //{0}不足，要进行购买吗？

        //120010 立即购买
        //120075 取消

        StaticData.OpenCommonTips(str, 120010, async () =>
        {
            await StaticData.OpenRechargeUI(rechargeType);
        }, null, 120075);
    }

    #endregion
}