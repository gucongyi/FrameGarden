using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chapter1_module3 : ChapterControllerBasics
{
    #region 字段
    /// <summary>
    /// 下一步按钮
    /// </summary>
    Button _nextStepBtn;
    RectTransform _oneScene;
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
        //_nextStepBtn = transform.GetComponent<Button>();
        //_nextStepBtn.onClick.RemoveAllListeners();
        //_nextStepBtn.onClick.AddListener(ClickBtn);
        _oneScene = transform.Find("One") as RectTransform;
        caricaturePlayerController = _oneScene.Find("CaricaturePlayer").GetComponent<CaricaturePlayerController>();
        nextBtn = _oneScene.Find("NextBtn").GetComponent<Button>();
        nextText = nextBtn.transform.Find("Text").GetComponent<Text>();

        //ClickBtn();
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
                ShowOneScene();
                break;
        }
    }

    void ShowOneScene()
    {
        caricaturePlayerController.Initial(OnclickNextBtn, CaricatureChanged);
    }
    //对步骤特殊处理
    void CaricatureChanged(int index)
    {
        //if (index == 4)//条漫第四张隐藏按钮
            //backBtn.gameObject.SetActive(false);
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
