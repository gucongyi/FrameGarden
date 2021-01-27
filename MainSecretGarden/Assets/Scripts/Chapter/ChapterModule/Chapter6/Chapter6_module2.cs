using Company.Cfg;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chapter6_module2 : ChapterControllerBasics
{
    #region 字段
    /// <summary>
    /// 下一步按钮
    /// </summary>
    Button _nextStepBtn;
    RectTransform _oneScene;
    CaricaturePlayerController caricaturePlayerController;

    RectTransform _twoScene;
    Image nv1_face;
    DialogueBoxBubbleComponent twoBubbleComponent;

    RectTransform _threeScene;
    [SerializeField]
    GameObject prefab;
    GameObject prefabItem;

    RectTransform _fourScene;
    Image fourImage_nv1_face1;
    Image fourImage_nv1_face2;
    Image fourImage_nv3_face;
    DialogueBoxBubbleComponent fourBubbleComponent;

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
        caricaturePlayerController = _oneScene.Find("CaricaturePlayer").GetComponent<CaricaturePlayerController>();

        _twoScene = transform.Find("Two") as RectTransform;
        nv1_face = _twoScene.Find("nvzhu_face").GetComponent<Image>();
        twoBubbleComponent = transform.Find("DialogueBox_Bubble_two").GetComponent<DialogueBoxBubbleComponent>();

        _threeScene = transform.Find("Three") as RectTransform;

        _fourScene = transform.Find("Four") as RectTransform;
        fourImage_nv1_face1 = _fourScene.transform.Find("nv1_face1").GetComponent<Image>();
        fourImage_nv1_face2 = _fourScene.transform.Find("nv1_face2").GetComponent<Image>();
        fourImage_nv3_face = _fourScene.transform.Find("nv3_face").GetComponent<Image>();
        fourBubbleComponent = transform.Find("DialogueBox_Bubble_four").GetComponent<DialogueBoxBubbleComponent>();

        nextBtn = transform.Find("NextChapterBtn").GetComponent<Button>();
        nextText = nextBtn.transform.Find("NextChapterText").GetComponent<Text>();

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
        caricaturePlayerController.ClickBtn();
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
                twoBubbleComponent.Close();
                ShowThreeScene();
            }, BeforeTwoDialogue, AfterTwoDialogue);
            twoBubbleComponent.Show();
        });
    }
    void BeforeTwoDialogue(ChapterDialogueTextDefine data)
    {
        if (data.ID == 16000051)
        {
            nv1_face.gameObject.SetActive(true);
        }
    }
    void AfterTwoDialogue(ChapterDialogueTextDefine data)
    {

    }

    async void ShowThreeScene()
    {
        prefabItem = Instantiate(prefab);
        _twoScene.GetComponent<CanvasGroup>().alpha = 0;
        _twoScene.GetComponent<CanvasGroup>().blocksRaycasts = false;
        await UniTask.Delay(5000);
        ShowFourScene();
    }

    void ShowFourScene()
    {
        _threeScene.GetComponent<CanvasGroup>().DOFade(0, 1f).OnComplete(() =>
        {
            _threeScene.GetComponent<CanvasGroup>().blocksRaycasts = false;

            fourBubbleComponent.Initial(() =>
            {
                fourBubbleComponent.Close();
                ReadOverThisChapter();
            }, BeforeFourDialogue, AfterFourDialogue);
            fourBubbleComponent.Show();
        });
    }

    void BeforeFourDialogue(ChapterDialogueTextDefine data)
    {
        if (data.ID == 16000056|| data.ID == 16000061|| data.ID == 16000065 || data.ID == 16000067)
        {
            fourImage_nv1_face1.gameObject.SetActive(false);
            fourImage_nv1_face2.gameObject.SetActive(true);
        }
        if (data.ID == 16000057|| data.ID == 16000066)
        {
            fourImage_nv1_face1.gameObject.SetActive(true);
            fourImage_nv1_face2.gameObject.SetActive(false);
        }
        if (data.ID == 16000060)
        {
            fourImage_nv1_face1.gameObject.SetActive(false);
            fourImage_nv1_face2.gameObject.SetActive(false);
        }
        if (data.ID == 16000062)
        {
            fourImage_nv1_face1.gameObject.SetActive(true);
            fourImage_nv1_face2.gameObject.SetActive(false);
            fourImage_nv3_face.gameObject.SetActive(true);
        }
        if (data.ID == 16000063)
        {
            fourImage_nv3_face.gameObject.SetActive(false);
        }
    }

    void AfterFourDialogue(ChapterDialogueTextDefine data)
    {

    }

    void ReadOverThisChapter()
    {
        nextText.text = ChapterTool.GetChapterFunctionString(10000000);//下一章
        nextBtn.gameObject.SetActive(true);
        ChapterModuleManager._Instance.ClickEndChapterBtn(nextBtn, NextStep, () =>
        {
            GameObject.Destroy(prefabItem.gameObject);
        });
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
