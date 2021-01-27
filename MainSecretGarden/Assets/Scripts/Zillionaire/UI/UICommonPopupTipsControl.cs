using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;



/// <summary>
/// ui通用弹窗提示类型
/// </summary>
public enum CommonPopupType
{
    TipsBuy,            //单纯提示是否购买
    TipsSell,           //单纯提示是否出售
    TipsBuyAndCurrency, //购买+货币显示
}

/// <summary>
/// ui通用弹窗提示控制类
/// </summary>
public class UICommonPopupTipsControl : MonoBehaviour
{

    private Action callBackConfirmBuy;
    private Action callBackCancelBuy;
    //外部赋值，指初始化一次，如有调整，方便修改
    #region CommonUI
    public Text textTips;
    public Button buttonCancel;
    public Button buttonMask;
    public Button buttonBuy;

    Vector3 btnCancelPos;
    Vector3 btnBuyPos;
    #endregion
    #region ButtonBuy Only Text
    public GameObject goTypeButtonOnlyText;
    public UILocalize uiLocalizeTypeButtonOnlyText;
    #endregion

    #region ButtonBuy Icon And Num
    public GameObject goTypeButtonIconAndNum;
    public Image imageCurrency;
    public Text textCurrencyNum;
    #endregion
    /// <summary>
    /// 再想想
    /// </summary>
    public Button _buttonThinkAgain;
    /// <summary>
    /// 再想想文字描述
    /// </summary>
    public Text _buttonThinkAgainText;


    private Transform _backgroundImage;

    private void Awake()
    {
        _backgroundImage = transform.Find("BackgroundImage");
        if (_backgroundImage != null)
            UniversalTool.ReadyPopupAnim(_backgroundImage);

        buttonMask.onClick.RemoveAllListeners();
        buttonMask.onClick.AddListener(() =>
        {
            UniversalTool.CancelPopAnim(_backgroundImage, Cancel);
        });

        buttonCancel.onClick.RemoveAllListeners();
        buttonCancel.onClick.AddListener(() =>
        {
            UniversalTool.CancelPopAnim(_backgroundImage, Cancel);
        });

        buttonBuy.onClick.RemoveAllListeners();
        buttonBuy.onClick.AddListener(() =>
        {
            UniversalTool.CancelPopAnim(_backgroundImage, Confirm);
        });

        _buttonThinkAgain.onClick.RemoveAllListeners();
        _buttonThinkAgain.onClick.AddListener(() =>
        {
            UniversalTool.CancelPopAnim(_backgroundImage, Confirm);
        });

        //记录位置
        btnCancelPos = buttonCancel.transform.localPosition;
        btnBuyPos = buttonBuy.transform.localPosition;
    }

    private void OnEnable()
    {
        if (_backgroundImage != null)
            UniversalTool.StartPopupAnim(_backgroundImage);
    }
    private void OnDisable()
    {
        if (_backgroundImage != null)
            UniversalTool.ReadyPopupAnim(_backgroundImage);
    }

    /// <summary>
    /// 界面初始化
    /// </summary>
    /// <param name="desc"></param>
    /// <param name="icon"></param>
    /// <param name="num"></param>
    /// <param name="confirmBuy"></param>
    /// <param name="cancelBuy"></param>
    /// <param name="idLocalizeButCancel"> 取消按钮 国际化id </param>
    /// <param name="btnPosExchange">按钮位置交换</param>
    public void InitialButtonBuyIconAndNum(string desc, Sprite icon, int num, Action confirmBuy, Action cancelBuy = null, int idLocalizeButCancel = -1, bool btnPosExchange = false)
    {
        OpenThinkAgainBtn(false);
        InitCommon(desc, confirmBuy, cancelBuy);
        if (btnPosExchange)
        {
            buttonMask.enabled = false;
            buttonCancel.transform.localPosition = btnBuyPos;
            buttonBuy.transform.localPosition = btnCancelPos;
        }
        else
        {
            buttonMask.enabled = true;
            buttonCancel.transform.localPosition = btnCancelPos;
            buttonBuy.transform.localPosition = btnBuyPos;
        }
        SetButCancelOnlyTextState(idLocalizeButCancel);
        SetButtonBuyIconAndNumState(icon, num);
    }

    /// <summary>
    /// 纯提示界面
    /// </summary>
    /// <param name="desc"></param>
    /// <param name="confirmName"></param>
    /// <param name="confirmBuy"></param>
    /// <param name="btnPosExchange">按钮位置交换</param>
    public void InitialButtonBuyOnlyText(string desc, Action confirmBuy, Action cancelBuy = null, int idLocalize = -1, int idLocalizeButCancel = -1, bool btnPosExchange = false)
    {
        OpenThinkAgainBtn(false);
        SetButCancelOnlyTextState(idLocalizeButCancel);
        SetButBuyOnlyTextState(idLocalize);
        InitCommon(desc, confirmBuy, cancelBuy);
        if (btnPosExchange)
        {
            buttonMask.enabled = false;
            buttonCancel.transform.localPosition = btnBuyPos;
            buttonBuy.transform.localPosition = btnCancelPos;
        }
        else
        {
            buttonMask.enabled = true;
            buttonCancel.transform.localPosition = btnCancelPos;
            buttonBuy.transform.localPosition = btnBuyPos;
        }
    }

    /// <summary>
    /// 只有一个按钮的界面
    /// </summary>
    /// <param name="desc"></param>
    /// <param name="confirmName"></param>
    /// <param name="confirmBuy"></param>
    public void InitialOneBtnPane(string desc, Action confirmBuy, int idLocalize = -1)
    {
        //点击空白不能关闭
        buttonMask.onClick.RemoveAllListeners();
        InitCommon(desc, confirmBuy, null);
        OpenThinkAgainBtn(true);
        _buttonThinkAgainText.text = StaticData.GetMultilingual(idLocalize);
    }
    public void OpenThinkAgainBtn(bool isOpen)
    {

        buttonCancel.gameObject.SetActive(!isOpen);
        buttonBuy.gameObject.SetActive(!isOpen);
        _buttonThinkAgain.gameObject.SetActive(isOpen);
    }
    private void InitCommon(string desc, Action confirmBuy, Action cancelBuy = null)
    {
        callBackConfirmBuy = confirmBuy;
        callBackCancelBuy = cancelBuy;
        textTips.text = desc;
    }


    private void SetButBuyOnlyTextState(int idLocalize)
    {
        goTypeButtonOnlyText.SetActive(true);
        goTypeButtonIconAndNum.SetActive(false);
        if (idLocalize != -1)
        {
            uiLocalizeTypeButtonOnlyText.SetOtherLanguageId(idLocalize);
        }
    }

    /// <summary>
    /// 设置按钮取消的描述
    /// </summary>
    /// <param name="idLocalize"></param>
    private void SetButCancelOnlyTextState(int idLocalize)
    {
        var textObj = buttonCancel.transform.Find("Text").gameObject;
        textObj.SetActive(true);
        if (idLocalize != -1)
        {
            textObj.GetComponent<UILocalize>().SetOtherLanguageId(idLocalize);
        }
    }

    private void SetButtonBuyIconAndNumState(Sprite icon, int num)
    {
        goTypeButtonOnlyText.SetActive(false);
        goTypeButtonIconAndNum.SetActive(true);

        imageCurrency.sprite = icon;
        textCurrencyNum.text = $"{num}";
    }

    /// <summary>
    /// 取消 关闭界面
    /// </summary>
    private void Cancel()
    {
        UIComponent.HideUI(UIType.UICommonPopupTips);
        callBackCancelBuy?.Invoke();
    }

    /// <summary>
    /// 确定 且关闭界面
    /// </summary>
    private void Confirm()
    {
        UIComponent.HideUI(UIType.UICommonPopupTips);
        callBackConfirmBuy?.Invoke();
    }
}