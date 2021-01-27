using Company.Cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Game.Protocal;

public class Chapter4_module2 : ChapterControllerBasics
{
    #region 字段
    /// <summary>
    /// 下一步按钮
    /// </summary>
    Button _nextStepBtn;

    RectTransform _oneScene;
    RectTransform _twoScene;
    RectTransform _threeScene;
    RectTransform _fourScene;

    //表情
    Image nan2Face1;//低头说话
    Image nan2Face2;
    Image nan2Face3;
    Image nvzhuFace1;
    Image nvzhuFace2;

    DialogueBoxBubbleComponent bubbleComponent;//1
    CaricaturePlayerController twoCaricature;//2
    CaricaturePlayerController fourCaricature;//4

    RectTransform top;//扫描遮罩
    RectTransform bottom;
    RectTransform QRCode;//二维码
    Text QRCode_name_text;
    Text QRCode_region_text;
    RectTransform infoView;
    Text name_text;
    Text region_text;
    Text set_text;
    Text friend_text;
    Text more_text;
    Button addFriend_btn;
    Text addFriend_text;
    Button nextChapterBtn;//下一章按钮
    Text nextChapterText;
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
        _twoScene = transform.Find("Two") as RectTransform;
        _threeScene = transform.Find("Three") as RectTransform;
        _fourScene = transform.Find("Four") as RectTransform;

        //表情
        nan2Face1 = _oneScene.Find("image/nan2Face1").GetComponent<Image>();
        nan2Face2 = _oneScene.Find("image/nan2Face2").GetComponent<Image>();
        nan2Face3 = _oneScene.Find("image/nan2Face3").GetComponent<Image>();
        nvzhuFace1 = _oneScene.Find("image/nvzhuFace1").GetComponent<Image>();
        nvzhuFace2 = _oneScene.Find("image/nvzhuFace2").GetComponent<Image>();

        bubbleComponent = _oneScene.Find("DialogueBox_Bubble").GetComponent<DialogueBoxBubbleComponent>();
        twoCaricature = _twoScene.Find("CaricaturePlayer").GetComponent<CaricaturePlayerController>();
        fourCaricature = _fourScene.Find("CaricaturePlayer").GetComponent<CaricaturePlayerController>();

        top = _threeScene.Find("QRCode/Top") as RectTransform;
        bottom = _threeScene.Find("QRCode/Bottom") as RectTransform;
        QRCode = _threeScene.Find("QRCode") as RectTransform;
        QRCode_name_text = _threeScene.Find("QRCode/BG/Code/name_text").GetComponent<Text>();
        QRCode_region_text = _threeScene.Find("QRCode/BG/Code/region_text").GetComponent<Text>();
        infoView = _threeScene.Find("info") as RectTransform;
        name_text = _threeScene.Find("info/BG/Code/personInfo/name_text").GetComponent<Text>();
        region_text = _threeScene.Find("info/BG/Code/personInfo/region_text").GetComponent<Text>();
        set_text = _threeScene.Find("info/BG/Code/personInfo/set_text").GetComponent<Text>();
        friend_text = _threeScene.Find("info/BG/Code/personInfo/friend_text").GetComponent<Text>();
        more_text = _threeScene.Find("info/BG/Code/personInfo/more_text").GetComponent<Text>();
        addFriend_btn = _threeScene.Find("info/BG/Code/addFriend_btn").GetComponent<Button>();
        addFriend_text = addFriend_btn.transform.Find("Text").GetComponent<Text>();

        nextChapterBtn = _fourScene.Find("NextChapterBtn").GetComponent<Button>();
        nextChapterText = nextChapterBtn.transform.Find("NextChapterText").GetComponent<Text>();


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
        }
    }

    void ShowOneScene()
    {
        bubbleComponent.Initial(() =>
        {
            bubbleComponent.Close();
            OpenClickBtn(true);
            Debug.Log("对话1完毕");
        }, DialogueBeforeAciton, DialogueAfterAction);
        bubbleComponent.Show();
    }

    void DialogueBeforeAciton(ChapterDialogueTextDefine data)
    {
        if (data.ID == 14000036 || data.ID == 14000038)
        {//男2低头说话
            nan2Face1.gameObject.SetActive(true);
        }
        if (data.ID == 14000040 || data.ID == 14000045)
        {//男2抬头说话
            nan2Face2.gameObject.SetActive(true);
        }
        if (data.ID == 14000041 || data.ID == 14000043 || data.ID == 14000044)
        {//女主表情1
            nvzhuFace1.gameObject.SetActive(true);
        }
        if (data.ID == 14000042)
        {//男主闭眼
            nan2Face3.gameObject.SetActive(true);
        }
        if (data.ID == 14000045)
        {//男主睁眼
            nan2Face3.gameObject.SetActive(false);
        }
        if (data.ID == 14000046)
        {//女主表情2
            nvzhuFace2.gameObject.SetActive(true);
        }
        if (data.ID == 14000048)
        {//女主表情关闭
            nvzhuFace1.gameObject.SetActive(false);
            nvzhuFace2.gameObject.SetActive(false);
        }
    }

    void DialogueAfterAction(ChapterDialogueTextDefine data)
    {

    }
    //显示场景2的两张条漫
    void ShowTwoScene()
    {
        ChapterTool.FadeInFadeOut(_oneScene.GetComponent<CanvasGroup>(), 0, 0.1f, null, () =>
          {
              _oneScene.GetComponent<CanvasGroup>().blocksRaycasts = false;
          });
        twoCaricature.Initial(() =>
        {
            //OpenClickBtn(true);
            ShowThreeScene();
        }, playMonitor);
    }
    //条漫步骤
    void playMonitor(int index)
    {

    }
    //显示二维码
    async void ShowThreeScene()
    {
        ShowInfo();
        _twoScene.GetComponent<CanvasGroup>().DOFade(0, 1f).OnComplete(() =>
         {
             _twoScene.GetComponent<CanvasGroup>().blocksRaycasts = false;
             top.DOAnchorPosY(0, 1f);//移动框
             bottom.DOAnchorPosY(0, 1f);
         });

        await UniTask.Delay(4000);
        infoView.DOAnchorPosX(0, 0.8f).OnComplete(() =>
        {//移动完成后设置
            QRCode.gameObject.SetActive(false);
            addFriend_btn.onClick.RemoveAllListeners();
            addFriend_btn.onClick.AddListener(async () =>
            {
                _threeScene.GetComponent<CanvasGroup>().DOFade(0, 1f).OnComplete(() =>
                {
                    _threeScene.GetComponent<CanvasGroup>().blocksRaycasts = false;
                });
                ShowFourScene();
            });
        });
    }

    void ShowInfo()
    {
        QRCode_name_text.text = ChapterTool.GetChapterFunctionString(10000005);
        QRCode_region_text.text = ChapterTool.GetChapterFunctionString(10000537);
        name_text.text = ChapterTool.GetChapterFunctionString(10000005);
        region_text.text = ChapterTool.GetChapterFunctionString(10000537);
        set_text.text = ChapterTool.GetChapterFunctionString(10000538);
        friend_text.text = ChapterTool.GetChapterFunctionString(10000539);
        more_text.text = ChapterTool.GetChapterFunctionString(10000540);
        addFriend_text.text = ChapterTool.GetChapterFunctionString(10000536);
    }

    //显示场景4的两张条漫
    void ShowFourScene()
    {
        fourCaricature.Initial(() =>
        {
            //显示下一章按钮
            nextChapterText.text = ChapterTool.GetChapterFunctionString(10000000);//下一章
            nextChapterBtn.gameObject.SetActive(true);
            ChapterModuleManager._Instance.ClickEndChapterBtn(nextChapterBtn, NextStep);
            //请求这一章读完
            ChapterModuleManager._Instance.RequestPassChapter();
        }, null);
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
