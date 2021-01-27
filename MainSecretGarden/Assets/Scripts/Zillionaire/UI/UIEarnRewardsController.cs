using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 获得奖励展示界面
/// </summary>
public class UIEarnRewardsController : MonoBehaviour
{
    #region 变量
    private Image _itemIcon;
    private Text _itemName;
    private Text _itemNum;

    private Button _butClose;
    #endregion

    #region 方法

    private void Start()
    {
    }

    private void Init()
    {
        if (_itemIcon != null)
            return;
        _itemIcon = transform.Find("BG/UIItem/BG/Icon").GetComponent<Image>();
        _itemName = transform.Find("BG/UIItem/BG/NameBg/Name").GetComponent<Text>();
        _itemNum =  transform.Find("BG/UIItem/BG/Num").GetComponent<Text>();
        _butClose = transform.Find("BG/ButClose").GetComponent<Button>();

        _butClose.onClick.RemoveAllListeners();
        _butClose.onClick.AddListener(OnClickClose);
    }

    /// <summary>
    /// 界面初始化值
    /// </summary>
    /// <param name="icon"></param>
    /// <param name="name"></param>
    /// <param name="num"></param>
    public void InitValue(Sprite icon, string name, int num) 
    {
        Init();

        _itemIcon.sprite = icon;
        _itemIcon.SetNativeSize();
        _itemName.text = name;
        _itemNum.text = num.ToString();
    }

    private void OnClickClose() 
    {

        UniversalTool.CancelPopAnim(transform.Find("BG"), UIClose);
    }

    private void UIClose() 
    {
        UIComponent.RemoveUI(UIType.UIEarnRewards);
    }
    #endregion

}
