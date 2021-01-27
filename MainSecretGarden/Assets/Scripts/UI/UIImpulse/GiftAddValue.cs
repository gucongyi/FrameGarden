using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 送礼对话
/// </summary>
public class GiftAddValue : MonoBehaviour
{
    public Text content;//对话内容
    public CanvasGroup canvasGroup;
    float Dealy = 2;//2秒时间透明
    public TimeCountDownComponent timeCountDown; //倒计时器

    public void ShowGiftDialogue(int value)
    {
        canvasGroup.alpha = 1;
        this.content.text = $"+{value}";
        if (timeCountDown != null)
        {
            timeCountDown.Init(Dealy, true, () =>
            {
                canvasGroup.DOFade(0, 0.8f);
            }, (x) =>
               {
                   //Debug.LogError($"倒计时{x}");
               });
        }
        else
            timeCountDown = StaticData.CreateTimer(Dealy, true, (go) =>
                {
                    canvasGroup.DOFade(0, 0.8f);
                }, (x) =>
                 {
                     //Debug.LogError($"倒计时{x}");
                 }, "giftTime");

    }
}
