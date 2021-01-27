using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollCameraWhenBoundsOutScreen : MonoBehaviour
{
    ManorCameraWorldComponent manorCameraWorldComponent;
    Canvas canvas;
    RectTransform rt;

    List<Vector2> listFourPoint = new List<Vector2>();
    List<Vector2> GetFourScreenPoint()
    {
        listFourPoint.Clear();
        var centerPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        for (int i = 0; i < 4; i++)
        {
            Vector2 point = new Vector2();
            listFourPoint.Add(point);
        }
        //左下
        listFourPoint[0] = centerPoint + new Vector2(-rt.sizeDelta.x, -rt.sizeDelta.y)/2f;
        //右下
        listFourPoint[1] = centerPoint + new Vector2(rt.sizeDelta.x, -rt.sizeDelta.y) / 2f;
        //右上
        listFourPoint[2] = centerPoint + new Vector2(rt.sizeDelta.x, rt.sizeDelta.y) / 2f;
        //左上
        listFourPoint[3] = centerPoint + new Vector2(-rt.sizeDelta.x, rt.sizeDelta.y) / 2f;
        return listFourPoint;
    }

    public void PlayCameraAnimWhenOutScreen(GameObject goUI,TileComponent tileComponent, UIWorldHandleManager.TypePointUI typePointUI)
    {
        //这里初始化
        manorCameraWorldComponent = Root2dSceneManager._instance.worldCameraComponent;
        rt = GetComponent<RectTransform>();


        var listFourPoint=GetFourScreenPoint();
        Vector2 vecDelta=Vector2.zero;
        //左上角
        var PointLeftTop=listFourPoint.Find((point)=>point.x<=0&&point.y>=Screen.height);
        //左下角
        var PointLeftBottom = listFourPoint.Find((point) => point.x <= 0 && point.y <=0);
        //右上角
        var PointRightTop = listFourPoint.Find((point) => point.x >= Screen.width && point.y >= Screen.height);
        //右下角
        var PointRightBottom = listFourPoint.Find((point) => point.x >= Screen.width && point.y <=0);
        //上
        var PointTop = listFourPoint.Find((point) => point.y >= Screen.height);
        //右
        var PointRight = listFourPoint.Find((point) => point.x >= Screen.width);
        //下
        var PointBottom = listFourPoint.Find((point) => point.y <=0);
        //左
        var PointLeft = listFourPoint.Find((point) => point.x <=0);

        if (PointLeftTop != Vector2.zero)
        {
            //(负，正)
            vecDelta = listFourPoint[3] - new Vector2(0, Screen.height);
        }
        else if (PointLeftBottom != Vector2.zero)
        {
            //(负，负)
            vecDelta = listFourPoint[0] - new Vector2(0, 0);
        }
        else if (PointRightTop != Vector2.zero)
        {
            //(正，正)
            vecDelta = listFourPoint[2] - new Vector2(Screen.width, Screen.height);
        }
        else if (PointRightBottom != Vector2.zero)
        {
            //(正，负)
            vecDelta = listFourPoint[1] - new Vector2(Screen.width, 0);
        }
        else if (PointTop != Vector2.zero)
        {
            vecDelta = new Vector2(0, listFourPoint[2].y- Screen.height);
        }
        else if (PointRight != Vector2.zero)
        {
            vecDelta = new Vector2(listFourPoint[2].x- Screen.width, 0);
        }
        else if (PointBottom != Vector2.zero)
        {
            vecDelta = new Vector2(0,listFourPoint[0].y);
        }
        else if (PointLeft != Vector2.zero)
        {
            vecDelta = new Vector2(listFourPoint[0].x,0);
        }

        manorCameraWorldComponent.PlayUIExceedReback(new Vector3(vecDelta.x, vecDelta.y),goUI,tileComponent, typePointUI);

    }
    

}
