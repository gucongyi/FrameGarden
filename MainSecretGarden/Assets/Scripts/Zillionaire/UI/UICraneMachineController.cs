using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Company.Cfg;

/// <summary>
/// 大富翁 娃娃机 抽奖界面控制
/// </summary>
public class UICraneMachineController : MonoBehaviour
{
    #region 变量
    public Sprite changePaw;
    public Sprite beforePaw;

    private Transform _bg;

    private Image _paw;
    private Image _usePaw;

    private Button _butAD;
    private Button _butFreeDraw;
    private Text _butFreeDrawText;
    private Button _butClose;

    private Text _remainingNum;

    private Image _brizeIcon;

    private CanvasGroup canvasGroup;
    private Transform craneMachine;
    /// <summary>
    /// 抽奖剩余次数
    /// </summary>
    private int _curRemainingNum = 0;

    /// <summary>
    /// 默认广告抽奖次数
    /// </summary>
    private int _defADNum = 0;

    /// <summary>
    /// 是否为广告抽奖
    /// </summary>
    private bool _isADLottery = false;


    /// <summary>
    /// 是否在抽奖状态
    /// </summary>
    private bool _isLotteryState = false;

    /// <summary>
    /// 抽奖获得物品的id
    /// </summary>
    private GoodIDCount _lotteryItem = new GoodIDCount();
    #endregion

    #region 方法

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


    private Action CloseCallback;

    private void Init()
    {
        if (_paw != null)
            return;

        _bg = transform.Find("BG");
        _paw = transform.Find("BG/Paw").GetComponent<Image>();
        _usePaw = transform.Find("BG/Paw/UsePaw").GetComponent<Image>();
        _remainingNum = transform.Find("BG/RemainingNum").GetComponent<Text>();

        _butAD = transform.Find("BG/ButAD").GetComponent<Button>();
        _butFreeDraw = transform.Find("BG/ButFreeDraw").GetComponent<Button>();
        _butFreeDrawText = transform.Find("BG/ButFreeDraw/Text").GetComponent<Text>();
        _butClose = transform.Find("BG/ButClose").GetComponent<Button>();

        _brizeIcon = transform.Find("BG/BrizeIcon").GetComponent<Image>();

        _butAD.onClick.RemoveAllListeners();
        _butAD.onClick.AddListener(OnClickAD);
        _butFreeDraw.onClick.RemoveAllListeners();
        _butFreeDraw.onClick.AddListener(OnClickConfirm);
        _butClose.onClick.RemoveAllListeners();
        _butClose.onClick.AddListener(OnClickCancel);

    }

    /// <summary>
    /// 界面初始值
    /// </summary>
    /// <param name="lotterNum"></param>
    /// <param name="callback"></param>
    public void InitValue(int lotterNum, Action callback)
    {
        _curRemainingNum = lotterNum;
        _defADNum = StaticData.configExcel.GetVertical().RichManLuckyCount;
        CloseCallback = callback;
        Init();
        UpdateRemainingNum();

        //
        ZillionaireManager._instance.UpdateActiveGuide();
    }

    /// <summary>
    /// 更新免费次数
    /// </summary>
    private void UpdateRemainingNum()
    {
        if (_curRemainingNum <= 0)
        {
            _butFreeDraw.interactable = false;
        }
        else 
        {
            _butFreeDraw.interactable = true;
        }
        _remainingNum.text = LocalizationDefineHelper.GetStringNameById(120006) + _curRemainingNum + LocalizationDefineHelper.GetStringNameById(120190);
    }

    /// <summary>
    /// 更新广告按钮
    /// </summary>
    private void UpdateADButton() 
    {
        if (_defADNum > 0)
        {
            _butAD.interactable = true;
        }
        else 
        {
            _butAD.interactable = false;
        }
    }

    #region 按钮事件

    /// <summary>
    /// 取消抽奖/关闭抽奖界面
    /// </summary>
    private async void OnClickCancel()
    {

        if (_isLotteryState)
            return;

        //取消动画
        UniversalTool.CancelUIAnimTwo(canvasGroup, craneMachine, null);
        await UniTask.Delay(TimeSpan.FromSeconds(0.35));

        CloseCallback?.Invoke();
        UIComponent.RemoveUI(UIType.UICraneMachine);
    }

    /// <summary>
    /// 开始抽奖
    /// </summary>
    private void OnClickConfirm() 
    {

        if (_curRemainingNum <= 0)
            return;
        if (_isLotteryState)
            return;
        _isLotteryState = true;
        _isADLottery = false;

        SendRequestServer();

        //关闭免费抽奖次数+更改UI文字
        _remainingNum.gameObject.SetActive(false);
        _butFreeDrawText.text = "再次走到免费";
        _butFreeDrawText.fontSize = 30;

    }

    /// <summary>
    /// 广告抽奖
    /// </summary>
    private void OnClickAD() 
    {

        if (_defADNum <= 0)
            return;
        if (_isLotteryState)
            return;
        _isLotteryState = true;
        _isADLottery = true;

        StaticData.OpenAd("CraneMachineAd", (code, msg) => {
            if (code == 1)
            {
                //todo 广告成功代码
                SendRequestServer();
                //数据打点
                StaticData.DataDot(DotEventId.CatchDollAd);
            }
            else 
            {
                //观看广告失败
                _isLotteryState = false;
                _isADLottery = false;
            }
        });

        
    }

    /// <summary>
    /// 发送请求到服务器
    /// </summary>
    private void SendRequestServer()
    {
        _lotteryItem = null;
        ZillionaireToolManager.NotifyServerChooseOrLucky(LuckyTyep.LuckyProp, null, ServerCallback);
    }

    /// <summary>
    /// 服务器回调
    /// </summary>
    /// <param name="sCLucky"></param>
    private void ServerCallback(SCLucky sCLucky)
    {
        //抽奖获取数据失败
        if (sCLucky == null) 
        {
            _isLotteryState = false;
            return;
        }

        //界面更新
        if (_isADLottery)
        {
            _defADNum -= 1;
            UpdateADButton();
        }
        else
        {
            _curRemainingNum -= 1;
            UpdateRemainingNum();
        }

        //物品返回
        if (sCLucky != null && sCLucky.Info != null && sCLucky.Info.GoodsId != 0) 
        {
            //加入临时表
            ZillionairePlayerManager._instance.CurrentPlayer.AddPlayerRewards(sCLucky.Info.GoodsId, sCLucky.Info.Count);
            //
            SetBrizeIcon(sCLucky.Info.GoodsId);
            _lotteryItem = new GoodIDCount();
            _lotteryItem.ID = sCLucky.Info.GoodsId;
            _lotteryItem.Count = sCLucky.Info.Count;
        }
        //播放效果
        PlayEffec();
    }

    /// <summary>
    /// 设置抓取的物品图片
    /// </summary>
    /// <param name="itemID"></param>
    private void SetBrizeIcon(int itemID) 
    {
        var item = StaticData.configExcel.GetGameItemByID(itemID);
        Sprite icon = ABManager.GetAsset<Sprite>(item.Icon);
        _brizeIcon.sprite = icon;
        _brizeIcon.SetNativeSize();
    }


    /// <summary>
    /// 播放抓娃娃抽奖动画
    /// </summary>
    private async void PlayEffec()
    {

        DOTween.To(() => _paw.rectTransform.sizeDelta, r => _paw.rectTransform.sizeDelta = r, new Vector2(27, 605), 0.8f).OnComplete(()=> { _bg.DOPunchPosition(new Vector3(10, 10, 10), 0.6f, 16, 1f).OnComplete(Brize); }).SetAutoKill();
        await UniTask.Delay(TimeSpan.FromSeconds(1.4));
        DOTween.To(() => _paw.rectTransform.sizeDelta, r => _paw.rectTransform.sizeDelta = r, new Vector2(27, 30), 0.6f).SetEase(Ease.Linear).SetAutoKill();

    }

    /// <summary>
    /// 被抓起的物品动画
    /// </summary>
    private void Brize()
    {
            Debug.Log("抓起物品");
            _usePaw.sprite = changePaw;
            _brizeIcon.enabled = true;
            DOTween.To(() => _brizeIcon.rectTransform.anchoredPosition, r => _brizeIcon.rectTransform.anchoredPosition = r, new Vector2(0, 400), 0.6f).SetEase(Ease.Linear).OnComplete(
                ()=> { 
                    _brizeIcon.enabled = false;
                    _brizeIcon.rectTransform.anchoredPosition = new Vector2(0, -204);
                    PlayAnimEnd();
                    /*DOTween.To(() => _brizeIcon.rectTransform.anchoredPosition, r => _brizeIcon.rectTransform.anchoredPosition = r, new Vector2(-30, -204), 2f).SetAutoKill();*/
                }).SetAutoKill();
    }

    /// <summary>
    /// 播放动画完成
    /// </summary>
    private void PlayAnimEnd() 
    {
        //开启没在抽奖状态
        _isLotteryState = false;

        //获得物品展示
        var item = StaticData.configExcel.GetGameItemByID(_lotteryItem.ID);
        string name = LocalizationDefineHelper.GetStringNameById(item.ItemName);
        Sprite icon = ABManager.GetAsset<Sprite>(item.Icon);
        StaticData.OpenEarnRewards(icon, name, (int)_lotteryItem.Count);

        //还原爪子
        _usePaw.sprite = beforePaw;
    }


    #endregion

    #endregion
}
