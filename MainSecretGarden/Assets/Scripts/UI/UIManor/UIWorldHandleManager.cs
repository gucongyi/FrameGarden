using Cysharp.Threading.Tasks;
using Game.Protocal;
using Google.Protobuf.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWorldHandleManager : MonoBehaviour
{
    #region 处理Tile的UI
    /*
     * 地块上操作：
     * 1.按下小地块快速滑动会滑动大地块
     * 2.长按小地块拖拽
     * 3.种植
     * 4.铲除，时间，施肥界面
     * 5.收获界面
     * 6.回收装饰物界面
     */
    public enum TypePointUI
    {
        Steal,
        ReBack,
        Plant,
        FirstFertilizer,
        GrowUp,
        Gain
    }
    public GameObject goPlant;
    public ScrollCameraWhenBoundsOutScreen scrollCameraWhenBoundsOutScreenPlant;
    public ScrollCameraWhenBoundsOutScreen scrollCameraWhenBoundsOutScreenGrowUp;
    public ScrollCameraWhenBoundsOutScreen scrollCameraWhenBoundsOutScreenGrowUpFriendManor;
    public ScrollCameraWhenBoundsOutScreen scrollCameraWhenBoundsOutScreenRebackDecorate;
    public ScrollCameraWhenBoundsOutScreen scrollCameraWhenBoundsOutScreenRebackDecorateAndRotate;
    public LoopHorizontalScrollRect loopHorizontalScrollRect;
    public PlantSeedDragComponent plantSeedDragComponent;
    [HideInInspector]
    public TileComponent currClickComponent;
    public ManorSelfGrowUpComponent SelfGrowUpComponent;
    //好友成长
    public GameObject goGrowUpFriendManor;
    //正在铲除
    [HideInInspector]
    public bool isEradicating;
    public GameObject GoEradicateEffect;
    [Header("化肥")]
    //种子第一阶段化肥直接显示
    public Transform TransGoFirstStageRoot;
    public GameObject GoFirstStageFertilizer;
    CSWareHouseStruct currFertilizerData;
    public UIFertilizerEffectComponent uiFertilizerEffectComponent;
    public GameObject prefabUiWatering;
    //正在施肥
    [HideInInspector]
    public bool isFertiliezeringAnimPlay;
    public Transform TransWateringParent;
    //进度条
    SeedGrowComponent seedGrowComponent;
    bool isShowGrowUpUI;
    //好友查看进度
    public Image imageSliderFriendManor;
    bool isShowGrowUpUIFriendManor;
    public Text textSeedNameFriendManor;
    public Text textRemainTimeFriendManor;
    ////种子名称
    string seedName;
    //剩余时间
    float remainTime;

    //收获
    public GameObject goGain;
    public Transform transParentGoGain;
    

    public CSHarvestData csHarvestData = new CSHarvestData();

    //回收装饰物
    public GameObject goReback;
    public Button ButtonReback;
    TileComponent currClickDecorateComponent;
    public long IdFriend;
    //回收装饰物加左右旋转装饰物的界面
    public GameObject goRebackAndRotate;
    public Button ButtonRebackInRotate;
    public Button ButtonRebackLeftRotate;
    public Button ButtonRebackRightRotate;
    public Button ButtonRotateOK;
    //是否正在处理装饰物
    public bool isRotatingDecorate;
    public class RotateInfo
    {
        public int idxDecorateRotate;
        public long SoilId;
        public int CropGoodId;
        public int regionId;
        public string modelName;
        //位置信息
        public Vector2 localPos;
        //旋转后是否能放置
        public bool isCanPlace;
    }
    //旋转之前的信息
    public RotateInfo BeforeRotateInfo = new RotateInfo();
    //旋转之后的信息
    public RotateInfo AfterRotateInfo = new RotateInfo();
    //工棚上的手
    public GameObject goHandleWorkShed;
    public class StealClass
    {
        public long SolidId;
        public bool isSteal;
    }
    public List<StealClass> listStealClass = new List<StealClass>();
    //偷取
    public GameObject goSteal;
    public Transform transParentGoSteal;
    List<CSStealStruct> ListStealInfo = new List<CSStealStruct>();
    //一键收取的场景中的手
    public Transform transParentOneKey;
    public GameObject goPrefabItemPlant;
    public GameObject goPrefabItemExp;
    public Transform tranParentExp;
    #endregion


    private void Awake()
    {
        isShowGrowUpUI = false;
        isShowGrowUpUIFriendManor = false;
        isFertiliezeringAnimPlay = false;
        //回收装饰物
        ButtonReback.onClick.RemoveAllListeners();
        ButtonReback.onClick.AddListener(OnButtonRebackClick);
        ButtonRebackInRotate.onClick.RemoveAllListeners();
        ButtonRebackInRotate.onClick.AddListener(OnButtonRebackClick);
        //装饰物旋转
        ButtonRebackLeftRotate.onClick.RemoveAllListeners();
        ButtonRebackLeftRotate.onClick.AddListener(OnButtonLeftRotateClick);
        ButtonRebackRightRotate.onClick.RemoveAllListeners();
        ButtonRebackRightRotate.onClick.AddListener(OnButtonRighteRotateClick);
        //确定旋转
        ButtonRotateOK.onClick.RemoveAllListeners();
        ButtonRotateOK.onClick.AddListener(()=>{ OnButtonRotateOKClick(); });
    }

    public void SetWorkShedHandleFinger(GameObject goWorkShed)
    {
        goHandleWorkShed.SetActive(goWorkShed.activeInHierarchy);
        goHandleWorkShed.GetComponent<RectTransform>().anchoredPosition = StaticData.ManorWorldPointToUICameraAnchorPos(goWorkShed.transform.position);
    }

    private void OnButtonRebackClick()
    {
        CSOrnamentalRecycle csOrnamentalRecycle = new CSOrnamentalRecycle()
        {
            SoilId = currClickDecorateComponent.SoilId
        };
        ManorProtocalHelper.SendOrnamentalRecycle(csOrnamentalRecycle, (succ) =>
         {
             //更新仓库装饰物
             StaticData.UpdateWareHouseItem(currClickDecorateComponent.CropGoodId, 1);
             //回收完更新装饰物界面
             UIManorComponent uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
             uiManorComponent.RefreshDecorateList();

             //回收装饰物之前关闭提示，否则会把这个提示删掉
             TipsTileComponent._instance.CloseAll();
             Destroy(currClickDecorateComponent.gameObject);
             goReback.SetActive(false);
             goRebackAndRotate.SetActive(false);
         });
    }
    //装饰物左旋
    private void OnButtonLeftRotateClick()
    {
        RotateDecorate(true);
    }
    //装饰物右旋
    private void OnButtonRighteRotateClick()
    {
        RotateDecorate(false);
    }
    //确认旋转
    public async void OnButtonRotateOKClick(bool isAutoPlayCloseDecorate = true)
    {
        if (isAutoPlayCloseDecorate)
        {
            //关闭装饰物界面
            UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor).uiManorAnim.PlayAnimCloseDecorate();
        }
        if (AfterRotateInfo.SoilId<=0)//没有旋转过
        {
            //关闭界面
            goRebackAndRotate.SetActive(false);
            //关闭提示
            TipsTileComponent._instance.CloseAll();
            isRotatingDecorate = false;
            return;
        }
        if (AfterRotateInfo.isCanPlace)
        {//能放，提交服务器
            CSManorDecorateRotate csManorDecorateRotate = new CSManorDecorateRotate()
            {
                SoilId = (int)AfterRotateInfo.SoilId,
                RotateIndex = AfterRotateInfo.idxDecorateRotate
            };
            ManorProtocalHelper.SendManorDecorateRotate(csManorDecorateRotate, async (succ) =>
            {
                if (isAutoPlayCloseDecorate)
                {
                    //关闭界面
                    goRebackAndRotate.SetActive(false);
                    isRotatingDecorate = false;
                }
                //关闭提示
                TipsTileComponent._instance.CloseAll();
                //排序
                Root2dSceneManager._instance.UpdateSortLayer(true);
            }, async (error) => {
                //旋转成最开始的样子
                await ResetToOldDecorate(isAutoPlayCloseDecorate);
            });
        }
        else
        {//不能放，旋转成最开始的样子
            await ResetToOldDecorate(isAutoPlayCloseDecorate);
        }

    }

    private async System.Threading.Tasks.Task ResetToOldDecorate(bool isAutoPlayCloseDecorate = true)
    {
        if (isAutoPlayCloseDecorate)
        {
            isRotatingDecorate = false;
            //关闭界面
            goRebackAndRotate.SetActive(false);
        }
        //关闭提示
        TipsTileComponent._instance.CloseAll();
        //旋转
        //删除现在的装饰物
        Destroy(currClickDecorateComponent.gameObject);
        //生成老的装饰物
        var goDecorate = await Root2dSceneManager._instance.GenerateDecorate(BeforeRotateInfo.modelName, BeforeRotateInfo.CropGoodId);
        goDecorate.GetComponent<TileComponent>().SetDecorateInfo(BeforeRotateInfo.idxDecorateRotate, BeforeRotateInfo.SoilId, BeforeRotateInfo.CropGoodId, BeforeRotateInfo.regionId);
        goDecorate.transform.localPosition = BeforeRotateInfo.localPos;
        //重新设置当前的组件
        currClickDecorateComponent = goDecorate.GetComponent<TileComponent>();
        //重置旋转后的信息
        AfterRotateInfo = new RotateInfo();
        //排序
        Root2dSceneManager._instance.UpdateSortLayer(true);
    }

    
    private async void RotateDecorate(bool isLeft)
    {
        var listModel = StaticData.configExcel.GetDecorateByDecorateId(currClickDecorateComponent.CropGoodId).Model;
        int idxPreRotate = currClickDecorateComponent.idxDecorateRotate;

        int idxCurrRotate = 0;
        if (isLeft)
        {//左旋
            idxCurrRotate = idxPreRotate - 1;
            if (idxCurrRotate < 0)
            {
                idxCurrRotate = listModel.Count - 1;
            }
        }
        else
        {//右旋
            idxCurrRotate = idxPreRotate + 1;
            if (idxCurrRotate > listModel.Count - 1)
            {
                idxCurrRotate = 0;
            }
        }

        //记录旋转之后的信息
        if (currClickDecorateComponent != null)
        {
            AfterRotateInfo.idxDecorateRotate = idxCurrRotate;
            AfterRotateInfo.SoilId = currClickDecorateComponent.SoilId;
            AfterRotateInfo.CropGoodId = currClickDecorateComponent.CropGoodId;
            AfterRotateInfo.regionId = currClickDecorateComponent.regionId;
            AfterRotateInfo.modelName = listModel[idxCurrRotate];
            AfterRotateInfo.localPos = currClickDecorateComponent.transform.localPosition;
            //删除原来的装饰物
            Destroy(currClickDecorateComponent.gameObject);
        }
        //旋转
        //生成新的装饰物
        var goDecorate = await Root2dSceneManager._instance.GenerateDecorate(AfterRotateInfo.modelName, AfterRotateInfo.CropGoodId);
        goDecorate.GetComponent<TileComponent>().SetDecorateInfo(AfterRotateInfo.idxDecorateRotate, AfterRotateInfo.SoilId, AfterRotateInfo.CropGoodId, AfterRotateInfo.regionId);
        goDecorate.transform.localPosition = AfterRotateInfo.localPos;
        //重新设置当前的组件
        currClickDecorateComponent = goDecorate.GetComponent<TileComponent>();
        currClickDecorateComponent.CheckIsCanPlace(() =>
        {
            AfterRotateInfo.isCanPlace = false;
            //显示红色箭头
            TipsTileComponent._instance.SetCanPlace(currClickDecorateComponent.transform, false, currClickDecorateComponent);
        }, () =>
        {
            AfterRotateInfo.isCanPlace = true;
            //显示蓝色箭头
            TipsTileComponent._instance.SetCanPlace(currClickDecorateComponent.transform, true, currClickDecorateComponent);
        });
    }

    

    // Start is called before the first frame update
    void Start()
    {

    }
    public async void PlayItemHarvestEffect(TileComponent tile,int addCount)
    {
        var goPlant = ResourcesHelper.InstantiatePrefabFromABSetDefaultNotStretch(goPrefabItemPlant, transParentOneKey);
        //设置位置
        goPlant.GetComponent<RectTransform>().anchoredPosition = StaticData.ManorWorldPointToUICameraAnchorPos(tile.transform.position);
        //设置图片个数
        int cropId = tile.CropGoodId;
        int gameDefineId = StaticData.configExcel.GetManorCropByCropId(cropId).IdGainGameItem;
        string iconName = StaticData.configExcel.GetGameItemByID(gameDefineId).Icon;
        goPlant.GetComponent<UIItemPlantEffect>().ShowInfo(iconName, addCount);
    }
    public void PlayExpAnim(TileComponent tile,int addCount)
    {
        var goExp = ResourcesHelper.InstantiatePrefabFromABSetDefaultNotStretch(goPrefabItemExp, tranParentExp);
        //设置位置
        goExp.GetComponent<RectTransform>().anchoredPosition = StaticData.ManorWorldPointToUICameraAnchorPos(tile.transform.position)+new Vector2(-50f,50f);
        goExp.GetComponent<UIItemExpEffect>().ShowInfo(addCount);
    }

    public void PlayWateringAnim(TileComponent tile)
    {
        var goWatering = ResourcesHelper.InstantiatePrefabFromABSetDefaultNotStretch(prefabUiWatering, TransWateringParent);
        //设置位置
        goWatering.GetComponent<RectTransform>().anchoredPosition = StaticData.ManorWorldPointToUICameraAnchorPos(tile.transform.position);
        goWatering.SetActive(true);
    }

    public void OnButtonGainClick(SeedGrowComponent seedGrowComponent)
    {
        GainOneTile(seedGrowComponent);
    }
    //从一块地上收获
    public void GainOneTile(SeedGrowComponent willSeedGrowComponent)
    {
        csHarvestData.HarvestInfo.Clear();//只保存当前的这个可收获的
        CSHarvestStruct csHarvestStruct = new CSHarvestStruct()
        {
            SoilId = willSeedGrowComponent.tileComponent.SoilId
        };
        csHarvestData.HarvestInfo.Add(csHarvestStruct);
        ManorProtocalHelper.ManorHarvest(csHarvestData, async (succ) =>
        {
            CloseAll();
            int cropId = willSeedGrowComponent.tileComponent.CropGoodId;
            int gameDefineId = StaticData.configExcel.GetManorCropByCropId(cropId).IdGainGameItem;
            int addCount = 0;
            for (int j = 0; j < succ.HarvestResult.Count; j++)
            {
                if (succ.HarvestResult[j].HarvestId == gameDefineId)
                {
                    addCount = succ.HarvestResult[j].HarvestNum;
                    break;
                }
            }
            PlayItemHarvestEffect(willSeedGrowComponent.tileComponent, addCount);
            int addExp = 0;
            for (int i = 0; i < succ.HarvestResult.Count; i++)
            {
                addExp = succ.HarvestResult[i].HarvestExperience;
            }
            PlayExpAnim(willSeedGrowComponent.tileComponent, addExp);
            willSeedGrowComponent.tileComponent.CropGoodId = 0;
            //删除
            Destroy(willSeedGrowComponent.gameObject);
            willSeedGrowComponent.tileComponent.SetCurrGoPlant(null);
            //删除seedGrowCom
            Root2dSceneManager._instance.RemoveFromListSeedGrow(willSeedGrowComponent);
            willSeedGrowComponent = null;
            csHarvestData.HarvestInfo.Clear();
            //更新仓库 2020/9/29 huangjiangdong 将收获果实数据入库提前
            for (int i = 0; i < succ.HarvestResult.Count; i++)
            {
                //更新仓库果实数量
                StaticData.UpdateWareHouseItem(succ.HarvestResult[i].HarvestId, succ.HarvestResult[i].HarvestNum);
            }
        });
    }
    public void OnButtonStealClick(SeedGrowComponent willSeedGrowComponent)
    {
        CSStealData csStealData = new CSStealData();
        CSStealStruct csStealStruct = new CSStealStruct()
        {
            SoilId= willSeedGrowComponent.tileComponent.SoilId
        };
        csStealData.StealInfo.Add(csStealStruct);
        csStealData.StealUid = IdFriend;
        ManorProtocalHelper.StealFriendFruit(csStealData, async (scStealData) =>
        {
            StaticData.DataDot(Company.Cfg.DotEventId.FriendManorStealSucc);
            //已经偷取过的，设置状态为不能偷
            if (scStealData == null)
            {
                return;
            }
            if (scStealData.StealResult == null)
            {
                return;
            }
            if (scStealData.StealResult.Count <= 0)
            {
                return;
            }
            //偷取特效
            CloseAll();
            //表示能偷
            TileComponent tile = null;
            for (int i = 0; i < Root2dSceneManager._instance.objPool.transform.childCount; i++)
            {
                var tileGo = Root2dSceneManager._instance.objPool.transform.GetChild(i);
                TileComponent tileCom = tileGo.GetComponent<TileComponent>();
                if (tileGo.gameObject.activeInHierarchy && tileCom.SoilId == scStealData.StealResult[0].SoilId)
                {
                    tile = tileGo.GetComponent<TileComponent>();
                    break;
                }
            }
            
            int cropId = tile.CropGoodId;
            int gameDefineId = StaticData.configExcel.GetManorCropByCropId(cropId).IdGainGameItem;
            int addCount = 0;
            for (int j = 0; j < scStealData.StealResult.Count; j++)
            {
                if (scStealData.StealResult[j].StealId == gameDefineId)
                {
                    addCount = scStealData.StealResult[j].StealNum;
                    break;
                }
            }
            //播放偷取特效
            PlayItemHarvestEffect(tile, addCount);
            for (int i = 0; i < scStealData.StealResult.Count; i++)
            {
                var stealClass = this.listStealClass.Find(x => x.SolidId == scStealData.StealResult[i].SoilId);
                if (stealClass != null)
                {
                    stealClass.isSteal = false;
                }
                //更新仓库果实数量
                StaticData.UpdateWareHouseItem(scStealData.StealResult[i].StealId, scStealData.StealResult[i].StealNum);
            }
            SetFriendManorStealState(this.listStealClass);
        });
    }
    //设置可以所有地块可以偷取的状态
    public void SetFriendManorStealState(List<StealClass> listStealClass)
    {//只设置第一个的状态
        if (Root2dSceneManager._instance.isFriendManor == false)
        {
            return;
        }
        if (listStealClass == null)
        {
            return;
        }
        this.listStealClass = listStealClass;
        bool isCanSteal = false;

        if (listStealClass != null && listStealClass.Count > 0)
        {
            
            for (int j = 0; j < listStealClass.Count; j++)
            {
                //表示能偷
                if (listStealClass[j].isSteal)
                {
                    isCanSteal = true;
                    break;
                }
            }
        }

        UIManorComponent uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
        //能偷，设置状态
        if (isCanSteal)
        {
            uiManorComponent.PlayOneKeyGainEffect(true);
        }
        else
        {
            uiManorComponent.PlayOneKeyGainEffect(false);
        }
    }
    public void OnButtonGainOrStealOneKeyClick()
    {
        if (!Root2dSceneManager._instance.isFriendManor)
        {//一键收取
            var listSeedGrow = Root2dSceneManager._instance.GetListSeedGrow();
            var listRipe = listSeedGrow.FindAll(x => x.currCropPeriod == SeedGrowComponent.PeriodGrow.Ripe);
            if (listRipe != null && listRipe.Count > 0)
            {
                CSHarvestData csHarvestData = new CSHarvestData();
                for (int i = 0; i < listRipe.Count; i++)
                {
                    CSHarvestStruct csHarvestStruct = new CSHarvestStruct()
                    {
                        SoilId = listRipe[i].tileComponent.SoilId
                    };
                    csHarvestData.HarvestInfo.Add(csHarvestStruct);
                }
                ManorProtocalHelper.ManorHarvest(csHarvestData, async (succ) =>
                {
                    CloseAll();
                    //一键收取特效
                    for (int i = 0; i < listRipe.Count; i++)
                    {
                        int cropId = listRipe[i].tileComponent.CropGoodId;
                        int gameDefineId = StaticData.configExcel.GetManorCropByCropId(cropId).IdGainGameItem;
                        int addCount = 0;
                        for (int j = 0; j < succ.HarvestResult.Count; j++)
                        {
                            if (succ.HarvestResult[j].HarvestId == gameDefineId)
                            {
                                addCount = succ.HarvestResult[j].HarvestNum;
                                break;
                            }
                        }
                        PlayItemHarvestEffect(listRipe[i].tileComponent, addCount);
                        int addExp = 0;
                        for (int k = 0; k < succ.HarvestResult.Count; k++)
                        {
                            addExp = succ.HarvestResult[k].HarvestExperience;
                        }
                        PlayExpAnim(listRipe[i].tileComponent, addExp);
                    }
                    for (int i = 0; i < listRipe.Count; i++)
                    {
                        listRipe[i].tileComponent.CropGoodId = 0;
                        //删除
                        Destroy(listRipe[i].gameObject);
                        listRipe[i].tileComponent.SetCurrGoPlant(null);
                        //删除seedGrowCom
                        Root2dSceneManager._instance.RemoveFromListSeedGrow(listRipe[i]);
                    }
                    //更新仓库
                    for (int i = 0; i < succ.HarvestResult.Count; i++)
                    {
                        //更新仓库果实数量
                        StaticData.UpdateWareHouseItem(succ.HarvestResult[i].HarvestId, succ.HarvestResult[i].HarvestNum);
                    }
                });
            }
        }
        else
        {//一键偷取
            ListStealInfo.Clear();
            for (int i = 0; i < listStealClass.Count; i++)
            {
                if (listStealClass[i].isSteal)
                {
                    ListStealInfo.Add(new CSStealStruct() { SoilId = listStealClass[i].SolidId });
                }
            }
            CSStealData csStealData = new CSStealData();
            for (int i = 0; i < ListStealInfo.Count; i++)
            {
                csStealData.StealInfo.Add(ListStealInfo[i]);
            }
            csStealData.StealUid = IdFriend;
            ManorProtocalHelper.StealFriendFruit(csStealData, async (scStealData) =>
            {
                StaticData.DataDot(Company.Cfg.DotEventId.FriendManorStealSucc);
                //已经偷取过的，设置状态为不能偷
                if (scStealData == null)
                {
                    return;
                }
                if (scStealData.StealResult == null)
                {
                    return;
                }
                if (scStealData.StealResult.Count <= 0)
                {
                    return;
                }
                //偷取特效
                CloseAll();
                for (int i = 0; i < scStealData.StealResult.Count; i++)
                {

                    var stealClass = this.listStealClass.Find(x => x.SolidId == scStealData.StealResult[i].SoilId);
                    if (stealClass != null)
                    {
                        //播放特效
                        TileComponent tile = null;
                        for (int j = 0; j < Root2dSceneManager._instance.objPool.transform.childCount; j++)
                        {
                            var tileGo = Root2dSceneManager._instance.objPool.transform.GetChild(j);
                            TileComponent tileCom = tileGo.GetComponent<TileComponent>();
                            if (tileGo.gameObject.activeInHierarchy && tileCom.SoilId == stealClass.SolidId)
                            {
                                tile = tileGo.GetComponent<TileComponent>();
                                break;
                            }
                        }
                        //设置图片个数
                        int cropId = tile.CropGoodId;
                        int gameDefineId = StaticData.configExcel.GetManorCropByCropId(cropId).IdGainGameItem;
                        int addCount = 0;
                        for (int j = 0; j < scStealData.StealResult.Count; j++)
                        {
                            if (scStealData.StealResult[j].StealId == gameDefineId)
                            {
                                addCount = scStealData.StealResult[j].StealNum;
                                break;
                            }
                        }
                        PlayItemHarvestEffect(tile, addCount);
                    }
                }
                for (int i = 0; i < scStealData.StealResult.Count; i++)
                {

                    var stealClass = this.listStealClass.Find(x => x.SolidId == scStealData.StealResult[i].SoilId);
                    if (stealClass != null)
                    {
                        stealClass.isSteal = false;
                    }
                    //更新仓库果实数量
                    StaticData.UpdateWareHouseItem(scStealData.StealResult[i].StealId, scStealData.StealResult[i].StealNum);
                }
                SetFriendManorStealState(this.listStealClass);
            });

        }

    }
    public void OpenRebackDecorate(TileComponent tileComponent)
    {
        if (isRotatingDecorate)
        {//之前有打开旋转界面，先处理掉
            OnButtonRotateOKClick(false);
            StaticData.GetUIWorldHandleComponent().SetHandleTileUIClose();
        }

        isRotatingDecorate = true;
        //打开装饰物界面
        UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor).uiManorAnim.PlayAnimOpenDecorate();
        currClickDecorateComponent = tileComponent;
        //根据装饰物是否有多个下标 显示不同的界面
        var listModel=StaticData.configExcel.GetDecorateByDecorateId(tileComponent.CropGoodId).Model;
        if (listModel.Count > 1)
        {
            //显示旋转加回收界面
            SetWorldPos(tileComponent, goRebackAndRotate, TypePointUI.ReBack);
            goRebackAndRotate.SetActive(true);
            scrollCameraWhenBoundsOutScreenRebackDecorateAndRotate.PlayCameraAnimWhenOutScreen(goRebackAndRotate, tileComponent, TypePointUI.ReBack);
            //记录旋转之前的信息
            BeforeRotateInfo.idxDecorateRotate = currClickDecorateComponent.idxDecorateRotate;
            BeforeRotateInfo.SoilId = currClickDecorateComponent.SoilId;
            BeforeRotateInfo.CropGoodId = currClickDecorateComponent.CropGoodId;
            BeforeRotateInfo.regionId = currClickDecorateComponent.regionId;
            BeforeRotateInfo.modelName = listModel[BeforeRotateInfo.idxDecorateRotate];
            BeforeRotateInfo.localPos = tileComponent.transform.localPosition;
        }
        else
        {
            //显示回收界面
            SetWorldPos(tileComponent, goReback, TypePointUI.ReBack);
            goReback.SetActive(true);
            scrollCameraWhenBoundsOutScreenRebackDecorate.PlayCameraAnimWhenOutScreen(goReback, tileComponent, TypePointUI.ReBack);
        }
        
    }
    public void OpenRootPlant(TileComponent tileComponent)
    {
        //打开种植的时候情况列表
        Root2dSceneManager._instance.PlantData.PlantInfo.Clear();
        currClickComponent = tileComponent;
        SetWorldPos(tileComponent, goPlant, TypePointUI.Plant);
        goPlant.SetActive(true);

        var listSeed = StaticData.GetPlantSeeds();
        loopHorizontalScrollRect.ClearCells();
        loopHorizontalScrollRect.totalCount = listSeed.Count;
        //计算适配器初始值
        loopHorizontalScrollRect.GetComponent<UIPlantScrollViewAdapter>().CalcAdapterSizeByCount(listSeed.Count);
        //只有是显示状态刷新才有效
        loopHorizontalScrollRect.RefillCells();
        scrollCameraWhenBoundsOutScreenPlant.PlayCameraAnimWhenOutScreen(goPlant, tileComponent, TypePointUI.Plant);
    }
    public void OnButtonEradicateClick()
    {
        string desc = StaticData.GetMultilingual(120065);
        StaticData.OpenCommonTips(desc, 120064, EradicatePlant);
    }

    private void EradicatePlant()
    {
        if (seedGrowComponent!=null&& seedGrowComponent.currCropPeriod==SeedGrowComponent.PeriodGrow.Ripe)
        {
            StaticData.CreateToastTips($"作物已成熟，不能铲除！");
            SetHandleTileUIClose();
            return;
        }
        CSEradicate csEradicate = new CSEradicate()
        {
            SoilId = currClickComponent.SoilId
        };
        ManorProtocalHelper.ManorEradicatePlant(csEradicate, async (succ) =>
        {
            //关闭UI
            SetHandleTileUIClose();
            //铲除特效
            GoEradicateEffect.GetComponent<RectTransform>().anchoredPosition = StaticData.ManorWorldPointToUICameraAnchorPos(currClickComponent.transform.position);
            GoEradicateEffect.SetActive(true);
            isEradicating = true;
            await UniTask.Delay(500);
            GoEradicateEffect.SetActive(false);
            isEradicating = false;
            //播放音效点击
            GameSoundPlayer.Instance.PlaySoundEffect(MusicHelper.SoundEffectEradicate);
            currClickComponent.CropGoodId = 0;
            //删除
            if (seedGrowComponent != null)
            {
                Destroy(seedGrowComponent.gameObject);
            }
            currClickComponent.SetCurrGoPlant(null);
            //删除seedGrowCom
            Root2dSceneManager._instance.RemoveFromListSeedGrow(this.seedGrowComponent);
            this.seedGrowComponent = null;
        });
    }

    internal void SetGrowUpInfo(TileComponent tileComponent, SeedGrowComponent seedGrowComponent)
    {
        this.seedGrowComponent = seedGrowComponent;
        currClickComponent = tileComponent;
        var cropDefine = StaticData.configExcel.GetManorCropByCropId(tileComponent.CropGoodId);
        seedName = StaticData.GetMultiLanguageByGameItemId(cropDefine.IdGainGameItem);
        if (Root2dSceneManager._instance.isFriendManor)
        {
            isShowGrowUpUIFriendManor = true;
            SetWorldPos(tileComponent, goGrowUpFriendManor, TypePointUI.GrowUp);
        }
        else
        {
            if (!isFertiliezeringAnimPlay)
            {
                SetWorldPos(tileComponent, SelfGrowUpComponent.gameObject, TypePointUI.GrowUp);
                SelfGrowUpComponent.seedGrowComponent = seedGrowComponent;
                isShowGrowUpUI = true;
            }
        }
    }
    
    void CloseAll()
    {
        goGrowUpFriendManor.SetActive(false);
        SelfGrowUpComponent.gameObject.SetActive(false);
        goPlant.SetActive(false);
        goReback.SetActive(false);
        if (!isRotatingDecorate)
        {
            goRebackAndRotate.SetActive(false);
        }
        isShowGrowUpUIFriendManor = false;
        isShowGrowUpUI = false;
    }

    //操作单块地的UI操作
    public void SetHandleTileUIClose()
    {
        CloseAll();
    }
    private void CheckGain()
    {
        var listSeedGrow = Root2dSceneManager._instance.GetListSeedGrow();
        UIManorComponent uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
        if (uiManorComponent == null)
        {
            return;//正在加载中
        }
        if (listSeedGrow.Count > 0)
        {
            var listRipe = listSeedGrow.FindAll(x => x.currCropPeriod == SeedGrowComponent.PeriodGrow.Ripe);

            if (listRipe != null && listRipe.Count > 0)
            {
                if (!Root2dSceneManager._instance.isFriendManor)
                {
                    Root2dSceneManager._instance.GetManorBubbleComponent().AddTipsPertinence(20001);
                }
                uiManorComponent.PlayOneKeyGainEffect(true);
            }
            else
            {
                if (!Root2dSceneManager._instance.isFriendManor)
                {
                    Root2dSceneManager._instance.GetManorBubbleComponent().RemoveTipsPertinenceAll(20001);
                }
                uiManorComponent.PlayOneKeyGainEffect(false);
            }
        }
        else
        {//没有种植东西
            uiManorComponent.PlayOneKeyGainEffect(false);
            if (!Root2dSceneManager._instance.isFriendManor)
            {
                Root2dSceneManager._instance.GetManorBubbleComponent().RemoveTipsPertinenceAll(20001);
            }
        }
    }
    
    //获取可以收获的所有图标
    List<UIItemGainComponent> GetListItemGainComponent()
    {
        List<UIItemGainComponent> list = new List<UIItemGainComponent>();
        list.AddRange(transParentGoGain.GetComponentsInChildren<UIItemGainComponent>());
        return list;
    }
    //获取可以收获的所有图标
    List<UIGoStealComponent> GetListItemStealComponent()
    {
        List<UIGoStealComponent> list = new List<UIGoStealComponent>();
        list.AddRange(transParentGoGain.GetComponentsInChildren<UIGoStealComponent>());
        return list;
    }
    //有收获的 创建图标
    public void CreateGain(SeedGrowComponent seedGrowComponent)
    {
        var listGainComponent=GetListItemGainComponent();
        UIItemGainComponent uiItemGainComponent = null;
        if (listGainComponent != null && listGainComponent.Count > 0)
        {
            uiItemGainComponent = listGainComponent.Find(x => x.SoildId == seedGrowComponent.tileComponent.SoilId);
        }

        if (uiItemGainComponent != null)
        {
            uiItemGainComponent.gameObject.SetActive(true);
        }
        else
        {//创建
            var itemGain=GameObject.Instantiate(goGain);
            itemGain.transform.SetTransformDefalutWithParentNotStretch(transParentGoGain);
            itemGain.SetActive(false);
            //第一次设置好位置后打开
            SetWorldPos(seedGrowComponent.tileComponent, itemGain, TypePointUI.Gain);
            itemGain.SetActive(true);
            itemGain.GetComponent<UIItemGainComponent>().SetSeedGrowComponent(seedGrowComponent);
        }
    }
    //有收获的 创建图标
    public void CreateSteal(SeedGrowComponent seedGrowComponent)
    {
        var listStealComponent = GetListItemStealComponent();
        UIGoStealComponent uiItemStealComponent = null;
        if (listStealComponent != null && listStealComponent.Count > 0)
        {
            uiItemStealComponent = listStealComponent.Find(x => x.SoildId == seedGrowComponent.tileComponent.SoilId);
        }

        if (uiItemStealComponent != null)
        {
            uiItemStealComponent.gameObject.SetActive(true);
        }
        else
        {//创建
            var itemSteal = GameObject.Instantiate(goSteal);
            itemSteal.transform.SetTransformDefalutWithParentNotStretch(transParentGoSteal);
            itemSteal.SetActive(false);
            //第一次设置好位置后打开
            SetWorldPos(seedGrowComponent.tileComponent, itemSteal, TypePointUI.Steal);
            itemSteal.SetActive(true);
            itemSteal.GetComponent<UIGoStealComponent>().SetSeedGrowComponent(seedGrowComponent);
        }
    }
    //清空收获图标
    public void ClearAllGainGo()
    {
        transParentGoGain.DestroyAllChild();
    }
    //清空偷取图标
    public void ClearAllStealGo()
    {
        transParentGoSteal.DestroyAllChild();
    }

    public void SetWorldPos(TileComponent tileComponent, GameObject goUI, TypePointUI typePointUI)
    {
        Transform trans = tileComponent.transform;
        switch (typePointUI)
        {
            case TypePointUI.Steal:
                trans = tileComponent.TransPointGainHandle;
                break;
            case TypePointUI.ReBack:
                if (tileComponent.TransPointPointDecorate != null)
                {
                    trans = tileComponent.TransPointPointDecorate;
                }
                break;
            case TypePointUI.Plant:
                break;
            case TypePointUI.FirstFertilizer:
                trans = tileComponent.TransPointFirstStageFertilizer;
                break;
            case TypePointUI.GrowUp:
                trans = tileComponent.TransPointPointGrowUp;
                break;
            case TypePointUI.Gain:
                trans = tileComponent.TransPointGainHandle;
                break;
            default:
                break;
        }
        goUI.GetComponent<RectTransform>().anchoredPosition = StaticData.ManorWorldPointToUICameraAnchorPos(trans.position);
    }



    // Update is called once per frame
    void Update()
    {
        if (Root2dSceneManager._instance == null)
        {
            return;
        }
        if (!Root2dSceneManager._instance.isFriendManor)
        {//自己的庄园
         //检查是否有成熟的
            CheckGain();
        }
        if (this.seedGrowComponent == null)
        {
            return;
        }
        if (isShowGrowUpUI)
        {
            goGrowUpFriendManor.SetActive(false);
            var totalSecondsCurrPeriod = StaticData.GetSeedGrowComponentTotalSecond(seedGrowComponent, seedGrowComponent.currCropPeriod);

            if (seedGrowComponent.currCropPeriod == SeedGrowComponent.PeriodGrow.Ripe)
            {
                SelfGrowUpComponent.gameObject.SetActive(false);
            }
            else
            {
                SelfGrowUpComponent.gameObject.SetActive(true);
                scrollCameraWhenBoundsOutScreenGrowUp.PlayCameraAnimWhenOutScreen(SelfGrowUpComponent.gameObject, seedGrowComponent.tileComponent,TypePointUI.GrowUp);
                isShowGrowUpUI = false;
            }
        }
        if (isShowGrowUpUIFriendManor)
        {
            SelfGrowUpComponent.gameObject.SetActive(false);
            if (seedGrowComponent.remainTime <= 0f)
            {
                seedGrowComponent.remainTime = 0f;
            }
            textSeedNameFriendManor.text = $"{seedName}";
            int minute = (int)seedGrowComponent.remainTime / 60;
            int remainSeconds = (int)(seedGrowComponent.remainTime % 60f);

            textRemainTimeFriendManor.text = $"{seedGrowComponent.currCropPeriod.ToString()} {String.Format("{0:00}:{1:00}", minute, remainSeconds)}";
            var totalSecondsCurrPeriod = StaticData.GetSeedGrowComponentTotalSecond(seedGrowComponent, seedGrowComponent.currCropPeriod);
            imageSliderFriendManor.fillAmount = 1 - seedGrowComponent.remainTime / totalSecondsCurrPeriod;

            if (seedGrowComponent.currCropPeriod == SeedGrowComponent.PeriodGrow.Ripe)
            {
                goGrowUpFriendManor.SetActive(false);
            }
            else
            {
                goGrowUpFriendManor.SetActive(true);
                scrollCameraWhenBoundsOutScreenGrowUpFriendManor.PlayCameraAnimWhenOutScreen(goGrowUpFriendManor, seedGrowComponent.tileComponent,TypePointUI.GrowUp);
            }
        }

    }
    public int GetFertilizerByPeriod(SeedGrowComponent.PeriodGrow period)
    {
        List<int> Fertilizers = StaticData.configExcel.GetVertical().StageRelation;
        int propId = Fertilizers[0];
        switch (period)
        {
            case SeedGrowComponent.PeriodGrow.Seed:
                propId = Fertilizers[0];
                break;
            case SeedGrowComponent.PeriodGrow.Germinate:
                propId = Fertilizers[1];
                break;
            case SeedGrowComponent.PeriodGrow.GrowUp:
                propId = Fertilizers[2];
                break;
        }

        return propId;
    }

    //设置第一阶段化肥
    public void SetFirstStageFertilizer(SeedGrowComponent seedGrowComponent)
    {
        ItemFirstStageFertilizerComponent willSetItemFirstStageFertilizerComponent = null;
        ItemFirstStageFertilizerComponent[] itemFirstStageFertilizers = TransGoFirstStageRoot.GetComponentsInChildren<ItemFirstStageFertilizerComponent>();
        bool isFind = false;
        for (int i = 0; i < itemFirstStageFertilizers.Length; i++)
        {
            if (itemFirstStageFertilizers[i].seedGrowComponent == null)
            {
                continue;
            }
            if (itemFirstStageFertilizers[i].seedGrowComponent.tileComponent.SoilId == seedGrowComponent.tileComponent.SoilId)
            {
                isFind = true;
                willSetItemFirstStageFertilizerComponent = itemFirstStageFertilizers[i];
                break;
            }
        }
        if (isFind)
        {
            SetWorldPos(willSetItemFirstStageFertilizerComponent.seedGrowComponent.tileComponent, willSetItemFirstStageFertilizerComponent.gameObject, TypePointUI.FirstFertilizer);
        }
        else
        {
            //实例化
            GameObject goFirstStage = GameObject.Instantiate<GameObject>(GoFirstStageFertilizer);
            goFirstStage.transform.SetTransformDefalutWithParent(TransGoFirstStageRoot);
            willSetItemFirstStageFertilizerComponent = goFirstStage.GetComponent<ItemFirstStageFertilizerComponent>();
            willSetItemFirstStageFertilizerComponent.seedGrowComponent = seedGrowComponent;
            //设置屏幕位置
            SetWorldPos(willSetItemFirstStageFertilizerComponent.seedGrowComponent.tileComponent, goFirstStage, TypePointUI.FirstFertilizer);
        }

    }

    //点击地块直接施肥
    public void ClickTileToFirstFertilizer(TileComponent tileComponent)
    {
        ItemFirstStageFertilizerComponent willSetItemFirstStageFertilizerComponent = null;
        ItemFirstStageFertilizerComponent[] itemFirstStageFertilizers = TransGoFirstStageRoot.GetComponentsInChildren<ItemFirstStageFertilizerComponent>();
        bool isFind = false;
        for (int i = 0; i < itemFirstStageFertilizers.Length; i++)
        {
            if (itemFirstStageFertilizers[i].seedGrowComponent.tileComponent.SoilId == tileComponent.SoilId)
            {
                isFind = true;
                willSetItemFirstStageFertilizerComponent = itemFirstStageFertilizers[i];
                break;
            }
        }
        if (isFind)
        {
            willSetItemFirstStageFertilizerComponent.OnButtonFertilizerClick();
        }
    }

    //删除第一次施肥
    public void DestroyFirstStageFertilizer(TileComponent tileComponent)
    {
        ItemFirstStageFertilizerComponent willGetItemFirstStageFertilizerComponent = null;
        ItemFirstStageFertilizerComponent[] itemFirstStageFertilizers = TransGoFirstStageRoot.GetComponentsInChildren<ItemFirstStageFertilizerComponent>();
        bool isFind = false;
        for (int i = 0; i < itemFirstStageFertilizers.Length; i++)
        {
            if (itemFirstStageFertilizers[i].seedGrowComponent == null)
            {
                continue;
            }
            if (itemFirstStageFertilizers[i].seedGrowComponent.tileComponent.SoilId == tileComponent.SoilId)
            {
                isFind = true;
                willGetItemFirstStageFertilizerComponent = itemFirstStageFertilizers[i];
                break;
            }
        }
        if (isFind)
        {
            Destroy(willGetItemFirstStageFertilizerComponent.gameObject);
        }
    }


}
