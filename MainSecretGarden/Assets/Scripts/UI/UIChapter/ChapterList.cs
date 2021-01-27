using Game.Protocal;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterList : MonoBehaviour
{
    public GameObject item;
    ScrollRect scrollRect;
    //LoopVerticalScrollRect loop;
    public List<ChapterItem> chapterItems = new List<ChapterItem>();
    //章节缓存
    int _mainCount;//章节数量
    int unlockMaxChapterID;//解锁的最大章节ID
    int passChapterID;//阅读进度

    UIChapterComponent _uiChapter;

    void FindCompoment()
    {
        scrollRect = GetComponent<ScrollRect>();
        //loop = GetComponent<LoopVerticalScrollRect>();
        _uiChapter = GetComponentInParent<UIChapterComponent>();
    }
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="mainCount">此版本有多少章章节</param>
    /// <param name="passMaxChapterID">已过关的最大章节ID</param>
    /// <param name="unlockMaxChapterID">已解锁的最大章节ID</param>
    public void Init(int mainCount, int passMaxChapterID, int unlockMaxChapterID)
    {
        FindCompoment();
        this._mainCount = mainCount;
        this.passChapterID = passMaxChapterID;
        this.unlockMaxChapterID = unlockMaxChapterID;

        //loop.ClearCells();
        //loop.totalCount = mainCount;
        //loop.RefillCells();
        UpdateList(unlockMaxChapterID, passMaxChapterID);
        //章节列表埋点
        StaticData.DataDot(Company.Cfg.DotEventId.ChapterList);
    }

    public void UpdateList(int unlockMaxChapterID, int passMaxChapterID)
    {//i[0]为默认章节
        Clear();
        for (int i = 0; i < _mainCount; i++)
        {//判断每个章节是否已经解锁
            ChapterItem ci = Instantiate(item).GetComponent<ChapterItem>();
            chapterItems.Add(ci);
            //敬请期待
            if (i == _mainCount - 1)
            {
                SetParent(ci.gameObject.transform, scrollRect.content);
                //SetParent(ci.gameObject.transform, loop.content);
                ci.Set(998, $"敬请期待", $"敬请期待", null, false, false, false, false, 9999, UnlockChapter);
                return;
            }
            var section = StaticData.configExcel.GetSectionBySectionId(StaticData.configExcel.Section[0].SectionId + i);
            //是否显示为解锁状态      配置表里的章节id与服务器记录的最大解锁章节id 比较
            bool isUnlock = section.SectionId <= unlockMaxChapterID || section.SectionGrade <= StaticData.GetPlayerLevelByExp() ? true : false;
            //已解锁章节ID必定 >= 已通过章节ID
            //是否观看完毕           配置表里的章节id与服务器记录的最大已观看章节id 比较
            bool isWatchOver = section.SectionId <= passMaxChapterID ? true : false;
            bool isBeforeUnlock = section.SectionId - 1 <= unlockMaxChapterID ? true : false;
            bool isBeforeWatchOver = section.SectionId <= passMaxChapterID + 1 || section.SectionId == StaticData.configExcel.Section[0].SectionId ? true : false;
            //设置Item基础属性
            ci.Set(section.SectionId, ChapterTool.GetChapterFunctionString(section.SectionNumber), ChapterTool.GetChapterFunctionString(section.SectionNameId), section.Icon, isUnlock, isWatchOver, isBeforeUnlock, isBeforeWatchOver, section.SectionGrade, UnlockChapter);
            SetParent(ci.gameObject.transform, scrollRect.content);
            ci.name = $"Chapter{i}";
        }
    }
    //清除item
    public void Clear()
    {
        foreach (var item in chapterItems)
        {
            Destroy(item.gameObject);
        }
        this.chapterItems.Clear();
    }

    void SetParent(Transform thisTrans, Transform parentTrans)
    {
        thisTrans.SetParent(parentTrans);
        thisTrans.localPosition = Vector3.zero;
        thisTrans.localRotation = Quaternion.identity;
        thisTrans.localScale = Vector3.one;
        ChapterHelper.SetActive(thisTrans.gameObject, true);
    }

    /// <summary>
    /// 解锁章节
    /// </summary>
    void UnlockChapter()
    {
        this.unlockMaxChapterID += 1;
        ChapterHelper.UnlockChapter(this.unlockMaxChapterID);
        foreach (var item in chapterItems)
        {
            if (item.GetItemID() == unlockMaxChapterID)
            {
                item.SetUnlock(true);
            }
            if (item.GetItemID() == unlockMaxChapterID + 1)
            {
                item.SetBeforeUnlock(true);
            }
        }
    }
    /// <summary>
    /// 通关章节
    /// </summary>
    /// <param name="id">通关的章节id</param>
    public void PassChpter(int id)
    {
        if (id <= this.passChapterID) return;

        CSClearance cSClearance = new CSClearance() { SectionId = id };
        //bool receive = false;等待返回
        ProtocalManager.Instance().SendCSClearance(cSClearance, (x) =>
        {
            this.passChapterID = id;
            ChapterHelper.PassChapter(this.passChapterID);
            foreach (var item in chapterItems)
            {
                if (item.GetItemID() == this.passChapterID)
                {
                    item.SetWatchOver(true);//当前章节看完
                }
                if (item.GetItemID() == this.passChapterID + 1)
                {
                    item.SetBeforeWatchOver(true);//下一章的前一章标记看完
                }
            }
        }, (ErrorInfo e) => { Debug.Log("请求过关章节失败"); });
    }

}

