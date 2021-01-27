using Company.Cfg;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chapter6_module1 : ChapterControllerBasics
{
    #region 字段
    /// <summary>
    /// 下一步按钮
    /// </summary>
    Button _nextStepBtn;
    RectTransform _oneScene;
    CaricaturePlayerController caricaturePlayerController;

    RectTransform _twoScene;
    Image twoImage;
    DialogueBoxBubbleComponent twoBubbleComponent;

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
        caricaturePlayerController = _oneScene.Find("CaricaturePlayer").GetComponent<CaricaturePlayerController>();

        _twoScene = transform.Find("Two") as RectTransform;
        twoImage = _twoScene.Find("Image_1").GetComponent<Image>();
        twoBubbleComponent = transform.Find("DialogueBox_Bubble_two").GetComponent<DialogueBoxBubbleComponent>();


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
                MoveTwoScene();
                break;
            case 2:
                break;
        }
    }

    void ShowOneScene()
    {
        caricaturePlayerController.Initial(() =>
        {
            ShowTwoScene();
        }, CaricatureChanged);
        //caricaturePlayerController.ClickBtn();
    }
    //对步骤特殊处理
    void CaricatureChanged(int index)
    {
    }
    void ShowTwoScene()
    {
        _oneScene.GetComponent<CanvasGroup>().DOFade(0, 1f).OnComplete(() =>
        {
            _oneScene.GetComponent<CanvasGroup>().blocksRaycasts = false;

            OpenClickBtn(true);
        });
    }

    void MoveTwoScene()
    {
        twoImage.transform.DOLocalMoveX(-658, 1).OnComplete(() =>
        {
            twoBubbleComponent.Initial(() =>
            {
                twoBubbleComponent.Close();
                NextStep();
            }, BeforeTwoDialogue, AfterTwoDialogue);
            twoBubbleComponent.Show();
        });
    }

    void BeforeTwoDialogue(ChapterDialogueTextDefine data)
    {

    }
    void AfterTwoDialogue(ChapterDialogueTextDefine data)
    {

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
