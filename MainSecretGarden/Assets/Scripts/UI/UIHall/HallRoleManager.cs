using Company.Cfg;
using DG.Tweening;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 大厅角色/切换角色 管理
/// </summary>
public class HallRoleManager : MonoBehaviour
{

    private static HallRoleManager _instance;
    public static HallRoleManager Instance
    {
        get
        {
            if (_instance == null) 
            {
                _instance = GameObject.Find("RolePos").AddComponent<HallRoleManager>();
            }
                
            return _instance;
        }
    }

    #region 变量

    /// <summary>
    /// 当前选中的角色
    /// </summary>
    private int _curSelectedRoleID = 0;

    /// <summary>
    /// 当前选中的角色时装id
    /// </summary>
    private List<int> _curSelectedRoleChoicesID = new List<int>();

    /// <summary>
    /// 当前的角色
    /// </summary>
    private HallRoleNvDef _curRole;

    /// <summary>
    /// 角色挂载的对象
    /// </summary>
    private Transform _rolePos;

    #endregion

    #region 属性

    /// <summary>
    /// 当前选中的角色id
    /// </summary>
    public int CurSelectedRoleID { get { return _curSelectedRoleID; } }

    /// <summary>
    /// 当前选中的角色的时装id
    /// </summary>
    public List<int> CurSelectedRoleChoicesID { get { return _curSelectedRoleChoicesID; } }

    public HallRoleNvDef CurRole 
    {
        get { return _curRole; }
    }

    #endregion



    #region 函数

    /// <summary>
    /// 初始化角色挂载位置/角色挂载的父项
    /// </summary>
    private void InitRolePos() 
    {
        if (_rolePos != null)
            return;
        _rolePos = GameObject.Find("RolePos").transform;
        //DontDestroyOnLoad(_rolePos.gameObject);
    }

    #region 角色操作

    /// <summary>
    /// 初始化角色
    /// </summary>
    public async void InitRole()
    {
        InitRolePos();
        //
        UpdateCurSelectedRoleChoicesID();
        //
        await LoadRole(_curSelectedRoleID, _curSelectedRoleChoicesID);
    }

    /// <summary>
    /// 更新角色当前保存的时装
    /// </summary>
    public void UpdateCurSelectedRoleChoicesID() 
    {
        //服饰id
        _curSelectedRoleChoicesID.Clear();
        _curSelectedRoleID = StaticData.GetHallDefRoleID(ref _curSelectedRoleChoicesID);
    }

    /// <summary>
    /// 显示角色
    /// </summary>
    public void ShowRole()
    {
        if (_curSelectedRoleID != 0 && _curRole != null)
        {
            _curRole.gameObject.SetActive(true);
            _curRole.TouchPlayerNotPlayAnimCallback = TouchPlayer;
        }
        else 
        {
            InitRole();
        }

    }

    /// <summary>
    /// 隐藏角色
    /// </summary>
    public void HideRole()
    {
        if (_curSelectedRoleID != 0 && _curRole != null)
        {
            _curRole.gameObject.SetActive(false);
            _curRole.TouchPlayerNotPlayAnimCallback = null;
        }
    }

    /// <summary>
    /// 更新角色到换装界面
    /// </summary>
    public void UpdateRoleToSwitching() 
    {
        
        Vector3 torgetScale = new Vector3(0.88f, 0.88f, 1);
        GameObject role = _curRole.gameObject;
        role.SetActive(true);

        if (role.transform.localScale == torgetScale)
            return;

        role.transform.localPosition = new Vector3(-0.08f, 0.405f, 0);
        role.transform.localScale = torgetScale;
        //var gob = role.transform.GetComponentInChildren<Live2DRoleAnimControllerBase>().gameObject;
        //gob.transform.localScale = new Vector3(0.0f, 0.0f, 1);
        //DOTween.To(()=> role.transform.localPosition, pos => role.transform.localPosition = pos, new Vector3(0, 0.405f, 0), 0.6f);
        //DOTween.To(() => gob.transform.localScale, scale => gob.transform.localScale = scale, Vector3.one, 0.6f);
    }

    /// <summary>
    /// 更新角色到主页
    /// </summary>
    public void UpdateRoleToHall() 
    {
        Vector3 torgetScale = new Vector3(1.5f, 1.5f, 1f);
        GameObject role = _curRole.gameObject;
        role.transform.localPosition = new Vector3(0, -0.562f, 0);
        role.transform.localScale = torgetScale;
        
        //var gob = role.transform.GetComponentInChildren<Live2DRoleAnimControllerBase>().gameObject;
        //gob.transform.localScale = new Vector3(0.0f, 0.0f, 1);
        //DOTween.To(() => role.transform.localPosition, pos => role.transform.localPosition = pos, Vector3.zero, 0.6f);
        //DOTween.To(() => gob.transform.localScale, scale => gob.transform.localScale = scale, Vector3.one, 0.6f);
    }

    #endregion

    #region 角色数据通用函数

    /// <summary>
    /// 加载角色/时装（切换角色/时装）
    /// </summary>
    /// <param name="roleID"></param>
    /// <param name="choiceId"> 服饰id</param>
    public async Task LoadRole(int roleID, List<int> choicesId, bool isToHall = true)
    {

        //根据角色id获取角色prefab
        string path = StaticData.configExcel.GetHallRoleByID(roleID).Prefab;
        if (string.IsNullOrEmpty(path))
            path = "HallNanDef";
        GameObject Obj = await ABManager.GetAssetAsync<GameObject>(path);
        GameObject roleObj = Instantiate(Obj, _rolePos);

        _curRole = roleObj.GetComponent<HallRoleNvDef>();
        //角色是否可以点击
        _curRole.NotifyActiveRoleTouch(true);
        //角色点击是否播放动画 点击头部
        //_curRole.NotifyUpdateTouchIsPlayAnim(false);
        _curRole.TouchPlayerNotPlayAnimCallback = TouchPlayer;

        //更新角色时装
        _curRole.InitDressUp(choicesId);

        if (isToHall)
        {
            UpdateRoleToHall();
        }
        else 
        {
            UpdateRoleToSwitching();
        }
        
        return;
    }

    /// <summary>
    /// 获取角色时装是否解锁 测试代码 1/18 等待服务器更新后修改
    /// </summary>
    /// <param name="fashionID"></param>
    /// <returns></returns
    public bool GetRoleFashionIsUnlock(int roleID, int fashionID) 
    {
        //var cSHallRoleStruct = StaticData.GetRoleDefChoiceID(roleID);
        //if (cSHallRoleStruct == null)
        //    return false;

        //if (cSHallRoleStruct.ChoiceList.Contains(fashionID.ToString()))
        //    return true;

        return false;
    }

    /// <summary>
    /// 获取角色的时装id是否选中
    /// </summary>
    /// <param name="roleID"></param>
    /// <param name="fashionID"></param>
    /// <returns></returns>
    public bool GetRoleFashionIsSelected(int roleID, int fashionID) 
    {
        if (_curSelectedRoleID != roleID)
            return false;
        return _curSelectedRoleChoicesID.Contains(fashionID);
    }

    /// <summary>
    /// 获取当前角色的全部时装
    /// </summary>
    /// <param name="isActive"></param>
    public List<CostumeDefine> GetCurRoleAllFashions(int selectedRoleID = 0, List<int> roleFashionList = null) 
    {
        List<CostumeDefine> costumes = new List<CostumeDefine>();

        //获取玩家角色的数据
        if (selectedRoleID == 0 && roleFashionList == null) 
        {
            selectedRoleID = _curSelectedRoleID;
            for (int i = 0; i < StaticData.playerInfoData.userInfo.HallRoleInfo.Count; i++)
            {
                if (StaticData.playerInfoData.userInfo.HallRoleInfo[i].RoleId == selectedRoleID) 
                {
                    string choiceDesc = StaticData.playerInfoData.userInfo.HallRoleInfo[i].ChoiceList;
                    var fashionList = choiceDesc.Split(',');

                    roleFashionList = new List<int>();
                    for (int j = 0; j < fashionList.Length; j++)
                    {
                        
                        roleFashionList.Add(Convert.ToInt32(fashionList[j]));
                    }
                }
            }
        }

        //
        if (roleFashionList == null) 
        {
            return costumes;
        }

        CostumeDefine fashion = null;
        for (int i = 0; i < roleFashionList.Count; i++)
        {
            fashion = StaticData.configExcel.GetCostumeByCostumeId(roleFashionList[i]);
            if (fashion != null && fashion.RoleId == selectedRoleID)
            {
                costumes.Add(fashion);
            }
        }
        return costumes;
    }

    #endregion

    #region 角色点击相关

    /// <summary>
    /// 通知角色点击是否开启
    /// </summary>
    /// <param name="isActive"></param>
    public void NotifyRoleTouchIsActive(bool isActive)
    {
        if (_curRole != null)
        {
            _curRole.NotifyActiveRoleTouch(isActive);
        }
    }

    /// <summary>
    /// 点击角色
    /// </summary>
    /// <param name="type"></param>
    private async void TouchPlayer(HitLocation type)
    {
        Debug.Log("点击角色 部位类型 type = " + type);
        await StaticData.OpenUIRoleSwitching();
    }

    #endregion

    #region  服务器通知

    /// <summary>
    /// 通知服务器使用角色和服装 /穿戴
    /// </summary>
    public void NotifyServerUseRoleAndFashion(int roleID, List<int> costumesID, Action<bool> actionCallback)
    {
        CSSwitchoverCostume cSSwitchoverCostume = new CSSwitchoverCostume();
        cSSwitchoverCostume.RoleId = roleID;
        cSSwitchoverCostume.CostumeId.AddRange(costumesID);

        ProtocalManager.Instance().SendCSSwitchoverCostume(cSSwitchoverCostume, (SCEmptySwitchoverCostume sCEmptyDepartureRoom) =>
        {
            Debug.Log("通知服务器使用角色和服装成功！");
            actionCallback?.Invoke(true);
        },
        (ErrorInfo er) =>
        {
            Debug.Log("通知服务器使用角色和服装失败！Error：" + er.ErrorMessage);
            actionCallback?.Invoke(false);
        });
    }

    /// <summary>
    /// 通知服务器购买服装并使用 购买 购物车
    /// </summary>
    public void NotifyServerBuyAndUse(List<CSBuyProp> buyItems, Action<SCBuyProp> actionCallback) 
    {
        CSBuyPropInfo cSBuyPropInfo = new CSBuyPropInfo();
        cSBuyPropInfo.Info.AddRange(buyItems);

        ProtocalManager.Instance().SendCSBuyPropInfo(cSBuyPropInfo, (SCBuyProp sCBuyProp) =>
        {
            Debug.Log("通知服务器购买服装并使用成功！");
            actionCallback?.Invoke(sCBuyProp);
        },
        (ErrorInfo er) =>
        {
            Debug.Log("通知服务器购买服装并使用失败！Error：" + er.ErrorMessage);
            actionCallback?.Invoke(null);
        });
    }

    /// <summary>
    /// 通知服务器购买道具
    /// </summary>
    public void NotifyServerBuyItem(CSBuyProp buyItem, Action<SCBuyProp> actionCallback)
    {

        ProtocalManager.Instance().SendCSBuyProp(buyItem, (SCBuyProp sCBuyProp) =>
        {
            Debug.Log("通知服务器购买道具成功！");
            actionCallback?.Invoke(sCBuyProp);
        },
        (ErrorInfo er) =>
        {
            Debug.Log("通知服务器购买道具失败！Error：" + er.ErrorMessage);
            actionCallback?.Invoke(null);
        });
    }


    #endregion

    #endregion
}
