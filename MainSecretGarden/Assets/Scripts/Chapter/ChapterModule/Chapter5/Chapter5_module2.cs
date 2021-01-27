using Company.Cfg;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chapter5_module2 : ChapterControllerBasics
{
    #region 字段
    /// <summary>
    /// 下一步按钮
    /// </summary>
    Button _nextStepBtn;

    RectTransform _oneScene;
    DialogueBoxBubbleComponent dialogueBoxBubble_one;

    RectTransform _twoScene;
    DialogueBoxBubbleComponent dialogueBoxBubble_two;
    DialogueBoxBubbleComponent dialogueBoxBubble_three;

    Image nan_face1;
    Image nan_face2;
    Image nan_face3;
    Image nv_face1;
    Image nv_face2;
    Image nv_face3;
    Image nv_face;


    Button option1;
    Button option2;
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
        dialogueBoxBubble_two = transform.Find("DialogueBox_Bubble_Two").GetComponent<DialogueBoxBubbleComponent>();
        dialogueBoxBubble_three = transform.Find("DialogueBox_Bubble_Three").GetComponent<DialogueBoxBubbleComponent>();

        nan_face1 = _twoScene.Find("nanzhu/nanzhu_1").GetComponent<Image>();
        nan_face2 = _twoScene.Find("nanzhu/nanzhu_2").GetComponent<Image>();
        nan_face3 = _twoScene.Find("nanzhu/nanzhu_3").GetComponent<Image>();
        nv_face1 = _twoScene.Find("nvzhu/nvzhu_1").GetComponent<Image>();
        nv_face2 = _twoScene.Find("nvzhu/nvzhu_2").GetComponent<Image>();
        nv_face3 = _twoScene.Find("nvzhu/nvzhu_3").GetComponent<Image>();
        nv_face = _twoScene.Find("nvzhu/nvzhu_face").GetComponent<Image>();

        option1 = transform.Find("btns/option1").GetComponent<Button>();
        option2 = transform.Find("btns/option2").GetComponent<Button>();
        var data1 = ChapterTool.GetChapterData(15000038);
        var data2 = ChapterTool.GetChapterData(15000039);

        option1.transform.Find("Text").GetComponent<Text>().text = ChapterTool.GetDialogueString(data1);
        option2.transform.Find("Text").GetComponent<Text>().text = ChapterTool.GetDialogueString(data2);


        option1.onClick.AddListener(() =>
        {
            option1.gameObject.SetActive(false);
            option2.gameObject.SetActive(false);
            ImpulseHelper.OptionAddFavorable(15000038);
            SetNextIndex(2);
            dialogueBoxBubble_three.SetStartDialogueId(15000038);
            dialogueBoxBubble_three.Initial(() =>
            {
                dialogueBoxBubble_three.Close();
                NextStep();
            }, AfterThreeDialogue);
            dialogueBoxBubble_three.Show();

        });
        option2.onClick.AddListener(() =>
        {
            option1.gameObject.SetActive(false);
            option2.gameObject.SetActive(false);

            ImpulseHelper.OptionAddFavorable(15000039);
            SetNextIndex(3);
            dialogueBoxBubble_three.SetStartDialogueId(15000039);
            dialogueBoxBubble_three.Initial(() =>
            {
                dialogueBoxBubble_three.Close();
                NextStep();
            }, AfterThreeDialogue);
            dialogueBoxBubble_three.Show();
        });

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
                ShowTwoScene();
                break;
            case 3:

                break;
        }
    }

    void ShowOneScene()
    {
        dialogueBoxBubble_one.Initial(() =>
        {
            ClickBtn();
            dialogueBoxBubble_one.Close();
        }, BeforeOneDialogue, AfterOneDialogue);
        dialogueBoxBubble_one.Show();
    }

    void BeforeOneDialogue(ChapterDialogueTextDefine data)
    {

    }
    void AfterOneDialogue(ChapterDialogueTextDefine data)
    {

    }

    void ShowTwoScene()
    {
        _oneScene.GetComponent<CanvasGroup>().DOFade(0, 1f).OnComplete(() =>
        {
            _oneScene.GetComponent<CanvasGroup>().blocksRaycasts = false;
            dialogueBoxBubble_two.Initial(() =>
            {

            }, BeforeTwoDialogue, AfterTwoDialogue);
            dialogueBoxBubble_two.Show();
        });
    }

    void BeforeTwoDialogue(ChapterDialogueTextDefine data)
    {
        if (data.ID == 15000031 || data.ID == 15000034)
        {
            nan_face1.gameObject.SetActive(false);
            nan_face2.gameObject.SetActive(true);
        }
        if (data.ID == 15000032)
        {
            nan_face1.gameObject.SetActive(true);
            nan_face2.gameObject.SetActive(false);
        }
        if (data.ID == 15000036)
        {
            nv_face1.gameObject.SetActive(false);
            nv_face2.gameObject.SetActive(true);
        }
        if (data.ID == 15000037)
        {
            nan_face1.gameObject.SetActive(false);
            nan_face3.gameObject.SetActive(true);
        }
    }

    async void AfterTwoDialogue(ChapterDialogueTextDefine data)
    {
        if (data.ID == 15000037)
        {
            option1.gameObject.SetActive(true);
            option2.gameObject.SetActive(true);
            Debug.LogError("关闭对话框");
            await UniTask.Delay(500);
            dialogueBoxBubble_two.Close();//对话完后关闭对话框
        }
    }

    void AfterThreeDialogue(ChapterDialogueTextDefine data)
    {
        if (data.ID == 15000038)
        {//男2线
            nv_face3.gameObject.SetActive(true);
            nv_face2.gameObject.SetActive(false);
        }
        if (data.ID == 15000039)
        {//男1线
            nv_face3.gameObject.SetActive(true);
            nv_face2.gameObject.SetActive(false);
            nv_face.gameObject.SetActive(true);
        }
        if (data.ID == 15000044)
        {
            nv_face.gameObject.SetActive(false);
        }
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
