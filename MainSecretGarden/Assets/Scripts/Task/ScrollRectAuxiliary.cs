using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 滑动栏辅助工具
/// </summary>
public class ScrollRectAuxiliary : ScrollRect
{
    #region 字段
    /// <summary>
    /// 结束滑动
    /// </summary>
    public Action _endDragAction;
    /// <summary>
    /// 正在拖拽
    /// </summary>
    public Action _onDragAction;
    /// <summary>
    /// 开始滑动
    /// </summary>
    public Action _startDragAction;
    /// <summary>
    ///自动定位开始回调
    /// </summary>
    public Action _startMoveLocationAction;
    /// <summary>
    /// 自动定位结束回调
    /// </summary>
    public Action _endMoveLocationAction;
    /// <summary>
    /// 是否自动定位中心点
    /// </summary>
    public bool _isVoluntarilyLocation = false;
    /// <summary>
    /// 定位速度
    /// </summary>
    public float _locationMaxSpeed = 0.01f;
    #endregion
    #region 函数
    /// <summary>
    /// 正在拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        _onDragAction?.Invoke();
    }
    /// <summary>
    /// 结束拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (_isVoluntarilyLocation)
        {
            int index = 0;
            CurrBeCentralItem(out index);
            LocationItem(index);
        }
        _endDragAction?.Invoke();
    }
    /// <summary>
    /// 开始拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        _startDragAction?.Invoke();
    }

    /// <summary>
    /// 定位到item位置
    /// </summary>
    /// <param name="index"></param>
    public async void LocationItem(int index)
    {
        horizontal = false;
        _startMoveLocationAction?.Invoke();
        //获取移动距离
        float distance = GetMoveDistance(index);
        //移动比列
        float movePercentage = (distance / content.sizeDelta.x);
        while (horizontalScrollbar.value != movePercentage)
        {
            horizontalScrollbar.value = Mathf.MoveTowards(horizontalScrollbar.value, movePercentage, _locationMaxSpeed);
            await UniTask.Delay(TimeSpan.FromMilliseconds(10));
        }
        horizontal = true;
        _endMoveLocationAction?.Invoke();
    }
    /// <summary>
    /// 获取移动距离
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public float GetMoveDistance(int index)
    {
        float distance = 0;
        HorizontalLayoutGroup horizontalLayoutGroup = content.GetComponent<HorizontalLayoutGroup>();
        float spacing = 0;

        for (int i = 0; i < index; i++)
        {
            RectTransform itemRect = content.GetChild(i).GetComponent<RectTransform>();

            distance = distance + itemRect.rect.width;
            if (horizontalLayoutGroup != null)
            {
                spacing = spacing + horizontalLayoutGroup.spacing;
            }

        }
        distance = distance + spacing;

        return distance;

    }
    /// <summary>
    /// 消除锚点影响获取对象的世界坐标
    /// </summary>
    /// <param name="rec"></param>
    /// <returns></returns>
    private Vector3 ClearPivotOffset(RectTransform rec)
    {
        var offset = new Vector3((0.5f - rec.pivot.x) * rec.rect.width, (0.5f - rec.pivot.y) * rec.rect.height, 0.0f);
        var newPosition = rec.localPosition + offset;
        return rec.parent.TransformPoint(newPosition);
    }
    /// <summary>
    /// 获取当前最接近中心点的itme
    /// </summary>
    /// <returns></returns>
    public RectTransform CurrBeCentralItem(out int index)
    {
        RectTransform tageItem = null;

        Vector3 centralPoint = content.InverseTransformPoint(ClearPivotOffset(viewport));
        float distance = -1;
        index = 0;
        for (int i = 0; i < content.childCount; i++)
        {
            RectTransform itemTra = content.GetChild(i).GetComponent<RectTransform>();
            Vector3 itemPoint = content.InverseTransformPoint(ClearPivotOffset(itemTra));
            float currDistance = Vector3.Distance(centralPoint, itemPoint);
            if (distance == -1)
            {
                distance = currDistance;
                tageItem = itemTra;
                index = i;
            }
            else if (currDistance < distance)
            {
                distance = currDistance;
                tageItem = itemTra;
                index = i;
            }
        }
        return tageItem;
    }
    #endregion
}
