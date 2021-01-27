using Boo.Lang;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Protocal;
using Live2D.Cubism.Rendering;
using Org.BouncyCastle.Math.EC.Custom.Sec;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// 舞会管理器
/// </summary>
public class PartyManager : MonoBehaviour
{
    #region 变量

    public static PartyManager _instance;

    private Tweener _tweenerCamara;

    //服务器当前时间戳
    public long dateTime;

    //当前时间与活动结束时间差
    public float timeDifference;

    //角色管理器
    public PartyPlayerManager _partyPlayerManager;


    //玩家是否一直在晚会房间中
    public bool stayRoom = false;

    //晚会是否结束
    private bool partyEnd;

    #region 相机

    /// <summary>
    /// 相机定位点/限制点 左下
    /// </summary>
    private Vector3 _leftLowerCamaraPos = Vector3.zero;
    /// <summary>
    /// 相机定位点/限制点 右上下
    /// </summary>
    private Vector3 _rightUpCamaraPos = Vector3.zero;

    /// <summary>
    /// 地图组件名称
    /// </summary>
    private string _mapName = "Map";

    private Camera _useCamera;
    public Camera UseCamera 
    {
        get { return _useCamera; }
    }

    #endregion

    /// <summary>
    /// 经验发放检测间隔时间
    /// </summary>
    private float _soireeExperienceDetectTime = 1.0f;
    private float _expTimer = 0;
    #endregion

    #region 函数
    public PartyManager() 
    {
        if (_instance == null || _instance != this)
            _instance = this;
    }

    private void Start()
    {
        //Init();
    }

    private void Update()
    {
        GetOnClickScenePoint();

        RoleRanking();

        MoveTimer();

        //UpdateExperienceDistribution();
    }

    private void OnDestroy()
    {
        ClearCallback();
    }

    private void Init()
    {
        if (_useCamera == null && UICameraManager._instance != null)
            _useCamera = UICameraManager._instance.GetUiCamera();
        //相机定位
        GetCamaraLocationPoint();

        if (_partyPlayerManager == null)
            _partyPlayerManager = new PartyPlayerManager();

        InitCallback();
    }

    public async void InitValue(SCEntranceRoom sCEntranceRoom)
    {
        Init();
        _partyPlayerManager.SpawnRoleList(sCEntranceRoom, transform.Find(_mapName));
        //打开晚会主界面
        await OpenUIPartyMain();

        //玩家进入房间的时候获取当前时间戳
        GetTime();

        //初始化晚会是否结束状态
        partyEnd = false;


        SetTaskTime();

    }

    //测试
    private void SetTaskTime()
    {
        DateTime now = DateTime.Now;

        DateTime startGame = DateTime.Today.AddHours(19.0);//晚上7点

        if (now > startGame)
        {
            startGame = startGame.AddDays(1.0);
        }

        Debug.Log("游戏开始时间时间戳:" + PartyGuessTime.ConvertDateTimeToInt(startGame));

        Debug.Log("游戏开始时间"+startGame);



    }


    /// <summary>
    /// 获取点击屏幕的点
    /// </summary>
    private void GetOnClickScenePoint()
    {
        //鼠标左键/点击
        if (Input.GetMouseButtonUp(0))
        {
            //点击的是对象
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                Debug.Log("点击的对象名称："+ EventSystem.current.currentSelectedGameObject.name);
                EventSystem.current.SetSelectedGameObject(null);
                return;
            }
            //角色移动
            if (_partyPlayerManager != null)
                _partyPlayerManager.PlayerMove(Input.mousePosition);

            #region 点击
            ////需要碰撞到物体才可以
            //Ray ray = _useCamera.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;
            //bool isCollider = Physics.Raycast(ray, out hit);
            //Debug.Log("当前点击的屏幕位置："+ Input.mousePosition);
            //if (isCollider)
            //{
            //    Debug.Log("射线检测到的点是" + hit.point);
            //}

            ////判断是否点击到button
            //if (EventSystem.current.currentSelectedGameObject != null)
            //{
            //    //点击的button
            //    //设置 空
            //    EventSystem.current.SetSelectedGameObject(null);
            //}
            //else
            //{
            //    //点击的是屏幕``11
            //}
            #endregion
        }
    }

    /// <summary>
    /// 退出房间
    /// </summary>
    public async void QuitRoom() 
    {
        //销毁全部玩家
        if (_partyPlayerManager != null)
            _partyPlayerManager.PushPartyEnd();
        _partyPlayerManager = null;
        //关闭全部界面
        //跳转到主页
        await StaticData.PartyReturnLobby();
    }

    /// <summary>
    /// 获取当前服务器时间戳
    /// </summary>
    private void GetTime()
    {
        dateTime = TimeHelper.ServerTimeStampNow;
        Debug.Log("当前时间：" + dateTime);

        //测试数据
        dateTime = 1605070840000;

        //判断进行了几轮下注
        Debug.Log("进行的轮数："+(int)(dateTime - PartyGuessTime.Instance.startPartyTime) / 1000 / 60);
        
    }


    /// <summary>
    /// 竞猜倒计时 切换状态
    /// </summary>
    /// <returns></returns>
    public void GuessCountDown()
    {
        if (PartyManager._instance == null)
            return;

        //晚会活动结束
        if (partyEnd)
            return;


        //本阶段倒计时
        if (PartyGuessManager.Instance.TimeDifference >0)
        {
            //时间差向上取整 便于向用户展示 临时变量num
            var num = Math.Ceiling(PartyGuessManager.Instance.TimeDifference);
            
            UIPartyMainController._instance.countDownText.text = num.ToString();

            PartyGuessManager.Instance.TimeDifference -= Time.deltaTime;

            return;
        }

        Debug.Log("结束倒计时，进入下一个阶段");

        //切换阶段 
        switch (PartyGuessTime.Instance.guessState)
        {
            case guessState.bottompour:
                PartyGuessTime.Instance.guessState = guessState.guess;
                if (PartyGuessController.Instance.isGuessPalyer)
                {
                    PartyGuessController.Instance.StartGuess();//开始竞猜 停止下注
                }
                else
                {
                    PartyGuessController.Instance.StartGuessNoData();//开始竞猜，玩家没有下注
                }
                break;
            case guessState.guess:
                PartyGuessTime.Instance.guessState = guessState.bottompour;
                if (PartyGuessController.Instance.isGuessPalyer)
                {
                    PartyGuessController.Instance.EndGuess();//停止竞猜 可以下注
                }
                else
                {
                    PartyGuessController.Instance.EndGuessNoData();//停止竞猜
                }
                break;
        }
        PartyGuessTime.Instance.GuessTimeControl();//进入下一个阶段的倒计时
        UIPartyMainController._instance.ChangeGuessState();
    }


    /// <summary>
    /// 晚会倒计时
    /// </summary>
    public void PartyCountDown()
    {
        //如果晚会结束 就退出循环
        if (partyEnd) return;

        if (timeDifference <= 0)
        {
            EndGuess();
            return;
        }


        //时间差向上取整 便于向用户展示 临时变量num
        var num = Math.Ceiling(timeDifference);

        UIPartyMainController._instance.partyCountDownText.text = num.ToString();

        timeDifference -= Time.deltaTime;
    }

    /// <summary>
    /// 晚会活动结束
    /// </summary>
    private void EndGuess()
    {
        //如果是在下注阶段就关闭下注界面，如果在竞猜界面就关闭竞猜界面
        switch (PartyGuessTime.Instance.guessState)
        {
            case guessState.bottompour:
                //关闭下注界面
                StaticData.CloseGuess();
                break;
            case guessState.guess:
                //关闭竞猜结果界面
                StaticData.CloseGuessResultUI();
                break;
            default:
                break;
        }

        partyEnd = true;

        //关闭竞猜按钮
        UIPartyMainController._instance._butInGuess.gameObject.SetActive(false);

        Debug.Log("晚会结束");
    }


    #region 角色处理函数

    /// <summary>
    /// 更新角色移动时间
    /// </summary>
    /// <param name="movingTime"></param>
    public void UpdatePlayerMovingTime(float movingTime)
    {
        if (_partyPlayerManager != null)
            _partyPlayerManager.MovingTime = movingTime;
    }

    /// <summary>
    /// 对角色进行排序
    /// </summary>
    private void RoleRanking()
    {
        if (_partyPlayerManager != null)
            _partyPlayerManager.RoleRanking();
    }

    /// <summary>
    /// 移动计时
    /// </summary>
    private void MoveTimer() 
    {
        if (_partyPlayerManager != null)
            _partyPlayerManager.MoveTimer(Time.deltaTime);
    }

    /// <summary>
    /// 生成角色
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GameObject SpawnRole(GameObject prefab, Transform parent) 
    {
        return Instantiate(prefab, parent);
    }

    /// <summary>
    /// 销毁角色
    /// </summary>
    /// <param name="player"></param>
    public void DestroyPlayer(GameObject player) 
    {
        player.SetActive(false);
        Destroy(player);
    }

    #endregion

    #region 相机位置限定

    /// <summary>
    /// 相机跟随
    /// </summary>
    /// <param name="target"></param>
    public void CamaraFollow(Vector3 target, float moveTime)
    {
        CamaraPosLimit(ref target);
        if (_tweenerCamara != null && _tweenerCamara.IsPlaying())
        {
            _tweenerCamara.Kill();
        }
        _tweenerCamara = _useCamera.transform.DOMove(target, moveTime);
    }

    /// <summary>
    /// 获取相机的定位点
    /// </summary>
    private void GetCamaraLocationPoint()
    {
        SpriteRenderer spriteRenderer = transform.Find(_mapName).GetComponent<SpriteRenderer>();
        //Debug.Log("spriteRenderer.sprite.rect.width = " + spriteRenderer.size.x);
        //Debug.Log("spriteRenderer.sprite.rect.height = " + spriteRenderer.size.y);

        _leftLowerCamaraPos.x = spriteRenderer.size.x / 2 * -1;
        _leftLowerCamaraPos.y = spriteRenderer.size.y / 2 * -1;
        _rightUpCamaraPos.x = spriteRenderer.size.x / 2;
        _rightUpCamaraPos.y = spriteRenderer.size.y / 2;


        _leftLowerCamaraPos = spriteRenderer.transform.parent.TransformPoint(_leftLowerCamaraPos);
        _rightUpCamaraPos = spriteRenderer.transform.parent.TransformPoint(_rightUpCamaraPos);

        var screenPoint = _useCamera.WorldToScreenPoint(_leftLowerCamaraPos);
        screenPoint.x += Screen.width / 2;
        screenPoint.y += Screen.height / 2;
        _leftLowerCamaraPos = _useCamera.ScreenToWorldPoint(screenPoint);

        screenPoint = _useCamera.WorldToScreenPoint(_rightUpCamaraPos);
        screenPoint.x -= Screen.width / 2;
        screenPoint.y -= Screen.height / 2;
        _rightUpCamaraPos = _useCamera.ScreenToWorldPoint(screenPoint);

        Debug.Log("_leftLowerCamaraPos " + _leftLowerCamaraPos);
        Debug.Log("_rightUpCamaraPos " + _rightUpCamaraPos);
    }

    /// <summary>
    /// 相机位置限制
    /// </summary>
    /// <param name="targetPos"></param>
    private void CamaraPosLimit(ref Vector3 pos)
    {
        if (pos.x < _leftLowerCamaraPos.x)
        {
            pos.x = _leftLowerCamaraPos.x;
        }
        else if (pos.x > _rightUpCamaraPos.x)
        {
            pos.x = _rightUpCamaraPos.x;
        }

        if (pos.y < _leftLowerCamaraPos.y)
        {
            pos.y = _leftLowerCamaraPos.y;
        }
        else if (pos.y > _rightUpCamaraPos.y)
        {
            pos.y = _rightUpCamaraPos.y;
        }
    }
    #endregion

    #region 界面

    /// <summary>
    /// 打开晚会主界面界面
    /// </summary>
    public async UniTask OpenUIPartyMain()
    {
        await UIComponent.CreateUIAsync(UIType.UIPartyMain);
    }

    #endregion

    #region 回调函数 推送事件

    /// <summary>
    /// 初始化推送回调 
    /// </summary>
    private void InitCallback() 
    {
        PartyServerDockingManager.PushEntranceRoomCallback = PushPlayerEnterRoom;
        PartyServerDockingManager.PushPlayerMoveCallback = PushPlayerMove;
        PartyServerDockingManager.PushPlayerQuitRoomCallback = PushPlayerQuitRoom;
        PartyServerDockingManager.PushPartyEndCallback = PushPartyEnd;
        PartyServerDockingManager.PushPartyGuessInfoCallback = PushPartyGuessInfo;
    }

    /// <summary>
    /// 清空推送回调 
    /// </summary>
    private void ClearCallback() 
    {
        PartyServerDockingManager.PushEntranceRoomCallback = null;
        PartyServerDockingManager.PushPlayerMoveCallback = null;
        PartyServerDockingManager.PushPlayerQuitRoomCallback = null;
        PartyServerDockingManager.PushPartyEndCallback = null;
    }

    /// <summary>
    /// 推送玩家进入房间
    /// </summary>
    /// <param name="enterPlayer"></param>
    private void PushPlayerEnterRoom(SCEntranceRoomInfo enterPlayer) 
    {
        if (_partyPlayerManager != null)
            _partyPlayerManager.PushNewPlayerJoin(enterPlayer);
    }

    /// <summary>
    /// 推送玩家移动
    /// </summary>
    /// <param name="moveInfo"></param>
    private void PushPlayerMove(SCMoveLocation moveInfo) 
    {
        if (_partyPlayerManager != null)
            _partyPlayerManager.PushPlayerMove(moveInfo);
    }

    /// <summary>
    /// 推送有玩家退出房间
    /// </summary>
    /// <param name="moveInfo"></param>
    private void PushPlayerQuitRoom(SCDepartureRoom quitInfo)
    {
        if (_partyPlayerManager != null)
            _partyPlayerManager.PushPlayerQuitRoom(quitInfo);
    }

    /// <summary>
    /// 推送晚会结束(包含晚会结束时间)
    /// </summary>
    /// <param name="moveInfo"></param>
    private void PushPartyEnd(SCActivityFinish endInfo)
    {
        QuitRoom();

        //活动结束时间戳;
        Debug.Log("活动结束时间戳:" + endInfo.EndTime);
    }

    //测试 模拟获取结束时间戳 计算倒计时
    public void TestPushPartyActivityFinish()
    {
        //测试数据活动结束时间戳
        long activityFinishTime = 1605070980000;
        //测试数据当前时间戳
        long dateTime = 1605070960000;

        timeDifference = (activityFinishTime - dateTime) / 1000;
    }


    //测试数据竞猜比赛信息
    public string test="";

    /// <summary>
    /// 推送晚会竞猜比赛信息
    /// </summary>
    /// <param name="guessInfo"></param>
    private void PushPartyGuessInfo(SCpushGuessingInfo guessInfo)
    {

        Debug.Log(guessInfo.GuessingInfoOne);
        Debug.Log(guessInfo.GuessingInfoTwo);
        Debug.Log(guessInfo.GuessingInfoThree);


        //测试 判断哪个乌龟最终获胜

        switch (TestGuess(guessInfo))
        {
            case GuessingNumber.GuessingFirst:
                test = "WG1胜利";
                break;
            case GuessingNumber.GuessingSecond:
                test = "WG2胜利";
                break;
            case GuessingNumber.GuessingThird:
                test = "WG3胜利";
                break;
        }


    }


    //测试  始终是第一个乌龟胜利（不是指GuessingNumber.GuessingFirst）
    private GuessingNumber TestGuess(SCpushGuessingInfo guessInfo)
    {
        if (guessInfo.GuessingInfoOne.ElementAt(guessInfo.GuessingInfoOne.Count - 1).Status.ElementAt(0).Status == GuessingStatus.StatusFinish)
        {
            return guessInfo.GuessingInfoOne.ElementAt(guessInfo.GuessingInfoOne.Count - 1).Id;
        }
        if (guessInfo.GuessingInfoTwo.ElementAt(guessInfo.GuessingInfoOne.Count - 1).Status.ElementAt(0).Status == GuessingStatus.StatusFinish)
        {
            return guessInfo.GuessingInfoTwo.ElementAt(guessInfo.GuessingInfoOne.Count - 1).Id;
        }
        if (guessInfo.GuessingInfoThree.ElementAt(guessInfo.GuessingInfoOne.Count - 1).Status.ElementAt(0).Status == GuessingStatus.StatusFinish)
        {
            return guessInfo.GuessingInfoThree.ElementAt(guessInfo.GuessingInfoOne.Count - 1).Id;
        }
        else
        {
            return GuessingNumber.GuessingNone;
        }
    }
    #endregion 推送

    #region 晚会开始结束

    /*
    /// <summary>
    /// 更新经验发放
    /// </summary>
    private void UpdateExperienceDistribution() 
    {
        //每秒检测一次
        _expTimer += Time.deltaTime;
        if (_expTimer < _soireeExperienceDetectTime)
            return;

        _expTimer -= _soireeExperienceDetectTime;

        //游戏是否开始 游戏是否结束
        var curServerTime = TimeHelper.ServerDateTimeNow;
        if (curServerTime.Second != 0)
        {
            return;
        }

        var startTime = StaticData.configExcel.GetVertical().SoireeStartTime;
        //晚会进行中
        if (IsStartParty(curServerTime) && !IsEndParty(curServerTime)) 
        {
            int hour = curServerTime.Hour - startTime[0];
            int minute = curServerTime.Minute - startTime[1];
            minute += hour * 60;
            if (minute % StaticData.configExcel.GetVertical().SoireeExperience == 0) 
            {
                //发放 经验奖励
            }
        }
        
    }
    */


    /// <summary>
    /// 晚会是否开始
    /// </summary>
    /// <param name="curServerTime"></param>
    /// <returns></returns>
    public static bool IsStartParty(DateTime curServerTime) 
    {
        var startTime = StaticData.configExcel.GetVertical().SoireeStartTime;

        if (curServerTime.Hour > startTime[0])
        {
            return true;
        } else if (curServerTime.Hour == startTime[0]) 
        {
            if (curServerTime.Minute > startTime[1])
            {
                return true;
            } else if (curServerTime.Minute == startTime[1]) 
            {
                if (curServerTime.Second >= startTime[2])
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 晚会是否结束
    /// </summary>
    /// <param name="curServerTime"></param>
    /// <returns></returns>
    public static bool IsEndParty(DateTime curServerTime)
    {
        var EndTime = StaticData.configExcel.GetVertical().SoireeEndTime;

        if (curServerTime.Hour > EndTime[0])
        {
            return true;
        }
        else if (curServerTime.Hour == EndTime[0])
        {
            if (curServerTime.Minute > EndTime[1])
            {
                return true;
            }
            else if (curServerTime.Minute == EndTime[1])
            {
                if (curServerTime.Second > EndTime[2])
                    return true;
            }
        }
        return false;
    }


    #endregion

    

    #endregion
}
