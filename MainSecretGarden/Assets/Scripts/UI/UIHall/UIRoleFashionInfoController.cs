using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色时装信息显示控制器
/// </summary>
public class UIRoleFashionInfoController : MonoBehaviour, InterfaceScrollCell
{

    #region 变量
    private Button _butSelected;
    private Image _icon;
    private Text _name;
    private GameObject _unlock;
    private GameObject _conflict;
    private GameObject _frame;//边框 选中时使用

    /// <summary>
    /// 时装在表中的下标
    /// </summary>
    private int _index = 0;
    #endregion

    private Text FashionName
    {
        get { 
            if (_name == null)
                Init();
            return _name;
        }
    }

    #region 函数

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Init() 
    {
        if (_name != null)
            return;
        var icon = transform.Find("Icon");
        _butSelected = transform.GetComponent<Button>();
        _icon = icon.GetComponent<Image>();
        _name = transform.Find("NameBG/Name").GetComponent<Text>();
        _unlock = transform.Find("Unlock").gameObject;
        _frame = transform.Find("Frame").gameObject;
        _conflict = transform.Find("Conflict").gameObject;

        _butSelected.onClick.RemoveAllListeners();
        _butSelected.onClick.AddListener(OnClickSelected);
    }


    /// <summary>
    /// 实现接口当前显示的对象再列表中的下标//从0开始
    /// </summary>
    /// <param name="idx"></param>
    void InterfaceScrollCell.ScrollCellIndex(int idx)
    {
        UpdateShowInfo(idx);
        //throw new System.NotImplementedException();
    }

    /// <summary>
    /// 更新显示信息
    /// </summary>
    private void UpdateShowInfo(int index) 
    {
        _index = index;
        UIRoleSwitchingController.Instance.GetFashionInfo(index, LoadShowCallback, ClearSelected, ClearConflict);
    }

    /// <summary>
    /// 加载显示回调
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isUnlock"></param>
    /// <param name="icon"></param>
    /// <param name="isSelected">是否已经选中</param>
    /// <param name="isConflict">是否为冲突时装</param>
    private void LoadShowCallback(string name, bool isUnlock, Sprite icon, bool isSelected, bool isConflict) 
    {
        FashionName.text = name;
        _icon.sprite = icon;
        _unlock.SetActive(false);
        _frame.SetActive(isSelected);
        _conflict.SetActive(isConflict);
    }


    /// <summary>
    /// 点击选中
    /// </summary>
    private void OnClickSelected() 
    {
        //是否选中
        bool isSelected = false;
        UIRoleSwitchingController.Instance.SelectedFashion(_index, ref isSelected);

        //是否选中
        _frame.SetActive(isSelected);
    }

    /// <summary>
    /// 清除选中效果
    /// </summary>
    public void ClearSelected() 
    {
        _frame.SetActive(false);
    }

    /// <summary>
    /// 清除冲突效果
    /// </summary>
    public void ClearConflict()
    {
        _conflict.SetActive(false);
    }

    #endregion
}
