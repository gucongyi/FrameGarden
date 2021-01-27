using Cysharp.Threading.Tasks;
using Game.Protocal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;
/// <summary>
/// 大富翁ui管理器
/// </summary>
public class ZillionaireUIManager
{
    #region 字段

    /// <summary>
    /// 是否已经初始化
    /// </summary>
    private bool _isInitial = false;
    /// <summary>
    /// 单列字段
    /// </summary>
    public static ZillionaireUIManager _instance;
    /// <summary>
    /// 大富翁主页
    /// </summary>
    private HomePageControl _homePageControl;
    /// <summary>
    /// 大富翁游戏界面
    /// </summary>
    private PlayInterfaceControl _playInterfaceControl;


    /// <summary>
    /// 需要进入的地图id
    /// </summary>
    private int _needEnterMapID;

    #endregion

    #region 属性

    /// <summary>
    /// 大富翁游戏界面
    /// </summary>
    public PlayInterfaceControl PlayInterfaceControl { get { return _playInterfaceControl; } }

    #endregion

    #region 函数


    /// <summary>
    /// ui初始化 关卡加载调用
    /// </summary>
    public void Initial()
    {
        if (_instance == null || _instance != this)
        {
            _instance = this;
        }
        StaticData.DebugGreen("ZillionaireUIManager Initial");
    }

    /// <summary>
    /// 创建主页界面
    /// </summary>
    private async UniTask CreatHomePage()
    {
        GameObject home = await UIComponent.CreateUIAsync(UIType.UIHomePage);
        
        UIComponent.HideUI(UIType.UIHomePage);
        //初始主界面不显示
        home.SetActive(false);
        _homePageControl = home.GetComponent<HomePageControl>();
        _homePageControl.Initial();

        GameObject homeBG = await UIComponent.CreateUIAsync(UIType.UIHomePageBG, false, true);
        UIComponent.HideUI(UIType.UIHomePageBG);
        homeBG.SetActive(false);
    }

    /// <summary>
    /// 显示主页
    /// </summary>
    public async UniTask ShowHomePage()
    {
        //创建主界面
        await CreatHomePage();
        await UIComponent.CreateUIAsync(UIType.UIHomePage);
        UIComponent.CreateUI(UIType.UIHomePageBG, false, true);
        ZillionairePlayerManager._instance.ShowRole();
    }

    /// <summary>
    /// 隐藏主页
    /// </summary>
    public void HideHomePage()
    {
        UIComponent.HideUI(UIType.UIHomePage);
        UIComponent.HideUI(UIType.UIHomePageBG);
    }




    /// <summary>
    /// 显示大富翁主页 关卡加载完成 过度加载界面完成调用
    /// </summary>
    public async UniTask EnterHomePage()
    {
        //通知界面显示 包括角色
        await ShowHomePage();
    }

    

    /// <summary>
    /// 开始游戏
    /// </summary>
    /// <param name="data"></param>
    public void PlayGame(int selectMapID)
    {
        _needEnterMapID = selectMapID;
        ZillionaireToolManager.NotifyServerEnterMap(selectMapID, EnterMapCallback);
    }

    /// <summary>
    /// 进入地图回调
    /// </summary>
    /// <param name="isSuccess"></param>
    private void EnterMapCallback(bool isSuccess) 
    {
        if (isSuccess)
        {
            EnterGameCallbackSucceeded();
        }
        else 
        {
            EnterGameCallbackFailed();
        }
    }

    /// <summary>
    /// 进入游戏/地图 服务器回调成功
    /// </summary>
    private void EnterGameCallbackSucceeded()
    {
        //更新进入次数
        StaticData.UpdateZillionaireMapEnterCount(1);
        if (StaticData.GetZillionaireMapEnterCount() > StaticData.configExcel.GetVertical().DayZillionaireCount)
            StaticData.UpdateWareHouseAdmission(-1);
        _homePageControl.UpdateEnterGameShow();

        EnterGameMap(_needEnterMapID);
    }

    /// <summary>
    /// 进入游戏/地图 服务器回调失败
    /// </summary>
    private void EnterGameCallbackFailed()
    {

        Debug.Log("进入游戏/地图 服务器回调失败!");
    }

    /// <summary>
    /// 进入游戏地图
    /// </summary>
    private async void EnterGameMap(int enterMapID)
    {
        //加载地图
        await ZillionaireGameMapManager._instance.LoadMap(enterMapID);
        //播放背景音乐
        GameSoundPlayer.Instance.PlayBgMusic(MusicHelper.BgMusicRichManPlay);
        //加载页面
        HideHomePage();
        //显示战斗页面
        if (_playInterfaceControl == null)
        {
            GameObject playInterfaceObj = await UIComponent.CreateUIAsync(UIType.UIPlayInterface);
            _playInterfaceControl = playInterfaceObj.GetComponent<PlayInterfaceControl>();
            _playInterfaceControl.Initial();
        }
        else
        {
            UIComponent.CreateUI(UIType.UIPlayInterface);
            _playInterfaceControl.ResetData();
        }
    }

    /// <summary>
    /// 返回主界面
    /// </summary>
    public async UniTask ReturnHomePage()
    {
        //播放背景音乐
        GameSoundPlayer.Instance.PlayBgMusic(MusicHelper.BgMusicRichManHome);
        ZillionairePlayerManager._instance.PlayerOutGame();
        await ShowHomePage();
        UIComponent.HideUI(UIType.UIPlayInterface);
        ZillionaireGameMapManager._instance.PlayerOutGame();
    }


    #endregion
}
