using Company.Cfg;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 仓库宝箱
/// </summary>
public class WarehouseTreasureChest : MonoBehaviour
{
    #region 字段
    /// <summary>
    /// 宝箱数据
    /// </summary>
    List<GoodsData> _goodsDatas = new List<GoodsData>();
    int _key;
    /// <summary>
    /// 抖动动画
    /// </summary>
    DOTweenAnimation _joggleDOTweenAnimation;
    /// <summary>
    /// 数量显示
    /// </summary>
    Text _shuowNumber;
    /// <summary>
    /// 宝箱按钮
    /// </summary>
    Button _treasureChestBtn;
    ///// <summary>
    ///// 遮罩
    ///// </summary>
    //Transform _maskTra;
    ///// <summary>
    ///// 时间进度遮罩
    ///// </summary>
    //Image _maskImage;
    /// <summary>
    /// 计时器
    /// </summary>
    TimeCountDownComponent _timeCountDownComponent;
    /// <summary>
    /// 当前宝箱配置数据
    /// </summary>
    PackageDefine _configCurrData;
    /// <summary>
    /// 提示标语
    /// </summary>
    [SerializeField]
    Transform _showTipTra;
    /// <summary>
    /// 提示背景1
    /// </summary>
    Transform _tipOneBgTra;
    /// <summary>
    /// 提示背景2
    /// </summary>
    Transform _tipTwoBgTra;
    /// <summary>
    /// 提示背景3
    /// </summary>
    Transform _tipTherrBgTra;
    /// <summary>
    /// 时钟图标
    /// </summary>
    Transform _showTipTimeTra;
    /// <summary>
    /// 提示语
    /// </summary>
    Text _showTipText;
    /// <summary>
    /// 红点
    /// </summary>
    RectTransform _redDotRect;

    /// <summary>
    /// 倒计时时间
    /// </summary>
    long _countDownTime;
    /// <summary>
    /// 宝箱剩余时间
    /// </summary>
    float _remainingTime = 0f;
    /// <summary>
    /// 是否处于时间解锁中
    /// </summary>
    bool _isBeTimeUnlock = false;
    /// <summary>
    /// 是否可以领取奖励
    /// </summary>
    bool _isCanReceiveAward = false;
    /// <summary>
    /// 时间解锁回调和广告加速回调
    /// </summary>
    Action<long> _curTimeAction;
    /// <summary>
    /// 当前领取的宝箱
    /// </summary>
    int _currDataId = 0;
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    bool _isInitial;
    /// <summary>
    /// 是否初始化数据完毕
    /// </summary>
    bool _isInitialData;
    /// <summary>
    /// 是否可以播放抖动动画
    /// </summary>
    bool _isPalyJoggleDOTweenAnimation = false;
    #endregion
    #region 函数
    // Start is called before the first frame update
    void Start()
    {
        Initial();
    }
    // Update is called once per frame
    void Update()
    {
        //计时
        if (_timeCountDownComponent != null && _isBeTimeUnlock)
        {
            float RemainTime = _remainingTime;

            if (RemainTime <= 0)
            {
                //关闭计时器，点击直接解锁
                _isCanReceiveAward = true;
                //_maskImage.fillAmount = 1;
                //_maskTra.gameObject.SetActive(false);
                SetShowTip(3);
                RefreshTreasureChestUnlock();
                //GoGiftBoxTimer.SetActive(false);
            }
            else
            {
                ////计算开始时间
                //float configTime = _configCurrData.BoxUnlockTime;
                ////long startTime = _countDownTime + configTime;
                ////Debug.Log("解锁所需时间：" + configTime);
                ////获取当前服务器时间
                //long CurrTimeStampServer = TimeHelper.ServerTimeStampNow;
                ////Debug.Log("服务器当前时间：" + CurrTimeStampServer);
                ////计算剩余时间
                //long CurrRemainTime = _countDownTime - CurrTimeStampServer;
                ////Debug.Log("剩余时间：" + CurrRemainTime);
                ////计算剩余时间对于开始时间的百分比
                //float TimeThanColumn = (CurrRemainTime / 1000) / configTime;

                ////Debug.Log("时间百分比：" + TimeThanColumn);
                //_maskImage.fillAmount = TimeThanColumn;

                //设置倒计时时间

                int h = (int)(RemainTime / 3600);
                RemainTime = RemainTime % 3600;
                int m = (int)(RemainTime / 60);
                RemainTime = RemainTime % 60;
                int s = (int)RemainTime;
                string H = String.Format("{0:00}", h);
                string M = String.Format("{0:00}", m);
                string S = String.Format("{0:00}", s);
                string timeStr = H + ":" + M + ":" + S;
                SetShowTip(2, timeStr);
            }
        }
    }
    /// <summary>
    /// 初始化组件
    /// </summary>
    void Initial()
    {
        _joggleDOTweenAnimation = transform.GetComponent<DOTweenAnimation>();
        _shuowNumber = transform.Find("ShowNumber/Text").GetComponent<Text>();
        _treasureChestBtn = GetComponent<Button>();
        _treasureChestBtn.onClick.RemoveListener(ClickTresureChestBtn);
        _treasureChestBtn.onClick.AddListener(ClickTresureChestBtn);

        _redDotRect = transform.Find("RedDot").GetComponent<RectTransform>();
        //_maskTra = transform.Find("Mask");
        //_maskImage = _maskTra.GetComponent<Image>();


        _tipOneBgTra = _showTipTra.Find("OneBg");
        _tipTwoBgTra = _showTipTra.Find("TwoBg");
        _tipTherrBgTra = _showTipTra.Find("TherrBg");
        _showTipTimeTra = _showTipTra.Find("Time");
        _showTipText = _showTipTra.Find("Text").GetComponent<Text>();
        _isInitial = true;
    }
    /// <summary>
    /// 初始化宝箱数据
    /// </summary>
    public void InitialData(int key, List<GoodsData> datas)
    {
        if (!_isInitial)
        {
            Initial();
        }
        _key = key;
        _goodsDatas.Clear();
        if (datas == null)
        {
            transform.gameObject.SetActive(false);
            SetShowTip(0);
            OpenRedDot(false);
            return;
        }
        else
        {
            transform.gameObject.SetActive(true);
        }
        for (int i = 0; i < datas.Count; i++)
        {
            _goodsDatas.Add(datas[i].CopyThis());
        }

        int number = 0;
        for (int i = 0; i < _goodsDatas.Count; i++)
        {
            number = number + (int)_goodsDatas[i]._number;
        }
        _shuowNumber.text = "x" + number.ToString();
        int oldNumber = WarehouseTool.GetTreasureChaestsValue((int)_goodsDatas[0]._data.Rarity);

        OpenRedDot(oldNumber < _goodsDatas.Count);
        GetUnlocked();
        _isInitialData = true;
    }
    /// <summary>
    /// 创建空白计时器
    /// </summary>
    public void CreationTimer()
    {
        _timeCountDownComponent = StaticData.CreateTimer(100000 / 1000f, false, (go) =>
        {

        },
        (remainTime) =>
        {

        }, "WarehouseTreasureChest");
    }
    /// <summary>
    /// 开始计时
    /// </summary>
    public void StartCountingTime()
    {
        //计算当前服务器时间
        long CurrTimeStampServer = TimeHelper.ServerTimeStampNow;
        //计算剩余时间
        long CurrRemainTime = _countDownTime - CurrTimeStampServer;
        if (_isBeTimeUnlock)
        {
            if (CurrRemainTime <= 0f)
            {
                _isBeTimeUnlock = false;
                _isCanReceiveAward = true;
                //_maskTra.gameObject.SetActive(false);
                SetShowTip(3);
                RefreshTreasureChestUnlock();
                Destroy(_timeCountDownComponent.gameObject);
            }
            else
            {
                _timeCountDownComponent.Init(CurrRemainTime / 1000f, false, (go) =>
                {
                    //时间到了
                    _isCanReceiveAward = true;
                    _isBeTimeUnlock = false;
                    Destroy(_timeCountDownComponent.gameObject);
                },
                (remainTime) =>
                {
                    //记录剩余时间
                    _remainingTime = remainTime;
                    _isCanReceiveAward = false;
                });
            }
        }
        else
        {
            _isCanReceiveAward = false;
        }
    }
    /// <summary>
    /// 判断是否有解锁的宝箱
    /// </summary>
    public bool IsThereAnUnlock()
    {
        for (int i = 0; i < _goodsDatas.Count; i++)
        {
            if (WarehouseTool.IsThereAnUnlock(_goodsDatas[i]._id))
            {

                Debug.Log("有解锁的宝箱：" + _goodsDatas[i]._id);
                _currDataId = _goodsDatas[i]._id;
                return true;
            }

        }
        _currDataId = 0;
        return false;
    }
    /// <summary>
    /// 判断是否有正在时间解锁的宝箱
    /// </summary>
    public bool IsTimeThereAnUnlock()
    {
        for (int i = 0; i < _goodsDatas.Count; i++)
        {
            if (WarehouseTool.IsTimeThereAnUnlock(_goodsDatas[i]._id))
            {

                Debug.Log("有时间解锁的宝箱：" + _goodsDatas[i]._id);
                _currDataId = _goodsDatas[i]._id;
                return true;
            }

        }
        _currDataId = 0;
        return false;
    }
    /// <summary>
    /// 获取解锁状态
    /// </summary>
    public void GetUnlocked()
    {
        if (IsThereAnUnlock())
        {
            _isCanReceiveAward = true;
            _isBeTimeUnlock = false;
            //_maskTra.gameObject.SetActive(false);
            RefreshTreasureChestUnlock();
            Debug.Log("解锁");
            SetShowTip(3);
        }
        else if (IsTimeThereAnUnlock())
        {
            _isCanReceiveAward = false;
            _isBeTimeUnlock = true;
            Debug.Log("正在解锁");
            _configCurrData = WarehouseTool.GetTreasureChestConfig(_currDataId);
            TimeUnlockTreasure(true, WarehouseTool.GetTimeThereAnUnlockTime(_currDataId));

        }
        else
        {
            SetShowTip(1);
        }
    }
    /// <summary>
    /// 点击宝箱
    /// </summary>
    public void ClickTresureChestBtn()
    {
        GetUnlocked();
        OpenRedDot(false);
        WarehouseController.Instance.RefreshData();
        if (_isBeTimeUnlock)
        {
            StaticData.OpenTreasureSpeedUp(_configCurrData.ID, _countDownTime, SpeedUp);
            return;
        }
        else
        if (_isCanReceiveAward)
        {
            GetAward();
        }
        else
        {
            int id = _goodsDatas[0]._id;
            _configCurrData = WarehouseTool.GetTreasureChestConfig(id);
            StaticData.OpenTreasureUnlock(_configCurrData.ID, KeyUnlock, FreeUnlock, SpeedUp);
        }
    }
    /// <summary>
    /// 广告加速
    /// </summary>
    /// <param name="timeCountDownComponent"></param>
    /// <param name="time"></param>
    public void SpeedUp(Action<long> time)
    {
        int id = _goodsDatas[0]._id;
        _configCurrData = WarehouseTool.GetTreasureChestConfig(id);
        _curTimeAction = time;
        WarehouseTool.TreasureChestAdvertisingSpeedUp(id, TreasureChestAdvertisingSpeedUpAction);
    }
    /// <summary>
    /// 广告加速回调
    /// </summary>
    /// <param name="isSucceed"></param>
    /// <param name="unlockTime"></param>
    void TreasureChestAdvertisingSpeedUpAction(bool isSucceed, long unlockTime)
    {
        if (isSucceed)
        {   //计算时间差
            long timeDifference = _countDownTime - unlockTime;
            float remainTime = timeDifference / 1000f;

            remainTime = remainTime / 60;
            int m = (int)(remainTime);

            StaticData.CreateToastTips(string.Format(StaticData.GetMultilingual(120262), m));

            UpdateUnlockTime(unlockTime);
            _curTimeAction?.Invoke(unlockTime);

        }
        else
        {
            if (unlockTime == -1)
            {
                StaticData.CreateToastTips("操作过于频繁");
            }
        }
    }
    /// <summary>
    /// 免费解锁
    /// </summary>
    /// <param name="timeCountDownComponent"></param>
    /// <param name="time"></param>
    public void FreeUnlock(Action<long> time)
    {
        int id = _goodsDatas[0]._id;
        _configCurrData = WarehouseTool.GetTreasureChestConfig(id);
        _curTimeAction = time;
        WarehouseTool.UnlockTreasureChest(id, Game.Protocal.UnlockTreasureChest.CountDown, TimeUnlockTreasure);
    }
    /// <summary>
    /// 时间解锁回调
    /// </summary>
    /// <param name="isSucceed"></param>
    void TimeUnlockTreasure(bool isSucceed, long unlockTime)
    {
        if (isSucceed)
        {
            WarehouseTool.ChangeUnlockTreasureChestID(_configCurrData.BoxID, unlockTime);
            //记录时间戳
            _countDownTime = unlockTime;
            _currDataId = _configCurrData.BoxID;
            if (_timeCountDownComponent == null)
            {
                //创建空白的计时器
                CreationTimer();
            }
            _isBeTimeUnlock = true;
            //开始计时
            StartCountingTime();
            //_maskTra.gameObject.SetActive(true);
            SetShowTip(3);
            _curTimeAction?.Invoke(unlockTime);
        }

    }
    /// <summary>
    /// 钥匙解锁
    /// </summary>
    public void KeyUnlock()
    {
        int id = _goodsDatas[0]._id;
        _configCurrData = WarehouseTool.GetTreasureChestConfig(id);
        WarehouseTool.UnlockTreasureChest(id, Game.Protocal.UnlockTreasureChest.Key, KeyUnlockSucceed);
    }
    /// <summary>
    /// 钥匙解锁成功回调
    /// </summary>
    /// <param name="isSucceed"></param>
    /// <param name="unlockTime"></param>
    void KeyUnlockSucceed(bool isSucceed, long unlockTime)
    {
        if (isSucceed)
        {
            StaticData.UpdateWareHouseItem(_configCurrData.UseGoodsNum[0].ID, -(int)_configCurrData.UseGoodsNum[0].Count);
            WarehouseTool.ChangeUnlockTreasureChestID(_configCurrData.BoxID, TimeHelper.ServerTimeStampNow);
            WarehouseController.Instance.RefreshNewData();
            GetAward();
        }

    }
    /// <summary>
    /// 领取宝箱
    /// </summary>
    public void GetAward()
    {
        if (_currDataId != 0)
        {
            WarehouseTool.GetAward(_currDataId, GetAwardAction);
        }
    }
    /// <summary>
    /// 领取宝箱回调
    /// </summary>
    /// <param name="goodsDatas"></param>
    void GetAwardAction(List<Game.Protocal.CSWareHouseStruct> goodsDatas)
    {
        _isCanReceiveAward = false;
        StaticData.OpenReceiveAward(goodsDatas, _currDataId);
        WarehouseController.Instance.ConsumeTreasureChest(_currDataId);
        CloseJoggleDOTweenAnimation();
        WarehouseTool.ChangeUnlockTreasureChestID(_currDataId, 0);
        WarehouseController.Instance.RefreshNewData();
        CloseTime();
    }
    /// <summary>
    /// 刷新解锁信息
    /// </summary>
    public void RefreshTreasureChestUnlock()
    {
        if (_isCanReceiveAward)
        {
            _isPalyJoggleDOTweenAnimation = true;
            PalyJoggleDOTweenAnimation();
        }
        else
        {
            _isPalyJoggleDOTweenAnimation = false;
            CloseJoggleDOTweenAnimation();
        }
    }
    /// <summary>
    /// 关闭抖动动画
    /// </summary>
    void CloseJoggleDOTweenAnimation()
    {
        _isPalyJoggleDOTweenAnimation = false;
        PalyJoggleDOTweenAnimation();
        CancelInvoke();
    }
    /// <summary>
    /// 播放抖动动画
    /// </summary>
    public void PalyJoggleDOTweenAnimation()
    {
        if (!_isPalyJoggleDOTweenAnimation)
        {
            transform.localEulerAngles = Vector3.zero;
            _joggleDOTweenAnimation.enabled = false;
            return;
        }
        _joggleDOTweenAnimation.enabled = true;
        _joggleDOTweenAnimation.DORestart();
        Invoke("PalyJoggleDOTweenAnimation", 2f);
    }
    /// <summary>
    /// 更新解锁时间
    /// </summary>
    /// <param name="unlockTime"></param>
    public void UpdateUnlockTime(long unlockTime)
    {
        _countDownTime = unlockTime;
        _timeCountDownComponent.Dispose();
        StartCountingTime();
        WarehouseTool.ChangeUnlockTreasureChestID(_currDataId, unlockTime);
    }
    /// <summary>
    /// 关闭计时器
    /// </summary>
    public void CloseTime()
    {
        if (_timeCountDownComponent != null)
        {
            _timeCountDownComponent.Dispose();
            Destroy(_timeCountDownComponent.gameObject);
        }
    }
    /// <summary>
    /// 开关红点
    /// </summary>
    /// <param name="isOpen"></param>
    public void OpenRedDot(bool isOpen)
    {
        if (!isOpen)
        {
            if (_goodsDatas != null && _goodsDatas.Count > 0)
            {
                WarehouseTool.SaveTreasureChaestsValue((int)_goodsDatas[0]._data.Rarity, _goodsDatas.Count);
            }
            else
            {
                WarehouseTool.SaveTreasureChaestsValue(_key, 0);
            }

        }
        _redDotRect.gameObject.SetActive(isOpen);
    }
    /// <summary>
    /// 设置提示语
    /// </summary>
    /// <param name="type">0：未获得 1：未解锁 2：正在解锁 3：待领取</param>
    public void SetShowTip(int type, string time = "")
    {
        _tipOneBgTra.gameObject.SetActive(false);
        _tipTwoBgTra.gameObject.SetActive(false);
        _tipTherrBgTra.gameObject.SetActive(false);
        _showTipTimeTra.gameObject.SetActive(false);
        switch (type)
        {
            case 0:
                _tipTwoBgTra.gameObject.SetActive(true);
                _showTipText.color = new Color32(162, 132, 253, 255);
                _showTipText.text = "未获得";
                break;
            case 1:
                _tipOneBgTra.gameObject.SetActive(true);
                _showTipText.color = new Color32(63, 126, 240, 255);
                _showTipText.text = "未解锁";
                break;
            case 2:
                _showTipTimeTra.gameObject.SetActive(true);
                _tipTherrBgTra.gameObject.SetActive(true);
                _showTipText.color = new Color32(255, 255, 255, 255);
                _showTipText.text = time;
                break;
            case 3:
                _tipTherrBgTra.gameObject.SetActive(true);
                _showTipText.color = new Color32(255, 255, 255, 255);
                _showTipText.text = "领取奖励";
                break;
        }
        RectTransform showTipRect = _showTipText.transform.GetComponent<RectTransform>();
        switch (type)
        {
            case 2:
                showTipRect.sizeDelta = new Vector2(175, showTipRect.sizeDelta.y);
                _showTipText.fontStyle = FontStyle.Bold;
                _showTipText.alignment = TextAnchor.LowerLeft;
                break;
            default:
                showTipRect.sizeDelta = new Vector2(288, showTipRect.sizeDelta.y);
                _showTipText.fontStyle = FontStyle.Normal;
                _showTipText.alignment = TextAnchor.LowerCenter;
                break;
        }


    }
    #endregion
}
