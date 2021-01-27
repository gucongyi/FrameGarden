using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 聊天界面顶部按钮
/// </summary>
public class ChatTopBtnItemController : MonoBehaviour
{
    #region 字段
    /// <summary>
    /// 勾选按钮
    /// </summary>
    Toggle _toggle;
    /// <summary>
    /// 文字显示
    /// </summary>
    Text _showText;
    /// <summary>
    /// 按钮对应数据下标
    /// </summary>
    int _selectIndex;
    /// <summary>
    /// 新消息提示
    /// </summary>
    Transform _updateLabelTra;
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    bool _isInitial = false;

    #endregion
    #region 函数
    private void Awake()
    {
        if (!_isInitial)
        {
            Initial();
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// 组件初始化
    /// </summary>
    void Initial()
    {
        _toggle = GetComponent<Toggle>();
        _showText = transform.Find("Label").GetComponent<Text>();
        _toggle.onValueChanged.RemoveAllListeners();
        _toggle.onValueChanged.AddListener(SelectToggle);
        _updateLabelTra = transform.Find("UpdateLabel");
        _isInitial = true;
    }
    /// <summary>
    /// 设置按钮数据
    /// </summary>
    /// <param name="index"></param>
    /// <param name="showStr"></param>
    public void SetBtnData(int index, string showStr, ToggleGroup toggleGroup)
    {
        if (!_isInitial)
        {
            Initial();
        }
        _showText.text = showStr;
        _selectIndex = index;
        _toggle.group = toggleGroup;
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 按钮点击
    /// </summary>
    /// <param name="arg0"></param>
    private void SelectToggle(bool arg0)
    {
        if (arg0)
        {
            //Debug.Log("勾选顶部按钮：" + _showText.text);
            ChatPanelController._Instance.ClickTopBtn(_selectIndex);
            if (_selectIndex != 1)
            {
                OpenUpdateLabelTra(false);
            }
        }
    }

    public void SetSelect(bool isSelect)
    {
        _toggle.isOn = isSelect;
    }
    /// <summary>
    /// 开关新消息标签
    /// </summary>
    /// <param name="isOpen"></param>
    public void OpenUpdateLabelTra(bool isOpen)
    {
        if (_updateLabelTra==null)
        {
            return;
        }
        _updateLabelTra.gameObject.SetActive(isOpen);
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
    #endregion
}
