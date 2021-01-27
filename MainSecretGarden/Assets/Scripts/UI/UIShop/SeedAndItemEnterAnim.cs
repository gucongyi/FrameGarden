using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// 用于商店种子和道具的进程动画
/// </summary>
public class SeedAndItemEnterAnim : MonoBehaviour
{
    public ShoyType shoyType;
    //无限列表下的content
    public Transform content;
    //是否是第一次进入商城；
    public bool isFirstEnter = true;
    //起始位置
    private Vector3 currPosition;

    public enum ShoyType
    {
        Seed=0,
        Item=1,
    }

    //准备动画
    private void Awake()
    {
        //无限列表位置向下移动
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 70, 0);
        currPosition = transform.localPosition;
        //content下的物体全部隐藏
        content.GetComponent<CanvasGroup>().alpha = 0;
    }

    //开始动画
    private void OnEnable()
    {
        if (isFirstEnter && shoyType == ShoyType.Seed)
        {
            FirstEnter();
            isFirstEnter = false;
        }
        else
        {
            //无限列表位置向上移动
            transform.DOLocalMove(new Vector3(currPosition.x, currPosition.y + 70, 0), 0.4f).SetEase(Ease.Linear);
            //content下的物体全部显示
            content.GetComponent<CanvasGroup>().DOFade(1, 0.4f).SetEase(Ease.InExpo);
        }
    }

    //还原动画
    private void OnDisable()
    {
        //无限列表位置向下移动
        transform.localPosition = currPosition;
        //content下的物体全部隐藏
        content.GetComponent<CanvasGroup>().alpha = 0;
    }


    //第一次进入商城
    private void FirstEnter()
    {
        //无限列表位置向上移动
        transform.DOLocalMove(new Vector3(currPosition.x, currPosition.y + 70, 0), 0.5f).SetEase(Ease.Linear);
        //content下的物体全部显示
        content.GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.InExpo);
    }

    //退出动画
    public void BackAnim()
    {
        
        //无限列表位置向下移动
        transform.DOLocalMove(new Vector3(currPosition.x, currPosition.y, 0), 0.2f).SetEase(Ease.Linear);
        //content下的物体全部隐藏
        content.GetComponent<CanvasGroup>().DOFade(0, 0.2f).SetEase(Ease.InExpo);
    }


}
