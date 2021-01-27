using Company.Cfg;
using Cysharp.Threading.Tasks;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideCanvasComponent : MonoBehaviour
{
    public static GuideCanvasComponent _instance;
    bool isAllGuideFinish;
    bool isLoginGetInfoFinish;
    public UIGuideComponent uiGuideComponent;
    List<GuideTriggerConditionDefine> listGuideConditions;
    List<GuideLittleStepDefine> listGuideLittleStep;
    public GuideLittleStepDefine CurrExecuteGuideLittleStepDefine = null;
    int CurrExecuteGuideLittleStepId = 0;
    public bool isCurrStepGuiding;
    private void Awake()
    {
        _instance = this;
        isAllGuideFinish = false;
        isLoginGetInfoFinish = false;
        isCurrStepGuiding = false;
    }
    //根据服务器保存的信息判定引导是否已经完全完成，设置已经完成的
    public void SetGuideFinishListId(List<int> finishIdList)
    {
        if (finishIdList == null)
        {
            finishIdList=new List<int>();
        }
        listGuideConditions = StaticData.configExcel.GuideTriggerCondition;
        listGuideLittleStep = StaticData.configExcel.GuideLittleStep;
        isLoginGetInfoFinish = true;

        
        for (int i = 0; i < finishIdList.Count; i++)
        {
            var guideLittleStepDefine = listGuideLittleStep.Find(x => x.Id == finishIdList[i]);
            if (guideLittleStepDefine != null)
            {
                guideLittleStepDefine.IsFinish = true;
            }
        }
        isAllGuideFinish = true;
        for (int i = 0; i < listGuideLittleStep.Count; i++)
        {
            if (listGuideLittleStep[i].IsFinish==false)
            {
                isAllGuideFinish = false;
                break;
            }
        }
        if (isAllGuideFinish)
        {
            StaticData.isOpenGuide = false;
        }
    }
    private void Update()
    {
        //庄园第一次AVG没开始 不进行新手引导
        if (LocalInfoData.localManorInfoData == null)
        {
            return;
        }
        if (LocalInfoData.localManorInfoData.isFirstAvgDialogFinish == false)
        {
            return;
        }
        if (!isLoginGetInfoFinish)
        {//没拿到登录数据呢，返回
            return;
        }
        if (StaticData.isOpenGuide == false)
        {
            return;
        }
        if (isAllGuideFinish)//所有引导完成了返回
        {
            return;
        }
        //检查引导触发条件
        CheckAllGuideCondition();
        //引导触发检测
        CheckLittleStepTrigger();
    }

    private void CheckLittleStepTrigger()
    {
        bool isHaveTrigger = false;
        for (int i = 0; i < listGuideLittleStep.Count; i++)
        {
            if (listGuideLittleStep[i].IsFinish == false)//引导没完成
            {
                //判定条件
                bool isConditionMet = true;
                List<int> conditionIds= listGuideLittleStep[i].TriggerConditions;
                for (int eachConditionIdx = 0; eachConditionIdx < conditionIds.Count; eachConditionIdx++)
                {
                    GuideTriggerConditionDefine conditonDefine=listGuideConditions.Find(x => x.Id == conditionIds[eachConditionIdx]);
                    if (conditonDefine.isMet == false)
                    {
                        isConditionMet = false;
                        break;
                    }
                }
                if (isConditionMet)//条件满足，触发引导
                {
                    //触发到最新一个
                    BeginCurrGuideStep(listGuideLittleStep[i]);
                    isHaveTrigger = true;
                }

            }
        }
        //如果条件不满足了，比如施肥的已经变成了幼苗，关闭手指动画
        if (isHaveTrigger == false)
        {
            uiGuideComponent.GoUIGuide.SetActive(false);
            //不满足条件了，重置，以便下次触发判定
            CurrExecuteGuideLittleStepId = 0;
            //如果当前正在进行种植 拖出会关闭面板，条件不重置 
            if (isCurrStepGuiding && CurrExecuteGuideLittleStepDefine.Id == 10003)
            {
                return;
            }
            isCurrStepGuiding = false;
        }
    }

    //小步骤
    private async void  BeginCurrGuideStep(GuideLittleStepDefine guideLittleStepDefine)
    {
        if (CurrExecuteGuideLittleStepId == guideLittleStepDefine.Id)
        {
            //同一个id的新手引导不重复触发
            return;
        }
        //特殊处理
        if (CurrExecuteGuideLittleStepId>guideLittleStepDefine.Id)//后边来的先触发，前边的不触发了
        {
            //后边的条件不满足了，要触发前边的
            CurrExecuteGuideLittleStepId = 0;
            return;
        }
        isCurrStepGuiding = true;
        StaticData.DebugGreen($"======================触发引导：{guideLittleStepDefine.Id}");
        CurrExecuteGuideLittleStepDefine = guideLittleStepDefine;
        CurrExecuteGuideLittleStepId = CurrExecuteGuideLittleStepDefine.Id;
        //面板
        if (guideLittleStepDefine.isPanelAnim)
        {
            var goGuidePanel=await UIComponent.CreateUIAsync(guideLittleStepDefine.PrefabPanelAnim,isGuideCanvas:true);
            goGuidePanel.GetComponent<GuidePanelAnimComponent>().SetInfo(guideLittleStepDefine.PrefabPanelAnim);
            return;
        }
        //等对应的组件找到
        int idCurrLittleStep = guideLittleStepDefine.Id;
        GameObject targetGameObject = null;
        while (targetGameObject == null)
        {
            await UniTask.DelayFrame(1);
            targetGameObject = GameObject.Find(guideLittleStepDefine.SubPath);
            //针对Tile的对象池特殊处理
            if (guideLittleStepDefine.Id == 10002)
            {
                if (targetGameObject != null)
                {
                    var tiles = targetGameObject.transform.parent.GetComponentsInChildren<TileComponent>();
                    targetGameObject = null;
                    if (tiles.Length>1)
                    {
                        targetGameObject = tiles[0].gameObject;
                    }
                }
            }

            if (targetGameObject!=null&& !targetGameObject.activeInHierarchy)
            {
                //如果找到了控件，但是没有激活，重新找
                targetGameObject = null;
            }
            
        }
        switch (guideLittleStepDefine.Id)
        {
            case 10002:
                var tiles = targetGameObject.transform.parent.GetComponentsInChildren<TileComponent>();
                List<TileComponent> listTiles = new List<TileComponent>();
                for (int i = 0; i < tiles.Length; i++)
                {
                    if (tiles[i].gameObject.activeInHierarchy && tiles[i].regionId == 1)//第一个区域上的
                    {
                        listTiles.Add(tiles[i]);
                    }
                }
                //找区域1上最左侧的土地
                listTiles.Sort((a, b) => a.gameObject.transform.localPosition.x.CompareTo(b.gameObject.transform.localPosition.x));
                targetGameObject = listTiles[0].gameObject;
                break;
            case 10003:
                break;
            case 10004:
                var tileSeed = Root2dSceneManager._instance.GetTileSeed();
                targetGameObject = tileSeed.gameObject;
                break;
            case 10005:
                var tileRipe = Root2dSceneManager._instance.GetTileRipe();
                targetGameObject = tileRipe.gameObject;
                break;
            case 10011:
                var goodCom=targetGameObject.transform.GetComponentInParent<UIPoolItemGood>();
                targetGameObject=goodCom.transform.parent.GetChild(1).GetComponent<UIPoolItemGood>()._buyBtn.gameObject;
                break;
            default:
                break;
        }
        //找到了组件
        uiGuideComponent.SetUIInfo(guideLittleStepDefine, targetGameObject);
        uiGuideComponent.GoUIGuide.SetActive(true);
    }

    //用户操作后设置小步骤完成
    public void SetLittleStepFinish()
    {
        isCurrStepGuiding = false;
        int idLittleStep = CurrExecuteGuideLittleStepDefine.Id;
        uiGuideComponent.GoUIGuide.SetActive(false);
        //标记小步骤完成
        GuideLittleStepDefine guideLittleStepDefine = listGuideLittleStep.Find(x => x.Id == idLittleStep);
        if (guideLittleStepDefine != null)
        {
            guideLittleStepDefine.IsFinish = true;
        }
        SaveLittleStepToServer(idLittleStep);
        //同一个引导不同地方触发的，完成一个都算完成
        switch (idLittleStep)
        {
            case 10007:
                //标记小步骤完成
                GuideLittleStepDefine guideLittleStepDefine11007 = listGuideLittleStep.Find(x => x.Id == 11007);
                if (guideLittleStepDefine11007 != null)
                {
                    guideLittleStepDefine11007.IsFinish = true;
                }
                SaveLittleStepToServer(11007);
                break;
            case 11007:
                //标记小步骤完成
                GuideLittleStepDefine guideLittleStepDefine10007 = listGuideLittleStep.Find(x => x.Id == 10007);
                if (guideLittleStepDefine10007 != null)
                {
                    guideLittleStepDefine10007.IsFinish = true;
                }
                SaveLittleStepToServer(10007);
                break;
            default:
                break;
        }
    }

    private void CheckAllGuideCondition()
    {
        
        for (int i = 0; i < listGuideConditions.Count; i++)
        {//小步骤是否完成的条件
            if (listGuideConditions[i].LittleStepId>0)
            {
                var guideLittleStepDefine = listGuideLittleStep.Find(x => x.Id == listGuideConditions[i].LittleStepId);
                if (guideLittleStepDefine != null)
                {
                    if (guideLittleStepDefine.IsFinish && listGuideConditions[i].isCompelete == true)
                    {
                        listGuideConditions[i].isMet = true;
                    }
                    if (guideLittleStepDefine.IsFinish == false && listGuideConditions[i].isCompelete == false)
                    {
                        listGuideConditions[i].isMet = true;
                    }
                }
            }
            //等级是否完成的条件
            if (listGuideConditions[i].LevelReach>0)
            {
                int levelPlayer = StaticData.GetPlayerLevelAndCurrExp().level;
                if (levelPlayer >= listGuideConditions[i].LevelReach)
                {
                    listGuideConditions[i].isMet = true;
                }
            }
            //累计收获果实数量的条件
            if (listGuideConditions[i].FruitTotalGain > 0)
            {
                int totalGainedCount = 0;
                if (LocalInfoData.localManorInfoData != null)
                {
                    totalGainedCount = LocalInfoData.localManorInfoData.countTotalGain;
                }
                if (totalGainedCount >= listGuideConditions[i].FruitTotalGain)
                {
                    listGuideConditions[i].isMet = true;
                }
            }
            //获取当前果实数量

            if (listGuideConditions[i].FruitHave > 0)
            {
                int countHaveFruit = StaticData.GetCurrWareHouseFruitCount();
                if (countHaveFruit >= listGuideConditions[i].FruitHave)
                {
                    listGuideConditions[i].isMet = true;
                }
            }
        }
        //仓库界面打开
        var conditonInOrderNotOtherOpen = listGuideConditions.Find(x => x.Id == 30016);
        conditonInOrderNotOtherOpen.isMet = false;
        bool isHaveUIOrder = UIComponent.IsHaveUI(UIType.UIDeal);
        if (isHaveUIOrder)
        {
            GameObject goUI = UIComponent.GetExistUI(UIType.UIDeal);
            var dealController=UIComponent.GetComponentHaveExist<UIDealController>(UIType.UIDeal);
            if (goUI.activeInHierarchy && isLastChild(goUI.transform))
            {
                conditonInOrderNotOtherOpen.isMet = true;
            }
        }
        //升级界面种子解锁界面打开
        var conditonUISeedUnLock = listGuideConditions.Find(x => x.Id == 30013);
        conditonUISeedUnLock.isMet = false;
        SetUIExistConditionMet(conditonUISeedUnLock, UIType.UISeedUnlock);
        //庄园好友界面打开
        var conditonManorFriendOpen= listGuideConditions.Find(x => x.Id == 30022);
        conditonManorFriendOpen.isMet = false;
        SetUIExistConditionMet(conditonManorFriendOpen, UIType.UIFriend);
        
        //庄园奖励面板章节解锁打开
        var conditonUIChapterUnLock = listGuideConditions.Find(x => x.Id == 30015);
        conditonUIChapterUnLock.isMet = false;
        SetUIExistConditionMet(conditonUIChapterUnLock, UIType.UIChapterUnlock);
        //等级提升界面
        var conditonUILevelUp = listGuideConditions.Find(x => x.Id == 30019);
        conditonUILevelUp.isMet = false;
        SetUIExistConditionMet(conditonUILevelUp, UIType.UILevelUp);
        //商店界面打开,并且是种子标签页
        var conditonUIShop = listGuideConditions.Find(x => x.Id == 30017);
        conditonUIShop.isMet = false;
        bool isHaveUIShop = UIComponent.IsHaveUI(UIType.UIShop);
        if (isHaveUIShop)
        {
            GameObject goUI = UIComponent.GetExistUI(UIType.UIShop);
            var shopCtl = UIComponent.GetComponentHaveExist<UIShopComponent>(UIType.UIShop);
            if (goUI.activeInHierarchy && isLastChild(goUI.transform)&& shopCtl.CurrshopTabTag==UIShopComponent.ShopTabTags.TabSeed)
            {
                conditonUIShop.isMet = true;
            }
        }
        //商店界面购买个数弹窗没打开
        var conditonUIShopBuyNum = listGuideConditions.Find(x => x.Id == 30024);
        conditonUIShopBuyNum.isMet = false;
        bool isHaveUI = UIComponent.IsHaveUI(UIType.UIShop);
        if (isHaveUI)
        {
            GameObject goUI = UIComponent.GetExistUI(UIType.UIShop);
            if (goUI.activeInHierarchy && isLastChild(goUI.transform)&& goUI.GetComponent<UIShopComponent>()._buyAmountChoice.gameObject.activeInHierarchy==false)
            {
                conditonUIShopBuyNum.isMet = true;
            }
        }
        //庄园解锁支付道具界面
        var conditonUIManorRegionPay = listGuideConditions.Find(x => x.Id == 30014);
        conditonUIManorRegionPay.isMet = false;
        SetUIExistConditionMet(conditonUIManorRegionPay, UIType.UIManorRegionPay);
        //大富翁开始游戏界面
        var conditonUIRichManBeginGame = listGuideConditions.Find(x => x.Id == 30020);
        conditonUIRichManBeginGame.isMet = false;
        SetUIExistConditionMet(conditonUIRichManBeginGame, UIType.UIHomePage);
        //大富翁骰子界面打开
        var conditonUIRichManRandom = listGuideConditions.Find(x => x.Id == 30021);
        conditonUIRichManRandom.isMet = false;
        SetUIExistConditionMet(conditonUIRichManRandom, UIType.UIPlayInterface);
        //庄园未打开其它界面及窗口
        var conditonIngoManorNotOtherOpen = listGuideConditions.Find(x => x.Id == 30002);
        conditonIngoManorNotOtherOpen.isMet = false;
        //好友庄园一键收获激活状态
        var conditonManorFriendOneKeyGain = listGuideConditions.Find(x => x.Id == 30023);
        conditonManorFriendOneKeyGain.isMet = false;
        //作物面板打开
        var conditionPlantPanelOpen= listGuideConditions.Find(x => x.Id == 30003);
        conditionPlantPanelOpen.isMet = false;
        //处在成熟期
        var conditionPlantRipePeriod = listGuideConditions.Find(x => x.Id == 30004);
        conditionPlantRipePeriod.isMet = false;
        //处在种子期
        var conditionPlantSeedPeriod = listGuideConditions.Find(x => x.Id == 30005);
        conditionPlantSeedPeriod.isMet = false;
        bool isHaveManor = UIComponent.IsHaveUI(UIType.UIManor);
        if (isHaveManor)
        {
            GameObject goManor = UIComponent.GetExistUI(UIType.UIManor);
            if (goManor.activeInHierarchy 
                && isLastChild(goManor.transform)
                && isUIOpen(UIType.UISeedUnlock)==false
                && isUIOpen(UIType.UIChapterUnlock)==false
                &&isUIOpen(UIType.UILevelUp)==false)
            {
                if (Root2dSceneManager._instance!=null&&Root2dSceneManager._instance.isFriendManor == false)
                {
                    conditonIngoManorNotOtherOpen.isMet = true;
                }
                //作物面板条件
                if (StaticData.GetUIWorldHandleComponent()!=null)
                {
                    if (StaticData.GetUIWorldHandleComponent().goPlant.activeInHierarchy)
                    {
                        conditionPlantPanelOpen.isMet = true;
                    }
                }
            }
            if (Root2dSceneManager._instance!=null&& Root2dSceneManager._instance.isFriendManor&& goManor.GetComponent<UIManorComponent>().ButtonStealOneKey.gameObject.activeInHierarchy&& goManor.GetComponent<UIManorComponent>().ButtonStealOneKey.interactable)
            {
                conditonManorFriendOneKeyGain.isMet = true;
            }
            //处在种子期
            if (Root2dSceneManager._instance != null)
            {
                var seedCom=Root2dSceneManager._instance.GetTileSeed();
                if (seedCom != null)
                {
                    conditionPlantSeedPeriod.isMet = true;
                }
            }
            //处在成熟期
            if (Root2dSceneManager._instance != null)
            {
                var ripeCom = Root2dSceneManager._instance.GetTileRipe();
                if (ripeCom != null)
                {
                    conditionPlantRipePeriod.isMet = true;
                }
            }
            //庄园工棚打开
            var conditonWorkeShed = listGuideConditions.Find(x => x.Id == 30018);
            conditonWorkeShed.isMet = false;
            if (Root2dSceneManager._instance != null)
            {
                var goWorkShed=Root2dSceneManager._instance.manorUnLockWorkShedComponent.goWorkShed;
                if (goWorkShed != null&& goWorkShed.activeInHierarchy)
                {
                    conditonWorkeShed.isMet = true;
                }
            }
        }

    }

    public bool isUIOpen(string UIName)
    {
        bool isOpen = false;
        bool isHaveUI = UIComponent.IsHaveUI(UIName);
        if (isHaveUI)
        {
            GameObject goUI = UIComponent.GetExistUI(UIName);
            if (goUI.activeInHierarchy)
            {
                isOpen = true;
            }
        }
        return isOpen;
    }
    //设置界面打开条件满足
    public void SetUIExistConditionMet(GuideTriggerConditionDefine guideTriggerConditionDefine,string UIName)
    {
        bool isHaveUI = UIComponent.IsHaveUI(UIName);
        if (isHaveUI)
        {
            GameObject goUI = UIComponent.GetExistUI(UIName);
            if (goUI.activeInHierarchy && isLastChild(goUI.transform))
            {
                guideTriggerConditionDefine.isMet = true;
            }
        }
    }

    public bool isLastChild(Transform trans)
    {
        bool isLast=false;
        int index = trans.GetSiblingIndex();
        int num = trans.parent.childCount;
        num--;
        var TransLast=trans.parent.GetChild(num);
        //最后边的界面是隐藏的不算
        while (TransLast.gameObject.activeInHierarchy==false&& num>0)
        {
            TransLast = trans.parent.GetChild(num);
            if (TransLast.gameObject.activeInHierarchy == false)
            {
                num--;
            }
        }
        if (index == num)
        {
            isLast = true;
        }
        return isLast;
    }

    public void SaveLittleStepToServer(int littleStepId)
    {
        CSSaveGuidance csSaveGuidance = new CSSaveGuidance() { Guidance = littleStepId };
        ProtocalManager.Instance().SendCSSaveGuidance(csSaveGuidance, (succ) => {
            StaticData.DebugGreen($"=========================Guide:{littleStepId} 保存成功");
        },(error)=> { });
    }
}
