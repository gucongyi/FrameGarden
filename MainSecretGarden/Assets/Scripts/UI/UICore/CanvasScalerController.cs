using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerController : MonoBehaviour {
    
    private void Awake()
    {
        var arrayCanvas=transform.GetComponentsInChildren<Canvas>();
        for (int i = 0; i < arrayCanvas.Length; i++)
        {
            GetCur_scaleFactor(arrayCanvas[i]);
        }
    }

    /// <summary>
    /// 根据标准分辨率比率获取当前的需要使用的比例因子
    /// </summary>
    private void GetCur_scaleFactor(Canvas canvas) {


        float f = (float)Screen.height / (float)Screen.width;

        if (f < 1.775)
        {
            //canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
            ////float standard = 1242f / 2688f;
            //float standard = 0.5625f;
            //float standard_W = Screen.height * standard;
            //standard_W = standard_W / 1242f;
            ////canvas.GetComponent<CanvasScaler>().scaleFactor = standard_W;
            //canvas.scaleFactor = standard_W;
            //Debug.Log("比例因子 scaleFactor = " + standard_W);
            canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
        }
        else if (f < 2688f / 1242f)
        {
            canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
        }
        else//再长屏 1080：2400
        {
            canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
        }
        Debug.Log($"canvas name:{canvas.name} canvas.scaleFactor:{canvas.scaleFactor}");
    }
}
