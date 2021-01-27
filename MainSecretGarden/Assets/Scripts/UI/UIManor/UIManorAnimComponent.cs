using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManorAnimComponent : MonoBehaviour
{
    public RectTransform Left;
    public RectTransform Right;
    public RectTransform Top;
    public RectTransform SelfBottom;
    public RectTransform SelfIcons;
    public RectTransform SelfDecotate;
    public RectTransform FriendBottom;
    const float timeLength = 0.5f;
    public async UniTask PlayAnimEnterManor()
    {
        DoTweenToPos(Left,new Vector2(56f,Left.anchoredPosition.y));
        DoTweenToPos(Right, new Vector2(-103f, Right.anchoredPosition.y));
        DoTweenToPos(SelfBottom, new Vector2(SelfBottom.anchoredPosition.x,100f));
        DoTweenToPos(FriendBottom, new Vector2(FriendBottom.anchoredPosition.x,0f));
        DoTweenToPos(Top, new Vector2(Top.anchoredPosition.x, 0f));
        await UniTask.Delay((int)(timeLength*1000));
    }
    public async UniTask PlayAnimQuitManor()
    {
        DoTweenToPos(Left, new Vector2(-444f, Left.anchoredPosition.y));
        DoTweenToPos(Right, new Vector2(1000f, Right.anchoredPosition.y));
        DoTweenToPos(SelfBottom, new Vector2(SelfBottom.anchoredPosition.x, -500f));
        DoTweenToPos(FriendBottom, new Vector2(FriendBottom.anchoredPosition.x, -500f));
        DoTweenToPos(Top, new Vector2(Top.anchoredPosition.x, 500f));
        await UniTask.Delay((int)(timeLength * 1000));
    }

    public async UniTask PlayAnimOpenDecorate()
    {
        DoTweenToPos(Left, new Vector2(-444f, Left.anchoredPosition.y));
        DoTweenToPos(Right, new Vector2(1000f, Right.anchoredPosition.y));
        DoTweenToPos(SelfIcons, new Vector2(SelfIcons.anchoredPosition.x, -500f));
        DoTweenToPos(SelfDecotate, new Vector2(SelfDecotate.anchoredPosition.x, -100f));
        DoTweenToPos(Top, new Vector2(Top.anchoredPosition.x, 500f));
        await UniTask.Delay((int)(timeLength * 1000));
    }

    public async UniTask PlayAnimCloseDecorate()
    {
        DoTweenToPos(Left, new Vector2(56f, Left.anchoredPosition.y));
        DoTweenToPos(Right, new Vector2(-103f, Right.anchoredPosition.y));
        DoTweenToPos(SelfIcons, new Vector2(SelfIcons.anchoredPosition.x, 0f));
        DoTweenToPos(SelfDecotate, new Vector2(SelfDecotate.anchoredPosition.x, -600f));
        DoTweenToPos(Top, new Vector2(Top.anchoredPosition.x, 0f));
        await UniTask.Delay((int)(timeLength * 1000));
    }

    //好友庄园打开偷取列表
    public async UniTask PlayAnimFriendStealListToOpen()
    {
        DoTweenToPos(FriendBottom, new Vector2(FriendBottom.anchoredPosition.x, 281.02f));
        await UniTask.Delay((int)(timeLength * 1000));
    }
    //好友庄园关闭偷取列表
    public async UniTask PlayAnimFriendStealListToClose()
    {
        DoTweenToPos(FriendBottom, new Vector2(FriendBottom.anchoredPosition.x, 0f));
        await UniTask.Delay((int)(timeLength * 1000));
    }

    private void DoTweenToPos(RectTransform target,Vector2 endAnchorPos)
    {
        DOTween.To(() => target.anchoredPosition, (pos) => { target.anchoredPosition = pos; }, endAnchorPos, timeLength);
    }
}
