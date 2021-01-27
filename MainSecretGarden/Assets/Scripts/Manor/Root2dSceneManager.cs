using Company.Cfg;
using Cysharp.Threading.Tasks;
using Game.Protocal;
using Google.Protobuf.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using static SeedGrowComponent;
using static UIWorldHandleManager;

public class Root2dSceneManager : MonoBehaviour
{
    public static Root2dSceneManager _instance;
    public List<CSWareHouseStruct> awardIdsCurrManorRegion;
    public const int maxPlantLayer = 9999;
    public const int layerTile = 11;
    //20开始
    public const int layerInitPlant = 20;
    //每层预留5个
    public const int layerEveryHave = 1;
    //区域碰撞器
    public ManorRegionComponent[] arrayManorRegionComponent;
    //层级排序
    List<TileComponent> listChildActive = new List<TileComponent>();

    #region 需要排序的的文件夹
    public Transform transAllManorPlant;
    public Transform transTile;
    #endregion
    public GameObject currHandleTile;
    public bool isTileDrag;//是否有地块被拖拽
    public bool isZoom;//是否正在缩放
    public float zoomRadio;//缩放系数
    public ObjectPool objPool;
    //种植公用数据
    public CSPlantData PlantData=new CSPlantData();
    #region World UI
    [Header("World UI")]
    public ManorCameraWorldComponent worldCameraComponent;
    #endregion
    //是否是好友庄园UI
    [HideInInspector]
    public bool isFriendManor;
    //自己庄园的解锁情况
    [HideInInspector]
    public RepeatedField<SCUnlockAreaStruct> UnlockAreaInfoSelf;
    //好友庄园的解锁情况
    [HideInInspector]
    public RepeatedField<SCUnlockAreaStruct> UnlockAreaInfoFriend;
    #region 女主
    public TileComponent femaleManorManagerTileComponent;
    #endregion
    #region 解锁
    //对话组件
    public ManorUnLockDialogComponent manorUnLockDialogComponent;
    //施工组件
    public ManorUnLockWorkComponent manorUnLockWorkComponent;
    //工棚组件
    public ManorUnLockWorkShedComponent manorUnLockWorkShedComponent;
    #endregion
    //地鼠组件
    public MouseManorManager mouseManorManager;
    //工棚
    public TileComponent tileComWorkShed;

    //购买地块需要的货币信息
    public DealClass dealClassCurrency;

    private void Awake()
    {
        _instance = this;
        StaticData.isShowSelfLog = true;
        objPool.Initialize();
        zoomRadio = 4f;
        //设置相机
        UICameraManager._instance.SetAddWithBaseType();
    }
    private void OnDestroy()
    {
        _instance = null;
    }
    /// <summary>
    /// 关闭所有地块的选中状态
    /// </summary>
    public void CloseAllTileObjSelect()
    {
        Transform parentTrans = objPool.transform;
        var tileComs=parentTrans.GetComponentsInChildren<TileComponent>();
        for (int i = 0; i < tileComs.Length; i++)
        {
            tileComs[i].goTileClickFrame.SetActive(false);
        }
    }
    //获取种子期的一个地块
    public SeedGrowComponent GetTileSeed()
    {
        SeedGrowComponent seed = null;
        if (ListSeedGrow != null && ListSeedGrow.Count > 0)
        {
            seed = ListSeedGrow.Find(x => x.currCropPeriod == SeedGrowComponent.PeriodGrow.Seed);
        }
        return seed;
    }
    //获取成熟期的一个地块
    public SeedGrowComponent GetTileRipe()
    {
        SeedGrowComponent ripe = null;
        if (ListSeedGrow != null && ListSeedGrow.Count > 0)
        {
            ripe = ListSeedGrow.Find(x => x.currCropPeriod == SeedGrowComponent.PeriodGrow.Ripe);
        }
        return ripe;
    }

    //用来存储庄稼组件的列表
    List<SeedGrowComponent> ListSeedGrow = new List<SeedGrowComponent>();
    public void AddListSeedGrow(SeedGrowComponent seedGrow)
    {
        if (!ListSeedGrow.Contains(seedGrow))
        {
            ListSeedGrow.Add(seedGrow);
        }
    }
    public void RemoveFromListSeedGrow(SeedGrowComponent seedGrow)
    {
        if (ListSeedGrow.Contains(seedGrow))
        {
            ListSeedGrow.Remove(seedGrow);
        }
    }
    public List<SeedGrowComponent> GetListSeedGrow()
    {
        return ListSeedGrow;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="friendId">-1 表示自己</param>
    public async UniTask EnterManor(long friendId)
    {
        if (friendId == -1)
        {//自己
            StaticData.DataDot(Company.Cfg.DotEventId.OpenManor);
            StaticData.idPreEnterManor = -1;
            CSEmptyManorInfo csEmptyManorInfo = new CSEmptyManorInfo();
            ManorProtocalHelper.GetSelfManorInfo(csEmptyManorInfo, async (scManorData) =>
            {
                Root2dSceneManager._instance.isFriendManor = false;
                //设置头像信息
                StaticData.SetPlayerIconInfo();
                Root2dSceneManager._instance.UnlockAreaInfoSelf = scManorData.UnlockAreaInfo;
                // add_by_wsf
                mouseManorManager.playerUid = StaticData.Uid;
                // add_by_wsf end
                await GenerateExistPlants(scManorData.ManorInfo, false);
                SetRegionsUnLockInfoState();
                //根据本地数据恢复相机参数
                worldCameraComponent.SetWorldCameraParam();
                //判定是不是有公棚
                SetCameraToWorkShedRegion();
                //是否触发Avg动画
                if (LocalInfoData.localManorInfoData != null && LocalInfoData.localManorInfoData.isFirstAvgDialogFinish == false)
                {
                    //打开对话框
                    var UIDialogGo = await UIComponent.CreateUIAsync(UIType.UIManorUnLockRegionDialog);
                    UIDialogGo.GetComponent<UIManorUnLockRegionDialogComponent>().BeginFirstAVGDialog();
                }
            });
        }
        else
        {//好友
            StaticData.DataDot(Company.Cfg.DotEventId.EnterFriendManor);
            //新手引导
            if (friendId == 1 && StaticData.isOpenGuide && GuideCanvasComponent._instance != null&& GuideCanvasComponent._instance.CurrExecuteGuideLittleStepDefine.Id==10023)
            {
                CSEmptyManorGuidance csEmptyManorGuidance = new CSEmptyManorGuidance()
                {
                };
                ManorProtocalHelper.LookFriendGuide(csEmptyManorGuidance, async (scManorData) =>
                {
                    Root2dSceneManager._instance.isFriendManor = true;
                    //设置头像信息
                    StaticData.SetPlayerIconInfo();
                    Root2dSceneManager._instance.UnlockAreaInfoFriend = scManorData.UnlockAreaInfo;
                    StaticData.GetUIWorldHandleComponent().IdFriend = friendId;
                    mouseManorManager.playerUid = friendId;
                    SetFriendGuideStealClass(scManorData.ManorInfo);
                    await GenerateExistPlants(scManorData.ManorInfo, true);
                    SetRegionsUnLockInfoState();
                });
                return;
            }
            CSQueryOther csQueryOther = new CSQueryOther()
            {
                OtherUid= friendId
            };
            ManorProtocalHelper.LookFriendManorInfo(csQueryOther, async (scManorData) =>
            {
                Root2dSceneManager._instance.isFriendManor = true;
                //设置头像信息
                StaticData.SetPlayerIconInfo();
                Root2dSceneManager._instance.UnlockAreaInfoFriend = scManorData.UnlockAreaInfo;
                StaticData.GetUIWorldHandleComponent().IdFriend = friendId;
                // add_by_wsf
                mouseManorManager.playerUid = friendId;
                // add_by_wsf end

                //设置是否能偷的状态
                SetFriendManorListStealClass(scManorData);
                await GenerateExistPlants(scManorData.OtherManorInfo, true);
                SetRegionsUnLockInfoState();
            });
        }
    }

    private void SetCameraToWorkShedRegion()
    {
        int regionId = -1;
        ManorRegionComponent[] regionComs = GetAllManorRegionComponent();
        for (int i = 0; i < regionComs.Length; i++)
        {
            if (regionComs[i].areaState == AreaState.WorkShed)//工棚阶段
            {
                regionId = regionComs[i].regionId;
                break;
            }
        }
        if (regionId != -1)
        {
            var willLocalPos=Root2dSceneManager._instance.manorUnLockDialogComponent.GetWillSetPos(regionId);
            Root2dSceneManager._instance.worldCameraComponent.SetCameraToUnLockWorkShedRegion(willLocalPos);
        }
    }

    public ManorBubbleComponent GetManorBubbleComponent()
    {
        return Root2dSceneManager._instance.femaleManorManagerTileComponent.GetComponent<ManorBubbleComponent>();
    }

    private void SetFriendManorListStealClass(SCManorFriendData scManorData)
    {
        List<StealClass> listStealClass = new List<StealClass>();
        for (int i = 0; i < scManorData.OtherManorInfo.Count; i++)
        {
            if (scManorData.OtherManorInfo[i].SoilType== ManorScene.Tile&& scManorData.OtherManorInfo[i].CropGoodId!=0)
            {
                listStealClass.Add(new StealClass()
                {
                    SolidId = scManorData.OtherManorInfo[i].SoilId,
                    isSteal = scManorData.OtherManorInfo[i].IsSteal
                });
            }
        }
        StaticData.GetUIWorldHandleComponent().SetFriendManorStealState(listStealClass);
    }
    private void SetFriendGuideStealClass(RepeatedField<SCManorStruct> OtherManorInfo)
    {
        List<StealClass> listStealClass = new List<StealClass>();
        for (int i = 0; i < OtherManorInfo.Count; i++)
        {
            if (OtherManorInfo[i].SoilType == ManorScene.Tile && OtherManorInfo[i].CropGoodId != 0)
            {
                listStealClass.Add(new StealClass()
                {
                    SolidId = OtherManorInfo[i].SoilId,
                    isSteal = OtherManorInfo[i].IsSteal
                });
            }
        }
        StaticData.GetUIWorldHandleComponent().SetFriendManorStealState(listStealClass);
    }

    private void OpenCurrUnlockRegionCollider(int unlockRegionId,bool isFriendManor)
    {
        var allTileCom = transAllManorPlant.GetComponentsInChildren<TileComponent>();
        for (int i = 0; i < allTileCom.Length; i++)
        {
            if (isFriendManor)
            {//好友庄园，都不能拖动，包括地块，装饰物
                allTileCom[i].HandleColliders(false);
            }
            else
            {//自己的庄园，刚解锁的区域可以拖动
                if (allTileCom[i].regionId == unlockRegionId)
                {
                    allTileCom[i].HandleColliders(true);
                }
            }
        }
    }
    public ManorRegionComponent[] GetAllManorRegionComponent()
    {
        return arrayManorRegionComponent;
    }
    

    private void SetRegionsUnLockInfoState()
    {
        ManorRegionComponent[] regionComs = GetAllManorRegionComponent();
        var allTileCom = transAllManorPlant.GetComponentsInChildren<TileComponent>();
        //设置all（0地块）的状态
        for (int i = 0; i < regionComs.Length; i++)
        {
            if (regionComs[i].regionId==0)
            {
                regionComs[i].OnlySetAreaState(AreaState.RemoveWorkShed);
                break;
            }
        }
        //设置其它地块状态
        RepeatedField<SCUnlockAreaStruct> UnlockAreaInfo = null;
        if (Root2dSceneManager._instance.isFriendManor)
        {
            UnlockAreaInfo = Root2dSceneManager._instance.UnlockAreaInfoFriend;
        } else
        {
            UnlockAreaInfo = Root2dSceneManager._instance.UnlockAreaInfoSelf;
        }
        for (int i = 0; i < UnlockAreaInfo.Count; i++)
        {
            ManorRegionComponent currRegionCom = null;
            for (int j = 0; j < regionComs.Length; j++)
            {
                if (regionComs[j].regionId== UnlockAreaInfo[i].AreaId)
                {
                    currRegionCom = regionComs[j];
                    break;
                }
            }
            if (currRegionCom!=null)
            {
                currRegionCom.SetManorRegionAreaState(UnlockAreaInfo[i].State, UnlockAreaInfo[i].AreaUnlockTime);
            }
        }
    }
    public bool GetRegionIsFinishUnLock(int regionId)
    {
        bool isFinish = false;
        var manorsCom=GetAllManorRegionComponent();
        for (int i = 0; i < manorsCom.Length; i++)
        {
            if (manorsCom[i].regionId == regionId)
            {
                isFinish= manorsCom[i].GetRegionIsFinishUnLock();
            }
        }
        return isFinish;
    }
    private async UniTask GenerateExistPlants(RepeatedField<SCManorStruct> manorInfo,bool isFriendManor)
    {
        //进入庄园的时候 清空收获.偷取图标
        StaticData.GetUIWorldHandleComponent().ClearAllGainGo();
        StaticData.GetUIWorldHandleComponent().ClearAllStealGo();
        for (int i = 0; i < manorInfo.Count; i++)
        {
            SCManorStruct plant = manorInfo[i];
            GameObject go;
            TileComponent tileComponent = null;
            if (plant.SoilType == ManorScene.None)
            {
                Debug.LogError($"服务器初始化庄园Plant.id:{plant.SoilId}");
            }
            if (plant.SoilType != ManorScene.None && plant.SoilType != ManorScene.Tile && plant.SoilType != ManorScene.Npc
                && plant.CropGoodId!=0)
            {
                //装饰物(装饰物，礼盒，狗窝)
                //装饰物旋转索引
                int rotateIdx = plant.DecorateRotateIndex;
                var DecorateDefine = StaticData.configExcel.GetDecorateByDecorateId((int)plant.CropGoodId);
                string modelName = DecorateDefine.Model[rotateIdx];
                go = await GenerateDecorate(modelName, DecorateDefine.DecorateId);
                tileComponent = go.GetComponent<TileComponent>();
                go.transform.localPosition = new Vector2(plant.Xaxle, plant.Yaxle);
                tileComponent.regionId = plant.ParcelDivision;
                tileComponent.idxDecorateRotate = rotateIdx;
            }
            else if (plant.SoilType == ManorScene.Tile)
            {
                ////test
                //plant.CropGoodId = 400001;
                //plant.NextTime = 360000;
                ////test end
                go = GenerateTile();
                tileComponent = go.GetComponent<TileComponent>();
                go.transform.localPosition = new Vector2(plant.Xaxle, plant.Yaxle);
                go.GetComponent<TileComponent>().regionId = plant.ParcelDivision;
                //创建地块上庄稼
                tileComponent.CropGoodId = (int)plant.CropGoodId;
                tileComponent.SoilId = plant.SoilId;
                if (tileComponent.CropGoodId == 0)
                {
                    continue;
                }
                var prefabName = StaticData.configExcel.GetManorCropByCropId(tileComponent.CropGoodId).Model;
                var plantPrefab = await ABManager.GetAssetAsync<GameObject>(prefabName);
                var goPlant = GameObject.Instantiate<GameObject>(plantPrefab);
                Transform plantTrans = goPlant.transform;
                plantTrans.parent = tileComponent.rootPlant.transform;
                plantTrans.localPosition = Vector3.zero;
                plantTrans.localScale = Vector3.one;
                tileComponent.SetCurrGoPlant(goPlant);
                var seedGrowComponent = plantTrans.GetComponent<SeedGrowComponent>();
                seedGrowComponent.SetCropId((int)plant.CropGoodId, tileComponent);
                seedGrowComponent.GenerateTimer();
                seedGrowComponent.SetPeriod((SeedGrowComponent.PeriodGrow)plant.SoilStatus, plant.NextTime);
                Root2dSceneManager._instance.AddListSeedGrow(seedGrowComponent);

            }
            else if (plant.SoilType == ManorScene.Npc)
            {
                //设置Npc位置
                Root2dSceneManager._instance.femaleManorManagerTileComponent.transform.localPosition = new Vector2(plant.Xaxle, plant.Yaxle);
                Root2dSceneManager._instance.femaleManorManagerTileComponent.SoilId = plant.SoilId;
            }
            if (tileComponent != null)
            {
                tileComponent.SoilId = plant.SoilId;
            }

        }
        
        //排序
        UpdateSortLayer(true);
        //add_by_wsf
        //检查地鼠的生成
        if (!isFriendManor)
        {
            mouseManorManager.CheckSelfMouseGenerate();
        }
        else 
        {
            mouseManorManager.CheckFriendMouseGenerate();
        }
        //add_by_wsf end
    }

    public void UpdateSortLayer(bool isContainFemaleModle=false, bool isContainMouse = false,bool isContainWorkShed=false)
    {
        listChildActive.Clear();
        listChildActive.AddRange(transAllManorPlant.GetComponentsInChildren<TileComponent>());

        int countPlant = transTile.childCount;
        for (int i = 0; i < countPlant; i++)
        {
            if (transTile.GetChild(i).gameObject.activeInHierarchy)
            {
                listChildActive.Add(transTile.GetChild(i).gameObject.GetComponent<TileComponent>());
            }
        }
        if (isContainFemaleModle)
        {
            listChildActive.Add(Root2dSceneManager._instance.femaleManorManagerTileComponent);
        }
        if (isContainMouse)
        {
            listChildActive.Add(Root2dSceneManager._instance.mouseManorManager.GetComponent<TileComponent>());
        }
        if (isContainWorkShed)
        {
            listChildActive.Add(tileComWorkShed);
        }
        //给显示的排序
        listChildActive.Sort((a, b) => {
            if (a==null||b==null)
            {
                return 0;
            }
            //用Y坐标来排序
            float yA = a.transform.position.y;
            float yB = b.transform.position.y;
            float xA = a.transform.position.x;
            float xB = b.transform.position.x;
            float delta = 0.003f;
            if (yA > (yB + delta))
            {
                return -1;
            }
            else if (yA < (yB + delta))
            {
                return 1;//y降序
            }
            else
            {
                if (xA > xB)
                {
                    return 1;//x 升序
                }
                else if (xA < xB)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        });
        //设置层级
        for (int i = 0; i < listChildActive.Count; i++)
        {
            var tileCom = listChildActive[i];
            if (tileCom == null)
            {
                continue;
            }
            if (tileCom.RenderPlantList.Count>0)
            {//地块上植物
                for (int len = 0; len < tileCom.RenderPlantList.Count; len++)
                {
                    tileCom.RenderPlantList[len].sortingOrder = layerInitPlant + layerEveryHave * i + 1;
                    //地砖统一19
                    if (tileCom.name.Contains("Decorate_dizhuan"))
                    {
                        tileCom.RenderPlantList[len].sortingOrder = 19;
                    }
                }
            }
            if (tileCom.RenderTile != null)
            {//地块上植物
                tileCom.RenderTile.sortingOrder = layerTile;
            }
            if (tileCom.GetComponentInChildren<Live2D.Cubism.Rendering.CubismRenderController>()!=null && isContainFemaleModle)
            {
                tileCom.GetComponentInChildren<Live2D.Cubism.Rendering.CubismRenderController>().SortingOrder = layerInitPlant + layerEveryHave * i + 1;
            }
        }
    }

    public void RemoveObject(GameObject go)
    {
        objPool.ReturnObject(go);
    }
    public GameObject GenerateTile()
    {
        var go = objPool.GetObject();
        var sprites = go.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].sortingOrder = Root2dSceneManager.maxPlantLayer + i;
        }
        go.transform.localScale = Vector3.one;
        return go;
    }

    public async Task<GameObject> GenerateDecorate(string NameModel, int cropGoodId)
    {
        var prefab = await ABManager.GetAssetAsync<GameObject>($"{NameModel}");
        var go = GameObject.Instantiate<GameObject>(prefab);
        go.transform.parent = Root2dSceneManager._instance.transAllManorPlant;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.localScale = Vector3.one;
        var sprites = go.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].sortingOrder = Root2dSceneManager.maxPlantLayer + i;
        }
        go.GetComponent<TileComponent>().CropGoodId = cropGoodId;
        return go;
    }


    List<Collider2D> listOverlapPointCollider = new List<Collider2D>();
    // Update is called once per frame
    void Update()
    {
        UIManorComponent uiManorComponent=UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
        UIWorldHandleManager uiUIWorldHandleComponent = StaticData.GetUIWorldHandleComponent();
        if (uiManorComponent == null)
        {
            return;//场景加载过程中
        }
        if (Input.GetMouseButtonUp(0))//如果左键抬起重置
        {
            isTileDrag = false;
        }
        if (Input.GetMouseButtonDown(0))//左键按下
        {
            currHandleTile = null;
            bool isUIScrollMask = false;
            bool isUIDecorateMask = false;
            //EventSystem.current.IsPointerOverGameObject()在android上unity2019不起作用
            var clickObj = StaticData.UI_GetCurrentSelect();
            if (clickObj!=null)//表明是UI
            {
                isUIScrollMask = clickObj.CompareTag(TagHelper.UIScrollMask);
                isUIDecorateMask = clickObj.CompareTag(TagHelper.DecorateUIMask);
            }
            if (Root2dSceneManager._instance.isTileDrag)
            {
                //地块正在拖拽的时候，相机不允许平移
                return;
            }
            //覆盖的不是UI
            if (clickObj == null || isUIScrollMask)
            {
                CloseAllTileObjSelect();
                if (uiUIWorldHandleComponent!=null)
                {
                    uiUIWorldHandleComponent.SetHandleTileUIClose();
                }
                StaticData.DebugGreen("Not UI");
                Camera cameraWorld = Root2dSceneManager._instance.worldCameraComponent.cameraWorld;
                var worldPoint = cameraWorld.ScreenToWorldPoint(Input.mousePosition);
                Collider2D[] colliders = Physics2D.OverlapPointAll(worldPoint);
                //层级排序上边到下边
                /*
                 * TagHelper.CollierCanOver
                 * TagHelper.PlantRenderer//植物Render层，不能拖走的
                 * TagHelper.Tile
                 * TagHelper.Ground
                 */
                listOverlapPointCollider.Clear();
                //colliders 转list
                for (int i = 0; i < colliders.Length; i++)
                {
                    listOverlapPointCollider.Add(colliders[i]);
                }

                //add_by_wsf
                Collider2D colliderMouse = listOverlapPointCollider.Find(x => x.gameObject.CompareTag(TagHelper.MouseManor));
                if (colliderMouse != null) 
                {
                    //点击地鼠
                    Debug.Log("敲打地鼠~~~");
                    mouseManorManager.CatchMouse();
                    return;
                }
                //add_by_wsf end

                Collider2D colliderTileCom = null;
                colliderTileCom = listOverlapPointCollider.Find(x => x.gameObject.CompareTag(TagHelper.CollierCanOver));
                if (colliderTileCom==null)
                {
                    colliderTileCom = listOverlapPointCollider.Find(x => x.gameObject.CompareTag(TagHelper.Tile));
                }

                var colliderGrounds = listOverlapPointCollider.FindAll(x => x.gameObject.CompareTag(TagHelper.Ground));
                if (colliderTileCom != null)
                {//点击树或者小地块
                    StaticData.DebugGreen($"{colliderTileCom.name}");

                    TileComponent tileCom = colliderTileCom.gameObject.GetComponent<TileComponent>();
                    //树需要到上一层才有组件,种植的作物要上三层
                    if (tileCom == null)
                    {
                        tileCom = colliderTileCom.transform.GetComponentInParent<TileComponent>();
                    }

                    if (tileCom != null)//别墅，仓库没有TileComponent
                    {
                        currHandleTile = tileCom.gameObject;
                        tileCom.PressInScene();
                    }
                    
                    
                }
                else if(colliderGrounds != null&& colliderGrounds.Count>0)
                {
                    //点击到大地块
                    //大地块点击,没解锁的地方的除了Ground其它地方碰撞器都是禁用的
                    var colliderGround=colliderGrounds.Find(x => x.gameObject.GetComponent<ManorRegionComponent>().regionId != 0);
                    if (colliderGround != null)//为null表示只有0的碰撞器
                    {//有限处理没解锁的
                        ManorRegionComponent manorRegion = colliderGround.gameObject.GetComponent<ManorRegionComponent>();
                        StaticData.DebugGreen($"大地块按下");
                        manorRegion.PressDownRegion();
                    }
                    else
                    {//用来处理人物点击空白的行走
                        StaticData.DebugGreen($"大地块0按下");
                        ManorRegionComponent manorRegion = colliderGrounds[0].gameObject.GetComponent<ManorRegionComponent>();
                        manorRegion.PressDownRegion();
                    }
                }
                

            }
            //如果点击到了装饰物的UI面板
            if (isUIDecorateMask)
            {
                ContineClickDecorate();
            }

        }
    }

    private void ContineClickDecorate()
    {
        StaticData.DebugGreen("继续点击装饰物");
        Camera cameraWorld = Root2dSceneManager._instance.worldCameraComponent.cameraWorld;
        var worldPoint = cameraWorld.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] colliders = Physics2D.OverlapPointAll(worldPoint);
        listOverlapPointCollider.Clear();
        //colliders 转list
        for (int i = 0; i < colliders.Length; i++)
        {
            listOverlapPointCollider.Add(colliders[i]);
        }
        Collider2D colliderTileCom = null;
        colliderTileCom = listOverlapPointCollider.Find(x => x.gameObject.CompareTag(TagHelper.CollierCanOver));
        if (colliderTileCom == null)
        {
            colliderTileCom = listOverlapPointCollider.Find(x => x.gameObject.CompareTag(TagHelper.Tile));
        }
        if (colliderTileCom != null)
        {//点击树或者小地块
            StaticData.DebugGreen($"{colliderTileCom.name}");

            TileComponent tileCom = colliderTileCom.gameObject.GetComponent<TileComponent>();
            //树需要到上一层才有组件,种植的作物要上三层
            if (tileCom == null)
            {
                tileCom = colliderTileCom.transform.GetComponentInParent<TileComponent>();
            }
            //是装饰物
            if (tileCom != null && tileCom.typeTile == TypeManorDecorate.Decorate)
            {
                currHandleTile = tileCom.gameObject;
                tileCom.PressInScene();
            }
        }
    }

    private TimeCountDownComponent DealRedDotTimer;

    /// <summary>
    /// 通知订单红点 更新计时器
    /// </summary>
    public void NotifyUpdateDealRedDot() 
    {
        InitUpdateDealRedDotTimer();
        UpdateDealRedDot();
    }

    /// <summary>
    /// 更新订单红点
    /// </summary>
    private void UpdateDealRedDot() 
    {
        DealRedDotTimer.Init(5f, false, () => {

            RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Order);
            if (!StaticData.IsSubmintDeal())
            {
                UpdateDealRedDot();
            }
            else 
            {
                DealRedDotTimer.Dispose();
                DealRedDotTimer = null;
            }

        }, null);
    }


    /// <summary>
    /// 初始化订单红点计时器
    /// </summary>
    private void InitUpdateDealRedDotTimer() 
    {
        DealRedDotTimer = StaticData.CreateTimer(36000f, false, (go) =>
        {

        },
        (remainTime) =>
        {

        }, "UpdateDealRedDot");
    }
}
