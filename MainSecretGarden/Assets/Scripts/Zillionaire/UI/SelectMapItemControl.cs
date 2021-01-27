using Company.Cfg;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 大富翁主页选择地图列表里面的Item控制器
/// </summary>
public class SelectMapItemControl : MonoBehaviour
{
    #region 字段

    /// <summary>
    /// 背景图片
    /// </summary>
    private GameObject _backgroundImage;
    private Image _icon;
    private GameObject _name;
    private GameObject _unlock;
    private GameObject _frame;
    /// <summary>
    /// 是否为随机地图 展示图标
    /// </summary>
    private GameObject _randomMap;

    /// <summary>
    /// 是否被选中
    /// </summary>
    private bool _isSelect = false;

    /// <summary>
    /// 选中回调
    /// </summary>
    Action<int> _selectAction;

    #endregion

    #region 属性



    #endregion

    #region 函数
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// item初始化
    /// </summary>
    public void InitValue( Action<int> selectAction, bool isDefSelect = false)
    {

        InitComponent();
        _selectAction = selectAction;

        if (isDefSelect)
            SelectMap();
    }

    /// <summary>
    /// 组件初始化
    /// </summary>
    private void InitComponent() 
    {
        _backgroundImage = transform.Find("BackgroundImage").gameObject;
        _icon = _backgroundImage.transform.Find("Icon").gameObject.GetComponent<Image>();
        _name = _backgroundImage.transform.Find("Name").gameObject;
        _unlock = _backgroundImage.transform.Find("Unlock").gameObject;
        _randomMap = _backgroundImage.transform.Find("RandomMap").gameObject;
        _frame = _backgroundImage.transform.Find("Frame").gameObject;

        //事件监听
        _backgroundImage.GetComponent<Button>().onClick.RemoveAllListeners();
        _backgroundImage.GetComponent<Button>().onClick.AddListener(SelectMap);
    }

    /// <summary>
    /// 显示地图icon
    /// </summary>
    /// <param name="icon"></param>
    private void ShowMapIcon(Sprite icon) 
    {
        _icon.sprite = icon;
    }

    /// <summary>
    /// 选中地图
    /// </summary>
    private void SelectMap()
    {
        //if (!_mapData._isUnlock)
        //{
        //    //打开解锁界面/提示界面
        //    OpenBuyMap();
        //    return;
        //}
        if (_isSelect)
            return;

        _isSelect = true;
        _frame.SetActive(_isSelect);
        //_selectAction?.Invoke(_mapData._mapID);

        _backgroundImage.transform.localScale = new Vector3(1.16f, 1.16f, 1.16f);
    }


    /// <summary>
    /// 放弃选中地图
    /// </summary>
    public void UnSelectMap()
    {
        _isSelect = false;
        _frame.SetActive(_isSelect);
        _backgroundImage.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 解锁地图
    /// </summary>
    private void UnlockMap() 
    {
        _unlock.SetActive(false);
        //_mapData._isUnlock = true;
    }

    /// <summary>
    /// 打开购买地图界面
    /// </summary>
    private async void OpenBuyMap()
    {
        Debug.Log("打开购买地图界面");

        //StaticData.configExcel.ZillionaireMapData
        //ZillionaireMapDataDefine mapdata = StaticData.configExcel.GetZillionaireMapDataByID(_mapData._mapID);

        //string desc = LocalizationDefineHelper.GetStringNameById(120008);
        //desc = desc.Replace("&", "<color=yellow>" + mapdata.UnlockLevel.ToString() + "</color>");
        //获取道具图标
        //Sprite icon = await ZillionaireToolManager.LoadItemSprite(mapdata.MapBuy.ID);
        //ZillionaireUIManager._instance.OpenBuyTips(desc, icon, (int)mapdata.MapBuy.Count, ConfirmBuyMap);
    }

    /// <summary>
    /// 确认购买
    /// </summary>
    private void ConfirmBuyMap() 
    {
        //ZillionaireToolManager.NotifyServerBuyMap(_mapData._mapID, BuyMapSuccess, BuyMapFail);
    }

    /// <summary>
    /// 购买地图成功
    /// </summary>
    private void BuyMapSuccess(SCBuyProp sCBuyProp)
    {
        //地图解锁
        UnlockMap();
        SelectMap();

        //通知本地数据地图解锁了
        //StaticData.UnlockZillionaireMap(_mapData._mapID);

        //货币更新
        StaticData.UpdateBackpackProps(sCBuyProp);
    }

    /// <summary>
    /// 购买地图失败
    /// </summary>
    private void BuyMapFail()
    {
        Debug.Log("购买地图失败");
    }

    #endregion
}
