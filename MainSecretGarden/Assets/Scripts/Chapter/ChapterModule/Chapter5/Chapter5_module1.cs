using Company.Cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class Chapter5_module1 : ChapterControllerBasics
{
    #region 字段
    /// <summary>
    /// 下一步按钮
    /// </summary>
    Button _nextStepBtn;
    RectTransform _oneScene;
    CaricaturePlayerController caricaturePlayerController;

    RectTransform _twoScene;
    Image twoImage_1;
    Image twoImage_2;
    Image twoImage_3;
    Image twoNvZhuFace_1;
    Image twoNvZhuFace_2;
    Image twoNvZhuFace_3;//闭嘴
    DialogueBoxBubbleComponent twoBubbleComponent;

    RectTransform _threeScene;
    DialogueBoxBubbleComponent threeubbleComponent;

    BuyFlowersController buyFlowersController;
    DialogueBoxTetragonumComponent dialogueBox_BuyFlowersTip;//选花提示
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
        twoImage_1 = _twoScene.Find("Image_1").GetComponent<Image>();
        twoImage_2 = _twoScene.Find("Image_2").GetComponent<Image>();
        twoImage_3 = _twoScene.Find("Image_3").GetComponent<Image>();
        twoNvZhuFace_1 = _twoScene.Find("nvzhuFace_1").GetComponent<Image>();
        twoNvZhuFace_2 = _twoScene.Find("nvzhuFace_2").GetComponent<Image>();
        twoNvZhuFace_3 = _twoScene.Find("nvzhuFace_3").GetComponent<Image>();
        twoBubbleComponent = transform.Find("DialogueBox_Bubble_two").GetComponent<DialogueBoxBubbleComponent>();

        _threeScene = transform.Find("Three") as RectTransform;
        threeubbleComponent = transform.Find("DialogueBox_Bubble_three").GetComponent<DialogueBoxBubbleComponent>();

        buyFlowersController = transform.Find("BuyFlowers").GetComponent<BuyFlowersController>();
        dialogueBox_BuyFlowersTip = transform.Find("DialogueBox_BuyFlowersTip").GetComponent<DialogueBoxTetragonumComponent>();

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
                ShowThreeScene();
                break;
            case 4:
                MoveThreeScene();
                break;
            case 5:
                ShowBuyFlowersController();
                break;
        }
    }

    void ShowOneScene()
    {
        caricaturePlayerController.Initial(async () =>
        {
            await UniTask.Delay(500);
            ClickBtn();
        }, CaricatureChanged);
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
            twoBubbleComponent.Initial(() =>
            {
                ClickBtn();
                twoBubbleComponent.Close();
            }, BeforeTwoDialogue, AfterTwoDialogue);
            twoBubbleComponent.Show();
        });
        //闪退
        //await ChapterTool.FadeInFadeOut(_oneScene.GetComponent<CanvasGroup>(), 0, 0.1f, null, () =>
        //{
        //    _oneScene.GetComponent<CanvasGroup>().blocksRaycasts = false;
        //    twoBubbleComponent.Initial(() =>
        //    {
        //        OpenClickBtn(true);
        //        twoBubbleComponent.Close();
        //    }, BeforeTwoDialogue, AfterTwoDialogue);
        //});
        //twoBubbleComponent.Show();
    }

    void BeforeTwoDialogue(ChapterDialogueTextDefine data)
    {
        //image2
        if (data.ID == 15000005)
        {
            twoNvZhuFace_1.gameObject.SetActive(true);
            twoNvZhuFace_3.gameObject.SetActive(true);
            twoImage_1.gameObject.SetActive(false);
            twoImage_2.gameObject.SetActive(true);
        }
        if (data.ID == 15000006)
        {
            twoNvZhuFace_3.gameObject.SetActive(false);
        }
        if (data.ID == 15000008)
        {
            twoNvZhuFace_1.gameObject.SetActive(false);
        }
        if (data.ID == 15000009)
        {
            twoNvZhuFace_2.gameObject.SetActive(true);
        }
        //image3
        if (data.ID == 15000010)
        {
            twoImage_2.gameObject.SetActive(false);
            twoNvZhuFace_2.gameObject.SetActive(false);
            twoImage_3.gameObject.SetActive(true);
            twoNvZhuFace_3.gameObject.SetActive(true);
        }
        if (data.ID == 15000012)
        {
            twoNvZhuFace_3.gameObject.SetActive(false);
            twoNvZhuFace_2.gameObject.SetActive(true);
        }
        if (data.ID == 15000015)
        {
            twoNvZhuFace_2.gameObject.SetActive(false);
            twoImage_3.gameObject.SetActive(false);
            twoImage_1.gameObject.SetActive(true);
        }
        if (data.ID == 15000016)
        {
            twoNvZhuFace_2.gameObject.SetActive(true);
        }
        if (data.ID == 15000019)
        {
            twoNvZhuFace_2.gameObject.SetActive(false);
        }

        if (data.ID == 15000020)
        {
            twoNvZhuFace_1.gameObject.SetActive(true);
        }
    }
    void AfterTwoDialogue(ChapterDialogueTextDefine data)
    {

    }

    async void ShowThreeScene()
    {
        await ChapterTool.FadeInFadeOut(_twoScene.GetComponent<CanvasGroup>(), 0, 0.1f, null, () =>
        {
            _twoScene.GetComponent<CanvasGroup>().blocksRaycasts = false;
            ClickBtn();
        });
    }

    async void MoveThreeScene()
    {
        RectTransform bg = _threeScene.Find("Image_1") as RectTransform;
        await ChapterTool.MoveUi(bg, new Vector2(-929.71f, 0), 0.1f, 0.1f, null, () =>
        {
            threeubbleComponent.Initial(() =>
            {
                ClickBtn();
                threeubbleComponent.Close();
            });
        });
        threeubbleComponent.Show();
    }

    async void ShowBuyFlowersController()
    {
        dialogueBox_BuyFlowersTip.Initial(() =>
        {//提示初始化
            dialogueBox_BuyFlowersTip.Close();
        });
        await ChapterTool.FadeInFadeOut(_threeScene.GetComponent<CanvasGroup>(), 0, 0.1f, null, () =>
        {
            _threeScene.GetComponent<CanvasGroup>().blocksRaycasts = false;
            buyFlowersController.Initial(() =>
            {
                Debug.Log("选花完成");
                NextStep();
            });
            dialogueBox_BuyFlowersTip.Show();
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