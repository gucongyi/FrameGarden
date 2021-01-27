
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

/// <summary>
/// 大富翁主页控制器
/// </summary>
public class DiceController : MonoBehaviour
{
    #region 变量
    /// <summary>
    /// 动画控制器
    /// </summary>
    protected Animator _animation;

    private bool _isThrow = true;

    #endregion

    #region 方法

    protected Animator PlayerAnimation
    {
        get
        {
            if (_animation == null)
                _animation = gameObject.GetComponent<Animator>();
            return _animation;
        }
    }

    /// <summary>
    /// 是否可以投掷
    /// </summary>
    public bool IsThrow { get { return _isThrow; } }

    private Action DiceAnimEnd;

    /// <summary>
    /// 通知播放骰子动画
    /// </summary>
    public void NotifyPlayDiceAnim(int num, Action diceAnimEnd) 
    {
        _isThrow = false;
        DiceAnimEnd = diceAnimEnd;
        string animName = string.Empty;
        switch (num) 
        {
            case 1: animName = "DiceNum1";  break;
            case 2: animName = "DiceNum2";  break;
            case 3: animName = "DiceNum3";  break;
            case 4: animName = "DiceNum4";  break;
            case 5: animName = "DiceNum5";  break;
            case 6: animName = "DiceNum6";  break;
            default: animName = "DiceNum3"; break;
        }

        PlayerAnimation.SetTrigger("IsThrow");
        PlayerAnimation.SetTrigger(animName);
    }

    public void InitIsThrow() 
    {
        _isThrow = true;
    }

    /// <summary>
    /// 动画播放完成
    /// </summary>
    public void AnimEnd()
    {
        Debug.Log("DiceController AnimEnd 动画播放完成");
        _isThrow = true;
        DiceAnimEnd?.Invoke();
    }

    #endregion
}
