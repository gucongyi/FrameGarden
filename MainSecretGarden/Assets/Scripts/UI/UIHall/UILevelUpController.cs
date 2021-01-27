using Company.Cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 等级提升界面 控制
/// </summary>
public class UILevelUpController : MonoBehaviour
{
    private enum ReturnOpenType 
    {
        Open_None,
        Open_Chapter,
        Open_Mall,
        Open_UnLockRegion
    }

    #region 变量

    private Transform _bgTra;
    private Button _butClose;

    private Text _curLv;

    private Transform _rewardsContent;
    private Transform _miniList;

    private Transform _seedMiniList;
    private GameObject _seedItem;
    private RawImage _effect;

    private GameObject _butterfly;

    /// <summary>
    /// item路径
    /// </summary>
    private string _itemPath = "UIPublicItem";

    /// <summary>
    /// 返回打开界面类型
    /// </summary>
    private ReturnOpenType _returnOpenType = ReturnOpenType.Open_None;

    private int _unLockRegionId = -1;
    private int _maxUnLockChapter = 0;
    private List<GameItemDefine> _unlockCrops = new List<GameItemDefine>();

    #endregion

    #region 方法/函数

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {
        _bgTra = transform.Find("BG_Image");
        if (_bgTra != null)
            UniversalTool.ReadyPopupAnim(_bgTra);
        Init();
    }

    private void OnEnable()
    {
        SpawnHuDie();
        if (_bgTra != null)
            UniversalTool.StartPopupAnim(_bgTra);
    }

    private void OnDisable()
    {
        if (_bgTra != null)
            UniversalTool.ReadyPopupAnim(_bgTra);
    }


    private void Init()
    {
        if (_curLv != null)
            return;

        _effect = transform.Find("BG_Image/Butterfly").GetComponent<RawImage>();

        _butClose = transform.Find("BG").GetComponent<Button>();
        _curLv = transform.Find("BG_Image/LvBG/CurLv").GetComponent<Text>();
        _rewardsContent = transform.Find("BG_Image/RawardsScrollView/Viewport/Content");
        _miniList = transform.Find("BG_Image/MiniList");

        _seedMiniList = transform.Find("BG_Image/SeedBG/SeedMiniList"); 
        _seedItem = transform.Find("BG_Image/Seed").gameObject;

        _butClose.onClick.RemoveAllListeners();
        _butClose.onClick.AddListener(() => { 
            UniversalTool.CancelPopAnim(_bgTra, OnClickClose); });

    }

    /// <summary>
    /// 初始化值
    /// </summary>
    /// <param name="lastLv"></param>
    /// <param name="curLv"></param>
    /// <param name="willUnLockRegionId"> 需停药解锁的区域id</param>
    public void InitValue(int lastLv, int curLv, int willUnLockRegionId = -1)
    {
        //角色升级获得 2020-11-27
        //1.章节开启
        //2.奖励道具
        //3.仓库格子
        //4.商店种子购买权限

        Init();

        _curLv.text = curLv.ToString();
        //增加的仓库数量
        int addWarehouseCount = 0;
        //获得奖励
        List<GoodIDCount> rewards = GetLevelUpRewards(lastLv, curLv, ref addWarehouseCount);
        SpawnRewards(rewards);

        _unLockRegionId = willUnLockRegionId;

        //章节开启
        int unLockChapter = 0;
        IsUnlockChapter(curLv, ref unLockChapter, ref _maxUnLockChapter);

        //商店种子购买权限
        _unlockCrops.Clear();
        IsUnlockCrop(lastLv, curLv, ref _unlockCrops);
        //
        SpawnSeedShow();

        _returnOpenType = ReturnOpenType.Open_None;
        if (willUnLockRegionId != -1)
        {
            _returnOpenType = ReturnOpenType.Open_UnLockRegion;
        } else if (_unlockCrops.Count > 0) 
        {
            //_returnOpenType = ReturnOpenType.Open_Mall;
        }
        else if (unLockChapter < _maxUnLockChapter)
        {
            _returnOpenType = ReturnOpenType.Open_Chapter;
        }

        //升级奖励入库
        StaticData.LevelUpRewardEntrance(rewards, addWarehouseCount, _maxUnLockChapter);
    }

    /// <summary>
    /// 生成显示的种子
    /// </summary>
    private async void SpawnSeedShow() 
    {
        for (int i = 0; i < _seedMiniList.childCount; i++)
        {
            Destroy(_seedMiniList.GetChild(i).gameObject);
        }

        GameObject itemObj = null;
        GameItemDefine itemData = null;
        for (int i = 0; i < _unlockCrops.Count; i++)
        {
            itemData = StaticData.configExcel.GetGameItemByID(_unlockCrops[i].ID);
            Sprite icon = await ZillionaireToolManager.LoadItemSpriteByIconName(itemData.Icon);
            string name = LocalizationDefineHelper.GetStringNameById(itemData.ItemName);

            itemObj = Instantiate(_seedItem, _seedMiniList);
            itemObj.transform.Find("Icon").GetComponent<Image>().sprite = icon;
            itemObj.transform.Find("NameBG/Name").GetComponent<Text>().text = name;
            itemObj.SetActive(true);
        }
    }


    /// <summary>
    /// 种子是否可以在商城购买
    /// </summary>
    /// <param name="seedID"></param>
    /// <returns></returns>
    private bool SeedInStore(int seedID) 
    {
        foreach (var item in StaticData.configExcel.Store)
        {
            if (item.GoodId == seedID)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 是否有解锁的种子购买权限
    /// </summary>
    /// <param name="lastLv"></param>
    /// <param name="curLv"></param>
    /// <returns></returns>
    private bool IsUnlockCrop(int lastLv, int curLv, ref List<GameItemDefine> unlockCrops) 
    {
        bool isReturn = false;
        foreach (var item in StaticData.configExcel.GameItem)
        {
            if (item.ItemType != TypeGameItem.Seed)
                continue;
            if (item.Grade <= curLv && item.Grade > lastLv && SeedInStore(item.ID))
            {
                unlockCrops.Add(item);
                isReturn = true;
            }
        }

        return isReturn;
    }

    /// <summary>
    /// 是否有解锁章节
    /// </summary>
    /// <param name="curLv"></param>
    /// <returns></returns>
    private bool IsUnlockChapter(int curLv, ref int curMaxChapter, ref int unLockMaxChapter) 
    {
        int maxChapter = StaticData.playerInfoData.userInfo.UnlockSectionId;
        string chapterName = string.Empty;
        unLockMaxChapter = maxChapter;
        foreach (var item in StaticData.configExcel.Section)
        {
            if (maxChapter >= item.SectionId)
                continue;

            if (item.SectionGrade <= curLv)
            {
                if (unLockMaxChapter < item.SectionId)
                    unLockMaxChapter = item.SectionId;
            }
        }
        curMaxChapter = maxChapter;
        if (unLockMaxChapter == maxChapter)
            return false;
        return true;
    }


    /// <summary>
    /// 生成奖励物品
    /// </summary>
    /// <param name="rewards"></param>
    private async void SpawnRewards(List<GoodIDCount> rewards) 
    {
        if (rewards == null || rewards.Count <= 0)
            return;

        //移除上次的物品
        for (int i = 0; i < _rewardsContent.childCount; i++)
        {
            Destroy(_rewardsContent.GetChild(i).gameObject);
        }
        for (int i = 0; i < _miniList.childCount; i++)
        {
            Destroy(_miniList.GetChild(i).gameObject);
        }

        Transform parent = null;
        //
        if (rewards.Count <= 4)
        {
            parent = _miniList;
            _miniList.gameObject.SetActive(true);
            _rewardsContent.parent.parent.gameObject.SetActive(false);
        }
        else 
        {
            parent = _rewardsContent;
            _miniList.gameObject.SetActive(false);
            _rewardsContent.parent.parent.gameObject.SetActive(true);
        }

        GameObject obj = await ABManager.GetAssetAsync<GameObject>(_itemPath);

        GameObject itemObj = null;
        foreach (var item in rewards)
        {
            itemObj = Instantiate(obj, parent);
            itemObj.GetComponent<UIItemShow>().InitValue(item.ID, (int)item.Count);
        }
    }

    /// <summary>
    /// 获取升级的全部奖励
    /// </summary>
    /// <param name="lastLv"></param>
    /// <param name="curLv"></param>
    /// <param name="addWarehouseCount"> 新增的仓库数 </param>
    /// <returns></returns>
    private List<GoodIDCount> GetLevelUpRewards(int lastLv, int curLv, ref int addWarehouseCount) 
    {
        Dictionary<int, GoodIDCount> rewards  = new Dictionary<int, GoodIDCount>();

        PlayerGradeDefine playerGrade = null;
        for (int i = lastLv +1 ; i <= curLv; i++) 
        {
            playerGrade = StaticData.configExcel.GetPlayerGradeByGrade(i);

            addWarehouseCount += playerGrade.WarehouseCount;

            if (playerGrade == null)
                continue;
            foreach (var item in playerGrade.UpgradeAwards) 
            {
                if (rewards.ContainsKey(item.ID))
                {
                    rewards[item.ID].Count += item.Count;
                }
                else 
                {
                    rewards.Add(item.ID, item);
                }
            }
        }

        List<GoodIDCount> rewardList = new List<GoodIDCount>();
        foreach (var item in rewards)
        {
            rewardList.Add(item.Value);
        }

        return rewardList;
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    private void OnClickClose() 
    {
        CloseUI();
    }

    private void CloseUI() 
    {
        HideDutterfly();
        switch (_returnOpenType)    
        {
            case ReturnOpenType.Open_None:
                //UIComponent.HideUI(UIType.UILevelUp);
                break;
            case ReturnOpenType.Open_Chapter:
                StaticData.OpenChapterUnlock(_maxUnLockChapter);
                break;
            case ReturnOpenType.Open_Mall:
                StaticData.OpenSeedUnlock(_unlockCrops);
                break;
            case ReturnOpenType.Open_UnLockRegion:
                StaticData.TiggerUnLockArea(_unLockRegionId);
                break;
        }
        UIComponent.RemoveUI(UIType.UILevelUp);
    }


    public async void SpawnHuDie()
    {
        _effect.texture.width = 512;
        _effect.texture.height = 512;

        Transform parent = UIRoot.instance.GetUIRootCanvas().transform.parent;
        string perfabName = "LevelUpHuDie";
        var obj = await ABManager.GetAssetAsync<GameObject>(perfabName);
        _butterfly = Instantiate(obj, parent);
    }

    private void HideDutterfly()
    {
        _butterfly.SetActive(false);
        Destroy(_butterfly);
        _butterfly = null;
    }


    #endregion


}
