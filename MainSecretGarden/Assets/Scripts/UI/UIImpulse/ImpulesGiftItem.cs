using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//男主id 礼物id 数量
public class ImpulesGiftItem : MonoBehaviour, InterfaceScrollCell
{
    ImpulesNPCView impulesNPCView;
    //public Transform _newestFlag;//上新
    public Image Icon;
    public Button useBtn;
    public Text itemName;
    public Text Amount;
    public Text n2pValue;
    public Text p2nValue;
    public ImpulseGiftGoods itemInfo;
    public Image PitchOn;//选中效果
    private bool isInit = false;

    private void Init()
    {
        if (!isInit)
        {
            isInit = true;
            impulesNPCView = transform.GetComponentInParent<ImpulesNPCView>();
            RegisterEventListener();
        }
    }
    private void RegisterEventListener()
    {
        useBtn.onClick.RemoveAllListeners();
        useBtn.onClick.AddListener(OnclickItem);
    }
    //点击item
    private void OnclickItem()
    {
        foreach (var item in impulesNPCView.uIImpulseComponent.ImpulseGiftGoods)
        {
            item.isPitchOn = false;
        }
        itemInfo.isPitchOn = true;
        ImpulesGiftItem[] impulesGifts = impulesNPCView.giftList.content.GetComponentsInChildren<ImpulesGiftItem>();
        foreach (var item in impulesGifts)
        {
            if (item.itemInfo.isPitchOn != true)
            {
                //选中效果
                item.PitchOn.gameObject.SetActive(false);
            }
        }
        impulesNPCView.ShowPitchOn(this.itemInfo, this);
        PitchOn.gameObject.SetActive(true);
    }

    public async void ScrollCellIndex(int idx)
    {//更新数据函数
        Init();
        itemInfo = impulesNPCView.GetGiftItem(idx);
        if (itemInfo.goodsIcon != "")
            Icon.sprite = await ABManager.GetAssetAsync<Sprite>(itemInfo.goodsIcon);
        itemName.text = StaticData.GetMultiLanguageByGameItemId(itemInfo.goodsID);
        n2pValue.text = $"+{itemInfo.NpcAddValue}";
        p2nValue.text = $"+{itemInfo.playerAddValue}";
        Amount.text = itemInfo.goodsAmount.ToString();
        if (itemInfo.isPitchOn == true)
        {
            //选中效果
            PitchOn.gameObject.SetActive(true);
        }
        else
            PitchOn.gameObject.SetActive(false);
    }
    /// <summary>
    /// 刷新道具显示数量
    /// </summary>
    public void RefreshAmount()
    {
        Amount.text = itemInfo.goodsAmount.ToString();
    }

    //public void New()
    //{
    //    var nowTime = TimeHelper.ServerTimeStampNow;
    //    //判定上新状态
    //    if (nowTime > curStoreDefine.UpNewTimeBegin && nowTime < curStoreDefine.UpNewTimeEnd)
    //    {
    //        _newestFlag.gameObject.SetActive(true);
    //    }
    //    else
    //    {
    //        _newestFlag.gameObject.SetActive(false);
    //    }
    //}
}
