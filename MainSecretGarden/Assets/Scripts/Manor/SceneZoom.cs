using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SceneZoom : MonoBehaviour
{
    Camera cameraWorld;
    //缩放
    private Vector2 cachedPositionFinger0;//缓存手指0位置
    private Vector2 cachedPositionFinger1;//缓存手指1位置
    private Vector2 BeginPosFinger0;//缓存手指0位置
    private Vector2 BeginPosFinger1;//缓存手指1位置
    bool isScaleBegin = false;
    private float zoomParam = 1.5f;//默认放大1.5倍
    private float zoomSpeed = 0.1f;//缩放速度
    float currCameraSize=0f;

    const float maxCameraSizeRadio = 6;
    public const float minCameraSize = 0.40f;
    const float realMaxCameraSize = minCameraSize / maxCameraSizeRadio;
    const float maxCameraSize = minCameraSize / maxCameraSizeRadio/1.2f;
    private const float zoomMoveDistanceThresholdBegin = 4f;
    void Start()
    {
        cameraWorld = Root2dSceneManager._instance.worldCameraComponent.cameraWorld;
    }
    private int IsLagerByDistance(Vector2 positionFinger0, Vector2 positionFinger1)
    {
        int result;
        float disBegin = Vector2.Distance(BeginPosFinger0, BeginPosFinger1);
        float disPre = Vector2.Distance(cachedPositionFinger0, cachedPositionFinger1);
        float disCurr = Vector2.Distance(positionFinger0, positionFinger1);
        float delta = disCurr - disPre;
        Debug.LogWarning($"disBegin:{disBegin} disPre:{disPre} disCurr:{disCurr} distance:{delta}");

        if (Mathf.Abs(disCurr - disBegin) > zoomMoveDistanceThresholdBegin)
        {
            isScaleBegin = true;
        }

        if (isScaleBegin)
        {
            if (delta > 0)//放
            {
                result = 1;
            }
            else//缩
            {
                result = 2;
            }
        }
        else
        {
            result = 0;
        }
        cachedPositionFinger0 = positionFinger0;
        cachedPositionFinger1 = positionFinger1;
        return result;
    }

    public void SetCurrZoomParam(float zoomParam)
    {
        this.zoomParam = zoomParam;
        if (this.zoomParam < 1)
        {
            this.zoomParam = 1;//不能比原尺寸小
        }
        if (this.zoomParam > 1*maxCameraSizeRadio)
        {
            this.zoomParam = 1 * maxCameraSizeRadio;
        }
    }

    private void UpdateScale()
    {
        if (Root2dSceneManager._instance.isTileDrag)
        {//如果正在操作一个地块，不能进行缩放操作
            if (Input.touchCount > 1)
            {//Input.multiTouchEnabled在Android设备上一直为true
                return;
            }  
        }
        TestScaleOnEditor();

        if (Input.touchCount > 1)
        {
            var positionFinger0 = Input.GetTouch(0).position;
            var positionFinger1 = Input.GetTouch(1).position;
            if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Began)
            {
                cachedPositionFinger0 = positionFinger0;
                cachedPositionFinger1 = positionFinger1;
                BeginPosFinger0 = positionFinger0;
                BeginPosFinger1 = positionFinger1;
                AllowDrag(false);
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(1).phase == TouchPhase.Ended)
            {
                isScaleBegin = false;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                AllowDrag(false);
                var result = IsLagerByDistance(positionFinger0, positionFinger1);
                if (result == 0)
                {
                    return;
                }
                else if (result == 1)
                {
                    Zoom(true);
                }
                else if (result == 2)
                {
                    Zoom(false);
                }
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            //播放最大到真正最大的回弹效果
            PlaySpringFromMaxToRealMax();
        }

        if (Input.touchCount == 0)
        {
            AllowDrag(true);
        }
    }

    void PlaySpringFromMaxToRealMax()
    {
        if (currCameraSize>=maxCameraSize&& currCameraSize<=realMaxCameraSize)
        {
            DOTween.To(()=>currCameraSize, (size) => 
            {
                currCameraSize = size;
                zoomParam= minCameraSize/currCameraSize;
                SetSceneZoom(size);
            }, realMaxCameraSize, 0.5f);
        }
        
    }

    private void Zoom(bool isZoomOut)
    {
        //功能是否开启
        if (!StaticData.IsOpenFunction(10009, false))
        {
            return;
        }
        
        bool isDecorateRotating=StaticData.GetUIWorldHandleComponent().isRotatingDecorate;
        if (isDecorateRotating)
        {//装饰物正在旋转 不能缩放
            return;
        }
        if (isZoomOut)
        {
            zoomParam = (zoomParam + zoomSpeed);//放大size变小
        }
        else
        {
            zoomParam = zoomParam - zoomSpeed;
        }
        LocalInfoData.localManorInfoData.CameraZoom = zoomParam;
        JsonHelper.SaveDataToPersist<LocalManorInfoData>(LocalInfoData.localManorInfoData);
        currCameraSize = GetCameraSizeByZoom(zoomParam);
        SetSceneZoom(currCameraSize);
    }

    public static float GetCameraSizeByZoom(float zoomParam)
    {
        float currCameraSize=1f;
        if (zoomParam < 1)
        {
            zoomParam = 1;//不能比原尺寸小
        }
        if (zoomParam > 1 * maxCameraSizeRadio * 1.2f)//最大三倍/1.2f 用来回弹
        {
            zoomParam = maxCameraSizeRadio * 1.2f;
        }
        currCameraSize = minCameraSize/zoomParam;
        if (currCameraSize < maxCameraSize)
        {
            currCameraSize = maxCameraSize;
        }
        if (currCameraSize > minCameraSize)
        {
            currCameraSize = minCameraSize;
        }
        Root2dSceneManager._instance.zoomRadio = zoomParam;
        return currCameraSize;
    }

    private void SetSceneZoom(float cameraSize)
    {
        cameraWorld.orthographicSize = cameraSize;
    }
    

    private void AllowDrag(bool allow)
    {
        Root2dSceneManager._instance.isZoom = !allow;
    }
    private void Update()
    {
        UpdateScale();
    }

    private void TestScaleOnEditor()
    {
        if (Root2dSceneManager._instance.isTileDrag)
        {//如果正在操作一个地块，不能进行缩放操作
            return;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Zoom(true);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Zoom(false);
        }
    }
    
}