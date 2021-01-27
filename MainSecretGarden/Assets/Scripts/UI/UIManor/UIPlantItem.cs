using Cysharp.Threading.Tasks;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static StaticData;

public class UIPlantItem : MonoBehaviour, InterfaceScrollCell
{
    public UIPanelDrag uiPanelDrag;
    [HideInInspector]
    public GameObject goPrefab;
    [HideInInspector]
    public int CropGoodId;
    GameObject goPlant;
    [Header("UI设置")]
    public Text TextLevel;
    public Image imageIcon;
    public GameObject goNum;
    public Text TextNum;
    public GameObject goPrice;
    public Image imageCoin;
    public Text TextPrice;
    public Text TextTime;
    public GameObject goBgRarity1;
    public GameObject goBgRarity2;
    public GameObject goDealNeed;
    PlantSeed currPlantSeed;

    // Start is called before the first frame update
    void Start()
    {
        uiPanelDrag.actionOnClick = OnPlantClick;
        uiPanelDrag.actionFromUpDrag = OnDragOutFromPanel;
        uiPanelDrag.actionFromBottomDrag = OnDragOutFromPanel;
    }

    public void ToBuyCurrency()
    {
        //关闭UI
        StaticData.GetUIWorldHandleComponent().SetHandleTileUIClose();
        //string SeedName = StaticData.GetMultiLanguageByGameItemId(CropGoodId);
        string CurrencyName = StaticData.GetMultiLanguageByGameItemId(currPlantSeed.coinPriceId);
        string Tips = string.Format(StaticData.GetMultilingual(120068), CurrencyName);
        StaticData.OpenCommonTips(Tips, 120010, async () => {
            ////跳转商城
            //await StaticData.OpenShopUI(0);
            //跳转金币商店
            if (currPlantSeed.coinPriceId==StaticData.configExcel.GetVertical().GoldGoodsId)
            {
                await StaticData.OpenRechargeUI(0);
            } else if (currPlantSeed.coinPriceId == StaticData.configExcel.GetVertical().JewelGoodsId)
            {
                await StaticData.OpenRechargeUI(1);
            }
        });
    }

    public async void ScrollCellIndex(int idx)
    {
        //数据
        var listSeed = StaticData.GetPlantSeeds();
        currPlantSeed = listSeed[idx];
        if (currPlantSeed == null)
        {
            return;
        }
        await SetUIInfo();
        this.CropGoodId = currPlantSeed.idSeed;
        var manorCropDefine=StaticData.configExcel.ManorCrop.Find(x => x.IdSeed == currPlantSeed.idSeed);
        var gameItemDefine = StaticData.configExcel.GetGameItemByID(manorCropDefine.IdGainGameItem);
        goPrefab = await ABManager.GetAssetAsync<GameObject>(manorCropDefine.Model);
    }

    private async UniTask SetUIInfo()
    {
        TextLevel.text=$"{currPlantSeed.level}";
        imageIcon.sprite=await ABManager.GetAssetAsync<Sprite>(currPlantSeed.iconName);
        goNum.SetActive(currPlantSeed.numSeed>0);
        goPrice.SetActive(currPlantSeed.numSeed <= 0);
        TextNum.text = $"{currPlantSeed.numSeed}";
        goBgRarity1.SetActive(false);
        goBgRarity2.SetActive(false);
        goDealNeed.SetActive(false);
        if (currPlantSeed.isRarity == false)
        {
            imageCoin.sprite = await ABManager.GetAssetAsync<Sprite>(currPlantSeed.coinName);
            TextPrice.text = $"{currPlantSeed.price}";
            TextPrice.color = new Color32(255, 255, 255, 255);
            //判定金币是否足够
            if (StaticData.GetWareHouseGold()< currPlantSeed.price)
            {//显示红色
                TextPrice.color = new Color32(255, 0, 0, 255);
            }
            goBgRarity1.SetActive(true);
        }
        else
        {
            goBgRarity2.SetActive(true);
        }
        if (currPlantSeed.isDealNeed)
        {
            goDealNeed.SetActive(true);
        }
        TextTime.text = $"{currPlantSeed.timeShow}";
}

    public void OnDragUp()
    {
        PlantSeedDragComponent plantSeedDragComponent = StaticData.GetUIWorldHandleComponent().plantSeedDragComponent;
        plantSeedDragComponent.gameObject.SetActive(false);
        plantSeedDragComponent.isDrag = false;
        //种植
        if (Root2dSceneManager._instance.PlantData.PlantInfo.Count>0)
        {
            //种植
            ManorProtocalHelper.ManorPlant(Root2dSceneManager._instance.PlantData, (succ) => {
                StaticData.UpdateSeedMinus1(Root2dSceneManager._instance.PlantData);
                Root2dSceneManager._instance.PlantData.PlantInfo.Clear();
                //更新货币
                if (DragWillCostCoin > 0)
                {
                    StaticData.UpdateWareHouseItem(currPlantSeed.coinPriceId, -DragWillCostCoin);
                }
            });
        }
    }

    List<Collider2D> listOverlapPointCollider = new List<Collider2D>();
    private void OnDragOutFromPanel(PointerEventData eventData)
    {
        int seedCount = StaticData.GetWareHouseItem(CropGoodId).GoodNum;
        if (seedCount <= 0)
        {
            //判定金钱
            if (StaticData.GetWareHouseItem(currPlantSeed.coinPriceId).GoodNum < currPlantSeed.price)
            {
                ToBuyCurrency();
                uiPanelDrag.OnEndDrag(eventData);
                return;
            }
        }
        PlantSeedDragComponent plantSeedDragComponent= StaticData.GetUIWorldHandleComponent().plantSeedDragComponent;
        plantSeedDragComponent.gameObject.SetActive(true);
        uiPanelDrag.OnEndDrag(eventData);
        StaticData.GetUIWorldHandleComponent().SetHandleTileUIClose();
        plantSeedDragComponent.GetComponent<Image>().sprite = uiPanelDrag.GetComponent<Image>().sprite;
        plantSeedDragComponent.BeginDrag(this);
    }

    int DragNeedSeedCount;
    int DragWillCostCoin=0;
    public void ResetDragNeedSeedCount()
    {
        DragNeedSeedCount = 1;
        DragWillCostCoin = 0;
    }
    public void HandlePlant()
    {
        //划过的地块都种植
        var worldPoint = Root2dSceneManager._instance.worldCameraComponent.cameraWorld.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] colliders = Physics2D.OverlapPointAll(worldPoint);
        listOverlapPointCollider.Clear();
        //colliders 转list
        for (int i = 0; i < colliders.Length; i++)
        {
            listOverlapPointCollider.Add(colliders[i]);
        }

        Collider2D colliderTileCom = colliderTileCom = listOverlapPointCollider.Find(x => x.gameObject.CompareTag(TagHelper.Tile));
        if (colliderTileCom != null
            && colliderTileCom.GetComponent<TileComponent>()!=null
            && !colliderTileCom.GetComponent<TileComponent>().isNPC
            && colliderTileCom.GetComponent<TileComponent>().CropGoodId==0)
        {

            StaticData.GetUIWorldHandleComponent().SetHandleTileUIClose();
            int haveSeedCount = StaticData.GetWareHouseItem(CropGoodId).GoodNum;
            //判定当前种子是否还够
            if (DragNeedSeedCount <= haveSeedCount)
            {
                DragNeedSeedCount++;
            }
            else
            {
                if (currPlantSeed.isRarity)
                {
                    //稀有种子直接移除
                    Destroy(gameObject);
                    //向服务器发消息种植，关闭拖拽的UI
                    OnDragUp();
                    Root2dSceneManager._instance.isTileDrag = false;
                    return;
                }

                //金钱加上
                DragWillCostCoin += (int)currPlantSeed.price;
                //判定金钱
                if (StaticData.GetWareHouseItem(currPlantSeed.coinPriceId).GoodNum < DragWillCostCoin)
                {
                    //减掉最后一个的钱
                    //金钱加上
                    DragWillCostCoin -= (int)currPlantSeed.price;
                    //向服务器发消息种植，关闭拖拽的UI
                    OnDragUp();
                    Root2dSceneManager._instance.isTileDrag = false;
                    return;
                }
            }
            //实例化
            goPlant = GameObject.Instantiate<GameObject>(goPrefab);
            CSPlantStruct csPlantStruct = new CSPlantStruct()
            {
                SoilId = colliderTileCom.GetComponent<TileComponent>().SoilId,
                CropGoodId = this.CropGoodId
            };
            var tileComponent = colliderTileCom.GetComponent<TileComponent>();
            if (tileComponent.typeTile == Company.Cfg.TypeManorDecorate.Tile)
            {
                tileComponent.Plant(goPlant, csPlantStruct, () => {
                    //设置地块上庄稼的id
                    tileComponent.CropGoodId = this.CropGoodId;
                    AfterPlantCropSetInfo(tileComponent);
                });
            }
        }
    }

    private void AfterPlantCropSetInfo(TileComponent tileComponent)
    {
        //设置庄稼
        var seedGrowCom = goPlant.GetComponent<SeedGrowComponent>();
        seedGrowCom.SetCropId(CropGoodId, tileComponent);
        seedGrowCom.GenerateTimer();
        seedGrowCom.SetPeriod(SeedGrowComponent.PeriodGrow.Seed);
        Root2dSceneManager._instance.AddListSeedGrow(seedGrowCom);
    }

    private void OnPlantClick(PointerEventData eventData)
    {
        int seedCount = StaticData.GetWareHouseItem(CropGoodId).GoodNum;
        int costCoinCount = 0;
        if (seedCount <= 0)
        {
            //判定金钱
            if (StaticData.GetWareHouseItem(currPlantSeed.coinPriceId).GoodNum< currPlantSeed.price)
            {
                ToBuyCurrency();
                return;
            }
            costCoinCount = (int)currPlantSeed.price;
        }
        UIWorldHandleManager uiUIWorldHandleComponent = StaticData.GetUIWorldHandleComponent();
        //实例化
        goPlant = GameObject.Instantiate<GameObject>(goPrefab);
        CSPlantStruct csPlantStruct = new CSPlantStruct()
         {
             SoilId = uiUIWorldHandleComponent.currClickComponent.SoilId,
             CropGoodId = this.CropGoodId
         };
        if (uiUIWorldHandleComponent.currClickComponent!=null)
        {
            uiUIWorldHandleComponent.currClickComponent.Plant(goPlant, csPlantStruct,()=> {
                //设置地块上庄稼的id
                uiUIWorldHandleComponent.currClickComponent.CropGoodId = this.CropGoodId;
                AfterPlantCropSetInfo(uiUIWorldHandleComponent.currClickComponent);
            });
            
        }
        //种植
        ManorProtocalHelper.ManorPlant(Root2dSceneManager._instance.PlantData, (succ) => {
            StaticData.UpdateSeedMinus1(Root2dSceneManager._instance.PlantData);
            //更新货币
            if (costCoinCount > 0)
            {
                StaticData.UpdateWareHouseItem(currPlantSeed.coinPriceId, -costCoinCount);
            }
            uiUIWorldHandleComponent.SetHandleTileUIClose();
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            uiPanelDrag.m_DragPlane.GetComponent<Image>().enabled = true;
        }
    }
}
