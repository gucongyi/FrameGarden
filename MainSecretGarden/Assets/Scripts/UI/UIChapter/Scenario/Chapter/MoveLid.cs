using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveLid : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    bool isFinished = false;
    RectTransform thisRect;
    float MaxY = 810f;
    float MaxX = 450f;
    float MinY = -690f;//外部传入
    float MinX = -450f;
    Action FinishedAction;//外部传入动画
    Vector2 lastPos;

    public void Init(Action FinishedAnima)
    {
        this.FinishedAction = FinishedAnima;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        thisRect = this.transform as RectTransform;
        lastPos = thisRect.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 APos = thisRect.anchoredPosition;
        APos += eventData.delta;
        float x = Mathf.Clamp(APos.x, MinX, MaxX);
        float y = Mathf.Clamp(APos.y, MinY, MaxY);
        thisRect.anchoredPosition = new Vector2(x, y);//改变位置
        if (y <= MinY || y >= MaxY)
        {
            isFinished = true;
        }
        else
        {
            isFinished = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isFinished)
        {
            thisRect.anchoredPosition = lastPos;//回弹
        }
        if (isFinished)
        {
            this.GetComponent<Image>().raycastTarget = false;//不可再点击
            FinishedAction?.Invoke();
        }
    }
}
