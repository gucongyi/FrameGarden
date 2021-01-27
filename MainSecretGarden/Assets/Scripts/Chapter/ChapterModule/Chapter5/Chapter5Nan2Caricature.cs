using Company.Cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chapter5Nan2Caricature : CaricaturePlayerSpecialModuleBasics
{
    public GameObject bg;
    public Image nvzhuFace1;//正常说话
    public Image nvzhuFace2;//害羞

    public DialogueBoxBubbleComponent DialogueBox_two;

    public override void Initial()
    {
        bg.gameObject.SetActive(true);

        base.Initial();
    }

    public override void MoveEnd()
    {
        DialogueBox_two.Initial(() =>
        {
            DialogueBox_two.Close();
            transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
            Over();
        }, BeforeDialogue);
        DialogueBox_two.Show();
        base.MoveEnd();
    }


    void BeforeDialogue(ChapterDialogueTextDefine data)
    {
        if (data.ID == 15000089)
        {
            nvzhuFace1.gameObject.SetActive(false);
            nvzhuFace2.gameObject.SetActive(true);
        }
        if (data.ID == 15000091)
        {
            nvzhuFace2.gameObject.SetActive(false);
        }
        if (data.ID == 15000093)
        {
            nvzhuFace2.gameObject.SetActive(true);
        }
    }

}
