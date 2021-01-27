using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemExpEffect : MonoBehaviour
{
    public Text textAddNum;
    public AnimationCurve curve;
    public CanvasGroup canvasGroup;
    public async void ShowInfo(int addCount)
    {
        transform.localScale = Vector2.one;
        canvasGroup.alpha = 1f;
        textAddNum.text = $"+{addCount}";
        UIManorComponent uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
        var transUIExp=uiManorComponent.goPlayerIcon.GetComponent<UIPlayerImageComponent>().imageExp.transform;
        var RectTranPos = StaticData.RectTransWorldPointToUICameraAnchorPos(transUIExp.position);
        PalyAnim(RectTranPos);
        Destroy(gameObject, 5f);
    }

    public void PalyAnim(Vector3 rectTransExtPos)
    {
        float animTimeLength = 1.5f;
        DOTween.To(() => transform.GetComponent<RectTransform>().anchoredPosition + new Vector2(-50f, 50f),
            (anchorpos) =>
            {
                transform.GetComponent<RectTransform>().anchoredPosition = anchorpos+new Vector2(0f, 0f);
            }, rectTransExtPos, animTimeLength).SetEase(Ease.Linear);
        DOTween.To(() => canvasGroup.alpha, (alpha) => canvasGroup.alpha = alpha, 0, animTimeLength).SetEase(Ease.InQuad); 
    }
    
}
