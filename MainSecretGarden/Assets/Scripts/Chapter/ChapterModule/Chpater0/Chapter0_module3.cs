using Cysharp.Threading.Tasks;
using Game.Protocal;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class Chapter0_module3 : ChapterControllerBasics
{
    #region 字段
    /// <summary>
    /// 下一步按钮
    /// </summary>
    Button _nextStepBtn;
    bool isFirst = false;

    //拥抱场景对话气泡的位置
    Vector2 hugGrandpaDialoguePos = new Vector2(-68, 734);
    Vector2 hugNvZhuDialoguePos = new Vector2(105, -222);
    //场景
    RectTransform _oneScene;

    public RectTransform carMoveBG;//车外移动背景
    public RectTransform nvDialogue;//女主头像
    public Text name_nv;//女主名字
    public TextAnimationManager dialogueText_nv;//文本
    public Text name_nan;//男主名字
    public RectTransform nanDialogue;//男主头像
    public TextAnimationManager dialogueText_nan;

    public Image hugGrandpa_image;//拥抱爷爷
    DialogueBoxBubbleComponent dialogueBoxBubble;


    public RectTransform endScene;//最后一幕
    Button end_btn;//结束按钮

    CancellationTokenSource cancellationToken = new CancellationTokenSource();

    float moveXspeed = 0.1f;
    /// <summary>
    /// 点击计数
    /// </summary>
    int _clickIndex = 0;
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

        if (StaticData.playerInfoData.userInfo.SectionId < StaticData.configExcel.Section[0].SectionId)
            isFirst = true;//初次阅读

        #region data
        carMoveBG = _oneScene.Find("CarMoveBG") as RectTransform;
        //带头像框
        nvDialogue = carMoveBG.Find("nvDialogueBox") as RectTransform;
        name_nv = nvDialogue.transform.Find("box_image/name_left/name_text").GetComponent<Text>();
        dialogueText_nv = nvDialogue.transform.Find("box_image/dialogue_text").GetComponent<TextAnimationManager>();
        nanDialogue = carMoveBG.Find("nanDialogueBox") as RectTransform;
        name_nan = nanDialogue.transform.Find("box_image/name_left/name_text").GetComponent<Text>();
        dialogueText_nan = nanDialogue.transform.Find("box_image/dialogue_text").GetComponent<TextAnimationManager>();


        hugGrandpa_image = _oneScene.Find("HugGrandpa_image").GetComponent<Image>();
        dialogueBoxBubble = _oneScene.Find("DialogueBox_Bubble").GetComponent<DialogueBoxBubbleComponent>();

        endScene = _oneScene.Find("EndScene") as RectTransform;
        end_btn = endScene.transform.Find("JoinGarden_btn").GetComponent<Button>();
        #endregion


        ShowCarMoveBG(cancellationToken);
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
        if (index > 6) return;
        switch (index)
        {
            case 0://取消等待男主说话
                cancellationToken.Cancel();
                ChapterHelper.SetActive(nvDialogue.gameObject, false);
                ShowCarMoveBGAnswer();
                break;
            case 1:
                ShowHugGrandpa_image();
                break;
        }
    }

    //显示车外live2D动画
    async UniTask ShowCarMoveBG(CancellationTokenSource cancellationTokenSource)
    {
        ChapterHelper.SetActive(carMoveBG.gameObject, true);

        await UniTask.Delay(500);
        //带头像女主说话
        var data = ChapterTool.GetChapterData(10000028);
        ChapterHelper.SetActive(nvDialogue.gameObject, true);
        name_nv.text = StaticData.playerInfoData.userInfo.Name;
        dialogueText_nv.word = ChapterTool.GetDialogueString(data);
        dialogueText_nv.Play();
        dialogueText_nv.Speed6Play();
        dialogueText_nv.onAnimationEnd.AddListener(() => { OpenClickBtn(true); });
        await UniTask.Delay(TimeSpan.FromMilliseconds(2700), cancellationToken: cancellationTokenSource.Token);
        OpenClickBtn(false);//玩家如果没点击先关闭按钮
        ChapterHelper.SetActive(nvDialogue.gameObject, false);
        _clickIndex++;
        ShowCarMoveBGAnswer();
    }

    //车内回答
    async void ShowCarMoveBGAnswer()
    {//男主回答
        ChapterHelper.SetActive(nanDialogue.gameObject, true);
        //带头像男主说话
        name_nan.text = ChapterTool.GetChapterFunctionString(10000002);
        var data = ChapterTool.GetChapterData(10000029);
        dialogueText_nan.word = ChapterTool.GetDialogueString(data);
        dialogueText_nan.Play();
        dialogueText_nan.Speed6Play();

        await UniTask.Delay(2700);
        ChapterHelper.SetActive(nanDialogue.gameObject, false);

        WallLoop wallLoop = carMoveBG.transform.Find("MoveBg").GetComponent<WallLoop>();

        wallLoop.callback = () =>
        {
            ClickBtn();
        };
    }

    //显示拥抱
    void ShowHugGrandpa_image()
    {
        ChapterHelper.SetActive(hugGrandpa_image.gameObject, true);
        dialogueBoxBubble.Initial(() =>
        {
            ShowEndScene();
            dialogueBoxBubble.Close();
        });
        dialogueBoxBubble.Show();
    }

    Animator anim;

    //显示最后一幕
    async void ShowEndScene()
    {
        //隐藏之前的所有图
        ChapterHelper.SetActive(carMoveBG.gameObject, false);
        ChapterHelper.SetActive(hugGrandpa_image.gameObject, false);
        ChapterHelper.SetActive(endScene.gameObject, true);
        var parfab = await ABManager.GetAssetAsync<GameObject>("openDoor");
        var go = GameObject.Instantiate(parfab);
        go.transform.parent = _oneScene;
        anim = go.transform.Find("Camera/kaimen").GetComponent<Animator>();

        //显示最后一幕时  请求保存

        if (isFirst)
        {//新手引导初次阅读
            ChapterModuleManager._Instance.RequestPassChapter();
            end_btn.onClick.RemoveAllListeners();
            end_btn.onClick.AddListener(() =>
            {
                end_btn.enabled = false;
                GoToManor();
            });
        }
        else
        {
            ChapterModuleManager._Instance.ClickEndChapterBtn(end_btn, null, async () =>
            {
                anim.SetBool("openDoor", true);
                await UniTask.Delay(1500);
                endScene.GetComponent<CanvasGroup>().DOFade(0, 3f);
                await UniTask.Delay(3500);
            }, 5500);
        }
        //首次通关序章
        if (isFirst)
            end_btn.transform.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120224);//去庄园
        else
            end_btn.transform.Find("Text").GetComponent<Text>().text = ChapterTool.GetChapterFunctionString(10000000);//下一章
    }

    async void GoToManor()
    {
        anim.SetBool("openDoor", true);
        await UniTask.Delay(2000);
        endScene.GetComponent<CanvasGroup>().DOFade(0, 0.7f);
        await UniTask.Delay(700);
        NextStep();
        //请求订单
        StaticData.RequestDeals();
        await StaticData.ToManorSelf();
        GameSoundPlayer.Instance.PlayBgMusic(MusicHelper.BgMusicManor);
    }

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
