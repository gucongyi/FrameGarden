using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 界面选中控制
/// </summary>
public class UIToggleNew : MonoBehaviour
{
    /// <summary>
    /// 是否选中
    /// </summary>
    [SerializeField]
    private bool _isOn;
    private GameObject _def;
    private GameObject _selected;
    private Button _button;

    private int ID;

    public Action<int> OnClickSelected;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Init() 
    {
        if (_def != null)
            return;
        _def = transform.Find("Def").gameObject;
        _selected = transform.Find("Selected").gameObject;
        _button = gameObject.GetComponent<Button>();
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnClickToggle);
    }

    /// <summary>
    /// 初始化值
    /// </summary>
    /// <param name="id">id</param>
    /// <param name="isOn">是否选中</param>
    /// <param name="uncheck">取消选中事件/行为</param>
    /// <param name="onClickSelected">点击选中</param>
    public void InitValue(int id, bool isOn) 
    {
        ID = id;
        Init();

        SetToggle(isOn);
    }

    /// <summary>
    /// 点击按钮 不允许取消点击
    /// </summary>
    private void OnClickToggle() 
    {
        if (_isOn)
            return;
        //SetToggle(true);
        OnClickSelected.Invoke(ID);
    }

    /// <summary>
    /// 设置是否选中
    /// </summary>
    /// <param name="isOn"></param>
    public void SetToggle(bool isOn) 
    {
        _isOn = isOn;
        _def.SetActive(!isOn);
        _selected.SetActive(isOn);
    }

    /// <summary>
    /// 取消选中
    /// </summary>
    public void Uncheck() 
    {
        if (_isOn == false)
            return;
        SetToggle(false);
    }

}
