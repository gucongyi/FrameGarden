using Company.Cfg;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 大富翁地图格子配置数据
/// </summary>
public class ZillionaireGameMapGridDefInfo : MonoBehaviour
{
    #region 变量

    /// <summary>
    /// 显示的图片
    /// </summary>
    private SpriteRenderer _iconSprite;
    //
    private GameObject _transparentCoverEffect;
    /// <summary>
    /// 随机事件格子样式
    /// </summary>
    private GameObject _randomEventGridStyle;

    private Transform _itemTransform;

    /// <summary>
    /// 默认图片
    /// </summary>
    private Sprite _defSprite = null;
    /// <summary>
    /// 随机事件图片
    /// </summary>
    private Sprite _randomEventSprite = null;


    /// <summary>
    /// 格子下一步移动方向
    /// </summary>
    [SerializeField]
    private PlayerAnimDirection _nextStepMoveDirection = PlayerAnimDirection.Down;
    /// <summary>
    /// 反向移动下一格子移动方向 
    /// </summary>
    [SerializeField]
    private PlayerAnimDirection _reverseNextStepMoveDirection = PlayerAnimDirection.Down;


    /// <summary>
    /// 格子id 格子的编号/下标 重1开始
    /// </summary>
    public int _id;

    /// <summary>
    /// 格子配置数据
    /// </summary>
    private ZillionaireLatticeDataDefine _gridInfo = null;

    private bool _isActiveEvent;

    #endregion

    #region 属性

    /// <summary>
    /// 是否激活事件 服务器随机的事件 对应字段 EventSecond
    /// </summary>
    public bool IsActiveEvent { get { return _isActiveEvent; } set { _isActiveEvent = value; } }

    /// <summary>
    /// 格子数据
    /// </summary>
    public ZillionaireLatticeDataDefine GridInfo { get { return _gridInfo; } }

    /// <summary>
    /// 显示的图片
    /// </summary>
    private SpriteRenderer IconSprite 
    {
        get 
        {
            if (_iconSprite == null)
                _iconSprite = transform.Find("vice_Item").GetComponent<SpriteRenderer>();
            return _iconSprite;
        } 
    }


    private GameObject TransparentCoverEffect
    {
        get 
        {
            if (_transparentCoverEffect == null)
                _transparentCoverEffect = transform.Find("Effect").gameObject;

            return _transparentCoverEffect;
        }

    }

    private GameObject RandomEventGridStyle 
    {
        get 
        {
            if (_randomEventGridStyle == null)
                _randomEventGridStyle = transform.Find("RandomEffect").gameObject;
            return _randomEventGridStyle;
        }
    }

    //
    private Transform ItemTransformget
    {
        get
        {
            if (_itemTransform == null)
                _itemTransform = transform.Find("vice_Item");
            return _itemTransform;
        } 
    }

    /// <summary>
    /// 格子下一步移动方向
    /// </summary>
    public PlayerAnimDirection NextStepMoveDirection { get { return _nextStepMoveDirection; } }

    /// <summary>
    /// 反向移动下一格子移动方向 
    /// </summary>
    public PlayerAnimDirection ReverseNextStepMoveDirection { get { return _reverseNextStepMoveDirection; } }

    /// <summary>
    /// 格子id
    /// </summary>
    public int ID { get { return _id; } set { _id = value; } }

    /// <summary>
    /// 格子局部坐标
    /// </summary>
    public Vector3 LocalPosition { get { return transform.localPosition; } }

    /// <summary>
    /// 格子世界坐标
    /// </summary>
    public Vector3 WorldPosition { get { return transform.position; } }

    #endregion

    #region 函数

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 初始化格子图片
    /// </summary>
    private async void InitGridIcon() 
    {

        if (GridInfo.IsEvent)
        {
            _defSprite = await ZillionaireToolManager.LoadEventZillionaireSprite(GridInfo.EventFirst);
        }
        else 
        {
            if (GridInfo.BasicReward != null && GridInfo.BasicReward.ID != 0)
            {
                _defSprite = await ZillionaireToolManager.LoadItemZillionaireSprite(GridInfo.BasicReward.ID);
            }
        }

        SetGridSprite(_defSprite);

        if (GridInfo.EventSecond != 0) 
        {
            _randomEventSprite = await ZillionaireToolManager.LoadEventZillionaireSprite(GridInfo.EventSecond);
        }
        //等待图标加载完成再显示图标
        ShowGridIcon();
    }

    /// <summary>
    /// 初始话格子显示
    /// </summary>
    /// <param name="id"> 格子的下标/id /0开始 </param>
    /// <param name="rewardID"></param>
    public void InitValue(int id, ZillionaireLatticeDataDefine gridInfo)
    {
        ID = id;
        _gridInfo  = gridInfo;

        //格子icon 大小设置到0.5
        ItemTransformget.localScale = new Vector3(0.4f, 0.4f, 1.0f);
        InitGridIcon();
        
    }

    private TweenCallback _runTween;

    /// <summary>
    /// 通知激活随机事件
    /// </summary>
    public void NotifyUpdateRandomEvent(bool isActive)
    {
        //更新效果
        Debug.Log("更新随机事件 NotifyUpdateRandomEvent _isActiveEvent = " + isActive);
        _isActiveEvent = isActive;
        RandomEventGridStyle.SetActive(isActive);
        if (isActive)
        {
            RandomEventGridBreathingAnim();
        }
        else 
        {
            if (_runTween != null)
                DOTween.Kill(_runTween);
        }
    }

    /// <summary>
    /// 随机事件格子呼吸动画1
    /// </summary>
    private void RandomEventGridBreathingAnim() 
    {
        SpriteRenderer sprite = RandomEventGridStyle.GetComponent<SpriteRenderer>();

        Color targetColor = Color.white;
        targetColor.a = 0.6f;
        _runTween = DOTween.To(() => sprite.color, color => sprite.color = color, targetColor, 0.6f).SetEase(Ease.Linear).onComplete = BreathingAnim2; 
    }

    /// <summary>
    /// 随机事件格子 呼吸动画 2
    /// </summary>
    private void BreathingAnim2() 
    {
        SpriteRenderer sprite = RandomEventGridStyle.GetComponent<SpriteRenderer>();

        Color targetColor = Color.white;
        _runTween = DOTween.To(() => sprite.color, color => sprite.color = color, targetColor, 0.6f).SetEase(Ease.Linear).onComplete = RandomEventGridBreathingAnim;
    }

    /// <summary>
    /// 显示格子装载的图片
    /// </summary>
    private void ShowGridIcon(bool isShow = true)
    {

        if (!isShow) //隐藏奖励icon
        {
            IconSprite.gameObject.SetActive(false);
            return;
        }

        if (IsActiveEvent)//是否为随机事件
        {
            SetGridSprite(_randomEventSprite);
        }
        else 
        {
            SetGridSprite(_defSprite);
        }

        if (IconSprite.sprite != null)
            ShowIconAnim();
    }

    /// <summary>
    /// 设置格子图片
    /// </summary>
    /// <param name="icon"></param>
    /// <param name="isRandomRot">是否随机方向</param>
    private void SetGridSprite(Sprite icon, bool isRandomRot = true) 
    {
        //默认隐藏
        IconSprite.gameObject.SetActive(false);
        IconSprite.sprite = icon;

        //默认设置没有旋转
        var rot = ItemTransformget.rotation;
        rot.y = 0;
        ItemTransformget.rotation = rot;
        
    }


    /// <summary>
    /// 播放拾取物品效果
    /// </summary>
    /// <param name="isLoadIcon"> 是否加载图片</param>
    public void PlayPickupItemEffect(bool isLoadIcon = false) 
    {
        //没有物品可以拾取
        if (GridInfo.BasicReward == null)
            return;

        Vector3 targetPos = Vector3.zero;
        Sprite icon = IconSprite.sprite;
        bool isEvent = false;
        if (GridInfo.IsEvent || IsActiveEvent)
        {
            var item = StaticData.configExcel.GetGameItemByID(GridInfo.BasicReward.ID);
            icon = ABManager.GetAsset<Sprite>(item.Icon);
            isEvent = true;
        }

        if (GridInfo.BasicReward.ID == StaticData.configExcel.GetVertical().GoldGoodsId
                || GridInfo.BasicReward.ID == StaticData.configExcel.GetVertical().JewelGoodsId)
            targetPos = GetItemEffectTargetPos();
        if (icon == null)
            return;


        StaticData.OpenPickupItemEffect(icon, ItemTransformget.localScale.x, GetItemEffectStartPos(), targetPos, GridInfo.BasicReward.ID, isEvent);
    }


    /// <summary>
    /// 隐藏icon
    /// </summary>
    /// <param name="isPickupItem"> 是否拾取物品</param>
    public void HideIcon(bool isPickupItem = false) 
    {
        //拾取物品效果
        if (isPickupItem && !GridInfo.IsEvent && !IsActiveEvent) 
            PlayPickupItemEffect(true);

        ShowGridIcon(false);
    }

    /// <summary>
    /// 显示icon
    /// </summary>
    public void ShowIcon() 
    {
        ShowGridIcon();
    }

    public void OnlyShowIcon() 
    {
        ShowIconAnim();
    }

    /// <summary>
    /// 显示格子动画
    /// </summary>
    private void ShowIconAnim() 
    {
        var changeColor = Color.white;
        changeColor.a = 0.0f;
        IconSprite.color = changeColor;
        IconSprite.gameObject.SetActive(true);
        float changeA = 0.0f;
        DOTween.To(() => changeA, x => { changeA = x; changeColor.a = changeA; IconSprite.color = changeColor; } , 1.0f, 0.5f);
    }

    /// <summary>
    /// 获取物品的开始位置
    /// </summary>
    /// <returns></returns>
    public Vector3 GetItemEffectStartPos() 
    {
        var localPos = ItemTransformget.position;

        if (ID == ZillionairePlayerManager._instance.CurrentPlayer.CurGridID)
            localPos.y += 2.2f * transform.parent.localScale.x * transform.parent.parent.localScale.x;

        //localPos.y += 3.2f * transform.parent.localScale.x * transform.parent.parent.localScale.x;
        //世界坐标转换为屏幕坐标
        var pos = Camera.main.WorldToScreenPoint(localPos);
        return pos; 
    }

    /// <summary>
    /// 获临时存放处
    /// </summary>
    /// <returns></returns>
    private Vector3 GetItemEffectTargetPos() 
    {
        var localPos = ZillionaireUIManager._instance.PlayInterfaceControl.GetEarnRewardsWorldPos();
        //世界坐标转换为屏幕坐标
        var pos = Camera.main.WorldToScreenPoint(localPos);
        return pos;
    }


    #region effect

    /// <summary>
    /// 是否激活格子覆盖效果
    /// </summary>
    /// <param name="isActive"></param>
    public void ActiveTransparentCoverEffect(bool isActive = true)
    {
        TransparentCoverEffect.SetActive(isActive);
    }

    #endregion



    #endregion
}
