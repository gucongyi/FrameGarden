using Cysharp.Threading.Tasks;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManorSelfGrowUpComponent : MonoBehaviour
{
    public enum TypeFertilizer
    {
        fertilizer2,
        fertilizer3
    }
    public Button ButtonFertilizer2;//第二种化肥
    public Text TextNumNormal2;
    public Text TextNumZero2;
    public Button ButtonFertilizer3;//第三种化肥
    public Text TextNumNormal3;
    public Text TextNumZero3;
    public Button ButtonEradicate;
    public Text TextRemainTimeCurrPeriod;
    public Text TextNextPeriodTip;
    public Text TextGainTip;
    #region 广告一键加速所有按钮
    public Button ButtonFertilizerAd;
    public Text TextAdNumNormal;
    public Text TextAdZero;
    public Text TextAdTotal;
    #endregion
    public UIWorldHandleManager uiWorldHandleManager;
    [HideInInspector]
    public SeedGrowComponent seedGrowComponent;
    // Start is called before the first frame update
    void Start()
    {
        TextNextPeriodTip.gameObject.SetActive(false);
        TextGainTip.gameObject.SetActive(false);
        //对种子期到成熟期的物品操作
        ButtonEradicate.onClick.RemoveAllListeners();
        ButtonEradicate.onClick.AddListener(uiWorldHandleManager.OnButtonEradicateClick);
        ButtonFertilizer2.onClick.RemoveAllListeners();
        ButtonFertilizer2.onClick.AddListener(() =>
        {
            OnButtonFertilizerClick(TypeFertilizer.fertilizer2);
        });
        ButtonFertilizer3.onClick.RemoveAllListeners();
        ButtonFertilizer3.onClick.AddListener(() =>
        {
            OnButtonFertilizerClick(TypeFertilizer.fertilizer3);
        });
        ButtonFertilizerAd.onClick.RemoveAllListeners();
        ButtonFertilizerAd.onClick.AddListener(OnButtonFerlizerAdClick);
    }

    private void OnEnable()
    {
        UpdateFertilizerAdCount();
    }

    private void UpdateFertilizerAdCount()
    {
        TextAdNumNormal.gameObject.SetActive(false);
        TextAdZero.gameObject.SetActive(false);
        int adCount = StaticData.configExcel.GetVertical().CropSpeedCount;
        int usedCount = StaticData.playerInfoData.userInfo.CropCount;
        int remainCount = adCount - usedCount;
        if (remainCount <= 0)
        {
            TextAdZero.gameObject.SetActive(true);
            ButtonFertilizerAd.interactable = false;
        }
        else
        {
            TextAdNumNormal.gameObject.SetActive(true);
            TextAdNumNormal.text = $"{remainCount}";
            ButtonFertilizerAd.interactable = true;
        }
        TextAdTotal.text = $"/{adCount}";
    }

    void OnButtonFerlizerAdClick()
    {
        StaticData.OpenAd("FerlizerAddAd", (code, msg) => {
            if (code == 1)
            {
                StaticData.DataDot(Company.Cfg.DotEventId.ManorPlantAdIncrease);
                OnButtonAdClick();
            }
        });
    }
    private void OnButtonAdClick()
    {
        List<SeedGrowComponent> listSeedGrow = Root2dSceneManager._instance.GetListSeedGrow();
        //23阶段的作物
        List<SeedGrowComponent> listSeedGrow23 = new List<SeedGrowComponent>();
        if (listSeedGrow != null && listSeedGrow.Count > 0)
        {
            for (int i = 0; i < listSeedGrow.Count; i++)
            {
                if (listSeedGrow[i].currCropPeriod == SeedGrowComponent.PeriodGrow.Germinate
                    || listSeedGrow[i].currCropPeriod == SeedGrowComponent.PeriodGrow.GrowUp)
                {
                    listSeedGrow23.Add(listSeedGrow[i]);
                }
            }
        }
        if (listSeedGrow23.Count > 0)
        {
            CSEmptyCropSpeed csEmptyCropSpeed = new CSEmptyCropSpeed();
            ManorProtocalHelper.FertilizerAdIncrease(csEmptyCropSpeed, (succ) =>
            {
                //关闭UI
                uiWorldHandleManager.SetHandleTileUIClose();
                StaticData.playerInfoData.userInfo.CropCount++;
                if (StaticData.playerInfoData.userInfo.CropCount <= 0)
                {
                    StaticData.playerInfoData.userInfo.CropCount = 0;
                }
                UpdateFertilizerAdCount();
                for (int i = 0; i < listSeedGrow23.Count; i++)
                {
                    SeedGrowComponent.PeriodGrow nextPeriod = SeedGrowComponent.PeriodGrow.Seed;
                    float remainTimeToRipe= listSeedGrow23[i].remainTimeGerminateToRipe / 2f;
                    float remainTime = listSeedGrow23[i].remainTime- remainTimeToRipe;
                    if (remainTime <= 0)
                    {
                        //下一个时期
                        switch (listSeedGrow23[i].currCropPeriod)
                        {
                            case SeedGrowComponent.PeriodGrow.Seed:
                                nextPeriod = SeedGrowComponent.PeriodGrow.Germinate;
                                break;
                            case SeedGrowComponent.PeriodGrow.Germinate:
                                remainTime = remainTimeToRipe;
                                nextPeriod = SeedGrowComponent.PeriodGrow.GrowUp;
                                break;
                            case SeedGrowComponent.PeriodGrow.GrowUp:
                                remainTime = remainTimeToRipe;
                                nextPeriod = SeedGrowComponent.PeriodGrow.Ripe;
                                break;
                            case SeedGrowComponent.PeriodGrow.Ripe:
                                break;
                        }
                        //remainTime = StaticData.GetSeedGrowComponentTotalSecond(listSeedGrow23[i], nextPeriod);
                        
                    }
                    else
                    {
                        nextPeriod = listSeedGrow23[i].currCropPeriod;
                    }

                    listSeedGrow23[i].SetPeriod(nextPeriod, (long)remainTime * 1000);
                }
            }, (error) =>
            {
                uiWorldHandleManager.SetHandleTileUIClose();
            });
        }
        else
        {
            //关闭UI
            uiWorldHandleManager.SetHandleTileUIClose();
        }
    }

    private void OnButtonFertilizerClick(TypeFertilizer typeFertilizer)
    {
        CSWareHouseStruct currFertilizerData = null;
        switch (typeFertilizer)
        {
            case TypeFertilizer.fertilizer2:
                currFertilizerData = StaticData.GetFertilizerCountByWhich(1);//第二种化肥
                break;
            case TypeFertilizer.fertilizer3:
                currFertilizerData = StaticData.GetFertilizerCountByWhich(2);//第二种化肥
                break;
            default:
                break;
        }
        if (currFertilizerData.GoodNum <= 0)
        {
            //关闭UI
            uiWorldHandleManager.SetHandleTileUIClose();
            string FertilizerName = StaticData.GetMultiLanguageByGameItemId(currFertilizerData.GoodId);
            string Tips = string.Format(StaticData.GetMultilingual(120068), FertilizerName);
            StaticData.OpenCommonTips(Tips, 120010, async () =>
            {
                //跳转商城
                await StaticData.OpenShopUI(1);
            });
            return;
        }
        CSFertilizer csFertilizer = new CSFertilizer()
        {
            SoilId = seedGrowComponent.tileComponent.SoilId,
            FertilizerId = currFertilizerData.GoodId
        };
        ManorProtocalHelper.UseFertilizer(csFertilizer, async (succ) =>
        {
            //关闭UI
            uiWorldHandleManager.SetHandleTileUIClose();
            //施肥特效
            string iconFretilizer = string.Empty;
            int propId = csFertilizer.FertilizerId;
            iconFretilizer = StaticData.configExcel.GetGameItemByID(propId).Icon;
            uiWorldHandleManager.uiFertilizerEffectComponent.ShowInfo(iconFretilizer);
            uiWorldHandleManager.uiFertilizerEffectComponent.GetComponent<RectTransform>().anchoredPosition = StaticData.ManorWorldPointToUICameraAnchorPos(seedGrowComponent.tileComponent.transform.position);
            uiWorldHandleManager.uiFertilizerEffectComponent.gameObject.SetActive(true);
            uiWorldHandleManager.isFertiliezeringAnimPlay = true;
            await UniTask.Delay(1000);
            uiWorldHandleManager.uiFertilizerEffectComponent.gameObject.SetActive(false);
            uiWorldHandleManager.isFertiliezeringAnimPlay = false;
            //end 施肥特效
            SeedGrowComponent.PeriodGrow nextPeriod = SeedGrowComponent.PeriodGrow.Seed;
            var remainTime = seedGrowComponent.remainTime - StaticData.GetFertilizerAddTimeMilliSeconds(currFertilizerData.GoodId) / 1000f;
            if (remainTime <= 0)
            {
                //下一个时期
                switch (seedGrowComponent.currCropPeriod)
                {
                    case SeedGrowComponent.PeriodGrow.Seed:
                        nextPeriod = SeedGrowComponent.PeriodGrow.Germinate;
                        break;
                    case SeedGrowComponent.PeriodGrow.Germinate:
                        nextPeriod = SeedGrowComponent.PeriodGrow.GrowUp;
                        //成长期减去对应的时间
                        remainTime = StaticData.GetSeedGrowComponentTotalSecond(seedGrowComponent, nextPeriod) + remainTime;
                        if (remainTime <= 0)
                        {
                            //跳过了成长期,直接到成熟期
                            remainTime = 0f;
                            nextPeriod = SeedGrowComponent.PeriodGrow.Ripe;
                        }
                        break;
                    case SeedGrowComponent.PeriodGrow.GrowUp:
                        nextPeriod = SeedGrowComponent.PeriodGrow.Ripe;
                        break;
                    case SeedGrowComponent.PeriodGrow.Ripe:
                        //如果成熟阶段继续施肥还是成熟阶段
                        nextPeriod = SeedGrowComponent.PeriodGrow.Ripe;
                        break;
                }
                
            }
            else
            {
                nextPeriod = seedGrowComponent.currCropPeriod;
            }

            seedGrowComponent.SetPeriod(nextPeriod, (long)remainTime * 1000);
            //化肥数量-1
            StaticData.UpdateFertilizerMinus1(currFertilizerData.GoodId);
        }, (error) =>
        {
            //关闭UI
            uiWorldHandleManager.SetHandleTileUIClose();
        });
    }

    // Update is called once per frame
    void Update()
    {
        var secondFertilizer = StaticData.GetFertilizerCountByWhich(1);//第二种化肥
        if (secondFertilizer.GoodNum > 0)
        {
            TextNumNormal2.gameObject.SetActive(true);
            TextNumNormal2.text = $"{secondFertilizer.GoodNum}";
            TextNumZero2.gameObject.SetActive(false);
        }
        else
        {
            TextNumNormal2.gameObject.SetActive(false);
            TextNumZero2.gameObject.SetActive(true);
        }

        var thirdFertilizer = StaticData.GetFertilizerCountByWhich(2);//第三种化肥
        if (thirdFertilizer.GoodNum > 0)
        {
            TextNumNormal3.gameObject.SetActive(true);
            TextNumNormal3.text = $"{thirdFertilizer.GoodNum}";
            TextNumZero3.gameObject.SetActive(false);
        }
        else
        {
            TextNumNormal3.gameObject.SetActive(false);
            TextNumZero3.gameObject.SetActive(true);
        }

        //显示倒计时
        if (seedGrowComponent.remainTimeGerminateToRipe <= 0f)
        {
            seedGrowComponent.remainTimeGerminateToRipe = 0f;
        }
        int hour = (int)seedGrowComponent.remainTimeGerminateToRipe / 3600;
        int minute = (int)seedGrowComponent.remainTimeGerminateToRipe % 3600 / 60;
        int remainSeconds = (int)(seedGrowComponent.remainTimeGerminateToRipe % 60f);
        //string PeriodLocalName = StaticData.GetMultilingual(120078);
        switch (seedGrowComponent.currCropPeriod)
        {
            case SeedGrowComponent.PeriodGrow.Seed:
                //PeriodLocalName = StaticData.GetMultilingual(120078);
                break;
            case SeedGrowComponent.PeriodGrow.Germinate:
                //PeriodLocalName = StaticData.GetMultilingual(120079);
                TextNextPeriodTip.gameObject.SetActive(false);
                TextGainTip.gameObject.SetActive(true);
                break;
            case SeedGrowComponent.PeriodGrow.GrowUp:
                //PeriodLocalName = StaticData.GetMultilingual(120080);
                TextNextPeriodTip.gameObject.SetActive(false);
                TextGainTip.gameObject.SetActive(true);
                break;
        }
        TextRemainTimeCurrPeriod.text = $"{string.Format("{0:00}:{1:00}:{2:00}", hour,minute, remainSeconds)}";
    }
}
