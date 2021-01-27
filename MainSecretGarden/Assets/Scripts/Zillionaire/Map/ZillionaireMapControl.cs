using Company.Cfg;
using Game.Protocal;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// 大富翁地图控制器 表现层
/// </summary>
public class ZillionaireMapControl : MonoBehaviour
{
    #region 字段

    /// <summary>
    /// 格子数据字典
    /// </summary>
    Dictionary<int, ZillionaireGameMapGridDefInfo> _mapGridDic = new Dictionary<int, ZillionaireGameMapGridDefInfo>();

    /// <summary>
    /// 地图事件管理者
    /// </summary>
    private MapEventManagerNew _mapEventManager;

    /// <summary>
    /// 玩家当前选择的地图 新的 11-19 
    /// </summary>
    private ZillionaireMapDataDefine _curSelectMap;

    #endregion

    #region 属性

    /// <summary>
    /// 格子数据字典
    /// </summary>
    public Dictionary<int, ZillionaireGameMapGridDefInfo> MapGridDic { get { return _mapGridDic; } }

    /// <summary>
    /// 地图事件管理者
    /// </summary>
    public MapEventManagerNew CurMapEventManager {
        get
        {
            return _mapEventManager;
        }
    }

    /// <summary>
    /// 玩家当前选择的地图 新的 11-19 
    /// </summary>
    public ZillionaireMapDataDefine CurSelectMap { get { return _curSelectMap; } }

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

    /// <summary>
    /// 初始化地图数据
    /// </summary>
    public void InitValue(ZillionaireMapDataDefine mapData)
    {
        _curSelectMap = mapData;

        if (_mapEventManager == null)
            _mapEventManager = new MapEventManagerNew();

        //获取格子数据并赋值
        GetMapGridData();
    }

    /// <summary>
    /// 获取格子地图
    /// </summary>
    private void GetMapGridData()
    {
        _mapGridDic.Clear();

        ZillionaireLatticeDataDefine grid = null;
        //获取地图配置格子数据 //格子下标 重0开始
        for (int i = 0; i < _curSelectMap.MapGridMax.Count; i++)
        {
            grid = StaticData.configExcel.GetZillionaireLatticeDataByID(_curSelectMap.MapGridMax[i]);
            //获取格子并且初始化值
            Transform itemObj = transform.GetChild(i);//根据下标获取格子 重0开始
            ZillionaireGameMapGridDefInfo gridDefInfo = itemObj.GetComponent<ZillionaireGameMapGridDefInfo>();
            gridDefInfo.InitValue(i, grid);
            //事件
            //CurMapEventManager.AddMapEvent(item.Value.ZillionaireGameAwardId, item.Value.ZillionaireGameId - 1, this);
            _mapGridDic.Add(i, gridDefInfo);
        }
    }

    /// <summary>
    /// 根据id获取格子数据
    /// </summary>
    /// <returns></returns>
    public ZillionaireGameMapGridDefInfo GetGridDataByID(int id)
    {
        if (MapGridDic.ContainsKey(id))
            return MapGridDic[id];

        return null;
    }

    /// <summary>
    /// 自我销毁
    /// </summary>
    public void DestroySelf()
    {
        Destroy(gameObject);
    }


    #endregion
}
