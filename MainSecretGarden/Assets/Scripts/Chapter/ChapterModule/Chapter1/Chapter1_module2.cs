using Company.Cfg;
using Cysharp.Threading.Tasks;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
/// <summary>
/// 第0章第一部分
/// </summary>
public class Chapter1_module2 : ChapterControllerBasics
{
    #region 字段
    /// <summary>
    /// 场景1
    /// </summary>
    RectTransform _oneIamgeRect;
    /// <summary>
    /// 场景1画布控制
    /// </summary>
    CanvasGroup _oneIamgeCanvasGroup;
    /// <summary>
    /// 场景2
    /// </summary>
    RectTransform _twoImageRect;
    /// <summary>
    /// 场景2画布控制
    /// </summary>
    CanvasGroup _twoImagCanvasGroup;
    /// <summary>
    /// 下一步按钮
    /// </summary>
    Button _clickBtn;
    /// <summary>
    /// 背景画布集合
    /// </summary>
    [SerializeField]
    List<CanvasGroup> _bgs = new List<CanvasGroup>();

    RectTransform _therrBoxRect;
    /// <summary>
    /// 对话1
    /// </summary>
    DialogueBoxBubbleComponent _dialogueBase;
    DialogueBoxTetragonumComponent _dialogueBoxTetragonumComponent;
    /// <summary>
    /// 对话2
    /// </summary>
    DialogueBoxBubbleComponent _dialogueBaseTwo;
    /// <summary>
    /// 对话3
    /// </summary>
    DialogueBoxBubbleComponent _dialogueBaseTherr;
    /// <summary>
    /// 对话4
    /// </summary>
    DialogueBoxBubbleComponent _dialogueBaseFour;
    /// <summary>
    /// 对话4
    /// </summary>
    DialogueBoxBubbleComponent _dialogueBaseFive;
    RectTransform _fourRect;
    CanvasGroup _fourCanvasGroup;
    RectTransform _cupRect;
    CanvasGroup _cupCanvasGroup;
    CanvasGroup _bgtOneCanvasGroup;
    /// <summary>
    /// 按钮组件
    /// </summary>
    UIPanelDrag _cupRectDrag;
    RectTransform _bgTwoRect;
    CanvasGroup _bgTwoCanvasGroup;
    RectTransform _phoneRect;
    CanvasGroup _phoneCanvasGroup;
    RectTransform _answerThePhoneRect;
    CanvasGroup _answerThePhoneCanvasGroup;
    Button _phoneBtn;
    Button _oneBtn;
    Button _twoBtn;

    RectTransform _fiveRect;

    RectTransform _fiveBtnBoxRect;
    Button _fiveBtnOne;
    Text _fiveBtnOneText;
    Button _fiveBtnTwo;
    Text _fiveBtnTwoText;
    Button _fiveBtnTherr;
    Text _fiveBtnTherrText;

    //表情变化
    Image nanface1_image;//男闭嘴
    Image nanface2_image;//男说话
    Image nvface1_image;//女张望
    Image nvface2_image;//女说话
    Image nvface3_image;//女闭嘴
    Image nvface4_image;//女害羞

    //phone
    RectTransform nv2_Rect;
    Image nv1Face1_phone;
    Image nv1Face2_phone;
    Image nv1Face3_phone;
    Image nv2Face1_phone;
    Image nv2Face2_phone;
    Image nv2Face3_phone;

    //endScene
    Image nanface1_image_endScene;//男闭嘴
    Image nanface2_image_endScene;//男说话
    Image nvface1_image_endScene;//女闭嘴
    Image nvface2_image_endScene;//女说话

    /// <summary>
    /// 引导
    /// </summary>
    ChapterGuidance _chapterGuidance;
    /// <summary>
    /// 点击计数
    /// </summary>
    int _clickIndex = 0;
    #endregion
    #region 函数

    // Start is called before the first frame update
    void Start()
    {
        //Initial();
    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// 初始化组件
    /// </summary>
    public override void Initial()
    {
        _oneIamgeRect = transform.Find("One").GetComponent<RectTransform>();
        _oneIamgeCanvasGroup = _oneIamgeRect.GetComponent<CanvasGroup>();
        _oneIamgeRect.gameObject.SetActive(true);
        _twoImageRect = transform.Find("Two").GetComponent<RectTransform>();
        _twoImagCanvasGroup = _twoImageRect.GetComponent<CanvasGroup>();
        _therrBoxRect = transform.Find("TherBox").GetComponent<RectTransform>();
        _fourRect = transform.Find("Four").GetComponent<RectTransform>();
        _fourCanvasGroup = _fourRect.GetComponent<CanvasGroup>();
        _cupRect = _fourRect.Find("Cup").GetComponent<RectTransform>();
        _cupCanvasGroup = _cupRect.GetComponent<CanvasGroup>();
        _bgtOneCanvasGroup = _fourRect.Find("BgOne").GetComponent<CanvasGroup>();
        _cupRectDrag = _cupRect.GetComponent<UIPanelDrag>();
        _cupRectDrag.m_DragPlane = _fourRect;
        _cupRectDrag.actionOnPointerUp = PointerUp;
        _cupRectDrag.actionOnPointerDown = PointerDown;

        _answerThePhoneRect = transform.Find("AnswerThePhone").GetComponent<RectTransform>();
        _answerThePhoneCanvasGroup = _answerThePhoneRect.GetComponent<CanvasGroup>();

        _phoneRect = _fourRect.Find("Phone").GetComponent<RectTransform>();
        _phoneCanvasGroup = _phoneRect.GetComponent<CanvasGroup>();
        _bgTwoRect = _fourRect.Find("BgTwo").GetComponent<RectTransform>();
        _bgTwoCanvasGroup = _bgTwoRect.GetComponent<CanvasGroup>();
        _clickBtn = GetComponent<Button>();
        _clickBtn.onClick.RemoveListener(ClickBtn);
        _clickBtn.onClick.AddListener(ClickBtn);

        _phoneBtn = _bgTwoRect.Find("PhoneBtn").GetComponent<Button>();
        _oneBtn = _phoneRect.Find("BtnOne").GetComponent<Button>();
        _twoBtn = _phoneRect.Find("BtnTwo").GetComponent<Button>();

        _fiveRect = transform.Find("Five").GetComponent<RectTransform>();
        _fiveBtnBoxRect = transform.Find("FiveBtnBox").GetComponent<RectTransform>();
        _fiveBtnOne = _fiveBtnBoxRect.Find("One").GetComponent<Button>();
        _fiveBtnOneText = _fiveBtnOne.transform.Find("Text").GetComponent<Text>();
        _fiveBtnTwo = _fiveBtnBoxRect.Find("Two").GetComponent<Button>();
        _fiveBtnTwoText = _fiveBtnTwo.transform.Find("Text").GetComponent<Text>();
        _fiveBtnTherr = _fiveBtnBoxRect.Find("Therr").GetComponent<Button>();
        _fiveBtnTherrText = _fiveBtnTherr.transform.Find("Text").GetComponent<Text>();
        _oneBtn.onClick.RemoveAllListeners();
        _oneBtn.onClick.AddListener(ClickOneBtn);
        _twoBtn.onClick.RemoveAllListeners();
        _twoBtn.onClick.AddListener(ClickTwoBtn);


        _fiveBtnOneText.text = ChapterTool.GetDialogueString(12000063);
        _fiveBtnTwoText.text = ChapterTool.GetDialogueString(12000064);
        _fiveBtnTherrText.text = ChapterTool.GetChapterFunctionString(10000000);
        _fiveBtnOne.onClick.RemoveAllListeners();
        _fiveBtnOne.onClick.AddListener(ClickFiveBtnOne);
        _fiveBtnTwo.onClick.RemoveAllListeners();
        _fiveBtnTwo.onClick.AddListener(ClickFiveBtnTwo);
        _phoneBtn.onClick.RemoveAllListeners();
        _phoneBtn.onClick.AddListener(ClickBtn);
        ChapterModuleManager._Instance.ClickEndChapterBtn(_fiveBtnTherr, NextStep);
        //_fiveBtnTherr.onClick.RemoveAllListeners();
        //_fiveBtnTherr.onClick.AddListener(ClickFiveBtnTherr);

        _dialogueBase = transform.Find("DialogueBox_Bubble").GetComponent<DialogueBoxBubbleComponent>();
        _dialogueBoxTetragonumComponent = transform.Find("DialogueBox_Tetragonum").GetComponent<DialogueBoxTetragonumComponent>();
        _dialogueBaseTwo = transform.Find("DialogueBox_BubbleTwo").GetComponent<DialogueBoxBubbleComponent>();
        _dialogueBaseTherr = transform.Find("DialogueBox_BubbleTherr").GetComponent<DialogueBoxBubbleComponent>();
        _dialogueBaseFour = transform.Find("DialogueBox_BubbleFour").GetComponent<DialogueBoxBubbleComponent>();
        _dialogueBaseFive = transform.Find("DialogueBox_BubbleFive").GetComponent<DialogueBoxBubbleComponent>();
        _chapterGuidance = _fourRect.Find("ChapterGuidance").GetComponent<ChapterGuidance>();

        //表情
        nanface1_image = transform.Find("Two/nanface1_image").GetComponent<Image>();
        nanface2_image = transform.Find("Two/nanface2_image").GetComponent<Image>();
        nvface1_image = transform.Find("Two/nvface1_image").GetComponent<Image>();
        nvface2_image = transform.Find("Two/nvface2_image").GetComponent<Image>();
        nvface3_image = transform.Find("Two/nvface3_image").GetComponent<Image>();
        nvface4_image = transform.Find("Two/nvface4_image").GetComponent<Image>();

        //phone表情
        nv2_Rect = _answerThePhoneRect.Find("nv2") as RectTransform;
        nv1Face1_phone = _answerThePhoneRect.Find("nv1/nv1Face1").GetComponent<Image>();
        nv1Face2_phone = _answerThePhoneRect.Find("nv1/nv1Face2").GetComponent<Image>();
        nv1Face3_phone = _answerThePhoneRect.Find("nv1/nv1Face3").GetComponent<Image>();
        nv2Face1_phone = nv2_Rect.Find("nv2Face1").GetComponent<Image>();
        nv2Face2_phone = nv2_Rect.Find("nv2Face2").GetComponent<Image>();
        nv2Face3_phone = nv2_Rect.Find("nv2Face3").GetComponent<Image>();

        //endScene表情
        nanface1_image_endScene = _fiveRect.Find("nanface1_image").GetComponent<Image>();
        nanface2_image_endScene = _fiveRect.Find("nanface2_image").GetComponent<Image>();
        nvface1_image_endScene = _fiveRect.Find("nvface1_image").GetComponent<Image>();
        nvface2_image_endScene = _fiveRect.Find("nvface2_image").GetComponent<Image>();

        ClickBtn();
        base.Initial();
    }

    private void ClickFiveBtnTwo()
    {
        _fiveBtnOne.transform.gameObject.SetActive(false);
        _fiveBtnTwo.transform.gameObject.SetActive(false);
        _dialogueBaseFive.SetStartDialogueId(12000064);
        _dialogueBaseFive.Initial(() =>
        {
            _dialogueBaseFive.Close();
            //下一章显示请求保存
            ChapterModuleManager._Instance.RequestPassChapter();
            _fiveBtnTherr.gameObject.SetActive(true);
            Debug.Log("对话4完毕");
        }, EndSceneSpeakBeforeAction);
        _dialogueBaseFive.Show();
    }

    private void ClickFiveBtnOne()
    {
        _fiveBtnOne.transform.gameObject.SetActive(false);
        _fiveBtnTwo.transform.gameObject.SetActive(false);
        //增加好感
        ImpulseHelper.OptionAddFavorable(12000063);
        _dialogueBaseFive.SetStartDialogueId(12000063);
        _dialogueBaseFive.Initial(() =>
        {
            _dialogueBaseFive.Close();
            //下一章显示请求保存
            ChapterModuleManager._Instance.RequestPassChapter();
            _fiveBtnTherr.gameObject.SetActive(true);
            Debug.Log("对话4完毕");
        }, EndSceneSpeakBeforeAction);
        _dialogueBaseFive.Show();
    }
    /// <summary>
    /// 接电话
    /// </summary>
    private void ClickTwoBtn()
    {
        _twoBtn.enabled = false;
        _phoneCanvasGroup.DOFade(0, 1f).OnComplete(() =>
        {
            _phoneRect.gameObject.SetActive(false);
            _dialogueBaseTherr.Initial(() =>
            {
                _dialogueBaseTherr.Close();
                Debug.Log("对话3完毕");
                //OpenClickBtn(true);
                ClickBtn();
            }, PhoneBeforeAction);
            _dialogueBaseTherr.Show();
        });

        //await ChapterTool.FadeInFadeOut(_phoneCanvasGroup, 0, fadeOut, new CancellationTokenSource(), () =>
        //{
        //    _phoneRect.gameObject.SetActive(false);
        //    _dialogueBaseTherr.Initial(() =>
        //    {
        //        _dialogueBaseTherr.Close();
        //        Debug.Log("对话3完毕");
        //        //OpenClickBtn(true);
        //        ClickBtn();
        //    }, PhoneBeforeAction);
        //    _dialogueBaseTherr.Show();
        //});
    }

    void PhoneBeforeAction(ChapterDialogueTextDefine data)
    {
        if (data.ID == 12000048)
        {
            //Debug.LogError("暂时关闭对话");

            //暂时关闭对话点击
            //_dialogueBaseTwo.MomentCloseOrOpen(true);
            //_dialogueBaseTwo.OpenClickBtn(false);
            //OpenClickBtn(true);
        }
        if (data.ID == 12000049)
        {
            _dialogueBaseTherr.SetRoleSpeakAeforeAwait(true);
            nv1Face1_phone.gameObject.SetActive(true);
            nv2_Rect.DOAnchorPosY(0, 0.5f).OnComplete(() =>//女二出现
             {
                 nv2Face1_phone.gameObject.SetActive(false);
                 _dialogueBaseTherr.SetRoleSpeakAeforeAwait(false);
             });
        }
        if (data.ID == 12000051)
        {
            nv1Face1_phone.gameObject.SetActive(false);
            nv1Face2_phone.gameObject.SetActive(true);//眯眼说话
            nv2Face1_phone.gameObject.SetActive(true);
        }
        if (data.ID == 12000052)
        {
            nv1Face2_phone.gameObject.SetActive(false);
            nv1Face3_phone.gameObject.SetActive(true);

            nv2Face1_phone.gameObject.SetActive(false);
            nv2Face2_phone.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 拒接 聊天
    /// </summary>
    private void ClickOneBtn()
    {
        _oneBtn.enabled = false;
        ChapterTool.OpenDialogueBoxWeChat(12000043, 10000001, 10000003, () => { Debug.Log("微信聊天完毕！"); OprnFourTwo(); });

    }

    public void OprnFour()
    {
        _answerThePhoneCanvasGroup.DOFade(0, 1f).OnComplete(() =>
         {
             Debug.Log("开启第四个对话");
             _dialogueBaseFour.Initial(() =>
             {
                 _dialogueBaseFour.Close();
                 //OpenClickBtn(true);
                 ClickBtn();
                 Debug.Log("对话4完毕");
             }, EndSceneSpeakBeforeAction);
             _dialogueBaseFour.Show();
         });
        //await ChapterTool.FadeInFadeOut(_answerThePhoneCanvasGroup, 0, fadeOut, new CancellationTokenSource());
        //Debug.Log("开启第四个对话");
        //_dialogueBaseFour.Initial(() =>
        //{
        //    _dialogueBaseFour.Close();
        //    //OpenClickBtn(true);
        //    ClickBtn();
        //    Debug.Log("对话4完毕");
        //}, EndSceneSpeakBeforeAction);
        //_dialogueBaseFour.Show();
    }
    public void OprnFourTwo()
    {
        _answerThePhoneRect.gameObject.SetActive(false);
        _fourRect.gameObject.SetActive(false);
        Debug.Log("开启第四个对话");
        _dialogueBaseFour.Initial(() =>
        {
            _dialogueBaseFour.Close();
            //OpenClickBtn(true);
            ClickBtn();
            Debug.Log("对话4完毕");
        }, EndSceneSpeakBeforeAction);
        _dialogueBaseFour.Show();
    }

    void EndSceneSpeakBeforeAction(ChapterDialogueTextDefine data)
    {
        if (data.ID == 12000056 || data.ID == 12000058 || data.ID == 12000060 || data.ID == 12000062 || data.ID == 12000065 || data.ID == 12000067)
        {//男说话 女闭嘴
            nanface1_image_endScene.gameObject.SetActive(false);
            nanface2_image_endScene.gameObject.SetActive(true);

            nvface1_image_endScene.gameObject.SetActive(true);
            nvface2_image_endScene.gameObject.SetActive(false);
        }
        if (data.ID == 12000055 || data.ID == 12000057 || data.ID == 12000059 || data.ID == 12000061 || data.ID == 12000063 || data.ID == 12000064 || data.ID == 12000066 || data.ID == 12000069)
        {//女说话
            nvface1_image_endScene.gameObject.SetActive(false);
            nvface2_image_endScene.gameObject.SetActive(true);

            nanface1_image_endScene.gameObject.SetActive(true);
            nanface2_image_endScene.gameObject.SetActive(false);
        }
    }


    private void PointerUp(PointerEventData obj)
    {

        Debug.Log("鼠标放开");
        if (Vector3.Distance(_cupRectDrag.transform.localPosition, new Vector3(-40, 0)) > 100)
        {
            _cupRectDrag.localPos = new Vector2(437, -694);
        }
        else
        {
            _cupRectDrag.localPos = new Vector3(-40, 247);
            _cupRectDrag.enabled = false;
            //OpenClickBtn(true);
            ClickBtn();
        }
    }
    private void PointerDown(PointerEventData obj)
    {
        _chapterGuidance.gameObject.SetActive(false);
    }


    /// <summary>
    /// 点击
    /// </summary>
    public void ClickBtn()
    {
        OpenClickBtn(false);
        _phoneBtn.transform.Find("ChapterClickGuidance").gameObject.SetActive(false);
        StepCut(_clickIndex);
        _clickIndex++;
    }
    /// <summary>
    /// 步骤切换
    /// </summary>
    /// <param name="index"></param>
    public async void StepCut(int index)
    {
        switch (index)
        {
            case 0:
                await ChapterTool.MoveUi(_oneIamgeRect, new Vector2(-490f, 0), 0.1f, 0.1f);
                await ChapterTool.FadeInFadeOut(_oneIamgeCanvasGroup, 0, fadeOut, null, () => { _oneIamgeRect.gameObject.SetActive(false); ClickBtn(); });
                //OpenClickBtn(true);
                break;
            case 1:
                _dialogueBase.Initial(() =>
                {
                    Debug.Log("对话1完毕");
                    //OpenClickBtn(true);
                    _dialogueBase.Close();
                    //_clickIndex = 2;
                    ClickBtn();
                    Debug.Log("_clickIndex:" + _clickIndex);
                }, TwoSceneSpeakBeforeCallBack, TwoSceneSpeakAfterCallBack);
                _dialogueBase.Show();
                //OpenClickBtn(false);
                break;
            case 2:
                //await ChapterTool.FadeInFadeOut(_twoImagCanvasGroup, 0, 0.1f, null, () =>
                //{
                _twoImagCanvasGroup.alpha = 0;
                _dialogueBoxTetragonumComponent.Initial(async () =>
                {
                    await ChapterTool.FadeInFadeOut(_twoImagCanvasGroup, 1, fadeOut, new CancellationTokenSource(), () =>
                     {
                         _therrBoxRect.gameObject.SetActive(false);
                         _dialogueBase.SetStartDialogueId(12000036);
                         _dialogueBase.Initial(() =>
                         {
                             ClickBtn();//心想完成后直接下个画面
                             //OpenClickBtn(true);
                             _dialogueBase.Close();
                         });
                         _dialogueBase.Show();
                     });
                    _dialogueBoxTetragonumComponent.Close();
                }, SpeakBeforeAction, SpeakRearAction);
                _dialogueBoxTetragonumComponent.Show();
                //OpenClickBtn(false);
                //_dialogueBoxTetragonumComponent.OpenClickBtn(false);
                //});
                break;
            case 3:
                await ChapterTool.FadeInFadeOut(_twoImagCanvasGroup, 0, fadeOut, null, () => { _twoImagCanvasGroup.gameObject.SetActive(false); });
                _dialogueBaseTwo.Initial(() =>
                {
                    _dialogueBaseTwo.Close();
                    Debug.Log("对话2完毕");
                    ChapterTool.MoveUi(_bgTwoRect, new Vector3(-737, 0, 0), 0.8f, 0.2f);
                    ChapterTool.ChangeUiSize(_bgTwoRect, new Vector3(2737, 5808, 1), 90, 10, null, () =>
                    {
                        _phoneBtn.enabled = true;
                        _phoneBtn.transform.GetComponent<Image>().enabled = true;
                        _phoneBtn.transform.Find("ChapterClickGuidance").gameObject.SetActive(true);
                    });
                }, SpeakAeforeActionTwo, SpeakRearActionTwo, SpeakBoxCloseActionTwo);
                _dialogueBaseTwo.Show();
                Debug.Log("对话2初始化完毕");
                break;
            case 4:
                Debug.Log("第四步");

                await _dialogueBaseTwo.CloseAllShowText();
                await UniTask.WaitUntil(() => _dialogueBaseTwo.IsAllClose(0));
                await UniTask.Delay(TimeSpan.FromSeconds(1));
                _dialogueBaseTwo.MomentCloseOrOpen(true);
                _dialogueBaseTwo.OpenClickBtn(false);
                _dialogueBaseTwo.gameObject.SetActive(false);
                ChapterTool.FadeInFadeOut(_fourCanvasGroup, 1, fadeOut, new CancellationTokenSource());
                await ChapterTool.FadeInFadeOut(_twoImagCanvasGroup, 0, fadeOut, null, () =>
                {
                    _twoImageRect.gameObject.SetActive(false);
                    _chapterGuidance.gameObject.SetActive(true);
                    _chapterGuidance.PlayGuidanceAnima(_chapterGuidance.transform.localPosition, new Vector3(-40, 247));
                });
                break;
            case 5:
                ChapterTool.FadeInFadeOut(_cupCanvasGroup, 0, fadeOut, null, () => { _cupRect.gameObject.SetActive(false); });
                await ChapterTool.FadeInFadeOut(_bgtOneCanvasGroup, 0, fadeOut, new CancellationTokenSource(), () =>
                {
                    _bgtOneCanvasGroup.gameObject.SetActive(false);
                    _dialogueBaseTwo.gameObject.SetActive(true);
                    _dialogueBaseTwo.MomentCloseOrOpen(false);
                    _dialogueBaseTwo.OpenClickBtn(true);
                    _dialogueBaseTwo.GetRoleData(10000001).SetRolePoint(new Vector3(24, 543));
                    _dialogueBaseTwo.GetRoleData(10000002).SetRolePoint(new Vector3(0, -1550));
                    _dialogueBaseTwo.ClickBtn();
                });
                break;
            case 6:
                _phoneBtn.enabled = false;
                await ChapterTool.FadeInFadeOut(_bgTwoCanvasGroup, 0, fadeOut, null, () => { _bgTwoRect.gameObject.SetActive(false); });
                break;
            case 7:
                OprnFour();
                break;
            case 8:
                _fiveBtnBoxRect.gameObject.SetActive(true);
                _fourRect.gameObject.SetActive(false);
                _answerThePhoneRect.gameObject.SetActive(false);
                break;
        }
    }
    //场景2说话前回调
    void TwoSceneSpeakBeforeCallBack(ChapterDialogueTextDefine data)
    {
        if (data.ID == 12000029 )
        {//男说话
            SwitchFace_Nan(true);
        }
        if (data.ID == 12000032)
        {
            SwitchFace_Nan(true);
            SwitchFace_Nv(3);
        }
        if (data.ID == 12000030)
        {//女说话
            SwitchFace_Nv(2);
        }
    }
    //男主说话
    void SwitchFace_Nan(bool isSpeak)
    {//T说话 F闭嘴
        nanface1_image.gameObject.SetActive(!isSpeak);
        nanface2_image.gameObject.SetActive(isSpeak);
    }
    //女主切换表情
    void SwitchFace_Nv(int index)
    {//1张望 2张嘴 3闭嘴 4害羞
        switch (index)
        {
            case 1:
                nvface1_image.gameObject.SetActive(true);
                nvface2_image.gameObject.SetActive(false);
                nvface3_image.gameObject.SetActive(false);
                nvface4_image.gameObject.SetActive(false);
                break;
            case 2:
                nvface1_image.gameObject.SetActive(false);
                nvface2_image.gameObject.SetActive(true);
                nvface3_image.gameObject.SetActive(false);
                nvface4_image.gameObject.SetActive(false);
                SwitchFace_Nan(false);//女说话时男闭嘴
                break;
            case 3:
                nvface1_image.gameObject.SetActive(false);
                nvface2_image.gameObject.SetActive(false);
                nvface3_image.gameObject.SetActive(true);
                nvface4_image.gameObject.SetActive(false);
                break;
            case 4:
                nvface1_image.gameObject.SetActive(false);
                nvface2_image.gameObject.SetActive(false);
                nvface3_image.gameObject.SetActive(false);
                nvface4_image.gameObject.SetActive(true);
                break;
        }
    }

    //场景2说话后回调
    void TwoSceneSpeakAfterCallBack(ChapterDialogueTextDefine data)
    {
        if (data.ID == 12000032)
        {
            SwitchFace_Nv(4);
            //_dialogueBase.OpenClickBtn(false);//关闭按钮点击
            //await UniTask.Delay(300);
            //_dialogueBase.OpenClickBtn(true);
        }
        if (data.ID == 12000033)
        {//33句说完后男闭嘴
            SwitchFace_Nan(false);
        }
    }

    float fadeOut = 0.1f;
    /// <summary>
    /// 人物说话前回调(环顾四周)
    /// </summary>
    /// <param name="data"></param>
    public async void SpeakBeforeAction(ChapterDialogueTextDefine data)
    {
        if (data.ID == 12000035)
        {
            await ChapterTool.FadeInFadeOut(_bgs[0], 0, fadeOut);

        }
    }
    /// <summary>
    /// 人物说话后回调(环顾四周)
    /// </summary>
    /// <param name="data"></param>
    public async void SpeakRearAction(ChapterDialogueTextDefine data)
    {
        if (data.ID == 12000034)
        {
            //await ChapterTool.FadeInFadeOut(_bgs[0], 0, fadeOut);
            //await UniTask.Delay(TimeSpan.FromSeconds(1));
            //_bgs[0].alpha = 1;//直接切
            //_dialogueBoxTetragonumComponent.ClickBtn();
        }
        else if (data.ID == 12000035)
        {
            //await ChapterTool.FadeInFadeOut(_bgs[0], 0, fadeOut);

        }
    }
    /// <summary>
    /// 人物说话前回调（对杯）
    /// </summary>
    /// <param name="data"></param>
    public async void SpeakAeforeActionTwo(ChapterDialogueTextDefine data)
    {
        if (data.ID == 12000038)
        {
            //_dialogueBaseTwo.SetRoleSpeakAeforeAwait(true);
            //Debug.LogError("开启对话前等待回调");
        }
    }
    /// <summary>
    /// 人物说话后回调
    /// </summary>
    /// <param name="data"></param>
    public void SpeakRearActionTwo(ChapterDialogueTextDefine data)
    {
        if (data.ID == 12000038)
        {
            //Debug.LogError("暂时关闭对话");
            _dialogueBaseTwo.MomentCloseOrOpen(true);
            _dialogueBaseTwo.OpenClickBtn(false);
            OpenClickBtn(true);
        }
    }

    private void SpeakBoxCloseActionTwo(ChapterDialogueTextDefine data)
    {

    }

    void OpenClickBtn(bool isOpen)
    {
        _clickBtn.enabled = isOpen;
    }
    #endregion
}
