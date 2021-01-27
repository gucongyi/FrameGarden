using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 大富翁总控（入口）
/// </summary>
public class ZillionaireManager : MonoBehaviour
{
    #region 字段
    /// <summary>
    /// 单例字段
    /// </summary>
    public static ZillionaireManager _instance;
    /// <summary>
    /// 游戏地图管理
    /// </summary>
    ZillionaireGameMapManager _zillionaireGameMapManager = new ZillionaireGameMapManager();
    /// <summary>
    /// 游戏ui管理
    /// </summary>
    ZillionaireUIManager _zillionaireUIManager = new ZillionaireUIManager();
    /// <summary>
    /// 玩家管理器
    /// </summary>
    ZillionairePlayerManager _zillionairePlayerManager = new ZillionairePlayerManager();

    #endregion



    #region 函数
    private void Awake()
    {
        if (_instance == null || _instance != this)
        {
            _instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        InitScenes();
        Initial();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        _zillionaireGameMapManager = null;
        _zillionaireUIManager = null;
        _zillionairePlayerManager = null;
    }

    /// <summary>
    /// 场景初始化 分辨率适配
    /// </summary>
    private void InitScenes() 
    {
        float f = (float)Screen.height / (float)Screen.width;
        if (f < 1.775)
        {
            float ratio = 2688f / 1242f;
            ratio = ratio / f;
            //float ratio = 2688f / (float)Screen.height;
            transform.localScale = new Vector3(ratio, ratio, 1.0f);
        }
        else
        {

            float ratio = 2688f / 1242f;
            ratio = ratio / f;
            transform.localScale = new Vector3(ratio, ratio, 1.0f);
        }

    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Initial()
    {
        _zillionaireGameMapManager.Initial();
        _zillionairePlayerManager.Initial();
        _zillionaireUIManager.Initial();
        Debug.Log("ZillionaireManager Initial");

    }

    /// <summary>
    /// 更新激活新手引导
    /// </summary>
    public void UpdateActiveGuide() 
    {
    }

    #endregion
}


