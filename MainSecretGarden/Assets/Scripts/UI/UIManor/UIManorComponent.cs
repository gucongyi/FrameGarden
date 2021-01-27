using Company.Cfg;
using Cysharp.Threading.Tasks;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManorComponent : MonoBehaviour
{
    public enum StateUIAnim
    {
        None,
        UIManorClose,
        UIManorOpen
    }
    #region 庄园动画
    public UIManorAnimComponent uiManorAnim;
    #endregion
    public GameObject rootRight;
    StateUIAnim stateUIAnim = StateUIAnim.None;
    //自己庄园根目录
    public GameObject goRootSelfManor;
    public GameObject goRootFriendManor;
    public Button ButtonReturnMyManor;

    #region 庄园人物气泡
    public ManorDialogueRightBubble personBubble;
    #endregion
    #region 庄园内部操作UI
    public UIWorldHandleManager uIWorldHandleManager;
    #endregion
    
   
    #region 日志
    //public Button ButtonManorLog;
    #endregion
    #region 庄园一键浇水
    #endregion
    #region 好友庄园偷取列表
    public Button ButtonManorFriendClose;
    public Button ButtonManorFriendOpen;
    public LoopHorizontalScrollRect lsFriendStealList;
    #endregion

    #region 红点
    //签到红点
    public Transform RedDotSignTrans;
    //商城红点
    public Transform RedDotShopTrans;
    // 仓库红点
    public Transform RedDotWarehouseDotTrans;
    //邮件红点
    public Transform RedDotMailTrans;
    //好友红点
    public Transform RedDotFriendTrans;
    //任务红点
    public Transform RedDotTaskTrans;
    //订单红点
    public Transform RedDotOrderTrans;
    //自由换装红点
    public Transform RedDotDressUpTrans;
    //心动时刻红点
    public Transform RedDotHeartBeatTrans;
    //章节红点
    public Transform RedDotChapterTrans;
    // 装饰红点
    public Transform RedDotManorDressTrans;
    #endregion
    #region Top
        #region 头像父控件
        public Transform transPlayerIconParent;
        [HideInInspector]
        public GameObject goPlayerIcon;
        #endregion
        #region 货币父控件
        public Transform transCoinParent;
        public Transform transDiamondParent;
        public Transform transWaterParent;
        GameObject goCoin;
        GameObject goDiamond;
        GameObject goWater;
        public GameObject goTopCommon;
        #endregion
    #endregion
    #region Left
    //签到
    public Button ButtonSign;

    #region 聊天
        public Transform transChat;
        GameObject goUIChat;
    #endregion
    #endregion
    #region Right
    //商城
    public Button ButtonStore;
    //仓库
    public Button buttonWareHouse;
    //邮件
    public Button ButtonMail;
    //好友
    public Button ButtonFriend;
    //任务
    public Button ButtonTask;
    //订单
    public Button ButtonOrder;
    //一键浇水
    public Button ButtonWateringOneKey;
    #region 一键收获
    public Button ButtonGainOneKey;
    public GameObject GoButtonGainOneKeyEffect;
    //一键偷取
    public Button ButtonStealOneKey;
    public GameObject GoButtonStealOneKeyEffect;
    bool isManorUIClose;
    #endregion
    #endregion

    #region Bottom
    //换装
    public Button ButtonDressUp;
    //心动时刻
    public Button ButtonHeartBeat;
    //章节
    public Button ButtonChapter;
    //大富翁
    public Button ButtonRichman;
    #region 装饰
    public Button ButtonDecorate;
    public List<CSWareHouseStruct> listDecorateMyHave = new List<CSWareHouseStruct>();
    public LoopHorizontalScrollRect loopHorizontalScrollRect;
    public Button buttonDecorateClose;
    public Button ButtonDecorateToBuy;
    #endregion
    #endregion



    // Start is called before the first frame update
    void Awake()
    {
        goRootSelfManor.SetActive(false);
        goRootFriendManor.SetActive(false);
        ButtonReturnMyManor.onClick.RemoveAllListeners();
        ButtonReturnMyManor.onClick.AddListener(OnButtonReturnMyManorClick);
        ButtonDecorate.onClick.RemoveAllListeners();
        ButtonDecorate.onClick.AddListener(OnButtonDecorateManor);
        //设置装饰物品
        RefreshDecorateList();
        loopHorizontalScrollRect.RefillCells();
        buttonDecorateClose.onClick.RemoveAllListeners();
        buttonDecorateClose.onClick.AddListener(OnButtonDecorateClose);
        buttonWareHouse.onClick.RemoveAllListeners();
        buttonWareHouse.onClick.AddListener(OnButtonWareHouseClick);

        ButtonGainOneKey.onClick.RemoveAllListeners();
        ButtonGainOneKey.onClick.AddListener(OnButtonGainOneKeyClick);
        ButtonStealOneKey.onClick.RemoveAllListeners();
        ButtonStealOneKey.onClick.AddListener(OnButtonStealOneKeyClick);
        ButtonStore.onClick.RemoveAllListeners();
        ButtonStore.onClick.AddListener(OnButtonStoreClick);
        ButtonDecorateToBuy.onClick.RemoveAllListeners();
        ButtonDecorateToBuy.onClick.AddListener(OnButtonDecorateToBuyClick);
        ButtonRichman.onClick.RemoveAllListeners();
        ButtonRichman.onClick.AddListener(OnButtonRichmanClick);
        //ButtonManorLog.onClick.RemoveAllListeners();
        //ButtonManorLog.onClick.AddListener(OnButtonManorLogClick);
        ButtonOrder.onClick.RemoveAllListeners();
        ButtonOrder.onClick.AddListener(OnButtonOrderClick);
        ButtonTask.onClick.RemoveAllListeners();
        ButtonTask.onClick.AddListener(OnButtonTaskClick);
        ButtonFriend.onClick.RemoveAllListeners();
        ButtonFriend.onClick.AddListener(OnButtonFriendClick);
        ButtonManorFriendClose.onClick.RemoveAllListeners();
        ButtonManorFriendClose.onClick.AddListener(OnButtonManorFriendCloseClick);
        ButtonManorFriendOpen.onClick.RemoveAllListeners();
        ButtonManorFriendOpen.onClick.AddListener(OnButtonManorFriendOpenClick);
        ButtonWateringOneKey.onClick.RemoveAllListeners();
        ButtonWateringOneKey.onClick.AddListener(OnButtonWateringOneKeyClick);
        ButtonSign.onClick.RemoveAllListeners();
        ButtonSign.onClick.AddListener(OnButtonSignClick);
        ButtonMail.onClick.RemoveAllListeners();
        ButtonMail.onClick.AddListener(OnButtonMailClick);
        ButtonDressUp.onClick.RemoveAllListeners();
        ButtonDressUp.onClick.AddListener(OnButtonDressUpClick);
        ButtonHeartBeat.onClick.RemoveAllListeners();
        ButtonHeartBeat.onClick.AddListener(OnButtonHeartBeatClick);
        ButtonChapter.onClick.RemoveAllListeners();
        ButtonChapter.onClick.AddListener(OnButtonChapterClick);
        RegisterDot();
        UpdateRedDot();
        //添加头像
        goPlayerIcon = StaticData.CreatePlayerImage(transPlayerIconParent);
        //添加金币
        goCoin = StaticData.CreateCoinNav(transCoinParent);
        goDiamond = StaticData.CreateDiamondNav(transDiamondParent);
        goWater = StaticData.CreateWaterNav(transWaterParent);
        //goUIChat
        goUIChat = StaticData.CreateUIChat(transChat);
        StaticData.AddUpdateDealTipsAction(UpdateOrderTips);
    }

    private async void OnButtonChapterClick()
    {
        StaticData.DataDot(Company.Cfg.DotEventId.ChapterIcon);
        //章节是否开启
        if (!StaticData.IsOpenFunction(10007))
            return;

        await StaticData.OpenChapterUI();
    }

    private async void OnButtonHeartBeatClick()
    {
        StaticData.DataDot(Company.Cfg.DotEventId.HeartMomentIcon);
        //心动时刻
        if (!StaticData.IsOpenFunction(10015))
            return;

        await StaticData.OpenImpulseUI();
    }

    private async void OnButtonDressUpClick()
    {
        StaticData.DataDot(Company.Cfg.DotEventId.FashionIcon);
        //好友
        if (!StaticData.IsOpenFunction(10019))
            return;
        await StaticData.OpenUIRoleSwitching();
    }

    private async void OnButtonMailClick()
    {
        StaticData.DataDot(Company.Cfg.DotEventId.MailIcon);
        //邮件
        if (!StaticData.IsOpenFunction(10004))
            return;
        StaticData.DataDot(DotEventId.MailIcon);
        await StaticData.OpenMailbox();
    }

    private async void OnButtonSignClick()
    {
        StaticData.DataDot(Company.Cfg.DotEventId.ActivityIcon);
        await UIComponent.CreateUIAsync(UIType.UISign);
    }

    /// <summary>
    /// 注册红点
    /// </summary>
    void RegisterDot()
    {
        RedDotManager.AddDotTra(RedDotManager.RedDotKey.Task, RedDotTaskTrans);
        RedDotManager.AddDotTra(RedDotManager.RedDotKey.Shopping, RedDotShopTrans.transform);
        RedDotManager.AddDotTra(RedDotManager.RedDotKey.Warehouse, RedDotWarehouseDotTrans);
        RedDotManager.AddDotTra(RedDotManager.RedDotKey.ManorDecorate, RedDotManorDressTrans);
        RedDotManager.AddDotTra(RedDotManager.RedDotKey.Order, RedDotOrderTrans);
        RedDotManager.AddDotTra(RedDotManager.RedDotKey.Activity, RedDotSignTrans);
        RedDotManager.AddDotTra(RedDotManager.RedDotKey.Mailbox, RedDotMailTrans);
        RedDotManager.AddDotTra(RedDotManager.RedDotKey.Chapter, RedDotChapterTrans);
    }
    /// <summary>
    /// 更新红点
    /// </summary>
    void UpdateRedDot()
    {
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Task);
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Shopping);
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Warehouse);
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.ManorDecorate);
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Order);
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Activity);
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Mailbox);
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Chapter);
    }

    //打开好友偷取列表
    private void OnButtonManorFriendCloseClick()
    {
        StaticData.RefreshFriendManorList(async () =>
        {
            GenerateFriendStealListUI();
            //播放动画
            await uiManorAnim.PlayAnimFriendStealListToOpen();
            ButtonManorFriendClose.gameObject.SetActive(false);
            ButtonManorFriendOpen.gameObject.SetActive(true);
        });
    }

    //关闭好友偷取列表
    private async void OnButtonManorFriendOpenClick()
    {
        //播放动画
        await uiManorAnim.PlayAnimFriendStealListToClose();
        ButtonManorFriendClose.gameObject.SetActive(true);
        ButtonManorFriendOpen.gameObject.SetActive(false);
    }

    private async void OnButtonFriendClick()
    {
        //好友
        if (!StaticData.IsOpenFunction(10006))
            return;
        //新手引导完成
        if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.isCurrStepGuiding)
        {
            GuideCanvasComponent._instance.SetLittleStepFinish();
        }
        await StaticData.OpenFriend(false);
    }

    private async void OnButtonOrderClick()
    {
        //庄园打开订单 商店的时候关闭内部操作UI
        uIWorldHandleManager.SetHandleTileUIClose();
        if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.isCurrStepGuiding)
        {
            GuideCanvasComponent._instance.SetLittleStepFinish();
        }
        await StaticData.OpenOrder();
    }

    private void actionUpdateTaskRedDot(bool isOpen)
    {
        if (RedDotTaskTrans != null)
        {
            RedDotTaskTrans.gameObject.SetActive(isOpen);
        }
    }

    /// <summary>
    /// 更新订单提示
    /// </summary>
    private void UpdateOrderTips()
    {
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Order);
        //goRedDotOrder.SetActive(StaticData.IsSubmintDeal());
    }


    //任务界面
    private async void OnButtonTaskClick()
    {

        //完成新手任务
        if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.CurrExecuteGuideLittleStepDefine.Id == 11025)
        {
            GuideCanvasComponent._instance.SetLittleStepFinish();
        }
        StaticData.DataDot(DotEventId.TaskIcon);
        await StaticData.OpenTaskPanel();
    }

    private async void OnButtonDecorateToBuyClick()
    {
        //跳转商城
        await StaticData.OpenShopUI(2, () =>
        {
            RefreshDecorateList();
        });
    }

    private void OnButtonManorLogClick()
    {
        //功能是否开启
        if (!StaticData.IsOpenFunction(10018))
        {
            return;
        }
        //todo 庄园日志
        UIComponent.CreateUI(UIType.UIManorLog);
    }

    private async void OnButtonRichmanClick()
    {
        //功能是否开启
        if (!StaticData.IsOpenFunction(10011))
        {
            return;
        }

        if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.isCurrStepGuiding)
        {
            GuideCanvasComponent._instance.SetLittleStepFinish();
        }
        UICameraManager._instance.SetDefault();
        UIComponent.RemoveUI(UIType.UIFriend);
        UIComponent.RemoveUI(UIType.Warehouse);
        UIComponent.RemoveUI(UIType.UIShop);
        UIComponent.RemoveUI(UIType.UIManor);
        await StaticData.OpenMonopoly();
        StaticData.DataDot(Company.Cfg.DotEventId.EnterRichMan);
    }

    private async void OnButtonWareHouseClick()
    {
        //庄园打开仓库 商店的时候关闭内部操作UI
        uIWorldHandleManager.SetHandleTileUIClose();
        //关闭地块选中
        Root2dSceneManager._instance.CloseAllTileObjSelect();
        StaticData.DataDot(DotEventId.WareHouseIcon);
        await StaticData.OpenWareHouse(1);
    }

    public void OnButtonDecorateClose()
    {
        uiManorAnim.PlayAnimCloseDecorate();
        //执行提交操作
        if (StaticData.GetUIWorldHandleComponent().isRotatingDecorate)
        {
            StaticData.GetUIWorldHandleComponent().OnButtonRotateOKClick();
            StaticData.GetUIWorldHandleComponent().SetHandleTileUIClose();
        }
    }
    //购买完装饰物重新填充
    public void RefreshDecorateList()
    {
        listDecorateMyHave = StaticData.GetWareHouseDecorate();
        ButtonDecorateToBuy.gameObject.SetActive(listDecorateMyHave.Count <= 0);
        loopHorizontalScrollRect.totalCount = listDecorateMyHave.Count;
        loopHorizontalScrollRect.RefillCells();
    }


    private async void OnButtonStoreClick()
    {
        //章节是否开启
        if (!StaticData.IsOpenFunction(10008))
        {
            return;
        }
        //庄园打开仓库 商店的时候关闭内部操作UI
        uIWorldHandleManager.SetHandleTileUIClose();
        //关闭地块选中


        Root2dSceneManager._instance.CloseAllTileObjSelect();
        await StaticData.OpenShopUI(2);


        //更新外部红点
        ShopTool.isLookStore = true;
        ShopTool.LookDataSave();//保存查看数据

        ShopTool.SavaShopData();//保存新链表
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Shopping);

    }
    public async void DragTileOrDecorateCloseSelfManor(bool isClose)
    {
        if (Root2dSceneManager._instance.isFriendManor)//好友庄园不能长按拖动
        {
            return;
        }
        isManorUIClose = isClose;
        goRootSelfManor.SetActive(!isClose);
        ////头像
        //goPlayerIcon.SetActive(!isClose);
        ////货币
        //goCoin.SetActive(!isClose);
        //goDiamond.SetActive(!isClose);
        //顶部包括货币，头像
        goTopCommon.SetActive(!isClose);
        ////日志按钮
        //ButtonManorLog.gameObject.SetActive(!isClose);
        ////聊天
        //goUIChat.SetActive(!isClose);
        //一件收获
        ButtonGainOneKey.gameObject.SetActive(!isClose);
        //一键偷取
        ButtonStealOneKey.gameObject.SetActive(!isClose);
        //任务
        ButtonTask.gameObject.SetActive(!isClose);
    }
    public void OpenManorRoot(bool isSelf)
    {
        goRootSelfManor.SetActive(isSelf);
        goRootFriendManor.SetActive(!isSelf);
        ButtonTask.gameObject.SetActive(isSelf);
        //ButtonManorLog.gameObject.SetActive(isSelf);
        //设置自己庄园或者好友庄园的一键收获按钮
        ButtonGainOneKey.gameObject.SetActive(isSelf);
        ButtonStealOneKey.gameObject.SetActive(!isSelf);
        //设置气泡
        ManorBubbleComponent._instance.personBubble = personBubble;
        //货币显示
        goDiamond.SetActive(isSelf);
        goCoin.SetActive(isSelf);
        goWater.SetActive(isSelf);
        //进入庄园 重置偷取列表状态
        ButtonManorFriendClose.gameObject.SetActive(true);
        ButtonManorFriendOpen.gameObject.SetActive(false);
        //订单
        ButtonOrder.gameObject.SetActive(isSelf);

        //好友
        ButtonFriend.gameObject.SetActive(isSelf);

        StaticData.RefreshFriendManorList(() =>
        {
            GenerateFriendStealListUI();
        });
        if (!isSelf)
        {
            //关闭操作按钮
            StaticData.GetUIWorldHandleComponent().SetHandleTileUIClose();
        }
        
    }

    public void PlayOneKeyGainEffect(bool isPlay)
    {
        if (!Root2dSceneManager._instance.isFriendManor)
        {
            ButtonGainOneKey.interactable = isPlay;
            GoButtonGainOneKeyEffect.SetActive(isPlay);
            if (isManorUIClose)
            {
                GoButtonGainOneKeyEffect.SetActive(false);
                GoButtonStealOneKeyEffect.SetActive(false);
            }
        }
        else
        {
            ButtonStealOneKey.interactable = isPlay;
            GoButtonStealOneKeyEffect.SetActive(isPlay);
            if (isManorUIClose)
            {
                GoButtonStealOneKeyEffect.SetActive(false);
            }
        }
        
    }
    private void OnButtonGainOneKeyClick()
    {
        //功能是否开启
        if (!StaticData.IsOpenFunction(10020))
        {
            return;
        }
        StaticData.GetUIWorldHandleComponent().OnButtonGainOrStealOneKeyClick();
    }
    private void OnButtonStealOneKeyClick()
    {
        StaticData.GetUIWorldHandleComponent().OnButtonGainOrStealOneKeyClick();
    }
    private void OnButtonDecorateManor()
    {
        StaticData.DataDot(Company.Cfg.DotEventId.DecorateIcon);
        //功能是否开启
        if (!StaticData.IsOpenFunction(10012))
        {
            return;
        }
        uiManorAnim.PlayAnimOpenDecorate();
    }
    private async void OnButtonReturnMyManorClick()
    {
        //播放庄园退出动画
        UIManorComponent uiManorCom = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
        if (uiManorCom != null)
        {
            await uiManorCom.uiManorAnim.PlayAnimQuitManor();
        }
        //加载场景
        UIComponent.HideUI(UIType.UIManor);
        await SceneManagerComponent._instance.ChangeSceneAsync(EnumScene.Manor, true);
        await UIComponent.CreateUIAsync(UIType.UIManor);
        UIManorComponent uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
        uiManorComponent.OpenManorRoot(true);
        if (Root2dSceneManager._instance != null)
        {
            await Root2dSceneManager._instance.EnterManor(-1);
        }
        //移动到特定视角时间
        await UniTask.Delay(1000);
        UIComponent.HideUI(UIType.UISceneLoading);
        await StaticData.PlayManorEnterAnim();
    }

    public async void OnButtonEnterFriendManor(long idFriend)
    {
        if (StaticData.idPreEnterManor == idFriend)
        {
            StaticData.CreateToastTips($"正在当前好友的庄园中！");
            return;
        }
        StaticData.idPreEnterManor = idFriend;
        UIComponent.HideUI(UIType.UIManor);
        await SceneManagerComponent._instance.ChangeSceneAsync(EnumScene.Manor, true);
        await UIComponent.CreateUIAsync(UIType.UIManor);

        UIManorComponent uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
        uiManorComponent.OpenManorRoot(false);
        if (Root2dSceneManager._instance != null)
        {
            await Root2dSceneManager._instance.EnterManor(idFriend);
        }
        StaticData.SetSceneState(StaticData.SceneState.ManorFriend);
        //移动到特定视角时间
        await UniTask.Delay(1000);
        UIComponent.HideUI(UIType.UISceneLoading);
        await StaticData.PlayManorEnterAnim();
    }
    //好友庄园界面刷新
    /// <summary>
    /// 生成庄园好友列表UI
    /// </summary>
    private void GenerateFriendStealListUI()
    {
        lsFriendStealList.ClearCells();
        lsFriendStealList.totalCount = StaticData.playerInfoData.listFriendStealInfo.Count;
        lsFriendStealList.RefillCells();
    }


    //庄园一键浇水
    void OnButtonWateringOneKeyClick()
    {
        if (Root2dSceneManager._instance.isFriendManor)
        {//好友庄园不可操作
            return;
        }
        var firstFertilizer = StaticData.GetFertilizerCountByWhich(0);
        //水滴数量不足提示
        if (firstFertilizer.GoodNum <= 0)
        {
            //关闭UI
            StaticData.GetUIWorldHandleComponent().SetHandleTileUIClose();
            string FertilizerName = StaticData.GetMultiLanguageByGameItemId(firstFertilizer.GoodId);
            string Tips = string.Format(StaticData.GetMultilingual(120068), FertilizerName);
            StaticData.OpenCommonTips(Tips, 120010, async () =>
            {
                //跳转商城
                await StaticData.OpenShopUI(1);
            });
            return;
        }
        //对接服务器
        CSEmptyOnceWatering csEmptyOnceWatering = new CSEmptyOnceWatering();
        ManorProtocalHelper.SendCSEmptyOnceWatering(csEmptyOnceWatering,(succ) =>
        {
            if (succ == null)
            {
                return;
            }
            if (succ.SoilId == null)
            {
                return;
            }
            if (succ.SoilId.Count<=0)
            {
                return;
            }
            List<ItemFirstStageFertilizerComponent> listItemWaterComponent = new List<ItemFirstStageFertilizerComponent>();
            listItemWaterComponent.AddRange(uIWorldHandleManager.TransGoFirstStageRoot.GetComponentsInChildren<ItemFirstStageFertilizerComponent>());
            if (listItemWaterComponent != null && listItemWaterComponent.Count > 0)
            {
                //关闭UI
                StaticData.GetUIWorldHandleComponent().SetHandleTileUIClose();
                //播放音效点击
                GameSoundPlayer.Instance.PlaySoundEffect(MusicHelper.SoundEffectWatering);
                for (int i = 0; i < succ.SoilId.Count; i++)
                {
                    var itemWaterCom=listItemWaterComponent.Find(x => x.seedGrowComponent.tileComponent.SoilId == succ.SoilId[i]);

                    if (itemWaterCom != null)
                    {
                        //点击浇水的时候，关闭水滴
                        itemWaterCom.ButtonFertilizer.gameObject.SetActive(false);
                        itemWaterCom.TextNumNormal.gameObject.SetActive(false);
                        itemWaterCom.TextNumZero.gameObject.SetActive(false);
                        //施肥特效
                        StaticData.GetUIWorldHandleComponent().PlayWateringAnim(itemWaterCom.seedGrowComponent.tileComponent);
                        
                        //end 施肥特效
                        SeedGrowComponent.PeriodGrow nextPeriod = SeedGrowComponent.PeriodGrow.Germinate;//种子的下一时期，幼苗期
                                                                                                         //直接进入下一个时期
                        float remainTime = StaticData.GetSeedGrowComponentTotalSecond(itemWaterCom.seedGrowComponent, nextPeriod);
                        itemWaterCom.seedGrowComponent.SetPeriod(nextPeriod, (long)remainTime * 1000);
                    }
                }
                //更新水滴数量
                StaticData.UpdateWareHouseItem(firstFertilizer.GoodId,-succ.SoilId.Count);
                //看是否还有能浇水的
                SetCanWateringOnekey();
            }
        });
    }

    public async void SetCanWateringOnekey()
    {
        //等2ms，因为删除水滴组件Destory有延迟
        await UniTask.Delay(200);
        List<ItemFirstStageFertilizerComponent> listItemWaterComponent = new List<ItemFirstStageFertilizerComponent>();
        listItemWaterComponent.AddRange(uIWorldHandleManager.TransGoFirstStageRoot.GetComponentsInChildren<ItemFirstStageFertilizerComponent>());
        if (listItemWaterComponent.Count <= 0)
        {
            //禁用浇水按钮
            ButtonWateringOneKey.interactable = false;
        }
        else
        {
            ButtonWateringOneKey.interactable = true;
        }
    }
}
