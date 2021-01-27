using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.UI;

public class UISignTipControll : MonoBehaviour
{
    public UISignComponent uISignComponent;
    public Transform _down;//奖励展示栏
    public Transform _content;
    public Transform _sureBtn;//确认提示
    public Transform _vowTipMask;//许愿奖励遮罩
    private List<Transform> lsItem=new List<Transform>();//用于动画的链表

    public Transform _butterflyEff;//蝴蝶动效
    private Transform _nowbutterflyEff;//当前蝴蝶动效
   


    //准备动画
    private void Awake()
    {
        _down.localScale = new Vector3(0, 1, 1);
        _sureBtn.gameObject.SetActive(false);
        _vowTipMask.gameObject.SetActive(false);
        for (int i = 0; i < uISignComponent.lsItemVowAnim.Count; i++)
        {
            lsItem.Add(uISignComponent.lsItemVowAnim[i]);
        }

        //创建蝴蝶动效
        _nowbutterflyEff = Instantiate(_butterflyEff, UIRoot.instance.transform.parent);
    }

    //开始动画
    private async void OnEnable()
    {

        _down.DOScale(new Vector3(1, 1, 1), 0.3f);

        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));

        foreach (Transform item in _content)
        {
            item.DOScale(new Vector3(1, 1, 1), 0.2f);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));

        _sureBtn.gameObject.SetActive(true);
        _vowTipMask.gameObject.SetActive(true);

    }

    //结束动画
    private void OnDisable()
    {
        _down.localScale = new Vector3(0, 1, 1);
        _sureBtn.gameObject.SetActive(false);
        _vowTipMask.gameObject.SetActive(false);
        //删除奖励物品
        for (int i = 0; i < _content.childCount; i++)
        {
            Destroy(_content.GetChild(i).gameObject);
        }
        lsItem.Clear();
        uISignComponent.lsItemVowAnim.Clear();

        //销毁动效
        Destroy(_nowbutterflyEff.gameObject);
    }
}
