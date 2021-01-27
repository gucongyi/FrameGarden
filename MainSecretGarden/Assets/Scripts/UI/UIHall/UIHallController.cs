using Company.Cfg;
using Cysharp.Threading.Tasks;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 主界面
/// </summary>
public class UIHallController : MonoBehaviour
{
    #region 变量

    /// <summary>
    /// 进入界面动画时间
    /// </summary>
    [SerializeField]
    private float _enableUIAnimTime = 0.7f;
    /// <summary>
    /// 进入界面动画缓动类型
    /// </summary>
    [SerializeField]
    private Ease _enableUIAnimEase = Ease.InSine;

    [SerializeField]
    private float _disableUIAnimTime = 0.4f;
    //private Ease _disableUIAnimEase = Ease.InQuint;

    private Transform _top;
    private Transform _middle;
    private Transform _bottom;

    private Transform _bottomLeftList;
    private Transform _topRightList;

    private Button _partyBut;
    private Button _buddyBut;
    private Button _dressUpBut;

    private Button _activityBut;
    private Button _mailBut;
    private Button _taskBut;
    private GameObject _activityRedDot;

    private Button _warehouseBut;
    private Button _mallBut;
    private Button _manorBut;
    private Button _heartbeatBut;
    private Button _chapterBut;

    private GameObject _mallRedDot;

    public GameObject goRedDotTask;
    public GameObject _mailRedDot;
    public GameObject warehouseDot;
    public GameObject chapterDot;
    public GameObject manor;

    private CanvasGroup _topCG;
    private CanvasGroup _bottomCG;
    private CanvasGroup _middleLeftListCG;
    private CanvasGroup _middleRightListCG;

    private float _topYDef = 0f;
    private float _bottomYDef = 0f;
    private float _middleLeftListXDef = 0f;
    private float _middleRightListXDef = 0f;

    /// <summary>
    /// 是否需要播放ui动画
    /// </summary>
    private bool _isNeedPlayUIAnim = false;

    #region 角色

    /// <summary>
    /// 当前选中的角色
    /// </summary>
    private int _curSelectedRoleID = 0;

    /// <summary>
    /// 当前选中的角色时装id
    /// </summary>
    private int _curSelectedRoleChoiceID = 0;


    /// <summary>
    /// 生成的角色服装列表 
    /// </summary>
    private Dictionary<int, Live2DRoleControllerBase> _spawnRoleChoices = new Dictionary<int, Live2DRoleControllerBase>();



    #endregion

    #endregion

    #region 函数/方法

    private void InitAnimTra()
    {
        if (_top != null)
            return;

        _top = transform.Find("Top");
        _bottom = transform.Find("Bottom");
        _middle = transform.Find("Middle");
        _bottomLeftList = _middle.Find("Left_List");
        _topRightList = _middle.Find("Right_List");

        _topCG = _top.GetComponent<CanvasGroup>();
        _bottomCG = _bottom.GetComponent<CanvasGroup>();
        _middleLeftListCG = _bottomLeftList.GetComponent<CanvasGroup>();
        _middleRightListCG = _topRightList.GetComponent<CanvasGroup>();

        _topYDef = _top.localPosition.y;
        Debug.Log("_topYDef = " + _topYDef);
        _bottomYDef = _bottom.localPosition.y;
        Debug.Log("_bottomYDef = " + _bottomYDef);
        _middleLeftListXDef = _bottomLeftList.localPosition.x;
        _middleRightListXDef = _topRightList.localPosition.x;

        InitEvent();
    }

    private void InitEvent()
    {
        StaticData.CreatePlayerImage(_top.Find("UIPlayerInfo/AvatarTra"));
        StaticData.CreateCoinNav(_top.Find("UICurrencyInfo/GoldTra"));
        StaticData.CreateDiamondNav(_top.Find("UICurrencyInfo/DiamondTra"));
        //StaticData.CreateWaterNav(_top.Find("UICurrencyInfo/WaterTra"));

        _partyBut = _topRightList.Find("Party/But_BG").GetComponent<Button>();
        _buddyBut = _topRightList.Find("Buddy/But_BG").GetComponent<Button>();
        _dressUpBut = _topRightList.Find("DressUp/But_BG").GetComponent<Button>();

        _activityBut = _bottomLeftList.Find("Activity/But_BG").GetComponent<Button>();
        _mailBut = _bottomLeftList.Find("Mail/But_BG").GetComponent<Button>();
        _taskBut = _bottomLeftList.Find("Task/But_BG").GetComponent<Button>();

        _activityRedDot = _bottomLeftList.Find("Activity/RedDot").gameObject;

        _warehouseBut = _bottom.Find("BG/Warehouse").GetComponent<Button>();
        _mallBut = _bottom.Find("BG/Mall").GetComponent<Button>();
        _manorBut = _bottom.Find("BG/Manor").GetComponent<Button>();
        _heartbeatBut = _bottom.Find("BG/Heartbeat").GetComponent<Button>();
        _chapterBut = _bottom.Find("BG/Chapter").GetComponent<Button>();

        _mallRedDot = _bottom.Find("BG/Mall/RedDot").gameObject;

        _partyBut.onClick.RemoveAllListeners();
        _partyBut.onClick.AddListener(OnClickParty);
        _buddyBut.onClick.RemoveAllListeners();
        _buddyBut.onClick.AddListener(OnClickBuddy);

        _activityBut.onClick.RemoveAllListeners();
        _activityBut.onClick.AddListener(OnClickActivity);
        _mailBut.onClick.RemoveAllListeners();
        _mailBut.onClick.AddListener(OnClickMail);

        _warehouseBut.onClick.RemoveAllListeners();
        _warehouseBut.onClick.AddListener(OnClickWareHouse);
        _mallBut.onClick.RemoveAllListeners();
        _mallBut.onClick.AddListener(OnClickMall);
        _manorBut.onClick.RemoveAllListeners();
        _manorBut.onClick.AddListener(OnClickManor);
        _heartbeatBut.onClick.RemoveAllListeners();
        _heartbeatBut.onClick.AddListener(OnClickHeartbeat);
        _chapterBut.onClick.RemoveAllListeners();
        _chapterBut.onClick.AddListener(OnClickChapter);
        _taskBut.onClick.RemoveAllListeners();
        _taskBut.onClick.AddListener(OnButTaskClick);

        _dressUpBut.onClick.RemoveAllListeners();
        _dressUpBut.onClick.AddListener(OnClickDressUp);

        RegisterDot();
        UpdateRedDot();
        //
        InitAnim();

        OnEnableUpdateValue();

    }

    // Start is called before the first frame update
    void Start()
    {
        InitAnimTra();
    }
    /// <summary>
    /// 注册红点
    /// </summary>
    void RegisterDot()
    {
        RedDotManager.AddDotTra(RedDotManager.RedDotKey.Task, goRedDotTask.transform);
        RedDotManager.AddDotTra(RedDotManager.RedDotKey.Activity, _activityRedDot.transform);
        RedDotManager.AddDotTra(RedDotManager.RedDotKey.Mailbox, _mailRedDot.transform);
        RedDotManager.AddDotTra(RedDotManager.RedDotKey.Shopping, _mallRedDot.transform);
        RedDotManager.AddDotTra(RedDotManager.RedDotKey.Warehouse, warehouseDot.transform);
        RedDotManager.AddDotTra(RedDotManager.RedDotKey.Chapter, chapterDot.transform);
        RedDotManager.AddDotTra(RedDotManager.RedDotKey.Manor, manor.transform);
    }
    /// <summary>
    /// 更新红点
    /// </summary>
    void UpdateRedDot()
    {
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Task);
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Activity);
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Mailbox);
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Shopping);
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Warehouse);
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Chapter);
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Manor);
    }

    private void actionUpdateTaskRedDot(bool isOpen)
    {
        if (goRedDotTask != null)
        {
            goRedDotTask.SetActive(isOpen);
        }
    }
    /// <summary>
    /// 开关邮件红点
    /// </summary>
    /// <param name="isOpen"></param>
    private void actionUpdateMailRedDot(bool isOpen)
    {
        Debug.Log("actionUpdateMailRedDot 开关邮件红点 333");
        if (_mailRedDot != null)
            _mailRedDot.SetActive(isOpen);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRoleTouch();
    }
    private void UpdateManorGainInfo()
    {
        CSEmptyCropMature csemptycropmature = new CSEmptyCropMature();
        ProtocalManager.Instance().SendCSEmptyCropMature(csemptycropmature, succ =>
        {
            ManorRedDotTool.isManorHaveGain = succ.Mature;
            RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Manor);
        }, error => {
        });
    }

    private void OnEnable()
    {
        OnEnableUIAnim();
        //每次打开大厅请求一次
        UpdateManorGainInfo();

        if (_top != null)
            OnEnableUpdateValue();
    }


    /// <summary>
    /// 
    /// </summary>
    private void OnEnableUpdateValue()
    {
        //显示角色
        HallRoleManager.Instance.ShowRole();
        RegisterDot();
        UpdateRedDot();
    }

    private void OnDisable()
    {
        //隐藏角色
        if (HallRoleManager.Instance != null )
            HallRoleManager.Instance.HideRole();

        //需要显示情况
        if (UIRoleSwitchingController.Instance != null && UIRoleSwitchingController.Instance.gameObject.activeInHierarchy) 
        {
            HallRoleManager.Instance.ShowRole();
        }
    }


    #region 界面按钮操作

    /// <summary>
    /// 点击活动按钮
    /// </summary>
    private async void OnClickActivity()
    {

        StaticData.DataDot(Company.Cfg.DotEventId.ActivityIcon);
        //活动
        //if (!StaticData.IsOpenFunction(10016))
        //return;

        //打开签到界面,todo
        await UIComponent.CreateUIAsync(UIType.UISign);
    }

    /// <summary>
    /// 邮箱
    /// </summary>
    private async void OnClickMail()
    {

        StaticData.DataDot(Company.Cfg.DotEventId.MailIcon);
        //邮件
        if (!StaticData.IsOpenFunction(10004))
            return;
        StaticData.DataDot(DotEventId.MailIcon);
        await StaticData.OpenMailbox();

    }

    /// <summary>
    /// 任务按钮触发
    /// </summary>
    private async void OnButTaskClick()
    {

        //完成新手任务
        if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.isCurrStepGuiding)
        {
            GuideCanvasComponent._instance.SetLittleStepFinish();
        }
        await StaticData.OpenTaskPanel();
        StaticData.DataDot(DotEventId.TaskIcon);
    }



    /// <summary>
    /// 打开舞会界面
    /// </summary>
    private void OnClickParty()
    {

        StaticData.DataDot(Company.Cfg.DotEventId.EveningPartyIcon);
        //晚会
        if (!StaticData.IsOpenFunction(10017))
            return;

        //请求房间信息
        PartyServerDockingManager.NotifyServerRequestRoomList(RequestRoomListSuccess, RequestRoomListFailed);
    }

    /// <summary>
    /// 请求房间信息 成功
    /// </summary>
    /// <param name="sCRoomListInfo"></param>
    private async void RequestRoomListSuccess(SCRoomListInfo sCRoomListInfo)
    {
        await StaticData.OpenUIPartyChooseRoom(sCRoomListInfo);
    }

    /// <summary>
    /// 请求房间信息 失败
    /// </summary>
    /// <param name="sCRoomListInfo"></param>
    private void RequestRoomListFailed()
    {

    }

    /// <summary>
    /// 好友
    /// </summary>
    private async void OnClickBuddy()
    {

        StaticData.DataDot(Company.Cfg.DotEventId.MainFriendIcon);
        //好友
        if (!StaticData.IsOpenFunction(10006))
            return;

        await StaticData.OpenFriend(false);
    }

    /// <summary>
    /// 点击换装按钮 进入换装页面
    /// </summary>
    private async void OnClickDressUp()
    {

        StaticData.DataDot(Company.Cfg.DotEventId.FashionIcon);
        //好友
        if (!StaticData.IsOpenFunction(10019))
            return;
        await StaticData.OpenUIRoleSwitching();
    }


    /// <summary>
    /// 点击仓库
    /// </summary>
    private async void OnClickWareHouse()
    {

        //仓库
        if (!StaticData.IsOpenFunction(10002))
            return;
        StaticData.DataDot(DotEventId.WareHouseIcon);
        await StaticData.OpenWareHouse(2);
        if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.isCurrStepGuiding)
        {
            GuideCanvasComponent._instance.SetLittleStepFinish();
        }

    }

    /// <summary>
    /// 商城
    /// </summary>
    private async void OnClickMall()
    {

        StaticData.DataDot(Company.Cfg.DotEventId.ShopIcon);
        //商城
        if (!StaticData.IsOpenFunction(10008))
            return;

        await StaticData.OpenShopUI(2);


        //更新外部红点
        ShopTool.isLookStore = true;
        ShopTool.LookDataSave();//保存查看数据
        
        ShopTool.SavaShopData();//保存新链表

        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Shopping);
    }

    /// <summary>
    /// 进入庄园
    /// </summary>
    private async void OnClickManor()
    {

        StaticData.DataDot(Company.Cfg.DotEventId.ManorIcon);
        await StaticData.ToManorSelf();
    }

    /// <summary>
    /// 打开心动时刻
    /// </summary>
    private async void OnClickHeartbeat()
    {

        StaticData.DataDot(Company.Cfg.DotEventId.HeartMomentIcon);
        //心动时刻
        if (!StaticData.IsOpenFunction(10015))
            return;

        await StaticData.OpenImpulseUI();
    }

    /// <summary>
    /// 章节
    /// </summary>
    private async void OnClickChapter()
    {

        StaticData.DataDot(Company.Cfg.DotEventId.ChapterIcon);
        //章节是否开启
        if (!StaticData.IsOpenFunction(10007))
            return;

        await StaticData.OpenChapterUI();
    }


    #endregion

    #region 角色


    /// <summary>
    /// 更新角色是否可以被点击
    /// </summary>
    private void UpdateRoleTouch()
    {
        int index = transform.GetSiblingIndex();
        int num = transform.parent.childCount;

        if (index == num - 1)
        {
            //通知开启角色点击
            HallRoleManager.Instance.NotifyRoleTouchIsActive(true);
            return;
        }

        for (int i = num - 1; i > index; i--)
        {
            if (transform.parent.GetChild(i).gameObject.activeInHierarchy)
            {
                //通知角色点击无效
                HallRoleManager.Instance.NotifyRoleTouchIsActive(false);
                return;
            }
        }

        //通知开启角色点击
        HallRoleManager.Instance.NotifyRoleTouchIsActive(true);
    }

    #endregion

    #region UI动画



    private void InitAnim()
    {
        InitAnimTra();

        Vector3 targetPos = _top.localPosition;
        targetPos.y = _topYDef + 360;
        _top.localPosition = targetPos;

        targetPos = _bottom.localPosition;
        targetPos.y = _bottomYDef - 360;
        _bottom.localPosition = targetPos;

        targetPos = _bottomLeftList.localPosition;
        targetPos.x = _middleLeftListXDef - 360;
        _bottomLeftList.localPosition = targetPos;

        targetPos = _topRightList.localPosition;
        targetPos.x = _middleRightListXDef + 360;
        _topRightList.localPosition = targetPos;

        _topCG.alpha = 0.0f;
        _bottomCG.alpha = 0.0f;
        _middleLeftListCG.alpha = 0.0f;
        _middleRightListCG.alpha = 0.0f;

        if (_isNeedPlayUIAnim)
        {
            Debug.Log("InitAnim _isNeedPlayUIAnim = true");
            _isNeedPlayUIAnim = false;
            OnEnableUIAnim();
        }
    }

    private void OnEnableUIAnim()
    {
        if (_middleLeftListXDef == 0)
        {
            Debug.Log("OnEnableUIAnim _isNeedPlayUIAnim = true");
            _isNeedPlayUIAnim = true;
            return;
        }

        Debug.Log("OnEnableUIAnim _isNeedPlayUIAnim = false");
        InitAnim();

        _top.DOLocalMoveY(_topYDef, _enableUIAnimTime).SetEase(_enableUIAnimEase);
        DOTween.To(() => _topCG.alpha, alpha => _topCG.alpha = alpha, 1f, _enableUIAnimTime).SetEase(_enableUIAnimEase);
        _bottom.DOLocalMoveY(_bottomYDef, _enableUIAnimTime).SetEase(_enableUIAnimEase);
        DOTween.To(() => _bottomCG.alpha, alpha => _bottomCG.alpha = alpha, 1f, _enableUIAnimTime).SetEase(_enableUIAnimEase);

        _bottomLeftList.DOLocalMoveX(_middleLeftListXDef, _enableUIAnimTime).SetEase(_enableUIAnimEase);
        DOTween.To(() => _middleLeftListCG.alpha, alpha => _middleLeftListCG.alpha = alpha, 1f, _enableUIAnimTime).SetEase(_enableUIAnimEase);
        _topRightList.DOLocalMoveX(_middleRightListXDef, _enableUIAnimTime).SetEase(_enableUIAnimEase);
        DOTween.To(() => _middleRightListCG.alpha, alpha => _middleRightListCG.alpha = alpha, 1f, _enableUIAnimTime).SetEase(_enableUIAnimEase);
    }


    private void OnDisableUIAnim()
    {
        _top.DOLocalMoveY(_topYDef + 360, _disableUIAnimTime);
        DOTween.To(() => _topCG.alpha, alpha => _topCG.alpha = alpha, 0f, _disableUIAnimTime);
        _bottom.DOLocalMoveY(_bottomYDef - 360, _disableUIAnimTime);
        DOTween.To(() => _bottomCG.alpha, alpha => _bottomCG.alpha = alpha, 0f, _disableUIAnimTime);

        _bottomLeftList.DOLocalMoveX(_middleLeftListXDef - 360, _disableUIAnimTime);
        DOTween.To(() => _middleLeftListCG.alpha, alpha => _middleLeftListCG.alpha = alpha, 0f, _disableUIAnimTime);
        _topRightList.DOLocalMoveX(_middleRightListXDef + 360, _disableUIAnimTime);
        DOTween.To(() => _middleRightListCG.alpha, alpha => _middleRightListCG.alpha = alpha, 0f, _disableUIAnimTime);
    }

    #endregion

    #endregion

}
