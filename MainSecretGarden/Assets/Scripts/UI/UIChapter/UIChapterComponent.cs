using Game.Protocal;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.IO;
using Cysharp.Threading.Tasks;
using System.Collections;
using Google.Protobuf.Collections;
using Company.Cfg;

/// <summary>
/// 货币类型（解锁番外）
/// </summary>
public enum CurrencyType
{
    Gold,
    Diamond,
    PurpleGold
}

public class UIChapterComponent : MonoBehaviour
{
    public static UIChapterComponent Instance;
    public enum ChapterType
    {
        Main,
        OAD
    }
    ChapterType line;
    int _curVersionsMaxCount;//当前版本最大章节数量
    int _mainMaxCount;//最大主线章节数量（目前总共有多少章节）
    int _OADMaxCount;//最大番外数量（目前总共有多少番外）
    int _passChapterID;//以读章节
    int _unlockMaxChapterID;//最大章节可解锁章节id
    List<string> OADid = new List<string>();//已解锁的番外id
    public RepeatedField<CSAdvStruct> ADList;
    //int level;
    ChapterList m_chapterList;
    //OADList m_OADList;

    /// <summary>
    /// 番外总数量
    /// </summary>
    public List<ExtraStoryDefine> OADinfoList = new List<ExtraStoryDefine>();
    /// <summary>
    /// 显示的番外数量
    /// </summary>
    public List<ExtraStoryDefine> ShowOADinfoListCount = new List<ExtraStoryDefine>();
    public LoopVerticalScrollRect m_OADList;
    /// <summary>
    /// 紫金币购买页面
    /// </summary>
    public BuyPurpleGoldAmountChoice purpleGoldBuyView;

    Button btn_back;
    Button btn_chapter;
    Button btn_OAD;
    Button btn_zjb;
    Text text_zjb;
    Text text_chapterBtn;
    Text text_OADBtn;

    //动画
    CanvasGroup btn_chapter_canvasGroup;
    CanvasGroup btn_OAD_canvasGroup;
    CanvasGroup diamondBtn_canvasGroup;
    CanvasGroup ZjbBtn_canvasGroup;//紫金币
    float diamondStartPosY = 100f;//1444f;
    float diamondEndPosY = -136f;//1208f;
    bool isInit = false;
    GameObject Mask;//动画时候遮挡按钮点击

    Text chapterText;
    Text OADText;
    Shadow chapterShadow;
    Shadow OADShadow;
    Outline chapterOutLine;
    Outline OADOutLine;

    Sprite selectBtn_sprite;//章节番外切换资源替换
    Sprite noSelectBtn_sprite;
    Color selectText_Color = new Color(231f / 255f, 95f / 255f, 140f / 255f);
    Color noselectText_Color = new Color(235f / 255f, 242f / 255f, 253f / 255f);

    //红点
    Image chapterRedDot;
    Image oadRedDot;
    #region 属性
    public List<string> OADID
    {
        get
        {
            OADid.Clear();
            this.OADid.AddRange(StaticData.playerInfoData.userInfo.ExtraStory.Split(','));
            return this.OADid;
        }
    }
    #endregion

    public bool isTest;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        if (!isInit)
        {
            FindComponent();
            RegisteredEvents();
            isInit = true;
            //Debug.LogError("章节初始化");
            //Debug.LogError(Instance.GetHashCode());
        }
    }

    public void OpenChapterView()
    {
        line = ChapterType.Main;
        //字段赋值
        GetUserInfo();
        if (_mainMaxCount >= _curVersionsMaxCount)
        {
            m_chapterList.Init(_curVersionsMaxCount + 1, _passChapterID, _unlockMaxChapterID);
        }
        //红点
        ShowChapterBtnRedDot();
        ShowOADBtnRedDot();
        //进入章节动画
        CutInUIChapter();
        StartCoroutine(CutIn(1));

        //Debug.Log(TimeHelper.loginServerDateTime);//登陆时的服务器时间  用户登陆时的时间
        //Debug.Log(TimeHelper.ServerDateTimeNow);//服务器当前的时间  实时更新

        ////Debug.Log(TimeHelper.ServerTime(TimeHelper.Now()));//服务器当前的时间戳
        //var time = TimeHelper.ServerTime(TimeHelper.LoginServerTime);//登陆时的服务器时间戳

        //Debug.Log($"服务器当前的时间戳{TimeHelper.Now()}");
        //Debug.Log($"登陆时的服务器时间戳{TimeHelper.LoginServerTime}");
        //var time111 = TimeHelper.GetDateTime(0, 6, 0, 0);
        //Debug.Log($"6dian cai neng {time111}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.LogError("读完0章");
            m_chapterList.PassChpter(1000);
        }
        if (Input.GetKeyDown(KeyCode.F9))
        {
            Debug.LogError("读完第1章");
            m_chapterList.PassChpter(1001);
        }
        if (Input.GetKeyDown(KeyCode.F10))
        {
            Debug.LogError("读完第2章");
            m_chapterList.PassChpter(1002);
        }
        if (Input.GetKeyDown(KeyCode.F11))
        {
            Debug.LogError("读完第3章");
            m_chapterList.PassChpter(1003);
        }
    }

    void GetUserInfo()
    {
        //获取玩家等级
        //level = StaticData.GetPlayerLevelAndCurrExp().level;
        _curVersionsMaxCount = StaticData.configExcel.GetVertical().CurVersionsChapterMaxCount;
        this._mainMaxCount = StaticData.configExcel.Section.Count;//做一章敬请期待
        this._OADMaxCount = StaticData.configExcel.ExtraStory.Count;
        this._passChapterID = StaticData.playerInfoData.userInfo.SectionId;
        this._unlockMaxChapterID = StaticData.playerInfoData.userInfo.UnlockSectionId;
        this.OADid.AddRange(StaticData.playerInfoData.userInfo.ExtraStory.Split(','));
        this.ADList = StaticData.playerInfoData.userInfo.AdvInfo;

        //localChapterID = PlayerPrefs.GetString("LocalChapterID");
        //localOADID = PlayerPrefs.GetString("LocalOADID");

        ShowOADinfoListCount.Clear();
        OADinfoList.Clear();
        foreach (var item in StaticData.configExcel.ExtraStory)
        {
            if (item.ExtraStoryId > StaticData.configExcel.GetVertical().CurVersionsOADMaxCount)
                break;
#if UNITY_EDITOR
            if (isTest)
            {
                if (!ShowOADinfoListCount.Contains(item))
                    ShowOADinfoListCount.Add(item);
            }
            else
            {
                if (item.PassChapterID <= _passChapterID)
                {
                    if (!ShowOADinfoListCount.Contains(item))
                        ShowOADinfoListCount.Add(item);
                }
            }
            if (!OADinfoList.Contains(item))
                OADinfoList.Add(item);
#else
            if (item.PassChapterID <= _passChapterID)
                {
                    if (!ShowOADinfoListCount.Contains(item))
                        ShowOADinfoListCount.Add(item);
                }
            if (!OADinfoList.Contains(item))
                OADinfoList.Add(item);
#endif
        }
        ShowOADinfoListCount.Reverse();
        ChangeList(ShowOADinfoListCount.Count);
        //前端等级解锁
        int level = StaticData.GetPlayerLevelByExp();
        int maxChapterID = 0;
        foreach (var item in StaticData.configExcel.Section)
        {
            if (item.SectionGrade <= level)
            {
                maxChapterID = item.SectionId;
                continue;
            }
            else
                break;
        }
        if (this._unlockMaxChapterID <= maxChapterID)
        {
            StaticData.playerInfoData.userInfo.UnlockSectionId = maxChapterID;
            this._unlockMaxChapterID = maxChapterID;
        }

        if (StaticData.playerInfoData.favorableData.Count <= 0)
        {
            ImpulseHelper.CacheFavorable();
        }
        else
            Debug.Log("本地已缓存好感数据");
    }

    void FindComponent()
    {
        m_chapterList = transform.Find("ChapterList").GetComponent<ChapterList>();
        m_OADList = transform.Find("OADList").GetComponent<LoopVerticalScrollRect>();

        btn_back = transform.Find("BackBtn").GetComponent<Button>();
        btn_chapter = transform.Find("ChapterBtn").GetComponent<Button>();
        btn_OAD = transform.Find("OADBtn").GetComponent<Button>();
        text_chapterBtn = transform.Find("ChapterBtn/Title").GetComponent<Text>();
        text_OADBtn = transform.Find("OADBtn/Title").GetComponent<Text>();
        //动画
        btn_chapter_canvasGroup = btn_chapter.GetComponent<CanvasGroup>();
        btn_OAD_canvasGroup = btn_OAD.GetComponent<CanvasGroup>();
        diamondBtn_canvasGroup = transform.Find("UIDiamondNav").GetComponent<CanvasGroup>();
        ZjbBtn_canvasGroup = transform.Find("UIZjbNav").GetComponent<CanvasGroup>();
        btn_zjb = ZjbBtn_canvasGroup.GetComponent<Button>();//点击打开快捷购买
        text_zjb = ZjbBtn_canvasGroup.transform.Find("Num").GetComponent<Text>();
        Mask = transform.Find("Mask").gameObject;

        chapterText = btn_chapter.transform.Find("Title").GetComponent<Text>();
        OADText = btn_OAD.transform.Find("Title").GetComponent<Text>();
        chapterShadow = chapterText.GetComponent<Shadow>();
        OADShadow = OADText.GetComponent<Shadow>();
        chapterOutLine = chapterText.GetComponent<Outline>();
        OADOutLine = OADText.GetComponent<Outline>();

        chapterRedDot = btn_chapter.transform.Find("redDot").GetComponent<Image>();
        oadRedDot = btn_OAD.transform.Find("redDot").GetComponent<Image>();


        //获取资源
        selectBtn_sprite = ABManager.GetAsset<Sprite>("ty_qp_tabbtn_1");
        noSelectBtn_sprite = ABManager.GetAsset<Sprite>("ty_qp_tabbtn_2");

    }

    void RegisteredEvents()
    {
        btn_back.onClick.RemoveAllListeners();
        btn_back.onClick.AddListener(async () =>
        {
            ExitUIChapter();
            await UniTask.Delay(500);
            UIComponent.RemoveUI(UIType.UIChapter);
        });

        btn_chapter.onClick.RemoveAllListeners();
        btn_chapter.onClick.AddListener(() => OnClickOADBtn(ChapterType.Main));
        btn_OAD.onClick.RemoveAllListeners();
        btn_OAD.onClick.AddListener(() => OnClickOADBtn(ChapterType.OAD));
    }
    //章节番外切换按钮
    void OnClickOADBtn(ChapterType chapterType)
    {
        btn_chapter.enabled = false;
        btn_OAD.enabled = false;
        GetUserInfo();//刷新数据
        if (chapterType == ChapterType.Main)
        {
            if (line == ChapterType.Main)
            {
                btn_chapter.enabled = true;
                btn_OAD.enabled = true;
                return;
            }
            line = ChapterType.Main;

            btn_chapter.image.sprite = selectBtn_sprite;
            btn_OAD.image.sprite = noSelectBtn_sprite;
            chapterText.color = selectText_Color;
            OADText.color = noselectText_Color;
            chapterShadow.effectColor = new Color(1f, 1f, 1f);
            chapterShadow.effectDistance = new Vector2(1, -2);
            OADShadow.effectColor = new Color(63f / 255f, 119f / 255f, 188f / 255f);
            OADShadow.effectDistance = new Vector2(2, 1);
            chapterShadow.enabled = true;
            chapterOutLine.enabled = false;
            OADShadow.enabled = false;
            OADOutLine.enabled = true;

            SwitchAnima(line);
            //动画后再清除
            //m_chapterList.Clear();
            //m_chapterList.Init(_mainMaxCount, _passChapterID, _unlockMaxChapterID);
        }
        else
        {//番外开启
            if (!StaticData.IsOpenFunction(10021))
            {
                btn_chapter.enabled = true;
                btn_OAD.enabled = true;
                return;
            }
            if (line == ChapterType.OAD)
            {
                btn_chapter.enabled = true;
                btn_OAD.enabled = true;
                return;
            }
            line = ChapterType.OAD;

            btn_chapter.image.sprite = noSelectBtn_sprite;
            btn_OAD.image.sprite = selectBtn_sprite;
            chapterText.color = noselectText_Color;
            OADText.color = selectText_Color;
            OADShadow.effectColor = new Color(1f, 1f, 1f);
            OADShadow.effectDistance = new Vector2(1, -2);
            chapterShadow.effectColor = new Color(63f / 255f, 119f / 255f, 188f / 255f);
            chapterShadow.effectDistance = new Vector2(2, 1);
            chapterShadow.enabled = false;
            chapterOutLine.enabled = true;
            OADShadow.enabled = true;
            OADOutLine.enabled = false;

            SwitchAnima(line);
            ChapterHelper.SetActive(m_OADList.gameObject, true);
            //m_OADList.Clear();
            //m_OADList.Init(_OADMaxCount, OADid);
        }

        //用户信息
        //StaticData.playerInfoData.userInfo
        //Debug.Log("点击了切换章节按钮");
        ////通关请求
        //CSClearance cSClearance = new CSClearance() { SectionId = 1001 };
        //ProtocalManager.Instance().SendCSClearance(cSClearance, (SCEmptyClearance cs) => {}, (ErrorInfo e) => { Debug.Log("章节请求失败"); });
    }

    public async void SwitchAnima(ChapterType chapterType)
    {
        Mask.gameObject.SetActive(true);

        switch (chapterType)
        {
            case ChapterType.Main:
                //全部移动到左边
                ChapterHelper.SetActive(m_chapterList.gameObject, true);
                m_chapterList.Init(_curVersionsMaxCount + 1, _passChapterID, _unlockMaxChapterID);

                m_OADList.transform.DOLocalMoveX(1242, 0.3f);
                ZjbBtn_canvasGroup.DOFade(0, switchBtnPosDutation);
                (ZjbBtn_canvasGroup.transform as RectTransform).DOAnchorPosY(diamondStartPosY, switchBtnPosDutation);
                diamondBtn_canvasGroup.DOFade(1, switchBtnPosDutation);
                (diamondBtn_canvasGroup.transform as RectTransform).DOAnchorPosY(diamondEndPosY, switchBtnPosDutation);

                m_chapterList.transform.DOLocalMoveX(0, 0.3f).OnComplete(() =>
                {
                    btn_chapter.enabled = true;
                    btn_OAD.enabled = true;
                });

                StartCoroutine(CutIn(2));
                await UniTask.Delay(300);
                break;
            case ChapterType.OAD:

                ChapterHelper.SetActive(m_OADList.gameObject, true);
                m_chapterList.transform.DOLocalMoveX(-1242, 0.3f);
                ZjbBtn_canvasGroup.DOFade(1, switchBtnPosDutation);
                (ZjbBtn_canvasGroup.transform as RectTransform).DOAnchorPosY(diamondEndPosY, switchBtnPosDutation);
                diamondBtn_canvasGroup.DOFade(0, switchBtnPosDutation);
                (diamondBtn_canvasGroup.transform as RectTransform).DOAnchorPosY(diamondStartPosY, switchBtnPosDutation);

                m_OADList.transform.DOLocalMoveX(0, 0.3f).OnComplete(() =>
                {
                    btn_chapter.enabled = true;
                    btn_OAD.enabled = true;
                    Mask.gameObject.SetActive(false);
                });
                await UniTask.Delay(300);
                m_chapterList.Clear();
                break;
        }
    }
    float CutInDutation = 0.8f;//大厅进入动画时间
    float ExitDutation = 0.3f;//退出动画时间
    float switchBtnPosDutation = 0.5f;//内部切换动画时间
    //章节进入动画（从大厅）
    void CutInUIChapter()
    {//开启遮罩
        Mask.gameObject.SetActive(true);
        btn_chapter_canvasGroup.DOFade(1, CutInDutation);
        btn_OAD_canvasGroup.DOFade(1, CutInDutation);
        (diamondBtn_canvasGroup.transform as RectTransform).DOAnchorPosY(diamondEndPosY, CutInDutation);
        diamondBtn_canvasGroup.DOFade(1, CutInDutation).OnComplete(() =>
        {//关闭遮罩
            Mask.gameObject.SetActive(false);
        });
    }
    //退出章节页面动画
    void ExitUIChapter()
    {
        btn_chapter_canvasGroup.DOFade(0, ExitDutation);
        btn_OAD_canvasGroup.DOFade(0, ExitDutation);
        switch (line)
        {
            case ChapterType.Main://章节退出动画
                (diamondBtn_canvasGroup.transform as RectTransform).DOAnchorPosY(diamondStartPosY, ExitDutation);
                diamondBtn_canvasGroup.DOFade(0, ExitDutation);
                StartCoroutine(CutIn(3));
                break;
            case ChapterType.OAD://番外退出动画
                (ZjbBtn_canvasGroup.transform as RectTransform).DOAnchorPosY(diamondStartPosY, ExitDutation);
                ZjbBtn_canvasGroup.DOFade(0, ExitDutation);
                m_OADList.transform.DOLocalMoveX(-1242, ExitDutation);
                break;
        }

    }

    /// <summary>
    /// 章节进入动画(从番外切换)
    /// </summary>
    /// <param name="hallOrOAD">1:hall 2:OAD</param>
    /// <returns></returns>
    IEnumerator CutIn(int hallOrOAD)
    {
        for (int i = 0; i < m_chapterList.chapterItems.Count; i++)
        {//准
            m_chapterList.chapterItems[i].PlayCutInReady(hallOrOAD);
        }
        int time = 0;
        while (time < m_chapterList.chapterItems.Count)
        {
            m_chapterList.chapterItems[time].PlayCutInAnima(hallOrOAD);
            time++;
            yield return new WaitForSeconds(0.08f);
        }
        //关闭遮罩
        Mask.gameObject.SetActive(false);
    }


    #region 番外
    /// <summary>
    /// 打开番外购买弹窗
    /// </summary>
    /// <param name="OADdata"></param>
    public async void OpenBuyOAD(ExtraStoryDefine OADdata, int Index, OADItem currItem, Action BuySucceedCallBack = null)
    {//购买番外逻辑
     //点击解锁直接判断 
     //1、货币足够 → 直接扣
     //2、不够 → 询问购买紫金币纯文本弹窗 → 点击购买 弹出增减页面 → 钻石不够 → 弹出提示钻石充值纯文本弹窗 → 确认购买 钻石充值界面

        CurrencyType currencyType = CurrencyType.PurpleGold;//默认为紫金币类型
        CSWareHouseStruct item = new CSWareHouseStruct();
        if (OADdata.Price[0].ID == StaticData.configExcel.GetVertical().PurpleGoldsId)
        {//紫金币
            currencyType = CurrencyType.PurpleGold;
            item = StaticData.GetWareHouseItem(StaticData.configExcel.GetVertical().PurpleGoldsId);
        }
        else if (OADdata.Price[0].ID == StaticData.configExcel.GetVertical().JewelGoodsId)
        {//钻石
            currencyType = CurrencyType.Diamond;
            item = StaticData.GetWareHouseItem(StaticData.configExcel.GetVertical().JewelGoodsId);
        }
        else if (OADdata.Price[0].ID == StaticData.configExcel.GetVertical().GoldGoodsId)
        {//金币
            currencyType = CurrencyType.Gold;
            item = StaticData.GetWareHouseItem(StaticData.configExcel.GetVertical().GoldGoodsId);
        }

        if (item.GoodNum >= OADdata.Price[0].Count)
        {//货币足够
         //请求解锁
            CSBuyExtraStory cSBuyExtraStory = new CSBuyExtraStory() { ExtraStoryId = OADdata.ExtraStoryId };
            ProtocalManager.Instance().SendCSBuyExtraStory(cSBuyExtraStory, (SCBuyExtraStory sCBuyExtraStory) =>
            {
                if (currencyType == CurrencyType.PurpleGold)
                {
                    StaticData.UpdateWareHouseItems(StaticData.configExcel.GetVertical().PurpleGoldsId, (int)(StaticData.GetWareHouseItem(StaticData.configExcel.GetVertical().PurpleGoldsId).GoodNum - OADdata.Price[0].Count));
                }
                StaticData.CreateToastTips(StaticData.GetMultilingual(120234));//章节购买成功
                ChapterHelper.UnlockOAD(OADdata.ExtraStoryId);
                foreach (var goodsInfo in sCBuyExtraStory.CurrencyInfo)
                {
                    StaticData.UpdateWareHouseItems(goodsInfo.GoodsId, (int)goodsInfo.Count);
                }
                BuySucceedCallBack?.Invoke();
            }, (ErrorInfo e) =>
            {
                Debug.LogError("番外解锁请求失败");
            });
        }
        else
        {//货币不足
            Debug.Log("货币不足");
            switch (currencyType)
            {
                case CurrencyType.Gold:
                    UnlockOAD_Gold();
                    break;
                case CurrencyType.Diamond:
                    UnlockOAD_Diamond();
                    break;
                case CurrencyType.PurpleGold:
                    UnlockOAD_PurpleGold();
                    break;
            }
        }
    }
    /// <summary>
    /// 解锁番外_金币
    /// </summary>
    void UnlockOAD_Gold()
    {
        string tipStr = StaticData.GetMultilingual(120245);//金币不足，需要购买吗
        StaticData.OpenCommonTips(tipStr, 120010, async () =>
        {
            await StaticData.OpenRechargeUI(0);
        }, null, 120075);


    }
    /// <summary>
    /// 解锁番外_钻石
    /// </summary>
    void UnlockOAD_Diamond()
    {
        string tipStr = StaticData.GetMultilingual(120243);//钻石不足，需要购买吗
        StaticData.OpenCommonTips(tipStr, 120010, async () =>
        {
            await StaticData.OpenRechargeUI(1);
        }, null, 120075);
    }
    /// <summary>
    /// 解锁番外_紫金币
    /// </summary>
    void UnlockOAD_PurpleGold()
    {//弹出紫金币快捷购买界面
        string tip1 = StaticData.GetMultilingual(120291);//紫金币不足，需要购买吗
        StaticData.OpenCommonTips(tip1, 120010, async () =>
        {//确认购买打开紫星币数量选择界面
            purpleGoldBuyView.ShowBuyView(1, null, null, () =>
            {//购买失败回调
                string tip2 = StaticData.GetMultilingual(120243);//钻石不足，需要购买吗
                StaticData.OpenCommonTips(tip2, 120010, async () =>
                {
                    await StaticData.OpenRechargeUI(1);
                }, null, 120075);
            });
        }, null, 120075);
    }

    /// <summary>
    /// 打开番外
    /// </summary>
    /// <param name="Index"></param>
    public void OpenOAD(ExtraStoryDefine OADinfo)
    {
        ChapterHelper.CreateOAD(OADinfo);
    }

    /// <summary>
    /// 获取当前item展示数据
    /// </summary>
    /// <param name="listIndex"></param>
    /// <returns></returns>
    public ExtraStoryDefine ItemDataShow(int listIndex)
    {
        if (listIndex < ShowOADinfoListCount.Count)
        {
            return ShowOADinfoListCount[listIndex];
        }
        return null;
    }
    /// <summary>
    /// 更改列表
    /// </summary>
    /// <param name="coutn"></param>
    public void ChangeList(int count)
    {
        m_OADList.ClearCells();
        m_OADList.totalCount = count;
        m_OADList.RefillCells();
    }

    #endregion

    #region 红点
    /// <summary>
    /// 显示章节按钮红点
    /// </summary>
    public void ShowChapterBtnRedDot()
    {
        bool isShow = false;
        foreach (var item in StaticData.configExcel.Section)
        {
            if (item.SectionId <= StaticData.playerInfoData.userInfo.UnlockSectionId)
            {
                if (!ChapterHelper.localChapterIDList.Contains(item.SectionId))
                {
                    isShow = true;
                }
            }
        }
        chapterRedDot.gameObject.SetActive(isShow);
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Chapter);
    }
    /// <summary>
    /// 显示番外按钮的红点
    /// </summary>
    public void ShowOADBtnRedDot()
    {
        bool isShow = false;
        foreach (var item in StaticData.configExcel.ExtraStory)
        {
            if (item.ExtraStoryId > StaticData.configExcel.GetVertical().CurVersionsOADMaxCount) break;
            if (item.PassChapterID > StaticData.playerInfoData.userInfo.SectionId)
                continue;
            if (!ChapterHelper.localOADList.Contains(item.ExtraStoryId))
            {
                isShow = true;
            }
        }
        oadRedDot.gameObject.SetActive(isShow);
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Chapter);
    }

    #endregion
}






/// <summary>
/// 章节助手类
/// </summary>
public class ChapterHelper
{
    public static string localChapterID = "";//本地存的已解锁章节
    public static string localOADID = "";//本地存的已解锁番外
    //红点
    public static List<int> localChapterIDList = new List<int>();
    public static List<int> localOADList = new List<int>();

    /// <summary>
    /// 当前进入的章节ID
    /// </summary>
    public static int curChapterID;

    /// <summary>
    /// 大厅章节icon显示红点
    /// </summary>
    /// <returns></returns>
    public static bool ChpaterUpdateRedDot()
    {
        bool isShow = false;
        localChapterID = PlayerPrefs.GetString("LocalChapterID");
        //Debug.LogError(localChapterID);
        localOADID = PlayerPrefs.GetString("LocalOADID");
        //Debug.LogError(localOADID);
        var chapterArray = localChapterID.Split(',');
        var oadArray = localOADID.Split(',');
        foreach (var item in chapterArray)
        {
            int id;
            if (int.TryParse(item, out id))
            {
                if (!localChapterIDList.Contains(id))
                    localChapterIDList.Add(int.Parse(item));
            }
        }
        foreach (var item in oadArray)
        {
            int id;
            if (int.TryParse(item, out id))
            {
                if (!localOADList.Contains(id))
                    localOADList.Add(int.Parse(item));
            }
        }

        foreach (var item in StaticData.configExcel.Section)
        {
            if (item.SectionId > StaticData.playerInfoData.userInfo.UnlockSectionId) break;
            if (localChapterIDList.Contains(item.SectionId)) continue;
            isShow = true;
            return isShow;//任意一个没包含都显示红点
        }

        foreach (var item in StaticData.configExcel.ExtraStory)
        {
            if (item.ExtraStoryId > StaticData.configExcel.GetVertical().CurVersionsOADMaxCount) break;
            if (item.PassChapterID > StaticData.playerInfoData.userInfo.SectionId) continue;
            if (localOADList.Contains(item.ExtraStoryId)) continue;
            isShow = true;
            return isShow;
        }

        return isShow;
    }



    /// <summary>
    /// 消除红点(章节番外点击)
    /// </summary>
    /// <param name="isChapterOAD">章节还是番外 T章节 F番外</param>
    public static void ReduceRedDot(int id, bool isChapterOAD)
    {
        if (isChapterOAD)
        {
            if (!localChapterIDList.Contains(id))
            {
                if (localChapterID == "")
                {
                    localChapterID += $"{id.ToString()}";
                }
                else
                {
                    localChapterID += $",{id.ToString()}";
                }
                PlayerPrefs.SetString("LocalChapterID", localChapterID);
                //Debug.LogError($"保存本地LocalChapterID，值为{localChapterID}");
                localChapterIDList.Add(id);
            }
            if (UIChapterComponent.Instance != null)
            {
                UIChapterComponent.Instance.ShowChapterBtnRedDot();
            }
        }
        else
        {
            if (!localOADList.Contains(id))
            {
                if (localOADID == "")
                {
                    localOADID += $"{id.ToString()}";
                }
                else
                {
                    localOADID += $",{id.ToString()}";
                }
                PlayerPrefs.SetString("LocalOADID", localOADID);
                //Debug.LogError($"保存本地LocalOADID，值为{localOADID}");
                localOADList.Add(id);
            }
            if (UIChapterComponent.Instance != null)
            {
                UIChapterComponent.Instance.ShowOADBtnRedDot();
            }
        }
    }

    /// <summary>
    /// 序章是否完成
    /// </summary>
    /// <returns></returns>
    public static bool IsFinishSection()
    {
        bool isfinished = false;
        if (StaticData.playerInfoData.userInfo.SectionId >= StaticData.configExcel.Section[0].SectionId)
            isfinished = true;
        return isfinished;
    }
    /// <summary>
    /// 新用户进入序章
    /// </summary>
    public static async UniTask NewUserJoinChapter()
    {
        //StaticData.LobbyToManor();
        if (StaticData.playerInfoData.userInfo.SectionId < 997)
        {//第一次进入的新用户
            int id = StaticData.configExcel.Section[0].SectionId;
            await EnterIntoChapter(id);
        }
    }
    /// <summary>
    /// 下一章是否解锁
    /// </summary>
    public static bool NextChapterIsUnlock(int chapterID)
    {
        bool nextisUnlock = false;
        if (StaticData.playerInfoData.userInfo.UnlockSectionId >= chapterID + 1)
        {
            nextisUnlock = true;
        }
        return nextisUnlock;
    }

    /// <summary>
    /// 上一章是否读完
    /// </summary>
    /// <returns></returns>
    public static bool BeforeChapterIsReadOver(int chapterID)
    {
        bool beforeIsReadOver = false;
        if (StaticData.playerInfoData.userInfo.SectionId >= chapterID - 1)
        {
            beforeIsReadOver = true;
        }
        return beforeIsReadOver;
    }


    /// <summary>
    /// 章节里进下一章
    /// </summary>
    /// <param name="chapterID"></param>
    public static void JoinNextChapter(int chapterID, Action callback)
    {
        //如果下一章已经解锁 直接进入
        if (NextChapterIsUnlock(chapterID))
        {
            //callback?.Invoke();
            EnterIntoChapter(chapterID + 1);
        }
        else
            PopupBuyChapterView(chapterID + 1, callback);

    }
    /// <summary>
    /// 弹出购买章节界面
    /// </summary>
    public async static void PopupBuyChapterView(int chapterID, Action cancelCallBack)
    {//如果还没出下一章TODO
        var chapterInfo = StaticData.configExcel.GetSectionBySectionId(chapterID);
        if (chapterInfo == null)
        {
            StaticData.ToManorSelf();
            cancelCallBack?.Invoke();
            return;
        }
        int id = chapterInfo.UnlockPrice[0].ID;//取到钻石图片的id
        int count = (int)chapterInfo.UnlockPrice[0].Count;//取到数量
        Sprite sprite = await ZillionaireToolManager.LoadItemSprite(id);
        string str = $"你的等级不够解锁下一章了哦";
        StaticData.OpenCommonBuyTips(str, sprite, count, () =>
        {
            if (StaticData.GetWareHouseDiamond() >= count)
            {
                //扣除资源
                //刷新章节
                StaticData.UpdateWareHouseDiamond(-count);
                CSBuySection cSBuySection = new CSBuySection() { SectionId = chapterID };
                ProtocalManager.Instance().SendCSBuySection(cSBuySection, (SCBuySection x) =>
                {
                    StaticData.CreateToastTips("章节购买成功");
                    ChapterHelper.UnlockChapter(chapterID);//存入前端缓存
                    foreach (var goodsInfo in x.CurrencyInfo)
                    {
                        StaticData.UpdateWareHouseItems(goodsInfo.GoodsId, (int)goodsInfo.Count);
                    }
                    //进入下一章
                    EnterIntoChapter(chapterID);
                }, (ErrorInfo e) =>
                {
                    StaticData.CreateToastTips("章节购买失败");
                    Debug.LogError("章节购买失败" + e.webErrorCode);
                }, false);
            }
            else
            {
                StaticData.OpenRechargeUI();
            }
        },
        () => //取消购买直接进庄园
        {
            cancelCallBack?.Invoke();
            StaticData.ToManorSelf();
        }, 120212);

    }
    /// <summary>
    /// 进入章节（生成对象章节ID的章节预制体）
    /// </summary>
    /// <param name="chapterID">章节ID</param>
    /// <param name="IsfromManor">是否从庄园进入</param>
    public async static UniTask EnterIntoChapter(int chapterID, bool IsfromManor = false)
    {//如果进入的不是序章
     //进入新的章节预制体
     //根据章节ID获取标题的文字
     //生成标题，标题播放完后生成对应预制章节
        if (IsfromManor)
        {//如果是从庄园进章节
            curChapterID = chapterID;
            bool isReadOver = BeforeChapterIsReadOver(chapterID);
            await StaticData.ToManorSelf();
            if (isReadOver)
            {//如果上一章读完
                await CreateChapter(chapterID, true);
                return;
            }
            else
            {
                await StaticData.OpenChapterUI();
                //提示
                StaticData.CreateToastTips(StaticData.GetMultilingual(120205));//请把上一章节看完
                return;
            }
        }
        await CreateChapter(chapterID);
    }

    /// <summary>
    /// 创建章节
    /// </summary>
    /// <param name="chapterID"></param>
    /// <returns></returns>
    private static async System.Threading.Tasks.Task CreateChapter(int chapterID, bool isManor = false)
    {
        CSEntranceSection cSEntranceSection = new CSEntranceSection() { SectionId = chapterID };
        ProtocalManager.Instance().SendCSEntranceSection(cSEntranceSection, (SCEmptyEntranceSection sCEmptyEntranceSection) =>
          {
              Debug.Log("进入" + chapterID + "章记录成功");
          },
            (ErrorInfo e) =>
            {
                Debug.LogError(e.ErrorMessage);
            });

        if (!isManor)
            await CreateChapterTitle(chapterID);
        //消除红点
        ReduceRedDot(chapterID, true);
        //播放章节背景音乐
        GameSoundPlayer.Instance.PlayBgMusic(MusicHelper.BgMusicChapter);

        ChapterTool.LoadChapterManager(chapterID - StaticData.configExcel.Section[0].SectionId);

        //进入章节后移除章节列表  重章节里回来后再出列表
        UIComponent.RemoveUI(UIType.UIChapter);
    }

    /// <summary>
    /// 创建章节标题
    /// </summary>
    /// <param name="chapterID"></param>
    /// <returns></returns>
    public static async System.Threading.Tasks.Task CreateChapterTitle(int chapterID)
    {
        var title = await UIComponent.CreateUIAsync(UIType.ChapterTitle, true);
        title.transform.SetRectTransformStretchAllWithParent(UIRoot.instance.GetUIRootCanvasTop().transform);
        ChapterTitle chapterTitle = title.GetComponent<ChapterTitle>();
        var section = StaticData.configExcel.GetSectionBySectionId(chapterID);
        string titleStr = ChapterTool.GetChapterFunctionString(section.SectionTitle[0]);
        string titleContent = ChapterTool.GetChapterFunctionString(section.SectionTitle[1]);
        chapterTitle.Play(titleStr, titleContent);
    }

    /// <summary>
    /// 解锁章节(前端缓存存入)
    /// </summary>
    /// <param name="chapterID">章节ID</param>
    public static void UnlockChapter(int chapterID)
    {
        if (StaticData.playerInfoData.userInfo.UnlockSectionId >= chapterID) return;

        StaticData.playerInfoData.userInfo.UnlockSectionId = chapterID;

    }

    /// <summary>
    /// 读完一个章节(前端缓存存入)
    /// </summary>
    /// <param name="passChapterID">读完的章节ID</param>
    public static void PassChapter(int passChapterID)
    {
        if (StaticData.playerInfoData.userInfo.SectionId <= passChapterID)
            StaticData.playerInfoData.userInfo.SectionId = passChapterID;
    }
    /// <summary>
    /// 解锁番外
    /// </summary>
    /// <param name="OADID">番外ID</param>
    public static void UnlockOAD(int OADID)
    {
        if (StaticData.playerInfoData.userInfo.ExtraStory != "")
        {
            StaticData.playerInfoData.userInfo.ExtraStory += $",{OADID.ToString()}";
        }
        else
        {
            StaticData.playerInfoData.userInfo.ExtraStory += $"{OADID.ToString()}";
        }

    }
    /// <summary>
    /// 游戏对象显示隐藏
    /// </summary>
    /// <param name="obj">游戏对象</param>
    /// <param name="isActive">显示或隐藏</param>
    public static void SetActive(GameObject obj, bool isActive, Action callback = null)
    {
        if (obj == null) return;
        obj.SetActive(isActive);
        callback?.Invoke();
    }
    /// <summary>
    /// 延迟指定组件
    /// </summary>
    /// <param name="component">组件</param>
    /// <param name="millisecond">毫秒</param>
    public static async void DelayOpenBtn(Behaviour component, int millisecond = 800)//默认800毫秒
    {
        await Cysharp.Threading.Tasks.UniTask.Delay(millisecond);
        SetEnable(component, true);
    }
    /// <summary>
    /// 延迟显示指定游戏对象
    /// </summary>
    /// <param name="component">游戏对象</param>
    /// <param name="millisecond">毫秒</param>
    public static async void DelaySetActice(GameObject gameObject, int millisecond = 800)//默认800毫秒
    {
        await Cysharp.Threading.Tasks.UniTask.Delay(millisecond);
        Fade(gameObject, 1, 0.8f, 0);
    }
    /// <summary>
    /// 启用组件Enabled
    /// </summary>
    /// <param name="component">组件</param>
    /// <param name="isEnable">开启或者关闭</param>
    public static void SetEnable(Behaviour component, bool isEnabled)
    {
        if (component == null)
            return;
        component.enabled = isEnabled;
    }
    /// <summary>
    /// 设置父物体
    /// </summary>
    /// <param name="obj">设置的游戏对象gameObject</param>
    /// <param name="parent">设置的父物体Transfotm</param>
    public static void SetParent(GameObject obj, Transform parent)
    {
        obj.transform.SetParent(parent);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
        SetActive(obj, true);
    }

    /// <summary>
    /// 淡入淡出CanvasGroup 初始值默认为1
    /// </summary>
    /// <param name="obj">游戏对象</param>
    /// <param name="endValue">目标Alpha值</param>
    /// <param name="duration">持续时间</param>
    /// <param name="startValue">开始Alpha值（默认为1）</param>
    public static void Fade(GameObject obj, float endValue, float duration, float startValue = 1, TweenCallback callback = null)
    {
        if (obj == null) return;
        if (obj.GetComponent<CanvasGroup>() == null)
        {
            CanvasGroup canvasGroup = obj.AddComponent<CanvasGroup>();
            SetActive(obj, true);
            canvasGroup.alpha = startValue;
            canvasGroup.DOFade(endValue, duration).OnComplete(callback);
        }
        else//如果已经出现过一次 就先删除再添加
        {
            GameObject.DestroyImmediate(obj.GetComponent<CanvasGroup>());
            SetActive(obj, false);
            obj.transform.SetAsLastSibling();
            Fade(obj, endValue, duration, startValue, callback);
        }
    }

    /// <summary>
    /// 通过文本表ID返回文本内容
    /// </summary>
    /// <param name="ChapterTextID">文本表ID</param>
    /// <param name="linguisticType">当前选择语言</param>
    /// <returns></returns>
    public static string GetTableDialogue(int ChapterTextID)
    {
        var word = StaticData.configExcel.GetChapterDialogueTextByID(ChapterTextID);
        string str = string.Empty;
        switch (StaticData.linguisticType)
        {
            case Company.Cfg.LinguisticType.Simplified:
                str = word.SimplifiedChinese;
                break;
            case Company.Cfg.LinguisticType.Complex:
                str = word.TraditionalChinese;
                break;
            case Company.Cfg.LinguisticType.English:
                str = word.English;
                break;
        }
        str = string.Format(str, StaticData.playerInfoData.userInfo.Name);
        return str;
    }

    /// <summary>
    /// 自适应全屏幕宽高
    /// </summary>
    public static void AudoSelf(RectTransform rect)
    {
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);
    }

    /// <summary>
    /// 创建番外
    /// </summary>
    /// <param name="chapterID"></param>
    /// <returns></returns>
    public static async void CreateOAD(ExtraStoryDefine OADinfo)
    {

        //播放章节背景音乐
        GameSoundPlayer.Instance.PlayBgMusic(MusicHelper.BgMusicChapter);
        ReduceRedDot(OADinfo.ExtraStoryId, false);
        await LoadOAD(OADinfo.PrefabName);
    }
    /// <summary>
    /// 加载番外预制
    /// </summary>
    /// <param name="index"></param>
    public static async System.Threading.Tasks.Task LoadOAD(string prefabName)
    {
        //播放章节背景音乐
        GameSoundPlayer.Instance.PlayBgMusic(MusicHelper.BgMusicChapter);
        //OADID = OADID - StaticData.configExcel.ExtraStory[0].ExtraStoryId + 1;
        if (OADBase._Instance != null)
        {
            var curOAD = StaticData.configExcel.GetExtraStoryByExtraStoryId(OADBase._Instance._OADIndex);
            UIComponent.RemoveUI(curOAD.PrefabName);
        }
        GameObject obj = await UIComponent.CreateUIAsync(prefabName);
        OADBase._Instance = obj.GetComponent<OADBase>();
        OADBase._Instance.Initial();
    }
    /// <summary>
    /// 用&换行读章节功能文本表
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string ReadChapterFuncTable(int id)
    {
        ChapterFunctionTextDefine data = StaticData.configExcel.GetChapterFunctionTextByID(id);
        string str = "";
        switch (StaticData.linguisticType)
        {
            case LinguisticType.Simplified:
                str = data.SimplifiedChinese;
                break;
            case LinguisticType.Complex:
                str = data.TraditionalChinese;
                break;
            case LinguisticType.English:
                str = data.English;
                break;
            default:
                str = data.SimplifiedChinese;
                break;
        }
        str = string.Format(str, StaticData.playerInfoData.userInfo.Name);
        string strTwo = SetLineFeed(str);
        return strTwo;
    }
    /// <summary>
    /// 设置换行符&符号
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string SetLineFeed(string str)
    {
        char[] charArray = str.ToCharArray();
        string strTwo = "";
        for (int i = 0; i < charArray.Length; i++)
        {
            char ca = charArray[i];
            if (ca == '&')
            {
                strTwo = strTwo + "\n";
            }
            else
            {
                strTwo = strTwo + ca;
            }
        }
        strTwo = string.Format(strTwo, StaticData.playerInfoData.userInfo.Name);
        return strTwo;
    }
    /// <summary>
    /// 获取番外文字
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static OADDialogueTextDefine GetOADData(int id)
    {
        OADDialogueTextDefine data = StaticData.configExcel.GetOADDialogueTextByID(id);
        return data;
    }
    /// <summary>
    /// 获取番外对话文本内容
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string GetOADDialogueString(OADDialogueTextDefine data)
    {
        string str = "";
        switch (StaticData.linguisticType)
        {
            case LinguisticType.Simplified:
                str = data.SimplifiedChinese;
                break;
            case LinguisticType.Complex:
                str = data.TraditionalChinese;
                break;
            case LinguisticType.English:
                str = data.English;
                break;
            default:
                str = data.SimplifiedChinese;
                break;
        }
        str = string.Format(str, StaticData.playerInfoData.userInfo.Name);
        string strTwo = ChapterTool.SetLineFeed(str, data.StringNumber);
        return strTwo;
    }
}
