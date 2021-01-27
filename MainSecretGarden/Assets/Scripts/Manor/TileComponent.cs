using Company.Cfg;
using Cysharp.Threading.Tasks;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using static ManorCameraWorldComponent;

public enum TypeTile
{
    Tile,//地块
    Tree
}

public class TileComponent : MonoBehaviour
{
    #region 地块信息
    public TypeManorDecorate typeTile;
    //是否是庄园中的人物
    public bool isNPC;
    public long SoilId;//地块id
    public int CropGoodId;//地块上植物id,包括庄稼，树，喷泉，栅栏等
    public int regionId;//大地块id
    //当前装饰物的旋转下标
    public int idxDecorateRotate;
    int currCollisonGroundRegionId = -1;
    
    #endregion
    bool isBeginPlace;
    //拖出的UI面板
    GameObject GoUIRootScroll;
    public Transform PointsParent;
    public GameObject goTileClickFrame;
    List<Vector2> pointsTile = new List<Vector2>();
    Collider2D selfCollider2D;
    bool isDragFromScene;
    Vector2 originPos;
    Vector2 offsetPos;
    /// <summary>
    /// 输入处理相关逻辑变量
    /// </summary>
    bool isPress;
    bool isLongPress;
    bool isDrag;
    const float timeOnClickMinThreshold = 0.05f;
    const float timeLongPressThreshold = 0.2f;
    float timeCurrPress = 0f;
    //记录按下时候地块位置
    Vector2 tileMousePos;
    //种植
    [Header("作物的跟目录，TypeTile是Tree时不用给值")]
    public GameObject rootPlant;
    GameObject currGoPlant;
    //用来获取render
    SpriteRenderer renderTile;
    SpriteRenderer renderPlant;

    //装饰物类型
    TypeManorDecorate DecorateType;
    bool isUpAnimPlaying;

    #region World UI 的一些点
    public Transform TransPointGainHandle;
    public Transform TransPointFirstStageFertilizer;
    public Transform TransPointPointGrowUp;
    public Transform TransPointPointDecorate;
    #endregion


    public SpriteRenderer RenderTile
    {
        get
        {
            var renders = transform.GetComponentsInChildren<SpriteRenderer>();
            renderTile = renders.ToList().Find(x => x.transform.parent.gameObject.CompareTag(TagHelper.Tile));
            return renderTile;
        }
    }
    List<SpriteRenderer> listPlant = new List<SpriteRenderer>();
    public List<SpriteRenderer> RenderPlantList
    {
        get
        {
            var renders = transform.GetComponentsInChildren<SpriteRenderer>(true);
            listPlant = renders.ToList().FindAll(x => x.gameObject.CompareTag(TagHelper.PlantRenderer));
            return listPlant;
        }
    }
    public void SetPlantSortOrder(int sortOrder)
    {
        for (int i = 0; i < RenderPlantList.Count; i++)
        {
            RenderPlantList[i].sortingOrder = sortOrder + i;
        }
    }

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }
    //UI拖入场景的时候调用
    public void Init(GameObject goRootTab, TypeManorDecorate DecorateType)
    {
        this.DecorateType = DecorateType;
        //初始拖拽 旋转下标默认为0
        idxDecorateRotate = 0;
        isBeginPlace = false;
        ResetHandleVar();
        isPress = true;
        GoUIRootScroll = goRootTab;
        BeginDrag(false);
        Root2dSceneManager._instance.currHandleTile = gameObject;
    }
    //旋转装饰物替换装饰物
    public void SetDecorateInfo(int idxDecorateRotate, long SoilId, int CropGoodId,int regionId)
    {
        this.DecorateType = TypeManorDecorate.Decorate;
        this.idxDecorateRotate = idxDecorateRotate;
        this.SoilId = SoilId;
        this.CropGoodId = CropGoodId;
        this.regionId = regionId;
    }

    //场景中物体被点击的时候调用
    public void PressInScene()
    {
        ResetHandleVar();
        isPress = true;
        tileMousePos = Input.mousePosition;
        //庄园角色移动，打断气泡播放
        if (isNPC)
        {
            Root2dSceneManager._instance.GetManorBubbleComponent().BreakBeginAnimByTime();
        }
    }

    private void ResetHandleVar()
    {
        timeCurrPress = 0f;
        isPress = false;
        isLongPress = false;
        isDrag = false;
    }

    CancellationTokenSource ctsWaitPlayUpAnim;
    async UniTask BeginDrag(bool isDragFromScene)//isDragFromScene是否从放好了的在拖动
    {
        this.isDragFromScene = isDragFromScene;
        //设置初始位置
        SetTilePos(true);
        Root2dSceneManager._instance.isTileDrag = true;
        TipsTileComponent._instance.SetUpAnimPos(transform);
        //显示提示
        if (isDragFromScene)
        {
            //播放抬起动画
            ctsWaitPlayUpAnim = new CancellationTokenSource();
            isUpAnimPlaying = true;
            await TipsTileComponent._instance.PlayUpAnim(ctsWaitPlayUpAnim.Token);
            if (ctsWaitPlayUpAnim.Token.IsCancellationRequested)
            {
                return;
            }
            TipsTileComponent._instance.SetCanPlace(transform, true, this);
            isUpAnimPlaying = false;
            //从场景中拖起，设置最高层级
            SetPlantLayerMax();
        }
        else
        {
            TipsTileComponent._instance.SetCanPlace(transform, true, this);
        }

        //等动画播放再拖拽
        isDrag = true;
        isBeginPlace = true;
        //开始拖拽的时候记录World摄像机位置
        var worldComponent = Root2dSceneManager._instance.worldCameraComponent;
        worldComponent.SetPreCameraPos();
        //拖动地块的时候关闭地块UI
        UIWorldHandleManager uiUIWorldHandleComponent = StaticData.GetUIWorldHandleComponent();
        uiUIWorldHandleComponent.SetHandleTileUIClose();
        //关闭Manor UI
        UIManorComponent uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
        uiManorComponent.DragTileOrDecorateCloseSelfManor(true);
    }

    public void HandleColliders(bool isOpen)
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.CompareTag(TagHelper.CollierCanOver) || colliders[i].gameObject.CompareTag(TagHelper.Tile))
            {
                colliders[i].enabled = isOpen;
            }
        }
    }

    private void SetPlantLayerMax()
    {
        var sprites = gameObject.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].sortingOrder = Root2dSceneManager.maxPlantLayer + i;
            ////保证Tip后边被植物挡住
            //if (sprites[i].gameObject.CompareTag(TagHelper.TipsTile))
            //{
            //    sprites[i].sortingOrder = Root2dSceneManager.maxPlantLayer - 1;
            //}
        }
        if (isNPC)
        {
            if (GetComponentInChildren<Live2D.Cubism.Rendering.CubismRenderController>() != null)
            {
                GetComponentInChildren<Live2D.Cubism.Rendering.CubismRenderController>().SortingOrder = Root2dSceneManager.maxPlantLayer;
            }
        }
    }

    private void SetTilePos(bool isSetOriginPos)
    {
        //获取鼠标位置
        Vector2 mousePos = Input.mousePosition;
        //把鼠标的屏幕坐标转换成世界坐标
        Vector2 worldPos = Root2dSceneManager._instance.worldCameraComponent.cameraWorld.ScreenToWorldPoint(mousePos);

        if (isSetOriginPos)
        {
            //鼠标点击何轴心点之间的偏移，不处理点一下就会动一下，后边滑倒重叠
            offsetPos = new Vector2(transform.position.x, transform.position.y) - worldPos;
            //StaticData.DebugGreen($"offsetPos:{offsetPos}");
        }

        if (isSetOriginPos && isDragFromScene)
        {//记录原始位置
            originPos = worldPos + offsetPos;
        }
        //控制物体移动
        if (isDragFromScene)
        {
            transform.position = worldPos + offsetPos;
        }
        else
        {
            transform.position = worldPos;//UI拖到场景
        }
    }
    public async UniTask OnTileClick()
    {
        UIManorComponent uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
        switch (typeTile)
        {
            case TypeManorDecorate.None:
                if (isNPC)
                {
                    //todo 说话
                }
                break;
            case TypeManorDecorate.GiftBox:
                break;
            case TypeManorDecorate.DogHouse:
                break;
            case TypeManorDecorate.Tile:
                if (goTileClickFrame != null)
                {
                    goTileClickFrame.SetActive(true);
                }
                //if (!Root2dSceneManager._instance.isFriendManor)
                //{//不是好友，等女主移动到位置，好友庄园不需要移动
                //    var tileCom = await Root2dSceneManager._instance.femaleManorManager.PlayAnim(this);
                //    if (tileCom != this)
                //    {
                //        return;
                //    }
                //}
                if (currGoPlant == null)
                {//没有种植东西
                 //播放音效点击
                    GameSoundPlayer.Instance.PlaySoundEffect(MusicHelper.SoundEffectClickPlot);
                    if (!Root2dSceneManager._instance.isFriendManor)
                    {//不是好友庄园
                        StaticData.GetUIWorldHandleComponent().OpenRootPlant(this);
                        //新手引导点击地块算完成
                        if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.isCurrStepGuiding)
                        {
                            //点击的时候只保存点击一块一步，不干扰后边两步
                            if (GuideCanvasComponent._instance.CurrExecuteGuideLittleStepDefine.Id==10002)
                            {
                                GuideCanvasComponent._instance.SetLittleStepFinish();
                            }
                        }
                    }
                }
                else
                {//种植了东西
                    //好友的东西在SetGrowUpInfo中判定
                    SeedGrowComponent seedGrowComponent = transform.GetComponentInChildren<SeedGrowComponent>();
                    if (seedGrowComponent.currCropPeriod == SeedGrowComponent.PeriodGrow.Ripe)
                    {
                        if (!Root2dSceneManager._instance.isFriendManor)
                        {
                            StaticData.GetUIWorldHandleComponent().GainOneTile(seedGrowComponent);
                        }
                        else//偷取
                        {
                            StaticData.GetUIWorldHandleComponent().OnButtonStealClick(seedGrowComponent);
                        }
                    }
                    else
                    {
                        
                        if (StaticData.GetUIWorldHandleComponent().isFertiliezeringAnimPlay)
                        {
                            return;
                        }
                        if (StaticData.GetUIWorldHandleComponent().isEradicating)
                        {
                            return;
                        }
                        //播放音效点击
                        GameSoundPlayer.Instance.PlaySoundEffect(MusicHelper.SoundEffectClickPlot);
                        //第二个时期
                        if (seedGrowComponent.currCropPeriod != SeedGrowComponent.PeriodGrow.Seed)
                        {
                            StaticData.GetUIWorldHandleComponent().SetGrowUpInfo(this, seedGrowComponent);
                        }
                        else
                        {
                            //直接算施肥
                            StaticData.GetUIWorldHandleComponent().ClickTileToFirstFertilizer(this);
                        }
                    }
                }
                break;
            case TypeManorDecorate.Decorate:
                //if (!Root2dSceneManager._instance.isFriendManor)
                //{//不是好友，等女主移动到位置，好友庄园不需要移动
                //    var tileCom = await Root2dSceneManager._instance.femaleManorManager.PlayAnim(this);
                //    if (tileCom != this)
                //    {
                //        return;
                //    }
                //}
                if (!Root2dSceneManager._instance.isFriendManor)
                {
                    StaticData.GetUIWorldHandleComponent().OpenRebackDecorate(this);
                }
                break;
        }
    }


    public void Plant(GameObject goPlant, CSPlantStruct csPlantStruct, Action ActionRealPlant)
    {
        if (currGoPlant != null)
        {//已经种植了东西，返回
            //实例化的删除掉
            Destroy(goPlant);
            return;
        }
        ActionRealPlant?.Invoke();
        Transform plantTrans = goPlant.transform;
        plantTrans.parent = rootPlant.transform;
        plantTrans.localPosition = Vector3.zero;
        plantTrans.localScale = Vector3.one;
        SetCurrGoPlant(goPlant);
        //播放音效点击
        GameSoundPlayer.Instance.PlaySoundEffect(MusicHelper.SoundEffectPlanting);
        //种植协议增加作物
        Root2dSceneManager._instance.PlantData.PlantInfo.Add(csPlantStruct);
        //更新作物层级
        Root2dSceneManager._instance.UpdateSortLayer(true);
    }

    public void SetCurrGoPlant(GameObject goPlant)
    {
        currGoPlant = goPlant;
    }

    // Update is called once per frame
    void Update()
    {
        if (Root2dSceneManager._instance.currHandleTile != this.gameObject)
        {
            return;
        }
        var worldComponent = Root2dSceneManager._instance.worldCameraComponent;
        var clickObj = StaticData.UI_GetCurrentSelect();
        bool isUIScrollMask = false;
        bool isUIDecorateMask = false;
        if (clickObj != null)//表明是UI
        {
            isUIScrollMask = clickObj.CompareTag(TagHelper.UIScrollMask);
            isUIDecorateMask = clickObj.CompareTag(TagHelper.DecorateUIMask);
        }
        if (!isDrag && Input.GetMouseButtonUp(0)&& (clickObj==null|| isUIScrollMask|| isUIDecorateMask))//鼠标抬起
        {
            //关闭掉正在处理的装饰物
            if (isUIDecorateMask&& StaticData.GetUIWorldHandleComponent().isRotatingDecorate)
            {
                StaticData.GetUIWorldHandleComponent().OnButtonRotateOKClick(false);
                StaticData.GetUIWorldHandleComponent().SetHandleTileUIClose();
            }
            isPress = false;
            isDrag = false;
            isLongPress = false;
            if (timeCurrPress > timeOnClickMinThreshold && timeCurrPress <= timeLongPressThreshold)
            {
                StaticData.DebugGreen("触发小地块点击");
                OnTileClick();
            }
            timeCurrPress = 0f;
        }
        else if (isDrag)
        {
            //正在拖拽，判定是否小于边缘

            EnumDirection direction = EnumDirection.None;
            for (int i = 0; i < worldComponent.listDirection.Count; i++)
            {
                float xDir = worldComponent.listDirection[i].x;
                float yDir = worldComponent.listDirection[i].y;
                var newPos = Input.mousePosition + new Vector3(xDir, yDir, 0) * ManorCameraWorldComponent.SizeScreenToSide;
                //四个方向判定
                if (newPos.x <= 0f)
                {
                    direction = EnumDirection.Left;
                    break;
                }
                else if (newPos.x >= Screen.width)
                {
                    direction = EnumDirection.Right;
                    break;
                }
                else if (newPos.y <= 0f)
                {
                    direction = EnumDirection.Bottom;
                    break;
                }
                else if (newPos.y >= Screen.height)
                {
                    direction = EnumDirection.Up;
                    break;
                }
            }
            if (direction != EnumDirection.None)//表示是自动滑动
            {
                worldComponent.TriggerAutoScroll(direction);
            }
            else
            {//改变了方向
                worldComponent.EndAutoScroll();
            }
        }
        //只要鼠标抬起，就停止自动滚动
        if (Input.GetMouseButtonUp(0))//滑动过程中鼠标抬起也终止自动滑动
        {
            worldComponent.EndAutoScroll();
        }

        if (isPress && !isDrag)
        {
            timeCurrPress += Time.deltaTime;
            //小地块滑动
            if (Vector2.Distance(tileMousePos, Input.mousePosition) <= 10f)//10f做为滑动的阈值
            {
                if (timeCurrPress >= timeLongPressThreshold)
                {
                    if (!Root2dSceneManager._instance.isFriendManor)//好友庄园不能长按拖动
                    {
                        //正在处理装饰物
                        if (StaticData.GetUIWorldHandleComponent().isRotatingDecorate && typeTile == TypeManorDecorate.Decorate)
                        {
                            isPress = false;
                            return;
                        }
                        isLongPress = true;
                        StaticData.DebugGreen("触发小地块拖动");
                        BeginDrag(true);
                    }
                }
            }
        }
        if (isBeginPlace && Input.GetMouseButton(0))//鼠标按住处理
        {
            SetTilePos(false);
            //播放完抬起动画再检查
            if (isUpAnimPlaying == false)
            {
                CheckIsCanPlace(() =>
                {
                    //显示红色箭头
                    TipsTileComponent._instance.SetCanPlace(transform, false, this);
                }, () =>
                {
                    //显示蓝色箭头
                    TipsTileComponent._instance.SetCanPlace(transform, true, this);
                });
            }
        }
        if (Input.GetMouseButtonUp(0))//鼠标抬起
        {
            ctsWaitPlayUpAnim?.Cancel();
            isUpAnimPlaying = false;
            //庄园角色移动，继续气泡播放
            if (isNPC)
            {
                Root2dSceneManager._instance.GetManorBubbleComponent().ResetBeginAnimByTime();
            }
            //关闭提示
            TipsTileComponent._instance.CloseAll();
            //打开Manor UI
            UIManorComponent uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
            uiManorComponent.DragTileOrDecorateCloseSelfManor(false);
            TipsTileComponent._instance.CloseUpAnim();
            ResetHandleVar();
            if (GoUIRootScroll != null)
            {
                GoUIRootScroll.SetActive(true);
            }
            CheckIsCanPlace(() =>
            {
                if (isBeginPlace == false)
                {
                    return;
                }
                CancelPlaceHandle();
                isBeginPlace = false;
                return;
            }, () =>
            {
                if (isBeginPlace == false)
                {
                    return;
                }
                isBeginPlace = false;
                //放置的时候排序
                Root2dSceneManager._instance.UpdateSortLayer(true);
                if (!this.isDragFromScene)//创建过程
                {
                    CSDrag csDrag = new CSDrag()
                    {
                        Type = (int)DecorateType,
                        Xaxle = transform.localPosition.x,
                        Yaxle = transform.localPosition.y,
                        GoodId = this.CropGoodId
                    };
                    ManorProtocalHelper.CreateManorGoByDrag(csDrag, (scDrag) =>
                    {
                        //设置id
                        this.SoilId = scDrag.SoilId;
                        //地块使用数+1
                        //可以放置,地块数量变化
                        if (DecorateType == TypeManorDecorate.Tile && this.CropGoodId == 0)
                        {
                            //扣除货币
                            StaticData.UpdateWareHouseItem(Root2dSceneManager._instance.dealClassCurrency.IdGameItem, -Root2dSceneManager._instance.dealClassCurrency.Price);
                        }
                        else if (DecorateType == TypeManorDecorate.Decorate&&this.CropGoodId != 0)
                        {
                            StaticData.DecorateMinusOne(this.CropGoodId);
                            uiManorComponent.RefreshDecorateList();
                            //拖拽生成装饰物
                            StaticData.GetUIWorldHandleComponent().OpenRebackDecorate(this);
                        }

                    });
                }
                else
                {
                    //移动位置
                    CSChangeLocation csChangeLocation = new CSChangeLocation()
                    {
                        SoilId = this.SoilId,
                        Xaxle = transform.localPosition.x,
                        Yaxle = transform.localPosition.y
                    };
                    ManorProtocalHelper.MoveManorGo(csChangeLocation, (succ) => { });
                }

            });

        }

    }

    public void CheckIsCanPlace(Action actionNotAllowPlace, Action actionCanPlace = null)
    {
        //放下时候添加点，因为一直是动的
        int count = PointsParent.childCount;
        pointsTile.Clear();
        for (int i = 0; i < count; i++)
        {
            Vector2 pointWorldPos = PointsParent.GetChild(i).position;
            pointsTile.Add(pointWorldPos);
        }
        bool isPointInGround = true;
        bool isUnlockRigion = true;
        bool isCollisionOtherTile = false;
        for (int i = 0; i < pointsTile.Count; i++)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(pointsTile[i]);
            if (colliders == null || colliders.Length <= 0)//没有
            {
                actionNotAllowPlace?.Invoke();
                return;
            }
            var collisionColliders = colliders.ToList().FindAll(x => x.gameObject.CompareTag(TagHelper.Ground));
            if (collisionColliders == null || collisionColliders.Count <= 0)
            {//有一个没有和地面碰撞
                isPointInGround = false;
                break;
            }
            var collider = collisionColliders.Find(x => x.GetComponent<ManorRegionComponent>().regionId != 0);
            if (collider != null)
            {
                currCollisonGroundRegionId = collider.GetComponent<ManorRegionComponent>().regionId;
            }
            else
            {
                currCollisonGroundRegionId = 0;
            }
            if (!Root2dSceneManager._instance.GetRegionIsFinishUnLock(currCollisonGroundRegionId))
            {
                isUnlockRigion = false;
            }
            if (selfCollider2D == null)//用的时候找一次
            {
                selfCollider2D = GetComponent<Collider2D>();
            }
            var collisionOtherTileColliders = colliders.ToList().FindAll(x => x.gameObject.CompareTag(TagHelper.Tile));
            if (collisionOtherTileColliders != null && collisionOtherTileColliders.Count > 0)
            {//和其他Tile相交
                for (int eachTile = 0; eachTile < collisionOtherTileColliders.Count; eachTile++)
                {
                    if (collisionOtherTileColliders[eachTile] != selfCollider2D)
                    {
                        isCollisionOtherTile = true;
                        break;
                    }
                }
                if (isCollisionOtherTile)
                {
                    break;//跳出第二层循环
                }
            }
        }

        if (isPointInGround == false)//有一个没有和地面碰撞，不是所有点都在地面内
        {
            actionNotAllowPlace?.Invoke();
        }
        else if (isCollisionOtherTile)//和其他Tile相交
        {
            actionNotAllowPlace?.Invoke();
        }
        else if (isUnlockRigion == false)
        {
            actionNotAllowPlace?.Invoke();
        }
        else
        {
            actionCanPlace?.Invoke();
        }
    }

    //撤销放置操作
    private void CancelPlaceHandle()
    {

        if (isDragFromScene)
        {
            float zPos = transform.position.z;
            transform.position = originPos;
        }
        else
        {
            //不是地块
            if (TypeManorDecorate.Tile != typeTile)
            {
                Destroy(gameObject);
            }
            else
            {
                Root2dSceneManager._instance.RemoveObject(gameObject);
            }
        }

        //取消放置是播放摄像机回弹动画
        var worldComponent = Root2dSceneManager._instance.worldCameraComponent;
        worldComponent.PlayRebackCameraPos();
        //取消放置的时候也要排序
        Root2dSceneManager._instance.UpdateSortLayer(true);

    }

}
