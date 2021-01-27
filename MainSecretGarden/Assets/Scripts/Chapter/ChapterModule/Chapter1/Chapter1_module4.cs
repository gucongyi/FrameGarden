using Company.Cfg;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Game.Protocal;
using System;

public class Chapter1_module4 : ChapterControllerBasics
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

    DialogueBoxBubbleComponent dialogueBox_two;
    DialogueBoxBubbleComponent dialogueBox_three;
    DialogueBoxBubbleComponent dialogueBox_four;
    DialogueBoxBubbleComponent dialogueBox_five;
    DialogueBoxBubbleComponent dialogueBox_six;
    //two
    GameObject twoSceneimage_nv1;
    GameObject twoSceneimage_nv2;
    GameObject twoSceneimage_nv3;
    GameObject twoSceneimage_nv4;
    GameObject twoSceneimage_nv5;
    GameObject twoSceneimage_nv6;
    GameObject twoSceneimage_nv7;
    GameObject twoSceneimage_nv8;
    GameObject twoSceneimage_nv9;
    GameObject twoSceneimage_nv10;
    GameObject twoSceneimage_nan1;
    //three
    GameObject threeImage_1;
    GameObject threeImage_2;
    GameObject threeImage_3;

    //four
    float duration = 3;//动画持续时间
    float strength = 20;//强度
    int vibrato = 20;//颤度
    //表情
    Image nv1face;
    [SerializeField] Sprite nv1head_putong;
    [SerializeField] Sprite nv1head_haixiu;
    [SerializeField] Sprite nv1head_jingya;
    [SerializeField] Sprite nv1head_weixiao;
    [SerializeField] Sprite nv1head_wunai;

    Image nan1face;
    [SerializeField] Sprite nan1head_putong;
    [SerializeField] Sprite nan1head_weixiao;

    Image nv2face;
    [SerializeField] Sprite nv2head_putong;
    [SerializeField] Sprite nv2head_haixiu;
    [SerializeField] Sprite nv2head_jingya;
    [SerializeField] Sprite nv2head_weixiao;
    [SerializeField] Sprite nv2head_wunai;
    //骰子
    RectTransform nv1dice;
    Image nv1point1;//点数
    Image nv1point2;//点数
    Image nv1point3;//点数
    Image nv1lid;//盖子

    RectTransform nan1dice;
    Image nan1point1;//点数
    Image nan1point2;//点数
    Image nan1point3;//点数
    Image nan1lid;//盖子

    RectTransform nv2dice;
    Image nv2point1;//点数
    Image nv2point2;//点数
    Image nv2point3;//点数
    Image nv2lid;//盖子
    [SerializeField] Sprite point1;
    [SerializeField] Sprite point2;
    [SerializeField] Sprite point3;
    [SerializeField] Sprite point4;
    [SerializeField] Sprite point5;
    [SerializeField] Sprite point6;

    Button startGame;//开始游戏按钮
    Text startText;

    Text jiesuanText;//结算文本

    Button nextChapterBtn;
    Text nextText;
    [SerializeField]
    GameObject dance_live2D;//跳舞live2D
    GameObject dance_live2D_instance;//实例(后续删除)
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
        _twoScene = transform.Find("Two") as RectTransform;
        _threeScene = transform.Find("Three") as RectTransform;
        _fourScene = transform.Find("Four") as RectTransform;
        dialogueBox_two = transform.Find("DialogueBox_Bubble_two").GetComponent<DialogueBoxBubbleComponent>();
        dialogueBox_three = transform.Find("DialogueBox_Bubble_three").GetComponent<DialogueBoxBubbleComponent>();
        dialogueBox_four = transform.Find("DialogueBox_Bubble_four").GetComponent<DialogueBoxBubbleComponent>();
        dialogueBox_five = transform.Find("DialogueBox_Bubble_five").GetComponent<DialogueBoxBubbleComponent>();
        dialogueBox_six = transform.Find("DialogueBox_Bubble_six").GetComponent<DialogueBoxBubbleComponent>();
        #region image引用
        //场景2表情切换
        twoSceneimage_nv1 = _twoScene.Find("Two_image_1").gameObject;
        twoSceneimage_nv2 = _twoScene.Find("Two_image_2").gameObject;
        twoSceneimage_nv3 = _twoScene.Find("Two_image_3").gameObject;
        twoSceneimage_nv4 = _twoScene.Find("Two_image_4").gameObject;
        twoSceneimage_nv5 = _twoScene.Find("Two_image_5").gameObject;
        twoSceneimage_nv6 = _twoScene.Find("Two_image_6").gameObject;
        twoSceneimage_nv7 = _twoScene.Find("Two_image_7").gameObject;
        twoSceneimage_nv8 = _twoScene.Find("Two_image_8").gameObject;
        twoSceneimage_nv9 = _twoScene.Find("Two_image_9").gameObject;
        twoSceneimage_nv10 = _twoScene.Find("Two_image_10").gameObject;
        twoSceneimage_nan1 = _twoScene.Find("Two_image_nan1").gameObject;

        threeImage_1 = _threeScene.Find("Image_1").gameObject;
        threeImage_2 = _threeScene.Find("Image_2").gameObject;
        threeImage_3 = _threeScene.Find("Image_3").gameObject;

        nv1face = _fourScene.Find("Top/nv1head").GetComponent<Image>();
        nan1face = _fourScene.Find("Top/nan1head").GetComponent<Image>();
        nv2face = _fourScene.Find("Top/nv2head").GetComponent<Image>();

        nv1dice = _fourScene.Find("Top/nv1dice") as RectTransform;
        nv1point1 = nv1dice.Find("point1").GetComponent<Image>();//点数
        nv1point2 = nv1dice.Find("point2").GetComponent<Image>();//点数
        nv1point3 = nv1dice.Find("point3").GetComponent<Image>();//点数
        nv1lid = nv1dice.Find("lid").GetComponent<Image>();//点数

        nan1dice = _fourScene.Find("Top/nan1dice") as RectTransform;
        nan1point1 = nan1dice.Find("point1").GetComponent<Image>();//点数
        nan1point2 = nan1dice.Find("point2").GetComponent<Image>();//点数
        nan1point3 = nan1dice.Find("point3").GetComponent<Image>();//点数
        nan1lid = nan1dice.Find("lid").GetComponent<Image>();//点数

        nv2dice = _fourScene.Find("Top/nv2dice") as RectTransform;
        nv2point1 = nv2dice.Find("point1").GetComponent<Image>();//点数
        nv2point2 = nv2dice.Find("point2").GetComponent<Image>();//点数
        nv2point3 = nv2dice.Find("point3").GetComponent<Image>();//点数
        nv2lid = nv2dice.Find("lid").GetComponent<Image>();//点数

        #endregion
        startGame = _fourScene.Find("Top/startBtn").GetComponent<Button>();
        startText = startGame.transform.Find("Text").GetComponent<Text>();
        jiesuanText = _fourScene.Find("Box/Text").GetComponent<Text>();
        nextChapterBtn = _fourScene.Find("Top/nextBtn").GetComponent<Button>();
        nextText = nextChapterBtn.transform.Find("Text").GetComponent<Text>();
        OpenClickBtn(false);
        ShowOne();
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
        if (index > 3)
        {
            Debug.LogError("跳出");
            return;
        }
        switch (index)
        {
            case 1:
                ShowThree();
                break;
            case 2:
                ShowFour();
                break;
        }
    }
    #region 123
    async void ShowOne()
    {
        if (dance_live2D != null)
            dance_live2D_instance = GameObject.Instantiate(dance_live2D);
        else
            Debug.LogError("没有跳舞live2D");
        await UniTask.Delay(5000);
        //await UniTask.Delay(300);
        ShowTwo();
    }

    async void ShowTwo()
    {
        await ChapterTool.FadeInFadeOut(_oneScene.GetComponent<CanvasGroup>(), 0, 0.3f, null, () =>
        {
            if (dance_live2D_instance != null)
                GameObject.Destroy(dance_live2D_instance);
            else
                Debug.LogError("没有预制");
            dialogueBox_two.Initial(() =>
            {
                dialogueBox_two.Close();
                ClickBtn();
                Debug.Log("对话完毕");
            }, TwoSpeakBeforeAciton, TwoSpeakAfterAction);
            dialogueBox_two.Show();
        });
    }

    void TwoSpeakBeforeAciton(ChapterDialogueTextDefine data)
    {
        if (data.ID == 12000114)//糟了
            twoSceneimage_nv2.gameObject.SetActive(true);
        //whatHappen
        if (data.ID == 12000115)
            twoSceneimage_nv3.gameObject.SetActive(true);
        //e 你抬头看看
        if (data.ID == 12000119)
            twoSceneimage_nv4.gameObject.SetActive(true);
        //还是担心你
        if (data.ID == 12000122)
            twoSceneimage_nan1.gameObject.SetActive(true);
        //该不会一直没走把
        if (data.ID == 12000123)
        {
            twoSceneimage_nan1.gameObject.SetActive(false);
            twoSceneimage_nv6.gameObject.SetActive(true);
        }
        //咖啡馆看书
        if (data.ID == 12000124)
        {//关闭按钮切换图片
            twoSceneimage_nan1.gameObject.SetActive(true);
        }

        //在这鱼龙混杂
        if (data.ID == 12000125)
            twoSceneimage_nan1.gameObject.SetActive(true);
        //我算被夸奖了吗
        if (data.ID == 12000126)
        {
            twoSceneimage_nan1.gameObject.SetActive(false);
            twoSceneimage_nv8.gameObject.SetActive(true);
        }
        if (data.ID == 12000127)
            twoSceneimage_nan1.gameObject.SetActive(true);
        //感觉自己多余
        if (data.ID == 12000128)
        {
            twoSceneimage_nan1.gameObject.SetActive(false);
            twoSceneimage_nv9.gameObject.SetActive(true);
        }
        //一起玩
        if (data.ID == 12000129)
        {
            twoSceneimage_nv10.gameObject.SetActive(true);
        }

    }

    async void TwoSpeakAfterAction(ChapterDialogueTextDefine data)
    {
        //抬头看
        if (data.ID == 12000121)
        {
            await UniTask.Delay(800);
            twoSceneimage_nv5.gameObject.SetActive(true);
        }
        //显示葱点赞
        if (data.ID == 12000124)
        {
            dialogueBox_two.GetComponent<Image>().raycastTarget = false;
            dialogueBox_two.OpenClickBtn(false);
            await UniTask.Delay(800);
            await dialogueBox_two.CompelCloseShowText(10000002);//关闭男主的对话
            twoSceneimage_nan1.gameObject.SetActive(false);
            twoSceneimage_nv7.gameObject.SetActive(true);
            await UniTask.Delay(800);
            dialogueBox_two.GetComponent<Image>().raycastTarget = true;
            dialogueBox_two.OpenClickBtn(true);
        }
    }
    //显示场景3
    async void ShowThree()
    {
        await ChapterTool.FadeInFadeOut(_twoScene.GetComponent<CanvasGroup>(), 0, 1f, null, () =>
        {
            dialogueBox_three.Initial(() =>
            {
                dialogueBox_three.Close();
                ClickBtn();
                Debug.Log("对话完毕");
            }, ThreeSpeakBeforeAction, null);
            dialogueBox_three.Show();
        });
    }
    //场景3对话调用前
    void ThreeSpeakBeforeAction(ChapterDialogueTextDefine data)
    {
        if (data.ID == 12000131)
        {
            dialogueBox_three.SetRoleSpeakAeforeAwait(true);
            ThreeMoveX(threeImage_1.transform as RectTransform, -1242,
                () =>
                {
                    dialogueBox_three.SetRoleSpeakAeforeAwait(false);
                });
        }
        if (data.ID == 12000132)
        {
            dialogueBox_three.SetRoleSpeakAeforeAwait(true);
            ThreeMoveX(threeImage_2.transform as RectTransform, 1242,
                () =>
                {
                    dialogueBox_three.SetRoleSpeakAeforeAwait(false);
                });
        }
        if (data.ID == 12000133)
        {
            dialogueBox_three.SetRoleSpeakAeforeAwait(true);
            ThreeMoveX(threeImage_3.transform as RectTransform, -1242,
                () =>
                {
                    dialogueBox_three.SetRoleSpeakAeforeAwait(false);
                });
        }
    }
    //场景3移动操作
    void ThreeMoveX(RectTransform rect, float endX, Action callback)
    {
        rect.gameObject.SetActive(true);
        float lostX = rect.anchoredPosition.x;
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x + endX, rect.anchoredPosition.y);
        rect.DOAnchorPosX(lostX, 0.5f).OnComplete(() => { callback?.Invoke(); });
    }
    #endregion
    #region 4
    async void ShowFour()
    {//隐藏第三场景
        await ChapterTool.FadeInFadeOut(_threeScene.GetComponent<CanvasGroup>(), 0, 1f, null);
        _fourScene.GetComponent<CanvasGroup>().blocksRaycasts = true;
        startGame.onClick.RemoveAllListeners();
        startGame.onClick.AddListener(() =>
        {//禁止连点
            startGame.enabled = false;
            startGame.gameObject.SetActive(false);
            FirstGames();
            startGame.enabled = true;
        });
        startText.text = ChapterTool.GetChapterFunctionString(10000543); //游戏开始 10000543
    }

    /// <summary>
    /// 第一轮游戏
    /// </summary>
    async void FirstGames()
    {
        nv1dice.transform.DOShakePosition(duration, strength, vibrato);
        nv2dice.transform.DOShakePosition(duration, strength, vibrato);
        nan1dice.transform.DOShakePosition(duration, strength, vibrato);

        jiesuanText.text = ChapterHelper.ReadChapterFuncTable(10000524);

        await UniTask.Delay(3300);//等待动画播放完成
        Debug.Log("显示第一轮计分");
        nv2lid.gameObject.SetActive(false);//女2先开
        jiesuanText.text += ChapterHelper.ReadChapterFuncTable(10000525);
        await UniTask.Delay(800);
        nv1lid.gameObject.SetActive(false);
        jiesuanText.text += ChapterHelper.ReadChapterFuncTable(10000526);
        await UniTask.Delay(800);
        nan1lid.gameObject.SetActive(false);
        jiesuanText.text += ChapterHelper.ReadChapterFuncTable(10000527);

        //开始对话
        dialogueBox_four.Initial(() =>
        {//12000135开始
            dialogueBox_four.Close();
            //OpenClickBtn(true);
            SecondGames();
            Debug.Log("对话完毕");
        }, FourSpeakBeforeAction);
        dialogueBox_four.Show();
    }

    void FourSpeakBeforeAction(ChapterDialogueTextDefine data)
    {//切换表情
        if (data.ID == 12000135)//这不科学
            SwitchFace(nv2head_jingya);
        if (data.ID == 12000136)
            SwitchFace(null, nv1head_weixiao);
        if (data.ID == 12000138)
            SwitchFace(nv2head_wunai, null);
        if (data.ID == 12000139)
            SwitchFace(nv2head_haixiu, null);
        if (data.ID == 12000141)
            SwitchFace(null, nv1head_putong);
        if (data.ID == 12000143)
            SwitchFace(null, nv1head_weixiao);
        if (data.ID == 12000144)
            SwitchFace(nv2head_wunai);
        if (data.ID == 12000145)
            SwitchFace(nv2head_weixiao);
        //2轮
        if (data.ID == 12000147)
            SwitchFace(null, nv1head_wunai);
        if (data.ID == 12000148)
            SwitchFace(nv2head_putong, nv1head_haixiu);
        if (data.ID == 12000152)
            SwitchFace(nv2head_weixiao, nv1head_haixiu);
        if (data.ID == 12000153)
            SwitchFace(null, null, nan1head_weixiao);
        if (data.ID == 12000155)
        {
            SwitchFace(null, null, nan1head_weixiao);
        }
        //3轮
        if (data.ID == 12000156)
            SwitchFace(nv2head_wunai);
        if (data.ID == 12000157)
            SwitchFace(nv2head_weixiao);
        if (data.ID == 12000159)
            SwitchFace(nv2head_weixiao, nv1head_wunai);
        if (data.ID == 12000160)
            SwitchFace(nv2head_wunai);
        if (data.ID == 12000161)
            SwitchFace(null, nv1head_putong);
        if (data.ID == 12000162)
            SwitchFace(nv2head_weixiao);
        if (data.ID == 12000164)
            SwitchFace(nv2head_jingya);
        if (data.ID == 12000165)
            SwitchFace(null, nv1head_weixiao);
        if (data.ID == 12000166)
            SwitchFace(nv2head_wunai);
        if (data.ID == 12000167)
            SwitchFace(null, nv1head_weixiao);
        if (data.ID == 12000168)
            SwitchFace(nv2head_weixiao);
    }

    /// <summary>
    /// 切换表情
    /// </summary>
    void SwitchFace(Sprite nv2Face = null, Sprite nv1Face = null, Sprite nan1Face = null)
    {
        if (nv2Face != null)
            nv2face.sprite = nv2Face;
        if (nv1Face != null)
            nv1face.sprite = nv1Face;
        if (nan1Face != null)
            nan1face.sprite = nan1Face;
        else
            nan1face.sprite = nan1head_putong;
    }

    /// <summary>
    /// 第二轮游戏
    /// </summary>
    void SecondGames()
    {
        SwitchFace(nv2head_putong, nv1head_putong);

        //开启游戏按钮
        startGame.gameObject.SetActive(true);
        startGame.onClick.RemoveAllListeners();
        startGame.onClick.AddListener(() =>
        {
            startGame.enabled = false;
            startGame.gameObject.SetActive(false);
            SecondStartGame();
            startGame.enabled = true;
        });
        startText.text = ChapterTool.GetChapterFunctionString(10000544); //下一轮
    }
    /// <summary>
    /// 第三轮游戏
    /// </summary>
    void ThirdlyGames()
    {
        SwitchFace(nv2head_putong, nv1head_putong, nan1head_putong);//切换回普通表情
        //开启游戏按钮
        startGame.gameObject.SetActive(true);
        startGame.onClick.RemoveAllListeners();
        startGame.onClick.AddListener(() =>
        {
            startGame.enabled = false;
            startGame.gameObject.SetActive(false);
            ThirdlyStartGame();
            startGame.enabled = true;
        });
        startText.text = ChapterTool.GetChapterFunctionString(10000544); //下一轮
    }

    //第二次点击开始游戏
    async void SecondStartGame()
    {
        jiesuanText.text = "";
        nv1lid.gameObject.SetActive(true);
        nv2lid.gameObject.SetActive(true);
        nan1lid.gameObject.SetActive(true);
        //设置骰子精灵
        nv1point1.sprite = point1;
        nv1point2.sprite = point2;
        nv1point3.sprite = point4;

        nv2point1.sprite = point4;
        nv2point2.sprite = point3;
        nv2point3.sprite = point6;

        nv1dice.transform.DOShakePosition(duration, strength, vibrato);
        nv2dice.transform.DOShakePosition(duration, strength, vibrato);
        nan1dice.transform.DOShakePosition(duration, strength, vibrato);

        jiesuanText.text = ChapterHelper.ReadChapterFuncTable(10000528);

        await UniTask.Delay(3300);//等待动画播放完成
        Debug.Log("显示第二轮计分");
        nv2lid.gameObject.SetActive(false);//女2先开
        jiesuanText.text += ChapterHelper.ReadChapterFuncTable(10000529);
        await UniTask.Delay(800);
        nv1lid.gameObject.SetActive(false);
        jiesuanText.text += ChapterHelper.ReadChapterFuncTable(10000530);
        await UniTask.Delay(800);
        nan1lid.gameObject.SetActive(false);
        jiesuanText.text += ChapterHelper.ReadChapterFuncTable(10000531);

        //重新开始对话
        //dialogueBox_four.SetStartDialogueId(12000146);
        dialogueBox_five.Initial(() =>
        {
            dialogueBox_five.Close();
            ThirdlyGames();

            Debug.Log("对话完毕");
        }, FourSpeakBeforeAction);
        dialogueBox_five.Show();
    }

    //第三次点击开始游戏
    async void ThirdlyStartGame()
    {
        jiesuanText.text = "";
        nv1lid.gameObject.SetActive(true);
        nv2lid.gameObject.SetActive(true);
        nan1lid.gameObject.SetActive(true);
        //设置骰子精灵
        nv1point1.sprite = point5;
        nv1point2.sprite = point4;
        nv1point3.sprite = point2;

        nv2point1.sprite = point2;
        nv2point2.sprite = point3;
        nv2point3.sprite = point5;

        nan1point1.sprite = point1;
        nan1point2.sprite = point1;
        nan1point3.sprite = point1;

        nv1dice.transform.DOShakePosition(duration, strength, vibrato);
        nv2dice.transform.DOShakePosition(duration, strength, vibrato);
        nan1dice.transform.DOShakePosition(duration, strength, vibrato);

        jiesuanText.text = ChapterHelper.ReadChapterFuncTable(10000532);
        await UniTask.Delay(3300);//等待动画播放完成
        Debug.Log("显示第三轮计分");

        nv2lid.gameObject.SetActive(false);//女2先开
        jiesuanText.text += ChapterHelper.ReadChapterFuncTable(10000533);
        await UniTask.Delay(800);
        nv1lid.gameObject.SetActive(false);
        jiesuanText.text += ChapterHelper.ReadChapterFuncTable(10000534);
        await UniTask.Delay(800);
        nan1lid.gameObject.SetActive(false);
        jiesuanText.text += ChapterHelper.ReadChapterFuncTable(10000535);

        //重新开始对话
        //dialogueBox_four.SetStartDialogueId(12000156);
        dialogueBox_six.Initial(() =>
        {
            dialogueBox_six.Close();
            //显示下一章按钮
            ReadOverThisChapter();

            Debug.Log("对话完毕");
        }, FourSpeakBeforeAction);
        dialogueBox_six.Show();
    }
    #endregion
    void ReadOverThisChapter()
    {
        nextText.text = ChapterTool.GetChapterFunctionString(10000000);//下一章
        nextChapterBtn.gameObject.SetActive(true);
        ChapterModuleManager._Instance.ClickEndChapterBtn(nextChapterBtn, NextStep);
        //请求保存
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