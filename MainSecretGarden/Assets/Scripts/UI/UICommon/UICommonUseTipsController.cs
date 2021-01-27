using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
/// <summary>
/// 通用物品使用提示
/// </summary>
public class UICommonUseTipsController : MonoBehaviour
{

    #region 变量

    private Button _butClose;

    private Text _Tips;

    private Image _itemIcon;
    private Text _itemNum;

    private Button _butCancel;
    private Button _butUse;
    private Text _butUseDesc;

    private int _useItemID;

    private Toggle _toggleTips;

    private Transform _backgroundImage;

    /// <summary>
    /// 进入是否不再提示
    /// </summary>
    private bool _isNotTips = false;
    #endregion

    #region 方法

    private Action<int> UseCallback;

    private void Awake()
    {
        _backgroundImage = transform.Find("BackgroundImage");
        if (_backgroundImage != null)
        UniversalTool.ReadyPopupAnim(_backgroundImage);
    }

    private void OnEnable()
    {
        if(_backgroundImage!=null)
            UniversalTool.StartPopupAnim(_backgroundImage);
    }
    private void OnDisable()
    {
        if (_backgroundImage != null)
            UniversalTool.ReadyPopupAnim(_backgroundImage);
    }


    private void Init() 
    {
        if (_Tips != null)
            return;

        _Tips = transform.Find("BackgroundImage/Tips").GetComponent<Text>();
        _itemIcon = transform.Find("BackgroundImage/Item/Icon").GetComponent<Image>();
        _itemNum = transform.Find("BackgroundImage/Item/Num").GetComponent<Text>();

        _butCancel = transform.Find("BackgroundImage/But_Cancel").GetComponent<Button>();
        _butUse = transform.Find("BackgroundImage/But_Use").GetComponent<Button>();
        _butUseDesc = _butUse.transform.Find("Text").GetComponent<Text>();
        _toggleTips = transform.Find("BackgroundImage/AgainTipsBG/Toggle").GetComponent<Toggle>();
        _toggleTips.isOn = false;
        _butClose = transform.Find("BG").GetComponent<Button>();

        _butClose.onClick.RemoveAllListeners();
        _butClose.onClick.AddListener(OnClickCancel);

        _butCancel.onClick.RemoveAllListeners();
        _butCancel.onClick.AddListener(()=> { UniversalTool.CancelPopAnim(_backgroundImage, OnClickCancel); });

        _butUse.onClick.RemoveAllListeners();
        _butUse.onClick.AddListener(() => { UniversalTool.CancelPopAnim(_backgroundImage, OnClickUse); });

        _toggleTips.onValueChanged.RemoveAllListeners();
        _toggleTips.onValueChanged.AddListener(OnValueChangedTips);
    }

    /// <summary>
    /// 给通用使用界面初始化值
    /// </summary>
    /// <param name="itemID"> 物品的id</param>
    /// <param name="icon"> 物品的图片</param>
    /// <param name="itemNum">物品剩余数量</param>
    /// <param name="tips"> 使用提示</param>
    /// <param name="useDesc"> 使用描述</param>
    /// <param name="useCallback"> 使用回调</param>
    public void InitValue(int itemID, Sprite icon, int itemNum, string tips, string useDesc, Action<int> useCallback) 
    {
        Init();
        _isNotTips = false;
        _useItemID = itemID;
        UseCallback = useCallback;
        _Tips.text = tips;
        _itemIcon.sprite = icon;
        _itemIcon.SetNativeSize();
        _itemNum.text = itemNum.ToString();
        _butUseDesc.text = useDesc;
    }



    /// <summary>
    /// 点击取消按钮
    /// </summary>
    private void OnClickCancel() 
    {
        UIComponent.RemoveUI(UIType.UICommonUseTips);
    }

    /// <summary>
    /// 点击使用按钮
    /// </summary>
    private void OnClickUse() 
    {
        UseCallback?.Invoke(_useItemID);
        OnClickCancel();
    }

    /// <summary>
    /// 切换是否再次提示
    /// </summary>
    /// <param name="isOn"></param>
    private void OnValueChangedTips(bool isOn) 
    {
        if (_isNotTips == isOn)
            return;

        _isNotTips = isOn;
        //存储数据
        if (_isNotTips)
        {
            PlayerPrefs.SetString(GameUITool.GetItemSaveTipsTimeKey(_useItemID), TimeHelper.CurGameTimeDay());
        }
        else 
        {
            PlayerPrefs.SetString(GameUITool.GetItemSaveTipsTimeKey(_useItemID), string.Empty);
        }
        
    }

    #endregion
}
