using Company.Cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chapter2_module3 : ChapterControllerBasics
{
    #region 字段
    /// <summary>
    /// 下一步按钮
    /// </summary>
    Button _nextStepBtn;

    RectTransform _oneScene;
    RectTransform _twocene;

    DialogueBoxBubbleComponent dialogue_One;
    Image tire_image2;
    Image tire_image3;
    Image tire_image4;
    //Image tire_image5;


    CaricaturePlayerController caricaturePlayerController;

    Button nextBtn;
    Text nextText;

    float moveXspeed = 350f;
    /// <summary>
    /// 点击计数
    /// </summary>
    int _clickIndex = 1;
    #endregion
    #region 函数

    public override void Initial()
    {
        _nextStepBtn = transform.GetComponent<Button>();
        _nextStepBtn.onClick.RemoveAllListeners();
        _nextStepBtn.onClick.AddListener(ClickBtn);
        _oneScene = transform.Find("One") as RectTransform;
        _twocene = transform.Find("Two") as RectTransform;

        tire_image2 = _oneScene.Find("tire_image2").GetComponent<Image>();
        tire_image3 = _oneScene.Find("tire_image3").GetComponent<Image>();
        tire_image4 = _oneScene.Find("tire_image4").GetComponent<Image>();
        //tire_image5 = _oneScene.Find("tire_image5").GetComponent<Image>();

        caricaturePlayerController = _twocene.Find("CaricaturePlayer").GetComponent<CaricaturePlayerController>();
        nextBtn = _twocene.Find("NextBtn").GetComponent<Button>();
        nextText = nextBtn.transform.Find("Text").GetComponent<Text>();

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
                ShowTwoScene();
                break;
            case 2:

                break;
        }
    }

    void ShowOneScene()
    {
        dialogue_One.Initial(() =>
        {
            dialogue_One.Close();
            OpenClickBtn(true);
            Debug.Log("对话完毕");
        }, OneSpeakBeforeAciton, OneSpeakAfterAction);
        dialogue_One.Show();
    }

    void OneSpeakBeforeAciton(ChapterDialogueTextDefine data)
    {
        if (data.ID == 1111111)//第三句话
        {//变为第二张图
            tire_image2.gameObject.SetActive(true);
        }
        if (data.ID == 1111112)//第6句话
        {
            tire_image3.gameObject.SetActive(true);
        }
        if (data.ID == 1111113)//第7句话
        {
            tire_image4.gameObject.SetActive(true);
        }
    }

    void OneSpeakAfterAction(ChapterDialogueTextDefine data)
    {

    }

    async void ShowTwoScene()
    {
        await ChapterTool.FadeInFadeOut(_oneScene.GetComponent<CanvasGroup>(), 0, 0.3f, null, () =>
        {
            caricaturePlayerController.Initial(OnclickNextBtn, CaricatureChanged);
        });
    }
    //对步骤特殊处理
    void CaricatureChanged(int index)
    {

    }

    async void OnclickNextBtn()
    {
        nextText.text = ChapterTool.GetChapterFunctionString(10000503);//去跳舞
        await ChapterTool.FadeInFadeOut(nextBtn.GetComponent<CanvasGroup>(), 1, 0.3f, null, () =>
        {
            nextBtn.enabled = true;
        });
        nextBtn.onClick.RemoveAllListeners();
        nextBtn.onClick.AddListener(() =>
        {
            nextBtn.enabled = false;
            caricaturePlayerController.gameObject.SetActive(false);
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
