using Company.Cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter5Nan1Caricature : CaricaturePlayerSpecialModuleBasics
{
    public RectTransform bg;

    public DialogueBoxBubbleComponent DialogueBox_two;

    public override void Initial()
    {
        bg.gameObject.SetActive(true);

        base.Initial();
    }
    public async override void MoveEnd()
    {
        await ChapterTool.MoveUi(bg, new Vector2(-517.5f, 0f), 0.1f, 0.1f, null, () =>
        {
            DialogueBox_two.Initial(() =>
            {
                DialogueBox_two.Close();
            }, DialogueBeforeAction);
            DialogueBox_two.Show();
        });
        base.MoveEnd();
    }

    public void DialogueBeforeAction(ChapterDialogueTextDefine data)
    {
        if (data.ID == 15000165)
        {
            Over();
            transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }
}