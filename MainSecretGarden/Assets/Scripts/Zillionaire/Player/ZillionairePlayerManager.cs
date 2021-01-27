using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 大富翁游戏玩家管理
/// </summary>
public class ZillionairePlayerManager
{
    #region 字段

    /// <summary>
    /// 单例字段
    /// </summary>
    public static ZillionairePlayerManager _instance;

    /// <summary>
    /// 当前玩家
    /// </summary>
    private ZillionairePlayerControl _currentPlayer;

    #endregion

    #region 属性

    /// <summary>
    /// 当前玩家
    /// </summary>
    public ZillionairePlayerControl CurrentPlayer { get { return _currentPlayer; } }

    #endregion

    #region 函数
    /// <summary>
    /// 初始化
    /// </summary>
    public void Initial()
    {
        if (_instance == null || _instance != this)
        {
            _instance = this;
        }
    }

    /// <summary>
    /// 主页初始话角色
    /// </summary>
    /// <param name="roleID"></param>
    public void HomePagInitRole(int roleID) 
    {
        LoadPlayer(false, roleID);
    }

    /// <summary>
    /// 玩家进入 游戏
    /// </summary>
    public void PlayerEnter(ZillionaireGameMapGridDefInfo gridData)
    {
        LoadPlayer(true, 0, gridData);
    }

    /// <summary>
    /// 显示角色 在界面展示
    /// </summary>
    public void ShowRole() 
    {
        if (_currentPlayer != null) 
        {
            _currentPlayer.gameObject.SetActive(true);
            _currentPlayer.transform.position = Vector3.zero;
            _currentPlayer.transform.localScale = new Vector3(0.66f, 0.66f, 1f);
            _currentPlayer.IsPlayerInHomePage = true;
        } 
    }

    /// <summary>
    /// 获取角色资源名称
    /// </summary>
    /// <returns></returns>
    private string GetRoleAssetName(int roleID = 0) 
    {
        if (roleID == 0)
            return UIType.ZillionairePlayer;
        return StaticData.configExcel.GetZillionaireRoleByID(roleID).Prefab;
    }

    /// <summary>
    /// 生成/加载角色
    /// </summary>
    /// <param name="isEnterGame"> 是否是进入游戏 </param>
    /// <param name="roleID"></param>
    /// <param name="gridData"></param>
    private async void LoadPlayer(bool isEnterGame, int roleID = 0, ZillionaireGameMapGridDefInfo gridData = null) 
    {
        if (_currentPlayer == null)
        {
            GameObject Obj = await ABManager.GetAssetAsync<GameObject>(GetRoleAssetName(roleID));
            GameObject playerObj = ZillionaireManager.Instantiate(Obj, ZillionaireManager._instance.transform);
            //playerObj.SetActive(true);
            _currentPlayer = playerObj.GetComponent<ZillionairePlayerControl>();
        }
        _currentPlayer.gameObject.SetActive(false);
        if (isEnterGame) 
        {
            Debug.Log("生成角色 进入大富翁游戏！");
            _currentPlayer.gameObject.SetActive(true);
            _currentPlayer.EnterOrigin(gridData);
        }
    }


    /// <summary>
    /// 玩家退出游戏
    /// </summary>
    public void PlayerOutGame() 
    {
        _currentPlayer.gameObject.SetActive(false);
    }

    /// <summary>
    /// 玩家移动
    /// </summary>
    /// <param name="zillionaireGameMapGrids"></param>
    public void PlayMove(List<ZillionaireGameMapGridDefInfo> zillionaireGameMapGrids, MoveType moveType = MoveType.DiceMove, bool isReverseMove = false)
    {
        _currentPlayer.Move(zillionaireGameMapGrids, moveType, isReverseMove);
    }
    /// <summary>
    /// 玩家是否移动完毕
    /// </summary>
    /// <returns></returns>
    public bool IsPlayerMoveEnd()
    {
        return _currentPlayer.IsEndMove;
    }
    #endregion

    /// <summary>
    /// 更加anim 获取角色控制器
    /// </summary>
    /// <param name="animControl"></param>
    /// <returns></returns>
    public ZillionairePlayerControl GetPlayerControlByAnimControl(ZillionairePlayerAnimControl animControl) 
    {
        return _currentPlayer;
    }
}

