using Cysharp.Threading.Tasks;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemFirstStageFertilizerComponent : MonoBehaviour
{
    public Button ButtonFertilizer;
    public Text TextNumNormal;
    public Text TextNumZero;
    [HideInInspector]
    public SeedGrowComponent seedGrowComponent;
    // Start is called before the first frame update
    void Start()
    {
        UIManorComponent uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
        uiManorComponent.SetCanWateringOnekey();
        TextNumNormal.gameObject.SetActive(false);
        TextNumZero.gameObject.SetActive(false);
        ButtonFertilizer.onClick.RemoveAllListeners();
        ButtonFertilizer.onClick.AddListener(OnButtonFertilizerClick);
    }

    public void OnButtonFertilizerClick()
    {
        if (Root2dSceneManager._instance.isFriendManor)
        {//好友庄园不可操作
            return;
        }
        var firstFertilizer = StaticData.GetFertilizerCountByWhich(0);
        if (firstFertilizer.GoodNum <= 0)
        {
            //关闭UI
            StaticData.GetUIWorldHandleComponent().SetHandleTileUIClose();
            string FertilizerName = StaticData.GetMultiLanguageByGameItemId(firstFertilizer.GoodId);
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
            FertilizerId = firstFertilizer.GoodId
        };
        ManorProtocalHelper.UseFertilizer(csFertilizer, async (succ) =>
        {
            //第一阶段使用化肥
            if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.CurrExecuteGuideLittleStepDefine.Id== 10004)
            {
                GuideCanvasComponent._instance.SetLittleStepFinish();
            }
            //点击浇水的时候，关闭水滴
            ButtonFertilizer.gameObject.SetActive(false);
            TextNumNormal.gameObject.SetActive(false);
            TextNumZero.gameObject.SetActive(false);
            //关闭UI
            StaticData.GetUIWorldHandleComponent().SetHandleTileUIClose();
            //施肥特效
            string iconFretilizer = string.Empty;
            int propId = csFertilizer.FertilizerId;
            iconFretilizer = StaticData.configExcel.GetGameItemByID(propId).Icon;
            StaticData.GetUIWorldHandleComponent().PlayWateringAnim(seedGrowComponent.tileComponent);
            //播放音效点击
            GameSoundPlayer.Instance.PlaySoundEffect(MusicHelper.SoundEffectWatering);
            //end 施肥特效
            SeedGrowComponent.PeriodGrow nextPeriod = SeedGrowComponent.PeriodGrow.Germinate;//种子的下一时期，幼苗期
            //直接进入下一个时期
            float remainTime = StaticData.GetSeedGrowComponentTotalSecond(seedGrowComponent, nextPeriod);
            seedGrowComponent.SetPeriod(nextPeriod, (long)remainTime * 1000);
            //化肥数量-1
            StaticData.UpdateFertilizerMinus1(propId);
            //设置是否能一键浇水
            UIManorComponent uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
            uiManorComponent.SetCanWateringOnekey();
        }, (error) =>
        {
            //关闭UI
            StaticData.GetUIWorldHandleComponent().SetHandleTileUIClose();
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (seedGrowComponent != null)
        {
            UIWorldHandleManager uiWorldHandleManager=StaticData.GetUIWorldHandleComponent();
            if (uiWorldHandleManager != null)
            {
                //设置位置
                uiWorldHandleManager.SetWorldPos(seedGrowComponent.tileComponent, gameObject, UIWorldHandleManager.TypePointUI.FirstFertilizer);
            }
        }
        if (Root2dSceneManager._instance == null)
        {
            return;
        }
        if (Root2dSceneManager._instance.isFriendManor|| seedGrowComponent==null)
        {
            if (ButtonFertilizer != null)
            {
                ButtonFertilizer.gameObject.SetActive(false);
            }
            
            return;
        }
        if (ButtonFertilizer != null)
        {
            ButtonFertilizer.gameObject.SetActive(true);
        }
        //var firstFertilizer=StaticData.GetFertilizerCountByWhich(0);
        //if (firstFertilizer.GoodNum > 0)
        //{
        //    TextNumNormal.gameObject.SetActive(true);
        //    TextNumNormal.text = $"{firstFertilizer.GoodNum}";
        //    TextNumZero.gameObject.SetActive(false);
        //}
        //else
        //{
        //    TextNumNormal.gameObject.SetActive(false);
        //    TextNumZero.gameObject.SetActive(true);
        //}
        TextNumNormal.gameObject.SetActive(false);
        TextNumZero.gameObject.SetActive(false);
    }
}
