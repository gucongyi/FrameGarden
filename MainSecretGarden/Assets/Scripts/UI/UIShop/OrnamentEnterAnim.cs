using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OrnamentEnterAnim : MonoBehaviour
{
    public CanvasGroup decoratePopIconCG;
    public Transform decoratePopIcon;
    //无限列表下的content
    public CanvasGroup content;
    //无限列表
    public Transform loopVerticalScroll;
    //起始位置
    private Vector3 currPosition;

    //准备动画
    private void Awake()
    {
        decoratePopIconCG.alpha = 0.0f;
        decoratePopIcon.localScale = decoratePopIcon.localScale * 0.8f;

        //无限列表动画
        //无限列表位置向下移动
        loopVerticalScroll.localPosition = new Vector3(loopVerticalScroll.localPosition.x, loopVerticalScroll.localPosition.y - 70, 0);
        currPosition = loopVerticalScroll.localPosition;
        Debug.Log(currPosition);
        //content下的物体全部隐藏

        content.GetComponent<CanvasGroup>().alpha = 0;
    }

    //开始动画
    private void OnEnable()
    {
        DOTween.To(() => decoratePopIconCG.alpha, alpha => decoratePopIconCG.alpha = alpha, 1.0f, 0.35f);
        decoratePopIcon.DOScale(new Vector3(2.5f,2.5f,1), 0.35f);

        //无限列表动画
        //无限列表位置向上移动
        loopVerticalScroll.DOLocalMoveY(currPosition.y + 70, 0.4f).SetEase(Ease.Linear);
        //content下的物体全部显示
        content.GetComponent<CanvasGroup>().DOFade(1, 0.4f).SetEase(Ease.InExpo);
    }

    //还原动画
    private void OnDisable()
    {
        decoratePopIconCG.alpha = 0.0f;
        decoratePopIcon.localScale = decoratePopIcon.localScale * 0.8f;

        //无限列表动画
        //无限列表位置向下移动
        loopVerticalScroll.localPosition = currPosition;
        //content下的物体全部隐藏
        content.GetComponent<CanvasGroup>().alpha = 0;
    }

    //退出动画
    public void BackAnim()
    {
        DOTween.To(() => decoratePopIconCG.alpha, alpha => decoratePopIconCG.alpha = alpha, 0f, 0.35f);
        decoratePopIcon.DOScale(new Vector3(2.5f, 2.5f, 1)*0.8f, 0.35f);


        //无限列表位置向下移动
        loopVerticalScroll.DOLocalMoveY(currPosition.y, 0.2f).SetEase(Ease.Linear);
        //content下的物体全部隐藏
        content.GetComponent<CanvasGroup>().DOFade(0, 0.2f).SetEase(Ease.InExpo);
    }
}
