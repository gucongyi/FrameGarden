using Company.Cfg;
using Game.Protocal;
using Live2D.Cubism.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 大富翁主页控制器
/// </summary>
public class HomePageControl : MonoBehaviour
{
    #region 字段

    /// <summary>
    /// 上边对齐 部分
    /// </summary>
    private Transform _top;

    /// <summary>
    /// 下边对齐 部分
    /// </summary>
    private Transform _bottom;

    /// <summary>
    /// 左边对齐 部分
    /// </summary>
    private Transform _left;

    /// <summary>
    /// 右边对齐 部分
    /// </summary>
    private Transform _right;

    /// <summary>
    /// 中间对齐 部分
    /// </summary>
    private Transform _middle;


    /// <summary>
    /// 地图列表content
    /// </summary>
    private GameObject _mapListContent;
    /// <summary>
    /// 入场卷：50
    /// </summary>
    private GameObject _admissionTicket;
    private Image _admissionTickeIcon;
    private Text _admissionTicketNum;

    /// <summary>
    /// 免费次数
    /// </summary>
    private Text _freeTimes;

    private Button _butFreeTimes;

    /// <summary>
    /// 奖励列表content
    /// </summary>
    private GameObject _rewardListContent;

    /// <summary>
    /// 奖励列表
    /// </summary>
    private Transform _rewardsTra;

    private GameObject _rewardListObj;

    /// <summary>
    /// 地图显示列表
    /// </summary>
    private Dictionary<int, SelectMapItemControl> _mapList = new Dictionary<int, SelectMapItemControl>();

    /// <summary>
    /// 选中的地图id
    /// </summary>
    private int _selectMapID = -1;

    /// <summary>
    /// 选中的角色的id
    /// </summary>
    private int _selectRoleID = 0;

    /// <summary>
    /// 购买的角色id
    /// </summary>
    private int _buyRoleID = 0;

    /// <summary>
    /// item路径
    /// </summary>
    private string _itemPath = "UIHomePageShowItem";


    #endregion 

    #region 函数

    private void OnEnable()
    {
        //if (_rewardsTra != null)
        //    UpdateMapReward();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Initial()
    {
        InitComponent();

        InitComponentAssignment();

        ZillionaireManager._instance.UpdateActiveGuide();
    }

    /// <summary>
    /// 组件初始化
    /// </summary>
    private void InitComponent()
    {
        _top = transform.Find("Top");
        _bottom = transform.Find("Bottom");
        _left = transform.Find("Left");
        _right = transform.Find("Right");
        _middle = transform.Find("Middle");

        _rewardListContent = _bottom.Find("RewardList/BG/ScrollView_Reward/Viewport/Content").gameObject;
        _rewardListObj = _bottom.Find("RewardList/BG").gameObject;

        _rewardsTra = _bottom.Find("RewardList/Rewards");

        _mapListContent = _bottom.Find("Map_BackgroundImage/ScrollView_map/Viewport/Content").gameObject;

        _admissionTicket = _bottom.Find("Image_name/AdmissionTicket").gameObject;
        _admissionTickeIcon = _admissionTicket.transform.Find("Icon").GetComponent<Image>();
        _admissionTicketNum = _admissionTicket.transform.Find("Num").GetComponent<Text>();

        _freeTimes = _bottom.Find("Image_name/FreeTimes").GetComponent<Text>();
        _butFreeTimes = _freeTimes.transform.GetComponent<Button>();
        _butFreeTimes.onClick.RemoveAllListeners();
        _butFreeTimes.onClick.AddListener(OnClickFreeTimes);

        StaticData.CreateCoinNav(_top.Find("UICurrencyInfo/GoldTra"));
        StaticData.CreateDiamondNav(_top.Find("UICurrencyInfo/DiamondTra"));
        StaticData.CreateWaterNav(_top.Find("UICurrencyInfo/WaterTra"));

        //返回及开始游戏 监听
        _top.Find("Return").GetComponent<Button>().onClick.RemoveAllListeners();
        _top.Find("Return").GetComponent<Button>().onClick.AddListener(OnClickReturn);
        _bottom.Find("StartGame").GetComponent<Button>().onClick.RemoveAllListeners();
        _bottom.Find("StartGame").GetComponent<Button>().onClick.AddListener(OnClickStartGame);

        InitAdmissionTickeIcon();
    }

    /// <summary>
    /// 免费次数 点击
    /// </summary>
    private void OnClickFreeTimes()
    {
        Debug.Log("当前点击的是免费次数显示！");
    }

    /// <summary>
    /// 组件初始化赋值
    /// </summary>
    private void InitComponentAssignment()
    {
        _selectMapID = StaticData.configExcel.ZillionaireMapData[0].ID;

        UpdateEnterGameShow();
        UpdateMapReward();
        InitRole();
    }

    #region 免费次数 + 入场卷数量

    /// <summary>
    /// 初始化入场券的icon
    /// </summary>
    private void InitAdmissionTickeIcon() 
    {
        string path = StaticData.configExcel.GetGameItemByID(StaticData.configExcel.GetVertical().AdmissionGoodsId).Icon;
        _admissionTickeIcon.sprite = ABManager.GetAsset<Sprite>(path);
    }

    /// <summary>
    /// 更新进入游戏显示 免费次数 + 入场卷数量
    /// </summary>
    public void UpdateEnterGameShow()
    {
        int freeTime = GetFreeTimesShow();
        if (freeTime > 0) //免费次数
        {
            _freeTimes.gameObject.SetActive(true);
            _admissionTicket.SetActive(false);

            _freeTimes.text = LocalizationDefineHelper.GetStringNameById(120006) + freeTime + LocalizationDefineHelper.GetStringNameById(120190);//
        }
        else //入场卷显示
        {
            _freeTimes.gameObject.SetActive(false);
            _admissionTicket.SetActive(true);
            _admissionTicketNum.text = GetAdmissionNum().ToString();

        }
    }

    /// <summary>
    /// 获取入场券数量
    /// </summary>
    /// <returns></returns>
    private int GetAdmissionNum() 
    {
        return StaticData.GetWareHouseAdmission();
    }

    /// <summary>
    /// 获取免费次数
    /// </summary>
    /// <returns></returns>
    private int GetFreeTimesShow() 
    {
        return StaticData.configExcel.GetVertical().DayZillionaireCount - StaticData.GetZillionaireMapEnterCount();
    }

    #endregion

    #region 角色部分

    /// <summary>
    /// 初始化角色
    /// </summary>
    private void InitRole()
    {
        int defRoleID = StaticData.GetDefRole();
        //生成角色
        ZillionairePlayerManager._instance.HomePagInitRole(defRoleID);
    }

    #endregion

    #region 奖励预览

    /// <summary>
    /// 更新地图奖励列表
    /// </summary>
    private async void UpdateMapReward(int mapID = 0)
    {
        RemoveRewardList();

        if (_rewardsTra.childCount > 0 || _rewardListContent.transform.childCount > 0)
            return;

        //获取奖励数据
        var mapInfo = StaticData.configExcel.GetZillionaireMapDataByID(_selectMapID);
        GameObject obj = await ABManager.GetAssetAsync<GameObject>(_itemPath);

        _rewardsTra.gameObject.SetActive(false);
        _rewardListObj.SetActive(false);
        Transform parent = null;
        if (mapInfo.RewardPreviewItems.Count <= 4)
        {
            _rewardsTra.gameObject.SetActive(true);
            parent = _rewardsTra;
        }
        else
        {
            _rewardListObj.SetActive(true);
            parent = _rewardListContent.transform;
        }

        GameObject itemObj = null;
        for (int i = 0; i < mapInfo.RewardPreviewItems.Count; i++)
        {
            itemObj = Instantiate(obj, parent);
            SetItemInfo(itemObj, mapInfo.RewardPreviewItems[i]);
        }

    }

    /// <summary>
    /// 移除奖励列表
    /// </summary>
    private void RemoveRewardList()
    {
    }


    /// <summary>
    /// 设置item信息
    /// </summary>
    /// <param name="itemObj"></param>
    /// <param name="id"></param>
    private async void SetItemInfo(GameObject itemObj, int id)
    {
        GameItemDefine itemData = StaticData.configExcel.GetGameItemByID(id);

        Sprite icon = await ZillionaireToolManager.LoadItemSpriteByIconName(itemData.Icon);
        Image itemIcon = itemObj.transform.Find("BG/Icon").GetComponent<Image>();
        itemIcon.sprite = icon;
        itemIcon.SetNativeSize();
        itemObj.transform.Find("BG/Name").GetComponent<Text>().text = LocalizationDefineHelper.GetStringNameById(itemData.ItemName);
    }
    #endregion


    #region 按钮事件 

    /// <summary>
    /// 开始游戏
    /// </summary>
    private void OnClickStartGame()
    {

        //新手引导标记完成
        if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.isCurrStepGuiding)
        {
            GuideCanvasComponent._instance.SetLittleStepFinish();
        }
        //1.可以免费入场
        if (StaticData.configExcel.GetVertical().DayZillionaireCount > StaticData.GetZillionaireMapEnterCount())
        {
            //直接进入游戏
            EnterGame();
            return;
        }
        //2.入场券提示
        if (GetAdmissionNum() > 0)
        {
            //提示设置 每日
            var timeDay = PlayerPrefs.GetString(GameUITool.GetItemSaveTipsTimeKey(StaticData.configExcel.GetVertical().AdmissionGoodsId));
            if (string.IsNullOrEmpty(timeDay) || !TimeHelper.IsTheSameDay(timeDay))
            {
                //提示
                OpenAdmissionUseTips();
                return;
            }
            else
            {
                //直接进入游戏
                EnterGame();
                return;
            }

        }
        else //入场券不足提示
        {
            OpenNotAdmissionTips();
            return;
        }
    }

    /// <summary>
    /// 入场券使用提示
    /// </summary>
    private void OpenAdmissionUseTips() 
    {
        int itemID = StaticData.configExcel.GetVertical().AdmissionGoodsId;
        Sprite icon = _admissionTickeIcon.sprite;
        int itemNum = GetAdmissionNum();
        string tips = LocalizationDefineHelper.GetStringNameById(120181); //已经没有剩余次数啦，要使用入场卷进入吗？
        string useDesc = LocalizationDefineHelper.GetStringNameById(120180);
        StaticData.OpenCommonUseTips(itemID, icon, itemNum, tips, AdmissionUseTipsCallback, useDesc);
    }

    /// <summary>
    /// 入场券使用回调
    /// </summary>
    /// <param name="useItemID"></param>
    private void AdmissionUseTipsCallback(int useItemID) 
    {
        EnterGame();
    }

    /// <summary>
    /// 入场券不足提示
    /// </summary>
    private void OpenNotAdmissionTips() 
    {
        //已经没有剩余次数和入场券啦，在庄园抓地鼠开启宝箱可以获得入场券哦
        string desc = LocalizationDefineHelper.GetStringNameById(120189);
        int idUILocalizeButtonName = 120188;//去试试
        int idUILocalizeButCancelName = 120075;//取消
        StaticData.OpenCommonTips(desc, idUILocalizeButtonName, NotAdmissionTipsCallback, null, idUILocalizeButCancelName);
    }

    /// <summary>
    /// 没有入场券提示回调
    /// </summary>
    private void NotAdmissionTipsCallback() 
    {
        //进入大富翁界面
        OnClickReturn();
    }

    /// <summary>
    /// 进入游戏
    /// </summary>
    private void EnterGame() 
    {
        ZillionaireUIManager._instance.PlayGame(_selectMapID);
    }


    /// <summary>
    /// 返回到庄园
    /// </summary>
    private async void OnClickReturn()
    {

        //await StaticData.RichManReturnLobby();
        await StaticData.RichManReturnToManor();
    }

    

    #endregion



    #endregion
}
