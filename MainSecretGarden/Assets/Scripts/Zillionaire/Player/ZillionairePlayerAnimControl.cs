using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ZillionairePlayerAnimControl : Live2DRoleAnimControllerBase
{
    #region 变量

    #endregion

    #region 属性

    #endregion

    #region 函数

    // Update is called once per frame
    void Update()
    {
        
    }



    /// <summary>
    /// 初始化动画控制器参数
    /// </summary>
    protected override void InitAnimParameter()
    {
        base.InitAnimParameter();
    }

    /// <summary>
    /// 设置动画控制器方向
    /// </summary>
    /// <param name="dir"></param>
    protected override void SetDirection(PlayerAnimDirection dir)
    {
        base.SetDirection(dir);
    }

    /// <summary>
    /// 设置动画类型
    /// </summary>
    protected override void SetAnimType(PlayerAnimType type) 
    {
        base.SetAnimType(type);
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="type"></param>
    public override void PlayAnim(PlayerAnimDirection dir, PlayerAnimType type) 
    {
        base.PlayAnim(dir, type);
    }


    /// <summary>
    /// 监听动画事件 起跳
    /// </summary>
    protected override void TakeoffCompleted() 
    {
        base.TakeoffCompleted();
    }

    #endregion
}
