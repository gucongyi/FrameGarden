using Company.Cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 宝箱解锁弹窗
/// </summary>
public class GiftBoxUnlockingController : MonoBehaviour
{
    #region 字段
    /// <summary>
    /// 关闭按钮
    /// </summary>
    Button _closeBtn;
    /// <summary>
    /// 遮罩按钮
    /// </summary>
    Button _maskBtn;
    /// <summary>
    /// bgbox
    /// </summary>
    Transform _bgBoxTra;
    /// <summary>
    /// 计时器盒子
    /// </summary>
    Transform _timeBoxTra;
    /// <summary>
    /// 弹窗名字
    /// </summary>
    Text _titleNameText;
    /// <summary>
    /// 弹窗icon
    /// </summary>
    Image _iconImage;
    /// <summary>
    /// 随机奖励标题
    /// </summary>
    Text _showPrizeTitleText;
    /// <summary>
    /// item生成点
    /// </summary>
    Transform _showPrizeItemGenericPoint;
    /// <summary>
    /// 显示时间文本box
    /// </summary>
    Transform _showTimeBoxTra;
    /// <summary>
    /// 时间部分标题
    /// </summary>
    Text _ShowTimeBoxTitelText;
    /// <summary>
    /// 时
    /// </summary>
    Text _hourText;
    /// <summary>
    /// 分
    /// </summary>
    Text _minuteText;
    /// <summary>
    /// 秒
    /// </summary>
    Text _secondText;
    /// <summary>
    /// 按钮box
    /// </summary>
    Transform _btnBoxTra;
    /// <summary>
    /// 时间加速按钮
    /// </summary>
    Button _speedUpBtn;
    /// <summary>
    /// 要塞解锁按钮
    /// </summary>
    Button _keyBtn;
    /// <summary>
    /// 所需钥匙
    /// </summary>
    Text _keyNumber;
    /// <summary>
    /// 玩家剩余钥匙
    /// </summary>
    Text _surplusKeyNumber;
    /// <summary>
    /// 免费解锁按钮
    /// </summary>
    Button _freeBtn;
    /// <summary>
    /// item母体
    /// </summary>
    Transform _showPrizeItem;
    /// <summary>
    /// 钥匙解锁回调
    /// </summary>
    Action _keyUnlockAction;
    /// <summary>
    /// 免费解锁回调
    /// </summary>
    Action<Action<long>> _freeUnlockAction;
    /// <summary>
    /// 加速回调
    /// </summary>
    Action<Action<long>> _speedAction;
    /// <summary>
    /// 计时器
    /// </summary>
    TimeCountDownComponent _timeCountDownComponent;
    /// <summary>
    /// 宝箱配置数据
    /// </summary>
    PackageDefine _packageDefine;
    /// <summary>
    /// 宝箱item集合
    /// </summary>
    List<ShowPrizeItemController> _items = new List<ShowPrizeItemController>();
    /// <summary>
    /// 解锁时间
    /// </summary>
    long _unlockTime;
    /// <summary>
    /// 宝箱剩余时间
    /// </summary>
    float _remainingTime = 0f;
    /// <summary>
    /// 是否时间加速完毕
    /// </summary>
    bool _isSpeedUpFinish = false;
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    bool _isInitial = false;
    #endregion
    #region 函数
    private void Awake()
    {
        if (!_isInitial)
        {
            Initial();
        }
        if (_bgBoxTra != null)
            UniversalTool.ReadyPopupAnim(_bgBoxTra);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        if (_bgBoxTra != null)
            UniversalTool.StartPopupAnim(_bgBoxTra);
    }
    private void OnDisable()
    {
        if (_bgBoxTra != null)
            UniversalTool.ReadyPopupAnim(_bgBoxTra);
    }
    // Update is called once per frame
    void Update()
    {
        if (_timeCountDownComponent != null && _isSpeedUpFinish == false)
        {
            float RemainTime = _remainingTime;
            if (RemainTime <= 0)
            {
                //关闭计时器，点击直接解锁
                _isSpeedUpFinish = true;
                Destroy(_timeCountDownComponent);
                _hourText.text = String.Format("{0:00}", 0);
                _minuteText.text = String.Format("{0:00}", 0);
                _secondText.text = String.Format("{0:00}", 0);
                Close();
            }
            else
            {
                int h = (int)(RemainTime / 3600);
                RemainTime = RemainTime % 3600;
                int m = (int)(RemainTime / 60);
                RemainTime = RemainTime % 60;
                int s = (int)RemainTime;
                _hourText.text = String.Format("{0:00}", h);
                _minuteText.text = String.Format("{0:00}", m);
                _secondText.text = String.Format("{0:00}", s);
            }
        }
    }
    /// <summary>
    /// 初始化组件
    /// </summary>
    public void Initial()
    {

        _closeBtn = transform.Find("BgBox/Close").GetComponent<Button>();
        _maskBtn = transform.Find("ButtonMask").GetComponent<Button>();
        // bgbox
        _bgBoxTra = transform.Find("BgBox");
        // 弹窗名字
        _titleNameText = _bgBoxTra.Find("TitleBg/TextTile").GetComponent<Text>();
        // 弹窗icon
        _iconImage = _bgBoxTra.Find("IconBox/Icon").GetComponent<Image>();
        // 随机奖励标题
        _showPrizeTitleText = _bgBoxTra.Find("ShowPrizeBox/Title").GetComponent<Text>();
        // item生成点
        _showPrizeItemGenericPoint = _bgBoxTra.Find("ShowPrizeBox/PrizeItemBox/Scroll View").GetComponent<ScrollRect>().content;
        // 显示时间文本box
        _showTimeBoxTra = _bgBoxTra.Find("BtnBox/ShowTimeBox");
        // 时间部分标题
        _ShowTimeBoxTitelText = _showTimeBoxTra.Find("Title").GetComponent<Text>();
        // 时
        _hourText = _showTimeBoxTra.Find("TimeBox/Hour").GetComponent<Text>();
        // 分
        _minuteText = _showTimeBoxTra.Find("TimeBox/Minute").GetComponent<Text>();
        // 秒
        _secondText = _showTimeBoxTra.Find("TimeBox/Second").GetComponent<Text>();
        // 按钮box
        _btnBoxTra = _bgBoxTra.Find("BtnBox");
        // 时间加速按钮
        _speedUpBtn = _btnBoxTra.Find("SpeedUp").GetComponent<Button>();
        // 要塞解锁按钮
        _keyBtn = _btnBoxTra.Find("KeyBtn").GetComponent<Button>();
        _keyNumber = _keyBtn.transform.Find("Box/Number").GetComponent<Text>();
        _surplusKeyNumber = _keyBtn.transform.Find("SurplusKey/Number").GetComponent<Text>();
        // 免费解锁按钮
        _freeBtn = _btnBoxTra.Find("FreeBtn").GetComponent<Button>();
        // item母体
        _showPrizeItem = _bgBoxTra.Find("Item");
        _timeBoxTra = transform.Find("TimeBox");
        SetPanelMultilingual();

        _maskBtn.onClick.RemoveListener(Close);
        _maskBtn.onClick.AddListener(Close);
        _closeBtn.onClick.RemoveAllListeners();
        _closeBtn.onClick.AddListener(Close);
        _speedUpBtn.onClick.RemoveListener(ClickSpeedUp);
        _speedUpBtn.onClick.AddListener(ClickSpeedUp);
        _keyBtn.onClick.RemoveListener(ClickKeyUnlock);
        _keyBtn.onClick.AddListener(ClickKeyUnlock);
        _freeBtn.onClick.RemoveListener(ClickFreeUnlock);
        _freeBtn.onClick.AddListener(ClickFreeUnlock);
        _isInitial = true;
    }
    /// <summary>
    /// 打开解锁界面
    /// </summary>
    /// <param name="id"></param>
    /// <param name="KeyUnlock"></param>
    /// <param name="freeUnlock"></param>
    public void ShowUnlock(int id, Action KeyUnlock, Action<Action<long>> freeUnlock, Action<Action<long>> speedAction)
    {
        _keyUnlockAction = KeyUnlock;
        _freeUnlockAction = freeUnlock;
        _speedAction = speedAction;
        _packageDefine = WarehouseTool.GetTreasureChestConfigData(id);
        ShowData(_packageDefine);
        ChangeInterface(0);
    }
    /// <summary>
    /// 打开加速界面
    /// </summary>
    /// <param name="id"></param>
    /// <param name="timeStamp"></param>
    /// <param name="speedUpAction"></param>
    public void ShowSpeedUp(int id, long timeStamp, Action<Action<long>> speedAction)
    {
        _speedAction = speedAction;
        _packageDefine = WarehouseTool.GetTreasureChestConfigData(id);
        ShowData(_packageDefine, 1);
        ChangeInterface(1);
        RefreshTime(timeStamp);
    }
    /// <summary>
    /// 展示宝箱数据
    /// </summary>
    /// <param name="data"></param>
    async void ShowData(PackageDefine data, int type = 0)
    {
        GameItemDefine gameItemDefine = WarehouseTool.GetGameItemData(data.BoxID);

        DestroyTime();
        _titleNameText.text = StaticData.GetMultilingual(gameItemDefine.ItemName);

        _keyNumber.text = data.UseGoodsNum[0].Count.ToString();
        _surplusKeyNumber.text = WarehouseTool.GetWareHouseGold(data.UseGoodsNum[0].ID).ToString();
        ClearItem();
        List<int> awardIds = GetAward(data);
        if (awardIds != null && awardIds.Count > 0)
        {
            for (int i = 0; i < awardIds.Count; i++)
            {
                CreateItem(awardIds[i]);
            }
        }
        _iconImage.sprite = null;
        _iconImage.sprite = await ABManager.GetAssetAsync<Sprite>(gameItemDefine.Icon);
        //_iconImage.SetNativeSize();

        if (type == 0)
        {
            _showTimeBoxTra.gameObject.SetActive(false);
        }
        else
        {
            _showTimeBoxTra.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// 获取宝箱配置的奖励数据
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    List<int> GetAward(PackageDefine data)
    {
        List<int> ids = new List<int>();
        for (int i = 0; i < data.PackageContent.Count; i++)
        {
            if (!ids.Contains(data.PackageContent[i].ID))
            {
                ids.Add(data.PackageContent[i].ID);
            }
        }

        return ids;
    }
    /// <summary>
    /// 改变面板界面
    /// </summary>
    /// <param name="type">0：解锁界面 1：加速界面</param>
    void ChangeInterface(int type)
    {
        if (type == 0)
        {
            _showTimeBoxTra.gameObject.SetActive(false);
            _keyBtn.gameObject.SetActive(true);
            _freeBtn.gameObject.SetActive(true);
            _speedUpBtn.gameObject.SetActive(false);
        }
        else
        {
            _showTimeBoxTra.gameObject.SetActive(true);
            _keyBtn.gameObject.SetActive(false);
            _freeBtn.gameObject.SetActive(false);
            _speedUpBtn.gameObject.SetActive(true);
        }

    }
    /// <summary>
    /// 生成奖励预览item
    /// </summary>
    /// <param name="probabilityReward"></param>
    void CreateItem(int awardId)
    {
        Transform itemTra = GameObject.Instantiate(_showPrizeItem, _showPrizeItemGenericPoint);
        ShowPrizeItemController showPrizeItemController = itemTra.GetComponent<ShowPrizeItemController>();
        showPrizeItemController.ShowData(awardId);
        _items.Add(showPrizeItemController);
    }
    /// <summary>
    /// 清理item
    /// </summary>
    void ClearItem()
    {

        for (int i = 0; i < _items.Count; i++)
        {
            Destroy(_items[i].gameObject);
        }
        _items.Clear();
    }
    /// <summary>
    /// 点击钥匙解锁
    /// </summary>
    public void ClickKeyUnlock()
    {
        int keyNumber = (int)_packageDefine.UseGoodsNum[0].Count;
        int surplusKeyNumber = WarehouseTool.GetWareHouseGold(_packageDefine.UseGoodsNum[0].ID);
        if (surplusKeyNumber < keyNumber)
        {
            StaticData.CreateToastTips("剩余钥匙不足");
        }
        else
        {
            _keyUnlockAction?.Invoke();
            Close();
        }


    }
    /// <summary>
    /// 点击免费解锁
    /// </summary>
    public void ClickFreeUnlock()
    {
        _freeUnlockAction?.Invoke(StartCounting);
    }
    /// <summary>
    /// 点击加速
    /// </summary>
    public void ClickSpeedUp()
    {
        StaticData.OpenAd("GiftBoxUnLockingAd", (code, msg) =>
        {
            if (code == 1)
            {
                StaticData.DataDot(DotEventId.GiftBoxAdIncrease);
                _speedAction?.Invoke(RefreshTime);
            }
        });
    }
    /// <summary>
    /// 开始计时
    /// </summary>
    /// <param name="unlockTime"></param>
    void StartCounting(long unlockTime)
    {
        _unlockTime = unlockTime;
        ChangeInterface(1);
        CreationTimer();
        StartCountingTime();
    }
    /// <summary>
    /// 刷新计时器
    /// </summary>
    /// <param name="unlockTime"></param>
    void RefreshTime(long unlockTime)
    {
        _unlockTime = unlockTime;
        ChangeInterface(1);
        if (_timeCountDownComponent != null)
        {
            _timeCountDownComponent.Dispose();
            StartCountingTime();
        }
        else
        {
            CreationTimer();
            StartCountingTime();
        }

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

        }, "GiftBoxUnlocingController");
        _timeCountDownComponent.transform.SetParent(_timeBoxTra);
    }
    /// <summary>
    /// 开始计时
    /// </summary>
    public void StartCountingTime()
    {
        //计算当前服务器时间
        long CurrTimeStampServer = TimeHelper.ServerTimeStampNow;
        //计算剩余时间
        long CurrRemainTime = _unlockTime - CurrTimeStampServer;

        if (CurrRemainTime <= 0f)
        {
            _isSpeedUpFinish = true;
            Destroy(_timeCountDownComponent.gameObject);
            _hourText.text = String.Format("{0:00}", 0);
            _minuteText.text = String.Format("{0:00}", 0);
            _secondText.text = String.Format("{0:00}", 0);
            Close();
        }
        else
        {
            _timeCountDownComponent.Init(CurrRemainTime / 1000f, false, (go) =>
            {
                //时间到了
                _isSpeedUpFinish = true;
                Destroy(_timeCountDownComponent.gameObject);
                _hourText.text = String.Format("{0:00}", 0);
                _minuteText.text = String.Format("{0:00}", 0);
                _secondText.text = String.Format("{0:00}", 0);
                Close();
            },
            (remainTime) =>
            {
                //记录剩余时间
                _remainingTime = remainTime;
                _isSpeedUpFinish = false;
            });
        }

    }
    /// <summary>
    /// 关闭
    /// </summary>
    public void Close()
    {
        DestroyTime();
        UniversalTool.CancelPopAnim(_bgBoxTra, () =>
        {
            _packageDefine = null;
            ClearItem();
            UIComponent.HideUI(UIType.GiftBoxUnlocking);
        });

    }

    public void DestroyTime() {
        if (_timeCountDownComponent != null)
        {
            _timeCountDownComponent.Dispose();
            Destroy(_timeCountDownComponent.gameObject);
            _timeCountDownComponent = null;
        }
        if (_timeBoxTra.childCount > 0)
        {
            for (int i = _timeBoxTra.childCount - 1; i >= 0; i--)
            {
                Destroy(_timeBoxTra.GetChild(i).gameObject);
            }
        }
    }
    /// <summary>
    /// 设置多语言
    /// </summary>
    void SetPanelMultilingual()
    {
        _showPrizeTitleText.text = StaticData.GetMultilingual(120133);
        _ShowTimeBoxTitelText.text = StaticData.GetMultilingual(120134);
        _keyBtn.transform.Find("SurplusKey/Title").GetComponent<Text>().text = StaticData.GetMultilingual(120135);
        _speedUpBtn.transform.Find("Box/Number").GetComponent<Text>().text = StaticData.GetMultilingual(120066);
        _freeBtn.transform.Find("Box/Number").GetComponent<Text>().text = StaticData.GetMultilingual(120136);
    }
    #endregion
}
