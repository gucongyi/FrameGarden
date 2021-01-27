using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动画随机播放器
/// 用于控制随机播放动画 Anim
/// </summary>
public class RandomPlayAnimController : MonoBehaviour
{

    #region 变量

    private Animator _animation;

    [SerializeField]
    private string _live2DName;

    [SerializeField]
    private int _live2DAnimNum = 2;

    [SerializeField]
    private string _defAnimTriggerName = "Anim";

    /// <summary>
    /// 检测间隔
    /// </summary>
    [SerializeField]
    private float _detectionInterval = 12.1f;

    private float _useDetectionInterval = 0;
    private float _timer = 0.0f;

    #endregion

    #region 属性
    private Animator PlayerAnimation
    {
        get
        {
            if (_animation == null)
                _animation = transform.Find(_live2DName).GetComponent<Animator>();
            return _animation;
        }
    }
    #endregion

    #region 函数

    // Start is called before the first frame update
    void Start()
    {
        _useDetectionInterval = _detectionInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (_timer >= _useDetectionInterval)
        {
            _timer -= _useDetectionInterval;
            PlayRandomAnim();
        }
        _timer += Time.deltaTime;
    }

    /// <summary>
    /// 播放随机动画
    /// </summary>
    private void PlayRandomAnim()
    {
        int index = Random.Range(0, _live2DAnimNum);
        index += 1;
        PlayerAnimation.SetTrigger(_defAnimTriggerName + index.ToString());
        //间隔时间随机
        _useDetectionInterval = Random.Range(_detectionInterval - 0.5f, _detectionInterval + 1.2f);
    }
    #endregion
}
