using Company.Cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//创建装饰物
public class UIItemRegionPayProp : MonoBehaviour,InterfaceScrollCell
{
    public Image imageIcon;
    public Text TextNum;
    public Text TextName;

    private void Awake()
    {
    }

    public async void ScrollCellIndex(int idx)
    {
        //获取配置文件
        UIManorRegionPay uiManorRegionPay=UIComponent.GetComponentHaveExist<UIManorRegionPay>(UIType.UIManorRegionPay);
        if (uiManorRegionPay == null)
        {
            return;
        }
        if (uiManorRegionPay.manorRegionComponent == null)
        {
            return;
        }
        var dealClass = StaticData.configExcel.GetAreaUnlockByID(uiManorRegionPay.manorRegionComponent.regionId).ConsumptionGood;
        var propDefine=StaticData.configExcel.GetGameItemByID(dealClass[idx].IdGameItem);
        imageIcon.sprite = await ABManager.GetAssetAsync<Sprite>(propDefine.Icon);
        int currHaveCount = StaticData.GetWareHouseItem(dealClass[idx].IdGameItem).GoodNum;
        if (currHaveCount < dealClass[idx].Price)
        {
            TextNum.text = $"<color=#EF72A4>{currHaveCount}</color>/{dealClass[idx].Price}";
        } else
        {
            TextNum.text = $"<color=#E7E7E9>{currHaveCount}</color>/{dealClass[idx].Price}";
        }
        TextName.text = StaticData.GetMultilingual(propDefine.ItemName);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
