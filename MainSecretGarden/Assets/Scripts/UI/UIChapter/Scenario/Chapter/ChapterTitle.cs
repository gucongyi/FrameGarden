using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ChapterTitle : MonoBehaviour
{
    public Image line_image;
    public RectTransform TextRect;
    public RectTransform circle;
    public Text titleText;
    public Image TextRect_image;
    public Image circle_image;

    Vector3 line_image_endScale;
    float TextRect_endPosX;
    float circle_endPosX;

    //动画持续时间
    float lineDuration = 1.2f;//直线动画
    float maskDuration = 0.8f;//遮罩动画
    float fadeDuration = 2f;//渐入动画
    Color titleText_endColor = new Color(211f / 255f, 146f / 255f, 245f / 255f, 1);

    public void Play(string title, string titleContent, bool isAutoFade = false)
    {
        Init(title, titleContent);
        var s = DOTween.Sequence();
        s.AppendInterval(0.1f);
        s.Append(line_image.transform.DOScale(1, lineDuration));
        s.Append(TextRect.DOAnchorPosX(TextRect_endPosX, maskDuration).OnComplete(() =>
        {
            TextRect_image.enabled = false;
        }));
        s.Join(circle.DOAnchorPosX(circle_endPosX, maskDuration).OnComplete(() =>
        {
            circle_image.enabled = false;
        }));
        s.Append(titleText.DOColor(titleText_endColor, fadeDuration));
        s.Play().OnComplete(() =>
        {
            if (isAutoFade)
            {
                ChapterHelper.Fade(this.gameObject, 0, 2f, 1, () =>
                {
                    GameObject.Destroy(this.gameObject);
                    UIComponent.RemoveUI(UIType.ChapterTitle);
                });
            }
            else
            {
                var fadeOutBtn = transform.GetComponent<Button>();
                fadeOutBtn.onClick.RemoveAllListeners();
                fadeOutBtn.onClick.AddListener(() =>
                {
                    fadeOutBtn.enabled = false;
                    ChapterHelper.Fade(this.gameObject, 0, 2f, 1, () =>
                    {
                        GameObject.Destroy(this.gameObject);
                        UIComponent.RemoveUI(UIType.ChapterTitle);
                        ChapterTool.ShowChapterManager();
                    });
                   
                });
              
            }
        });
    }

    private void Init(string title, string titleContent)
    {
        this.TextRect.GetComponent<Text>().text = title;
        this.titleText.text = titleContent;

        line_image_endScale = line_image.transform.localScale;
        line_image.transform.localScale = Vector3.zero;

        TextRect_endPosX = TextRect.transform.localPosition.x;
        TextRect.transform.localPosition = new Vector3(-200, -69, 0);

        circle_endPosX = circle.transform.localPosition.x;
        circle.transform.localPosition = new Vector3(125, 62, 0);

        //恢复遮罩
        TextRect_image.enabled = true;
        circle_image.enabled = true;
    }

}
