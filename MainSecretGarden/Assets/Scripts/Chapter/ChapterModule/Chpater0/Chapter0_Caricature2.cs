using Company.Cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 序章条漫特殊模块2
/// </summary>
public class Chapter0_Caricature2 : CaricaturePlayerSpecialModuleBasics
{
    public RectTransform image1;
    public Image image2;
    public DialogueBoxBubbleComponent DialogueBox;

    public override void Initial()
    {
        image1.gameObject.SetActive(true);
        base.Initial();
    }

    public override void Click()
    {
        DialogueBox.ClickBtn();

        base.Click();
    }

    public override void MoveEnd()
    {
        _synchronizationBtn = DialogueBox.GetComponent<Button>();
        DialogueBox.Initial(() =>
        {
            //Over();
            DialogueBox.Close();
        }, BeforeDialogue, AfterDialogue);
        DialogueBox.Show();
        base.MoveEnd();
    }

    void BeforeDialogue(ChapterDialogueTextDefine data)
    {
        //OpenClick(false);
        if (data.ID == 10000025)
        {
            image2.gameObject.SetActive(true);
        }
    }
    void AfterDialogue(ChapterDialogueTextDefine data)
    {
        //OpenClick(true);
        if (data.ID == 10000025)
        {
            Over();
         
            transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }
}
