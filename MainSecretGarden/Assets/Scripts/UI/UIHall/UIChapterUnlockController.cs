using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 章节解锁 控制
/// </summary>
public class UIChapterUnlockController : MonoBehaviour
{
    #region 变量

    private Transform _bgTra;

    private Text _chpaterName;
    private Image _chpaterIcon;

    private Button _butCancel;
    private Button _butEnter;

    private int _unlockChapterID;

    #endregion

    #region 方法

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        _bgTra = transform.Find("BG_Image");
        if (_bgTra != null)
            UniversalTool.ReadyPopupAnim(_bgTra);
    }

    private void OnEnable()
    {
        if (_bgTra != null)
            UniversalTool.StartPopupAnim(_bgTra);
    }

    private void OnDisable()
    {
        if (_bgTra != null)
            UniversalTool.ReadyPopupAnim(_bgTra);
    }

    private void Init() 
    {
        if (_chpaterName != null)
            return;

        _chpaterName = transform.Find("BG_Image/DividingLineImage/ChapterName").GetComponent<Text>();
        _chpaterIcon = transform.Find("BG_Image/ChapterIcon").GetComponent<Image>();

        _butCancel = transform.Find("BG_Image/But_Cancel").GetComponent<Button>();
        _butEnter = transform.Find("BG_Image/But_Enter").GetComponent<Button>();

        _butCancel.onClick.RemoveAllListeners();
        _butCancel.onClick.AddListener( ()=> { 
            UniversalTool.CancelPopAnim(_bgTra, OnClickCancel);  } );

        _butEnter.onClick.RemoveAllListeners();
        _butEnter.onClick.AddListener(() => { 
            UniversalTool.CancelPopAnim(_bgTra, OnClickEnter); });
    }

    public async void InitValue(int chapterID) 
    {
        Init();
        _unlockChapterID = chapterID;//Section
        var chapterInfo = StaticData.configExcel.GetSectionBySectionId(chapterID);

        _chpaterName.text = ChapterTool.GetChapterFunctionString(chapterInfo.SectionNumber) + LocalizationDefineHelper.GetStringNameById(120127) +  ChapterTool.GetChapterFunctionString(chapterInfo.SectionNameId) + LocalizationDefineHelper.GetStringNameById(120128);
        //章节icon
        if (!string.IsNullOrEmpty(chapterInfo.Icon)) 
        {
            _chpaterIcon.sprite = await ABManager.GetAssetAsync<Sprite>(chapterInfo.Icon);
            _chpaterIcon.SetNativeSize();
        }
            
    }

    private void OnClickCancel() 
    {
        UIComponent.RemoveUI(UIType.UIChapterUnlock);
    }

    private void OnClickEnter()
    {
        //新手引导标记完成
        if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.isCurrStepGuiding)
        {
            GuideCanvasComponent._instance.SetLittleStepFinish();
        }
        //需要打开界面
        ChapterHelper.EnterIntoChapter(_unlockChapterID,true);
        OnClickCancel();
    }



    #endregion
}
