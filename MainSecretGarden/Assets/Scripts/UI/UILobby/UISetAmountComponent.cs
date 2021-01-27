using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISetAmountComponent : MonoBehaviour
{
    public Button addBtn;
    public Button reduceBtn;
    public InputField inputFiled;
    public Button sureBtn;
    /// <summary>
    /// 默认值
    /// 2020/8/5 huangjiangdong
    /// </summary>
    public string _defaultValue;
    public int result;
    /// <summary>
    /// 数值变化监听
    /// 2020/8/5 huangjiangdong
    /// </summary>
    public Action<int> _changeAction;

    public Action<int> act;
    /// <summary>
    /// 最大值
    /// 2020/8/5 huangjiangdong
    /// </summary>
    public int _maxValue;
    /// <summary>
    /// 最小值
    /// 2020/8/5 huangjiangdong
    /// </summary>
    public int _minValue;
    void Start()
    {
        inputFiled.text = _defaultValue;
        RegisterEvent();
    }

    private void RegisterEvent()
    {
        addBtn.onClick.RemoveAllListeners();
        addBtn.onClick.AddListener(() => OnClickAddBtn());
        reduceBtn.onClick.RemoveAllListeners();
        reduceBtn.onClick.AddListener(() => OnClickReduceBtn());
        if (sureBtn != null) 
        {
            sureBtn.onClick.RemoveAllListeners();
            sureBtn.onClick.AddListener(() => OnClickSureBtn());
        }

        inputFiled.onEndEdit.RemoveAllListeners();
        inputFiled.onEndEdit.AddListener((tempText) =>
        {
            inputFiled.text = ShowNumber(inputFiled.text);
        });

        inputFiled.onValueChanged.RemoveAllListeners();
        inputFiled.onValueChanged.AddListener(OnChange);

        RepeatButton ButtonRepeateAdd = addBtn.GetComponent<RepeatButton>();
        if (ButtonRepeateAdd!=null)
        {
            ButtonRepeateAdd.onPress.RemoveAllListeners();
            ButtonRepeateAdd.onPress.AddListener(OnClickAddBtn);
        }

        RepeatButton ButtonRepeateReduce = reduceBtn.GetComponent<RepeatButton>();
        if(ButtonRepeateReduce!=null)
        {
            ButtonRepeateReduce.onPress.RemoveAllListeners();
            ButtonRepeateReduce.onPress.AddListener(OnClickReduceBtn);
        }

    }
    /// <summary>
    /// 当值发生变化时监听
    /// 2020/8/5 huangjiangdong
    /// </summary>
    /// <param name="arg0"></param>
    private void OnChange(string arg0)
    {
        if (string.IsNullOrEmpty(arg0))
        {
            result = 0;
            inputFiled.text = "";
        }
        else
        {
            //转换输入框的值
            result = int.Parse(ShowNumber(arg0));
            inputFiled.text = result.ToString();
        }
        
        //传递回调
        _changeAction?.Invoke(result);
    }

    private void OnClickAddBtn()
    {//扩展 提示金币不足？限制数量增长？
        if (int.TryParse(inputFiled.text, out int count))
        {
            inputFiled.text = ShowNumber(++count);
        }
    }
    private void OnClickReduceBtn()
    {
        if (int.TryParse(inputFiled.text, out int count))
        {
            inputFiled.text = ShowNumber(--count);
        }
    }
    private void OnClickSureBtn()
    {//手动设置输入框值后点击按钮 优先触发onEnd事件
        result = int.Parse(ShowNumber(inputFiled.text));//int.Parse必定能成功
        inputFiled.text = result.ToString();
        act?.Invoke(result);
    }

    private string ShowNumber(string num)
    {
        if (int.TryParse(num, out int amount))
        {
            /// 修改最大值最小值
            /// 2020/8/5 黄江东
            if (amount > _maxValue) amount = _maxValue;
            if (amount < _minValue) amount = _minValue;
        }
        else
        {
            amount = 1;
        }
        num = amount.ToString();
        return num;
    }

    private string ShowNumber(int num)
    {
        /// 修改最大值最小值
        /// 2020/8/5 黄江东
        if (num > _maxValue) num = _maxValue;
        if (num < _minValue) num = _minValue;
        return num.ToString();
    }
    public void ResetDefalut() 
    {
        inputFiled.text = _defaultValue;
    }
}
