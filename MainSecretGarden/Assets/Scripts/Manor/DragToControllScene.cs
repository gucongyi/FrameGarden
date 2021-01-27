using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragToControllScene : MonoBehaviour, IDragHandler
{
    ManorCameraWorldComponent manorCameraWorldComponent;
    private void Start()
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Root2dSceneManager._instance.isZoom)
        {
            return;
        }
        if (Root2dSceneManager._instance.isTileDrag)
        {
            return;
        }
        //功能是否开启
        if (!StaticData.IsOpenFunction(10009,false))
        {
            return;
        }
        manorCameraWorldComponent = Root2dSceneManager._instance.worldCameraComponent;
        //减是相机要和地块方向反向，从而实现拖拽的功能
        manorCameraWorldComponent.SetDragData(eventData);
    }

    
}