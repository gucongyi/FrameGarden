using Company.Cfg;
using Game.Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chapter2_module2 : ChapterControllerBasics
{
    #region 字段
    /// <summary>
    /// 下一步按钮
    /// </summary>
    Button _nextStepBtn;

    RectTransform _oneScene;
    RectTransform _twocene;

    DialogueBoxBubbleComponent dialogue_One;
    //场景1表情切换
    Image tire_image1;
    Image tire_image2;
    Image tire_image4;
    Image tire_image5;


    CaricaturePlayerController caricaturePlayerController;

    Button nextBtn;
    Text nextText;

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
        dialogue_One = transform.Find("DialogueBox_Bubble_one").GetComponent<DialogueBoxBubbleComponent>();
        tire_image1 = _oneScene.Find("tire_image1").GetComponent<Image>();
        tire_image2 = _oneScene.Find("tire_image2").GetComponent<Image>();
        tire_image4 = _oneScene.Find("tire_image4").GetComponent<Image>();
        tire_image5 = _oneScene.Find("tire_image5").GetComponent<Image>();

        caricaturePlayerController = _twocene.Find("CaricaturePlayer").GetComponent<CaricaturePlayerController>();
        nextBtn = _twocene.Find("NextBtn").GetComponent<Button>();
        nextText = nextBtn.transform.Find("Text").GetComponent<Text>();

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
        }
    }

    void ShowOneScene()
    {
        dialogue_One.Initial(() =>
        {
            dialogue_One.Close();
            ClickBtn();
            Debug.Log("对话完毕");
        }, OneSpeakBeforeAciton, OneSpeakAfterAction);
        dialogue_One.Show();
    }

    void OneSpeakBeforeAciton(ChapterDialogueTextDefine data)
    {
        if (data.ID == 13000050)//第三句话
        {//变为第二张图
            tire_image1.gameObject.SetActive(false);
            tire_image2.gameObject.SetActive(true);
        }
        //第6句话 小葱 今天幸苦你了
        if (data.ID == 13000053)//第7句话
        {
            //Debug.LogError("切换第3张图");
            //tire_image3.gameObject.SetActive(true);
        }
        if (data.ID == 13000054)//第7句话
        {
            tire_image2.gameObject.SetActive(false);
            tire_image4.gameObject.SetActive(true);
        }
        if (data.ID == 13000055)//第7句话
        {
            tire_image4.gameObject.SetActive(false);
            tire_image5.gameObject.SetActive(true);
        }
    }

    void OneSpeakAfterAction(ChapterDialogueTextDefine data)
    {

    }

    async void ShowTwoScene()
    {
        await ChapterTool.FadeInFadeOut(_oneScene.GetComponent<CanvasGroup>(), 0, 0.3f, null, () =>
        {
            caricaturePlayerController.Initial(ReadOverThisChapter, CaricatureChanged);
        });
    }
    //对步骤特殊处理
    void CaricatureChanged(int index)
    {

    }

    void ReadOverThisChapter()
    {
        nextText.text = ChapterTool.GetChapterFunctionString(10000000);//下一章
        nextBtn.gameObject.SetActive(true);
        ChapterModuleManager._Instance.ClickEndChapterBtn(nextBtn, NextStep);
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
