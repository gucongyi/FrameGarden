using Company.Cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chapter4_module1 : ChapterControllerBasics
{
    #region 字段
    /// <summary>
    /// 下一步按钮
    /// </summary>
    Button _nextStepBtn;
    RectTransform _oneScene;

    CaricaturePlayerController caricaturePlayerController;

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

        }
    }

    void ShowOneScene()
    {
        caricaturePlayerController.Initial(NextStep, CaricatureChanged);
    }
    //对步骤特殊处理
    void CaricatureChanged(int index)
    {
       // if (index == 4)
            //backBtn.gameObject.SetActive(false);
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

