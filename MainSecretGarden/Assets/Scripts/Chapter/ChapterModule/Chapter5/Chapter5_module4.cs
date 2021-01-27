using Company.Cfg;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 男一线
/// </summary>
public class Chapter5_module4 : ChapterControllerBasics
{
    #region 字段
    /// <summary>
    /// 下一步按钮
    /// </summary>
    Button _nextStepBtn;

    RectTransform _oneScene;
    CaricaturePlayerController Caricature;

    RectTransform _twoScene;
    DialogueBoxBubbleComponent dialogueBoxBubble_two;
    Image role_nv1;
    Image role_nv2;
    Image role_nan1;
    Image role_nan2;
    Image nvface;
    Image nanface;
    Image door1;
    Image door2;
    Image jiantou;
    Image lianHongImage;//对话上的脸红Q图
    Image Triangle;//三角
    RectTransform _threeScene;
    HotPotController hotPotController;
    DialogueBoxBubbleComponent dialogueBoxBubble_three;

    RectTransform _fourScene;
    public GameObject walk_5_parfab;
    GameObject walk_5;
    DialogueBoxBubbleComponent dialogueBoxBubble_four;
    Image nanzhuFace;

    RectTransform _fiveScene;
    CaricaturePlayerController Caricature_five;

    RectTransform _sixScene;
    DialogueBoxBubbleComponent dialogueBoxBubble_six;
    Image image1;
    Image image2;
    Image image3;

    /// <summary>
    /// 点击计数
    /// </summary>
    int _clickIndex = 1;
    #endregion
    #region 函数
    // Start is called before the first frame update
    void Start()
    {
        door2.transform.DOLocalMoveY(1, 1).Loops();
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
        Caricature = _oneScene.Find("CaricaturePlayer").GetComponent<CaricaturePlayerController>();

        _twoScene = transform.Find("Two") as RectTransform;
        dialogueBoxBubble_two = transform.Find("DialogueBox_Bubble_Two").GetComponent<DialogueBoxBubbleComponent>();
        role_nv1 = _twoScene.Find("role_nvzhu1").GetComponent<Image>();
        role_nv2 = _twoScene.Find("role_nvzhu2").GetComponent<Image>();
        role_nan1 = _twoScene.Find("role_nanzhu").GetComponent<Image>();
        role_nan2 = _twoScene.Find("role_nanzhu2").GetComponent<Image>();
        nvface = role_nv2.transform.Find("role_nvface").GetComponent<Image>();
        nanface = role_nan2.transform.Find("role_nanface").GetComponent<Image>();
        door1 = _twoScene.Find("door1").GetComponent<Image>();
        door2 = _twoScene.Find("door2").GetComponent<Image>();
        jiantou = _twoScene.Find("jiantou").GetComponent<Image>();
        lianHongImage = transform.Find("lianHongQ").GetComponent<Image>();
        Triangle = transform.Find("Triangle").GetComponent<Image>();

        _threeScene = transform.Find("Three") as RectTransform;
        hotPotController = _threeScene.Find("HotPot").GetComponent<HotPotController>();
        dialogueBoxBubble_three = transform.Find("DialogueBox_Bubble_Three").GetComponent<DialogueBoxBubbleComponent>();

        _fourScene = transform.Find("Four") as RectTransform;
        dialogueBoxBubble_four = transform.Find("DialogueBox_Bubble_Four").GetComponent<DialogueBoxBubbleComponent>();
        nanzhuFace = _fourScene.Find("nanzhuFace").GetComponent<Image>();

        _fiveScene = transform.Find("Five") as RectTransform;
        Caricature_five = _fiveScene.transform.Find("CaricaturePlayer").GetComponent<CaricaturePlayerController>();

        _sixScene = transform.Find("Six") as RectTransform;
        dialogueBoxBubble_six = transform.Find("DialogueBox_Bubble_Six").GetComponent<DialogueBoxBubbleComponent>();
        image1 = _sixScene.Find("Image1").GetComponent<Image>();
        image2 = _sixScene.Find("Image2").GetComponent<Image>();
        image3 = _sixScene.Find("Image3").GetComponent<Image>();
        
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
            case 3://出电梯
                DOAnima();
                break;
            case 4://火锅游戏
                ShowThreeScene();
                break;
            case 5:
                ShowFourScene();
                break;
            case 6:
                ShowFiveScene();
                break;
            case 7:
                ShowSixScene();
                break;
        }
    }

    void ShowOneScene()
    {
        Caricature.Initial(() =>
        {
            ClickBtn();
        }, CaricatureChanged);
        //Caricature.ClickBtn();
    }
    //对步骤特殊处理
    void CaricatureChanged(int index)
    {

    }

    async void ShowTwoScene()
    {
        await ChapterTool.FadeInFadeOut(_oneScene.GetComponent<CanvasGroup>(), 0, 0.1f, null, () =>
            {
                _oneScene.GetComponent<CanvasGroup>().blocksRaycasts = false;
            dialogueBoxBubble_two.Initial(() =>
            {
                dialogueBoxBubble_two.Close();
                ClickBtn();
            }, BeforeTwoDialogue, AfterTwoDialogue, null, BoxCloseBefore);
                dialogueBoxBubble_two.Show();
            });

    }

    async void AutoDialogue()
    {
        await UniTask.Delay(1000);//等待1秒自动说话
        await dialogueBoxBubble_two.ClickBtn();
        await UniTask.Delay(2000);
        await dialogueBoxBubble_two.ClickBtn();
        dialogueBoxBubble_two.MomentCloseOrOpen(false);
        dialogueBoxBubble_two.OpenClickBtn(true);
    }

    void BoxCloseBefore(ChapterDialogueTextDefine data)
    {
        if (data.ID == 15000170)
        {//脸红Q图隐藏
            Debug.Log("隐藏Q办脸");
            lianHongImage.gameObject.SetActive(false);
        }
        if (data.ID == 15000178)//15000178
        {//女主对话框位置还原
            dialogueBoxBubble_two.SetRolePoint(new Vector3(252, -38, 0), 10000001);
            Triangle.gameObject.SetActive(false);
        }

    }

    void BeforeTwoDialogue(ChapterDialogueTextDefine data)
    {
        if (data.ID == 15000167)
        {//关闭手动点击
            dialogueBoxBubble_two.MomentCloseOrOpen(true);
            dialogueBoxBubble_two.OpenClickBtn(false);
            AutoDialogue();
        }
        if (data.ID == 15000169)
        {//脸红Q图显示
            lianHongImage.gameObject.SetActive(true);
        }
        if (data.ID == 15000170)
        {
            role_nan1.gameObject.SetActive(false);
            role_nan2.gameObject.SetActive(true);
        }
        if (data.ID == 15000171)
        {
            role_nv1.gameObject.SetActive(false);
            role_nv2.gameObject.SetActive(true);
        }
        if (data.ID == 15000177)
        {
            dialogueBoxBubble_two.SetRolePoint(new Vector3(102, 0, 0), 10000001);
            Triangle.gameObject.SetActive(true);

            nanface.gameObject.SetActive(true);
            nvface.gameObject.SetActive(true);
        }
    }
    void AfterTwoDialogue(ChapterDialogueTextDefine data)
    {

    }
    //电梯动画
    void DOAnima()
    {//583.5-732.9
        var tween = jiantou.GetComponent<DOTweenAnimation>();
        tween.DOKill();
        float y = jiantou.transform.localPosition.y;
        if (659 < y || y < 659)
        {
            float dis = y - 583.5f;
            jiantou.transform.DOLocalMoveY(583.5f, dis / 115f).OnComplete(() =>
            {
                jiantou.transform.localPosition = new Vector3(jiantou.transform.localPosition.x, 732.9f, jiantou.transform.localPosition.z);
                jiantou.transform.DOLocalMoveY(659f, 0.65f).OnComplete(() =>
                {
                    //开门
                    door1.transform.DOLocalMoveX(-619f, 1.5f);
                    door2.transform.DOLocalMoveX(659f, 1.5f);
                    ClickBtn();
                });
            });
        }
    }
    //火锅游戏
    async void ShowThreeScene()
    {
        await ChapterTool.FadeInFadeOut(_twoScene.GetComponent<CanvasGroup>(), 0, 0.03f, null, () =>
            {
                _twoScene.GetComponent<CanvasGroup>().blocksRaycasts = false;
                hotPotController.Initial(() =>
                {
                    ClickBtn();
                }, () => //结束回调
                {
                    //打开对话
                    dialogueBoxBubble_three.Initial(() =>
                    {
                        dialogueBoxBubble_three.Close();
                        hotPotController.OpenAffirmBtn(true);
                    });
                    dialogueBoxBubble_three.Show();
                });
            });
    }

    async void ShowFourScene()
    {
        ChapterTool.FadeInFadeOut(_threeScene.GetComponent<CanvasGroup>(), 0, 0.1f, null, () =>
            {
                _threeScene.GetComponent<CanvasGroup>().blocksRaycasts = false;
            });

        await UniTask.Delay(100);
        walk_5 = GameObject.Instantiate(walk_5_parfab);
        dialogueBoxBubble_four.Initial(() =>
        {
            dialogueBoxBubble_four.Close();
            ClickBtn();
        }, BeforeFourDialogue);

        await UniTask.Delay(5500);//等待
        dialogueBoxBubble_four.Show();
    }

    //对步骤特殊处理
    void BeforeFourDialogue(ChapterDialogueTextDefine data)
    {
        if (data.ID == 15000188)
        {
            nanzhuFace.gameObject.SetActive(true);
        }
    }

    void ShowFiveScene()
    {
        //ChapterTool.FadeInFadeOut(nanzhuFace.GetComponent<CanvasGroup>(), 0, 0.1f);
        //ChapterTool.FadeInFadeOut(_fourScene.GetComponent<CanvasGroup>(), 0, 0.1f, null, () =>
        //    {
        //        _fourScene.GetComponent<CanvasGroup>().blocksRaycasts = false;
        //        GameObject.Destroy(walk_5.gameObject);
        //    });
        //直接切
        nanzhuFace.gameObject.SetActive(false);
        _fourScene.gameObject.SetActive(false);
        GameObject.Destroy(walk_5.gameObject);
        Caricature_five.Initial(() =>
        {
            ClickBtn();
        }, CaricatureChanged_Five);
    }
    //对步骤特殊处理
    void CaricatureChanged_Five(int index)
    {

    }

    async void ShowSixScene()
    {
        await ChapterTool.FadeInFadeOut(_fiveScene.GetComponent<CanvasGroup>(), 0, 0.1f, null, () =>
            {
                _fiveScene.GetComponent<CanvasGroup>().blocksRaycasts = false;
                PlayTexture();

                //dialogueBoxBubble_six.Initial(() =>
                //{
                //    dialogueBoxBubble_six.Close();
                //    NextStep();
                //});
                //dialogueBoxBubble_six.Show();
            });
    }

    async void PlayTexture()
    {
        await ChapterTool.FadeInFadeOut(image1.GetComponent<CanvasGroup>(), 0, 0.01f, null, async () =>
        {
            await ChapterTool.FadeInFadeOut(image2.GetComponent<CanvasGroup>(), 0, 0.01f, null, () =>
            {
                dialogueBoxBubble_six.Initial(() =>
                {
                    dialogueBoxBubble_six.Close();
                    NextStep();
                });
                dialogueBoxBubble_six.Show();
            });
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
