using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum OADType
{
    plainText,
    mixtureText,
    cartoon
}
public class OADBase : MonoBehaviour
{
    #region 字段
    public static OADBase _Instance;
    [SerializeField]
    int _OADindex;
    [SerializeField]
    protected int TitleId;
    protected Text Title;
    [SerializeField]
    protected int startDialogueId;
    [SerializeField]
    protected OADType OADType;
    [Header("纯文字")]
    [SerializeField]
    protected Text plainText;
    [Header("图片文字组合")]
    [SerializeField]
    protected List<Text> mixtureText = new List<Text>();
    [Header("条漫")]
    [SerializeField]
    protected List<GameObject> cartoon = new List<GameObject>();
    List<Text> cartoonTexts = new List<Text>();
    Button BackBtn;
    RectTransform _contentRect;
    #endregion
    #region 属性
    public int _OADIndex { get { return _OADindex; } }
    #endregion
    #region 函数
    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
    }
    public virtual void Initial()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
        InitialOADDialogues();
    }
    /// <summary>
    /// 初始化多语言
    /// </summary>
    void InitialOADDialogues()
    {
        Title = transform.Find("Top/Title").GetComponent<Text>();
        var OADinfo = StaticData.configExcel.GetExtraStoryByExtraStoryId(_OADindex);
        Title.text = ChapterHelper.GetOADDialogueString(ChapterHelper.GetOADData(OADinfo.ExtraStoryName));
        _contentRect = transform.Find("Scroll View/Viewport/Content").GetComponent<RectTransform>();
        BackBtn = transform.Find("BackBtn").GetComponent<Button>();
        BackBtn.onClick.RemoveAllListeners();
        BackBtn.onClick.AddListener(ClickBackBtn);
        switch (OADType)
        {
            case OADType.plainText:
                Debug.Log("初始化纯文本番外");
                PlainTextInitial();
                break;
            case OADType.mixtureText:
                Debug.Log("初始化图文番外");
                MixtureTextInitial();
                break;
            case OADType.cartoon:
                Debug.Log("初始化条漫番外");
                CatoonInitial();
                break;
        }
    }
    /// <summary>
    /// 纯文本多语言初始化
    /// </summary>
    private void PlainTextInitial()
    {
        var data = ChapterHelper.GetOADData(startDialogueId);
        string dialogue = ChapterHelper.GetOADDialogueString(data);

        plainText.text = dialogue;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRect);
    }
    /// <summary>
    /// 图文多语言初始化
    /// </summary>
    private void MixtureTextInitial()
    {
        for (int i = 0; i < mixtureText.Count; i++)
        {
            var data = ChapterHelper.GetOADData(startDialogueId);
            string dialogue = ChapterHelper.GetOADDialogueString(data);

            mixtureText[i].text = dialogue;
            if (data.NextDialogId != 0)
                startDialogueId = data.NextDialogId;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRect);
    }
    /// <summary>
    /// 条漫多语言初始化
    /// </summary>
    private void CatoonInitial()
    {
        foreach (GameObject item in cartoon)
        {
            cartoonTexts.Add(item.transform.Find("Box/Text").GetComponent<Text>());
        }

        for (int i = 0; i < cartoonTexts.Count; i++)
        {
            var data = ChapterHelper.GetOADData(startDialogueId);
            string dialogue = ChapterHelper.GetOADDialogueString(data);

            cartoonTexts[i].text = dialogue;
            if (data.NextDialogId != 0)
                startDialogueId = data.NextDialogId;
        }
    }

    public void ClickBackBtn()
    {
        UIComponent.RemoveUI($"OAD{OADBase._Instance._OADIndex - StaticData.configExcel.ExtraStory[0].ExtraStoryId + 1}");
        _Instance = null;
    }
    #endregion
}
