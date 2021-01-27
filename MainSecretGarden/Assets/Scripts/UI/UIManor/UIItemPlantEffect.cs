using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemPlantEffect : MonoBehaviour
{
    public List<Image> IconFruits;
    public Text textAddNum;
    public AnimationCurve curve;
    public CanvasGroup canvasGroup;
    public async void ShowInfo(string iconName,int addCount)
    {
        transform.localScale = Vector2.one;
        for (int i = 0; i < IconFruits.Count; i++)
        {
            IconFruits[i].sprite = await ABManager.GetAssetAsync<Sprite>(iconName);
        }
        canvasGroup.alpha = 1f;
        textAddNum.text = $"+{addCount}";
        UIManorComponent uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
        var RectTranPos = StaticData.RectTransWorldPointToUICameraAnchorPos(uiManorComponent.buttonWareHouse.transform.position);
        PalyAnim(RectTranPos);
        Destroy(gameObject, 5f);
    }

    public void PalyAnim(Vector3 rectTransWareHousePos)
    {
        float timeAdd = 0f;
        float animTimeLength = 1.5f;
        DOTween.To(() => transform.GetComponent<RectTransform>().anchoredPosition,
            (anchorpos) =>
            {
                timeAdd += Time.deltaTime;
                transform.GetComponent<RectTransform>().anchoredPosition = anchorpos + new Vector2(curve.Evaluate(timeAdd/ animTimeLength) * 300f, curve.Evaluate(timeAdd / animTimeLength) * 200f);
            }, rectTransWareHousePos, animTimeLength).SetEase(Ease.InQuad);
        DOTween.To(() => transform.localScale, (scale) => transform.localScale = scale, Vector3.one*0.5f, animTimeLength).SetEase(Ease.InQuad);
        DOTween.To(() => canvasGroup.alpha, (alpha) => canvasGroup.alpha = alpha, 0, animTimeLength).SetEase(Ease.InQuad); 
    }
}
