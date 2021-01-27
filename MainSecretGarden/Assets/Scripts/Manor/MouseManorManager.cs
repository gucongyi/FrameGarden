using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Protocal;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class MouseManorManager : MonoBehaviour
{
    //作物位置列表
    [HideInInspector]
    public List<Vector2> plantPosList = new List<Vector2>();
    public MouseGenerateInfo mouseGenerateInfo;
    public GameObject Mouse;
    public GameObject MouseCollider;
    public GameObject HammerPivot;
    [HideInInspector]
    public long playerUid;

    private bool isActive = false;
    private bool isUser;
    private int generateTimeDelta;
    private TimeCountDownComponent mouseGenTimer;
    private Animator mouseAnim;
    private GameObject uiBoxReceive;
    private GameObject Hammer;
    private Button btnSingleReceive;
    private Button btnDoubleReceive;
    private Image itemIcon;
    private List<Vector2> mousePosList = new List<Vector2>();
    private int mouseNumMark = 0;   
    //老鼠的数字标志
    //表格配置
    //自己庄园生成地鼠概率
    float mouseRate = 0.2f;
    //好友庄园地鼠生成概率
    float mouseFriendRate = 0.1f;
    //玩家每日生成数量上限
    int mouseGenerateNum = 5;
    CancellationTokenSource ctsWaitTileReach;

    void Start()
    {
        Init();
    }
    private void OnDestroy()
    {
        SetBreakAwait();
    }
    private void Init()
    {
        ctsWaitTileReach = new CancellationTokenSource();
        mouseAnim = Mouse.GetComponent<Animator>();
        Hammer = HammerPivot.transform.Find("Hammer").gameObject;
        mouseRate = StaticData.configExcel.GetVertical().ManorSelfMouseRate;
        mouseFriendRate = StaticData.configExcel.GetVertical().ManorFriendMouseRate;
        mouseGenerateNum = StaticData.configExcel.GetVertical().ManorMarmotCount;
    }
    /// <summary>
    /// 获取生成地鼠的间隔时间
    /// </summary>
    /// <returns></returns>
    private int GetGenerateTimeDelta()
    {
        if (generateTimeDelta == 0)
        {
            //地鼠生成时间间隔，5分钟
            //generateTimeDelta = (int)(0.2 * 60);
            generateTimeDelta = StaticData.configExcel.GetVertical().ManorMarmotRefresh;
        }
        return generateTimeDelta;
    }
    /// <summary>
    /// 更新作物位置列表
    /// </summary>
    private void UpdatePlantPosList()
    {
        plantPosList.Clear();
        foreach (var elem in Root2dSceneManager._instance.GetListSeedGrow())
        {
            plantPosList.Add(elem.transform.position);
        }
    }
    /// <summary>
    /// 生成地鼠定时器
    /// </summary>
    /// <param name="timeDelta"></param>
    private void CreateMouseTimer(float timeDelta)
    {
        if (timeDelta == 0) 
        {
            timeDelta = GetGenerateTimeDelta();
        }
        StaticData.DebugGreen($"创建地鼠生成定时器：{timeDelta}");
        //创建生成地鼠定时器,时间比生产间隔多5秒
        mouseGenTimer = StaticData.CreateTimer(timeDelta/1000f + 5, true, (go) =>
        {
            if (isUser)
            {
                CheckSelfMouseGenerate();
            }
            Destroy(go);
            Destroy(mouseGenTimer.gameObject);
        },
        (remainTime) => { }, "MouseGenerate");
    }
    /// <summary>
    /// 检查玩家地鼠的生成
    /// </summary>
    public async UniTask CheckSelfMouseGenerate()
    {
        //1秒后再执行生成逻辑，防止其它依赖组件还未准备就绪
        await UniTask.Delay(1000, cancellationToken: ctsWaitTileReach.Token);
        isUser = true;
        UpdatePlantPosList();
        if (plantPosList.Count == 0) 
        {
            return;
        }
        LoadMouseInfo();
        //检查本地地鼠生成数据
        if (isActive)
        {
            SetMouseActiveAndMove();
            return;
        }
        //今日生成数量判定
        int nowYear = TimeHelper.ServerDateTimeNow.Year;
        int nowDay = TimeHelper.ServerDateTimeNow.DayOfYear;
        if (nowYear == mouseGenerateInfo.UserYear && nowDay == mouseGenerateInfo.UserDayOfYear && mouseGenerateInfo.UserMouseNum >= mouseGenerateNum)
        {
            StaticData.DebugGreen($"数量满了~今日生成地鼠数量:{mouseGenerateNum}");
            return;
        }
        long nowTime = TimeHelper.ServerTimeStampNow;
        if (nowTime - mouseGenerateInfo.UserLastGenerateTime < GetGenerateTimeDelta())
        {
            CreateMouseTimer((nowTime - mouseGenerateInfo.UserLastGenerateTime)/1000);
            return;
        }
        GenerateUserMouse();
    }
    /// <summary>
    /// 检查好友地鼠的生成
    /// </summary>
    public async UniTask CheckFriendMouseGenerate()
    {
        //1秒后再执行生成逻辑，防止其它依赖组件还未准备就绪
        await UniTask.Delay(1000, cancellationToken: ctsWaitTileReach.Token);
        isUser = false;
        UpdatePlantPosList();
        if (plantPosList.Count == 0)
        {
            return;
        }
        LoadMouseInfo();
        //好友每日生成数量上限
        int mouseGenerateNumFriend = 5;
        //检查时间和抓捕数量
        if (TimeHelper.ServerDateTimeNow.Year == mouseGenerateInfo.FriendYear && TimeHelper.ServerDateTimeNow.DayOfYear == mouseGenerateInfo.FriendDayOfYear && mouseGenerateInfo.ListFriendUidMouseCatch.Count >= mouseGenerateNumFriend)
        {
            return;
        }
        //更新好友日期数据
        if (!(TimeHelper.ServerDateTimeNow.Year == mouseGenerateInfo.FriendYear && TimeHelper.ServerDateTimeNow.DayOfYear == mouseGenerateInfo.FriendDayOfYear))
        {
            mouseGenerateInfo.ListFriendUidMouseCatch.Clear();
        }
        //检查该好友今日是否抓捕过地鼠
        if (mouseGenerateInfo.ListFriendUidMouseCatch.Contains(playerUid))
        {
            return;
        }
        GenerateFriendMouse();
    }
    /// <summary>
    /// 玩家地鼠生成计算
    /// </summary>
    private void GenerateUserMouse()
    {
        //功能是否开启
        if (!StaticData.IsOpenFunction(10010, false))
        {
            Debug.Log(string.Format("玩家地鼠生成计算 功能是否开启"));
            return;
        }
        Debug.Log(string.Format("玩家地鼠生成计算111 mouseRate:{0}", mouseRate));
        float rateCriticalPoint = mouseRate;
        float randomNum = RandomHelper.RandomNumber(0, 100)/100f;

        Debug.Log(string.Format("玩家地鼠生成计算111 rateCriticalPoint:{0}//randomNum:{1}", rateCriticalPoint, randomNum));
        if (plantPosList.Count > 0 && randomNum <= rateCriticalPoint)
        {
            isActive = true;
            //更改生成日期
            UpdateMouseDay();
        }
        else
        {
            StaticData.DebugGreen("生成地鼠的概率没中，创建下次生成定时器！");
            CreateMouseTimer(0);
        }
        mouseGenerateInfo.UserLastGenerateTime = TimeHelper.ServerTimeStampNow;
        StaticData.DebugGreen($"本次判定地鼠时间:{mouseGenerateInfo.UserLastGenerateTime}");
        //地鼠生成记录
        SetMouseActiveAndMove();
        SaveMouseInfo();
    }
    /// <summary>
    /// 好友地鼠生成计算
    /// </summary>
    private void GenerateFriendMouse()
    {
        //功能是否开启
        if (!StaticData.IsOpenFunction(10010, false))
        {
            return;
        }

        Debug.Log(string.Format("玩家地鼠生成计算 好友地鼠生成计算"));

        int rateCriticalPoint = (int)(100 * mouseFriendRate);
        int randomNum = RandomHelper.RandomNumber(0, 100);
        if (randomNum > rateCriticalPoint)
        {
            StaticData.DebugGreen("生成老鼠的概率没中！");
            return;
        }
        UpdateMouseDay();
        isActive = true;
        SetMouseActiveAndMove();
        //地鼠生成记录
        SaveMouseInfo();
    }
    /// <summary>
    /// 更新地鼠生成日期
    /// </summary>
    private void UpdateMouseDay()
    {
        if (isUser)
        {
            if (!(mouseGenerateInfo.UserYear == TimeHelper.ServerDateTimeNow.Year && mouseGenerateInfo.UserDayOfYear == TimeHelper.ServerDateTimeNow.DayOfYear))
            {
                mouseGenerateInfo.UserYear = TimeHelper.ServerDateTimeNow.Year;
                mouseGenerateInfo.UserDayOfYear = TimeHelper.ServerDateTimeNow.DayOfYear;
                mouseGenerateInfo.UserMouseNum = 1;
            }
        }
        else
        {
            if (!(mouseGenerateInfo.FriendYear == TimeHelper.ServerDateTimeNow.Year && mouseGenerateInfo.FriendDayOfYear == TimeHelper.ServerDateTimeNow.DayOfYear))
            {
                mouseGenerateInfo.FriendYear = TimeHelper.ServerDateTimeNow.Year;
                mouseGenerateInfo.FriendDayOfYear = TimeHelper.ServerDateTimeNow.DayOfYear;
            }
        }
    }
    /// <summary>
    /// 抓捕地鼠计数增加
    /// </summary>
    private void IncreaseCatchAmountUser()
    {
        if (!(mouseGenerateInfo.UserYear == TimeHelper.ServerDateTimeNow.Year && mouseGenerateInfo.UserDayOfYear == TimeHelper.ServerDateTimeNow.DayOfYear))
        {
            mouseGenerateInfo.UserYear = TimeHelper.ServerDateTimeNow.Year;
            mouseGenerateInfo.UserDayOfYear = TimeHelper.ServerDateTimeNow.DayOfYear;
            mouseGenerateInfo.UserMouseNum = 1;
        }
        else
        {
            mouseGenerateInfo.UserMouseNum += 1;
        }
    }
    /// <summary>
    /// 抓捕地鼠，请求服务器
    /// </summary>
    public async UniTask CatchMouse()
    {
        //抓到地鼠后碰撞器离开关闭
        MouseCollider.SetActive(false);
        mouseNumMark += 1;
        UIComponent.CreateUI(UIType.UIEventMask);
        //播放敲打动画
        HammerPivot.transform.localPosition = Mouse.transform.localPosition + new Vector3(0.5f, -0.3f, 0);
        Hammer.SetActive(true);
        HammerPivot.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.2f).OnComplete(() =>
        {
            HammerPivot.transform.DOLocalRotate(new Vector3(0,0,35), 0.1f).OnComplete(() =>
            {
                Hammer.SetActive(false);
            });
        });
        await UniTask.Delay(300, cancellationToken: ctsWaitTileReach.Token);
        //清除地鼠
        isActive = false;
        SaveMouseInfo();
        mouseAnim.Play("beida");
        //被打动画0.833s
        await UniTask.Delay(900, cancellationToken: ctsWaitTileReach.Token);
        if (gameObject == null) 
        {
            return;
        }
        SetMouseActiveAndMove();
        UIComponent.HideUI(UIType.UIEventMask);
        //请求服务器
        CSEmptyMarmotAwardInfo csEmptyMarmotAwardInfo = new CSEmptyMarmotAwardInfo()
        { };
        if (isUser)
        {
            ProtocalManager.Instance().SendCSEmptyMarmotAwardInfo(csEmptyMarmotAwardInfo, (scMarmotAwardInfo) =>
            {
                StaticData.DataDot(Company.Cfg.DotEventId.CatchMouseSucc);
                IncreaseCatchAmountUser();
                mouseGenerateInfo.UserLastGenerateTime = TimeHelper.ServerTimeStampNow;
                CreateMouseTimer(0);
                SaveMouseInfo();
                CreateReceiveBoxUI(scMarmotAwardInfo.AwardInfo);
            }, (error) => { });
        }
        else
        {
            CSFriendMarmot csFriendMarmot = new CSFriendMarmot()
            {
                FriendUid = playerUid
            };
            ProtocalManager.Instance().SendCSEmptyMarmotAwardInfo(csEmptyMarmotAwardInfo, (scMarmotAwardInfo) =>
            {
                StaticData.DataDot(Company.Cfg.DotEventId.CatchMouseSucc);
                mouseGenerateInfo.ListFriendUidMouseCatch.Add(playerUid);
                SaveMouseInfo();
                CreateReceiveBoxUI(scMarmotAwardInfo.AwardInfo);
            }, (error) => { });
        }
    }
    /// <summary>
    /// 玩家庄园抓地鼠奖励
    /// </summary>
    /// <param name="isAd"></param>
    private void GetMouseAwardUser(bool isAd)
    {
        CSOneselfMarmot csOneselfMarmot = new CSOneselfMarmot()
        {
            IsAdvert = isAd
        };
        ProtocalManager.Instance().SendCSOneselfMarmot(csOneselfMarmot, (scOneselfMarmot) => {
            foreach (var elem in scOneselfMarmot.AwardInfo)
            {
                StaticData.UpdateWareHouseItem(elem.GoodId, elem.GoodNum, elem.IsLock);

            }
            //待删，todo
            string tipStr = "请到仓库查看奖励！";
            StaticData.CreateToastTips(tipStr);
        }, (error) => { });
    }
    /// <summary>
    /// 好友庄园抓地鼠奖励
    /// </summary>
    /// <param name="isAd"></param>
    private void GetMouseAwardFriend(bool isAd)
    {
        CSFriendMarmot csFriendMarmot = new CSFriendMarmot()
        {
            FriendUid = playerUid,
            IsAdvert = isAd
        };
        ProtocalManager.Instance().SendCSFriendMarmot(csFriendMarmot, (scFriendMarmot) =>
        {
            foreach (var elem in scFriendMarmot.AwardInfo)
            {
                StaticData.UpdateWareHouseItem(elem.GoodId, elem.GoodNum, elem.IsLock);
            }
            //待删，todo
            string tipStr = "请到仓库查看奖励！";
            StaticData.CreateToastTips(tipStr);
        }, (error) => { });
    }
    /// <summary>
    /// 生成领取奖励UI
    /// </summary>
    /// <param name="awardInfo"></param>
    private void CreateReceiveBoxUI(CSWareHouseStruct awardInfo) 
    {
        uiBoxReceive = UIComponent.CreateUI(UIType.UIManorBoxReceive);
        //设置显示信息
        uiBoxReceive.GetComponent<UIManorMouseBox>().SetInfo(awardInfo, this);
    }
    
    /// <summary>
    /// 单倍奖励点击
    /// </summary>
    public void OnSingleReceiveClick()
    {
        if (isUser)
        {
            GetMouseAwardUser(false);
        }
        else 
        {
            GetMouseAwardFriend(false);
        }
        UIComponent.HideUI(UIType.UIManorBoxReceive);
    }
    /// <summary>
    /// 双倍奖励点击
    /// </summary>
    public void OnDoubleReceiveClick()
    {
        StaticData.OpenAd("MouseDoubleReceiveAd", (code, msg) => {
            if (code == 1)
            {
                StaticData.DataDot(Company.Cfg.DotEventId.CatchMouseDoubleAdGet);
                if (isUser)
                {
                    GetMouseAwardUser(true);
                }
                else
                {
                    GetMouseAwardFriend(true);
                }
                UIComponent.HideUI(UIType.UIManorBoxReceive);
            }
        });
        
    }
    /// <summary>
    /// 加载地鼠存储的信息
    /// </summary>
    public void LoadMouseInfo()
    {
        if (mouseGenerateInfo == null)
        {
            string path = UniversalTool.GetSaveFilePath("MonarMouse.json");
            string json = UniversalTool.LoadJson(path);
            StaticData.DebugGreen(path);
            if (!String.IsNullOrEmpty(json))
            {
                mouseGenerateInfo = JsonMapper.ToObject<MouseGenerateInfo>(json);
            }
            else
            {
                mouseGenerateInfo = new MouseGenerateInfo();
            }
            if (isUser) 
            {
                isActive = mouseGenerateInfo.IsActiveMouse;
            }
        }
    }
    /// <summary>
    /// 保存地鼠信息到json
    /// </summary>
    public void SaveMouseInfo()
    {
        if (isUser) 
        {
            mouseGenerateInfo.IsActiveMouse = isActive;
        }
        string path = UniversalTool.GetSaveFilePath("MonarMouse.json");
        string jsonStr = JsonMapper.ToJson(mouseGenerateInfo);
        UniversalTool.SaveJson(path, jsonStr);
    }
    /// <summary>
    /// 设置地鼠的激活和位置状态
    /// </summary>
    private void SetMouseActiveAndMove()
    {
        if (Mouse != null)
        {
            Mouse.SetActive(isActive);
            MouseCollider.SetActive(isActive);

        }
        if (isActive)
        {
            ChangeMouseMove();
        }

    }
    /// <summary>
    /// 在指定圆环内生成一个随机点
    /// </summary>
    /// <param name="pos">中心点</param>
    /// <param name="insideRadius">内圆半径</param>
    /// <param name="outsideRadius">外圆半径</param>
    /// <returns></returns>
    private Vector2 GetRandomPosInCircularRing(Vector2 centerPos, float insideRadius, float outsideRadius) 
    {
        //在内圆内随机生成一点
        Vector2 circlePoint = UnityEngine.Random.insideUnitCircle * insideRadius;
        //随机点等比向外扩展
        float circlePointRate = circlePoint.magnitude / insideRadius;
        float extendLength = insideRadius + circlePointRate * (outsideRadius - insideRadius);
        Vector2 outCirclePoint = circlePoint.normalized * extendLength;
        //将分布范围变为一个横宽竖窄的椭圆
        outCirclePoint.Scale(new Vector2(1.0f, 0.6f));
        Vector2 resultPoint = outCirclePoint + centerPos;
        return resultPoint;
    }
    /// <summary>
    /// 获取地鼠生成的位置
    /// </summary>
    /// <returns></returns>
    private void GetMousePos() 
    {
        mousePosList.Clear();
        float insideRadius = 3.0f;
        float outsideRadius = 6.0f;
        int randomPlantNum; //随机作物的索引
        Vector2 randomPlantPos; //随机作物的位置
        Vector2 randomMousePos; //随机作物的位置
        //查找地鼠坐标点次数
        int findPointTimes = 1000;
        for (int i = 0; i < 10; i++) 
        {
            //获取随机作物位置
            randomPlantNum = RandomHelper.RandomNumber(0, plantPosList.Count);
            randomPlantPos = plantPosList[randomPlantNum];
            int rightPoint = 0;
            for (int j = 0; j < findPointTimes; j++) 
            {
                randomMousePos = GetRandomPosInCircularRing(randomPlantPos, insideRadius * 0.025f * 0.1f, outsideRadius * 0.025f * 0.3f);
                if (CheckMousePosCorrect(randomMousePos))
                {
                    rightPoint += 1;
                    mousePosList.Add(randomMousePos);
                }
                if ( rightPoint > 10)
                {
                    break;
                }
            }
        }
    }

    List<Collider2D> listOverlapPointCollider = new List<Collider2D>();
    /// <summary>
    /// 检测生成点是否合法
    /// </summary>
    /// <param name="mousePos"></param>
    /// <returns></returns>
    private bool CheckMousePosCorrect(Vector2 mousePos) 
    {
        bool isCorrect = false;
        listOverlapPointCollider.Clear();
        Collider2D[] colliders = Physics2D.OverlapPointAll(mousePos);
        for (int i = 0; i < colliders.Length; i++)
        {
            listOverlapPointCollider.Add(colliders[i]);
        }
        var colliderGround = listOverlapPointCollider.FindAll(x => x.gameObject.CompareTag(TagHelper.Ground));
        var colliderTile = listOverlapPointCollider.FindAll(x => x.gameObject.CompareTag(TagHelper.Tile));
        if (colliderTile.Count == 0 && colliderGround.Count == 1 && colliderGround[0].name == "RegionAll") 
        {
            isCorrect = true;
        }
        return isCorrect;
    }
    /// <summary>
    /// 地鼠移动更变
    /// </summary>
    private void ChangeMouseMove() 
    {
        mouseNumMark += 1;
        Vector2 mousePos;
        if (mousePosList.Count == 0)
        {
            GetMousePos();
            //随机地点也没找到合适点，则判定地鼠生成失败
            if (mousePosList.Count == 0)
            {
                StaticData.DebugGreen("位置点数量为0~~~");
                isActive = false;
                SaveMouseInfo();
                if (isUser)
                {
                    CheckSelfMouseGenerate();
                }
                else 
                {
                    CheckFriendMouseGenerate();
                }
                return;
            }
        }
        //获取随机作物位置
        int randomMousePosNum = RandomHelper.RandomNumber(0, mousePosList.Count);
        mousePos = mousePosList[randomMousePosNum];
        mousePosList.RemoveAt(randomMousePosNum);
        //设置地鼠位置
        transform.position = new Vector2(mousePos.x, mousePos.y);
        //刷新地鼠和作物的层级
        Root2dSceneManager._instance.UpdateSortLayer(true, true);
        PlayMouseAnimBeforeCatch(mouseNumMark);
    }
    /// <summary>
    /// 老鼠被抓之前的动画播放
    /// </summary>
    /// <param name="markNum"></param>
    private async UniTask PlayMouseAnimBeforeCatch(int markNum) 
    {
        mouseAnim.Play("chuchang");
        await UniTask.Delay(3000, cancellationToken: ctsWaitTileReach.Token);
        if (CheckDelayCondAndMark(markNum))
            return;
        //播放随机待机动画
        int ramdonActionNum = RandomHelper.RandomNumber(0, 3);
        switch (ramdonActionNum)
        {
            case 0:
                mouseAnim.Play("daiji1");
                break;
            case 1:
                mouseAnim.Play("daiji2");
                break;
            case 2:
                mouseAnim.Play("daiji3");
                break;
        }
        await UniTask.Delay(5000,cancellationToken: ctsWaitTileReach.Token);
        if (CheckDelayCondAndMark(markNum))
            return;
        mouseAnim.Play("tuichang2");
        await UniTask.Delay(3000, cancellationToken: ctsWaitTileReach.Token);
        if (CheckDelayCondAndMark(markNum))
            return;
        ////重置动画播放
        mouseAnim.Play("empty");
        await UniTask.Delay(2000, cancellationToken: ctsWaitTileReach.Token);
        if (CheckDelayCondAndMark(markNum))
            return;
        ChangeMouseMove();
    }
    public void SetBreakAwait()
    {
        ctsWaitTileReach.Cancel();
    }
    private bool CheckDelayCondAndMark(int markNum) 
    {
        bool result = false;
        if (markNum != mouseNumMark) 
        {
            result = true;
        }
        return result;
    }
}

/// <summary>
/// 地鼠存储信息
/// </summary>
public class MouseGenerateInfo
{
    public bool IsActiveMouse;  //玩家地鼠是否为激活状态
    public long UserLastGenerateTime;   //玩家上一次生成地鼠的时间戳
    public int UserDayOfYear;   //玩家上一次生成地鼠的day数
    public int UserYear;    //玩家上一次生成地鼠的year数
    public int UserMouseNum;    //记录天数下的地鼠生成数量
    public int FriendDayOfYear; //好友上一次抓捕地鼠的day数
    public int FriendYear;  //好友上一次抓捕地鼠的year数
    public List<long> ListFriendUidMouseCatch = new List<long>();   //已抓捕地鼠的好友uid列表
}
public class MousePosInfo 
{
    public long PlayerUid;
    public float PosX;
    public float PosY;
}
