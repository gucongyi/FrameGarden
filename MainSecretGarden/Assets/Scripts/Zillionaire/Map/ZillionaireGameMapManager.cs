using Company.Cfg;
using Cysharp.Threading.Tasks;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
/// <summary>
/// 大富翁游戏地图管理 逻辑层
/// </summary>
public class ZillionaireGameMapManager
{
    #region 字段
    /// <summary>
    /// 单列字段
    /// </summary>
    public static ZillionaireGameMapManager _instance;

    /// <summary>
    /// 当前地图控制类
    /// </summary>
    private ZillionaireMapControl _zillionaireMapControl;

    #endregion

    #region 属性


    /// <summary>
    /// 当前地图控制类
    /// </summary>
    public ZillionaireMapControl CurZillionaireMapControl { get { return _zillionaireMapControl; } }
    #endregion

    #region 函数
    /// <summary>
    /// 初始化 ZillionaireManager 中初始化
    /// </summary>
    public void Initial()
    {
        if (_instance == null || _instance != this)
        {
            _instance = this;
        }
    }


    /// <summary>
    /// 加载地图预制 进入大富翁游戏
    /// </summary>
    /// <param name="data"></param>
    public async Task LoadMap(int mapID)
    {
        var mapData = StaticData.configExcel.GetZillionaireMapDataByID(mapID);

        //加载prefab
        GameObject Obj = await ABManager.GetAssetAsync<GameObject>(mapData.Prefab);
        GameObject mapObjNew = GameObject.Instantiate(Obj, ZillionaireManager._instance.transform);
        _zillionaireMapControl = mapObjNew.GetComponent<ZillionaireMapControl>();
        _zillionaireMapControl.InitValue(mapData);

        ZillionairePlayerManager._instance.PlayerEnter(_zillionaireMapControl.MapGridDic[0]);
    }

    /// <summary>
    /// 玩家离开大富翁游戏
    /// </summary>
    public void PlayerOutGame() 
    {
        _zillionaireMapControl.gameObject.SetActive(false);
        _zillionaireMapControl.DestroySelf();
        _zillionaireMapControl = null;
    }

    /// <summary>
    /// 搜寻玩家行进路径
    /// </summary>
    /// <param name="index">色子点数</param>
    public async void SearchRoute(int index, MoveType moveType = MoveType.DiceMove)
    {

        int moveStep = index;
        bool isReverseMove = moveStep > 0 ? false : true;
        List<int> path = new List<int>();
        int endPos = ZillionairePlayerManager._instance.CurrentPlayer.CurGridID;
        int max = CurZillionaireMapControl.MapGridDic.Count;

        path.Add(ZillionairePlayerManager._instance.CurrentPlayer.CurGridID);
        while (moveStep != 0)
        {
            if (moveStep > 0)
            {
                moveStep -= 1;
                endPos += 1;
            }
            else
            {
                moveStep += 1;
                endPos -= 1;
            }
            if (endPos >= max)
            {
                endPos -= max;
            }
            else if (endPos < 0)
            {
                endPos += max;
            }
            path.Add(endPos);
        }
        //设置玩家移动后的位置
        ZillionairePlayerManager._instance.CurrentPlayer.CurGridID = path[path.Count - 1];

        List<ZillionaireGameMapGridDefInfo> movingPath = new List<ZillionaireGameMapGridDefInfo>();
        foreach (var item in path)
        {
            if (ZillionaireGameMapManager._instance.CurZillionaireMapControl.MapGridDic.ContainsKey(item))
            {
                movingPath.Add(ZillionaireGameMapManager._instance.CurZillionaireMapControl.MapGridDic[item]);
            }
        }

        //为格子添加路径效果
        int activeNum = 1;
        while (activeNum < movingPath.Count)
        {
            movingPath[activeNum].ActiveTransparentCoverEffect(true);
            activeNum += 1;
            if (activeNum == movingPath.Count)
                movingPath[activeNum - 1].ShowIcon();
            await UniTask.Delay(100);
        }

        ZillionairePlayerManager._instance.PlayMove(movingPath, moveType, isReverseMove);
    }


    /// <summary>
    /// 根据id获取格子数据
    /// </summary>
    /// <returns></returns>
    public ZillionaireGameMapGridDefInfo GetGridDataByID(int id)
    {
        return _zillionaireMapControl.GetGridDataByID(id);
    }

    /// <summary>
    /// 获取购买体力信息
    /// </summary>
    /// <returns></returns>
    public StoreDefine GetBuyPowerInfo() 
    {
        return StaticData.configExcel.GetStoreByShopId(CurZillionaireMapControl.CurSelectMap.PowerBuyID);
    }

    /// <summary>
    /// 获取购买体力次数
    /// </summary>
    /// <returns></returns>
    public int GetBuyPowerTime() 
    {
        return CurZillionaireMapControl.CurSelectMap.PowerBuyTime - ZillionairePlayerManager._instance.CurrentPlayer.BuyPowerNum;
    }

    #endregion
}