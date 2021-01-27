using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public static class ExtensionMethodHelper
{
    public static void SetTransformDefalut(this Transform trans)
    {
        trans.localPosition = Vector3.zero;
        trans.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        trans.localEulerAngles = Vector3.zero;
        trans.localScale = Vector3.one;
    }
    public static void SetTransformDefalutWithParent(this Transform trans,Transform parentTrans)
    {
        trans.SetParent(parentTrans);
        trans.localPosition = Vector3.zero;
        trans.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        trans.localEulerAngles = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.gameObject.SetActive(true);
    }

    public static void SetTransformDefalutWithParentNotStretch(this Transform trans, Transform parentTrans)
    {
        trans.SetParent(parentTrans);
        trans.localPosition = Vector3.zero;
        trans.localEulerAngles = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.gameObject.SetActive(true);
    }
    public static void SetRectTransformStretchAllWithParent(this Transform trans, Transform parentTrans)
    {
        trans.SetParent(parentTrans);
        RectTransform rectTrans = trans.GetComponent<RectTransform>();
        rectTrans.anchorMin = Vector2.zero;
        rectTrans.anchorMax = Vector2.one;
        rectTrans.pivot=0.5f* Vector2.one;
        rectTrans.offsetMin = Vector2.zero;
        rectTrans.offsetMax = Vector2.zero;
        trans.localEulerAngles = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.gameObject.SetActive(true);
    }
    public static void SetTransformDefalutWithParent(this Transform trans, Transform parentTrans,Vector3 localPos)
    {
        trans.SetParent(parentTrans);
        trans.localPosition = localPos;
        trans.localEulerAngles = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.gameObject.SetActive(true);
    }
    public static void SetTransformZAndScaleWithParent(this Transform trans, Transform parentTrans)
    {
        trans.SetParent(parentTrans);
        trans.localScale = Vector3.one;
        trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y,0);
        trans.gameObject.SetActive(true);
    }
    public static void SetTransformInfoCopyTrans(this Transform trans, Transform copyTrans)
    {
        trans.position = copyTrans.position;
        trans.rotation = copyTrans.rotation;
    }
    public static void DestroyAllChild(this Transform trans)
    {
        if (trans.childCount < 0) return;
        int count = trans.childCount;
        for (int i = count-1; i >=0; i--)
        {
            GameObject.Destroy(trans.GetChild(i).gameObject);
        }
    }
    public static T AddComponentIfNull<T>(this GameObject go) where T:Component
    {
        T component=go.GetComponent<T>();
        if (component==null)
        {
            component = go.AddComponent<T>();
        }
        return component;
    }

    public static T Get<T>(this GameObject gameObject, string key) where T : UnityEngine.Object
    {
        try
        {
            return gameObject.GetComponent<ReferenceCollector>().Get<T>(key);
        }
        catch (Exception e)
        {
            throw new Exception($"获取{gameObject.name}的ReferenceCollector key失败, key: {key}", e);
        }
    }
}
