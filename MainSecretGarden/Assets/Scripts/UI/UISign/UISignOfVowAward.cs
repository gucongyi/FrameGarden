using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Company.Cfg;
using Game.Protocal;
using UnityEngine.UI;

public class UISignOfVowAward : MonoBehaviour, InterfaceScrollCell
{
    public UISignComponent uISignComponent;

    //组件
    private Transform _icon;
    private Transform _iconName;
    private Transform _Number;
    private Transform _GradeBg;
    private Transform _Grade;
    private Transform _RareBg;
    public Transform _inFo;

    //许愿奖励
    public SCEverydayAward sCVowAward;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        _icon = transform.Find("Icon");
        _iconName = transform.Find("IconName/Text");
        _Number = transform.Find("Number");
        _GradeBg = transform.Find("Grade");
        _Grade = transform.Find("Grade/Text");
        _RareBg = transform.Find("RareBg");
    }

    public async void ScrollCellIndex(int idx)
    {
        Init();

        if (uISignComponent == null)
        {
            uISignComponent = UIComponent.GetComponentHaveExist<UISignComponent>(UIType.UISign);
        }

        sCVowAward = uISignComponent.LsSCVowAward[idx];

        var itemConfig = StaticData.configExcel.GetGameItemByID(sCVowAward.GoodId);
        //包裹配置信息
        var awadConfig = StaticData.configExcel.GetPackageByID(sCVowAward.ParcelId);

        //道具等级 只有种子和果实有
        if (itemConfig.ItemType == TypeGameItem.Seed || itemConfig.ItemType == TypeGameItem.Fruit)
        {
            _GradeBg.gameObject.SetActive(true);
            _Grade.GetComponent<Text>().text = itemConfig.Grade.ToString();

        }
        else
        {
            _GradeBg.gameObject.SetActive(false);
        }

        //道具地板
        switch (itemConfig.Rarity)
        {
            case TypeRarity.None:
                _RareBg.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>("sd_sp_k1");
                break;
            case TypeRarity.Primary:
                _RareBg.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>("sd_sp_k1");
                break;
            case TypeRarity.Intermediate:
                _RareBg.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>("sd_sp_k2");
                break;
            case TypeRarity.Senior:
                _RareBg.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>("sd_sp_k3");
                break;
            default:
                break;
        }


        //道具名字
        _iconName.GetComponent<Text>().text = StaticData.GetMultilingual(itemConfig.ItemName);

        //道具图片
        _icon.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(itemConfig.Icon);

        //道具数量
        _Number.GetComponent<Text>().text = sCVowAward.GoodNum.ToString();

        //幸运级别描述
        _inFo.GetComponent<Text>().text = StaticData.GetMultilingual(awadConfig.Description);

    }
}
