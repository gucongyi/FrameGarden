using Company.Cfg;
using Cysharp.Threading.Tasks;
using Game.Protocal;
using Google.Protobuf.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 任务面板工具类
/// </summary>
public static class TaskPanelTool
{
    #region 字段
    static List<SCGetTaskInfoStruct> _sCGetTaskInfoStructs = new List<SCGetTaskInfoStruct>();
    /// <summary>
    /// 是否开启任务红点
    /// </summary>
    public static bool _isOpenTaskTag = false;
    #endregion
    #region 函数
    /// <summary>
    /// 获取服务器数据
    /// </summary>
    /// <param name="endAction"></param>
    public static void GetServersData(Action<bool, List<SCGetTaskInfoStruct>> endAction)
    {
        CSEmptyGetTaskInfo cSEmptyGetTaskInfo = new CSEmptyGetTaskInfo();

        ProtocalManager.Instance().SendCSEmptyGetTaskInfo(new CSEmptyGetTaskInfo(), (data) =>
        {
            SCGetTaskInfo sCGetTaskInfo = data;
            List<SCGetTaskInfoStruct> datas = new List<SCGetTaskInfoStruct>();
            if (data != null)
            {
                for (int i = 0; i < data.TaskInfo.Count; i++)
                {
                    SCGetTaskInfoStruct datainfo = data.TaskInfo[i];
                    //datainfo.IsGet = true;
                    datas.Add(datainfo);
                }
            }
            RecordData(true, datas);
            endAction?.Invoke(true, datas);

        }, (er) =>
        {
            RecordData(false, null);
            endAction?.Invoke(false, null);
        });

    }
    /// <summary>
    /// 获取所有需要展示的任务数据
    /// </summary>
    public static Dictionary<int, TaskDefine> GetAllData()
    {
        // 所有需要展示的任务数据以id储存
        Dictionary<int, TaskDefine> allDataDic = new Dictionary<int, TaskDefine>();

        for (int i = 0; i < StaticData.configExcel.Task.Count; i++)
        {
            TaskDefine data = StaticData.configExcel.Task[i];

            if (data.IsDisplay)
            {
                if (data.TaskType == Company.Cfg.TaskType.DailyTask)
                {
                    //功能是否开启
                    if (StaticData.IsOpenFunction(10014, false))
                    {
                        if (data.TaskGrade <= StaticData.GetPlayerLevelByExp())
                        {
                            if (!allDataDic.ContainsKey(data.TaskID))
                            {
                                allDataDic.Add(data.TaskID, data);
                            }
                        }
                    }
                }
                else
                {
                    if (data.TaskGrade <= StaticData.GetPlayerLevelByExp())
                    {
                        if (!allDataDic.ContainsKey(data.TaskID))
                        {
                            allDataDic.Add(data.TaskID, data);
                        }
                    }
                }

            }
        }
        return allDataDic;
    }
    /// <summary>
    /// 领取奖励
    /// </summary>
    /// <param name="define"></param>
    /// <param name="endAction"></param>
    public static void GetTaskAward(List<TaskDefine> defines, bool isOneKey, Action<bool, SCGetTaskAward> endAction)
    {
        List<CSWareHouseStruct> datas = new List<CSWareHouseStruct>();
        Dictionary<int, CSWareHouseStruct> dataDic = new Dictionary<int, CSWareHouseStruct>();
        for (int i = 0; i < defines.Count; i++)
        {
            TaskDefine taskDefine = defines[i];
            for (int e = 0; e < taskDefine.TaskAward.Count; e++)
            {
                GoodIDCount data = taskDefine.TaskAward[e];
                if (dataDic.ContainsKey(data.ID))
                {
                    dataDic[data.ID].GoodNum = dataDic[data.ID].GoodNum + (int)data.Count;
                }
                else
                {
                    CSWareHouseStruct cSWareHouseStruct = new CSWareHouseStruct();
                    cSWareHouseStruct.GoodId = data.ID;
                    cSWareHouseStruct.GoodNum = (int)data.Count;
                    dataDic.Add(cSWareHouseStruct.GoodId, cSWareHouseStruct);
                }
            }
        }

        foreach (var item in dataDic)
        {
            datas.Add(item.Value);
        }

        StaticData.OpenCommonReceiveAwardTips(StaticData.GetMultilingual(120246), StaticData.GetMultilingual(120195), "", () =>
        {
            GetTaskAwardAction(defines, isOneKey, endAction);
        }, null, datas);

    }
    /// <summary>
    /// 领取奖励
    /// </summary>
    /// <param name="define"></param>
    /// <param name="endAction"></param>
    public static void GetTaskAwardAction(List<TaskDefine> defines, bool isOneKey, Action<bool, SCGetTaskAward> endAction)
    {
        CSGetTaskAward cSGetTaskAward = new CSGetTaskAward();
        cSGetTaskAward.IsOneKey = isOneKey;

        for (int i = 0; i < defines.Count; i++)
        {
            int id = defines[i].TaskID;
            cSGetTaskAward.TaskIDs.Add(id);
        }
        ProtocalManager.Instance().SendCSGetTaskAward(cSGetTaskAward, (data) =>
        {
            endAction?.Invoke(true, data);
            GameSoundPlayer.Instance.PlaySoundEffect(MusicHelper.SoundEffectEarnRewards);
        }, (er) =>
        {
            endAction?.Invoke(false, null);
        });
    }
    /// <summary>
    /// 跳转任务场景
    /// </summary>
    /// <param name="taskSceneTag"></param>
    public static async void SkipTaskScene(TaskSceneTag taskSceneTag)
    {
        EnumScene currScene = SceneManagerComponent._instance._currSceneTage;
        //taskSceneTag = TaskSceneTag.ManorFriends;
        switch (taskSceneTag)
        {
            case TaskSceneTag.None:
                Debug.Log("没有需要跳转的场景");
                break;
            case TaskSceneTag.SignIn:
                Debug.Log("没有需要跳转的场景");
                break;
            case TaskSceneTag.GetFavorability:
                Debug.Log("没有需要跳转的场景");
                break;
            case TaskSceneTag.Manor:
            case TaskSceneTag.DressUpTheManor:
            case TaskSceneTag.ManorFriends:
            case TaskSceneTag.Deal:
                switch (currScene)
                {
                    case EnumScene.Empty:
                        await StaticData.ToManorSelf();
                        break;
                    case EnumScene.TestPlant:
                        break;
                    case EnumScene.Manor:
                        break;
                    case EnumScene.Zillionaire:
                        await StaticData.RichManReturnToManor();
                        break;
                    case EnumScene.Party:
                        await StaticData.ToManorSelf();
                        break;
                }
                switch (taskSceneTag)
                {
                    case TaskSceneTag.DressUpTheManor:
                        await UniTask.Delay(500);
                        UIManorComponent uiManaorCom = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
                        if (uiManaorCom != null)
                        {
                            uiManaorCom.uiManorAnim.PlayAnimOpenDecorate();
                        }
                        break;
                    case TaskSceneTag.ManorFriends:
                        StaticData.OpenFriend(true);
                        break;
                    case TaskSceneTag.Deal:
                        //打开订单
                        StaticData.EnterUIDeal();
                        break;
                }
                break;
            case TaskSceneTag.ZillionaireSetout:
                //功能是否开启
                if (!StaticData.IsOpenFunction(10011))
                {
                    return;
                }
                else
                {
                    UICameraManager._instance.SetDefault();
                    UIComponent.RemoveUI(UIType.UIFriend);
                    UIComponent.RemoveUI(UIType.Warehouse);
                    UIComponent.RemoveUI(UIType.UIShop);
                    UIComponent.RemoveUI(UIType.UIManor);
                    await StaticData.OpenMonopoly();
                    StaticData.DataDot(Company.Cfg.DotEventId.EnterRichMan);
                }
                break;
            case TaskSceneTag.Friend:
            case TaskSceneTag.ChapterList:
            case TaskSceneTag.ThrobbingCasting:
                switch (currScene)
                {
                    case EnumScene.Empty:
                        break;
                    case EnumScene.TestPlant:
                        break;
                    case EnumScene.Manor:
                        break;
                    case EnumScene.Zillionaire:
                        await StaticData.ToManorSelf();
                        break;
                    case EnumScene.Party:
                        break;
                    default:
                        break;
                }
                switch (taskSceneTag)
                {
                    case TaskSceneTag.Warehouse:
                        await StaticData.OpenWareHouse();
                        break;
                    case TaskSceneTag.PropWarehouse:
                        await StaticData.OpenWareHouse();
                        break;
                    case TaskSceneTag.Friend:
                        await StaticData.OpenFriend(false);
                        break;
                    case TaskSceneTag.ZillionaireSetout:
                        break;
                    case TaskSceneTag.ChapterList:
                        //章节是否开启
                        if (!StaticData.IsOpenFunction(10007))
                            return;
                        await StaticData.OpenChapterUI();
                        break;
                    case TaskSceneTag.ThrobbingCasting:
                        //心动时刻
                        if (!StaticData.IsOpenFunction(10015))
                            return;
                        await StaticData.OpenImpulseUI();
                        break;
                    case TaskSceneTag.Shop:
                        await StaticData.OpenShopUI(0);
                        break;
                }
                break;
            case TaskSceneTag.Shop:
                await StaticData.OpenShopUI(0);
                break;
            case TaskSceneTag.Warehouse:
                await StaticData.OpenWareHouse();
                break;
            case TaskSceneTag.PropWarehouse:
                await StaticData.OpenWareHouse(2);
                break;
            case TaskSceneTag.FruitWarehouse:
                await StaticData.OpenWareHouse(1);
                break;


        }
    }
    /// <summary>
    /// 去除锚点影响获取对象的世界坐标
    /// </summary>
    /// <param name="rec"></param>
    /// <returns></returns>
    public static Vector3 ClearPivotOffset(RectTransform rec)
    {
        var offset = new Vector3((0.5f - rec.pivot.x) * rec.rect.width,
        (0.5f - rec.pivot.y) * rec.rect.height, 0.0f);
        var newPosition = rec.localPosition + offset;
        return rec.parent.TransformPoint(newPosition);
    }
    /// <summary>
    /// 记录服务器数据
    /// </summary>
    /// <param name="isSucceed"></param>
    /// <param name="datas"></param>
    public static void RecordData(bool isSucceed, List<SCGetTaskInfoStruct> datas)
    {
        if (isSucceed)
        {
            _sCGetTaskInfoStructs.Clear();
            _sCGetTaskInfoStructs.AddRange(datas);
        }
    }
    /// <summary>
    /// 初始更新任务标签
    /// </summary>
    public static void InitialUpdateTaskTag()
    {
        GetServersData(InitialUpdateTaskTagAction);
    }
    /// <summary>
    /// 初始更新标签回调
    /// </summary>
    /// <param name="isSucceed"></param>
    /// <param name="datas"></param>
    public static void InitialUpdateTaskTagAction(bool isSucceed, List<SCGetTaskInfoStruct> datas)
    {
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Task);
    }

    public static bool IsOpenDot()
    {

        bool isOpen = false;
        Dictionary<int, TaskDefine> allDataDic = GetAllData();
        Dictionary<int, SCGetTaskInfoStruct> sCGetTaskInfoStructDic = new Dictionary<int, SCGetTaskInfoStruct>();
        for (int i = 0; i < _sCGetTaskInfoStructs.Count; i++)
        {
            SCGetTaskInfoStruct infoStruct = _sCGetTaskInfoStructs[i];
            if (sCGetTaskInfoStructDic.ContainsKey(infoStruct.TaskID))
            {
                sCGetTaskInfoStructDic[infoStruct.TaskID] = infoStruct;
            }
            else
            {
                sCGetTaskInfoStructDic.Add(infoStruct.TaskID, infoStruct);
            }

        }
        int finishIndex = 0;
        foreach (var item in sCGetTaskInfoStructDic)
        {
            if (allDataDic.ContainsKey(item.Key))
            {
                if (item.Value.Schedule >= allDataDic[item.Key].FinishNum && !item.Value.IsGet)
                {
                    finishIndex = finishIndex + 1;
                }
            }
        }
        if (finishIndex >= allDataDic.Count)
        {
            isOpen = false;
            _isOpenTaskTag = false;
        }
        else
        {
            //更新状态
            foreach (var item in allDataDic)
            {
                if (sCGetTaskInfoStructDic.ContainsKey(item.Key))
                {

                    if (sCGetTaskInfoStructDic[item.Key].IsGet)
                    {
                        isOpen = true;
                        _isOpenTaskTag = true;
                        return isOpen;
                    }

                }

            }

            //没有可以领取的任务
            isOpen = false;
            _isOpenTaskTag = false;

            return isOpen;

        }
        return isOpen;
    }
    #endregion

}
/// <summary>
/// 任务面板奖励item类
/// </summary>
public class TaskPanelAwardItem
{
    #region 字段
    /// <summary>
    /// item对象实列
    /// </summary>
    RectTransform _thisRect;
    /// <summary>
    /// 图片icon
    /// </summary>
    Image _icon;
    /// <summary>
    /// 数量显示
    /// </summary>
    Text _numberText;
    /// <summary>
    /// 名称显示
    /// </summary>
    Text _nameText;
    /// <summary>
    /// item展示的数据
    /// </summary>
    GoodIDCount _showData;
    #endregion
    #region 函数
    /// <summary>
    /// 初始化itme
    /// </summary>
    /// <param name="tra"></param>
    /// <param name="data"></param>
    public void Initial(RectTransform tra, GoodIDCount data)
    {
        _thisRect = tra;
        _icon = _thisRect.Find("IconBox/Icon").GetComponent<Image>();
        _numberText = _thisRect.Find("IconBox/Number").GetComponent<Text>();
        _nameText = _thisRect.Find("Name").GetComponent<Text>();
        _showData = data;
        ShowData();
    }
    /// <summary>
    /// 展示数据
    /// </summary>
    public async void ShowData()
    {
        _numberText.text = _showData.Count.ToString();
        GameItemDefine gameItemDefine = WarehouseTool.GetGameItemData(_showData.ID);
        _nameText.text = StaticData.GetMultilingual(gameItemDefine.ItemName);
        try
        {
            _icon.sprite = null;
            _icon.sprite = await ABManager.GetAssetAsync<Sprite>(gameItemDefine.Icon);
        }
        catch (System.Exception er)
        {
            Debug.Log("奖励icon获取失败：" + gameItemDefine.ID);
        }
        _thisRect.gameObject.SetActive(true);
    }

    /// <summary>
    /// 卸载
    /// </summary>
    public void Dispose()
    {
        GameObject.Destroy(_thisRect.gameObject);
    }
    #endregion

}