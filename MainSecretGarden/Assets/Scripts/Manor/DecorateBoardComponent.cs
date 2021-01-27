using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecorateBoardComponent : MonoBehaviour
{
    public GameObject goLock;
    public GameObject goUnLocking;
    public Text TextH;
    public Text TextM;
    public Text TextS;
    TimeCountDownComponent timeBoxTimerComponent;
    float RemainTime;
    int willUnlockGrade = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public float GetRemianTime()
    {
        return RemainTime;
    }
    // Update is called once per frame
    void Update()
    {
        int level=StaticData.GetPlayerLevelAndCurrExp().level;
        if (level>= willUnlockGrade)
        {
            goLock.SetActive(false);
            goUnLocking.SetActive(true);
        }
    }

    internal void SetTips(int grade)
    {
        willUnlockGrade = grade;
        goLock.SetActive(true);
        goLock.GetComponent<Text>().text = $"{grade}级解锁";
        goUnLocking.SetActive(false);
    }
    public void GenerateRegionTimer()
    {
        //创建空的计时器
        timeBoxTimerComponent = StaticData.CreateTimer(100000 / 1000f, false, (go) =>
        {
        },
        (remainTime) =>
        {
        },"DecorateBoard");
    }
    public void SetRegionTimeChange(long timeStamp,ManorRegionComponent manorRegionComponent)
    {
        //计算当前服务器时间
        long CurrTimeStampServer = TimeHelper.ServerTimeStampNow;
        long CurrRemainTime = timeStamp - CurrTimeStampServer;
        if (CurrRemainTime <= 0f)
        {
            manorRegionComponent.EndRoadWork();
            UIComponent.HideUI(UIType.UIManorRegionAdIncrease);
            Destroy(timeBoxTimerComponent.gameObject);
        }
        else
        {
            timeBoxTimerComponent.Init(CurrRemainTime / 1000f, false, (go) =>
            {
                manorRegionComponent.EndRoadWork();
                Destroy(timeBoxTimerComponent.gameObject);
            },
            (remainTime) =>
            {
                this.RemainTime = remainTime;
                if (goLock == null)
                {
                    return;
                }
                //设置倒计时时间
                goLock.SetActive(false);
                goUnLocking.SetActive(true);
                int h = (int)(remainTime / 3600);
                remainTime = remainTime % 3600;
                int m = (int)(remainTime / 60);
                remainTime = remainTime % 60;
                int s = (int)remainTime;
                TextH.text = String.Format("{0:00}", h);
                TextM.text = String.Format("{0:00}", m);
                TextS.text = String.Format("{0:00}", s);
            });
        }
    }
}
