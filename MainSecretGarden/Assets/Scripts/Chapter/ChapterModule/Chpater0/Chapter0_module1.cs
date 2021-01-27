using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chapter0_module1 : ChapterControllerBasics
{
    #region 字段
    /// <summary>
    /// 下一步按钮
    /// </summary>
    Button _nextStepBtn;
    //场景
    RectTransform _oneScene;
    string ParticleName = "XuZhang_Particle";
    string clickGuidanceName = "ChapterClickGuidance";//点击引导
    ParticleSystem particle;//特效
    #region 主持人手初始值
    //初始手旋转值
    Vector2 Resethand1Pos = new Vector2(885.3f, 1322.3f);//张  手臂
    Vector3 Resethand1Rotation = new Vector3(0, 0, 0f);
    Vector2 Resethand2Pos = new Vector2(1041f, 1567f);//手掌
    Vector3 Resethand2Rotation = new Vector3(0, 0, 21.3f);
    float ScreenRatio;//屏幕比例
    #endregion


    Image graduation_image;//毕业典礼
    Image platform_image;//校长讲台
    DialogueBoxTetragonumComponent dialogueBox;
    Image papapa_image;//啪啪啪
    Text papapa_text1;
    Text papapa_text2;
    Image compere_image;//女主持人
    Image compereHand_image;//手臂
    Image compereHand2_image;//手掌
    Image appearOnTheStage0_image;//上台背景
    Image appearOnTheStage1_image;//上台背景
    Image appearOnTheStage2_image;//上台背景
    Image appearOnTheStage3_image;//上台背景
    Image appearOnTheStage4_image;//上台背景
    Image lecture_image;//女主说话
    Image passerby_image;//路人
    DialogueBubble passerbyDialgoue;//路人对话
    Image groupPhoto_image;//合照
    Image white;//拍照白板
    RectTransform PhotoMaskTop;
    RectTransform PhotoMaskBottom;
    Button photo_btn;//拍照按钮
    [SerializeField]
    Texture2D tex;

    //拍照黑屏时长 ms
    int durationTime = 150;
    Button save_btn;
    Text savebtn_text;
    Button next_btn;
    Text nextbtn_text;

    /// <summary>
    /// 点击计数
    /// </summary>
    int _clickIndex = 1;
    #endregion
    #region 函数
    private void Start()
    {
        //Initial();
    }
    public override void Initial()
    {
        ScreenRatio = Screen.height / Screen.width;
        _nextStepBtn = transform.GetComponent<Button>();
        _nextStepBtn.onClick.RemoveAllListeners();
        _nextStepBtn.onClick.AddListener(ClickBtn);

        _oneScene = transform.Find("One") as RectTransform;

        #region data
        graduation_image = _oneScene.Find("graduation_image").GetComponent<Image>();
        platform_image = _oneScene.Find("platform_image").GetComponent<Image>();
        dialogueBox = transform.Find("DialogueBox_Tetragonum").GetComponent<DialogueBoxTetragonumComponent>();
        papapa_image = _oneScene.Find("papapa_image").GetComponent<Image>();
        papapa_text1 = papapa_image.transform.Find("Text1").GetComponent<Text>();
        papapa_text2 = papapa_image.transform.Find("Text2").GetComponent<Text>();

        compere_image = _oneScene.Find("compere_image").GetComponent<Image>();
        compereHand_image = compere_image.transform.Find("compereHand_image").GetComponent<Image>();
        compereHand2_image = compere_image.transform.Find("compereHand2_image").GetComponent<Image>();
        appearOnTheStage0_image = transform.Find("appearOnTheStage0_image").GetComponent<Image>();
        appearOnTheStage1_image = appearOnTheStage0_image.transform.Find("appearOnTheStage1_image").GetComponent<Image>();
        appearOnTheStage2_image = appearOnTheStage0_image.transform.Find("appearOnTheStage2_image").GetComponent<Image>();
        appearOnTheStage3_image = appearOnTheStage0_image.transform.Find("appearOnTheStage3_image").GetComponent<Image>();
        appearOnTheStage4_image = appearOnTheStage0_image.transform.Find("appearOnTheStage4_image").GetComponent<Image>();
        lecture_image = _oneScene.Find("lecture_image").GetComponent<Image>();
        passerby_image = _oneScene.Find("passerby_image").GetComponent<Image>();
        passerbyDialgoue = passerby_image.transform.Find("passerbyDialgoue").GetComponent<DialogueBubble>();
        groupPhoto_image = _oneScene.Find("groupPhoto_image").GetComponent<Image>();
        white = groupPhoto_image.transform.Find("white").GetComponent<Image>();

        PhotoMaskTop = transform.Find("PhotoMaskTop") as RectTransform;
        PhotoMaskBottom = transform.Find("PhotoMaskBottom") as RectTransform;
        photo_btn = PhotoMaskBottom.Find("photo_btn").GetComponent<Button>();

        //下一步按钮
        save_btn = _oneScene.Find("NextBtnGroup/SaveButton").GetComponent<Button>();
        savebtn_text = save_btn.transform.Find("Text").GetComponent<Text>();
        next_btn = _oneScene.Find("NextBtnGroup/NextButton").GetComponent<Button>();
        nextbtn_text = next_btn.transform.Find("Text").GetComponent<Text>();
        #endregion
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

    int changeIndex = 0;
    public void StepCut(int index)
    {
        //if (index > 6) return;
        switch (index)
        {
            case 1:
                ShowGraduationCeremony_image();
                break;
            case 2:
                ShowPaPaPa_Image();
                break;
            case 3:
                ShowCompere_Image(1);
                break;
            case 4://显示登台
                
                ShowAppearOnTheStage_Image(1);
                break;
            case 5:
                ShowAppearOnTheStage_Image(2);
                break;
            case 6:
                ShowAppearOnTheStage_Image(3);
                break;
            case 7:
                ShowAppearOnTheStage_Image(4);
                break;
            case 8:
                ShowLecture_Image(1);
                break;
            case 9:
                ShowPasserby_Image();
                break;
            case 10:
                ShowLecture_Image(2);
                break;
            case 11:
                ShowCompere_Image(2);
                break;
            case 12:
                ShwoGroupPhoto_Image();
                break;
        }
    }

    //显示讲台
    async void ShowGraduationCeremony_image()
    {
        //1.15修改
        graduation_image.transform.DOScale(2.5f,3f).SetEase(Ease.Linear);
        graduation_image.transform.DOLocalMoveY(1000f, 3f).SetEase(Ease.Linear);
        await UniTask.Delay(900);
        graduation_image.transform.GetComponent<CanvasGroup>().DOFade(0f, 0.5f).SetEase(Ease.Linear);
        platform_image.gameObject.SetActive(true);
        platform_image.transform.GetComponent<CanvasGroup>().DOFade(1f, 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            dialogueBox.Initial(() =>
            {
                dialogueBox.Close();
                //OpenClickBtn(true);
                ClickBtn();
            });
            dialogueBox.Show();
        });
    }
    //显示鼓掌
    void ShowPaPaPa_Image()
    {
        ChapterHelper.SetActive(papapa_image.gameObject, true, async () =>
          {
              ChapterHelper.SetActive(platform_image.gameObject, false);
              papapa_text1.text = ChapterTool.GetChapterFunctionString(10000542);
              papapa_text2.text = ChapterTool.GetChapterFunctionString(10000542);
              papapa_text1.transform.GetComponent<CanvasGroup>().DOFade(1, 0.4f);
              papapa_text1.transform.DOLocalMove(new Vector3(-14, 1016, 0), 0.4f);
              papapa_text2.transform.GetComponent<CanvasGroup>().DOFade(1, 0.2f);
              papapa_text2.transform.DOLocalMove(new Vector3(560, -1100, 0), 0.2f);
              await UniTask.Delay(500);
              OpenClickBtn(true);
          });
    }
    //显示主持人
    void ShowCompere_Image(int index)
    {
        switch (index)
        {
            case 1:
                ChapterHelper.SetActive(compere_image.gameObject, true, async () =>
                {
                    ChapterHelper.SetActive(papapa_image.gameObject, false);
                    await UniTask.Delay(100);
                    (compereHand_image.transform as RectTransform).DOAnchorPos(Resethand1Pos, 0.3f);
                    compereHand_image.transform.DORotate(Resethand1Rotation, 0.3f);
                    (compereHand2_image.transform as RectTransform).DOAnchorPos(Resethand2Pos, 0.3f);
                    compereHand2_image.transform.DORotate(Resethand2Rotation, 0.3f);
                });
                dialogueBox.SetStartDialogueId(10000003);
                dialogueBox.Initial(() =>
                {
                    dialogueBox.Close();
                    //OpenClickBtn(true);
                    ClickBtn();
                });
                dialogueBox.Show();
                break;
            case 2:
                particle.Clear();
                particle.Pause();//暂停特效
                ChapterHelper.SetActive(compere_image.gameObject, true, () =>
                  {
                      ChapterHelper.SetActive(lecture_image.gameObject, false);
                  });
                dialogueBox.SetStartDialogueId(10000007);
                dialogueBox.Initial(() =>
                {
                    dialogueBox.Close();
                    //OpenClickBtn(true);
                    ClickBtn();
                });
                dialogueBox.Show();

                break;
        }
    }
    //显示登台
    async void ShowAppearOnTheStage_Image(int index)
    {
        switch (index)
        {
            case 1:
                ChapterHelper.SetActive(appearOnTheStage0_image.gameObject, true, () =>
                {
                    ChapterHelper.SetActive(compere_image.gameObject, false);
                    ChapterHelper.SetActive(appearOnTheStage1_image.gameObject, true);
                });
                break;
            case 2:
                MoveY(appearOnTheStage2_image.rectTransform, -1170);
                break;
            case 3:
                MoveY(appearOnTheStage3_image.rectTransform, -1539);
                break;
            case 4:
                if (ScreenRatio < 1.8f)
                {
                    appearOnTheStage0_image.rectTransform.DOAnchorPosY(475f, 0.6f);
                }
                if (ScreenRatio > 1.8f)
                {
                    appearOnTheStage0_image.rectTransform.DOAnchorPosY(175f, 0.6f);
                }
                MoveY(appearOnTheStage4_image.rectTransform, -2154);
                break;
        }
        await UniTask.Delay(800);
        OpenClickBtn(true);
    }
    //登台
    void MoveY(RectTransform trans, float targetY, float duration = 0.8f)
    {
        trans.anchoredPosition = new Vector2(trans.anchoredPosition.x, -2875f);
        trans.gameObject.SetActive(true);
        trans.DOAnchorPosY(targetY, duration);
    }

    //显示女主讲话
    void ShowLecture_Image(int index)
    {
        switch (index)
        {
            case 1:
                //生成特效
                var ParticleParfab = ABManager.GetAsset<GameObject>(ParticleName);
                GameObject parParfab = GameObject.Instantiate(ParticleParfab);
                particle = parParfab.GetComponent<ParticleSystem>();
                ChapterHelper.SetParent(parParfab, UIRoot.instance.GetUIRootCanvas().transform);
                (parParfab.transform as RectTransform).anchoredPosition = Vector2.zero;
                particle.Play();
                ChapterHelper.SetActive(lecture_image.gameObject, true, () =>
                {
                    ChapterHelper.SetActive(appearOnTheStage0_image.gameObject, false);
                });
                dialogueBox.SetStartDialogueId(10000004);
                dialogueBox.Initial(() =>
                {
                    dialogueBox.Close();
                    //OpenClickBtn(true);
                    ClickBtn();
                });
                dialogueBox.Show();
                break;
            case 2:
                particle.Play();//开启特效
                ChapterHelper.SetActive(lecture_image.gameObject, true, () =>
                {
                    ChapterHelper.SetActive(passerby_image.gameObject, false);
                });
                dialogueBox.SetStartDialogueId(10000006);
                dialogueBox.Initial(() =>
                {
                    dialogueBox.Close();
                    //OpenClickBtn(true);
                    ClickBtn();
                });
                dialogueBox.Show();
                break;
        }

    }
    //显示路人讲话
    async void ShowPasserby_Image()
    {
        var data = ChapterTool.GetChapterData(10000005);
        string dialogue = ChapterTool.GetDialogueString(data);
        particle.Clear();
        particle.Pause();//暂停特效
        ChapterHelper.SetActive(passerby_image.gameObject, true, () =>
         {
             ChapterHelper.SetActive(lecture_image.gameObject, false);
             passerbyDialgoue.Play(dialogue);
         });
        await UniTask.Delay(800);
        OpenClickBtn(true);
    }
    //显示合照
    async void ShwoGroupPhoto_Image()
    {
        ChapterHelper.SetActive(groupPhoto_image.gameObject, true, () =>
         {
             ChapterHelper.SetActive(compere_image.gameObject, false);
             PhotoMaskTop.DOAnchorPosY(0, 0.8f);
             PhotoMaskBottom.DOAnchorPosY(0, 0.8f);
         });
        //生成点击引导
        var clickGuidance = await ClickGuidance(photo_btn.transform);
        (clickGuidance.transform as RectTransform).anchoredPosition = Vector2.zero;
        //拍照
        photo_btn.onClick.RemoveAllListeners();
        photo_btn.onClick.AddListener(() => { GameObject.Destroy(clickGuidance); ClickPhotoBtn(); });

        //清除之前生成的所有
        if (particle != null)
            GameObject.Destroy(particle.gameObject);
    }

    /// <summary>
    /// 创建点击引导
    /// </summary>
    public async System.Threading.Tasks.Task<GameObject> ClickGuidance(Transform parent, float DelayTime = 0)
    {
        int time = (int)DelayTime * 1000;
        await Cysharp.Threading.Tasks.UniTask.Delay(time);
        var parfab = await ABManager.GetAssetAsync<GameObject>(clickGuidanceName);
        GameObject go = GameObject.Instantiate(parfab);
        ChapterHelper.SetParent(go, parent);
        return go;
    }

    //拍照效果
    async void TakePhotos()
    {
        ChapterHelper.SetActive(white.gameObject, true);
        await UniTask.Delay(durationTime);
        ChapterHelper.SetActive(white.gameObject, false);

        PhotoMaskTop.anchoredPosition = new Vector2(0, ChapterBase.photoMask1InitY);
        PhotoMaskBottom.anchoredPosition = new Vector2(0, ChapterBase.photoMask2InitY);
        groupPhoto_image.transform.DOScale(ChapterBase.lessenScale, ChapterBase.lessenScaleDurationTime);
        groupPhoto_image.rectTransform.DOAnchorPosY(ChapterBase.photoY, ChapterBase.lessenScaleDurationTime).OnComplete(() =>
        {
            ChapterHelper.Fade(save_btn.transform.parent.gameObject, 1, 1f, 0);
            //按钮条件注册
            save_btn.onClick.RemoveAllListeners();
            save_btn.onClick.AddListener(SavePhotoBtn);
            savebtn_text.text = "保存图片";//TODO

            next_btn.onClick.RemoveAllListeners();
            next_btn.onClick.AddListener(ClickNext);
            nextbtn_text.text = "下一步";//TODO
        });
    }
    //保存图片
    async void SavePhotoBtn()
    {
        //string path = Application.streamingAssetsPath + "/xuzhang.png";
        //UniversalTool.SaveTexture2DToFile(tex, path);
        string path = Application.streamingAssetsPath + "/xuzhang.png";
        UniversalTool.CaptureScreenAndSave(path);
        StaticData.CreateToastTips("保存成功");//TODO
    }
    //进行下一步
    void ClickNext()
    {
        if (particle != null)
            GameObject.Destroy(particle.gameObject);
        NextStep();
    }
    //点击相机按钮
    void ClickPhotoBtn()
    {
        photo_btn.enabled = false;
        TakePhotos();
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