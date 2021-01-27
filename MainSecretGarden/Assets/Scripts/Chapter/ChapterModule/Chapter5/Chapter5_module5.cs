using Company.Cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Game.Protocal;

public class Chapter5_module5 : ChapterControllerBasics
{
    #region 字段
    /// <summary>
    /// 下一步按钮
    /// </summary>
    Button _nextStepBtn;

    RectTransform _oneScene;

    RectTransform _twoScene;
    Image _twoBg;
    Button _twoScene_backBtn;
    DialogueBoxTetragonumComponent dialogueBox;


    Button nextBtn;
    Text nextText;
    /// <summary>
    /// 点击计数
    /// </summary>
    int _clickIndex = 1;
    #endregion
    #region 函数
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Initial()
    {
        _nextStepBtn = transform.GetComponent<Button>();
        _nextStepBtn.onClick.RemoveAllListeners();
        _nextStepBtn.onClick.AddListener(ClickBtn);

        _oneScene = transform.Find("One") as RectTransform;

        _twoScene = transform.Find("Two") as RectTransform;
        _twoBg = _twoScene.Find("NewInfo").GetComponent<Image>();
        _twoScene_backBtn = _twoBg.transform.Find("NewInfo2/backbtn").GetComponent<Button>();
        _twoScene_backBtn.onClick.RemoveAllListeners();
        _twoScene_backBtn.onClick.AddListener(() =>
        {
            isFirst = true;
            transform.GetComponent<Image>().raycastTarget = false;
            transform.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            dialogueBoxWeChat.PauseIn(false);
            _twoBg.rectTransform.DOAnchorPosX(1242, 0.2f);
        });
        dialogueBox = transform.Find("DialogueBox_Tetragonum_Two").GetComponent<DialogueBoxTetragonumComponent>();


        nextBtn = transform.Find("NextChapterBtn").GetComponent<Button>();
        nextText = nextBtn.transform.Find("NextChapterText").GetComponent<Text>();

        ClickBtn();
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
        switch (index)
        {
            case 1:
                ShowOneScene();
                break;
            case 2:
                ChatNv2();
                break;
        }
    }
    async void ShowOneScene()
    {
        await ChapterTool.MoveUi(_oneScene.Find("bg") as RectTransform, new Vector2(-843f, 0f), 0.1f, 0.1f, null, () =>
        {
            ClickBtn();
        });
    }

    DialogueBoxWeChatComponent dialogueBoxWeChat;
    //和女2聊天
    async void ChatNv2()
    {
        ChapterModuleManager._Instance.OpenBackButton(false);
        GameObject obj = await UIComponent.CreateUIAsync(UIType.DialogueBox_WeChat);
        dialogueBoxWeChat = obj.GetComponent<DialogueBoxWeChatComponent>();
        dialogueBoxWeChat._isOut = false;
        dialogueBoxWeChat.Initial(() =>
        {
            Debug.Log("微信聊天完毕！");
            transform.GetComponent<Image>().raycastTarget = true;
            ReadOverThisChapter();
            //nextBtn.onClick.AddListener(() =>
            //{//结束整章
            //    ReadOverThisChapter();
            //});

        }, BeforeWeChatDialogue, AfterWeChatDialogue, WeChatDialogueClick);
        dialogueBoxWeChat.SetInitialData(15000107, 10000001, 10000003);
        _oneScene.GetComponent<CanvasGroup>().alpha = 0;
        _oneScene.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    void BeforeWeChatDialogue(ChapterDialogueTextDefine data)
    {

    }
    bool isFirst = false;
    bool createGuidance = false;//引导点击
    void AfterWeChatDialogue(ChapterDialogueTextDefine data)
    {
        if (data.ID == 15000136)
        {//TODO
            CreateGuidance();
        }
    }
    void WeChatDialogueClick(ChapterDialogueTextDefine data)
    {
        if (data.ID == 15000136)
        {//链接

            dialogueBoxWeChat.PauseOut(false, 1);
            ChapterModuleManager._Instance.transform.SetAsLastSibling();
            createGuidance = true;
            if (guidance != null)
            {
                Destroy(guidance);
            }
            _twoBg.rectTransform.DOAnchorPosX(0, 0.3f).OnComplete(async () =>
            {
                if (isFirst) return;
                await UniTask.Delay(1000);
                if (isFirst) return;
                dialogueBox.Initial(() =>
                {
                    dialogueBox.Close();
                });
                dialogueBox.Show();
            });
        }
    }
    GameObject guidance;
    async void CreateGuidance()
    {
        await UniTask.Delay(1200);
        if (!createGuidance)
        {//生成引导
            var parfab = await ABManager.GetAssetAsync<GameObject>("FullScreenClickGuidance");
            guidance = GameObject.Instantiate(parfab);

            guidance.transform.SetRectTransformStretchAllWithParent(UIRoot.instance.GetUIRootCanvas().transform);
            //ChapterHelper.SetParent(guidance, UIRoot.instance.GetUIRootCanvas().transform);
            //ChapterHelper.AudoSelf(guidance.transform as RectTransform);
            (guidance.transform.Find("ClickGuidance") as RectTransform).anchoredPosition = new Vector2(0, 442);
        }
    }

    void ReadOverThisChapter()
    {
        nextText.text = ChapterTool.GetChapterFunctionString(10000000);//下一章
        nextBtn.gameObject.SetActive(true);
        ChapterModuleManager._Instance.ClickEndChapterBtn(nextBtn, NextStep, () =>
         {
             GameObject.Destroy(dialogueBoxWeChat.gameObject);
         });
        //条漫阅读结束后即刻请求保存读完
        ChapterModuleManager._Instance.RequestPassChapter();
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
