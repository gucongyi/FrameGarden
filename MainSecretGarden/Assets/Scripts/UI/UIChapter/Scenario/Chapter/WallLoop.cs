using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
//12000->-9287
//1-1  11956-9342
public class WallLoop : MonoBehaviour
{
    public Action callback;

    public RectTransform[] items;
    private void Start()
    {
        items[0].DOAnchorPosX(28683f-10000, 8f).OnComplete(async () =>//围墙
        {
           await Cysharp.Threading.Tasks.UniTask.Delay(400);
           callback?.Invoke();
        });
        items[1].DOAnchorPosX(21896 - 10000, 8f);//庄园内树
        items[2].DOAnchorPosX(21617 - 10000, 8f);//地面
        items[3].DOAnchorPosX(998-10000, 10f);//房子
        items[4].DOAnchorPosX(950-10000, 10f);//背景草
        items[5].DOAnchorPosX(900-10000, 10f);//天空
    }


}
