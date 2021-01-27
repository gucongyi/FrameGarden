using Company.Cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chapter6_Caricature : CaricaturePlayerSpecialModuleBasics
{
    public RectTransform bg;

    public DialogueBoxBubbleComponent DialogueBox_one;
    [SerializeField]
    Image nv1_1;
    [SerializeField]
    Image nv1_2;
    [SerializeField]
    Image nv3_face;

    public override void Initial()
    {
        bg.gameObject.SetActive(true);
        nv1_1.gameObject.SetActive(true);
        base.Initial();
    }

    public override void MoveEnd()
    {
        DialogueBox_one.Initial(() =>
        {
            DialogueBox_one.Close();
            transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
            Over();
        }, BeforeDialogue);
        DialogueBox_one.Show();
        base.MoveEnd();
    }

    void BeforeDialogue(ChapterDialogueTextDefine data)
    {
        if (data.ID == 16000039)
        {
            nv1_1.gameObject.SetActive(false);
            nv1_2.gameObject.SetActive(true);
        }
        if (data.ID == 16000040)
        {
            nv3_face.gameObject.SetActive(true);
            nv1_1.gameObject.SetActive(true);
            nv1_2.gameObject.SetActive(false);
        }
        if (data.ID == 16000041)
        {
            nv3_face.gameObject.SetActive(false);
        }

    }


}
