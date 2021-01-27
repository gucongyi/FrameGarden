using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UIPlantScrollViewAdapter : MonoBehaviour
{
    public const float widthOne = 178f;
    public const float minAllWidth = 445;
    public const float maxAllWidth = 950;
    public RectTransform rectContent;
    RectTransform selfRectTransform;
    const float ySelfSizeDelta= 306f;
    private void Awake()
    {
        selfRectTransform = GetComponent<RectTransform>();
    }

    
    void LateUpdate()
    {
        CalcAdapterSize();
    }
    public void CalcAdapterSizeByCount(int totalCount)
    {
        float xSizeDelta = (totalCount+0.5f)* widthOne;
        if (xSizeDelta > maxAllWidth)
        {
            xSizeDelta = maxAllWidth;
        }
        if (xSizeDelta < minAllWidth)
        {
            xSizeDelta = minAllWidth;
        }
        selfRectTransform.sizeDelta = new Vector2(xSizeDelta, ySelfSizeDelta);
    }
    void CalcAdapterSize()
    {
        float xSizeDelta = rectContent.sizeDelta.x+0.5f* widthOne;
        if (rectContent.childCount <= 2)
        {
            xSizeDelta = rectContent.sizeDelta.x + 0.5f * widthOne;
        }
        if (xSizeDelta > maxAllWidth)
        {
            xSizeDelta = maxAllWidth;
        }
        if (xSizeDelta < minAllWidth)
        {
            xSizeDelta = minAllWidth;
        }
        selfRectTransform.sizeDelta = new Vector2(xSizeDelta, ySelfSizeDelta);
    }
}
