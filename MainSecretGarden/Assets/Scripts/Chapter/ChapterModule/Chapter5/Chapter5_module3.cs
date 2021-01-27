using Company.Cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 男二线
/// </summary>
public class Chapter5_module3 : ChapterControllerBasics
{
    #region 字段
    /// <summary>
    /// 下一步按钮
    /// </summary>
    Button _nextStepBtn;

    RectTransform _oneScene;
    DialogueBoxBubbleComponent dialogueBoxBubble_one;

    RectTransform _twoScene;
    CaricaturePlayerController caricaturePlayerController_two;

    RectTransform _threeScene;
    DialogueBoxBubbleComponent dialogueBoxBubble_three;

    RectTransform _fourScene;
    DialogueBoxTetragonumComponent dialogueBox;

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
        dialogueBoxBubble_one = transform.Find("DialogueBox_Bubble_One").GetComponent<DialogueBoxBubbleComponent>();

        _twoScene = transform.Find("Two") as RectTransform;
        caricaturePlayerController_two = _twoScene.Find("CaricaturePlayer").GetComponent<CaricaturePlayerController>();

        _threeScene = transform.Find("Three") as RectTransform;
        dialogueBoxBubble_three = transform.Find("DialogueBox_Bubble_Three").GetComponent<DialogueBoxBubbleComponent>();

        _fourScene = transform.Find("Four") as RectTransform;
        dialogueBox = transform.Find("DialogueBox_Tetragonum").GetComponent<DialogueBoxTetragonumComponent>();

        ShowOneScene();
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
        switch (index)
        {
            case 1:
                break;
        }
    }
    void ShowOneScene()
    {
        dialogueBoxBubble_one.Initial(() =>
        {
            dialogueBoxBubble_one.Close();
            ShowTwoScene();
        }, BeforeOneDialogue, AfterOneDialogue);
        dialogueBoxBubble_one.Show();
    }

    void BeforeOneDialogue(ChapterDialogueTextDefine data)
    {
        if (data.ID == 15000056)
        {
            _oneScene.Find("Image1").gameObject.SetActive(false);
            _oneScene.Find("Image2").gameObject.SetActive(true);
        }
    }
    void AfterOneDialogue(ChapterDialogueTextDefine data)
    {

    }

    async System.Threading.Tasks.Task ShowTwoScene()
    {
        await ChapterTool.FadeInFadeOut(_oneScene.GetComponent<CanvasGroup>(), 0, 0.1f, null, async () =>
            {
                _oneScene.GetComponent<CanvasGroup>().blocksRaycasts = false;
                await ChapterTool.OpenDialogueBoxWeChat(15000058, 10000001, 10000005, () =>
                {
                    Debug.Log("微信聊天完毕！");
                    caricaturePlayerController_two.Initial(() =>
                    {
                        ShowThreeScene();
                    }, CaricatureChanged);
                });
            });

    }
    //对步骤特殊处理
    void CaricatureChanged(int index)
    {

    }
    //护士对话
    void ShowThreeScene()
    {
        _twoScene.GetComponent<CanvasGroup>().alpha = 0;
        _twoScene.GetComponent<CanvasGroup>().blocksRaycasts = false;
        dialogueBoxBubble_three.Initial(() =>
        {
            dialogueBoxBubble_three.Close();
            ShowFourScene();
        }, BeforeThreeDialogue, AfterThreeDialogue);
        dialogueBoxBubble_three.Show();
    }

    void BeforeThreeDialogue(ChapterDialogueTextDefine data)
    {
        if (data.ID == 15000099)
        {//转头
            _threeScene.Find("bg/nvzhuface_1").gameObject.SetActive(false);
            _threeScene.Find("bg/nvzhuface_2").gameObject.SetActive(true);
        }
    }
    void AfterThreeDialogue(ChapterDialogueTextDefine data)
    {

    }

    void ShowFourScene()
    {
        _threeScene.GetComponent<CanvasGroup>().alpha = 0;
        _threeScene.GetComponent<CanvasGroup>().blocksRaycasts = false;
        dialogueBox.Initial(() =>
        {
            dialogueBox.Close();
            //进入聊天
            ChatDriver();
        });
        dialogueBox.Show();
    }
    //和司机聊天
    async void ChatDriver()
    {
        await ChapterTool.OpenDialogueBoxWeChat(15000105, 10000001, 10000058, () =>
        {
            Debug.Log("微信聊天完毕！进入下一部分");
            NextStep();
        });
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
