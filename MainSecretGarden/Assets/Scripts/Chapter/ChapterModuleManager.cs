using Company.Cfg;
using Cysharp.Threading.Tasks;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 章节模块管理器基础类
/// </summary>
public class ChapterModuleManager : MonoBehaviour
{
    #region 字段
    public static ChapterModuleManager _Instance;
    [SerializeField]
    int _chapterIndex;
    /// <summary>
    /// 章节所有模块集合
    /// </summary>
    [SerializeField]
    List<ChapterModule> _chapterModuleBasics = new List<ChapterModule>();
    /// <summary>
    /// 下一步的模块
    /// </summary>
    ChapterModule _nextModule;
    /// <summary>
    /// 当前步骤
    /// </summary>
    int _currStepIndex = 0;
    /// <summary>
    /// 章节返回按钮
    /// </summary>
    Button backBtn;
    #endregion
    #region 属性
    public int _ChapterIndex { get { return _chapterIndex; } }
    #endregion
    #region 函数
    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
    }
    public virtual async UniTask Initial()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
        await InitialChapterMoudles();
    }
    /// <summary>
    /// 初始化所有模块
    /// </summary>
    async UniTask InitialChapterMoudles()
    {
        for (int i = 0; i < _chapterModuleBasics.Count; i++)
        {
            _chapterModuleBasics[i].Initial(NextStep);
        }
        if (_chapterModuleBasics[0]._IsPrestrain)
        {
            await _chapterModuleBasics[0].Creation();
        }
        //返回按钮
        backBtn = transform.Find("BackBtn").GetComponent<Button>();
        backBtn.onClick.RemoveAllListeners();
        backBtn.onClick.AddListener(() =>
        {
            //OpenModule(false);
            ChapterModuleManager._Instance.Dispose();
            GameSoundPlayer.Instance.PlayBgMusic(MusicHelper.BgMusicLobby);
        });
        backBtn.gameObject.SetActive(StaticData.playerInfoData.userInfo.SectionId >= _chapterIndex ? true : false);
    }
    /// <summary>
    /// 展示对应模块
    /// </summary>
    public virtual async UniTask Show()
    {
        await _chapterModuleBasics[_currStepIndex].Show();
        if (_nextModule != null && _nextModule == _chapterModuleBasics[_currStepIndex])
        {
            _nextModule = null;
        }
        await PrestrainModule();
    }
    /// <summary>
    /// 下一步
    /// </summary>
    public virtual async void NextStep()
    {
        _currStepIndex = _chapterModuleBasics[_currStepIndex]._NextStepIndex;
        if (_currStepIndex == -1)
        {
            Dispose();
        }
        else
        {
            await Show();
        }


    }
    /// <summary>
    /// 预加载下一步
    /// </summary>
    public virtual async UniTask PrestrainModule()
    {
        if (_chapterModuleBasics[_currStepIndex]._IsPrestrain)
        {
            int nextStepIndex = _chapterModuleBasics[_currStepIndex]._NextStepIndex;
            if (nextStepIndex == -1)
            {
                return;
            }
            await _chapterModuleBasics[nextStepIndex].Creation();
            if (_nextModule != null && _nextModule != _chapterModuleBasics[nextStepIndex])
            {
                _nextModule.Dispose();
            }
            _nextModule = _chapterModuleBasics[nextStepIndex];
        }
        else
        {
            _nextModule.Dispose();
            _nextModule = null;
        }
    }

    public void DestroyCurrChapter()
    {
        for (int i = 0; i < _chapterModuleBasics.Count; i++)
        {
            _chapterModuleBasics[i].Dispose();
        }
    }
    public void Dispose()
    {
        DestroyCurrChapter();
        UIComponent.RemoveUI($"ChapterManager{_chapterIndex - StaticData.configExcel.Section[0].SectionId}");
    }

    /// <summary>
    /// 请求通关章节
    /// </summary>
    public void RequestPassChapter()
    {
        if (StaticData.playerInfoData.userInfo.SectionId >= this._ChapterIndex)
        {
            Debug.Log("之前已读过该章节");
            return;
        }
        //请求读完当前章节
        CSClearance cSClearance = new CSClearance() { SectionId = this._ChapterIndex };
        ProtocalManager.Instance().SendCSClearance(cSClearance, (SCEmptyClearance empty) =>
        {
            ChapterHelper.PassChapter(this._ChapterIndex);
            RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Chapter);
        }, (ErrorInfo e) => { Debug.Log("请求过关章节失败"); });
    }

    /// <summary>
    /// 点击进入下一章章节按钮事件
    /// </summary>
    /// NextAction 卸载当前章节
    /// callback 卸载章节中生成的某些对象的回调
    public void ClickEndChapterBtn(Button btn, Action NextAction, Action callback = null, int delayTime = 0)
    {//下一章是否解锁
        btn.onClick.RemoveAllListeners();
        if (ChapterHelper.NextChapterIsUnlock(this._ChapterIndex))
        {
            btn.onClick.AddListener(() =>
            {
                btn.enabled = false;
                callback?.Invoke();
                EnterIntoNextChapter(this._ChapterIndex + 1, delayTime);
            });
        }
        else
        {//打开解锁购买弹窗
            btn.onClick.AddListener(() =>
            {
                PopupBuyChapterView(this._ChapterIndex + 1, NextAction, callback, delayTime);
            });
        };
    }

    /// <summary>
    /// 进入下一章章节前等待一段时间
    /// </summary>
    /// <param name="delayTime"></param>
    private async void EnterIntoNextChapter(int chapterID, int delayTime)
    {
        await UniTask.Delay(delayTime);
        await ChapterHelper.EnterIntoChapter(chapterID);
    }

    /// <summary>
    /// 弹出购买章节界面
    /// </summary>
    public async void PopupBuyChapterView(int chapterID, Action nextAction, Action cancelCallBack = null, int dealyTime = 0)
    {//如果还没出下一章
        if (chapterID - StaticData.configExcel.Section[0].SectionId > StaticData.configExcel.GetVertical().CurVersionsChapterMaxCount - 1)
        {//显示未完待续页面，点击后淡出到大厅
            //var expect = await ABManager.GetAssetAsync<GameObject>("ExpectTipView");
            //GameObject go = GameObject.Instantiate(expect);
            //go.transform.SetRectTransformStretchAllWithParent(UIRoot.instance.GetUIRootCanvas().transform);
            //go.GetComponent<Button>().onClick.AddListener(() =>
            //{
            //    go.GetComponent<Button>().enabled = false;
            //    cancelCallBack?.Invoke();
            //    ChapterTool.FadeInFadeOut(go.GetComponent<CanvasGroup>(), 0, 0.01f, null, () =>
            //        {
            //            Destroy(go.gameObject);
            //            GameSoundPlayer.Instance.PlayBgMusic(MusicHelper.BgMusicLobby);
            //        });
            //});
            cancelCallBack?.Invoke();
            UIComponent.RemoveUI(UIType.UIChapter);

            await StaticData.ToManorSelf();
            GameSoundPlayer.Instance.PlayBgMusic(MusicHelper.BgMusicManor);
            return;
        }
        var chapterInfo = StaticData.configExcel.GetSectionBySectionId(chapterID);
        int id = chapterInfo.UnlockPrice[0].ID;//取到钻石图片的id
        int count = (int)chapterInfo.UnlockPrice[0].Count;//取到数量
        Sprite sprite = await ZillionaireToolManager.LoadItemSprite(id);
        string str = StaticData.GetMultilingual(120225); //$"你的等级不够解锁下一章了哦";
        StaticData.OpenCommonBuyTips(str, sprite, count, () =>
        {
            if (StaticData.GetWareHouseDiamond() >= count)
            {
                //扣除资源
                //刷新章节
                StaticData.UpdateWareHouseDiamond(-count);
                CSBuySection cSBuySection = new CSBuySection() { SectionId = chapterID };
                ProtocalManager.Instance().SendCSBuySection(cSBuySection, async (SCBuySection x) =>
                {
                    StaticData.CreateToastTips(StaticData.GetMultilingual(120234));//("章节购买成功");
                    ChapterHelper.UnlockChapter(chapterID);//存入前端缓存
                    foreach (var goodsInfo in x.CurrencyInfo)
                    {//更新前端的货币信息
                        StaticData.UpdateWareHouseItems(goodsInfo.GoodsId, (int)goodsInfo.Count);
                    }
                    //进入下一章
                    EnterIntoNextChapter(chapterID, dealyTime);

                    cancelCallBack?.Invoke();

                    //await ChapterHelper.EnterIntoChapter(chapterID);
                }, (ErrorInfo e) =>
                {
                    StaticData.CreateToastTips(StaticData.GetMultilingual(120235));//("章节购买失败");
                    Debug.LogError("章节购买失败" + e.webErrorCode);
                }, false);
            }
            else
            {//打开提示钻石不足界面
                UnlockView();
            }
        },
        async () => //取消按钮改为去庄园
        {
            nextAction?.Invoke();
            if (dealyTime != 0)
                await UniTask.Delay(dealyTime);
            await StaticData.ToManorSelf();
            Dispose();
            UIComponent.RemoveUI(UIType.UIChapter);
            GameSoundPlayer.Instance.PlayBgMusic(MusicHelper.BgMusicManor);
        }, 120212, true);
    }

    void UnlockView()
    {
        string str = StaticData.GetMultilingual(120243);//钻石不足，需要购买吗
        StaticData.OpenCommonTips(str, 120010, async () =>
        {
            await StaticData.OpenRechargeUI(1);
        }, null, 120075);
    }

    /// <summary>
    /// 返回按钮显示隐藏
    /// </summary>
    public void OpenBackButton(bool isShow)
    {
        backBtn.gameObject.SetActive(isShow);
    }

    #endregion
}
