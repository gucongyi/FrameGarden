using Cysharp.Threading.Tasks;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterBase : MonoBehaviour
{
    public int curChapterID;

    public int beginID;//句子起始ID
    public List<int> dialogIdList;//对话分支id

    //聊天界面头像资源名称
    public const string womanRole1HeadPortraitName = "liaotian_yuansu7";//苏薇薇头像名称
    public const string manRole1HeadPortraitName = "liaotian_yuansu6";//陆探微头像名称
    public const string womanRole2HeadPortraitName = "yuxiaocong_touxiang";//小葱头像名称
    //带头像资源的气泡（头像资源名称）
    public const string nvzhuHead = "nvzhu";
    public const string nanzhuHead = "nanzhu";
    //头像气泡parfab资源名称
    [HideInInspector] public string headBubble_left = "DialogueHeadBubble_Left";
    [HideInInspector] public string headBubble_right = "DialogueHeadBubble_Right";

    //相机参数
    [HideInInspector] public const float photoMask1InitY = 268f;//拍照遮罩1初始高度
    [HideInInspector] public const float photoMask2InitY = -471f;//拍照遮罩2初始高度
    [HideInInspector] public const float lessenScale = 0.5f;//照片缩小后的大小
    [HideInInspector] public const float lessenScaleDurationTime = 1.2f;//照片缩小的持续时间
    [HideInInspector] public const float photoY = 267f;//照片缩小后的高度

    #region 方框对话位置
    [HideInInspector] public Vector2 dialogueBox_Pos = new Vector2(0, -710f);
    #endregion

    //引导
    public GameObject slideGuidance;//滑动引导
    public GameObject clickGuidance;//点击引导

    public ChapterTitle title;//每章标题

    public float ScrenRatio;//屏幕比例
    /// <summary>
    /// 对话结束后显示指定按钮的回调(旁白对话完成后开始界面按钮)
    /// </summary>
    public Action ShowBtnCallBack;
    /// <summary>
    /// 观看完调用(外面传入)
    /// </summary>
    public Action WatchOver;
    /// <summary>
    /// 每个部分完成后调用
    /// </summary>
    public Action everyPartOverCallBack;


    //条漫引导位置
    [HideInInspector] public Vector2 VerticalCartoon_startPos = new Vector2(257f, -600f);
    [HideInInspector] public Vector2 VerticalCartoon_endPos = new Vector2(257f, 600f);

    /// <summary>
    /// 创建滑动引导
    /// </summary>
    public ChapterGuidance SlideGuidance(Transform parent)
    {
        GameObject go = GameObject.Instantiate(slideGuidance);
        ChapterHelper.SetParent(go, parent);
        var guidance = go.GetComponent<ChapterGuidance>();
        return guidance;
    }
    /// <summary>
    /// 创建点击引导
    /// </summary>
    public async System.Threading.Tasks.Task<GameObject> ClickGuidance(Transform parent, float DelayTime = 0)
    {
        int time = (int)DelayTime * 1000;
        await Cysharp.Threading.Tasks.UniTask.Delay(time);
        GameObject go = GameObject.Instantiate(clickGuidance);
        ChapterHelper.SetParent(go, parent);
        return go;
    }

    /// <summary>
    /// 获取文本
    /// </summary>
    /// <returns></returns>
    public string GetDialogue()
    {//自动赋值下一句
        var dialogue = StaticData.configExcel.GetChapterDialogueTextByID(beginID);
        if (dialogue.DialogIdList.Count > 0)//有分支的存在数组里  
        {
            dialogIdList.Clear();
            dialogIdList.AddRange(dialogue.DialogIdList);
        }
        else//没有的 也往数组里放一份
        {
            beginID = dialogue.NextDialogId;
            dialogIdList.Clear();
            dialogIdList.Add(dialogue.NextDialogId);//此时 List 应该只有1个元素
        }
        return ChapterHelper.GetTableDialogue(dialogue.ID);
    }

    /// <summary>
    /// 条漫下拉时设置位置
    /// </summary>
    public void VerticalCartoonSetLocalPosition(RectTransform contentRect, float endValue)
    {
        var loc = contentRect.localPosition;
        loc.y = endValue;
        contentRect.localPosition = loc;
    }

    /// <summary>
    /// 设置对话完成回调
    /// </summary>
    public void WordOverOpenBtn(Behaviour component, int DelayTime = 0)
    {
        ShowBtnCallBack = async () =>
        {//有对话时  对话显示完成后开启按钮
            await UniTask.Delay(DelayTime);
            ChapterHelper.SetEnable(component, true);
            ShowBtnCallBack = null;
        };
    }

    /// <summary>
    /// 设置每个部分完成后的回调
    /// </summary>
    public void SetEveryPartOverCallBack(Action callback, int DelayTime = 0)
    {
        everyPartOverCallBack = async () =>
        {//有对话时  对话显示完成后开启按钮
            await UniTask.Delay(DelayTime);
            if (callback != null)
            {
                everyPartOverCallBack = null;
                everyPartOverCallBack += callback;
            }
        };
    }
    //
    protected virtual void Start()
    {
        UIComponent.HideUI(UIType.UIManor);
    }

    /// <summary>
    /// 点击进入下一章章节按钮
    /// </summary>
    public void OnClickNextChapterBtn(Button btn)
    {
        if (ChapterHelper.NextChapterIsUnlock(curChapterID))
        {//进入下一章
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {//请求读完章节
                CSClearance cSClearance = new CSClearance() { SectionId = curChapterID };
                ProtocalManager.Instance().SendCSClearance(cSClearance, (x) =>
                {
                    ChapterHelper.PassChapter(curChapterID);
                    //打开下一个章节
                    ChapterHelper.EnterIntoChapter(curChapterID + 1);
                    Destroy(gameObject);//销毁自身
                }, (ErrorInfo e) => { Debug.Log("请求过关章节失败"); });
            });
        }
        else
        {//打开解锁页面
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(async () =>
            {
                var chapterInfo = StaticData.configExcel.GetSectionBySectionId(curChapterID + 1);
                int id = chapterInfo.UnlockPrice[0].ID;//取到钻石图片的id
                int count = (int)chapterInfo.UnlockPrice[0].Count;//取到数量
                Sprite sprite = await ZillionaireToolManager.LoadItemSprite(id);
                string str = $"你的等级不够解锁下一章了哦";
                StaticData.OpenCommonBuyTips(str, sprite, count, () =>
                {
                    ClickCallBACK((count), () =>
                    {//购买成功扣掉钻石后直接进入下一章节
                        ChapterHelper.EnterIntoChapter(curChapterID + 1);
                        Destroy(this.gameObject);
                    });
                });
            });
        };
    }

    void ClickCallBACK(int unlockPrice, Action ClickCallBack)
    {
        if (StaticData.GetWareHouseDiamond() >= unlockPrice)
        {
            //扣除资源
            //刷新章节
            StaticData.UpdateWareHouseDiamond(-unlockPrice);
            CSBuySection cSBuySection = new CSBuySection() { SectionId = curChapterID + 1 };
            ProtocalManager.Instance().SendCSBuySection(cSBuySection, (SCBuySection x) =>
            {
                StaticData.CreateToastTips("章节购买成功");
                ChapterHelper.UnlockChapter(curChapterID + 1);
                foreach (var goodsInfo in x.CurrencyInfo)
                {
                    StaticData.UpdateWareHouseItems(goodsInfo.GoodsId, (int)goodsInfo.Count);
                }
                ClickCallBack?.Invoke();
            }, (ErrorInfo e) =>
            {
                StaticData.CreateToastTips("章节购买失败");
                Debug.LogError("章节购买失败" + e.webErrorCode);
            }, false);
        }
        else
        {
            StaticData.OpenRechargeUI();
        }
    }

    /// <summary>
    /// 通关章节
    /// </summary>
    /// <param name="id">通关的章节id</param>
    //public void PassChpter(int id)
    //{
    //    ChapterHelper.EnterIntoChapter(id, UIRoot.instance.GetUIRootCanvas().transform, () =>
    //    {
    //        CSClearance cSClearance = new CSClearance() { SectionId = id };
    //        ProtocalManager.Instance().SendCSClearance(cSClearance, (x) =>
    //        {
    //            ChapterHelper.PassChapter(id);
    //        }, (ErrorInfo e) => { Debug.Log("请求过关章节失败"); });
    //    });
    //}
    //返回按钮
    public void BackBtn()
    {
        Destroy(gameObject);
        UIComponent.CreateUI(UIType.UIManor);
        UIComponent.CreateUI(UIType.UIChapter);
    }
}
