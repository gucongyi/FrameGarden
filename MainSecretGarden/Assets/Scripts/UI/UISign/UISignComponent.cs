using Company.Cfg;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using static RedDotManager;

public class UISignComponent : MonoBehaviour
{
    public Text _accuDayText;//累积签到天数
    public Text _todaySignText;
    public Button _CloseVowTip;
    public Transform _VowTip;
    public Text _VowTipGrade;

    #region 许愿牌
    public List<Button> plateList=new List<Button>();
    #endregion

    #region 点击特效
    private Transform _clickEff;
    #endregion


    private int nextAccuAwardDay = 0;
    private int nextAccuAwardId = 0;
    private int nextAccuAwardCount = 0;

    private long nextSignTime=0;//下次签到时间戳

    //组件
    private Transform _vow;
    private Transform _other;
    private Button _backBtn;
    private Toggle _tabVow;
    private Toggle _tabOther;
    private Text _vowRedText;
    private Text _otherRedText;
    private Transform _redHint;//每日许愿活动提醒

    //累积奖励无限列表
    public LoopHorizontalScrollRect ls;
    //许愿奖励无限列表
    public LoopHorizontalScrollRect lsVow;


    //奖励配置链表
    public List<SignInDefine> listSign;
    //许愿奖励配置链表
    public List<SCEverydayAward> LsSCVowAward=new List<SCEverydayAward>();

    //许愿奖励
    private SCEverydayAward sCVowAward;

    //LS下的Content；累积奖励
    public Transform content;
    //每日许愿
    public Transform contentTip;

    //用于每日许愿奖励动画
    public List<Transform> lsItemVowAnim;

    void Start()
    {
        Init();
    }
    private void Init() 
    {
        _vow = transform.Find("Vow");
        _other = transform.Find("Other");
        _backBtn = transform.Find("BackBtn").GetComponent<Button>();
        _tabVow = transform.Find("Tab/TabVow").GetComponent<Toggle>();
        _tabOther = transform.Find("Tab/TabOther").GetComponent<Toggle>();
        _vowRedText = transform.Find("Tab/TabVow/ClickMask/Text").GetComponent<Text>();
        _otherRedText= transform.Find("Tab/TabOther/ClickMask/Text").GetComponent<Text>();
        _redHint = transform.Find("Tab/TabVow/Hint");

        //获取点击特效
        _clickEff = UIRoot.instance.GetClickEffect();

        RegisterEventListener();
        InitAward();
        InitTodaySign();


    }

    //用于玩家在线判断签到刷新
    private void OnEnable()
    {
        if (_todaySignText == null|| _redHint==null|| plateList.Count==0) return;
        InitTodaySign();
    }

    private void RegisterEventListener() 
    {
        #region 许愿牌监听
        if (plateList.Count != 0)
        {
            for (int i = 0; i < plateList.Count; i++)
            {
                plateList[i].onClick.RemoveAllListeners();
                plateList[i].onClick.AddListener(OnButtonTodaySignClick);
            }
        }
        #endregion



        _backBtn.onClick.RemoveAllListeners();
        _backBtn.onClick.AddListener(OnBackClick);
        _CloseVowTip.onClick.RemoveAllListeners();
        _CloseVowTip.onClick.AddListener(OnClickVowTip);
    }
    private async void OnBackClick() 
    {
        //退出动画
        if (_vow.gameObject.activeSelf)
        {
            _vow.GetComponent<UIVowControll>().BackAnim();
        }

        //更新红点
        RedDotManager.UpdateRedDot(RedDotKey.Activity);

        await UniTask.Delay(TimeSpan.FromSeconds(0.3));

        //还原标签页状态
        _tabVow.isOn = true;
        _tabOther.isOn = false;


        UIComponent.HideUI(UIType.UISign);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //打开许愿弹窗
            _VowTip.gameObject.SetActive(true);
        }
    }


    /// <summary>
    /// 点击今日签到(今日许愿)
    /// </summary>
    private void OnButtonTodaySignClick() 
    {
        //播放点击特效
        _clickEff.localPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);//获取鼠标点击位置
        _clickEff.GetComponent<ParticleSystem>().Play();

        //请求服务器进行今日签到
        CSEmptySignIn csEmptySignIn = new CSEmptySignIn();
        ProtocalManager.Instance().SendCSEmptySignIn(csEmptySignIn, (scEverydayAward) =>
        {
            StaticData.DebugGreen($"包裹id:{scEverydayAward.ParcelId},道具id:{scEverydayAward.GoodId},道具数量:{scEverydayAward.GoodNum}");
            //许愿奖励配置
            sCVowAward = scEverydayAward;
            GetVowData();
            _VowTipGrade.text = StaticData.configExcel.GetPackageByID(scEverydayAward.ParcelId).PackageName;

            //道具入库
            StaticData.UpdateWareHouseItem(scEverydayAward.GoodId, scEverydayAward.GoodNum);
            StaticData.playerInfoData.userInfo.SignDays += 1;
            if (StaticData.playerInfoData.userInfo.SignInInfo.Count == 0)
            {
                StaticData.playerInfoData.userInfo.SignInInfo.Add(new SCSignInStruct()
                {
                    DayNumber = 1,
                    SignInTime = TimeHelper.ServerTimeStampNow
                });
            }
            else
            {
                StaticData.playerInfoData.userInfo.SignInInfo[0].SignInTime = TimeHelper.ServerTimeStampNow;
            }

            //InitTodaySign();

            //重置签到时间
            for (int i = 0; i < StaticData.playerInfoData.userInfo.SignInInfo.Count; i++)
            {
                var data = StaticData.playerInfoData.userInfo.SignInInfo[i];
                if (data.DayNumber == 1)
                {
                    data.SignInTime = nextSignTime;
                    break;
                }
            }
            //更新今日许愿UI
            _todaySignText.text = "今日已许愿";
            for (int i = 0; i < plateList.Count; i++)
            {
                plateList[i].enabled = false;
            }
            _redHint.gameObject.SetActive(false);

            //更新累积签到天数
            _accuDayText.text = StaticData.playerInfoData.userInfo.SignDays.ToString();
            //更新累积奖励UI
            for (int i = 0; i < content.childCount; i++)
            {
                content.GetChild(i).GetComponent<UISignOfAward>().JudgeAccuAward();
            }
            //循环添加每日许愿奖励
            for (int i = 0; i < contentTip.childCount; i++)
            {
                lsItemVowAnim.Add(contentTip.GetChild(i));
            }
            //打开许愿弹窗
            _VowTip.gameObject.SetActive(true);

        }, (error) => { });
    }

    /// <summary>
    /// 初始化今日签到和活动提醒
    /// </summary>
    private void InitTodaySign() 
    {
        bool signStatus = GetTodaySignStatus();
        //判定今日是否已签到
        if (!signStatus)
        {
            //多语言todo
            _todaySignText.text = "今日已许愿";
            for (int i = 0; i < plateList.Count; i++)
            {
                plateList[i].enabled = false;
            }
            _redHint.gameObject.SetActive(false);
        }
        else //未签到
        {
            _todaySignText.text = "选择一张许愿牌";
            for (int i = 0; i < plateList.Count; i++)
            {
                plateList[i].enabled = true;
            }
            _redHint.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 初始化累积奖励栏
    /// </summary>
    private void InitAward()
    {
        //累积签到天数
        _accuDayText.text = StaticData.playerInfoData.userInfo.SignDays.ToString();

        //获取累积奖励配置
        GetConfigData();
    }

    /// <summary>
    /// 获取今日的签到状态
    /// </summary>
    /// <returns></returns>
    private bool GetTodaySignStatus() 
    {
        bool signStatus = false;
        if (StaticData.playerInfoData.userInfo.SignInInfo.Count == 0)
        {
            //今日0点时间戳
            long zeroTime = TimeHelper.ServerTimeStampNow - (TimeHelper.ServerTimeStampNow + 8 * 3600 * 1000) % 86400000;
            var signInRenewalTimeC = StaticData.configExcel.GetVertical().SignInRenewalTime;
            //签到时间对应的小时时间戳
            long timeDelta = signInRenewalTimeC[0] * 60 * 60 * 1000 + signInRenewalTimeC[1] * 60 * 1000 + signInRenewalTimeC[2] * 1000;
            long todaySignTimeStamp = zeroTime + timeDelta;
            //明天0点时间戳
            nextSignTime = todaySignTimeStamp + 24 * 3600 * 1000;
            signStatus = true;
        }
        else 
        {
            long lastSignTimeStamp=0;//上次签到刷新时间
            for (int i = 0; i < StaticData.playerInfoData.userInfo.SignInInfo.Count; i++)
            {
                var data = StaticData.playerInfoData.userInfo.SignInInfo[i];
                if (data.DayNumber == 1)
                {
                    lastSignTimeStamp = data.SignInTime;
                    break;
                }
            }
            //今日0点时间戳
            long zeroTime = TimeHelper.ServerTimeStampNow - (TimeHelper.ServerTimeStampNow + 8 * 3600 * 1000) % 86400000;
            var signInRenewalTimeC = StaticData.configExcel.GetVertical().SignInRenewalTime;
            //签到时间对应的小时时间戳
            long timeDelta = signInRenewalTimeC[0] * 60 * 60 * 1000 + signInRenewalTimeC[1] * 60 * 1000 + signInRenewalTimeC[2] * 1000;
            long todaySignTimeStamp = zeroTime + timeDelta;
            //明天0点时间戳
            nextSignTime = todaySignTimeStamp + 24 * 3600 * 1000;
            //if (lastSignTimeStamp < todaySignTimeStamp && TimeHelper.ServerTimeStampNow > todaySignTimeStamp)
            //{
            //    signStatus = true;
            //}
            if (TimeHelper.ServerTimeStampNow >= lastSignTimeStamp)
            {
                signStatus = true;
            }
        }
        return signStatus;
    }

    /// <summary>
    /// 控制标签页
    /// </summary>
    /// <param name="bo"></param>
    public void TabActivityControll(bool bo)
    {
        if (_tabVow == null) return;

        if (_tabVow.isOn)
        {
            _vow.gameObject.SetActive(true);
            _other.gameObject.SetActive(false);

            _vowRedText.gameObject.SetActive(true);
            _otherRedText.gameObject.SetActive(false);
            
        }
        else if (_tabOther) 
        {
            _vow.gameObject.SetActive(false);
            _other.gameObject.SetActive(true);

            _vowRedText.gameObject.SetActive(false);
            _otherRedText.gameObject.SetActive(true);
        }
        else
        {
            _vow.gameObject.SetActive(true);
            _other.gameObject.SetActive(false);

            _vowRedText.gameObject.SetActive(true);
            _otherRedText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 关闭许愿弹窗
    /// </summary>
    private void OnClickVowTip()
    {
        _VowTip.gameObject.SetActive(false);
    }

    /// <summary>
    /// 获取累积奖励配置
    /// </summary>
    private void GetConfigData()
    {
        listSign.Clear();
        var sign = StaticData.configExcel.SignIn;
        foreach (var item in sign)
        {
            listSign.Add(item);
        }

        //无限列表刷新
        ls.ClearCells();
        ls.totalCount = listSign.Count;
        ls.RefillCells();
    }


    /// <summary>
    /// 获取许愿奖励配置
    /// </summary>
    private void GetVowData()
    {
        LsSCVowAward.Clear();
        LsSCVowAward.Add(sCVowAward);

        lsVow.ClearCells();
        lsVow.totalCount = LsSCVowAward.Count;
        lsVow.RefillCells();
    }
}
