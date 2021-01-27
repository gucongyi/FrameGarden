using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 气泡对话
/// </summary>
public class DialogueBubble : MonoBehaviour
{
    public Text content;//对话内容

    public RectTransform angPos;//冒出动画

    bool play = false;//第一次播放动画

    //传本地化后的内容
    public void Play(string dialogueContent)
    {
        this.gameObject.SetActive(true);
        if (!play)
        {
            this.transform.localScale = Vector3.zero;
            this.transform.DOScale(1f, 0.3f);
            play = true;
        }
        //content.alignment = alignment;
        //content.alignment = TextAnchor.MiddleLeft;
        content.text = dialogueContent;
    }
}