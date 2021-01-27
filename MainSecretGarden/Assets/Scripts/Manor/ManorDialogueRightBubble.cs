using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 右气泡对话框 （章节用）
/// </summary>
public class ManorDialogueRightBubble:MonoBehaviour
{
    public Text textContent;
    public void SetContent(string dialogueContent, float scaleRadio)
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(scaleRadio, 0.3f);
        textContent.text = $"{dialogueContent}";
    }
}
