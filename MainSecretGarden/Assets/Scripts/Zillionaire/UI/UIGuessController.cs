using Company.Cfg;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 大富翁 第二种抽奖界面控制 选择 3选1
/// </summary>
public class UIGuessController : MonoBehaviour
{
    public class AnimElement
    {
        public Transform _itemPos;
        public Image _itemCup;
        public Image _itemIcon;
        public Text _itemNum;
    }

    #region 变量

    private Image _bg;

    private Button _butClose;
    private Transform _time;

    private Button _butAD;

    private List<AnimElement> _animElements = new List<AnimElement>();

    private Text _openNum;

    private CanvasGroup canvasGroup;
    private Transform craneMachine;


    /// <summary>
    /// 初始杯子位置
    /// </summary>
    private List<Vector3> initCupPos = new List<Vector3>();

    /// <summary>
    /// 使用过的杯子位置
    /// </summary>
    private List<Vector3> UsedCupPos = new List<Vector3>();

    /// <summary>
    /// 抽奖次数
    /// </summary>
    private int _lotterNum;

    /// <summary>
    /// 格子奖励
    /// </summary>
    private List<GoodIDCount> _gridRewards = new List<GoodIDCount>();

    /// <summary>
    /// 是否全已经抽奖 发出抽奖请求
    /// </summary>
    private bool _isAllLottery = false;


    /// <summary>
    /// 选中的卡片
    /// </summary>
    private List<int> _slectedCards = new List<int>();

    /// <summary>
    /// 是否需要关闭界面
    /// </summary>
    private bool _needCloseUI = false;

    /// <summary>
    /// 开始默认动画是否播放完
    /// </summary>
    private bool _isEndInitAnim = false;

    /// <summary>
    /// 是否点击打开杯子(包括单开和全开)
    /// </summary>
    private bool _isOpenCup = false;

    /// <summary>
    /// 动画进行中
    /// </summary>
    private bool _PlayingAnim = false;

    #endregion

    #region 方法

    private Action CloseCallback;

    //准备出现动画
    private void Awake()
    {
        canvasGroup = transform.Find("BG").GetComponent<CanvasGroup>();
        craneMachine = transform.Find("BG");
        UniversalTool.ReadyUIAnimTwo(canvasGroup, craneMachine);
    }
    //播放出现动画
    private void OnEnable()
    {
        UniversalTool.StartUIAnimTwo(canvasGroup, craneMachine);
    }
    //还原出现动画
    private void OnDisable()
    {
        UniversalTool.ReadyUIAnimTwo(canvasGroup, craneMachine);
    }


    private void Init()
    {
        if (_butClose != null)
            return;
        _bg = transform.Find("BG").GetComponent<Image>();//
        _bg.fillAmount = 0.658f;
        _butClose = transform.Find("BG/ButClose").GetComponent<Button>();//
        _butClose.transform.localScale = Vector3.zero;
        _butClose.gameObject.SetActive(false);


        _butAD = transform.Find("BG/ButAD").GetComponent<Button>();//
        _butAD.transform.localScale = Vector3.zero;

        _openNum = transform.Find("Time/Num").GetComponent<Text>();//

        _time = transform.Find("Time");
        _time.localScale = Vector3.zero;


        Button _butItem;
        string path = string.Empty;
        for (int i = 0; i < 3; i++)
        {
            AnimElement animElement = new AnimElement();
            path = "BG/Item";
            if (i > 0)
                path = path + i;
            Debug.Log("Init path = " + path);
            animElement._itemPos = transform.Find(path);
            animElement._itemIcon = animElement._itemPos.Find("Icon").GetComponent<Image>();
            animElement._itemCup = animElement._itemPos.Find("Cup").GetComponent<Image>();
            animElement._itemNum = animElement._itemPos.Find("Icon/num").GetComponent<Text>();
            _animElements.Add(animElement);

            _butItem = animElement._itemPos.Find("Cup").GetComponent<Button>();
            _butItem.onClick.RemoveAllListeners();
            switch (i)
            {
                case 0:
                    _butItem.onClick.AddListener(OnClickCard0);
                    break;
                case 1:
                    _butItem.onClick.AddListener(OnClickCard1);
                    break;
                case 2:
                    _butItem.onClick.AddListener(OnClickCard2);
                    break;
            }

        }

        _butClose.onClick.RemoveAllListeners();
        _butClose.onClick.AddListener(OnClickCancel);
        _butAD.onClick.RemoveAllListeners();
        _butAD.onClick.AddListener(OnClickAD);


        for (int i = 0; i < _animElements.Count; i++)
        {
            initCupPos.Add(_animElements[i]._itemPos.localPosition);
        }
    }

    /// <summary>
    /// 界面初始化数据
    /// </summary>
    /// <param name="lotterNum"></param>
    /// <param name="closeCallback"></param>
    public async void InitValue(int lotterNum, Action closeCallback)
    {
        CloseCallback = closeCallback;
        Init();

        GetRewards();
        _needCloseUI = false;
        _isAllLottery = false;
        _lotterNum = lotterNum;
        _slectedCards.Clear();
        _PlayingAnim = false;

        UpdateLotterNumShow();

        _PlayingAnim = true;
        await UniTask.Delay(TimeSpan.FromSeconds(0.8f));
        EnterAnim1();
    }

    /// <summary>
    /// 动画
    /// </summary>
    private async void EnterAnim1()
    {
        //先开杯子关杯子
        for (int i = 0; i < _animElements.Count; i++)
        {
            var cup = _animElements[i]._itemCup;
            var icon = _animElements[i]._itemIcon;

            CupAnim(cup,icon);
        }


        await UniTask.Delay(TimeSpan.FromSeconds(2));

       
        //打乱杯子
        for(int i = 0; i < _animElements.Count; i++)
        {
            var trans = _animElements[i]._itemPos;

            SortCupAnim(trans);
        }

        //默认动画播放完毕
        await UniTask.Delay(TimeSpan.FromSeconds(1));

        //显示提示语句
        DOTween.To(() => _bg.fillAmount, a => _bg.fillAmount = a, 1.0f, 0.3f).SetEase(Ease.Linear);

        _butAD.transform.DOScale(Vector3.one, 0.3f);
        _time.DOScale(Vector3.one, 0.3f);

        _isEndInitAnim = true;
        _PlayingAnim = false;
    }


    #region 各种动画

    /// <summary>
    /// 开杯子并隐藏
    /// </summary>
    private void OpenCupAnim(Image Cup)
    {
        DOTween.To(() => Cup.transform.localPosition, pos => Cup.transform.localPosition = pos, new Vector3(55, 350, 0), 0.6f).SetEase(Ease.Linear);
        DOTween.To(() => Cup.color, a => Cup.color = a, new Color(255, 255, 255, 0), 0.7f).SetEase(Ease.InQuint);
    }

    /// <summary>
    /// 关杯子并显示
    /// </summary>
    /// <param name="Cup"></param>
    private void CloseCupAnim(Image Cup)
    {
        DOTween.To(() => Cup.transform.localPosition, pos => Cup.transform.localPosition = pos, new Vector3(55, 0, 0), 0.6f).SetEase(Ease.Linear);
        DOTween.To(() => Cup.color, a => Cup.color = a, new Color(255, 255, 255, 255), 0.7f).SetEase(Ease.InQuint);
    }

    /// <summary>
    /// 关杯子和开杯子整合动画
    /// </summary>
    /// <param name="Cup"></param>
    private async void CupAnim(Image Cup,Image Icon)
    {
        OpenCupAnim(Cup);
        IconMinToMax(Icon);

        await UniTask.Delay(TimeSpan.FromSeconds(1f));

        CloseCupAnim(Cup);
    }

    /// <summary>
    /// 杯子打乱
    /// </summary>
    /// <param name="Cup"></param>
    private async void SortCupAnim(Transform Pos)
    {

        DOTween.To(() => Pos.localPosition, pos => Pos.localPosition = pos, new Vector3(0, 39, 0), 0.5f);

        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

        int randNUm = Rand();

        Vector3 vc3 = initCupPos[randNUm];

        while (UsedCupPos.Contains(vc3))
        {
            randNUm = Rand();

            vc3 = initCupPos[randNUm];
        }

        UsedCupPos.Add(vc3);

        DOTween.To(() => Pos.localPosition, pos => Pos.localPosition = pos, vc3, 0.5f);

    }

    /// <summary>
    /// 随机数
    /// </summary>
    /// <returns></returns>
    private int Rand()
    {
         int num =UnityEngine.Random.Range(0, 9);
        if (num < 3)
        {
            return 0;
        }
        if (num >= 3 && num < 6)
        {
            return 1;
        }

        return 2;
    }

    /// <summary>
    /// 物品从小到大
    /// </summary>
    /// <param name="Icon"></param>
    private void IconMinToMax(Image Icon)
    {
        DOTween.To(() => Icon.transform.localScale, size => Icon.transform.localScale = size, Vector3.one * 0.8f, 0.5f).SetEase(Ease.Linear);
    }


    #endregion



    /// <summary>
    /// 获取展示物品 规则在等级及以下获取奖励物品
    /// </summary>
    private void GetSeedAward() 
    {
        _gridRewards.Clear();
        var lv = StaticData.GetPlayerLevelAndCurrExp().level;

        List<GoodIDCount> allAwards = new List<GoodIDCount>();
        for (int i = 0; i < StaticData.configExcel.ZillionaireMapServerAward.Count; i++)
        {
            if (StaticData.configExcel.ZillionaireMapServerAward[i].ID <= lv) 
            {
                allAwards.AddRange(StaticData.configExcel.ZillionaireMapServerAward[i].SeedAward);
            }
        }
        for (int i = 0; i < StaticData.configExcel.ZillionaireMapServerAward[0].SeedAward.Count; i++)
        {
            int index = UnityEngine.Random.Range(0, allAwards.Count);
            _gridRewards.Add(allAwards[index]);
            allAwards.RemoveAt(index);
        }
        
    }

    /// <summary>
    /// 获取奖励并且显示奖励
    /// </summary>
    private void GetRewards()
    {
        GetSeedAward();

        string path = string.Empty;
        Sprite icon = null;
        for (int i = 0; i < _gridRewards.Count; i++)
        {
            path = StaticData.configExcel.GetGameItemByID(_gridRewards[i].ID).Icon;
            icon = ABManager.GetAsset<Sprite>(path);
            _animElements[i]._itemIcon.sprite = icon;
            _animElements[i]._itemNum.text = _gridRewards[i].Count.ToString();
        }
    }


    /// <summary>
    /// 更新抽奖次数显示
    /// </summary>
    private void UpdateLotterNumShow()
    {
        string desc = string.Empty;
        if (_lotterNum <= 0)
        {
            desc = "<color=#FE4F84>0</color>" + LocalizationDefineHelper.GetStringNameById(120190);//120190//次
        }
        else
        {
            desc = _lotterNum + LocalizationDefineHelper.GetStringNameById(120190);
        }

        _openNum.text = desc;
    }


    #region 按钮事件

    /// <summary>
    /// 取消抽奖/关闭抽奖界面
    /// </summary>
    private async void OnClickCancel()
    {

        //取消动画
        UniversalTool.CancelUIAnimTwo(canvasGroup, craneMachine, null);
        await UniTask.Delay(TimeSpan.FromSeconds(0.35));

        _needCloseUI = true;
        SendServer(true);
        //
    }

    private void OnClickCard0()
    {
        OnClickCard(0);
    }

    private void OnClickCard1()
    {
        OnClickCard(1);
    }

    private void OnClickCard2()
    {
        OnClickCard(2);
    }

    /// <summary>
    /// 点击卡片 
    /// </summary>
    /// <param name="id">卡片id 从0开始</param>
    private async void OnClickCard(int id)
    {

        if (_PlayingAnim)
            return;

        if (_isEndInitAnim == false)
        {
            Debug.Log("默认动画未播放完不能点击");
            return;
        }

        if (_isAllLottery || _lotterNum <= 0)
            return;
        //卡片是否被选中过了
        if (_slectedCards.Contains(id) || id >= _animElements.Count)
            return;

        //正在打开杯子中
        if (_isOpenCup)
        {
            Debug.Log("正在打开杯子不能点击");
            return;
        }

        //打开杯子中
        _isOpenCup = true;

        _lotterNum -= 1;

        _slectedCards.Add(id);
        if (_slectedCards.Count >= 3) 
        {
            //禁止点击按钮
            _butAD.interactable = false;
        }
            
        //更新cup状态
        var icon =_animElements[id]._itemCup;

        OpenCupAnim(icon);

        UpdateLotterNumShow();


        //等待动画执行完毕
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        _isOpenCup = false;
        //打开关闭按钮
        _butClose.gameObject.SetActive(true);
        if (_butClose.transform.localScale != Vector3.one)
            _butClose.transform.DOScale(Vector3.one, 0.3f);
    }

    /// <summary>
    /// 广告抽奖
    /// </summary>
    private void OnClickAD()
    {

        if (_PlayingAnim)
            return;

        if (_isEndInitAnim == false)
        {
            Debug.Log("默认动画未播放完不能点击");
            return;
        }
            

        if (_isAllLottery)
            return;

        //正在打开杯子中
        if (_isOpenCup)
        {
            Debug.Log("正在打开杯子不能点击");
            return;
        }

        //打开杯子中
        _isOpenCup = true;
        StaticData.OpenAd("GuessAd", (code, msg) => {
            if (code == 1)
            {
                //todo 广告成功代码
                ADSuccess();
                //数据打点
                StaticData.DataDot(DotEventId.GuessBeanAd);
                //禁止点击按钮
                _butAD.interactable = false;
                //打开关闭按钮
                _butClose.gameObject.SetActive(true);
                if (_butClose.transform.localScale != Vector3.one)
                    _butClose.transform.DOScale(Vector3.one, 0.3f);
            }
            else
            {
                _isOpenCup = false;
            }
        });
    }

    /// <summary>
    /// 观看广告成功
    /// </summary>
    private async void ADSuccess() 
    {
        for (int i = 0; i < _gridRewards.Count; i++)
        {
            if (!_slectedCards.Contains(i))
            {
                _slectedCards.Add(i);

                var icon = _animElements[i]._itemCup;

                OpenCupAnim(icon);
            }

        }
        SendServer();

        //等待动画执行完毕
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        _isOpenCup = false;
    }


    /// <summary>
    /// 发送请求到服务器
    /// </summary>
    private void SendServer(bool isClose = false)
    {
        //是否已经请求过 / 请求数据长度小于等于0
        if (_isAllLottery || _slectedCards.Count <= 0)
        {
            if (isClose)
                CloseUI();
            return;
        }

        List<int> rewards = new List<int>();
        for (int i = 0; i < _gridRewards.Count; i++)
        {
            if (_slectedCards.Contains(i))
            {
                rewards.Add(_gridRewards[i].ID);
            }
        }

        ZillionaireToolManager.NotifyServerChooseOrLucky(LuckyTyep.LuckySeed, rewards, ServerCallback);
    }

    /// <summary>
    /// 服务器回调
    /// </summary>
    /// <param name="sCLucky"></param>
    private void ServerCallback(SCLucky sCLucky)
    {
        _isAllLottery = true;

        //1.将获得的奖励加入临时表中
        for (int i = 0; i < _gridRewards.Count; i++)
        {
            if (_slectedCards.Contains(i))
            {
                ZillionairePlayerManager._instance.CurrentPlayer.AddPlayerRewards(_gridRewards[i].ID, (int)_gridRewards[i].Count);
            }
        }

        //关闭界面
        if (_needCloseUI)
            CloseUI();
            
    }
    private void CloseUI() 
    {
        UIComponent.RemoveUI(UIType.UIGuess);
        CloseCallback?.Invoke();
    }

    #endregion

    #endregion
}
