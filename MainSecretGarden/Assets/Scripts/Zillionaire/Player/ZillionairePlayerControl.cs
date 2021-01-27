using Cysharp.Threading.Tasks;
using Live2D.Cubism.Framework.LookAt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 大富翁玩家控制器
/// </summary>

/// <summary>
/// 移动类型
/// </summary>
public enum MoveType 
{
    DiceMove,   //色子移动
    EventMove   //事件移动
}

public class ZillionairePlayerControl : Live2DRoleControllerBase
{
    #region 字段
    /// <summary>
    /// 玩家移动速度
    /// </summary>
    [SerializeField]
    float _moveSpeed = 0.001f;

    /// <summary>
    /// 玩家移动路径格子集合
    /// </summary>
    List<ZillionaireGameMapGridDefInfo> _moveGrids = new List<ZillionaireGameMapGridDefInfo>();

    /// <summary>
    /// 当前格子数据
    /// </summary>
    ZillionaireGameMapGridDefInfo _currGridData;


    /// <summary>
    /// 玩家移动标识
    /// </summary>
    int _moveIndex = 0;

    /// <summary>
    /// 当前玩家所处的格子id 重0开始
    /// </summary>
    private int _curGridID;

    /// <summary>
    /// 是否移动完毕
    /// </summary>
    bool _isEndMove = true;

    /// <summary>
    /// 玩家当前体力
    /// </summary>
    private int _curStamina;
    /// <summary>
    /// 玩家购买体力次数
    /// </summary>
    private int _buyPowerNum;

    /// <summary>
    /// 玩家获得等奖励 奖励id 奖励数量
    /// </summary>
    private Dictionary<int, int> _curPlayerRewards = new Dictionary<int, int>();

    /// <summary>
    /// 本次格子奖励数量
    /// </summary>
    private Dictionary<int, int> _curGridRewards = new Dictionary<int, int>();


    /// <summary>
    /// 玩家是否处于主页
    /// </summary>
    private bool isPlayerInHomePage = true;

    /// <summary>
    /// 是否为反向移动
    /// </summary>
    private bool _isReverseMove = false;

    /// <summary>
    /// 玩家当前移动类型
    /// </summary>
    private MoveType _curMoveType;

    #endregion

    #region 属性
    /// <summary>
    /// 是否移动完毕
    /// </summary>
    public bool IsEndMove { get { return _isEndMove; } }
    /// <summary>
    /// 当前格子数据
    /// </summary>
    public ZillionaireGameMapGridDefInfo CurrGridData { get { return _currGridData; } }

    /// <summary>
    /// 购买体力次数
    /// </summary>
    public int BuyPowerNum { get { return _buyPowerNum; } set { _buyPowerNum = value; } }


    /// <summary>
    /// 玩家获得等奖励 奖励id 奖励数量
    /// </summary>
    public Dictionary<int, int> CurPlayerRewards { get { return _curPlayerRewards; } }

    /// <summary>
    /// 玩家当前格子id 重0开始
    /// </summary>
    public int CurGridID { get { return _curGridID; } set { _curGridID = value; } }

    /// <summary>
    /// 玩家当前的体力值
    /// </summary>
    public int CurStamina { get { return _curStamina; } }

    /// <summary>
    /// 玩家是否处于主页
    /// </summary>
    public bool IsPlayerInHomePage { get { return isPlayerInHomePage; } set { isPlayerInHomePage = value; SetRoleLookTargetActive(isPlayerInHomePage);  } }


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
        if (IsPlayerInHomePage) 
        {
            UpdateClickPlayer();
            UpdateClickLookTarget();
        }
        //随机动画检测
        RandomAnimDetect();

    }

    private void Init()
    {
        IsPlayerInHomePage = true;

        InitLookTarget();
        InitClickPlayer();
    }



    /// <summary>
    /// 玩家移动路径
    /// </summary>
    /// <param name="points"></param>
    public void Move(List<ZillionaireGameMapGridDefInfo> grids, MoveType moveType = MoveType.DiceMove, bool isReverseMove = false)
    {
        _moveIndex = 1;
        _moveGrids.Clear();
        _moveGrids = grids;
        _isReverseMove = isReverseMove;
        _isEndMove = false;
        _curMoveType = moveType;
        StartMovePlayAnim();
    }

    private PlayerAnimType _jumpType = PlayerAnimType.Jump;

    /// <summary>
    /// 开始移动播放动画
    /// </summary>
    private void StartMovePlayAnim()
    {
        //是否为事件跳跃 最后一步需要判断跳跃事件
        _jumpType = PlayerAnimType.Jump;
        if (_moveIndex >= _moveGrids.Count - 1 && (_moveGrids[_moveIndex].IsActiveEvent || _moveGrids[_moveIndex].GridInfo.IsEvent))
            _jumpType = PlayerAnimType.EventJump;

        //播放动画
        if (!_isReverseMove)
        {
            //Debug.Log();
            PlayRoleAnim(_moveGrids[_moveIndex - 1].NextStepMoveDirection, _jumpType);
        }
        else 
        {
            PlayRoleAnim(_moveGrids[_moveIndex - 1].ReverseNextStepMoveDirection, _jumpType);
        }
    }

    /// <summary>
    /// 开始移动位置
    /// </summary>
    private void StartMovePos()
    {
        //重新显示格子图片 12-05 不在显示格子上的物品
        if (_moveIndex == 1)
            _moveGrids[0].ShowIcon();
        StartCoroutine(MovePos());
    }

    /// <summary>
    /// 移动位置
    /// </summary>
    /// <returns></returns>
    private IEnumerator MovePos()
    {
        Vector3 point = _moveGrids[_moveIndex].WorldPosition;
        Vector3 tagePoint = transform.parent.InverseTransformPoint(point);

        float speed = _moveSpeed;
        if (_jumpType == PlayerAnimType.EventJump)
            speed /= 2.0f;

        while (Vector3.Distance(transform.localPosition, tagePoint) > 0.001f)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, tagePoint, speed);
            yield return new WaitForSeconds(Time.deltaTime * (0.01f));
        }
        yield return new WaitForSeconds(Time.deltaTime * (10f));
        CurStepMoveEnd();
    }

    /// <summary>
    /// 当前这一步移动完成
    /// </summary>
    private void CurStepMoveEnd()
    {
        //
        _moveGrids[_moveIndex].ActiveTransparentCoverEffect(false);

        //更新当前格子
        _currGridData = _moveGrids[_moveIndex];

        int nextMoveIndex = _moveIndex + 1;
        if (nextMoveIndex >= _moveGrids.Count)
        {
            CurMoveCompleted();
        }
        else
        {
            _moveIndex = nextMoveIndex;
            StartMovePlayAnim();
        }
    }

    /// <summary>
    /// 本次移动完成
    /// </summary>
    private void CurMoveCompleted()
    {
        _isEndMove = true;
        //隐藏格子icon
        _moveGrids[_moveIndex].HideIcon(true);
        //移动完成需要执行
        ZillionaireUIManager._instance.PlayInterfaceControl.CurRoleMoveEnd();
         
    }

    /// <summary>
    /// 发放本次移动奖励体力 及 播放获得动画
    /// </summary>
    public void IssueCurrentRewards() 
    {
        //发放奖励 体力
        if (IsRewardPhysicalPower())
        {
            AddPower(_curGridRewards[StaticData.configExcel.GetVertical().PhysicalPower]);
        }

        //播放角色获得动画 上一个格子设定的方向对应动画 
        //if (_curGridRewards.Count != 0)
        //{
        //    PlayRoleAnim(_moveGrids[_moveIndex - 1].NextStepMoveDirection, PlayerAnimType.Great);
        //}

        //清空本次获得数据
        _curGridRewards.Clear();
    }

    /// <summary>
    /// 玩家进入到x格子上
    /// </summary>
    public void PlayerEnterGrid(ZillionaireGameMapGridDefInfo gridData, bool isDelivery = false)
    {
        _curGridID = gridData.ID;
        //隐藏格子icon
        gridData.HideIcon(true);
        EnterOrigin(gridData, false, isDelivery);
    }

    /// <summary>
    /// 进入起点 会重置玩家数据
    /// </summary>
    /// <param name="gridData"></param>
    /// <param name="isReset"></param>
    /// <param name="isDelivery"></param>
    public void EnterOrigin(ZillionaireGameMapGridDefInfo gridData, bool isReset = true, bool isDelivery = false)
    {
        if (isReset) 
            ResetPlayerData();

        SetPlayerTargetGuid(gridData);
        transform.localScale = new Vector3(0.16f, 0.16f, 1f);
        //出场动画
        if (isReset)
            PlayRoleAnim(PlayerAnimDirection.Down, PlayerAnimType.Appearance);  
        if (isDelivery)
            PlayRoleAnim(PlayerAnimDirection.Down, PlayerAnimType.Idle, true);
    }

    /// <summary>
    /// 设置玩家到目标格子
    /// </summary>
    /// <param name="gridData"></param>
    public void SetPlayerTargetGuid(ZillionaireGameMapGridDefInfo gridData) 
    {
        _currGridData = gridData;
        //隐藏格子icon
        gridData.HideIcon(false);
        Vector3 point = _currGridData.WorldPosition;
        Vector3 tagePoint = transform.parent.InverseTransformPoint(point);
        transform.localPosition = tagePoint;    
    }


    /// <summary>
    /// 重置玩家数据
    /// </summary>
    private void ResetPlayerData()
    {
        _moveIndex = 0;
        _curStamina = ZillionaireGameMapManager._instance.CurZillionaireMapControl.CurSelectMap.BasicPower;
        BuyPowerNum = 0;
        _curGridRewards.Clear();
        _curGridID = 0;
        IsPlayerInHomePage = false;
        ClearPlayerRewards();
    }


    /// <summary>
    /// 清空玩家获得的奖励
    /// </summary>
    private void ClearPlayerRewards()
    {
        _curPlayerRewards.Clear();
    }

    /// <summary>
    /// 奖励是否为体力
    /// </summary>
    /// <returns></returns>
    private bool IsRewardPhysicalPower() 
    {
        foreach (var item in _curGridRewards)
        {
            if (item.Key == StaticData.configExcel.GetVertical().PhysicalPower)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 新增玩家获得的奖励 暂时存放本地 游戏结算时一起发放
    /// </summary>
    /// <param name="itemID"> 奖励道具id </param>
    /// <param name="num">奖励道具数量</param>
    public async void AddPlayerRewards(int itemID, int num)
    {
        Debug.Log(string.Format("获得的奖励id ={0}数量num = {1}", itemID, num));
        if (_curGridRewards.ContainsKey(itemID))
        {
            _curGridRewards[itemID] += num;
        }
        else 
        {
            _curGridRewards.Add(itemID, num);
        }
        
        //体力需要及时发放 不加入总表
        if (itemID != StaticData.configExcel.GetVertical().PhysicalPower)
        {
            if (_curPlayerRewards.ContainsKey(itemID))
            {
                _curPlayerRewards[itemID] += num;
            }
            else
            {
                _curPlayerRewards.Add(itemID, num);
            }
        }
        //等待0.4s
        await UniTask.Delay(400);
        //更新界面显示获得物品显示
        ZillionaireUIManager._instance.PlayInterfaceControl.UpdateRewardsShow(itemID, _curPlayerRewards[itemID]);

    }


    /// <summary>
    /// 更新体力
    /// </summary>
    private void UpdatePower(int power)
    {
        _curStamina += power;
        _curStamina = Mathf.Clamp(_curStamina, 0, ZillionaireGameMapManager._instance.CurZillionaireMapControl.CurSelectMap.BasicPower);

        if (ZillionaireUIManager._instance.PlayInterfaceControl != null)
            ZillionaireUIManager._instance.PlayInterfaceControl.UpdatePowerShow();


    }

    /// <summary>
    /// 消耗体力
    /// </summary>
    /// <returns></returns>
    public void DepletePower()
    {
        UpdatePower(ZillionaireUIManager._instance.PlayInterfaceControl.GetCurDiceConsume() * -1); ;
    }

    /// <summary>
    /// 添加体力
    /// </summary>
    public void AddPower(int addPower)
    {
        UpdatePower(addPower);
    }


    /// <summary>
    /// 体力是否足够
    /// </summary>
    /// <returns></returns>
    public bool IsEnoughPower()
    {
        return ZillionaireUIManager._instance.PlayInterfaceControl.GetCurDiceConsume() > _curStamina ? false : true;
    }

    /// <summary>
    /// 体力是否足够最小骰子需求
    /// </summary>
    /// <returns></returns>
    public bool IsMinPower() 
    {
        return ZillionaireToolManager.GetDiceMinConsume() > _curStamina ? false : true;
    }

    #region 动画事件

    /// <summary>
    /// 跳跃动画 起跳点
    /// </summary>
    public override void TakeoffCompleted()
    {
        StartMovePos();
    }

    /// <summary>
    /// 动画播放完成
    /// </summary>
    public override void AnimEnd()
    {
        base.AnimEnd();

        //玩家随机动画播放完成回到格子方向
        //不在主页 且 为随机动画  
        //玩家还没有开始移动
        if (!IsPlayerInHomePage && _isPlayingAnim && _moveIndex != 0)  
        {
            var grid = ZillionaireGameMapManager._instance.GetGridDataByID(CurGridID);
            PlayRoleAnim(grid.NextStepMoveDirection, PlayerAnimType.Idle);
        }
    }

    /// <summary>
    /// 跳跃完成
    /// </summary>
    public override void JumpEnd()
    {
        base.JumpEnd();
        Debug.Log("跳跃完成");

    }

    #endregion


    #endregion
}
