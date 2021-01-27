using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 功能开放提示界面控制
/// </summary>
public class UIOpenFunctionTipsController : MonoBehaviour
{
    #region 变量

    private Transform _bgTra;

    private Button _butClose;

    private Text _desc;


    #endregion

    #region 方法

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
    }

    private void OnEnable()
    {
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
        if (_butClose != null)
            return;
        _butClose = transform.Find("BG").GetComponent<Button>();
        _butClose.onClick.RemoveAllListeners();
        _butClose.onClick.AddListener(() => { UniversalTool.CancelPopAnim(_bgTra, OnClickClose); });

        _desc = transform.Find("BG_Image/Desc").GetComponent<Text>();
    }

    public void InitValue(int needLevel = 0) 
    {
        Init();

        string desc = string.Empty;
        if (needLevel == 0)//拼命开发
        {
            desc = LocalizationDefineHelper.GetStringNameById(120077);
        }
        else//等级开放
        {
            desc = string.Format(LocalizationDefineHelper.GetStringNameById(120218), needLevel);
        }
        _desc.text = desc;
    }

    private void OnClickClose() 
    {
        UIComponent.RemoveUI(UIType.UIOpenFunctionTips);
    }

    #endregion
}
