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
/// 奖励领取界面
/// </summary>
public class ReceiveAward : MonoBehaviour
{
    #region 字段 
    /// <summary>
    /// 遮罩点击按钮
    /// </summary>
    Button _maskBtn;
    /// <summary>
    /// 宝箱box
    /// </summary>
    Transform _treasureChestBoxTra;
    /// <summary>
    /// 宝箱icon
    /// </summary>
    Image _treasureChestIcon;
    /// <summary>
    /// 光点
    /// </summary>
    [SerializeField]
    List<CanvasGroup> _lightSpotCanvasGroups = new List<CanvasGroup>();
    /// <summary>
    /// 光效box
    /// </summary>
    RectTransform _lightingEffectRect;
    /// <summary>
    /// 宝箱背景光
    /// </summary>
    RectTransform _bgLightRect;
    /// <summary>
    /// 蝴蝶特效和分割线
    /// </summary>
    CanvasGroup _theButterflyEffectBox;
    /// <summary>
    /// 蝴蝶特效母体
    /// </summary>
    [SerializeField]
    HuDieController _huDieController;
    /// <summary>
    /// 当前创建的蝴蝶特效
    /// </summary>
    HuDieController _currHuDieController;
    /// <summary>
    /// 宝箱摇晃动画
    /// </summary>
    DOTweenAnimation _shakeDOTween;
    /// <summary>
    /// 宝箱移动
    /// </summary>
    DOTweenAnimation _treasureChestBoxTraDOTween;
    /// <summary>
    /// 奖品
    /// </summary>
    List<CSWareHouseStruct> _prizes = new List<CSWareHouseStruct>();
    /// <summary>
    /// 宝箱打开icon
    /// </summary>
    Sprite _openTreasureChestIcon;
    /// <summary>
    /// 商品展示item克隆母体
    /// </summary>
    ReceiveAwardGiftItemController _receiveAwardGiftItemController;
    /// <summary>
    /// 展示移动效果item
    /// </summary>
    ReceiveAwardGiftItemController _showMoveGiftItem;
    /// <summary>
    /// 商品展示集合
    /// </summary>
    List<ReceiveAwardGiftItemController> _receiveAwardGiftItemControllers = new List<ReceiveAwardGiftItemController>();
    /// <summary>
    /// 商品展示盒子
    /// </summary>
    Transform _giftBoxTra;
    /// <summary>
    /// 结束回调
    /// </summary>
    Action _endAction;
    /// <summary>
    /// 当前展示奖品
    /// </summary>
    int _currShowDataIndex = 0;
    /// <summary>
    /// 是否开始闪烁
    /// </summary>
    bool _isTwinkle = false;
    /// <summary>
    /// 是否初始话组件完毕
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

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// 初始化组件
    /// </summary>
    void Initial()
    {
        _maskBtn = transform.Find("Mask").GetComponent<Button>();
        _treasureChestBoxTra = transform.Find("TreasureChestBox");
        _lightingEffectRect = _treasureChestBoxTra.Find("LightingEffect").GetComponent<RectTransform>();
        _bgLightRect = _lightingEffectRect.Find("BgLight").GetComponent<RectTransform>();
        _treasureChestIcon = _treasureChestBoxTra.Find("TreasureChestIcon").GetComponent<Image>();
        _treasureChestBoxTraDOTween = _treasureChestBoxTra.GetComponent<DOTweenAnimation>();
        _shakeDOTween = _treasureChestIcon.transform.GetComponent<DOTweenAnimation>();
        _receiveAwardGiftItemController = transform.Find("GiftItem").GetComponent<ReceiveAwardGiftItemController>();
        _giftBoxTra = transform.Find("GiftBox");
        _showMoveGiftItem = _treasureChestBoxTra.Find("ShowMoveGiftItem").GetComponent<ReceiveAwardGiftItemController>();
        _theButterflyEffectBox = transform.Find("TheButterflyEffectBox").GetComponent<CanvasGroup>();
        _maskBtn.onClick.RemoveListener(MaskClick);
        _maskBtn.onClick.AddListener(MaskClick);

        _shakeDOTween.onComplete.RemoveAllListeners();
        _shakeDOTween.onComplete.AddListener(Transfiguration);

        _treasureChestBoxTraDOTween.onComplete.RemoveAllListeners();
        _treasureChestBoxTraDOTween.onComplete.AddListener(MoverEnd);
        _isInitial = true;
    }
    /// <summary>
    /// 重置宝箱
    /// </summary>
    void ResetTreasureChest()
    {
        _theButterflyEffectBox.alpha = 0;
        _treasureChestIcon.transform.localEulerAngles = Vector3.zero;
        _treasureChestBoxTra.localPosition = Vector3.zero;
        CanvasGroup boxCanvasGroup = _lightingEffectRect.GetComponent<CanvasGroup>();
        boxCanvasGroup.alpha = 0;
        for (int i = 0; i < _receiveAwardGiftItemControllers.Count; i++)
        {
            Destroy(_receiveAwardGiftItemControllers[i].gameObject);
        }
        _receiveAwardGiftItemControllers.Clear();
        _prizes.Clear();
        _currShowDataIndex = 0;
        _maskBtn.enabled = false;
    }
    /// <summary>
    /// 展示奖品
    /// </summary>
    /// <param name="goodsDatas"></param>
    /// <param name="treasureChestID">宝箱配置id</param>
    public void ShowPrize(List<CSWareHouseStruct> goodsDatas, int treasureChestID, Action endAction)
    {
        if (!_isInitial)
        {
            Initial();
        }
        ResetTreasureChest();
        _endAction = endAction;
        _prizes.AddRange(goodsDatas);
        SetTreasureChestShow(treasureChestID);

        _treasureChestBoxTraDOTween.DORestart();

    }
    /// <summary>
    /// 设置礼盒显示
    /// </summary>
    /// <param name="treasureChestID"></param>
    async void SetTreasureChestShow(int treasureChestID)
    {
        PackageDefine data = WarehouseTool.GetTreasureChestConfigData(treasureChestID);
        string closeTreasureChestIconName = data.AcceptThePrize[0];
        string OpenTreasureChestIconName = data.AcceptThePrize[1];

        Sprite closeTreasureChestIconSprite = null;
        closeTreasureChestIconSprite = await ABManager.GetAssetAsync<Sprite>(closeTreasureChestIconName);
        _treasureChestIcon.sprite = closeTreasureChestIconSprite;
        _treasureChestIcon.SetNativeSize();
        _openTreasureChestIcon = await ABManager.GetAssetAsync<Sprite>(OpenTreasureChestIconName);
    }
    /// <summary>
    /// 宝箱移动完毕
    /// </summary>
    async void MoverEnd()
    {
        CanvasGroup boxCanvasGroup = _lightingEffectRect.GetComponent<CanvasGroup>();
        await ChapterTool.FadeInFadeOut(boxCanvasGroup, 1, 10, null, () =>
        {
            _shakeDOTween.DORestart();
            if (_currHuDieController == null)
            {
                _currHuDieController = Instantiate(_huDieController);
            }
            ChapterTool.FadeInFadeOut(_theButterflyEffectBox, 1, 10);
            _isTwinkle = true;
            Twinkle();
        });
    }
    /// <summary>
    /// 变身
    /// </summary>
    void Transfiguration()
    {
        //Debug.Log("变身");
        _treasureChestIcon.sprite = _openTreasureChestIcon;
        _treasureChestIcon.SetNativeSize();
        ShowAward();
    }
    /// <summary>
    /// 展示奖励
    /// </summary>
    void ShowAward()
    {
        //Debug.Log("展示奖励");
        if (_prizes != null && _currShowDataIndex < _prizes.Count)
        {
            CSWareHouseStruct data = _prizes[_currShowDataIndex];
            //Debug.Log("奖品展示:" + data.GoodId);
            _showMoveGiftItem.transform.localPosition = Vector3.zero;
            _showMoveGiftItem.ShowPrize(data);
            _showMoveGiftItem.Show(true);

            ReceiveAwardGiftItemController item = GameObject.Instantiate(_receiveAwardGiftItemController, _giftBoxTra).GetComponent<ReceiveAwardGiftItemController>();
            item.ShowPrize(data);
            item.Show(false);
            _receiveAwardGiftItemControllers.Add(item);
            _currShowDataIndex++;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_giftBoxTra.GetComponent<RectTransform>());
            StartCoroutine(ItemShowMove(item));

        }
        else
        {
            //Debug.Log("奖品展示完毕");
            StopAllCoroutines();
            _maskBtn.enabled = true;
        }

    }
    /// <summary>
    /// 移动item
    /// </summary>
    /// <param name="tage"></param>
    /// <returns></returns>
    IEnumerator ItemShowMove(ReceiveAwardGiftItemController tage)
    {
        RectTransform rect = tage.transform.GetComponent<RectTransform>();

        Camera tageCamera = transform.parent.parent.Find("UICamera").GetComponent<Camera>();
        Vector3 vector3 = tageCamera.WorldToScreenPoint(new Vector3(rect.position.x, rect.position.y, rect.position.z));

        Vector2 tageVector = Vector3.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_showMoveGiftItem.transform.GetComponent<RectTransform>(), vector3, tageCamera, out tageVector);
        //Debug.Log("移动目标：" + tageVector);
        //Debug.Log("当前目标：" + _showMoveGiftItem.transform.localPosition);
        while (Vector3.Distance(_showMoveGiftItem.transform.localPosition, tageVector) > 0.1f)
        {
            _showMoveGiftItem.transform.localPosition = Vector3.MoveTowards(_showMoveGiftItem.transform.localPosition, tageVector, 100f);
            yield return new WaitForSeconds(0.01f);
        }
        //Debug.Log("奖品移动完毕");
        _showMoveGiftItem.Show(false);
        tage.Show(true);
        ShowAward();
    }
    /// <summary>
    /// 遮罩点击
    /// </summary>
    public void MaskClick()
    {
        StopAllCoroutines();
        _endAction?.Invoke();
        Destroy(_currHuDieController);
        _currHuDieController = null;
        UIComponent.HideUI(UIType.ReceiveAward);
    }

    public async UniTask Twinkle()
    {

        while (_isTwinkle)
        {
            int number = UnityEngine.Random.Range(0, _lightSpotCanvasGroups.Count);
            List<int> ids = GetTwinkleId(number);
            List<CanvasGroup> canvasGroups = GetCanvaGroup(ids);
            if (canvasGroups.Count > 0)
            {
                await ChapterTool.Glint(canvasGroups, 100, 0.1f, 1);
                await UniTask.Delay(TimeSpan.FromMilliseconds(70));
            }

        }
    }
    /// <summary>
    /// 获取闪烁光点id
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public List<int> GetTwinkleId(int number)
    {
        List<int> ids = new List<int>();
        int index = number;
        while (index > 0)
        {
            int id = UnityEngine.Random.Range(0, _lightSpotCanvasGroups.Count);
            while (ids.Contains(id))
            {
                id = UnityEngine.Random.Range(0, _lightSpotCanvasGroups.Count);
            }
            ids.Add(id);
            index--;
        }
        return ids;
    }

    public List<CanvasGroup> GetCanvaGroup(List<int> ids)
    {
        List<CanvasGroup> canvasGroups = new List<CanvasGroup>();
        for (int i = 0; i < ids.Count; i++)
        {
            canvasGroups.Add(_lightSpotCanvasGroups[ids[i]]);
        }
        return canvasGroups;
    }
    #endregion
}



