using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ChapterGuidance : MonoBehaviour
{
    public Image thisSprite;
    public float AnimaDuration = 1f;//持续时间

    /// <summary>
    /// 播放章节引导动画
    /// </summary>
    /// <param name="startPos">起始位置Achor</param>
    /// <param name="endPos">结束位置Achor</param>
    /// <param name="callback">动画委托</param>
    /// <param name="LoopCount">播放次数</param>
    /// <param name="DelayTime">延时播放</param>
    /// /// <param name="callBackIsLoop">委托是否循环播放</param>
    public void PlayGuidanceAnima(Vector2 startPos, Vector2 endPos, TweenCallback callback = null, int LoopCount = -1, float DelayTime = 0, bool callBackIsLoop = false)
    {
        var s = DOTween.Sequence();
        s.Append(thisSprite.DOColor(new Color(1, 1, 1, 0), 0));//隐藏
        s.AppendInterval(DelayTime);//延时一秒
        s.Append((transform as RectTransform).DOAnchorPos(startPos, 0));
        s.Join(thisSprite.DOColor(new Color(1, 1, 1, 1), 0));
        if (callback != null)
        {
            if (callBackIsLoop)
                s.AppendCallback(callback);
            else
                transform.DOScale(Vector3.one, 0).SetDelay(1).OnComplete(callback);
        }
        s.Append((transform as RectTransform).DOAnchorPos(endPos, AnimaDuration));
        s.Join(thisSprite.DOColor(new Color(1, 1, 1, 0), AnimaDuration));
        s.SetLoops(LoopCount);

        if (!s.IsPlaying())
            Destroy(this.gameObject);
    }
}
