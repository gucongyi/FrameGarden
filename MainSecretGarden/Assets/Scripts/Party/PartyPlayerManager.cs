using Game.Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 晚会角色管理器
/// </summary>
public class PartyPlayerManager
{
    #region 变量

    private Transform _mapTra;

    private PartyPlayerController _player;

    private List<PartyPlayerController> _roleList = new List<PartyPlayerController>();

    /// <summary>
    /// 移动中 //表示当前是否有角色正在移动
    /// </summary>
    private float _movingTime = 0.0f;


    #endregion

    #region 属性
    public float MovingTime 
    {
        get { return _movingTime; }
        set { 
            if (value > _movingTime)
                _movingTime = value;
        }
    }
    #endregion

    #region 函数

    /// <summary>
    /// 移动计时
    /// </summary>
    public void MoveTimer(float deltaTime) 
    {
        if (_movingTime <= 0)
            return;
        _movingTime -= deltaTime;
        if (_movingTime < 0)
            _movingTime = 0;
    }

    /// <summary>
    /// 生成玩家列表
    /// </summary>
    public void SpawnRoleList(SCEntranceRoom sCEntranceRoom, Transform mapTra) 
    {
        _mapTra = mapTra;

        if (sCEntranceRoom != null && sCEntranceRoom.SCRoomUserInfo != null) 
        {
            Debug.Log("生成玩家列表 sCEntranceRoom.SCRoomUserInfo.Count:"+ sCEntranceRoom.SCRoomUserInfo.Count);
            for (int i = 0; i < sCEntranceRoom.SCRoomUserInfo.Count; i++)
            {
                SpawnRole(sCEntranceRoom.SCRoomUserInfo[i]);
            }
        }
        //生成玩家自己的角色
        SCRoomUserStruct info = new SCRoomUserStruct();
        info.Uid = StaticData.Uid;
        info.Name = StaticData.playerInfoData.userInfo.Name;
        //info.CostumeId = ;
        //info.RoomId = ;
        SpawnRole(info);
    }

    /// <summary>
    /// 生成角色
    /// </summary>
    /// <param name="playerInfo"></param>
    private async void SpawnRole(SCRoomUserStruct playerInfo) 
    {
        string path = null;
        if (string.IsNullOrEmpty(path))
            path = "PartyNvDef";
        GameObject Obj = await ABManager.GetAssetAsync<GameObject>(path);
        GameObject roleObj = PartyManager._instance.SpawnRole(Obj, _mapTra);
        var curSelectedRole = roleObj.GetComponent<PartyPlayerController>();
        //
        curSelectedRole.InitValue(playerInfo);
        roleObj.transform.localPosition = new Vector3(playerInfo.Xaxle, playerInfo.Yaxle, 0f);
        //curSelectedRole.SetPlayerPos(new Vector3(playerInfo.Xaxle, playerInfo.Yaxle, 0f));
        _roleList.Add(curSelectedRole);

        //设置玩家自己
        if (playerInfo.Uid == StaticData.Uid)
            _player = curSelectedRole;
    }

    /// <summary>
    /// 角色移动
    /// </summary>
    /// <param name="screenPos">屏幕位置</param>
    public void PlayerMove(Vector3 screenPos) 
    {
        Debug.Log("屏幕位置:"+ screenPos);
        if (_player == null)
            return;
        _player.SetPlayerPos(screenPos);
    }

    /// <summary>
    /// 对角色进行排序
    /// </summary>
    public void RoleRanking()
    {
        if (_roleList.Count <= 1 || _movingTime == 0)
            return;
        _roleList.Sort((r1, r2) =>
        {
            return r1.transform.localPosition.y < r2.transform.localPosition.y ? -1 : 1;
        });

        for (int i = 0; i < _roleList.Count; i++)
        {
            _roleList[i].RenderController.SortingOrder = 99 - i;
        }
        Debug.Log("对角色进行排序");
    }


    /// <summary>
    /// 查询玩家
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    private PartyPlayerController FindPlayer(long uid)
    {
        return _roleList.Find((playerCon) => playerCon.PlayerInfo.Uid == uid ? true : false);
    }

    #region 推送事件

    /// <summary>
    /// 推送有新玩家加入
    /// </summary>
    /// <param name="enterPlayer"></param>
    public void PushNewPlayerJoin(SCEntranceRoomInfo enterPlayer) 
    {
        SCRoomUserStruct playerInfo = new SCRoomUserStruct();
        playerInfo.Uid = enterPlayer.Uid;
        playerInfo.Name = enterPlayer.Name;
        playerInfo.RoomId = enterPlayer.RoomId;
        playerInfo.CostumeId = enterPlayer.CostumeId;
        playerInfo.Xaxle = 0;
        playerInfo.Yaxle = 0;

        SpawnRole(playerInfo);
    }

    /// <summary>
    /// 推送玩家移动
    /// </summary>
    /// <param name="moveInfo"></param>
    public void PushPlayerMove(SCMoveLocation moveInfo) 
    {
        var playerCon = FindPlayer(moveInfo.Uid);
        if (playerCon != null) 
        {
            playerCon.SetPlayerLocalPos(new Vector3(moveInfo.Xaxle, moveInfo.Yaxle, 0f)); 
        }
    }

    /// <summary>
    /// 推送有玩家退出房间
    /// </summary>
    public void PushPlayerQuitRoom(SCDepartureRoom quitInfo) 
    {
        //销毁退出房间的玩家
        var playerCon = FindPlayer(quitInfo.Uid);
        if (playerCon != null)
        {
            _roleList.Remove(playerCon);
            PartyManager._instance.DestroyPlayer(playerCon.gameObject);
        }
    }

    /// <summary>
    /// 推送晚会结束
    /// </summary>
    public void PushPartyEnd() 
    {
        //销毁全部玩家
        GameObject obj = null;
        while (_roleList.Count > 0) 
        {
            obj = _roleList[0].gameObject;
            _roleList.RemoveAt(0);
            PartyManager._instance.DestroyPlayer(obj);
        }
    }

    #endregion

    #endregion
}
