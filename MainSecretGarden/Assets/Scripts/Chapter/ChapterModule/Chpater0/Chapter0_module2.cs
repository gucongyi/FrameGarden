using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chapter0_module2 : ChapterControllerBasics
{
    #region 字段
    /// <summary>
    /// 下一步按钮
    /// </summary>
    Button _nextStepBtn;
    //场景
    RectTransform _oneScene;
    RectTransform _twoScene;
    string walk = "walk";
    string clickGuidanceName = "ChapterClickGuidance";//点击引导
    string GuidanceName = "ChapterGuidance";
    GameObject walk_RewImage;
    public Image walk_image;//女主步行
    public Image bubble_image;//气泡手机
    public Button phone;
    public Button departBtn;//出发按钮

    //条漫引导位置
    [HideInInspector] public Vector2 VerticalCartoon_startPos = new Vector2(257f, -600f);
    [HideInInspector] public Vector2 VerticalCartoon_endPos = new Vector2(257f, 600f);
    public ScrollRect VerticalCartoon;//条漫1
    public Image tiaoman2;

    CaricaturePlayerController caricaturePlayerController;

    float moveXspeed = 0.1f;
    /// <summary>
    /// 点击计数
    /// </summary>
    int _clickIndex = 1;
    #endregion
    #region 函数
    private void Start()
    {
        //Initial();
    }
    public override void Initial()
    {
        _nextStepBtn = transform.GetComponent<Button>();
        _nextStepBtn.onClick.RemoveAllListeners();
        _nextStepBtn.onClick.AddListener(ClickBtn);
        _oneScene = transform.Find("One") as RectTransform;
        _twoScene = transform.Find("Two") as RectTransform;

        walk_image = _oneScene.Find("walk_image").GetComponent<Image>();//女主步行
        bubble_image = walk_image.transform.Find("bubble_image").GetComponent<Image>();//气泡手机
        phone = bubble_image.transform.Find("phone").GetComponent<Button>();

        //VerticalCartoon = _oneScene.Find("VerticalCartoon").GetComponent<ScrollRect>();//条漫1
        caricaturePlayerController = _twoScene.Find("CaricaturePlayer").GetComponent<CaricaturePlayerController>();


        //tiaoman2 = VerticalCartoon.content.Find("manhua2").GetComponent<Image>();
        departBtn = _twoScene.Find("departBtn").GetComponent<Button>();

        ShowWalk_Image();
        OpenClickBtn(false);
        base.Initial();
    }

    /// <summary>
    /// 点击
    /// </summary>
    public void ClickBtn()
    {
        OpenClickBtn(false);
        StepCut(_clickIndex);
        _clickIndex++;
    }

    public void StepCut(int index)
    {
        if (index > 19) return;
        switch (index)
        {
            case 1:
                //条漫初始化
                _oneScene.gameObject.SetActive(false);
                caricaturePlayerController.gameObject.SetActive(true);
                caricaturePlayerController.Initial(() =>
                {
                    VerticalCartoonClickGuidance(Vector2.zero, departBtn.transform);//生成按钮点击
                    ChapterHelper.DelaySetActice(departBtn.gameObject, 500);
                    VerticalCartoonNextBtn();
                }, CaricatureChanged);
                //VerticalCartoon.transform.DOLocalMoveY(2688f, 1f);
                //caricaturePlayerController.transform.DOLocalMoveY(0, 1f);
                break;
        }
    }
    //对步骤特殊处理
    void CaricatureChanged(int index)
    {
    }

    /// <summary>
    /// 创建滑动引导
    /// </summary>
    public async System.Threading.Tasks.Task<ChapterGuidance> SlideGuidance(Transform parent)
    {
        var parfab = await ABManager.GetAssetAsync<GameObject>(GuidanceName);
        GameObject go = GameObject.Instantiate(parfab);
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
        var parfab = await ABManager.GetAssetAsync<GameObject>(clickGuidanceName);
        GameObject go = GameObject.Instantiate(parfab);
        ChapterHelper.SetParent(go, parent);
        return go;
    }

    //女主走路
    async void ShowWalk_Image()
    {
        ChapterHelper.Fade(walk_image.gameObject, 1, 0.8f, 0);
        var parfab = await ABManager.GetAssetAsync<GameObject>(walk);
        walk_RewImage = GameObject.Instantiate(parfab);
        walk_RewImage.transform.parent = _oneScene;
        await UniTask.Delay(1000);
        GetPhoneCall();
    }
    //手机震动
    async void GetPhoneCall()
    {
        bubble_image.transform.localScale = Vector3.zero;
        bubble_image.transform.DOScale(1f, 0.3f);
        //生成点击引导
        var clickGuidance = await ClickGuidance(bubble_image.transform, 0.5f);
        (clickGuidance.transform as RectTransform).anchoredPosition = (phone.transform as RectTransform).anchoredPosition;
        ChapterHelper.SetActive(bubble_image.gameObject, true);
        phone.onClick.RemoveAllListeners();
        phone.onClick.AddListener(async () =>
        {
            if (walk_RewImage != null)
                GameObject.Destroy(walk_RewImage.gameObject);
            GameObject.Destroy(clickGuidance.gameObject);
            await OpenChatView();

            ChapterHelper.Fade(walk_RewImage, 0, 0.8f, 1);
        });
    }

    #region 聊天

    //打开聊天页面
    async System.Threading.Tasks.Task OpenChatView()
    {//注册按钮事件
        //打开聊天
        await ChapterTool.OpenDialogueBoxWeChat(10000008, 10000001, 10000002, () =>
        {
            Debug.Log("微信聊天完毕！");
            walk_image.gameObject.SetActive(false);
            //ShowVerticalCartoon();

            ClickBtn();
        });
    }

    #endregion

    #region 旧条漫
    ChapterGuidance guidance;//条漫滑动引导
    Vector2 clickGuidancePos = new Vector2(0, -247f);
    GameObject verticalCartoonClickGuidance;//条漫的点击引导
    //条漫1
    async void ShowVerticalCartoon()
    {
        ChapterHelper.Fade(VerticalCartoon.gameObject, 1, 0.8f, 0);
        VerticalCartoon.onValueChanged.AddListener(VerticalCartoonOver);
        //引导
        guidance = await SlideGuidance(VerticalCartoon.transform);
        guidance.PlayGuidanceAnima(VerticalCartoon_startPos, VerticalCartoon_endPos, () =>
        {
            VerticalCartoon.content.DOAnchorPosY(VerticalCartoon.content.anchoredPosition.y + 250f, 1).OnComplete(() =>
            {
                //引导移动完成后记录位置
                playerIsControll = false;
            });
        }, -1, 1);

    }
    bool playerIsControll;//玩家是否操作
    bool isStop = false;
    void VerticalCartoonOver(Vector2 vector2)
    {
        if (vector2.y < 0.988f) playerIsControll = true;
        if (playerIsControll && guidance != null)
        {
            GameObject.Destroy(guidance.gameObject);
            guidance = null;
        }
        if (VerticalCartoon.content.anchoredPosition.y > 2688f && !isStop)
        {
            isStop = true;
            VerticalCartoonSetLocalPosition(VerticalCartoon.content, 2688f);
            VerticalCartoon.vertical = false;
            VerticalCartoonClickGuidance(clickGuidancePos, tiaoman2.transform);//生成点击
            OpenClickBtn(true);
        }
    }

    // 条漫下拉时设置位置
    public void VerticalCartoonSetLocalPosition(RectTransform contentRect, float endValue)
    {
        var loc = contentRect.localPosition;
        loc.y = endValue;
        contentRect.localPosition = loc;
    }
    //创建引导
    async void VerticalCartoonClickGuidance(Vector2 pos, Transform parent)
    {
        //点击引导
        verticalCartoonClickGuidance = await ClickGuidance(parent);
        (verticalCartoonClickGuidance.transform as RectTransform).anchoredPosition = pos;
    }


    //条漫对话文本赋值
    void VerticalCartoonNextBtn()
    {
        departBtn.onClick.RemoveAllListeners();
        departBtn.onClick.AddListener(() =>
        {
            if (walk_RewImage != null)
                GameObject.Destroy(walk_RewImage.gameObject);
            NextStep();
        });
        departBtn.transform.Find("Text").GetComponent<Text>().text = "出发";
    }
    #endregion


    void OpenClickBtn(bool isOpen)
    {
        _nextStepBtn.enabled = isOpen;
    }


    public override void NextStep()
    {
        base.NextStep();
    }

    public override void Dispose()
    {

        base.Dispose();

    }
    #endregion


}
