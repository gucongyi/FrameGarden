using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class ManorCameraWorldComponent : MonoBehaviour
{
    //orthographicSize是相机高度一半的大小
    //高度算法 （放大倍数-1）*orthographicSize*40
    //高度默认一屏，宽度默认2.2屏

    float limitLeftX;
    float limitRightX;
    float limitBottomY;
    float limitTopY;
    const float WidthScreen= 2.2f;

    public const float radioUIToScene = 100f;//UGUI 和 2d Sprite比列 1:100
    public Camera cameraWorld;

    

    //自动滑动
    public const float speedAutoScoll = 0.1f;
    int xDirection = 0;
    int yDircetion = 0;
    bool isAutoScroll;
    public const float SizeScreenToSide = 100f;
    float radioWidthHeight = 1;
    
    public enum EnumDirection
    {
        None,
        Up,
        Right,
        Bottom,
        Left
    }
    public EnumDirection currDirection = EnumDirection.None;
    public List<Vector2> listDirection = new List<Vector2>()
    {
        Vector2.up,
        Vector2.right,
        Vector2.down,
        Vector2.left
    };
    Vector3 preCameraPos;
    private void Awake()
    {
        cameraWorld.GetUniversalAdditionalCameraData().cameraStack.Add(Camera.main);
        isAutoScroll = false;
        radioWidthHeight = (float)Screen.width / Screen.height;
    }

    private void Start()
    {
        
    }

    private static void SetCurrZoom()
    {
        SceneZoom sceneZoom = Root2dSceneManager._instance.transform.GetComponent<SceneZoom>();
        sceneZoom.SetCurrZoomParam(LocalInfoData.localManorInfoData.CameraZoom);
    }

    public void SetWorldCameraParam()
    {
        LocalInfoData.localManorInfoData = JsonHelper.LoadData<LocalManorInfoData>();
        if (LocalInfoData.localManorInfoData != null)
        {
            if (LocalInfoData.localManorInfoData.CameraManorLocalPos != null)
            {
                SetCameraLocalPos(LocalInfoData.localManorInfoData.CameraManorLocalPos);
            }
            cameraWorld.orthographicSize = SceneZoom.GetCameraSizeByZoom(LocalInfoData.localManorInfoData.CameraZoom);
        }
        else
        {
            LocalInfoData.localManorInfoData = new LocalManorInfoData()
            {
                CameraManorLocalPos = cameraWorld.transform.localPosition,
                CameraZoom = 3f,//默认放大1.5倍，最小0.5
                isFirstAvgDialogFinish = false,
                countTotalGain = 0
            };
            JsonHelper.SaveDataToPersist<LocalManorInfoData>(LocalInfoData.localManorInfoData);
        }
        SetCurrZoom();
    }

    public void Limit()
    {
        if (LocalInfoData.localManorInfoData == null)
        {
            return;
        }
        float x = cameraWorld.transform.localPosition.x;
        float y = cameraWorld.transform.localPosition.y;
        float zoomRadio = Root2dSceneManager._instance.zoomRadio;
        bool isLimit = false;
        float orthSize = Root2dSceneManager._instance.worldCameraComponent.cameraWorld.orthographicSize;
        float limitY = (zoomRadio - 1)* orthSize*40;//1倍到6倍
        float limitX = (WidthScreen* zoomRadio -1)* radioWidthHeight* orthSize * 40;
        limitLeftX = -limitX;
        limitRightX= limitX;
        limitBottomY= -limitY;
        limitTopY= limitY;
        if (cameraWorld.transform.localPosition.x <= limitLeftX)
        {
            x = limitLeftX;
            isLimit = true;
        }
        if (cameraWorld.transform.localPosition.x >= limitRightX)
        {
            x = limitRightX;
            isLimit = true;
        }
        if (cameraWorld.transform.localPosition.y <= limitBottomY)
        {
            y = limitBottomY;
            isLimit = true;
        }
        if (cameraWorld.transform.localPosition.y >= limitTopY)
        {
            y = limitTopY;
            isLimit = true;
        }

        cameraWorld.transform.localPosition = new Vector2(x, y);
        if (Mathf.Abs(LocalInfoData.localManorInfoData.CameraManorLocalPos.x-x) > float.Epsilon
            || Mathf.Abs(LocalInfoData.localManorInfoData.CameraManorLocalPos.y - y) > float.Epsilon)
        {
            if (isLimit)
            {
                LocalInfoData.localManorInfoData.CameraManorLocalPos = cameraWorld.transform.localPosition;
                JsonHelper.SaveDataToPersist<LocalManorInfoData>(LocalInfoData.localManorInfoData);
            }
        }
    }
    public void SetCameraLocalPos(Vector2 localPos)
    {
        cameraWorld.transform.localPosition = localPos;
        if (LocalInfoData.localManorInfoData==null)
        {
            LocalInfoData.localManorInfoData = JsonHelper.LoadData<LocalManorInfoData>();
        }
        LocalInfoData.localManorInfoData.CameraManorLocalPos = cameraWorld.transform.localPosition;
        JsonHelper.SaveDataToPersist<LocalManorInfoData>(LocalInfoData.localManorInfoData);
    }
    public void SetDragData(PointerEventData eventData)
    {
        float zoomRadio = Root2dSceneManager._instance.zoomRadio;
        var newPos = new Vector2(cameraWorld.transform.localPosition.x - eventData.delta.x / radioUIToScene/ zoomRadio,
            cameraWorld.transform.localPosition.y - eventData.delta.y / radioUIToScene/ zoomRadio);
        SetCameraLocalPos(newPos);
    }

    public void TriggerAutoScroll(EnumDirection direction)
    {
        if (currDirection== direction)
        {
            return;
        }
        isAutoScroll = true;
        switch (direction)
        {
            case EnumDirection.Up:
                yDircetion = 1;
                break;
            case EnumDirection.Right:
                xDirection = 1;
                break;
            case EnumDirection.Bottom:
                yDircetion = -1;
                break;
            case EnumDirection.Left:
                xDirection = -1;
                break;
        }
        currDirection = direction;
    }
    public void EndAutoScroll()
    {
        xDirection = 0;
        yDircetion = 0;
        isAutoScroll=false;
        currDirection = EnumDirection.None;
    }
    private void Update()
    {
        if (isAutoScroll)
        {
            var newPos = cameraWorld.transform.localPosition + new Vector3(xDirection, yDircetion, 0) * speedAutoScoll;
            SetCameraLocalPos(newPos);
        }
        //限制相机位置
        Limit();
    }

    public void SetPreCameraPos()
    {
        preCameraPos = cameraWorld.transform.localPosition;
    }

    public void PlayRebackCameraPos()
    {
        DOTween.To(() => cameraWorld.transform.localPosition, (pos) => SetCameraLocalPos(pos), preCameraPos, 1f);
    }

    public void PlayCameraToUnLockRegion(Vector3 willPos)
    {
        DOTween.To(() => cameraWorld.transform.localPosition, (pos) => SetCameraLocalPos(pos), willPos, 1f);
        //DOTween.To(() => cameraWorld.orthographicSize, (size) => { cameraWorld.orthographicSize = size; }, 0.5f, 1f);
        ////0.5f是放大
    }

    public void SetCameraToUnLockWorkShedRegion(Vector3 willPos)
    {
        SetCameraLocalPos(willPos);
    }
    public void SetCameraToTilePos(Vector3 willLocalPos)
    {
        SetCameraLocalPos(willLocalPos);
    }
    //public void PlayCameraToUnLockRegionOld()
    //{
    //    DOTween.To(() => cameraWorld.orthographicSize, (size) => { cameraWorld.orthographicSize = size; }, 1.08f, 1f);
    //}

    public void PlayUIExceedReback(Vector3 deltaVecScreen, GameObject goUI, TileComponent tileComponent, UIWorldHandleManager.TypePointUI typePointUI)
    {
        float zoomRadio = Root2dSceneManager._instance.zoomRadio;
        deltaVecScreen = deltaVecScreen / radioUIToScene/ zoomRadio*1.2f;
        DOTween.To(() => cameraWorld.transform.localPosition, (pos) => 
        {
            SetCameraLocalPos(pos);
            StaticData.GetUIWorldHandleComponent().SetWorldPos(tileComponent,goUI, typePointUI);
        }, cameraWorld.transform.localPosition+ deltaVecScreen, 1f);
        
    }

}
