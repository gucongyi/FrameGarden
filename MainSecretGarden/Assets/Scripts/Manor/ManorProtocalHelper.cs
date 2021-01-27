using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class ManorProtocalHelper
{
    //获取自己庄园的信息
    public static void GetSelfManorInfo(CSEmptyManorInfo csEmptyManorInfo, Action<SCManorData> ResponseSCManorDataCallBack)
    {
        if (StaticData.IsUsedLocalDataNotServer)
        {
            //测试
            SCManorData scManorData = new SCManorData();
            SCManorStruct scManorStruct = new SCManorStruct()
            {
                SoilId = 1001,
                SoilType = ManorScene.Tile,
                CropGoodId = 0,
                Yield = 0,
                TotalYield = 0,
                SoilStatus = 0,
                Xaxle = -8.4338960647583f,
                Yaxle = 12.0527410507202f,
                NextTime = 0,
                ParcelDivision = 1
            };
            scManorData.ManorInfo.Add(scManorStruct);
            ResponseSCManorDataCallBack(scManorData);
        } else
        {
            //这里处理默认的提示问题
            ProtocalManager.Instance().SendCSEmptyManorInfo(csEmptyManorInfo, ResponseSCManorDataCallBack, (error) => { }, false);
        }

    }

    //拖拽位置创建
    public static void CreateManorGoByDrag(CSDrag csDrag, Action<SCDrag> ResponseSCDragCallBack)
    {
        if (StaticData.IsUsedLocalDataNotServer)
        {
            //测试
            SCDrag scDrag = new SCDrag() { SoilId = 1002 };
            ResponseSCDragCallBack(scDrag);
        } else
        {
            ProtocalManager.Instance().SendCSDrag(csDrag, ResponseSCDragCallBack, (error) => { }, false);
        }
    }

    //移动位置
    public static void MoveManorGo(CSChangeLocation csChangeLocation, Action<SCEmptChangeLocationData> ResponseSCEmptChangeLocationDataCallBack)
    {
        if (StaticData.IsUsedLocalDataNotServer)
        {
            //测试
            SCEmptChangeLocationData scEmptChangeLocationData = new SCEmptChangeLocationData();
            ResponseSCEmptChangeLocationDataCallBack(scEmptChangeLocationData);

        }
        else
        {
            ProtocalManager.Instance().SendCSChangeLocation(csChangeLocation, ResponseSCEmptChangeLocationDataCallBack, (error) => { }, false);
        }
    }

    //种植
    public static void ManorPlant(CSPlantData csPlantData, Action<SCPlantResult> ResponseSCPlantResultCallBack)
    {
        if (StaticData.IsUsedLocalDataNotServer)
        {
            //测试
            StaticData.DebugGreen($"种植：csPlantData：{csPlantData.ToString()}");
            SCPlantResult scPlantResult = new SCPlantResult() { };
            ResponseSCPlantResultCallBack(scPlantResult);
        } else
        {
            ProtocalManager.Instance().SendCSPlantData(csPlantData, (succ)=> {
                //点击种子面板播种
                if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.isCurrStepGuiding)
                {
                    //施肥特殊判定，不干扰后边
                    if (GuideCanvasComponent._instance.CurrExecuteGuideLittleStepDefine.Id == 10003)
                    {
                        GuideCanvasComponent._instance.SetLittleStepFinish();
                    }
                }
                ResponseSCPlantResultCallBack(succ);
            }, (error) => { }, false);
        }
    }

    //提前铲除
    public static void ManorEradicatePlant(CSEradicate csEradicate, Action<SCEmptEradicate> ResponseSCEmptEradicateCallBack)
    {
        if (StaticData.IsUsedLocalDataNotServer)
        {
            //测试
            SCEmptEradicate scEmptEradicate = new SCEmptEradicate();
            ResponseSCEmptEradicateCallBack(scEmptEradicate);
        } else
        {
            ProtocalManager.Instance().SendCSEradicate(csEradicate, ResponseSCEmptEradicateCallBack, (error) => { }, false);
        }
    }

    //施肥
    public static void UseFertilizer(CSFertilizer csFertilizer, Action<SCEmptFertilizer> ResponseSCEmptFertilizerCallBack,Action<ErrorInfo> errorCallBack)
    {
        if (StaticData.IsUsedLocalDataNotServer)
        {
            //测试
            SCEmptFertilizer SCEmptFertilizer = new SCEmptFertilizer();
            ResponseSCEmptFertilizerCallBack(SCEmptFertilizer);
        } else
        {
            ProtocalManager.Instance().SendCSFertilizer(csFertilizer, ResponseSCEmptFertilizerCallBack, errorCallBack, true);
        }
    }
    //广告加速使用化肥
    public static void FertilizerAdIncrease(CSEmptyCropSpeed csEmptyCropSpeed, Action<SCEmptyCropSpeed> ResponseSCEmptyCropSpeedCallBack, Action<ErrorInfo> errorCallBack)
    {
        if (StaticData.IsUsedLocalDataNotServer)
        {
            //测试
            SCEmptyCropSpeed scEmptyCropSpeed = new SCEmptyCropSpeed();
            ResponseSCEmptyCropSpeedCallBack(scEmptyCropSpeed);
        }
        else
        {
            ProtocalManager.Instance().SendCSEmptyCropSpeed(csEmptyCropSpeed, ResponseSCEmptyCropSpeedCallBack, errorCallBack, true);
        }
    }

    //收获
    public static void ManorHarvest(CSHarvestData csHarvestData, Action<SCHarvestData> ResponseSCHarvestDataCallBack)
    {
        if (StaticData.IsUsedLocalDataNotServer)
        {
            SCHarvestStruct sc1 = new SCHarvestStruct()
            {
                SoilId = 1001,
                HarvestId= 500001,
                HarvestNum=30,
                HarvestExperience=20
            };
            //测试
            SCHarvestData scHarvestData = new SCHarvestData();
            scHarvestData.HarvestResult.Add(sc1);
            ResponseSCHarvestDataCallBack(scHarvestData);
        } else
        {
            ProtocalManager.Instance().SendCSHarvestData(csHarvestData, async (succ)=>
            {
                //保存本地数据 收获总数量
                for (int i = 0; i < succ.HarvestResult.Count; i++)
                {
                    LocalInfoData.localManorInfoData.countTotalGain += succ.HarvestResult[i].HarvestNum;
                }
                JsonHelper.SaveDataToPersist<LocalManorInfoData>(LocalInfoData.localManorInfoData);
                StaticData.DebugGreen($"==============GainOneTile id:{csHarvestData.HarvestInfo[0].SoilId}  middleTime:{Time.realtimeSinceStartup}");
                //播放音效点击
                GameSoundPlayer.Instance.PlaySoundEffect(MusicHelper.SoundEffectReward);
                //收获引导完成
                if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.CurrExecuteGuideLittleStepDefine.Id == 10005)
                {
                    GuideCanvasComponent._instance.SetLittleStepFinish();
                }
                //回调，播放动画
                ResponseSCHarvestDataCallBack(succ);
                await Cysharp.Threading.Tasks.UniTask.Delay(1600);

                
                //播完动画增加经验
                StaticData.AddPlayerExp(succ);
            }, (error) => { }, false);
        }
    }

    public static void TriggerManorRegionUnLock(int preLv)
    {
        int currLevel = StaticData.GetPlayerLevelAndCurrExp().level;
        var areaDefine = StaticData.configExcel.AreaUnlock.Find(x => x.Grade == currLevel);
        bool isTriggerUnLockArea = false;
        int willSetRegionId = 0;
        if (areaDefine != null)
        {
            int willUnLockRegionId = areaDefine.ID;
            //找到之前已经解锁的最大的区域的地块
            int preUnLockRegionId = 1;
            for (int i = 0; i < Root2dSceneManager._instance.UnlockAreaInfoSelf.Count; i++)
            {
                if (Root2dSceneManager._instance.UnlockAreaInfoSelf[i].State == AreaState.RemoveWorkShed)
                {
                    preUnLockRegionId = Root2dSceneManager._instance.UnlockAreaInfoSelf[i].AreaId;
                }
            }
            StaticData.DebugGreen($"当前完全解锁的大地块id：{preUnLockRegionId} 进入对话状态");
            if ((preUnLockRegionId + 1) == willUnLockRegionId)
            {//刚好差了一个
                isTriggerUnLockArea = true;
                willSetRegionId = willUnLockRegionId;
            }
            else if (willUnLockRegionId > (preUnLockRegionId + 1))
            {//跨区了，提示
                //取消提示 跟汪沟通2021.1.11
                //StaticData.CreateToastTips($"先解锁{preUnLockRegionId + 1}区域！");
            }
        }
        StaticData.DetectionUpgrade(preLv, isTriggerUnLockArea, willSetRegionId);
    }



    //解锁大区域
    public static void UnlockRegion(CSUnlockArea csUnlockArea, Action<SCUnlockArea> ResponseSCUnlockAreaCallBack)
    {
        if (StaticData.IsUsedLocalDataNotServer)
        {
            //测试
            SCUnlockArea scUnlockArea = new SCUnlockArea();
            ResponseSCUnlockAreaCallBack(scUnlockArea);
        } else
        {
            ProtocalManager.Instance().SendCSUnlockArea(csUnlockArea, ResponseSCUnlockAreaCallBack, (error) => { }, false);
        }
    }
    //施工加速
    public static void WorkLoadSpeedUp(CSWorkShedSpeedUp csWorkShedSpeedUp, Action<SCWorkShedSpeedUp> ResponseSCWorkShedSpeedUpCallBack)
    {
        if (StaticData.IsUsedLocalDataNotServer)
        {
            //测试
            SCWorkShedSpeedUp scWorkShedSpeedUp = new SCWorkShedSpeedUp();
            scWorkShedSpeedUp.UnlockTime = 1599792302000;
            ResponseSCWorkShedSpeedUpCallBack(scWorkShedSpeedUp);
        }
        else
        {
            ProtocalManager.Instance().SendCSWorkShedSpeedUp(csWorkShedSpeedUp, ResponseSCWorkShedSpeedUpCallBack, (error) => { }, false);
        }
    }

    //查看好友信息
    public static void LookFriendManorInfo(CSQueryOther csQueryOther, Action<SCManorFriendData> ResponseSCManorFriendDataCallBack)
    {
        if (StaticData.IsUsedLocalDataNotServer)
        {
            //测试
            SCManorFriendData scManorFriendData = new SCManorFriendData() { };
            SCManorStruct scManorStruct = new SCManorStruct()
            {
                SoilId = 1001,
                SoilType = ManorScene.Tile,
                CropGoodId = 0,
                Yield = 0,
                TotalYield = 0,
                SoilStatus = 0,
                Xaxle = -8.4338960647583f,
                Yaxle = 12.0527410507202f,
                NextTime = 0,
                ParcelDivision = 1
            };
            scManorFriendData.OtherManorInfo.Add(scManorStruct);
            ResponseSCManorFriendDataCallBack(scManorFriendData);
        } else
        {
            ProtocalManager.Instance().SendCSQueryOther(csQueryOther, ResponseSCManorFriendDataCallBack, (error) => { }, false);
        }
    }

    public static void LookFriendGuide(CSEmptyManorGuidance csEmptyManorGuidance, Action<SCManorGuidance> ResponseSCManorFriendDataCallBack)
    {
        ProtocalManager.Instance().SendCSEmptyManorGuidance(csEmptyManorGuidance, ResponseSCManorFriendDataCallBack, (error) => { }, false);
    }


    public static void StealFriendFruit(CSStealData csStealData, Action<SCStealData> ResponseSCStealDataCallBack)
    {
        if (StaticData.IsUsedLocalDataNotServer)
        {
            //测试
            SCStealData scStealData = new SCStealData();
            SCStealResult scStealResult = new SCStealResult()
            {
                SoilId = 1001,
                StealId = 1001,
                StealNum = 10
            };
            scStealData.StealResult.Add(scStealResult);
            ResponseSCStealDataCallBack(scStealData);
        } else
        {
            if (csStealData.StealUid==1 && StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.CurrExecuteGuideLittleStepDefine.Id == 10024)//新手引导好友庄园
            {
                CSEmptyStealManorGuidance csEmptyStealManorGuidance = new CSEmptyStealManorGuidance();
                ProtocalManager.Instance().SendCSEmptyStealManorGuidance(csEmptyStealManorGuidance, (scStealManorGuidance) => {
                    if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.isCurrStepGuiding)
                    {
                        GuideCanvasComponent._instance.SetLittleStepFinish();
                    }
                    SCStealData sCStealData = new SCStealData();
                    sCStealData.StealResult.AddRange(scStealManorGuidance.StealResult);
                    //设置偷取按钮
                    UIManorComponent uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
                    if (uiManorComponent != null)
                    {
                        uiManorComponent.PlayOneKeyGainEffect(false);
                    }
                    ResponseSCStealDataCallBack.Invoke(sCStealData);
                }, (error) => {
                    //设置偷取按钮
                    UIManorComponent uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
                    if (uiManorComponent != null)
                    {
                        uiManorComponent.PlayOneKeyGainEffect(false);
                    }
                }, false);

                return;
            }
            ProtocalManager.Instance().SendCSStealData(csStealData, ResponseSCStealDataCallBack, (error) => {
            }, true);
        }
    }

    //回收
    public static void SendOrnamentalRecycle(CSOrnamentalRecycle csOrnamentalRecycle, Action<SCEmptyOrnamentalRecycle> ResponseSCEmptyOrnamentalRecycleCallBack)
    {
        if (StaticData.IsUsedLocalDataNotServer)
        {
            SCEmptyOrnamentalRecycle scEmptyOrnamentalRecycle = new SCEmptyOrnamentalRecycle();
            ResponseSCEmptyOrnamentalRecycleCallBack(scEmptyOrnamentalRecycle);
        } else
        {
            ProtocalManager.Instance().SendCSOrnamentalRecycle(csOrnamentalRecycle, ResponseSCEmptyOrnamentalRecycleCallBack, (error) => { }, false);
        }
            
    }
    //装饰物旋转
    public static void SendManorDecorateRotate(CSManorDecorateRotate csManorDecorateRotate,Action<SCEmptyManorDecorateRotate> ResponseSCEmptyManorDecorateRotateCallBack, Action<ErrorInfo> errorCallBack)
    {
        if (StaticData.IsUsedLocalDataNotServer)
        {
            SCEmptyManorDecorateRotate scEmptyManorDecorateRotate = new SCEmptyManorDecorateRotate();
            ResponseSCEmptyManorDecorateRotateCallBack(scEmptyManorDecorateRotate);
        }
        else
        {
            ProtocalManager.Instance().SendCSManorDecorateRotate(csManorDecorateRotate, ResponseSCEmptyManorDecorateRotateCallBack, errorCallBack, false);
        }
    }


    //一键浇水
    public static void SendCSEmptyOnceWatering(CSEmptyOnceWatering csEmptyOnceWatering, Action<SCOnceWatering> ResponseSCSCOnceWateringCallBack)
    {
        if (StaticData.IsUsedLocalDataNotServer)
        {
            SCOnceWatering scOnceWatering = new SCOnceWatering();
            scOnceWatering.SoilId.Add(10001);
            ResponseSCSCOnceWateringCallBack(scOnceWatering);
        }
        else
        {
            ProtocalManager.Instance().SendCSEmptyOnceWatering(csEmptyOnceWatering, ResponseSCSCOnceWateringCallBack, null);
        }
    }

}
