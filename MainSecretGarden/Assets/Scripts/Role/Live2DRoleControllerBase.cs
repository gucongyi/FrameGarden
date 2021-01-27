using Live2D.Cubism.Framework;
using Live2D.Cubism.Framework.LookAt;
using Live2D.Cubism.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

/// <summary>
/// live2d 角色控制类
/// </summary>

public class Live2DRoleControllerBase : MonoBehaviour
{

    #region 变量

    /// <summary>
    /// 角色的动画控制类/对象
    /// </summary>
    protected Live2DRoleAnimControllerBase _curPlayerAnim;

    /// <summary>
    /// 玩家看向的目标
    /// </summary>
    protected CubismLookTarget _playerLookTarget;

    /// <summary>
    /// 玩家点击角色部位 检测/跟踪
    /// </summary>
    protected RaycastHitPlayer _playerClick;

    /// <summary>
    /// 角色点击是否被点击
    /// </summary>
    protected bool _roleTouchIsActive = true;

    /// <summary>
    /// 点击角色是否播放动画
    /// </summary>
    protected bool _roleTouchIsPlayAnim = true;

    /// <summary>
    /// 是否正在播放动画 用于随机播放和点击播放
    /// </summary>
    protected bool _isPlayingAnim;

    #region 随机动画播放 

    /// <summary>
    /// 随机动画间隔时间
    /// </summary>
    [SerializeField]
    protected float _randomAnimInterval = 12.0f;

    /// <summary>
    /// 计时器
    /// </summary>
    protected float _timer;

    /// <summary>
    /// 随机动画列表 闲置动画
    /// </summary>
    [SerializeField]
    protected List<PlayerAnimType> _randomAnimIdleList;


    #endregion
    /// <summary>
    /// 点击角色不播放动画回调
    /// </summary>
    public Action<HitLocation> TouchPlayerNotPlayAnimCallback;

    #region butterfly

    [SerializeField]
    protected List<ButterflyAndPlayerAnimTable> _animTable = new List<ButterflyAndPlayerAnimTable>();

    [SerializeField]
    protected bool _isNeedbutterfly = true; //是否使用蝴蝶

    protected GameObject _butterfly;
    protected Animator _butterflyAnim;
    protected bool _isPlayButterflyAnim = false;

    protected GameObject Butterflay
    {
        get
        {
            if (_butterfly == null && _isNeedbutterfly)
            {
                var tra = transform.Find("hudie");
                if (tra != null)
                    _butterfly = tra.gameObject;
            }
            return _butterfly;
        }
    }

    protected Animator ButterflyAnim
    {
        get
        {
            if (_butterflyAnim == null && Butterflay != null)
                _butterflyAnim = Butterflay.GetComponent<Animator>();
            return _butterflyAnim;
        }
    }

    #endregion

    #endregion

    #region 属性

    /// <summary>
    /// 角色的动画控制类/对象
    /// </summary>
    protected Live2DRoleAnimControllerBase CurPlayerAnim
    {
        get
        {
            if (_curPlayerAnim == null) 
            {
                _curPlayerAnim = transform.GetComponentInChildren<Live2DRoleAnimControllerBase>();
                _curPlayerAnim.SetRoleController(this);
            }
                
            return _curPlayerAnim;
        }
    }

    #endregion

    #region 函数
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRole();
    }

    private void OnEnable()
    {
        OnEnableRole();
    }

    protected virtual void Init() 
    {
        InitLookTarget();
        InitClickPlayer();
    }

    protected virtual void UpdateRole()
    {
        //
        UpdateClickPlayer();

        if (!_isPlayingAnim)
            UpdateClickLookTarget();

        //随机动画检测
        RandomAnimDetect();
    }

    protected virtual void OnEnableRole()
    {
        HideButterflay(true);

        _isPlayingAnim = false;
        //播放随机动画
        PlayRandomAnim();
    }



    #region 控制点击角色播放动画

    protected void InitClickPlayer()
    {
        _playerClick = transform.GetComponentInChildren<RaycastHitPlayer>();
        _playerClick.Init(PlayerClickCallback);
    }

    /// <summary>
    /// 角色点击回调
    /// </summary>
    /// <param name="type"></param>
    protected void PlayerClickCallback(HitLocation type) 
    {
        Debug.Log("角色点击回调 type ="+ type);
        if (_roleTouchIsPlayAnim)
        {
            NotifyPlayAnim(type);
        }
        else 
        {
            PlayerTouchEvent(type);
        }
    }

    /// <summary>
    /// 角色点击事件函数
    /// </summary>
    protected void PlayerTouchEvent(HitLocation type) 
    {
        Debug.Log("PlayerTouchEvent 角色点击事件函数 type = "+ type);
        TouchPlayerNotPlayAnimCallback?.Invoke(type);
    }

    /// <summary>
    /// 通知播放动画 点击动画
    /// </summary>
    /// <param name="type"></param>
    protected virtual void NotifyPlayAnim(HitLocation type)
    {
        
        if (_isPlayingAnim)
            return;

        switch (type)
        {
            case HitLocation.Loc_Hair:
                PlayRoleAnim(PlayerAnimDirection.Down, PlayerAnimType.Hair, true);
                break;
            case HitLocation.Loc_Clothes:
                PlayRoleAnim(PlayerAnimDirection.Down, PlayerAnimType.Clothes, true);
                break;
            case HitLocation.Loc_Bottoms:
                PlayRoleAnim(PlayerAnimDirection.Down, PlayerAnimType.Bottoms, true);
                break;
            default:
                PlayRandomAnim();
                break;
        }

    }

    private bool _isClickDetect = false;
    private Vector3 _clickDownPoint;
    /// <summary>
    /// 更新点击角色
    /// </summary>
    protected void UpdateClickPlayer()
    {
        //点击检测
        //按下
        if (Input.GetMouseButtonDown(0))
        {
            _isClickDetect = true;
            _clickDownPoint = Input.mousePosition;
        }

        if (_isClickDetect) 
        {
            if (Input.GetMouseButtonUp(0) && Vector3.Distance(_clickDownPoint, Input.mousePosition) <= 10)
            {
                _isClickDetect = false;
                //用于点击角色播放动画
                if (_roleTouchIsActive)
                    _playerClick.DoRaycast();
            }
        }
    }

    

    /// <summary>
    /// 通知激活点击
    /// </summary>
    public void NotifyActiveRoleTouch(bool isActive) 
    {
        _roleTouchIsActive = isActive;
    }

    /// <summary>
    /// 通知角色点击更新是否播放动画
    /// </summary>
    /// <param name="isPlayAnim"></param>
    public void NotifyUpdateTouchIsPlayAnim(bool isPlayAnim) 
    {
        _roleTouchIsPlayAnim = isPlayAnim;
    }


    #endregion

    #region 控制角色眼睛跟着目标旋转

    private Vector3 _mouseButtonDownPoint = Vector3.zero;

    protected void InitLookTarget()
    {
        //2021/1/8 add 角色不再看向点击点
        return;

        var lookTarget = GameObject.Find("Live2DLookTarget");
        if (lookTarget != null)
        {
            CurPlayerAnim.transform.GetComponent<CubismLookController>().Target = lookTarget;
            _playerLookTarget = lookTarget.GetComponent<CubismLookTarget>();
            _playerLookTarget.InitData(transform.position, false);
        }
    }
    /// <summary>
    /// 更新点击看向目标
    /// </summary>
    protected void UpdateClickLookTarget()
    {
        //控制是否开启角色看向目标
        if (_playerLookTarget != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _mouseButtonDownPoint = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _mouseButtonDownPoint = Vector3.zero;
            }

            //控制是否开启角色看向目标 玩家滑动距离大于5才进行目光跟随
            if (Input.GetMouseButton(0) && Vector3.Distance(Input.mousePosition, _mouseButtonDownPoint) >= 5)
            {
                if (!_playerLookTarget.IsActive())
                    _playerLookTarget.SetLookTargetActive(true);
            }
            else
            {
                if (_playerLookTarget.IsActive())
                    _playerLookTarget.SetLookTargetActive(false);
            }
        }
    }
    /// <summary>
    /// 设置看向目标是否激活
    /// </summary>
    /// <param name="isActive"></param>
    public void SetRoleLookTargetActive(bool isActive)
    {
        if (_playerLookTarget)
            _playerLookTarget.SetLookTargetActive(isActive);
    }

    #endregion

    #region 随机动画

    /// <summary>
    /// 随机动画检查
    /// </summary>
    protected virtual void RandomAnimDetect()
    {
        //
        if (_isPlayingAnim)
            return;

        _timer += Time.deltaTime;
        if (_randomAnimInterval <= _timer)
        {
            _timer = 0.0f;
            PlayRandomAnim();
        }
    }



    /// <summary>
    /// 播放随机动画
    /// </summary>
    protected virtual void PlayRandomAnim()
    {
        //
        if (_isPlayingAnim)
            return;

        //
        if (_randomAnimIdleList == null)
            _randomAnimIdleList = new List<PlayerAnimType>();

        //添加默认值
        if (_randomAnimIdleList.Count <= 0)
        {
            _randomAnimIdleList.Add(PlayerAnimType.Hair);
            _randomAnimIdleList.Add(PlayerAnimType.Clothes);
        }

        int index = UnityEngine.Random.Range(0, _randomAnimIdleList.Count);
        PlayRoleAnim(PlayerAnimDirection.Down, _randomAnimIdleList[index], true);
    }

    #endregion

    #region 播放动画

    /// <summary>
    /// 播放角色动画
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="type"></param>
    /// <param name="isPlayingAnim"> 闲置播放动作</param>
    protected virtual void PlayRoleAnim(PlayerAnimDirection dir, PlayerAnimType type, bool isPlayingAnim = false) 
    {
        _timer = 0.0f;
        _isPlayingAnim = true;
        CurPlayerAnim.PlayAnim(dir, type);
        if (!isPlayingAnim)
            _isPlayingAnim = false;

        //蝴蝶互动
        if (_isNeedbutterfly)
        {
            ButterflyAndPlayerAnimTable butterflyAndPlayerAnimTable=_animTable.Find((table) => type == table.PlayerAnim);
            string butterflyAnimName = string.Empty;
            if (butterflyAndPlayerAnimTable!=null)
            {
                butterflyAnimName = butterflyAndPlayerAnimTable.ButterflyAnimName;
            }
            if (string.IsNullOrEmpty(butterflyAnimName)) 
            {
                //隐藏蝴蝶
                HideButterflay();
                return;
            }
            ShowButterflay(butterflyAnimName);

        }
    }

    private void ShowButterflay(string butterflyAnimName) 
    {
        if (Butterflay == null)
            return;

        _isPlayButterflyAnim = true;
        Butterflay.SetActive(true);
        //Butterflay.GetComponent<CubismRenderController>().SortingOrder = -3;
        //Butterflay.transform.localScale = Vector3.zero;
        Butterflay.transform.DOScale(Vector3.one, 0.1f);//.onComplete = ()=> { Butterflay.GetComponent<CubismRenderController>().SortingOrder = 100; };
        ButterflyAnim.SetTrigger(butterflyAnimName);
    }

    /// <summary>
    /// 隐藏蝴蝶
    /// </summary>
    public void HideButterflay(bool isInit = false) 
    {
        if (Butterflay == null)
            return;

        _isPlayButterflyAnim = false;
        Butterflay.transform.localScale = Vector3.zero;
        //ButterflyAnim.StopPlayback();
        Butterflay.SetActive(false);
        //Butterflay.GetComponent<CubismRenderController>().SortingOrder = -3;
    }

    #endregion

    #region 动画事件

    /// <summary>
    /// 跳跃动画 起跳点
    /// </summary>
    public virtual void TakeoffCompleted()
    {

    }

    /// <summary>
    /// 动画播放完成
    /// </summary>
    public virtual void AnimEnd()
    {
        _isPlayingAnim = false;

        //蝴蝶互动
        if (_isPlayButterflyAnim && _isNeedbutterfly)
        {
            HideButterflay();
        }
    }

    /// <summary>
    /// 跳跃完成
    /// </summary>
    public virtual void JumpEnd()
    {
        
    }

    #endregion

    #endregion
}

[Serializable]
public class ButterflyAndPlayerAnimTable
{
    public PlayerAnimType PlayerAnim;
    public string ButterflyAnimName;
}