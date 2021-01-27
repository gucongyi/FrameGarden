using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 大富翁随机事件效果控制
/// </summary>
public class ZillionaireRandomEventEffectController : ZillionaireOrdinaryEventEffectController
{
    #region 变量


    #endregion

    #region 方法

    protected override void PlayEffect()
    {
        transform.DOScale(Vector3.one*1.15f, 0.3f).OnComplete(() => { transform.DOScale(Vector3.one, 0.05f).OnComplete(() => WaitNext()); });
    }

    protected override async void WaitNext()
    {
        //单位
        await UniTask.Delay(800);
        transform.DOScale(Vector3.zero, 0.12f).OnComplete(() => CloseUI());
    }

    #endregion
}
