using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 螃蟹控制器 
/// 1.只播放爱心动画螃蟹
/// 2.随机移动 + 随机表情 螃蟹
/// </summary>
public class CrabController : MonoBehaviour
{
    /// <summary>
    /// 螃蟹方向
    /// </summary>
    public enum CrabDirection 
    {
        LeftUp,
        LeftDown,
        RightDown
    }

    #region 变量
    private Animator _animation;

    /// <summary>
    /// 需要移动的目标点
    /// </summary>
    [SerializeField]
    private List<GameObject> _moveTargetList;

    /// <summary>
    /// 检测间隔
    /// </summary>
    [SerializeField]
    private float _detectionInterval = 2.3f;

    /// <summary>
    /// 移动速度
    /// </summary>
    [SerializeField]
    private float _moveSpeed = 0.08f;

    /// <summary>
    /// 只播放桃心
    /// </summary>
    [SerializeField]
    private bool _onlyPlayAixin = false;

    [SerializeField]
    private CrabDirection _curCrabDirection = CrabDirection.LeftDown;

    private float _useDetectionInterval = 0;
    private float _timer = 0.0f;
    private GameObject _target;
    private bool _isMoving = false;
    #endregion

    #region 属性
    private Animator PlayerAnimation
    {
        get
        {
            if (_animation == null)
                _animation = transform.Find("pangxie").GetComponent<Animator>();
            return _animation;
        }
    }
    #endregion


    #region 函数
    // Start is called before the first frame update
    void Start()
    {
        _useDetectionInterval = _detectionInterval;
        SetDirection(_curCrabDirection);
        PlayerAnimation.SetTrigger("Idle");
    }

    // Update is called once per frame
    void Update()
    {
        if (_timer >= _useDetectionInterval) 
        {
            _timer -= _useDetectionInterval;
            BehaviorDetection();
        }
        _timer += Time.deltaTime;
    }

    /// <summary>
    ///行为检测
    /// </summary>
    private void BehaviorDetection() 
    {
        //不移动
        if (_moveTargetList.Count <= 0)
        {
            PlayRandomExpressionAnim();
        }
        else 
        {
            EnterMove();
        }

        _useDetectionInterval = Random.Range(_detectionInterval-0.5f, _detectionInterval+1.2f); 

    }

    /// <summary>
    /// 播放随机表情动画
    /// </summary>
    private void PlayRandomExpressionAnim()
    {
        int index = Random.Range(0, 3);

        string triggerName = "Idel";

        switch (index) 
        {
            case 0: triggerName = "aixin"; break;
            case 1: triggerName = "keai"; break;
            case 2: triggerName = "shengqi"; break;
            default:break;
        }

        if (_onlyPlayAixin)
        {
            triggerName = "aixin";
        }
        else 
        {
            if (_moveTargetList.Count > 0 && triggerName == "aixin") 
            {
                triggerName = "keai";
            }
        }

        PlayerAnimation.SetTrigger(triggerName); 
    }

    /// <summary>
    /// 进入移动选择
    /// </summary>
    private void EnterMove() 
    {
        if (_isMoving)
            return;
        //随机目标点
        int index = Random.Range(0, _moveTargetList.Count);
        if (_target == _moveTargetList[index]) 
        {
            PlayRandomExpressionAnim();
            return;
        }
        _target = _moveTargetList[index];
        PlayerAnimation.SetTrigger("Move");
        StartCoroutine(MovePos());
    }

    /// <summary>
    /// 移动到点
    /// </summary>
    /// <returns></returns>
    private IEnumerator MovePos() 
    {
        _isMoving = true;
        Vector3 targetPoint = transform.parent.InverseTransformPoint(_target.transform.position);
        while (Vector3.Distance(transform.localPosition, targetPoint) > 0.01f) 
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPoint, _moveSpeed);
            yield return new WaitForSeconds(Time.deltaTime * (0.01f));
        }

        MoveEnd();
    }

    /// <summary>
    /// 移动完成
    /// </summary>
    private void MoveEnd() 
    {
        _isMoving = false;
        PlayRandomExpressionAnim();
    }

    /// <summary>
    /// 设置动画控制器方向
    /// </summary>
    /// <param name="dir"></param>
    private void SetDirection(CrabDirection dir)
    {
        _curCrabDirection = dir;

        PlayerAnimation.SetBool("RightDown", false);
        PlayerAnimation.SetBool("LeftDown", false);
        switch (dir)
        {
            case CrabDirection.LeftDown: PlayerAnimation.SetBool("LeftDown", true); break;
            case CrabDirection.RightDown: PlayerAnimation.SetBool("RightDown", true); break;
        }
    }

    #endregion
}
