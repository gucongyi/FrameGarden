using DG.Tweening;
using Game.Protocal;
using Live2D.Cubism.Rendering;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 晚会角色控制器
/// </summary>
public class PartyPlayerController : Live2DRoleControllerBase
{

    #region 变量

    /// <summary>
    /// 角色信息
    /// </summary>
    private SCRoomUserStruct _playerInfo;

    private Tweener _tweenerPlayer;

    private float _moveSleep = 0.4f;

    private CubismRenderController _renderController;


    #endregion

    #region 属性

    public CubismRenderController RenderController
    {
        get
        {
            if (_renderController == null)
                _renderController = transform.GetComponentInChildren<CubismRenderController>();
            return _renderController;
        }

    }

    public SCRoomUserStruct PlayerInfo 
    {
        get { return _playerInfo; }
    }

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

    public void InitValue(SCRoomUserStruct playerInfo) 
    {
        _playerInfo = playerInfo;
    }

    /// <summary>
    /// 设置玩家位置
    /// </summary>
    /// <param name="screenPos"></param>
    public void SetPlayerPos(Vector3 screenPos)
    {
        //将屏幕坐标转换到at的局部坐标中
        var target = PartyManager._instance.UseCamera.ScreenToWorldPoint(screenPos);

        SetPlayerLocalPos(target);
    }

    /// <summary>
    /// 设置玩家局部坐标
    /// </summary>
    public void SetPlayerLocalPos(Vector3 target)
    {
        //限制玩家移动范围
        //
        PlayerMove(target);

        if (_playerInfo.Uid == StaticData.Uid) 
        {
            //通知服务器角色移动
            PartyServerDockingManager.NotifyServerPlayerMove(target, PlayerMoveSuccess, PlayerMoveFailed);
        }
    }

    /// <summary>
    /// 角色移动
    /// </summary>
    /// <param name="target"></param>
    private void PlayerMove(Vector3 target) 
    {
        var moveTime = GetMoveTime(target);
        if (_tweenerPlayer != null && _tweenerPlayer.IsPlaying()) 
        {
            _tweenerPlayer.Kill();
        }
        _tweenerPlayer = transform.DOMove(target, moveTime);
        //更新角色移动时间 表示当前是否有角色正在移动
        PartyManager._instance.UpdatePlayerMovingTime(moveTime);
        
        //相机跟随玩家操控的角色
        if (_playerInfo.Uid == StaticData.Uid)
        {
            PartyManager._instance.CamaraFollow(target, moveTime);
        }
    }

    /// <summary>
    /// 移动成功
    /// </summary>
    private void PlayerMoveSuccess() 
    {

    }

    /// <summary>
    /// 移动失败
    /// </summary>
    private void PlayerMoveFailed() 
    {

    }


    /// <summary>
    /// 获取移动时间
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private float GetMoveTime(Vector3 target) 
    {
        return Vector3.Distance(transform.position, target) / _moveSleep;
        //return (transform.position - target).magnitude / _moveSleep;
    }

    #endregion


}
