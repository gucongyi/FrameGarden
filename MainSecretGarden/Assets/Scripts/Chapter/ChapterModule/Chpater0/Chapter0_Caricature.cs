using Company.Cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 序章条漫特殊模块1
/// </summary>
public class Chapter0_Caricature : CaricaturePlayerSpecialModuleBasics
{
    public RectTransform bg;
    public Image hand;
    //初始值
    //旋转new (0，0，25)
    //目标值（0，0，0）
    //位置new (-62，-548)
    //目标值（-223f, -245f）
    public Vector3 targetPos = new Vector3(-223f, -245f);
    float durationTime = 0.5f;
    public DialogueBoxBubbleComponent Dialogues;

    public override void Initial()
    {
        bg.gameObject.SetActive(true);
        base.Initial();
    }

    public override void MoveEnd()
    {
        //手动画 接着说话
        hand.transform.DOLocalRotate(Vector3.zero, durationTime);
        hand.transform.DOLocalMove(targetPos, durationTime).OnComplete(() =>
         {
             Dialogues.Initial(() =>
             {
                 Dialogues.Close();
             }, BeforeDialogue, AfterDialogue);
             Dialogues.Show();
             base.MoveEnd();
         });
    }

    void BeforeDialogue(ChapterDialogueTextDefine data)
    {

    }
    void AfterDialogue(ChapterDialogueTextDefine data)
    {
        if (data.ID == 10000022)
        {
            Over();
            transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }
}
