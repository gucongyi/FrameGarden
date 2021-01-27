using Company.Cfg;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//创建装饰物
public class UIIemManorRegionReward : MonoBehaviour,InterfaceScrollCell
{
    public Image icon;
    public Text textNum;
    List<CSWareHouseStruct> awardIdsCurrManorRegion;
    private void Awake()
    {

    }
    public void ScrollCellIndex(int idx)
    {
        //获取配置文件
        awardIdsCurrManorRegion = Root2dSceneManager._instance.awardIdsCurrManorRegion;
        var gameItemDefine= StaticData.configExcel.GetGameItemByID(awardIdsCurrManorRegion[idx].GoodId);
        icon.sprite = ABManager.GetAsset<Sprite>(gameItemDefine.Icon);
        textNum.text = $"x{awardIdsCurrManorRegion[idx].GoodNum}";
    }
}
