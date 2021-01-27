using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedGrowComponent : MonoBehaviour
{
    public enum PeriodGrow
    {
        Seed=0,
        Germinate,//发芽
        GrowUp,//成长
        Ripe//成熟期
    }
    public PeriodGrow currCropPeriod;
    public GameObject goSpriteSeed;
    public GameObject goSpriteGerminate;
    public GameObject goSpriteGrowUp;
    public GameObject goSpriteRipe;

    [HideInInspector]
    public int cropId;
    [HideInInspector]
    public TileComponent tileComponent;
    [HideInInspector]
    public float remainTime;
    //幼苗期到成熟的时间
    [HideInInspector]
    public float remainTimeGerminateToRipe;
    string seedName = string.Empty;
    Text textRemainTime;
    TimeCountDownComponent timeCountDownComponentSeed;
    TimeCountDownComponent timeCountDownComponentGerminate;
    TimeCountDownComponent timeCountDownComponentGrowUp;
    private void Awake()
    {
        
    }

    public void GenerateTimer()
    {
        //创建空的计时器
        timeCountDownComponentSeed = StaticData.CreateTimer(100000 / 1000f, false, (go) =>
        {
        },
        (remainTime) =>
        {
        }, "Seed");
        timeCountDownComponentGerminate = StaticData.CreateTimer(100000 / 1000f, false, (go) =>
        {
        },
        (remainTime) =>
        {
        }, "Germinate");
        timeCountDownComponentGrowUp = StaticData.CreateTimer(100000 / 1000f, false, (go) =>
        {
        },
        (remainTime) =>
        {
        }, "GrowUp");
    }

    public void SetCropId(int cropId, TileComponent tileComponent)
    {
        this.cropId = cropId;
        this.tileComponent = tileComponent;
    }
    public async void SetPeriod(PeriodGrow peroid, long currPeriodRemainTimeMilliseconds = -1)
    {
        if (goSpriteSeed != null)
        {
            goSpriteSeed.SetActive(peroid == PeriodGrow.Seed);
            goSpriteGerminate.SetActive(peroid == PeriodGrow.Germinate);
            goSpriteGrowUp.SetActive(peroid == PeriodGrow.GrowUp);
            goSpriteRipe.SetActive(peroid == PeriodGrow.Ripe);
        }
        currCropPeriod = peroid;
        
        long currPeriodRemainMilliSecond=-1;
        if (currPeriodRemainTimeMilliseconds != -1)//表示服务器过来的
        {
            currPeriodRemainMilliSecond = currPeriodRemainTimeMilliseconds;
        }
        long remainSeconds = StaticData.GetSeedGrowComponentTotalSecond(this,currCropPeriod);
        long remainSecondsGrowUp = StaticData.GetSeedGrowComponentTotalSecond(this, PeriodGrow.GrowUp);
        if (peroid!=PeriodGrow.Seed)
        {
            //如果不是种子期，删除化肥
            StaticData.GetUIWorldHandleComponent().DestroyFirstStageFertilizer(tileComponent);
        }
        switch (peroid)
        {
            case PeriodGrow.Seed:
                if (currPeriodRemainMilliSecond == -1)
                {
                    currPeriodRemainMilliSecond = remainSeconds * 1000;
                }
                timeCountDownComponentSeed.Init(currPeriodRemainMilliSecond / 1000f, false, (go) =>
                {
                    SetPeriod(PeriodGrow.Germinate);
                    Destroy(go);
                },
                (remainTime) =>
                {
                    this.remainTime = remainTime;
                });
                //种子期显示化肥
                StaticData.GetUIWorldHandleComponent().SetFirstStageFertilizer(this);
                break;
            case PeriodGrow.Germinate:
                StaticData.DestroySafe(timeCountDownComponentSeed);
                if (currPeriodRemainMilliSecond == -1)
                {
                    currPeriodRemainMilliSecond = remainSeconds * 1000;
                }
                timeCountDownComponentGerminate.Init(currPeriodRemainMilliSecond / 1000f, false, (go) =>
                {
                    SetPeriod(PeriodGrow.GrowUp);
                    Destroy(go);
                },
                (remainTime) =>
                {
                    this.remainTime = remainTime;
                    this.remainTimeGerminateToRipe = remainTime+ remainSecondsGrowUp;
                });
                break;
            case PeriodGrow.GrowUp:
                StaticData.DestroySafe(timeCountDownComponentSeed);
                StaticData.DestroySafe(timeCountDownComponentGerminate);
                if (currPeriodRemainMilliSecond == -1)
                {
                    currPeriodRemainMilliSecond = remainSeconds * 1000;
                }
                timeCountDownComponentGrowUp.Init(currPeriodRemainMilliSecond / 1000f, false, (go) =>
                {
                    SetPeriod(PeriodGrow.Ripe);
                    Destroy(go);
                }, (remainTime) =>
                {
                    this.remainTime = remainTime;
                    this.remainTimeGerminateToRipe = remainTime;
                });
                break;
            case PeriodGrow.Ripe:
                StaticData.DestroySafe(timeCountDownComponentSeed);
                StaticData.DestroySafe(timeCountDownComponentGerminate);
                StaticData.DestroySafe(timeCountDownComponentGrowUp);
                
                if (!Root2dSceneManager._instance.isFriendManor)
                {//自己
                    //获取对应目录下的收获图标
                    StaticData.GetUIWorldHandleComponent().CreateGain(this);
                }
                else
                {//好友
                    List<UIWorldHandleManager.StealClass> listStealClass = StaticData.GetUIWorldHandleComponent().listStealClass;
                    var StealClass=listStealClass.Find(x => x.SolidId == tileComponent.SoilId);
                    if (StealClass!=null&&StealClass.isSteal)
                    {
                        StaticData.GetUIWorldHandleComponent().CreateSteal(this);
                    }
                }
                break;
        }
    }

    
}
