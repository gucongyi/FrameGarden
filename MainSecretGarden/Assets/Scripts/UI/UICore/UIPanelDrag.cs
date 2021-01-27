using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPanelDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public bool isHaveScrollHorizen;
    public Vector2 localPos;
    const float value_MinDrag = 4f;
    const float totalThrehold = 10f;
    bool isDragSelf;
    float yTotalChange;
    float xTotalChange;
    //在这个范围内拖拽
    public RectTransform m_DragPlane;
    private Vector3 temp;
    RectTransform selfRectTransform;
    //自己注册，最多用两个互斥的
    public Action<PointerEventData> actionFromUpDrag;
    public Action<PointerEventData> actionFromBottomDrag;
    public Action<PointerEventData> actionFromLeftDrag;
    public Action<PointerEventData> actionFromRightDrag;
    public Action<PointerEventData> actionOnClick;
    public Action<PointerEventData> actionOnPointerDown;
    public Action<PointerEventData> actionOnPointerUp;
    bool isDragOut = false;

    float timePress;
    LoopScrollRect LoopScrollRect;
    private void Awake()
    {
        timePress = 0f;
        selfRectTransform = transform.GetComponent<RectTransform>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        yTotalChange = 0f;
        xTotalChange = 0f;
        isDragSelf = false;
        isDragOut = false;
        actionOnPointerDown?.Invoke(eventData);
        timePress = Time.realtimeSinceStartup;
        var rt = transform.GetComponent<RectTransform>();
        Vector3 GlobalVariableMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DragPlane, eventData.position, eventData.pressEventCamera, out GlobalVariableMousePos))
        {
            //鼠标按下时候记录下偏移的点
            temp = GlobalVariableMousePos - transform.GetComponent<RectTransform>().position;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        var pressedTime = Time.realtimeSinceStartup - timePress;
        if (pressedTime < 0.2f)
        {
            actionOnClick?.Invoke(eventData);
        }
        else
        {
            actionOnPointerUp?.Invoke(eventData);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        LoopScrollRect = GetComponentInParent<LoopScrollRect>();
        if (LoopScrollRect != null)
        {
            LoopScrollRect.OnBeginDrag(eventData);
        }
        if (!isHaveScrollHorizen)
        {
            LoopScrollRect = null;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (LoopScrollRect != null&&!isDragSelf)
        {
            if (Mathf.Abs(eventData.delta.x) > value_MinDrag&&Mathf.Abs(eventData.delta.x)> Mathf.Abs(eventData.delta.y))
            {
                LoopScrollRect.OnDrag(eventData);
                return;
            }
        }
        if (isDragOut)
        {
            //如果已经拖出了，不再判定
            return;
        }
        yTotalChange += eventData.delta.y;
        xTotalChange += eventData.delta.x;
        if (Mathf.Abs(yTotalChange)>totalThrehold)
        {
            //累计y值变化超过阈值认为是单体滑动
            isDragSelf = true;
        }
        Vector3 GlobalVariableMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DragPlane, eventData.position, eventData.pressEventCamera, out GlobalVariableMousePos))
        {
            selfRectTransform.position = GlobalVariableMousePos - temp;
        }
        if (!RectTransformUtility.RectangleContainsScreenPoint(m_DragPlane, eventData.position, eventData.pressEventCamera))//判定鼠标点是否不在当前Button的框内了
        {//判断第二个参数代表的位置是否在第一个参数rect的范围内
            //StaticData.DebugGreen($"拖出区域外  eventData.position:{eventData.position}");
            if (yTotalChange > totalThrehold)//上边拖出来了面板
            {
                actionFromUpDrag?.Invoke(eventData);
                isDragOut = true;
            }
            else if (yTotalChange < -totalThrehold)//下边拖出来了面板
            {
                actionFromBottomDrag?.Invoke(eventData);
                isDragOut = true;
            }
            else if (!isHaveScrollHorizen&& xTotalChange > totalThrehold)//右边拖出来了面板
            {
                actionFromRightDrag?.Invoke(eventData);
                isDragOut = true;
            }
            else if (!isHaveScrollHorizen && xTotalChange < -totalThrehold)//左边拖出来了面板
            {
                actionFromLeftDrag?.Invoke(eventData);
                isDragOut = true;
            }
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragSelf&&LoopScrollRect != null)
        {
            LoopScrollRect.OnEndDrag(eventData);
        }
        selfRectTransform.anchoredPosition = localPos;
    }
}
