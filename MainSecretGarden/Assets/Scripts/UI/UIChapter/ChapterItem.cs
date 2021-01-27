using Company.Cfg;
using DG.Tweening;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 章节Item
/// </summary>
public class ChapterItem : MonoBehaviour
{
    int chapterID;
    GameObject AnimaNode;//动画结点
    Text text_chapterTitle;
    Text text_chapterName;
    Image ChapterBg;//item背景Icon
    Image bigLockImage;//大🔒图
    Button btn_StartWatch;//进入按钮
    Text text_StartWatch;
    Button btn_unlock;//解锁按钮
    Text text_unlock;
    public bool IsBeforeUnlock;//上一章是否解锁
    public bool IsUnlock;//是否解锁
    public bool IsBeforeWatchOver;//上一章是否观看完毕
    public bool IsWatchOver;//是否观看完毕（下一章解锁后可阅读）

    public int unlockLevel;

    public Image redDot;//红点


    private void FindComponent()
    {
        AnimaNode = transform.Find("AnimaNode").gameObject;
        text_chapterTitle = transform.Find("AnimaNode/RightItem/ChaputerTile_text").GetComponent<Text>();
        text_chapterName = transform.Find("AnimaNode/RightItem/ChapterName_text").GetComponent<Text>();
        ChapterBg = transform.Find("AnimaNode/LeftItem/chapterBG").GetComponent<Image>();
        bigLockImage = transform.Find("AnimaNode/LeftItem/Lock_image").GetComponent<Image>();
        btn_StartWatch = transform.Find("AnimaNode/RightItem/StartWatch_btn").GetComponent<Button>();
        text_StartWatch = transform.Find("AnimaNode/RightItem/StartWatch_btn/start_text").GetComponent<Text>();
        btn_unlock = transform.Find("AnimaNode/RightItem/UnLock_btn").GetComponent<Button>();
        text_unlock = transform.Find("AnimaNode/RightItem/UnLock_btn/unlock_text").GetComponent<Text>();

        redDot = btn_StartWatch.transform.Find("redDot").GetComponent<Image>();
    }

    void RegisteredEvents(Action ClickCallBack)
    {
        //注册事件
        btn_StartWatch.onClick.RemoveAllListeners();
        btn_StartWatch.onClick.AddListener(() => OnClickAsync());

        btn_unlock.onClick.RemoveAllListeners();
        btn_unlock.onClick.AddListener(() => ClickNotUnlockBtn(ClickCallBack));
    }

    //番外的观看记录默认为false
    public async void Set(int chapterID, string chapterTitle, string chapterName, string ChapterBgIconName, bool isUnlock, bool isWatchOver, bool isBeforeUnlock, bool isBeforeWatchOver, int unlockLevel, Action ClickCallBack)
    {
        FindComponent();
        this.chapterID = chapterID;

        this.text_chapterTitle.text = chapterTitle;
        this.text_chapterName.text = chapterName;
        if (ChapterBgIconName != null)
            ChapterBg.sprite = await ABManager.GetAssetAsync<Sprite>(ChapterBgIconName);
        this.IsUnlock = isUnlock;
        this.IsWatchOver = isWatchOver;
        this.IsBeforeUnlock = isBeforeUnlock;
        this.IsBeforeWatchOver = isBeforeWatchOver;
        this.unlockLevel = unlockLevel;


        RegisteredEvents(ClickCallBack);
        SetLock();//更新解锁状态
    }
    //章节解锁后点击
    void OnClickAsync()
    {
        ChapterHelper.ReduceRedDot(chapterID, true);
        if (this.chapterID == 998)
        {
            StaticData.CreateToastTips(StaticData.GetMultilingual(120242));
            return;
        }
        //已解锁 判断阅读到了哪章
        //如果是第一个可读章节则进入章节  如果不是 提示先阅读之前章节
        if (IsBeforeWatchOver)
        {//进入章节内部逻辑
            Debug.Log("进入已解锁章节");
            ChapterHelper.EnterIntoChapter(this.chapterID);
        }
        else
        {//提示
            StaticData.CreateToastTips(StaticData.GetMultilingual(120205));//请把上一章节看完
            SetLock();
        }
    }
    //章节未解锁点击
    void ClickNotUnlockBtn(Action ClickCallBack)
    {
        //ChapterHelper.ReduceRedDot(chapterID, true);
        if (this.chapterID == 998)
        {
            StaticData.CreateToastTips(StaticData.GetMultilingual(120242));
            return;
        }
        //未解锁 判断解锁到了哪里  
        //如果是第一个可解锁的id则弹出购买界面  如果不是 提示先解锁之前章节
        if (IsBeforeUnlock)//上一张已解锁
        {
            var chapterInfo = StaticData.configExcel.GetSectionBySectionId(this.chapterID);
            UnlockView(chapterInfo, ClickCallBack);
        }
        else
        {
            StaticData.CreateToastTips(StaticData.GetMultilingual(120206));//之前的章节还没有解锁
        }
    }


    async void UnlockView(SectionDefine chapterInfo, Action ClickCallBack)
    {
        int id = chapterInfo.UnlockPrice[0].ID;//取到钻石图片的id
        int count = (int)chapterInfo.UnlockPrice[0].Count;//取到数量
        Sprite sprite = await ZillionaireToolManager.LoadItemSprite(id);
        string str = string.Format(StaticData.GetMultilingual(120241), unlockLevel);//该章节需要解锁，是否提前解锁//钻石不足，需要购买吗
        StaticData.OpenCommonBuyTips(str, sprite, count, () => ClickCallBACK(count, ClickCallBack));
    }

    //解锁信息更新
    void SetLock()
    {
        if (IsUnlock)
        {//已解锁的界面设置
            bigLockImage.gameObject.SetActive(false);
            btn_StartWatch.gameObject.SetActive(true);
            btn_unlock.gameObject.SetActive(false);

            text_StartWatch.text = StaticData.GetMultilingual(120203);//开始观看

        }
        else
        {
            bigLockImage.gameObject.SetActive(true);
            btn_StartWatch.gameObject.SetActive(false);
            btn_unlock.gameObject.SetActive(true);
            text_unlock.text = StaticData.GetMultilingual(120096);//解锁

        }
        if (chapterID == 998) redDot.gameObject.SetActive(false);
        if (ChapterHelper.localChapterIDList.Contains(chapterID))
        {
            redDot.gameObject.SetActive(false);
        }
        else
        {
            redDot.gameObject.SetActive(true);
        }

        //ShowRedDotAction(IsUnlock);//有未解锁的就显示红点
    }

    void ClickCallBACK(int unlockPrice, Action ClickCallBack)
    {
        if (StaticData.GetWareHouseDiamond() >= unlockPrice)
        {
            //扣除资源
            //刷新章节
            StaticData.UpdateWareHouseDiamond(-unlockPrice);
            CSBuySection cSBuySection = new CSBuySection() { SectionId = chapterID };
            ProtocalManager.Instance().SendCSBuySection(cSBuySection, (SCBuySection x) =>
                {
                    StaticData.CreateToastTips(StaticData.GetMultilingual(120234));//章节购买成功
                    ChapterHelper.UnlockChapter(this.chapterID);
                    foreach (var goodsInfo in x.CurrencyInfo)
                    {
                        StaticData.UpdateWareHouseItems(goodsInfo.GoodsId, (int)goodsInfo.Count);
                    }
                    //ChapterHelper.ChpaterUpdateRedDot();//刷新红点
                    RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Chapter);
                    UIChapterComponent.Instance.ShowChapterBtnRedDot();
                    ClickCallBack?.Invoke();
                }, (ErrorInfo e) =>
                {
                    StaticData.CreateToastTips(StaticData.GetMultilingual(120235));//("章节购买失败");
                    Debug.LogError("章节购买失败" + e.webErrorCode);
                }, false);
        }
        else
        {
            //打开钻石不足提示弹窗
            string str = StaticData.GetMultilingual(120243);//钻石不足，需要购买吗
            StaticData.OpenCommonTips(str, 120010, async () =>
            {
                await StaticData.OpenRechargeUI(1);
            }, null, 120075);
        }
    }
    //进入动画
    public void PlayCutInAnima(int hallOrOAD)
    {
        switch (hallOrOAD)
        {
            case 1:
                AnimaNode.transform.localPosition = new Vector2(1227, 0);
                AnimaNode.transform.DOLocalMoveX(0, 0.3f);
                break;
            case 2:
                AnimaNode.transform.localPosition = new Vector2(-1227, 0);
                AnimaNode.transform.DOLocalMoveX(0, 0.3f);
                break;
            case 3:
                AnimaNode.transform.localPosition = new Vector2(0, 0);
                AnimaNode.transform.DOLocalMoveX(-1227, 0.3f);
                break;
        }
    }

    //进入动画准备
    public void PlayCutInReady(int hallOrOAD)
    {
        switch (hallOrOAD)
        {
            case 1:
                AnimaNode.transform.localPosition = new Vector2(1227, 0);
                break;
            case 2:
                AnimaNode.transform.localPosition = new Vector2(-1227, 0);
                break;
            case 3:
                AnimaNode.transform.localPosition = new Vector2(0, 0);
                break;
        }

    }

    public int GetItemID()
    {
        return this.chapterID;
    }

    public void SetBeforeUnlock(bool isBeforeUnclock)
    {
        this.IsBeforeUnlock = isBeforeUnclock;
    }
    public void SetUnlock(bool isUnclock)
    {
        this.IsUnlock = isUnclock;
        SetLock();
    }
    public void SetBeforeWatchOver(bool isBeforeOver)
    {
        this.IsBeforeWatchOver = isBeforeOver;
    }
    public void SetWatchOver(bool isOver)
    {
        this.IsWatchOver = isOver;
    }


}

