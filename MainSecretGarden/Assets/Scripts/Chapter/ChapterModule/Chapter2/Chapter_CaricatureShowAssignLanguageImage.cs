using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 条漫显示指定语言的图片
/// </summary>
public class Chapter_CaricatureShowAssignLanguageImage : CaricaturePlayerSpecialModuleBasics
{
    public enum TextOrImage
    {
        Image,
        Text
    }
    public Image bg;
    public Image English_image;
    public Image China_image;
    public Text wordText;
    public int textID;
    public TextOrImage textOrImage;

    public override void Initial()
    {
        if (bg != null)
            bg.gameObject.SetActive(true);
        switch (textOrImage)
        {
            case TextOrImage.Text:
                wordText.text = ChapterTool.GetChapterFunctionString(textID);
                switch (StaticData.linguisticType)
                {
                    case Company.Cfg.LinguisticType.Simplified:
                        wordText.gameObject.SetActive(true);
                        break;
                    case Company.Cfg.LinguisticType.Complex:
                        wordText.gameObject.SetActive(true);
                        break;
                    case Company.Cfg.LinguisticType.English:
                        wordText.gameObject.SetActive(true);
                        break;
                }
                break;
            case TextOrImage.Image:
                switch (StaticData.linguisticType)
                {
                    case Company.Cfg.LinguisticType.Simplified:
                        China_image.gameObject.SetActive(true);
                        break;
                    case Company.Cfg.LinguisticType.Complex:
                        China_image.gameObject.SetActive(true);
                        break;
                    case Company.Cfg.LinguisticType.English:
                        English_image.gameObject.SetActive(true);
                        break;
                }
                break;
        }
        base.Initial();
    }
    public override void MoveEnd()
    {
        base.MoveEnd();

        Over();
        if (transform.GetComponent<CanvasGroup>() != null)
            transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
}
