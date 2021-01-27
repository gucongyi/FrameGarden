using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Company.Cfg;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public class Chapter1_module1 : ChapterControllerBasics
{
    #region 字段
    /// <summary>
    /// 下一步按钮
    /// </summary>
    Button _nextStepBtn;
    //场景
    RectTransform _oneScene;
    RectTransform _twoScene;
    RectTransform _threeScene;
    RectTransform _fourScene;
    //转场大树
    RectTransform _bigTreeScene;

    DialogueBoxBubbleComponent _dialogueTwo;//场景2对话 
    DialogueBoxBubbleComponent _dialogueThree;//场景3对话
    DialogueBoxBubbleComponent _dialogueFour;//场景4对话
    DialogueBoxBubbleComponent _dialogueFive;//hhhhh

    //表情切换scene1
    [SerializeField]
    List<Image> nvzhuFace_scene1 = new List<Image>();

    Image yeyeFace_scene1;
    Image nanzhuFace_scene1;

    //scene2
    Image nanzhuFace_scene2;
    Image nvzhuFace_scene2;
    Image yeyeFace1_scene2;
    Image yeyeFace2_scene2;

    RectTransform Option;//选项
    Button option1;//按钮1
    Button option2;//按钮2

    //scene3
    Image nanzhuFace1_scene3;
    Image nanzhuFace2_scene3;
    Image nvzhuFace_scene3;
    Image yeyeFace1_scene3;
    Image yeyeFace2_scene3;
    Image yeyeFace3_scene3;
    Image yeyeFace4_scene3;
    Image yeyeFace5_scene3;

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
        _threeScene = transform.Find("Three") as RectTransform;
        _fourScene = transform.Find("Four") as RectTransform;
        _bigTreeScene = transform.Find("CutTo") as RectTransform;

        _dialogueTwo = transform.Find("DialogueBox_Bubble_two").GetComponent<DialogueBoxBubbleComponent>();
        _dialogueThree = transform.Find("DialogueBox_Bubble_three").GetComponent<DialogueBoxBubbleComponent>();
        _dialogueFour = transform.Find("DialogueBox_Bubble_four").GetComponent<DialogueBoxBubbleComponent>();
        _dialogueFive = transform.Find("DialogueBox_Bubble_five").GetComponent<DialogueBoxBubbleComponent>();

        //表情切换
        yeyeFace_scene1 = _twoScene.Find("nvzhu_image/yeye_face").GetComponent<Image>();
        nanzhuFace_scene1 = _twoScene.Find("nanzhu_image/nanzhu_face").GetComponent<Image>();

        nanzhuFace_scene2 = _threeScene.Find("nanzhu_image/nanzhuFace").GetComponent<Image>();
        nvzhuFace_scene2 = _threeScene.Find("nvzhu_image/nvzhuFace").GetComponent<Image>();
        yeyeFace1_scene2 = _threeScene.Find("nvzhu_image/yeFace1").GetComponent<Image>();
        yeyeFace2_scene2 = _threeScene.Find("nvzhu_image/yeFace2").GetComponent<Image>();

        nanzhuFace1_scene3 = _fourScene.Find("threeCompany_image/nanzhuFace1").GetComponent<Image>();
        nanzhuFace2_scene3 = _fourScene.Find("threeCompany_image/nanzhuFace2").GetComponent<Image>();
        nvzhuFace_scene3 = _fourScene.Find("threeCompany_image/nvzhuFace").GetComponent<Image>();
        yeyeFace1_scene3 = _fourScene.Find("threeCompany_image/yeyeFace1").GetComponent<Image>();
        yeyeFace2_scene3 = _fourScene.Find("threeCompany_image/yeyeFace2").GetComponent<Image>();
        yeyeFace3_scene3 = _fourScene.Find("threeCompany_image/yeyeFace3").GetComponent<Image>();
        yeyeFace4_scene3 = _fourScene.Find("threeCompany_image/yeyeFace4").GetComponent<Image>();
        yeyeFace5_scene3 = _fourScene.Find("threeCompany_image/yeyeFace5").GetComponent<Image>();

        Option = transform.Find("SelectOption").GetComponent<RectTransform>();
        option1 = Option.Find("option1").GetComponent<Button>();
        option2 = Option.Find("option2").GetComponent<Button>();
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
        if (index > 6) return;
        switch (index)
        {
            case 1:
                moveOne();
                break;
            case 2:
                ShowTwoScene();
                break;
            case 3:
                ShowThreeScene();
                break;
            case 4:
                ShowFourScene();
                break;
            case 5:
                ShowOption();
                break;
            case 6://hhh对话
                //ShowHHH();
                break;
        }
    }
    //场景1移动
    async void moveOne()
    {
        await ChapterTool.MoveUi(_oneScene, new Vector2(-621, 0), moveXspeed, 0.1f, null, () => { ClickBtn(); });
    }
    //显示3人走路
    async void ShowTwoScene()
    {
        await ChapterTool.FadeInFadeOut(_oneScene.GetComponent<CanvasGroup>(), 0, 0.1f, null, () =>
            {
                _dialogueTwo.Initial(() =>
                {
                    Debug.Log("对话完毕");
                    ClickBtn();
                    _dialogueTwo.Close();
                }, SpeakAeforeAction_Two, null, SpeakCloseAction_Two);
            });
        _dialogueTwo.Show();
    }
    /// <summary>
    /// 人物说话前回调（移动镜头）
    /// </summary>
    /// <param name="data"></param>
    public async void SpeakAeforeAction_Two(ChapterDialogueTextDefine data)
    {
        _dialogueTwo.SetRoleSpeakAeforeAwait(true);
        float x = 589;
        switch (data.NameID)
        {
            case 10000001:
                x = 589;
                break;
            case 10000002:
                x = -452;
                break;
            case 10000004:
                x = 203;
                break;
        }
        await ChapterTool.MoveUi(_twoScene, new Vector2(x, _twoScene.localPosition.y), moveXspeed, 0.1f, null, () =>
        {
            if (data.ID == 12000001)
            {
                SwitchFace(3);
            }
            if (data.ID == 12000002 || data.ID == 12000005 || data.ID == 12000009)
            {//爷说话 说话前女闭嘴
                yeyeFace_scene1.gameObject.SetActive(false);
            }
            if (data.ID == 12000003 || data.ID == 12000007 || data.ID == 12000014)
            {//女主说话切换表情
                SwitchFace(1);//眯眼笑
            }
            if (data.ID == 12000006)
            {//打算的话。。。
                SwitchFace(4);//o嘴

            }
            if (data.ID == 12000010|| data.ID == 12000012)
            {
                SwitchFace(2);//害羞
            }
            if (data.ID == 12000011 || data.ID == 12000013)
            {//男说话 女主闭嘴
                nanzhuFace_scene1.gameObject.SetActive(true);
            }

            _dialogueTwo.SetRoleSpeakAeforeAwait(false);
        });
    }
    /// <summary>
    /// 人物说话后回调
    /// </summary>
    /// <param name="data"></param>
    public void SpeakCloseAction_Two(ChapterDialogueTextDefine data)
    {
        if (data.ID == 12000002|| data.ID == 12000005 || data.ID == 12000009|| data.ID == 12000011)
        {//12000002爷爷说话前，12000001关闭时女1闭嘴
            SwitchFace(0);
        }
        if (data.ID == 12000003 || data.ID == 12000006 || data.ID == 12000010)
        {//爷闭嘴
            yeyeFace_scene1.gameObject.SetActive(true);
        }
        if (data.ID == 12000012 || data.ID == 12000014)
        {//男说完闭嘴
            nanzhuFace_scene1.gameObject.SetActive(false);
        }
    }
    //女主显示指定表情
    void SwitchFace(int index)
    {
        for (int i = 0; i < nvzhuFace_scene1.Count; i++)
        {
            nvzhuFace_scene1[i].gameObject.SetActive(false);
        }
        nvzhuFace_scene1[index].gameObject.SetActive(true);
    }

    //显示侧面3人
    async void ShowThreeScene()
    {
        await ChapterTool.FadeInFadeOut(_twoScene.GetComponent<CanvasGroup>(), 0, 1f, null, () =>
         {
             _dialogueThree.Initial(() =>
             {
                 Debug.Log("对话完毕");
                 ClickBtn();
                 _dialogueThree.Close();
             }, SpeakBeforeAction_Three, SpeakAfterAction_Three);
         });
        _dialogueThree.Show();
    }
    //表情切换
    void SpeakBeforeAction_Three(ChapterDialogueTextDefine data)
    {
        if (data.ID == 12000015)
        {
            yeyeFace1_scene2.gameObject.SetActive(false);
        }
        if (data.ID == 12000017)
        {
            yeyeFace1_scene2.gameObject.SetActive(true);
            nvzhuFace_scene2.gameObject.SetActive(false);
        }
        if (data.ID == 12000018)
        {
            nvzhuFace_scene2.gameObject.SetActive(true);
            nanzhuFace_scene2.gameObject.SetActive(true);
        }
        if (data.ID == 12000019)
        {
            nanzhuFace_scene2.gameObject.SetActive(false);
            yeyeFace1_scene2.gameObject.SetActive(false);
            yeyeFace2_scene2.gameObject.SetActive(true);
        }
    }

    void SpeakAfterAction_Three(ChapterDialogueTextDefine data)
    {

    }

    //显示座椅3人的近景
    void ShowFourScene()
    {
        (_bigTreeScene.transform as RectTransform).DOAnchorPosX(-2500, 3f);
        //移动场景4
        (_fourScene.transform as RectTransform).DOAnchorPosX(-76, 3f).OnComplete(() =>
         {
             _dialogueFour.Initial(() =>
             {
                 //backBtn.gameObject.SetActive(false);//第三幅场景说完话隐藏按钮
                 Debug.Log("对话Four完毕");
                 _dialogueFour.Close();
                 ClickBtn();

             }, SpeakBeforeAction_Four, SpeakAfterAction_Four);
             _dialogueFour.Show();

             _bigTreeScene.gameObject.SetActive(false);
         });
    }

    //表情切换
    void SpeakBeforeAction_Four(ChapterDialogueTextDefine data)
    {
        if (data.ID == 12000020)
        {
            nanzhuFace1_scene3.gameObject.SetActive(false);
            nanzhuFace2_scene3.gameObject.SetActive(true);
            yeyeFace4_scene3.gameObject.SetActive(false);
            yeyeFace1_scene3.gameObject.SetActive(true);
        }
        if (data.ID == 12000021)
        {
            nanzhuFace1_scene3.gameObject.SetActive(true);
            nanzhuFace2_scene3.gameObject.SetActive(false);
            nvzhuFace_scene3.gameObject.SetActive(true);
        }
        if (data.ID == 12000022)
        {
            nvzhuFace_scene3.gameObject.SetActive(false);
            yeyeFace2_scene3.gameObject.SetActive(true);
        }
        if (data.ID == 12000023)
        {
            yeyeFace2_scene3.gameObject.SetActive(false);
            nanzhuFace1_scene3.gameObject.SetActive(false);
            nanzhuFace2_scene3.gameObject.SetActive(true);
        }
        if (data.ID == 12000024)
        {
            yeyeFace2_scene3.gameObject.SetActive(true);
            nanzhuFace1_scene3.gameObject.SetActive(true);
            nanzhuFace2_scene3.gameObject.SetActive(false);
        }
        if (data.ID == 12000025)
        {
            yeyeFace1_scene3.gameObject.SetActive(false);
            yeyeFace2_scene3.gameObject.SetActive(false);
            yeyeFace3_scene3.gameObject.SetActive(true);
        }

    }
    public void SpeakAfterAction_Four(ChapterDialogueTextDefine data)
    {

    }
    //显示选项按钮
    void ShowOption()
    {
        Option.gameObject.SetActive(true);
        option1.transform.Find("Text").GetComponent<Text>().text = ChapterTool.GetDialogueString(12000026);
        option2.transform.Find("Text").GetComponent<Text>().text = ChapterTool.GetDialogueString(12000027);
        option1.onClick.AddListener(() =>
        {
            //增加好感
            ImpulseHelper.OptionAddFavorable(12000026);
            Option.gameObject.SetActive(false);


            _dialogueFive.SetStartDialogueId(12000026);
            _dialogueFive.Initial(() =>
            {
                _dialogueFive.Close();
                NextStep();
            }, SpeakAeforeAction_Five);
            _dialogueFive.Show();

        });
        option2.onClick.AddListener(() =>
        {
            Option.gameObject.SetActive(false);
            _dialogueFive.SetStartDialogueId(12000027);
            _dialogueFive.Initial(() =>
            {
                _dialogueFive.Close();
                NextStep();
            }, SpeakAeforeAction_Five);
            _dialogueFive.Show();
        });
    }

    void ShowHHH()
    {
        _dialogueFive.Initial(() =>
        {
            Debug.Log("对话完毕");
            NextStep();
            _dialogueFive.Close();
        }, SpeakAeforeAction_Five);
        _dialogueFive.Show();
    }

    void SpeakAeforeAction_Five(ChapterDialogueTextDefine data)
    {
        if (data.ID == 12000026 || data.ID == 12000027)
        {
            nvzhuFace_scene3.gameObject.SetActive(true);
        }
        if (data.ID == 12000028)
        {
            nvzhuFace_scene3.gameObject.SetActive(false);
            yeyeFace3_scene3.gameObject.SetActive(false);
            yeyeFace5_scene3.gameObject.SetActive(true);
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