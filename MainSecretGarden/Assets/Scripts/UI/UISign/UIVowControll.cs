using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIVowControll : MonoBehaviour
{
    //底部栏
    public Transform _down;


    #region 变量
    //起始位置
    private Vector3 _currPosition;
    //变化位置
    private Vector3 _changePosition;
    #endregion

    //准备动画
    private void Awake()
    {
        if (_down == null) return;
        _currPosition = _down.localPosition;
        _down.localPosition = new Vector3(_currPosition.x, _currPosition.y - 376, _currPosition.z);
        _changePosition = _down.localPosition;
    }

    //开始动画
    private void OnEnable()
    {
        if (_down == null) return;
        _down.DOLocalMove(_currPosition, 0.3f);
    }

    //还原动画
    private void OnDisable()
    {
        if (_down == null) return;
        _down.localPosition = _changePosition;
    }

    //退出动画
    public void BackAnim()
    {
        _down.DOLocalMove(_changePosition, 0.3f);
    }
}
