using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 角色动画方向
/// </summary>
public enum PlayerAnimDirection
{
    Down,           //向下/正面
    LowerRight,     //右下
    UpperRight,     //右上
    LowerLeft,      //左下
    UpperLeft,      //左上
    Up              //向上/背面

}

/// <summary>
/// 角色动画类型
/// </summary>
[Serializable]
public enum PlayerAnimType
{
    Idle,           //

    Jump,           //跳跃
    EventJump,      //事件跳跃

    Great,          //棒棒哒
    Bored,          //无聊看手机

    Appearance,     //出场动画
    IdleShow,       //待机展示
    Hair,           //头发
    Clothes,        //衣服
    Bottoms,        //下装
}

/// <summary>
/// live2d 角色动画控制类 控制播放角色动画
/// </summary>

public class Live2DRoleAnimControllerBase : MonoBehaviour
{

    #region 变量

    /// <summary>
    /// 动画控制器
    /// </summary>
    protected Animator _animation;

    /// <summary>
    /// 角色方向
    /// </summary>
    protected PlayerAnimDirection _playerDir;

    /// <summary>
    /// 角色控制器
    /// </summary>
    protected Live2DRoleControllerBase _controllerBase;

    #endregion

    #region 属性
    public Animator PlayerAnimation
    {
        get
        {
            if (_animation == null)
                _animation = gameObject.GetComponent<Animator>();
            return _animation;
        }
    }
    #endregion

    #region 函数/方法
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// 设置角色控制器
    /// </summary>
    public virtual void SetRoleController(Live2DRoleControllerBase controllerBase) 
    {
        _controllerBase = controllerBase;
    }

    #region 动画播放

    /// <summary>
    /// 初始化动画控制器参数
    /// </summary>
    protected virtual void InitAnimParameter()
    {
        //PlayerAnimation.ResetTrigger("Idle");
        //PlayerAnimation.ResetTrigger("Jump");
        //PlayerAnimation.ResetTrigger("Great");
        //PlayerAnimation.ResetTrigger("Bored");
        //PlayerAnimation.ResetTrigger("Appearance");
        //PlayerAnimation.ResetTrigger("Hair");
        //PlayerAnimation.ResetTrigger("Clothes");

        PlayerAnimation.SetBool("Down", false);
        PlayerAnimation.SetBool("LowerRight", false);
        PlayerAnimation.SetBool("UpperRight", false);
        PlayerAnimation.SetBool("LowerLeft", false);
        PlayerAnimation.SetBool("UpperLeft", false);
        PlayerAnimation.SetBool("Up", false);
    }

    /// <summary>
    /// 设置动画控制器方向
    /// </summary>
    /// <param name="dir"></param>
    protected virtual void SetDirection(PlayerAnimDirection dir)
    {
        _playerDir = dir;
        switch (dir)
        {
            case PlayerAnimDirection.Down: PlayerAnimation.SetBool("Down", true); break;
            case PlayerAnimDirection.LowerRight: PlayerAnimation.SetBool("LowerRight", true); break;
            case PlayerAnimDirection.UpperRight: PlayerAnimation.SetBool("UpperRight", true); break;
            case PlayerAnimDirection.LowerLeft: PlayerAnimation.SetBool("LowerLeft", true); break;
            case PlayerAnimDirection.UpperLeft: PlayerAnimation.SetBool("UpperLeft", true); break;
            case PlayerAnimDirection.Up: PlayerAnimation.SetBool("Up", true); break;
            default: PlayerAnimation.SetBool("Down", true); break;
        }
    }

    /// <summary>
    /// 设置动画类型
    /// </summary>
    protected virtual void SetAnimType(PlayerAnimType type)
    {
        switch (type)
        {
            case PlayerAnimType.Idle: PlayerAnimation.SetTrigger("Idle"); break;

            case PlayerAnimType.Jump:
                PlayerAnimation.SetTrigger("Jump"); 
                break;
            case PlayerAnimType.EventJump: PlayerAnimation.SetTrigger("EventJump"); break;

            case PlayerAnimType.Great: PlayerAnimation.SetTrigger("Great"); break;
            case PlayerAnimType.Bored: PlayerAnimation.SetTrigger("Bored"); break;

            case PlayerAnimType.Appearance: PlayerAnimation.SetTrigger("Appearance"); break;
            case PlayerAnimType.IdleShow: PlayerAnimation.SetTrigger("IdleShow"); break;

            case PlayerAnimType.Hair: PlayerAnimation.SetTrigger("Hair"); break;
            case PlayerAnimType.Clothes: PlayerAnimation.SetTrigger("Clothes"); break;
            case PlayerAnimType.Bottoms: PlayerAnimation.SetTrigger("Bottoms"); break;

            default: PlayerAnimation.SetTrigger("Idle"); break;
        }

    }

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="type"></param>
    public virtual void PlayAnim(PlayerAnimDirection dir, PlayerAnimType type)
    {
        InitAnimParameter();
        if (dir != PlayerAnimDirection.Down)
        {
            if (type == PlayerAnimType.Appearance || type == PlayerAnimType.Hair || type == PlayerAnimType.Clothes
                || type == PlayerAnimType.Bottoms || type == PlayerAnimType.IdleShow)
            {
                Debug.LogError("播放动画类型戳我 PlayAnim dir = " + dir + " //type = " + type);
                return;
            }

        }

        Debug.Log(string.Format("播放动画类型 PlayAnim dir = {0},type = {1}", dir, type));

        SetDirection(dir);
        SetAnimType(type);
    }


    #endregion

    #region 监听动画事件

    /// <summary>
    /// 跳跃动画 起跳点
    /// </summary>
    protected virtual void TakeoffCompleted() 
    {
        Debug.Log("跳跃动画 起跳点 Live2DRoleAnimControllerBase TakeoffCompleted");
        if (_controllerBase != null)
            _controllerBase.TakeoffCompleted();
    }

    /// <summary>
    /// 动画播放完成
    /// </summary>
    protected virtual void AnimEnd() 
    {
        if (_controllerBase != null)
            _controllerBase.AnimEnd();
    }

    /// <summary>
    /// 跳跃完成
    /// </summary>
    protected virtual void JumpEnd() 
    {
        if (_controllerBase != null)
            _controllerBase.JumpEnd();
    }

    #endregion

    #endregion
}
